using System;

namespace System.Reflection.Metadata
{
	public static class TypeExtensions
	{
		public static string GetFullName (this MetadataReader reader, TypeReference type)
		{
			var ns = reader.GetString (type.Namespace);
			var name = reader.GetString (type.Name);
			return $"{ns}.{name}";
		}

		public static string GetFullName (this MetadataReader reader, TypeDefinition type)
		{
			var ns = reader.GetString (type.Namespace);
			var name = reader.GetString (type.Name);
			return $"{ns}.{name}";
		}

		public static string GetFullName (this MetadataReader reader, ExportedType type)
		{
			var ns = reader.GetString (type.Namespace);
			var name = reader.GetString (type.Name);
			return $"{ns}.{name}";
		}
	}
}
