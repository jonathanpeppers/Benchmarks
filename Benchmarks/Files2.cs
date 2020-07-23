using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Security.Cryptography;
using System.Text;
using Java.Interop.Tools.JavaCallableWrappers;
using Xamarin.Android.Tasks;

namespace Xamarin.Android.Tools
{

	static class Files2 {

		public static bool CopyIfChanged (string source, string destination)
		{
			if (HasFileChanged (source, destination)) {
				var directory = Path.GetDirectoryName (destination);
				if (!string.IsNullOrEmpty (directory))
					Directory.CreateDirectory (directory);

				if (!Directory.Exists (source)) {
					MonoAndroidHelper.SetWriteable (destination);
					File.Delete (destination);
					File.Copy (source, destination);
					MonoAndroidHelper.SetWriteable (destination);
					File.SetLastWriteTimeUtc (destination, DateTime.UtcNow);
					return true;
				}
			}

			return false;
		}

		public static bool CopyIfStringChanged (string contents, string destination)
		{
			//NOTE: this is not optimal since it allocates a byte[]. We can improve this down the road with Span<T> or System.Buffers.
			var bytes = Encoding.UTF8.GetBytes (contents);
			return CopyIfBytesChanged (bytes, destination);
		}

		public static bool CopyIfBytesChanged (byte[] bytes, string destination)
		{
			if (HasBytesChanged (bytes, destination)) {
				var directory = Path.GetDirectoryName (destination);
				if (!string.IsNullOrEmpty (directory))
					Directory.CreateDirectory (directory);

				MonoAndroidHelper.SetWriteable (destination);
				File.Delete (destination);
				File.WriteAllBytes (destination, bytes);
				return true;
			}
			return false;
		}

		public static bool CopyIfStreamChanged (Stream stream, string destination)
		{
			var directory = Path.GetDirectoryName (destination);
			if (!string.IsNullOrEmpty (directory))
				Directory.CreateDirectory (directory);
			MonoAndroidHelper.SetWriteable (destination);

			using (var mappedFile = MemoryMappedFile.CreateFromFile (destination, FileMode.Create, Path.GetFileNameWithoutExtension (destination), stream.Length))
			using (var mappedStream = mappedFile.CreateViewStream ()) {
				if (HashStream (stream) != HashStream (mappedStream)) {
					// HashStream read to the end
					stream.Position = mappedStream.Position = 0;
					stream.CopyTo (mappedStream);
					return true;
				}
			}
			return false;
		}

		// This is for if the file contents have changed.  Often we have to
		// regenerate a file, but we don't want to update it if hasn't changed
		// so that incremental build is as efficient as possible
		public static bool HasFileChanged (string source, string destination)
		{
			// If either are missing, that's definitely a change
			if (!File.Exists (source) || !File.Exists (destination))
				return true;

			var src_hash = HashFile (source);
			var dst_hash = HashFile (destination);

			// If the hashes don't match, then the file has changed
			if (src_hash != dst_hash)
				return true;

			return false;
		}

		public static bool HasStreamChanged (Stream source, string destination)
		{
			//If destination is missing, that's definitely a change
			if (!File.Exists (destination))
				return true;

			var src_hash = HashStream (source);
			var dst_hash = HashFile (destination);

			// If the hashes don't match, then the file has changed
			if (src_hash != dst_hash)
				return true;

			return false;
		}

		public static bool HasBytesChanged (byte [] bytes, string destination)
		{
			//If destination is missing, that's definitely a change
			if (!File.Exists (destination))
				return true;

			var src_hash = HashBytes (bytes);
			var dst_hash = HashFile (destination);

			// If the hashes don't match, then the file has changed
			if (src_hash != dst_hash)
				return true;

			return false;
		}

		public static string HashString (string s)
		{
			var bytes = Encoding.UTF8.GetBytes (s);
			return HashBytes (bytes);
		}

		public static string HashBytes (byte [] bytes)
		{
			using (HashAlgorithm hashAlg = new Crc64 ()) {
				byte [] hash = hashAlg.ComputeHash (bytes);
				return ToHexString (hash);
			}
		}

		public static string HashFile (string filename)
		{
			using (HashAlgorithm hashAlg = new Crc64 ()) {
				return HashFile (filename, hashAlg);
			}
		}

		public static string HashFile (string filename, HashAlgorithm hashAlg)
		{
			using (Stream file = new FileStream (filename, FileMode.Open, FileAccess.Read)) {
				byte[] hash = hashAlg.ComputeHash (file);
				return ToHexString (hash);
			}
		}

		public static string HashStream (Stream stream)
		{
			stream.Position = 0;

			using (HashAlgorithm hashAlg = new Crc64 ()) {
				byte[] hash = hashAlg.ComputeHash (stream);
				return ToHexString (hash);
			}
		}

		public static string ToHexString (byte[] hash)
		{
			char [] array = new char [hash.Length * 2];
			for (int i = 0, j = 0; i < hash.Length; i += 1, j += 2) {
				byte b = hash [i];
				array [j] = GetHexValue (b / 16);
				array [j + 1] = GetHexValue (b % 16);
			}
			return new string (array);
		}

		static char GetHexValue (int i) => (char) (i < 10 ? i + 48 : i - 10 + 65);
	}
}

