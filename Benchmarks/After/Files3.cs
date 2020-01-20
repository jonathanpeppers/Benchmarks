using Java.Interop.Tools.JavaCallableWrappers;
using System;
using System.Buffers;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Xamarin.Android.Tasks;

namespace Xamarin.Android.Tools
{

	static class Files3 {

		/// <summary>
		/// Windows has a MAX_PATH limit of 260 characters
		/// See: https://docs.microsoft.com/en-us/windows/win32/fileio/naming-a-file#maximum-path-length-limitation
		/// </summary>
		public const int MaxPath = 260;

		/// <summary>
		/// On Windows, we can opt into a long path with this prefix
		/// </summary>
		public const string LongPathPrefix = @"\\?\";

		/// <summary>
		/// Converts a full path to a \\?\ prefixed path that works on all Windows machines when over 260 characters
		/// NOTE: requires a *full path*, use sparingly
		/// </summary>
		public static string ToLongPath (string fullPath)
		{
			// On non-Windows platforms, return the path unchanged
			if (Path.DirectorySeparatorChar != '\\') {
				return fullPath;
			}
			return LongPathPrefix + fullPath;
		}

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
			if (HasStreamChanged (stream, destination, out bool destinationExists)) {
				if (destinationExists) {
					var attributes = File.GetAttributes (destination);
					if ((attributes & FileAttributes.ReadOnly) != 0)
						File.SetAttributes (destination, attributes & ~FileAttributes.ReadOnly);
					File.Delete (destination);
				} else {
					var directory = Path.GetDirectoryName (destination);
					if (!string.IsNullOrEmpty (directory))
						Directory.CreateDirectory (directory);
				}
				using (var fileStream = File.Create (destination)) {
					var buffer = ArrayPool<byte>.Shared.Rent (80 * 1024);
					try {
						stream.Position = 0; //HasStreamChanged read to the end
						int length = 0;
						while ((length = stream.Read (buffer, 0, buffer.Length)) > 0) {
							fileStream.Write (buffer, 0, length);
						}
					} finally {
						ArrayPool<byte>.Shared.Return (buffer);
					}
				}
				return true;
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

		public static bool HasStreamChanged (Stream source, string destination) =>
			HasStreamChanged (source, destination, out _);

		public static bool HasStreamChanged (Stream source, string destination, out bool destinationExists)
		{
			//If destination is missing, that's definitely a change
			if (!File.Exists (destination)) {
				destinationExists = false;
				return true;
			}

			destinationExists = true;
			var src_hash = HashStream (source);
			var dst_hash = HashFile (destination);

			// If the hashes don't match, then the file has changed
			if (src_hash != dst_hash)
				return true;

			return false;
		}

		public static bool HasBytesChanged(byte[] bytes, string destination)
		{
			//If destination is missing, that's definitely a change
			if (!File.Exists(destination))
				return true;

			var src_hash = HashBytes(bytes);
			var dst_hash = HashFile(destination);

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
			int length = hash.Length * 2;
			var array = ArrayPool<char>.Shared.Rent (length);
			try {
				for (int i = 0, j = 0; i < hash.Length; i += 1, j += 2) {
					byte b = hash [i];
					array [j] = GetHexValue (b / 16);
					array [j + 1] = GetHexValue (b % 16);
				}
				return new string (array, 0, length);
			} finally {
				ArrayPool<char>.Shared.Return (array);
			}
		}

		static char GetHexValue (int i) => (char) (i < 10 ? i + 48 : i - 10 + 65);

		public static void DeleteFile (string filename, object log)
		{
			try {
				File.Delete (filename);
			} catch (Exception ex) {
#if MSBUILD
				var helper = log as TaskLoggingHelper;
				helper.LogErrorFromException (ex);
#else
				Console.Error.WriteLine (ex.ToString ());
#endif
			}
		}

		const uint ppdb_signature = 0x424a5342;

		public static bool IsPortablePdb (string filename)
		{
			try {
				using (var fs = new FileStream (filename, FileMode.Open, FileAccess.Read)) {
					using (var br = new BinaryReader (fs)) {
						return br.ReadUInt32 () == ppdb_signature;
					}
				}
			}
			catch {
				return false;
			}
		}
	}
}

