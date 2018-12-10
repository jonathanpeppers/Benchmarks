using System;

namespace System.Reflection.Metadata.Cecil
{
	public abstract class Base
	{
		internal Base (MetadataReader reader, StringHandle handle)
		{
			Name = reader.GetString (handle);
		}

		public string Name {
			get;
			private set;
		}
	}
}
