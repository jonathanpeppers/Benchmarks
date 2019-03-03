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

		[Benchmark (Description = "System.Reflection.Metadata")]
		public void SystemReflectionMetadata ()
		{
			foreach (var assemblyFile in assemblies) {
				using (var stream = File.OpenRead (assemblyFile))
				using (var pe = new PEReader (stream)) {
					var reader = pe.GetMetadataReader ();
					var assembly = reader.GetAssemblyDefinition ();
					foreach (var t in reader.TypeDefinitions) {
						var type = reader.GetTypeDefinition (t);
						var name = reader.GetString (type.Name);
						if (name == "<Module>")
							continue;

						System.Reflection.Metadata.TypeDefinition? baseType = type;
						while ((baseType = GetBaseType (type, reader, assemblyFile)) != null) {
							name = reader.GetString (baseType.Value.Name);
						}
					}
				}
			}
		}

		System.Reflection.Metadata.TypeDefinition? GetBaseType (System.Reflection.Metadata.TypeDefinition type, MetadataReader reader, string assemblyFile)
		{
			var baseType = type.BaseType;
			if (baseType.Kind == HandleKind.TypeDefinition) {
				return reader.GetTypeDefinition ((TypeDefinitionHandle)baseType);
			} else if (baseType.Kind == HandleKind.TypeReference) {
				var typeRef = reader.GetTypeReference ((TypeReferenceHandle)baseType);
				var assembly = reader.GetAssemblyReference ((AssemblyReferenceHandle)typeRef.ResolutionScope);
				var assemblyName = reader.GetString (assembly.Name);

				var typeNS = reader.GetString (typeRef.Namespace);
				var typeName = reader.GetString (typeRef.Name);
				using (var stream = File.OpenRead (Path.Combine (Path.GetDirectoryName (assemblyFile), assemblyName + ".dll")))
				using (var pe = new PEReader (stream)) {
					var r = pe.GetMetadataReader ();
					foreach (var e in r.ExportedTypes) {
						var et = r.GetExportedType (e);
						if (r.GetString (et.Name) != typeName)
							continue;
						if (r.GetString (et.Namespace) != typeNS)
							continue;
						throw new System.Exception ();
					}

					throw new System.Exception ();
				}
			} else {
				throw new System.Exception ();
			}
		}
	}
}
