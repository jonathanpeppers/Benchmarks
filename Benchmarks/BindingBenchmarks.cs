using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Benchmarks
{
	[Orderer(SummaryOrderPolicy.FastestToSlowest)]
	[MemoryDiagnoser]
	public class BindingBenchmarks
	{
		class MyViewModel : INotifyPropertyChanged
		{
			public event PropertyChangedEventHandler PropertyChanged;

			void OnPropertyChanged ([CallerMemberName] string propertyName = null)
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			}

			MyViewModel child;

			public MyViewModel Child
			{
				get => child;
				set {
					child = value;
					OnPropertyChanged();
				}
			}

			string name;

			public string Name {
				get => name;
				set {
					name = value;
					OnPropertyChanged();
				}
			}
		}

		class MyObject : BindableObject
		{
			public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(MyObject));

			public string Text {
				get => (string)GetValue(TextProperty);
				set => SetValue(TextProperty, value);
			}
		}

		BindableObject bindableObject = new MyObject
		{
			BindingContext = new MyViewModel
			{
				Name = "Foo",
				Child = new MyViewModel
				{
					Child = new MyViewModel
					{
						Child = new MyViewModel
						{
							Name = "Bar"
						}
					}
				}
			}
		};

		[Benchmark]
		public void CtorSingle1()
		{
			new Binding("Name");
		}

		[Benchmark]
		public void CtorSingle2()
		{
			new Binding2("Name");
		}

		[Benchmark]
		public void CtorMultiple1()
		{
			new Binding("Child.Child.Child.Name");
		}

		[Benchmark]
		public void CtorMultiple2()
		{
			new Binding2("Child.Child.Child.Name");
		}

		[Benchmark]
		public void ApplySingle1()
		{
			bindableObject.SetBinding(MyObject.TextProperty, new Binding("Name"));
		}

		[Benchmark]
		public void ApplySingle2()
		{
			bindableObject.SetBinding(MyObject.TextProperty, new Binding2("Name"));
		}

		[Benchmark]
		public void ApplyMultiple1()
		{
			bindableObject.SetBinding(MyObject.TextProperty, new Binding("Child.Child.Child.Name"));
		}

		[Benchmark]
		public void ApplyMultiple2()
		{
			bindableObject.SetBinding(MyObject.TextProperty, new Binding2("Child.Child.Child.Name"));
		}

		[Benchmark]
		public void Clone1()
		{
			new Binding("Child.Child.Child.Name").Clone();
		}

		[Benchmark]
		public void Clone2()
		{
			new Binding2("Child.Child.Child.Name").Clone();
		}
	}
}
