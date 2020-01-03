using BenchmarkDotNet.Running;
using System;

namespace Benchmarks
{
	class Program
	{
		static void Main (string [] args)
		{
			new AssemblyCustomAttributes();
			var summary = BenchmarkRunner.Run<AssemblyCustomAttributes> ();
		}
	}
}
