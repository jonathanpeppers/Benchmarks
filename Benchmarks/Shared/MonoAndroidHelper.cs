using System;
using System.IO;

namespace Xamarin.Android.Tasks
{
	public class MonoAndroidHelper
	{
		public static void SetWriteable (string source)
		{
			if (!File.Exists (source))
				return;

			var fileInfo = new FileInfo (source);
			if (fileInfo.IsReadOnly)
				fileInfo.IsReadOnly = false;
		}
	}
}
