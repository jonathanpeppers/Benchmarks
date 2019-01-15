using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarks
{
	[Orderer (SummaryOrderPolicy.FastestToSlowest)]
	[MemoryDiagnoser]
	public class FileIO
	{
		string path;

		public FileIO ()
		{
			path = GetType ().Assembly.Location;
		}

		[Benchmark (Description = "Managed System.IO")]
		public void Managed ()
		{
			File.Exists (path);
		}

		[Benchmark (Description = "Native P/Invoke")]
		public void Native ()
		{
			FileExistsWindows (path);
		}

		internal static bool FileExistsWindows (string fullPath)
		{
			WIN32_FILE_ATTRIBUTE_DATA data = new WIN32_FILE_ATTRIBUTE_DATA ();
			bool success = false;

			success = GetFileAttributesEx (fullPath, 0, ref data);
			return success && (data.fileAttributes & FILE_ATTRIBUTE_DIRECTORY) == 0;
		}

		[DllImport ("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs (UnmanagedType.Bool)]
		internal static extern bool GetFileAttributesEx (String name, int fileInfoLevel, ref WIN32_FILE_ATTRIBUTE_DATA lpFileInformation);

		/// <summary>
		/// Contains information about a file or directory; used by GetFileAttributesEx.
		/// </summary>
		[StructLayout (LayoutKind.Sequential)]
		public struct WIN32_FILE_ATTRIBUTE_DATA
		{
			internal int fileAttributes;
			internal uint ftCreationTimeLow;
			internal uint ftCreationTimeHigh;
			internal uint ftLastAccessTimeLow;
			internal uint ftLastAccessTimeHigh;
			internal uint ftLastWriteTimeLow;
			internal uint ftLastWriteTimeHigh;
			internal uint fileSizeHigh;
			internal uint fileSizeLow;
		}

		internal const int FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
	}
}
