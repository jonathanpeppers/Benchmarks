using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using System;
using System.IO;
using Xamarin.Android.Tools;

namespace Benchmarks
{
	[Orderer (SummaryOrderPolicy.FastestToSlowest)]
	[MemoryDiagnoser]
	public class FilesBenchmarks
	{
		readonly Guid seed = Guid.NewGuid ();
		readonly MemoryStream stream = new MemoryStream ();
		readonly string tempDir = Path.Combine (Path.GetTempPath (), Path.GetRandomFileName ());
		readonly string tempFile;

		public FilesBenchmarks ()
		{
			Directory.CreateDirectory (tempDir);
			tempFile = Path.Combine (tempDir, Path.GetRandomFileName ());

			var bytes = seed.ToByteArray ();
			stream.Write (bytes, 0, bytes.Length);

			stream.Position = 0;
			using (var file = File.Create (tempFile)) {
				stream.CopyTo (file);
			}
		}

		[Benchmark]
		public void CopyIfStreamChanged1 ()
		{
			Files.CopyIfStreamChanged (stream, tempFile);
		}

		[Benchmark]
		public void CopyIfStreamChanged2 ()
		{
			Files2.CopyIfStreamChanged (stream, tempFile);
		}

		[Benchmark]
		public void CopyIfStreamChanged3 ()
		{
			Files3.CopyIfStreamChanged (stream, tempFile);
		}

		[Benchmark]
		public void NoExist1 ()
		{
			Files.CopyIfStreamChanged (stream, Path.Combine (tempDir, Path.GetRandomFileName ()));
		}

		[Benchmark]
		public void NoExist2 ()
		{
			Files2.CopyIfStreamChanged (stream, Path.Combine (tempDir, Path.GetRandomFileName ()));
		}

		[Benchmark]
		public void NoExist3 ()
		{
			Files3.CopyIfStreamChanged (stream, Path.Combine (tempDir, Path.GetRandomFileName ()));
		}

		[GlobalCleanup]
		public void GlobalCleanup ()
		{
			Directory.Delete (tempDir, recursive: true);
		}
	}
}
