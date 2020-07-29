using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Microsoft.Build.Framework;
using Microsoft.Build.Tasks;
using Microsoft.Build.Utilities;

namespace Benchmarks
{
	[MemoryDiagnoser]
	[Orderer (SummaryOrderPolicy.FastestToSlowest)]
	public class HashBenchmark
	{
		static readonly ITaskItem [] items;

		static HashBenchmark ()
		{
			items = new ITaskItem [1000];
			for (int i = 0; i < items.Length; i++) {
				items [i] = new TaskItem ($"Foo\\Bar\\Baz{i}.cs");
			}
		}

		Hash1 hash1 = new Hash1 {
			ItemsToHash = items,
		};

		public string Result { get; set; }

		[Benchmark]
		public void Hash1 ()
		{
			hash1.Execute ();
			Result = hash1.HashResult;
		}

		Hash2 hash2 = new Hash2 {
			ItemsToHash = items,
		};

		[Benchmark]
		public void Hash2 ()
		{
			hash2.Execute ();
			Result = hash2.HashResult;
		}
	}
}
