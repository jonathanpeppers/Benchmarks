using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection.PortableExecutable;

namespace System.Reflection.Metadata
{
	/// <summary>
	/// A replacement for DirectoryAssemblyResolver, using System.Reflection.Metadata
	/// </summary>
	public class AssemblyResolver : IDisposable
	{
		readonly Dictionary<string, PEReader> cache = new Dictionary<string, PEReader> ();
		// SortedSet<T>: an ordered collection, containing no duplicates
		readonly SortedSet<string> searchDirectories = new SortedSet<string> ();

		public MetadataReader GetAssemblyReader (string assemblyName)
		{
			var key = Path.GetFileNameWithoutExtension (assemblyName);
			if (!cache.TryGetValue (key, out PEReader reader)) {
				var assemblyPath = Resolve (assemblyName);
				cache.Add (key, reader = new PEReader (File.OpenRead (assemblyPath)));
			}
			return reader.GetMetadataReader ();
		}

		public string [] SearchDirectories => searchDirectories.ToArray ();

		public void AddSearchDirectory (string directory)
		{
			searchDirectories.Add (Path.GetFullPath (directory));
		}

		public string Resolve (string assemblyName)
		{
			if (Path.IsPathRooted (assemblyName)) {
				return assemblyName;
			}

			string assemblyPath = assemblyName;
			if (!assemblyPath.EndsWith (".dll", StringComparison.OrdinalIgnoreCase)) {
				assemblyPath += ".dll";
			}
			if (File.Exists (assemblyPath)) {
				return assemblyPath;
			}
			foreach (var dir in searchDirectories) {
				var path = Path.Combine (dir, assemblyPath);
				if (File.Exists (path))
					return path;
			}

			throw new FileNotFoundException ($"Could not load assembly '{assemblyName}'.", assemblyName);
		}

		public (bool, MetadataReader, TypeDefinition) ResolveType (MetadataReader reader, TypeReference type)
		{
			if (type.ResolutionScope.Kind == HandleKind.TypeReference) {
				return ResolveType (reader, reader.GetTypeReference ((TypeReferenceHandle)type.ResolutionScope));
			}
			if (type.ResolutionScope.Kind != HandleKind.AssemblyReference) {
				throw new InvalidOperationException ($"Cannot resolve ResolutionScope.Kind={type.ResolutionScope.Kind}!");
			}

			//Resolve the assembly
			var handle = (AssemblyReferenceHandle)type.ResolutionScope;
			var assemblyReference = reader.GetAssemblyReference (handle);
			var assemblyName = assemblyReference.GetAssemblyName ();
			var assemblyReader = GetAssemblyReader (assemblyName.Name);

			//Now find the type
			var fullName = reader.GetFullName (type);
			foreach (var h in assemblyReader.TypeDefinitions) {
				var typeDefinition = assemblyReader.GetTypeDefinition (h);
				if (assemblyReader.GetFullName (typeDefinition) == fullName) {
					return (true, assemblyReader, typeDefinition);
				}
			}

			return (false, null, default (TypeDefinition));
			//throw new InvalidOperationException ($"Unable to find type {fullName} in assembly {assemblyName.Name}!");
		}

		public void Dispose ()
		{
			foreach (var provider in cache.Values) {
				provider.Dispose ();
			}
			cache.Clear ();
		}
	}
}