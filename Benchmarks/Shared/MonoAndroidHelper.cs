using System;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using Xamarin.Android.Tools;

namespace Xamarin.Android.Tasks
{
	public class MonoAndroidHelper
	{
		// Set in ResolveSdks.Execute();
		// Requires that ResolveSdks.Execute() run before anything else
		public static string [] TargetFrameworkDirectories;
		public static AndroidVersions SupportedVersions;

		public static void RefreshSupportedVersions (string [] referenceAssemblyPaths)
		{
			SupportedVersions = new AndroidVersions (referenceAssemblyPaths);
		}

		public static bool IsFrameworkAssembly (string assembly)
		{
			return IsFrameworkAssembly (assembly, false);
		}

		public static bool IsFrameworkAssembly (string assembly, bool checkSdkPath)
		{
			if (IsSharedRuntimeAssembly (assembly)) {
#if MSBUILD
				bool treatAsUser = Array.BinarySearch (FrameworkAssembliesToTreatAsUserAssemblies, Path.GetFileName (assembly), StringComparer.OrdinalIgnoreCase) >= 0;
				// Framework assemblies don't come from outside the SDK Path;
				// user assemblies do
				if (checkSdkPath && treatAsUser && TargetFrameworkDirectories != null) {
					return ExistsInFrameworkPath (assembly);
				}
#endif
				return true;
			}
			return TargetFrameworkDirectories == null || !checkSdkPath ? false : ExistsInFrameworkPath (assembly);
		}

		public static bool IsSharedRuntimeAssembly (string assembly)
		{
			return Array.BinarySearch (Profile.SharedRuntimeAssemblies, Path.GetFileName (assembly), StringComparer.OrdinalIgnoreCase) >= 0;
		}

		public static bool ExistsInFrameworkPath (string assembly)
		{
			return TargetFrameworkDirectories
					// TargetFrameworkDirectories will contain a "versioned" directory,
					// e.g. $prefix/lib/xamarin.android/xbuild-frameworks/MonoAndroid/v1.0.
					// Trim off the version.
					.Select (p => Path.GetDirectoryName (p.TrimEnd (Path.DirectorySeparatorChar)))
					.Any (p => assembly.StartsWith (p));
		}

		public static bool IsReferenceAssembly (string assembly)
		{
			using (var stream = File.OpenRead (assembly))
			using (var pe = new PEReader (stream)) {
				var reader = pe.GetMetadataReader ();
				var assemblyDefinition = reader.GetAssemblyDefinition ();
				foreach (var handle in assemblyDefinition.GetCustomAttributes ()) {
					var attribute = reader.GetCustomAttribute (handle);
					var attributeName = reader.GetCustomAttributeFullName (attribute);
					if (attributeName == "System.Runtime.CompilerServices.ReferenceAssemblyAttribute")
						return true;
				}
				return false;
			}
		}
	}
}
