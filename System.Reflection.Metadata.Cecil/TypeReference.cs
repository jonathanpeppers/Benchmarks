using System;

namespace System.Reflection.Metadata.Cecil
{
	public class TypeReference : Base
	{
		readonly Metadata.TypeReference reference;

		internal TypeReference (MetadataReader reader, Metadata.TypeReference reference) : base (reader, reference.Name)
		{
			this.reference = reference;
		}
	}
}
