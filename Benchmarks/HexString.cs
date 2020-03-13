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

		[Benchmark]
		public unsafe void GrendelStaticArray ()
		{
			Grendel.ToHex (bytes);
		}

		// https://gist.github.com/grendello/da70f111380c4b2e689e072ed496ad74
		unsafe class Grendel
		{
			static readonly unsafe uint [] byteToHex = {
				3145776, 3211312, 3276848, 3342384, 3407920, 3473456, 3538992, 3604528, 3670064, 3735600,
				6357040, 6422576, 6488112, 6553648, 6619184, 6684720, 3145777, 3211313, 3276849, 3342385,
				3407921, 3473457, 3538993, 3604529, 3670065, 3735601, 6357041, 6422577, 6488113, 6553649,
				6619185, 6684721, 3145778, 3211314, 3276850, 3342386, 3407922, 3473458, 3538994, 3604530,
				3670066, 3735602, 6357042, 6422578, 6488114, 6553650, 6619186, 6684722, 3145779, 3211315,
				3276851, 3342387, 3407923, 3473459, 3538995, 3604531, 3670067, 3735603, 6357043, 6422579,
				6488115, 6553651, 6619187, 6684723, 3145780, 3211316, 3276852, 3342388, 3407924, 3473460,
				3538996, 3604532, 3670068, 3735604, 6357044, 6422580, 6488116, 6553652, 6619188, 6684724,
				3145781, 3211317, 3276853, 3342389, 3407925, 3473461, 3538997, 3604533, 3670069, 3735605,
				6357045, 6422581, 6488117, 6553653, 6619189, 6684725, 3145782, 3211318, 3276854, 3342390,
				3407926, 3473462, 3538998, 3604534, 3670070, 3735606, 6357046, 6422582, 6488118, 6553654,
				6619190, 6684726, 3145783, 3211319, 3276855, 3342391, 3407927, 3473463, 3538999, 3604535,
				3670071, 3735607, 6357047, 6422583, 6488119, 6553655, 6619191, 6684727, 3145784, 3211320,
				3276856, 3342392, 3407928, 3473464, 3539000, 3604536, 3670072, 3735608, 6357048, 6422584,
				6488120, 6553656, 6619192, 6684728, 3145785, 3211321, 3276857, 3342393, 3407929, 3473465,
				3539001, 3604537, 3670073, 3735609, 6357049, 6422585, 6488121, 6553657, 6619193, 6684729,
				3145825, 3211361, 3276897, 3342433, 3407969, 3473505, 3539041, 3604577, 3670113, 3735649,
				6357089, 6422625, 6488161, 6553697, 6619233, 6684769, 3145826, 3211362, 3276898, 3342434,
				3407970, 3473506, 3539042, 3604578, 3670114, 3735650, 6357090, 6422626, 6488162, 6553698,
				6619234, 6684770, 3145827, 3211363, 3276899, 3342435, 3407971, 3473507, 3539043, 3604579,
				3670115, 3735651, 6357091, 6422627, 6488163, 6553699, 6619235, 6684771, 3145828, 3211364,
				3276900, 3342436, 3407972, 3473508, 3539044, 3604580, 3670116, 3735652, 6357092, 6422628,
				6488164, 6553700, 6619236, 6684772, 3145829, 3211365, 3276901, 3342437, 3407973, 3473509,
				3539045, 3604581, 3670117, 3735653, 6357093, 6422629, 6488165, 6553701, 6619237, 6684773,
				3145830, 3211366, 3276902, 3342438, 3407974, 3473510, 3539046, 3604582, 3670118, 3735654,
				6357094, 6422630, 6488166, 6553702, 6619238, 6684774
			};
			static readonly uint* byteToHexUnsafe = (uint*) GCHandle.Alloc (byteToHex, GCHandleType.Pinned).AddrOfPinnedObject ();

			public static unsafe string ToHex (byte [] bytes)
			{
				var result = new char [bytes.Length * 2];
				fixed (byte* bytesp = bytes)
				fixed (char* resultp = result) {
					uint* bp = (uint*) resultp;
					for (int i = 0; i < bytes.Length; i++) {
						bp [i] = byteToHexUnsafe [bytesp [i]];
					}
				}
				return new string (result);
			}
		}
	}
}
