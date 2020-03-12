using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Benchmarks
{
	[Orderer (SummaryOrderPolicy.FastestToSlowest)]
	[MemoryDiagnoser]
	public class HexString
	{
		readonly byte [] bytes = Guid.NewGuid ().ToByteArray ();

		[Benchmark]
		public void XAToHexString ()
		{
			XamarinAndroid.ToHexString (bytes);
		}

		class XamarinAndroid
		{
			public static string ToHexString (byte [] hash)
			{
				char [] array = new char [hash.Length * 2];
				for (int i = 0, j = 0; i < hash.Length; i += 1, j += 2) {
					byte b = hash [i];
					array [j] = GetHexValue (b / 16);
					array [j + 1] = GetHexValue (b % 16);
				}
				return new string (array);
			}
		}

		[MethodImpl (MethodImplOptions.AggressiveInlining)]
		static char GetHexValue (int i) => (char) (i < 10 ? i + 48 : i - 10 + 65);

		[Benchmark]
		public void TizenToHex ()
		{
			Tizen.ToHex (bytes);
		}

		class Tizen
		{
			static readonly char [] HexDigits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };
			public static string ToHex (byte[] bin)
			{
				char [] hex = new char [32];
				for (var i = 0; i < 16; ++i) {
					hex [2 * i] = HexDigits [bin [i] >> 4];
					hex [2 * i + 1] = HexDigits [bin [i] & 0xf];
				}
				return new string (hex);
			}
		}

		[Benchmark]
		public void StackOverflow ()
		{
			SO.ByteArrayToHexViaLookup32 (bytes);
		}

		class SO
		{
			private static readonly uint [] _lookup32 = CreateLookup32 ();

			private static uint [] CreateLookup32 ()
			{
				var result = new uint [256];
				for (int i = 0; i < 256; i++) {
					string s = i.ToString ("X2");
					result [i] = ((uint) s [0]) + ((uint) s [1] << 16);
				}
				return result;
			}

			public static string ByteArrayToHexViaLookup32 (byte [] bytes)
			{
				var lookup32 = _lookup32;
				var result = new char [bytes.Length * 2];
				for (int i = 0; i < bytes.Length; i++) {
					var val = lookup32 [bytes [i]];
					result [2 * i] = (char) val;
					result [2 * i + 1] = (char) (val >> 16);
				}
				return new string (result);
			}
		}

		[Benchmark]
		public void StackOverflowUnsafe ()
		{
			SOUnsafe.ByteArrayToHexViaLookup32Unsafe (bytes);
		}

		unsafe class SOUnsafe
		{
			private static readonly uint [] _lookup32Unsafe = CreateLookup32Unsafe ();
			private static readonly uint* _lookup32UnsafeP = (uint*) GCHandle.Alloc (_lookup32Unsafe, GCHandleType.Pinned).AddrOfPinnedObject ();

			private static uint [] CreateLookup32Unsafe ()
			{
				var result = new uint [256];
				for (int i = 0; i < 256; i++) {
					string s = i.ToString ("X2");
					if (BitConverter.IsLittleEndian)
						result [i] = ((uint) s [0]) + ((uint) s [1] << 16);
					else
						result [i] = ((uint) s [1]) + ((uint) s [0] << 16);
				}
				return result;
			}

			public static string ByteArrayToHexViaLookup32Unsafe (byte [] bytes)
			{
				var lookupP = _lookup32UnsafeP;
				var result = new char [bytes.Length * 2];
				fixed (byte* bytesP = bytes)
				fixed (char* resultP = result) {
					uint* resultP2 = (uint*) resultP;
					for (int i = 0; i < bytes.Length; i++) {
						resultP2 [i] = lookupP [bytesP [i]];
					}
				}
				return new string (result);
			}
		}
	}
}
