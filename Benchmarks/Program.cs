using BenchmarkDotNet.Running;

namespace Benchmarks
{
	class Program
	{
		static void Main (string [] args)
		{
			//var f = new FilesBenchmarks ();
			//f.GlobalSetup ();
			//f.IterationSetup ();
			//f.CopyIfChanged2 ();

			BenchmarkSwitcher.FromAssembly (typeof (Program).Assembly).Run (args);
		}
	}
}
