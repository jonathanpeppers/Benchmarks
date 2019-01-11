using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Java.Interop.Tools.Cecil;
using Mono.Cecil;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace Benchmarks
{
	[Orderer (SummaryOrderPolicy.FastestToSlowest)]
	[MemoryDiagnoser]
	public class Cecil
	{
		readonly string [] assemblies;

		public Cecil ()
		{
			var bin = Path.GetDirectoryName (GetType ().Assembly.Location);
			var assembliesDir = Path.Combine (bin, "..", "..", "Assemblies");
			assemblies = Directory.GetFiles (assembliesDir, "*.dll");
		}

		[Benchmark (Description = "Mono.Cecil")]
		public void MonoCecil ()
		{
			using (var resolver = new DirectoryAssemblyResolver (Log, loadDebugSymbols: false))
			using (var memoryStream = new MemoryStream ()) {
				foreach (var assemblyFile in assemblies) {
					var assembly = resolver.Load (assemblyFile);
					foreach (var mod in assembly.Modules) {
						foreach (var resource in mod.Resources) {
							var name = resource.Name;
							if (resource is EmbeddedResource embedded) {
								memoryStream.SetLength (0);
								using (var resourceStream = embedded.GetResourceStream ()) {
									resourceStream.CopyTo (memoryStream);
								}
							}
						}
					}
				}
			}
		}

		void Log (TraceLevel level, string message) { }

		[Benchmark (Description = "System.Reflection.Metadata")]
		public void SystemReflectionMetadata ()
		{
			using (var memoryStream = new MemoryStream ()) {
				foreach (var assemblyFile in assemblies) {
					using (var stream = File.OpenRead (assemblyFile))
					using (var pe = new PEReader (stream)) {
						var reader = pe.GetMetadataReader ();
						var assembly = reader.GetAssemblyDefinition ();
						foreach (var r in reader.ManifestResources) {
							var resource = reader.GetManifestResource (r);
							var name = reader.GetString (resource.Name);
							memoryStream.SetLength (0);
							using (var resourceStream = pe.GetEmbeddedResourceStream (resource)) {
								resourceStream.CopyTo (memoryStream);
							}
						}
					}
				}
			}
		}
	}
}
