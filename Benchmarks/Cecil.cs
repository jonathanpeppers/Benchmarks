using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Java.Interop.Tools.Cecil;
using Mono.Cecil;
using System.Collections.Generic;
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
		readonly string assembliesDir;
		readonly string [] assemblies;

		public Cecil ()
		{
			var bin = Path.GetDirectoryName (GetType ().Assembly.Location);
			assembliesDir = Path.GetFullPath (Path.Combine (bin, "..", "..", "Assemblies"));
			assemblies = Directory.GetFiles (assembliesDir, "*.dll");
		}

		[Benchmark (Description = "Mono.Cecil")]
		public void MonoCecil ()
		{
			using (var resolver = new DirectoryAssemblyResolver (Log, loadDebugSymbols: false)) {
				foreach (var assemblyFile in assemblies) {
					var assembly = resolver.Load (assemblyFile);
					foreach (var mod in assembly.Modules) {
						foreach (var type in mod.Types) {
							var name = type.Name;
							var baseType = type;
							while ((baseType = baseType.BaseType?.Resolve ()) != null) {
								name = baseType.Name;
							}
						}
					}
				}
			}
		}

		void Log (TraceLevel level, string message) { }

		//[Benchmark (Description = "System.Reflection.Metadata")]
		public void SystemReflectionMetadata ()
		{
			using (var resolver = new AssemblyResolver ()) {
				resolver.AddSearchDirectory (assembliesDir);

				foreach (var assemblyFile in assemblies) {
					var reader = resolver.GetAssemblyReader (Path.GetFileName (assemblyFile));
					foreach (var t in reader.TypeDefinitions) {
						var type = reader.GetTypeDefinition (t);
						var name = reader.GetString (type.Name);
						if (name == "<Module>")
							continue;

						var resolved = new ResolvedTypeDefinition {
							Type = type,
							Reader = reader,
						};
						while ((resolved = GetBaseType (resolved.Type, resolver, resolved.Reader)) != null) {
							name = resolved.Reader.GetString (resolved.Type.Name);
						}
					}
				}
			}
		}

		ResolvedTypeDefinition GetBaseType (System.Reflection.Metadata.TypeDefinition type, AssemblyResolver resolver, MetadataReader reader)
		{
			var baseType = type.BaseType;
			if (baseType.IsNil) {
				return null;
			} if (baseType.Kind == HandleKind.TypeDefinition) {
				return new ResolvedTypeDefinition {
					Type = reader.GetTypeDefinition ((TypeDefinitionHandle)baseType),
					Reader = reader,
				};
			} else if (baseType.Kind == HandleKind.TypeReference) {
				var typeReference = reader.GetTypeReference ((TypeReferenceHandle)baseType);
				var (assemblyReader, typeDefinition) = resolver.ResolveType (reader, typeReference);
				return new ResolvedTypeDefinition {
					Type = typeDefinition,
					Reader = assemblyReader,
				};
			} else {
				return null;
			}
		}

		class ResolvedTypeDefinition
		{
			public MetadataReader Reader { get; set; }
			public System.Reflection.Metadata.TypeDefinition Type { get; set; }
		}
	}
}
