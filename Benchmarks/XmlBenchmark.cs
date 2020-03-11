using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using System.IO;
using System.Xml;

namespace Benchmarks
{
	[Orderer (SummaryOrderPolicy.FastestToSlowest)]
	[MemoryDiagnoser]
	public class XmlBenchmark
	{
		readonly string path;

		public XmlBenchmark ()
		{
			path = Path.Combine (Path.GetDirectoryName (GetType ().Assembly.Location), "X2009Primitives.xaml");
		}

		[Benchmark]
		public void XmlDocument ()
		{
			IsXamlXmlDocument (out _);
		}

		[Benchmark]
		public void XmlReader ()
		{
			IsXamlXmlReader (out _);
		}

		bool IsXamlXmlDocument (out string classname)
		{
			classname = null;
			using (var resourceStream = File.OpenRead (path)) {
				var xmlDoc = new XmlDocument ();
				xmlDoc.Load (resourceStream);

				var nsmgr = new XmlNamespaceManager (xmlDoc.NameTable);

				var root = xmlDoc.SelectSingleNode ("/*", nsmgr);
				if (root == null)
					return false;

				var rootClass = root.Attributes ["Class", XamlParser.X2006Uri] ??
					root.Attributes ["Class", XamlParser.X2009Uri];
				if (rootClass != null) {
					classname = rootClass.Value;
					return true;
				}

				return false;
			}
		}

		bool IsXamlXmlReader (out string classname)
		{
			classname = null;
			using (var resourceStream = File.OpenRead (path))
			using (var reader = System.Xml.XmlReader.Create (resourceStream)) {
				// Read to the first Element
				while (reader.Read () && reader.NodeType != XmlNodeType.Element) ;

				if (reader.NodeType != XmlNodeType.Element)
					return false;

				classname = reader.GetAttribute ("Class", XamlParser.X2009Uri) ??
					reader.GetAttribute ("Class", XamlParser.X2006Uri);
				if (classname != null)
					return true;

				return false;
			}
		}

		static class XamlParser
		{
			public const string XFUri = "http://xamarin.com/schemas/2014/forms";
			public const string XFDesignUri = "http://xamarin.com/schemas/2014/forms/design";
			public const string X2006Uri = "http://schemas.microsoft.com/winfx/2006/xaml";
			public const string X2009Uri = "http://schemas.microsoft.com/winfx/2009/xaml";
			public const string McUri = "http://schemas.openxmlformats.org/markup-compatibility/2006";
		}
	}
}
