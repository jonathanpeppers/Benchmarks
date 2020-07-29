using System.Diagnostics;
using BenchmarkDotNet.Running;

namespace Benchmarks
{
	class Program
	{
		static void Main (string [] args)
		{
#if DEBUG
			var b = new HashBenchmark ();
			b.Hash1 ();
			var hash1 = b.Result;
			b.Hash2 ();
			var hash2 = b.Result;
			Debug.Assert (hash1 == hash2, $"Hashes should match! {hash1}, {hash2}");
#endif

			BenchmarkSwitcher.FromAssembly (typeof (Program).Assembly).Run (args);
		}
	}
}
