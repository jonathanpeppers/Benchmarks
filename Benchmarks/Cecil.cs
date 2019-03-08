﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Java.Interop.Tools.Cecil;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
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

		void Log (TraceLevel level, string message) { }

		[Benchmark (Description = "System.Reflection.Metadata")]
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
					foreach (var a in assembly.GetCustomAttributes ()) {
						var attr = reader.GetCustomAttribute (a);
						if (attr.Constructor.Kind == HandleKind.MemberReference) {
							var ctor = reader.GetMemberReference ((MemberReferenceHandle)attr.Constructor);
							var attrType = reader.GetTypeReference ((TypeReferenceHandle)ctor.Parent);
							var name = reader.GetString (attrType.Name);
						} else if (attr.Constructor.Kind == HandleKind.MethodDefinition) {
							var ctor = reader.GetMethodDefinition ((MethodDefinitionHandle)attr.Constructor);
							var attrType = reader.GetTypeDefinition (ctor.GetDeclaringType ());
							var name = reader.GetString (attrType.Name);
						}
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

		[Benchmark (Description = "System.Reflection.MetadataLoadContext")]
		public void SystemReflectionMetadataLoadContext ()
		{
			var resolver = new SimpleResolver (assemblies);
			using (var context = new MetadataLoadContext (resolver)) {
				foreach (var assemblyFile in assemblies) {
					var assembly = context.LoadFromAssemblyPath (assemblyFile);
					foreach (var r in assembly.GetManifestResourceNames ()) {
						var name = r;
					}
					foreach (var a in assembly.CustomAttributes) {
						var name = a.AttributeType.Name;
					}
					foreach (var t in assembly.GetTypes ()) {
						var name = t.Name;
						foreach (var m in t.GetMethods()) {
							var mname = m.Name;
						}
					}
				}
			}
		}

		/// <summary>
		/// NOTE: we can't just use System.Reflection.PathAssemblyResolver, because it compares PublicKeyToken
		/// </summary>
		class SimpleResolver : MetadataAssemblyResolver
		{
			readonly Dictionary<string, string> assemblyNames = new Dictionary<string, string> (StringComparer.Ordinal);

			public SimpleResolver (string[] assemblies)
			{
				foreach (string assemblyPath in assemblies) {
					assemblyNames.Add (Path.GetFileNameWithoutExtension (assemblyPath), assemblyPath);
				}
			}

			public override Assembly Resolve (MetadataLoadContext context, AssemblyName assemblyName)
			{
				return context.LoadFromAssemblyPath (assemblyNames [assemblyName.Name]);
			}
		}
	}
}
