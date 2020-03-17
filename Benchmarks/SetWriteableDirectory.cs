using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using System;
using System.IO;

namespace Benchmarks
{
	[Orderer (SummaryOrderPolicy.FastestToSlowest)]
	[MemoryDiagnoser]
	public class SetWriteableDirectory
	{
		readonly string noExistDir = Path.Combine (Path.GetTempPath (), Path.GetRandomFileName ());
		readonly string readonlyDir = Path.Combine (Path.GetTempPath (), Path.GetRandomFileName ());
		readonly string writeableDir = Path.Combine (Path.GetTempPath (), Path.GetRandomFileName ());

		[GlobalSetup]
		public void GlobalSetup ()
		{
			Directory.CreateDirectory (readonlyDir);
			Directory.CreateDirectory (writeableDir);

			for (int i = 0; i < 10; i++) {
				File.WriteAllText (Path.Combine (readonlyDir, Path.GetRandomFileName ()), contents: "");
				File.WriteAllText (Path.Combine (writeableDir, Path.GetRandomFileName ()), contents: "");
			}

			var dir = new DirectoryInfo (readonlyDir);
			dir.Attributes &= FileAttributes.ReadOnly;
		}

		[IterationSetup]
		public void Setup ()
		{
			var dir = new DirectoryInfo (readonlyDir);
			dir.Attributes &= FileAttributes.ReadOnly;
		}

		[GlobalCleanup]
		public void GlobalCleanup ()
		{
			Directory.Delete (readonlyDir, recursive: true);
			Directory.Delete (writeableDir, recursive: true);
		}

		public static void SetWriteable (string source)
		{
			if (!File.Exists (source))
				return;

			var fileInfo = new FileInfo (source);
			if (fileInfo.IsReadOnly)
				fileInfo.IsReadOnly = false;
		}

		public static void SetDirectoryWriteable1 (string directory)
		{
			if (!Directory.Exists (directory))
				return;

			var dirInfo = new DirectoryInfo (directory);
			if ((dirInfo.Attributes | FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
				dirInfo.Attributes &= ~FileAttributes.ReadOnly;

			foreach (var dir in Directory.EnumerateDirectories (directory, "*", SearchOption.AllDirectories)) {
				dirInfo = new DirectoryInfo (dir);
				if ((dirInfo.Attributes | FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
					dirInfo.Attributes &= ~FileAttributes.ReadOnly;
			}

			foreach (var file in Directory.EnumerateFiles (directory, "*", SearchOption.AllDirectories)) {
				SetWriteable (Path.GetFullPath (file));
			}
		}

		public static void SetDirectoryWriteable2 (string directory)
		{
			var root = new DirectoryInfo (directory);
			if (!root.Exists)
				return;
			SetWriteable (root);

			foreach (FileSystemInfo child in root.EnumerateFileSystemInfos ("*", SearchOption.AllDirectories)) {
				SetWriteable (child);
			}
		}

		static void SetWriteable (FileSystemInfo info)
		{
			var fromAttr = info.Attributes;
			var toAttr = fromAttr & ~FileAttributes.ReadOnly;
			if (fromAttr != toAttr)
				info.Attributes = toAttr;
		}

		[Benchmark]
		public void Writeable1 ()
		{
			SetDirectoryWriteable1 (writeableDir);
		}

		[Benchmark]
		public void Writeable2 ()
		{
			SetDirectoryWriteable2 (writeableDir);
		}

		[Benchmark]
		public void Readonly1 ()
		{
			SetDirectoryWriteable1 (readonlyDir);
		}

		[Benchmark]
		public void Readonly2 ()
		{
			SetDirectoryWriteable2 (readonlyDir);
		}

		//[Benchmark]
		public void NoExist1 ()
		{
			SetDirectoryWriteable1 (noExistDir);
		}

		//[Benchmark]
		public void NoExist2 ()
		{
			SetDirectoryWriteable2 (noExistDir);
		}
	}
}
