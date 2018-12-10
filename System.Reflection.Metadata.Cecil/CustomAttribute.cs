using System;

namespace System.Reflection.Metadata.Cecil
{
	public class CustomAttribute
	{
		readonly Metadata.CustomAttribute attribute;

		internal CustomAttribute (MetadataReader reader, Metadata.CustomAttribute attribute)
		{
			this.attribute = attribute;

			var ctor = reader.GetMemberReference ((MemberReferenceHandle)attribute.Constructor);
			var attributeType = reader.GetTypeReference ((TypeReferenceHandle)ctor.Parent);
			AttributeType = new TypeReference (reader, attributeType);
		}

		public TypeReference AttributeType {
			get;
			private set;
		}
	}
}
