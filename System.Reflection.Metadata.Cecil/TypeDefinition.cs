using System.Collections.Generic;

namespace System.Reflection.Metadata.Cecil
{
	public class TypeDefinition : Base
	{
		readonly MetadataReader reader;
		readonly Metadata.TypeDefinition type;

		internal TypeDefinition (MetadataReader reader, Metadata.TypeDefinition type) : base (reader, type.Name)
		{
			this.reader = reader;
			this.type = type;
			methods = new Lazy<MethodDefinition []> (LoadMethods);
		}

		MethodDefinition [] LoadMethods ()
		{
			var methods = type.GetMethods ();
			var list = new List<MethodDefinition> (methods.Count);
			foreach (var m in methods) {
				var method = reader.GetMethodDefinition (m);
				list.Add (new MethodDefinition (reader, method));
			}
			return list.ToArray ();
		}

		readonly Lazy<MethodDefinition []> methods;

		public MethodDefinition [] Methods => methods.Value;
	}
}
