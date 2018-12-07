using BenchmarkDotNet.Attributes;
using Java.Interop.Tools.Cecil;
using Mono.Cecil;
using System;
using System.Diagnostics;
using System.IO;

namespace Benchmarks
{
	public class Cecil
	{
		readonly string [] assemblies;

		public Cecil ()
		{
			var bin = Path.GetDirectoryName (GetType ().Assembly.Location);
			var assembliesDir = Path.Combine (bin, "..", "..", "Assemblies");
			assemblies = Directory.GetFiles (assembliesDir, "*.dll");
		}

		[Benchmark]
		public void Defaults ()
		{
			using (var resolver = new DirectoryAssemblyResolver (Log, loadDebugSymbols: false)) {
				IterateAssemblies (resolver);
			}
		}

		[Benchmark]
		public void InMemory ()
		{
			var rp = new ReaderParameters {
				InMemory = true
			};
			using (var resolver = new DirectoryAssemblyResolver (Log, loadDebugSymbols: false, loadReaderParameters: rp)) {
				IterateAssemblies (resolver);
			}
		}

		void Log (TraceLevel level, string message) { }

		void IterateAssemblies (DirectoryAssemblyResolver resolver)
		{
			foreach (var assemblyFile in assemblies) {
				var assembly = resolver.Load (assemblyFile);
				foreach (var mod in assembly.Modules) {
					foreach (var resource in mod.Resources) {
						var name = resource.Name;
					}
					foreach (var attr in mod.CustomAttributes) {
						var name = attr.AttributeType.Name;
					}
					foreach (var type in mod.Types) {
						var name = type.Name;
					}
				}
			}
		}
	}
}
