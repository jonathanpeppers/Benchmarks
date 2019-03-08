using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Java.Interop.Tools.Cecil;
using Mono.Cecil;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using Xamarin.Tools.Zip;

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
			using (var memory = new MemoryStream ())
			using (var resolver = new DirectoryAssemblyResolver (Log, loadDebugSymbols: false)) {
				foreach (var assemblyFile in assemblies) {
					var assembly = resolver.Load (assemblyFile);
					foreach (var mod in assembly.Modules) {
						foreach (var r in mod.Resources) {
							if (r is EmbeddedResource resource) {
								using (var s = resource.GetResourceStream ()) {
									var hash = HashStream (s);
									if (resource.Name == "__AndroidLibraryProjects__.zip") {
										s.Position = 0;
										using (var zip = ZipArchive.Open (s)) {
											foreach (var entry in zip) {
												memory.SetLength (0);
												entry.Extract (memory);
											}
										}
									}
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
			using (var memory = new MemoryStream ()) {
				foreach (var assemblyFile in assemblies) {
					using (var stream = File.OpenRead (assemblyFile))
					using (var pe = new PEReader (stream)) {
						var reader = pe.GetMetadataReader ();
						var assembly = reader.GetAssemblyDefinition ();
						foreach (var r in reader.ManifestResources) {
							var resource = reader.GetManifestResource (r);
							var name = reader.GetString (resource.Name);
							using (var s = GetEmbeddedResourceStream (pe, resource)) {
								var hash = HashStream (s);
								if (name == "__AndroidLibraryProjects__.zip") {
									s.Position = 0;
									using (var zip = ZipArchive.Open (s)) {
										foreach (var entry in zip) {
											memory.SetLength (0);
											entry.Extract (memory);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Get the bytes in an embedded resource as a Stream.
		/// WARNING: It is incorrect to read from this stream after the PEReader has been disposed.
		/// 
		/// See:
		///		https://github.com/dotnet/corefx/issues/23372
		///		https://gist.github.com/nguerrera/6864d2a907cb07d869be5a2afed8d764
		/// </summary>
		public static unsafe Stream GetEmbeddedResourceStream (PEReader peReader, ManifestResource resource)
		{
			checked // arithmetic overflow here could cause AV
			{
				// Locate start and end of PE image in unmanaged memory.
				var block = peReader.GetEntireImage ();
				Debug.Assert (block.Pointer != null && block.Length > 0);

				byte* peImageStart = block.Pointer;
				byte* peImageEnd = peImageStart + block.Length;

				// Locate offset to resources within PE image.
				int offsetToResources;
				if (!peReader.PEHeaders.TryGetDirectoryOffset (peReader.PEHeaders.CorHeader.ResourcesDirectory, out offsetToResources)) {
					throw new BadImageFormatException ("Failed to get offset to resources in PE file.");
				}
				Debug.Assert (offsetToResources > 0);
				byte* resourceStart = peImageStart + offsetToResources + resource.Offset;

				// Get the length of the the resource from the first 4 bytes.
				if (resourceStart >= peImageEnd - sizeof (int)) {
					throw new BadImageFormatException ("resource offset out of bounds.");
				}

				int resourceLength = new BlobReader (resourceStart, sizeof (int)).ReadInt32 ();
				resourceStart += sizeof (int);
				if (resourceLength < 0 || resourceStart >= peImageEnd - resourceLength) {
					throw new BadImageFormatException ("resource offset or length out of bounds.");
				}

				return new UnmanagedMemoryStream (resourceStart, resourceLength);
			}
		}

		public static string HashStream (Stream stream)
		{
			using (HashAlgorithm hashAlg = new SHA1Managed ()) {
				byte [] hash = hashAlg.ComputeHash (stream);
				return BitConverter.ToString (hash);
			}
		}
	}
}
