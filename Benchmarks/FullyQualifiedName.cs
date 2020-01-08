using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using System;
using System.Runtime.CompilerServices;

namespace Benchmarks
{
	[Orderer(SummaryOrderPolicy.FastestToSlowest)]
	[MemoryDiagnoser]
	public class FullyQualifiedName
	{
		Type type = typeof(Guid);

		[Benchmark]
		public void AssemblyGetName()
		{
			var fullName = type.FullName + ", " + type.Assembly.GetName().Name;
		}

		[Benchmark]
		public void Substring()
		{
			var fullName = type.FullName + ", " + GetAssemblyName (type);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static string GetAssemblyName (Type type)
		{
			var name = type.Assembly.FullName;
			int index = name.IndexOf(",", StringComparison.Ordinal);
			if (index != -1)
			{
				return name.Substring(0, index);
			}
			return name;
		}

		[Benchmark]
		public void Substring2()
		{
			var fullName = type.FullName + ", " + GetAssemblyName2(type);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static string GetAssemblyName2(Type type)
		{
			var name = type.Assembly.FullName;
			int index = name.IndexOf(',');
			if (index != -1)
			{
				return name.Substring(0, index);
			}
			return name;
		}

		[Benchmark]
		public void Substring3()
		{
			var fullName = type.FullName + ", " + GetAssemblyName3(type);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static string GetAssemblyName3(Type type)
		{
			var name = type.Assembly.FullName;
			int index = name.IndexOf(",", StringComparison.OrdinalIgnoreCase);
			if (index != -1)
			{
				return name.Substring(0, index);
			}
			return name;
		}
	}
}
