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
			using (var resolver = new DirectoryAssemblyResolver (Log, loadDebugSymbols: false)) {
				foreach (var assemblyFile in assemblies) {
					var assembly = resolver.Load (assemblyFile);
					foreach (var mod in assembly.Modules) {
						foreach (var attr in mod.CustomAttributes) {
							var name = attr.AttributeType.Name;
							foreach (var arg in attr.ConstructorArguments) {
								var value = arg.Value.ToString ();
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
			var provider = new CustomAttributeProvider ();
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
						string name = null;
						if (attr.Constructor.Kind == HandleKind.MemberReference) {
							var ctor = reader.GetMemberReference ((MemberReferenceHandle)attr.Constructor);
							var attrType = reader.GetTypeReference ((TypeReferenceHandle)ctor.Parent);
							name = reader.GetString (attrType.Name);

							var decoded = attr.DecodeValue (provider);
							foreach (var arg in decoded.FixedArguments) {
								var value = arg.Value.ToString ();
							}
						} else if (attr.Constructor.Kind == HandleKind.MethodDefinition) {
							var ctor = reader.GetMethodDefinition ((MethodDefinitionHandle)attr.Constructor);
							var attrType = reader.GetTypeDefinition (ctor.GetDeclaringType ());
							name = reader.GetString (attrType.Name);

							var decoded = attr.DecodeValue (provider);
							foreach (var arg in decoded.FixedArguments) {
								var value = arg.Value.ToString ();
							}
						}

						//var blob = reader.GetBlobReader (attr.Value);

						
					}
				}
			}
		}

		class CustomAttributeProvider : ICustomAttributeTypeProvider<string>
		{
			public string GetPrimitiveType (PrimitiveTypeCode typeCode)
			{
				return null;
			}

			public string GetSystemType ()
			{
				return null;
			}

			public string GetSZArrayType (string elementType)
			{
				return null;
			}

			public string GetTypeFromDefinition (MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
			{
				return null;
			}

			public string GetTypeFromReference (MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
			{
				return null;
			}

			public string GetTypeFromSerializedName (string name)
			{
				return null;
			}

			public PrimitiveTypeCode GetUnderlyingEnumType (string type)
			{
				return default (PrimitiveTypeCode);
			}

			public bool IsSystemType (string type)
			{
				return false;
			}
		}
	}
}
