using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using System;
using System.IO;

namespace Benchmarks
{
	[Orderer (SummaryOrderPolicy.FastestToSlowest)]
	[MemoryDiagnoser]
	public class FilesBenchmarks
	{
		readonly string tempDir = Path.Combine (Path.GetTempPath (), Path.GetRandomFileName ());
		readonly string tempFile;
		readonly string tempFileName;

		public FilesBenchmarks ()
		{
			Directory.CreateDirectory (tempDir);
			tempFileName = Path.GetRandomFileName () + ".dll";
			tempFile = Path.Combine (tempDir, tempFileName);
			File.WriteAllText (tempFile, contents: "");
		}

		[Benchmark]
		public void Sample1 ()
		{
			DirectoryGetFile1 (tempDir, tempFileName);
		}

		static string DirectoryGetFile1 (string directory, string file)
		{
			if (!Directory.Exists (directory))
				return "";

			var files = Directory.GetFiles (directory, file);
			if (files != null && files.Length > 0)
				return files [0];

			return "";
		}

		[Benchmark]
		public void Sample2 ()
		{
			DirectoryGetFile2 (tempDir, tempFileName);
		}

		static string DirectoryGetFile2 (string directory, string file)
		{
			var path = Path.Combine (directory, file);
			if (File.Exists (path))
				return path;

			return "";
		}
	}
}
