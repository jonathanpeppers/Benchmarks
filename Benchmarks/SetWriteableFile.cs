using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using System;
using System.IO;

namespace Benchmarks
{
	[Orderer (SummaryOrderPolicy.FastestToSlowest)]
	[MemoryDiagnoser]
	public class SetWriteableFile
	{
		readonly string noExistFile = Path.Combine (Path.GetTempPath (), Path.GetRandomFileName ());
		readonly string readonlyFile = Path.GetTempFileName ();
		readonly string writeableFile = Path.GetTempFileName ();

		[IterationSetup]
		public void Setup()
		{
			File.SetAttributes (readonlyFile, FileAttributes.ReadOnly);
		}

		[GlobalCleanup]
		public void GlobalCleanup()
		{
			File.SetAttributes (readonlyFile, FileAttributes.Normal);
			File.Delete (readonlyFile);
			File.Delete (writeableFile);
		}

		static void SetWriteable1(string source)
		{
			if (!File.Exists (source))
				return;

			var fileInfo = new FileInfo (source);
			if (fileInfo.IsReadOnly)
				fileInfo.IsReadOnly = false;
		}

		static void SetWriteable2 (string source)
		{
			var fromAttr = File.GetAttributes (source);
			var toAttr = fromAttr & ~FileAttributes.ReadOnly;
			if (fromAttr != toAttr) {
				File.SetAttributes (source, toAttr);
			}
		}

		[Benchmark]
		public void Writeable1 ()
		{
			SetWriteable1 (writeableFile);
		}

		[Benchmark]
		public void Writeable2 ()
		{
			SetWriteable2 (writeableFile);
		}

		[Benchmark]
		public void Readonly1 ()
		{
			SetWriteable1 (readonlyFile);
		}

		[Benchmark]
		public void Readonly2 ()
		{
			SetWriteable2 (readonlyFile);
		}

		//[Benchmark]
		public void NoExist1 ()
		{
			SetWriteable1 (noExistFile);
		}

		//[Benchmark]
		public void NoExist2 ()
		{
			SetWriteable2 (noExistFile);
		}
	}
}
