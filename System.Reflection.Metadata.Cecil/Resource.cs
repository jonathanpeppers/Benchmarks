using System;

namespace System.Reflection.Metadata.Cecil
{
	public class Resource : Base
	{
		readonly ManifestResource resource;

		internal Resource (MetadataReader reader, ManifestResource resource) : base (reader, resource.Name)
		{
			this.resource = resource;
		}
	}
}
