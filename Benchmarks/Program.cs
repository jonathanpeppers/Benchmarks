using BenchmarkDotNet.Running;
using System;

namespace Benchmarks
{
	class Program
	{
		static void Main (string [] args)
		{
			var summary = BenchmarkRunner.Run<FileIO> ();
		}
	}
}
