using System.Collections.Generic;

namespace System.Reflection.Metadata.Cecil
{
	public class ModuleDefinition
	{
		readonly MetadataReader reader;

		internal ModuleDefinition (MetadataReader reader)
		{
			this.reader = reader;
			resources = new Lazy<Resource []> (LoadResources);
			customAttributes = new Lazy<CustomAttribute []> (LoadCustomAttributes);
			types = new Lazy<TypeDefinition []> (LoadTypes);
		}

		Resource [] LoadResources ()
		{
			var list = new List<Resource> (reader.ManifestResources.Count);
			foreach (var r in reader.ManifestResources) {
				var resource = reader.GetManifestResource (r);
				list.Add (new Resource (reader, resource));
			}
			return list.ToArray ();
		}

		readonly Lazy<Resource []> resources;

		public Resource [] Resources => resources.Value;

		CustomAttribute [] LoadCustomAttributes ()
		{
			var list = new List<CustomAttribute> (reader.CustomAttributes.Count);
			foreach (var a in reader.CustomAttributes) {
				var attribute = reader.GetCustomAttribute (a);
				if (attribute.Constructor.Kind == HandleKind.MemberReference) {
					list.Add (new CustomAttribute (reader, attribute));
				} else {
					//TODO: don't know how to get string in this case
					//attr.Constructor.Kind == HandleKind.MethodDefinition
				}
			}
			return list.ToArray ();
		}

		readonly Lazy<CustomAttribute []> customAttributes;

		public CustomAttribute [] CustomAttributes => customAttributes.Value;

		TypeDefinition [] LoadTypes ()
		{
			var list = new List<TypeDefinition> (reader.TypeDefinitions.Count);
			foreach (var t in reader.TypeDefinitions) {
				var type = reader.GetTypeDefinition (t);
				list.Add (new TypeDefinition (reader, type));
			}
			return list.ToArray ();
		}

		readonly Lazy<TypeDefinition []> types;

		public TypeDefinition [] Types => types.Value;
	}
}
