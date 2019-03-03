using BenchmarkDotNet.Running;
using System;

namespace Benchmarks
{
	class Program
	{
		static void Main (string [] args)
		{
			new Cecil ().SystemReflectionMetadata ();

			var summary = BenchmarkRunner.Run<Cecil> ();
		}
	}
}
