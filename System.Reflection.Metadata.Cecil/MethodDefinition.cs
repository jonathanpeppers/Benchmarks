using System;

namespace System.Reflection.Metadata.Cecil
{
	public class MethodDefinition : Base
	{
		readonly Metadata.MethodDefinition method;

		internal MethodDefinition (MetadataReader reader, Metadata.MethodDefinition method) : base (reader, method.Name)
		{
			this.method = method;
		}
	}
}
