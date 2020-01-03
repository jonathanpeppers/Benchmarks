using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using System;
using System.Linq;
using System.Reflection;

namespace Benchmarks
{
	[Orderer (SummaryOrderPolicy.FastestToSlowest)]
	[MemoryDiagnoser]
	public class AssemblyCustomAttributes
	{
		readonly Assembly[] assemblies;

		public AssemblyCustomAttributes ()
		{
			// Load random extra assemblies
			System.Net.Http.HttpClient c = new System.Net.Http.HttpClient();
			System.Xml.Linq.XDocument x = new System.Xml.Linq.XDocument();

			assemblies = AppDomain.CurrentDomain.GetAssemblies();
		}

		[Benchmark]
		public void GetCA1 ()
		{
			foreach (var assembly in assemblies)
			{
				assembly.GetCustomAttributes<AssemblyVersionAttribute>().ToArray();
				assembly.GetCustomAttributes<AssemblyFileVersionAttribute>().ToArray();
				assembly.GetCustomAttributes<AssemblyTitleAttribute>().ToArray();
				assembly.GetCustomAttributes<AssemblyProductAttribute>().ToArray();
			}
		}

		[Benchmark]
		public void GetCA2()
		{
			foreach (var assembly in assemblies)
			{
				var attributes = assembly.GetCustomAttributes();
				attributes.OfType<AssemblyVersionAttribute>().ToArray();
				attributes.OfType<AssemblyFileVersionAttribute>().ToArray();
				attributes.OfType<AssemblyTitleAttribute>().ToArray();
				attributes.OfType<AssemblyProductAttribute>().ToArray();
			}
		}

		[Benchmark]
		public void GetCA3()
		{
			foreach (var assembly in assemblies)
			{
				var attributes = assembly.GetCustomAttributes().ToArray();
				attributes.OfType<AssemblyVersionAttribute>().ToArray();
				attributes.OfType<AssemblyFileVersionAttribute>().ToArray();
				attributes.OfType<AssemblyTitleAttribute>().ToArray();
				attributes.OfType<AssemblyProductAttribute>().ToArray();
			}
		}
	}
}
