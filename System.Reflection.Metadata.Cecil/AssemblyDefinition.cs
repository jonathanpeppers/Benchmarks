using System.IO;
using System.Reflection.PortableExecutable;

namespace System.Reflection.Metadata.Cecil
{
	public class AssemblyDefinition : IDisposable
    {
		public static AssemblyDefinition ReadAssembly (string fileName)
		{
			return ReadAssembly (File.OpenRead (fileName));
		}

		public static AssemblyDefinition ReadAssembly (Stream stream)
		{
			return new AssemblyDefinition (stream);
		}

		readonly PEReader peReader;
		readonly MetadataReader reader;

		private AssemblyDefinition (Stream stream)
		{
			peReader = new PEReader (stream);
			reader = peReader.GetMetadataReader ();
			modules = new Lazy<ModuleDefinition []> (() => new [] { new ModuleDefinition (reader) });
		}

		readonly Lazy<ModuleDefinition []> modules;

		public ModuleDefinition [] Modules => modules.Value;

		public void Dispose () => peReader.Dispose ();
	}
}
