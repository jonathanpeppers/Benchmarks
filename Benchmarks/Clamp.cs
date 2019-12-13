using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using System;
using System.Runtime.CompilerServices;

namespace Benchmarks
{
	public static class Extensions
	{
		public static double Clamp1(this double self, double min, double max)
		{
			return Math.Min(max, Math.Max(self, min));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Clamp2(this double self, double min, double max)
		{
			if (max < min)
			{
				return self;
			}
			else if (self < min)
			{
				return min;
			}
			else if (self > max)
			{
				return max;
			}

			return self;
		}
	}

	[Orderer (SummaryOrderPolicy.FastestToSlowest)]
	[MemoryDiagnoser]
	public class Clamp
	{
		readonly Random rand = new Random();

		[Benchmark]
		public void Clamp1 ()
		{
			rand.NextDouble().Clamp1(rand.NextDouble(), rand.NextDouble());
		}

		[Benchmark]
		public void Clamp2 ()
		{
			rand.NextDouble().Clamp2(rand.NextDouble(), rand.NextDouble());
		}
	}
}
