using BenchmarkDotNet.Running;
using System;

namespace Benchmarks
{
	class Program
	{
		static void Main (string [] args)
		{
			//var b = new Benchmarks ();
			//b.Setup ();
			//b.ResolveAssemblies ();

			var summary = BenchmarkRunner.Run<Benchmarks> ();
		}
	}
}
