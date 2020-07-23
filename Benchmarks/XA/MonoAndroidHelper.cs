using System.IO;


namespace Xamarin.Android.Tasks
{
	public partial class MonoAndroidHelper
	{
		public static void SetWriteable (string source)
		{
			if (!File.Exists (source))
				return;

			var fileInfo = new FileInfo (source);
			if (fileInfo.IsReadOnly)
				fileInfo.IsReadOnly = false;
		}

		public static void SetDirectoryWriteable (string directory)
		{
			if (!Directory.Exists (directory))
				return;

			var dirInfo = new DirectoryInfo (directory);
			if ((dirInfo.Attributes | FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
				dirInfo.Attributes &= ~FileAttributes.ReadOnly;

			foreach (var dir in Directory.EnumerateDirectories (directory, "*", SearchOption.AllDirectories)) {
				dirInfo = new DirectoryInfo (dir);
				if ((dirInfo.Attributes | FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
					dirInfo.Attributes &= ~FileAttributes.ReadOnly;
			}

			foreach (var file in Directory.EnumerateFiles (directory, "*", SearchOption.AllDirectories)) {
				SetWriteable (Path.GetFullPath (file));
			}
		}
	}
}
