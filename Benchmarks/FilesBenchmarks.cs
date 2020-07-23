using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Xamarin.Android.Tools;

namespace Benchmarks
{
	[Orderer (SummaryOrderPolicy.FastestToSlowest)]
	[MemoryDiagnoser]
	public class FilesBenchmarks
	{
		const int size = 10 * 1024 * 1024;
		readonly MemoryStream stream = new MemoryStream (size);
		readonly string temp = Path.GetTempFileName ();
		readonly Random random = new Random ();

		[GlobalSetup]
		public void GlobalSetup ()
		{
			var bytes = new byte [10 * 1024 * 1024];
			stream.Write (bytes, 0, bytes.Length);
			File.WriteAllBytes (temp, bytes);
		}

		[GlobalCleanup]
		public void GlobalCleanup ()
		{
			File.Delete (temp);
		}

		[IterationSetup]
		public void IterationSetup ()
		{
			stream.Position = 0;
			stream.WriteByte ((byte)random.Next (byte.MaxValue));
		}

		[Benchmark]
		public void CopyIfChanged1 ()
		{
			Files.CopyIfStreamChanged (stream, temp);
		}

		[Benchmark]
		public void CopyIfChanged2 ()
		{
			Files2.CopyIfStreamChanged (stream, temp);
		}
	}
}
