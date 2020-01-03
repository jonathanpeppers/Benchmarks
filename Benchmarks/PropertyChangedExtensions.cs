using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Benchmarks
{
	[Orderer(SummaryOrderPolicy.FastestToSlowest)]
	[MemoryDiagnoser]
	public class PropertyChangedExtensions : BindableObject
	{
		PropertyChangedEventArgs args = new PropertyChangedEventArgs("Banana");
		BindableProperty bp1 = BindableProperty.Create("bp1", typeof(int), typeof (PropertyChangedExtensions));
		BindableProperty bp2 = BindableProperty.Create("bp2", typeof(int), typeof(PropertyChangedExtensions));
		BindableProperty bp3 = BindableProperty.Create("bp3", typeof(int), typeof(PropertyChangedExtensions));
		BindableProperty bp4 = BindableProperty.Create("bp4", typeof(int), typeof(PropertyChangedExtensions));
		BindableProperty bp5 = BindableProperty.Create("bp5", typeof(int), typeof(PropertyChangedExtensions));

		[Benchmark]
		public void Is1()
		{
			PropertyChangedEventArgsExtensions1.Is(args, bp1);
			PropertyChangedEventArgsExtensions1.Is(args, bp2);
			PropertyChangedEventArgsExtensions1.Is(args, bp3);
			PropertyChangedEventArgsExtensions1.Is(args, bp4);
			PropertyChangedEventArgsExtensions1.Is(args, bp5);
		}

		[Benchmark]
		public void Is2()
		{
			PropertyChangedEventArgsExtensions2.Is(args, bp1);
			PropertyChangedEventArgsExtensions2.Is(args, bp2);
			PropertyChangedEventArgsExtensions2.Is(args, bp3);
			PropertyChangedEventArgsExtensions2.Is(args, bp4);
			PropertyChangedEventArgsExtensions2.Is(args, bp5);
		}

		[Benchmark]
		public void IsOneOf1()
		{
			PropertyChangedEventArgsExtensions1.IsOneOf(args, bp1, bp2, bp3, bp4, bp5);
		}

		[Benchmark]
		public void IsOneOf2()
		{
			PropertyChangedEventArgsExtensions2.IsOneOf(args, bp1, bp2, bp3, bp4, bp5);
		}
	}

	internal static class PropertyChangedEventArgsExtensions1
	{
		public static bool Is(this PropertyChangedEventArgs args, BindableProperty property)
		{
			return args.PropertyName == property.PropertyName;
		}

		public static bool IsOneOf(this PropertyChangedEventArgs args, params BindableProperty[] properties)
		{
			foreach (BindableProperty property in properties)
			{
				if (args.PropertyName == property.PropertyName)
				{
					return true;
				}
			}

			return false;
		}
	}

	internal static class PropertyChangedEventArgsExtensions2
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Is(this PropertyChangedEventArgs args, BindableProperty property)
		{
			return args.PropertyName == property.PropertyName;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsOneOf(this PropertyChangedEventArgs args, BindableProperty p0, BindableProperty p1)
		{
			return args.PropertyName == p0.PropertyName ||
				args.PropertyName == p1.PropertyName;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsOneOf(this PropertyChangedEventArgs args, BindableProperty p0, BindableProperty p1, BindableProperty p2)
		{
			return args.PropertyName == p0.PropertyName ||
				args.PropertyName == p1.PropertyName ||
				args.PropertyName == p2.PropertyName;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsOneOf(this PropertyChangedEventArgs args, BindableProperty p0, BindableProperty p1, BindableProperty p2, BindableProperty p3)
		{
			return args.PropertyName == p0.PropertyName ||
				args.PropertyName == p1.PropertyName ||
				args.PropertyName == p2.PropertyName ||
				args.PropertyName == p3.PropertyName;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsOneOf(this PropertyChangedEventArgs args, BindableProperty p0, BindableProperty p1, BindableProperty p2, BindableProperty p3, BindableProperty p4)
		{
			return args.PropertyName == p0.PropertyName ||
				args.PropertyName == p1.PropertyName ||
				args.PropertyName == p2.PropertyName ||
				args.PropertyName == p3.PropertyName ||
				args.PropertyName == p4.PropertyName;
		}
	}
}
