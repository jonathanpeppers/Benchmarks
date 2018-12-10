﻿using BenchmarkDotNet.Attributes;
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

		[Benchmark (Description = "Mono.Cecil with default settings.")]
		public void MonoCecil ()
		{
			using (var resolver = new DirectoryAssemblyResolver (Log, loadDebugSymbols: false)) {
				IterateAssemblies (resolver);
			}
		}

		//[Benchmark (Description = "Mono.Cecil with InMemory=True setting.")]
		public void MonoCecil_InMemory ()
		{
			var rp = new ReaderParameters {
				InMemory = true
			};
			using (var resolver = new DirectoryAssemblyResolver (Log, loadDebugSymbols: false, loadReaderParameters: rp)) {
				IterateAssemblies (resolver);
			}
		}

		//[Benchmark (Description = "Mono.Cecil with ReadingMode.Deferred setting.")]
		public void MonoCecil_Deferred ()
		{
			var rp = new ReaderParameters {
				ReadingMode = ReadingMode.Deferred,
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
						foreach (var method in type.Methods) {
							var mname = method.Name;
						}
					}
				}
			}
		}

		[Benchmark (Description = "System.Reflection.Metadata with default settings.")]
		public void SystemReflectionMetadata ()
		{
			foreach (var assemblyFile in assemblies) {
				using (var stream = File.OpenRead (assemblyFile))
				using (var pe = new PEReader (stream)) {
					var reader = pe.GetMetadataReader ();
					var assembly = reader.GetAssemblyDefinition ();
					foreach (var r in reader.ManifestResources) {
						var resource = reader.GetManifestResource (r);
						var name = reader.GetString (resource.Name);
					}
					foreach (var a in reader.CustomAttributes) {
						var attr = reader.GetCustomAttribute (a);
						if (attr.Constructor.Kind == HandleKind.MemberReference) {
							var ctor = reader.GetMemberReference ((MemberReferenceHandle)attr.Constructor);
							var attrType = reader.GetTypeReference ((TypeReferenceHandle)ctor.Parent);
							var ns = reader.GetString (attrType.Namespace);
							var n = reader.GetString (attrType.Name);
							var name = ns + "." + n;
						} 
						//TODO: don't know how to get string in this case
						//else if (attr.Constructor.Kind == HandleKind.MethodDefinition) {
						//	var ctor = reader.GetMethodDefinition ((MethodDefinitionHandle)attr.Constructor);
						//	var name = reader.GetString (ctor.Name);
						//}
					}
					foreach (var t in reader.TypeDefinitions) {
						var type = reader.GetTypeDefinition (t);
						var name = reader.GetString (type.Name);
						foreach (var m in type.GetMethods ()) {
							var method = reader.GetMethodDefinition (m);
							var mname = reader.GetString (method.Name);
						}
					}
				}
			}
		}

		[Benchmark (Description = "System.Reflection.Metadata with Mono.Cecil compat.")]
		public void SystemReflectionMetadata_Compat ()
		{
			foreach (var assemblyFile in assemblies) {
				using (var assembly = System.Reflection.Metadata.Cecil.AssemblyDefinition.ReadAssembly (assemblyFile)) {
					foreach (var mod in assembly.Modules) {
						foreach (var resource in mod.Resources) {
							var name = resource.Name;
						}
						foreach (var attr in mod.CustomAttributes) {
							var name = attr.AttributeType.Name;
						}
						foreach (var type in mod.Types) {
							var name = type.Name;
							foreach (var method in type.Methods) {
								var mname = method.Name;
							}
						}
					}
				}
			}
		}
	}
}
