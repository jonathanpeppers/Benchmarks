using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Diagnostics;
using System.IO;
using Xamarin.Android.Build.Tests;
using Xamarin.Android.Tasks;

namespace Benchmarks
{
	[Orderer (SummaryOrderPolicy.FastestToSlowest)]
	[MemoryDiagnoser]
	public class Benchmarks
	{
		MockBuildEngine engine = new MockBuildEngine (TextWriter.Null);
		ITaskItem [] assemblies = new []
		{
			new TaskItem (@"C:\src\SmartHotel\Source\SmartHotel.Clients\SmartHotel.Clients.Android\bin\Debug\SmartHotel.Clients.Android.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\acr.userdialogs\7.0.4\lib\monoandroid90\Acr.UserDialogs.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\andhud\1.4.1\lib\MonoAndroid81\AndHUD.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\autofac\4.9.2\lib\netstandard2.0\Autofac.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\carouselview.formsplugin\5.2.0\lib\MonoAndroid\CarouselView.FormsPlugin.Abstractions.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\carouselview.formsplugin\5.2.0\lib\MonoAndroid\CarouselView.FormsPlugin.Android.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.verticalviewpager\1.0.1\lib\MonoAndroid\Com.Android.DeskClock.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.circlepageindicator\1.0.2\lib\MonoAndroid\Com.ViewPagerIndicator.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.ffimageloading\2.4.5.880-pre\lib\MonoAndroid10\FFImageLoading.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.ffimageloading.forms\2.4.5.880-pre\lib\MonoAndroid10\FFImageLoading.Forms.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.ffimageloading.forms\2.4.5.880-pre\lib\MonoAndroid10\FFImageLoading.Forms.Platform.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.ffimageloading\2.4.5.880-pre\lib\MonoAndroid10\FFImageLoading.Platform.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.forms\3.6.0.293080\lib\MonoAndroid90\FormsViewGroup.dll"),
			new TaskItem (@"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\ReferenceAssemblies\Microsoft\Framework\MonoAndroid\v1.0\Java.Interop.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\microcharts\0.7.1\lib\MonoAndroid10\Microcharts.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\microcharts\0.7.1\lib\MonoAndroid10\Microcharts.Droid.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\microcharts.forms\0.7.1\lib\netstandard1.4\Microcharts.Forms.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\microsoft.appcenter.analytics\1.14.0\lib\MonoAndroid403\Microsoft.AppCenter.Analytics.Android.Bindings.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\microsoft.appcenter.analytics\1.14.0\lib\MonoAndroid403\Microsoft.AppCenter.Analytics.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\microsoft.appcenter\1.14.0\lib\MonoAndroid403\Microsoft.AppCenter.Android.Bindings.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\microsoft.appcenter.crashes\1.14.0\lib\MonoAndroid403\Microsoft.AppCenter.Crashes.Android.Bindings.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\microsoft.appcenter.crashes\1.14.0\lib\MonoAndroid403\Microsoft.AppCenter.Crashes.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\microsoft.appcenter.distribute\1.14.0\lib\MonoAndroid403\Microsoft.AppCenter.Distribute.Android.Bindings.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\microsoft.appcenter.distribute\1.14.0\lib\MonoAndroid403\Microsoft.AppCenter.Distribute.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\microsoft.appcenter\1.14.0\lib\MonoAndroid403\Microsoft.AppCenter.dll"),
			new TaskItem (@"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\ReferenceAssemblies\Microsoft\Framework\MonoAndroid\v1.0\Microsoft.CSharp.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\microsoft.identity.client\1.0.304142221-alpha\lib\MonoAndroid10\Microsoft.Identity.Client.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\microsoft.identity.client\1.0.304142221-alpha\lib\MonoAndroid10\Microsoft.Identity.Client.Platform.dll"),
			new TaskItem (@"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\ReferenceAssemblies\Microsoft\Framework\MonoAndroid\v10.0\Mono.Android.dll"),
			new TaskItem (@"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\ReferenceAssemblies\Microsoft\Framework\MonoAndroid\v1.0\mscorlib.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\refractored.mvvmhelpers\1.3.0\lib\netstandard1.0\MvvmHelpers.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\newtonsoft.json\12.0.1\lib\netstandard2.0\Newtonsoft.Json.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\rg.plugins.popup\1.1.5.188\lib\MonoAndroid\Rg.Plugins.Popup.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\rg.plugins.popup\1.1.5.188\lib\MonoAndroid\Rg.Plugins.Popup.Droid.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\skiasharp\1.68.0\lib\MonoAndroid\SkiaSharp.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\skiasharp.views\1.68.0\lib\MonoAndroid\SkiaSharp.Views.Android.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\skiasharp.views.forms\1.68.0\lib\MonoAndroid\SkiaSharp.Views.Forms.dll"),
			new TaskItem (@"C:\src\SmartHotel\Source\SmartHotel.Clients\SmartHotel.Clients\bin\Debug\netstandard2.0\SmartHotel.Clients.Core.dll"),
			new TaskItem (@"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\ReferenceAssemblies\Microsoft\Framework\MonoAndroid\v1.0\System.Core.dll"),
			new TaskItem (@"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\ReferenceAssemblies\Microsoft\Framework\MonoAndroid\v1.0\System.dll"),
			new TaskItem (@"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\ReferenceAssemblies\Microsoft\Framework\MonoAndroid\v1.0\System.IO.Compression.dll"),
			new TaskItem (@"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\ReferenceAssemblies\Microsoft\Framework\MonoAndroid\v1.0\System.Net.Http.dll"),
			new TaskItem (@"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\ReferenceAssemblies\Microsoft\Framework\MonoAndroid\v1.0\System.Numerics.dll"),
			new TaskItem (@"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\ReferenceAssemblies\Microsoft\Framework\MonoAndroid\v1.0\System.Numerics.Vectors.dll"),
			new TaskItem (@"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\ReferenceAssemblies\Microsoft\Framework\MonoAndroid\v1.0\Facades\System.Runtime.dll"),
			new TaskItem (@"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\ReferenceAssemblies\Microsoft\Framework\MonoAndroid\v1.0\System.Xml.dll"),
			new TaskItem (@"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\ReferenceAssemblies\Microsoft\Framework\MonoAndroid\v1.0\System.Xml.Linq.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamanimation\1.2.0\lib\netstandard2.0\Xamanimation.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.arch.core.common\1.1.1.1\lib\monoandroid90\Xamarin.Android.Arch.Core.Common.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.arch.core.runtime\1.1.1.1\lib\monoandroid90\Xamarin.Android.Arch.Core.Runtime.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.arch.lifecycle.common\1.1.1.1\lib\monoandroid90\Xamarin.Android.Arch.Lifecycle.Common.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.arch.lifecycle.livedata.core\1.1.1.1\lib\monoandroid90\Xamarin.Android.Arch.Lifecycle.LiveData.Core.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.arch.lifecycle.livedata\1.1.1.1\lib\monoandroid90\Xamarin.Android.Arch.Lifecycle.LiveData.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.arch.lifecycle.runtime\1.1.1.1\lib\monoandroid90\Xamarin.Android.Arch.Lifecycle.Runtime.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.arch.lifecycle.viewmodel\1.1.1.1\lib\monoandroid90\Xamarin.Android.Arch.Lifecycle.ViewModel.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.support.animated.vector.drawable\28.0.0.1\lib\monoandroid90\Xamarin.Android.Support.Animated.Vector.Drawable.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.support.annotations\28.0.0.1\lib\monoandroid90\Xamarin.Android.Support.Annotations.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.support.asynclayoutinflater\28.0.0.1\lib\monoandroid90\Xamarin.Android.Support.AsyncLayoutInflater.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.support.collections\28.0.0.1\lib\monoandroid90\Xamarin.Android.Support.Collections.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.support.compat\28.0.0.1\lib\monoandroid90\Xamarin.Android.Support.Compat.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.support.coordinaterlayout\28.0.0.1\lib\monoandroid90\Xamarin.Android.Support.CoordinaterLayout.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.support.core.ui\28.0.0.1\lib\monoandroid90\Xamarin.Android.Support.Core.UI.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.support.core.utils\28.0.0.1\lib\monoandroid90\Xamarin.Android.Support.Core.Utils.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.support.cursoradapter\28.0.0.1\lib\monoandroid90\Xamarin.Android.Support.CursorAdapter.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.support.customtabs\28.0.0.1\lib\monoandroid90\Xamarin.Android.Support.CustomTabs.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.support.customview\28.0.0.1\lib\monoandroid90\Xamarin.Android.Support.CustomView.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.support.design\28.0.0.1\lib\monoandroid90\Xamarin.Android.Support.Design.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.support.documentfile\28.0.0.1\lib\monoandroid90\Xamarin.Android.Support.DocumentFile.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.support.drawerlayout\28.0.0.1\lib\monoandroid90\Xamarin.Android.Support.DrawerLayout.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.support.fragment\28.0.0.1\lib\monoandroid90\Xamarin.Android.Support.Fragment.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.support.interpolator\28.0.0.1\lib\monoandroid90\Xamarin.Android.Support.Interpolator.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.support.loader\28.0.0.1\lib\monoandroid90\Xamarin.Android.Support.Loader.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.support.localbroadcastmanager\28.0.0.1\lib\monoandroid90\Xamarin.Android.Support.LocalBroadcastManager.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.support.media.compat\28.0.0.1\lib\monoandroid90\Xamarin.Android.Support.Media.Compat.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.support.print\28.0.0.1\lib\monoandroid90\Xamarin.Android.Support.Print.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.support.slidingpanelayout\28.0.0.1\lib\monoandroid90\Xamarin.Android.Support.SlidingPaneLayout.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.support.swiperefreshlayout\28.0.0.1\lib\monoandroid90\Xamarin.Android.Support.SwipeRefreshLayout.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.support.transition\28.0.0.1\lib\monoandroid90\Xamarin.Android.Support.Transition.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.support.v4\28.0.0.1\lib\monoandroid90\Xamarin.Android.Support.v4.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.support.v7.appcompat\28.0.0.1\lib\monoandroid90\Xamarin.Android.Support.v7.AppCompat.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.support.v7.cardview\28.0.0.1\lib\monoandroid90\Xamarin.Android.Support.v7.CardView.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.support.v7.mediarouter\28.0.0.1\lib\monoandroid90\Xamarin.Android.Support.v7.MediaRouter.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.support.v7.palette\28.0.0.1\lib\monoandroid90\Xamarin.Android.Support.v7.Palette.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.support.v7.recyclerview\28.0.0.1\lib\monoandroid90\Xamarin.Android.Support.v7.RecyclerView.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.support.vector.drawable\28.0.0.1\lib\monoandroid90\Xamarin.Android.Support.Vector.Drawable.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.support.versionedparcelable\28.0.0.1\lib\monoandroid90\Xamarin.Android.Support.VersionedParcelable.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.android.support.viewpager\28.0.0.1\lib\monoandroid90\Xamarin.Android.Support.ViewPager.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.essentials\1.1.0\lib\monoandroid90\Xamarin.Essentials.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.forms\3.6.0.293080\lib\MonoAndroid90\Xamarin.Forms.Core.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.forms.maps\3.6.0.293080\lib\MonoAndroid90\Xamarin.Forms.Maps.Android.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.forms.maps\3.6.0.293080\lib\MonoAndroid90\Xamarin.Forms.Maps.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.forms\3.6.0.293080\lib\MonoAndroid90\Xamarin.Forms.Platform.Android.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.forms\3.6.0.293080\lib\MonoAndroid90\Xamarin.Forms.Platform.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.forms\3.6.0.293080\lib\MonoAndroid90\Xamarin.Forms.Xaml.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.googleplayservices.base\60.1142.1\lib\MonoAndroid80\Xamarin.GooglePlayServices.Base.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.googleplayservices.basement\60.1142.1\lib\MonoAndroid80\Xamarin.GooglePlayServices.Basement.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.googleplayservices.location\60.1142.1\lib\MonoAndroid80\Xamarin.GooglePlayServices.Location.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.googleplayservices.maps\60.1142.1\lib\MonoAndroid80\Xamarin.GooglePlayServices.Maps.dll"),
			new TaskItem (@"C:\Users\jopepper\.nuget\packages\xamarin.googleplayservices.tasks\60.1142.1\lib\MonoAndroid80\Xamarin.GooglePlayServices.Tasks.dll"),
		};
		string projectFile = @"C:\src\SmartHotel\Source\SmartHotel.Clients\SmartHotel.Clients.Android\SmartHotel.Clients.Android.csproj";
		string targetFrameworkVersion = "v10.0";
		string projectAssetsFile = @"C:\src\SmartHotel\Source\SmartHotel.Clients\SmartHotel.Clients.Android\obj\project.assets.json";
		string targetMoniker = "MonoAndroid,Version=v10.0";
		string referenceAssembliesDirectory = @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\ReferenceAssemblies\Microsoft\Framework\MonoAndroid\v10.0\;C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\ReferenceAssemblies\Microsoft\Framework\MonoAndroid\v9.0\;C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\ReferenceAssemblies\Microsoft\Framework\MonoAndroid\v8.1\;C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\ReferenceAssemblies\Microsoft\Framework\MonoAndroid\v8.0\;C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\ReferenceAssemblies\Microsoft\Framework\MonoAndroid\v7.1\;C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\ReferenceAssemblies\Microsoft\Framework\MonoAndroid\v7.0\;C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\ReferenceAssemblies\Microsoft\Framework\MonoAndroid\v6.0\;C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\ReferenceAssemblies\Microsoft\Framework\MonoAndroid\v5.1\;C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\ReferenceAssemblies\Microsoft\Framework\MonoAndroid\v5.0\;C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\ReferenceAssemblies\Microsoft\Framework\MonoAndroid\v4.4.87\;C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\ReferenceAssemblies\Microsoft\Framework\MonoAndroid\v4.4\;C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\ReferenceAssemblies\Microsoft\Framework\MonoAndroid\v1.0\;;;C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\ReferenceAssemblies\Microsoft\Framework\MonoAndroid\v1.0\Facades\";

		[GlobalSetup]
		public void Setup()
		{
			MonoAndroidHelper.RefreshSupportedVersions (new[] { @"C:\src\xamarin-android\bin\Debug\lib\xamarin.android\xbuild-frameworks\MonoAndroid\v1.0\" });
		}

		[Benchmark (Baseline = true)]
		public void ResolveAssemblies ()
		{
			var task = new ResolveAssemblies {
				BuildEngine = engine,
				Assemblies = assemblies,
				LinkMode = "None",
				ProjectFile = projectFile,
				TargetFrameworkVersion = targetFrameworkVersion,
				ProjectAssetFile = projectAssetsFile,
				TargetMoniker = targetMoniker,
				ReferenceAssembliesDirectory = referenceAssembliesDirectory,
			};
			var result = task.Execute ();
			Debug.Assert (result);
		}

		[Benchmark]
		public void ResolveAssemblies2 ()
		{
			var task = new ResolveAssemblies2 {
				BuildEngine = engine,
				Assemblies = assemblies,
				ProjectFile = projectFile,
				TargetFrameworkVersion = targetFrameworkVersion,
				ProjectAssetFile = projectAssetsFile,
				TargetMoniker = targetMoniker,
				ReferenceAssembliesDirectory = referenceAssembliesDirectory,
			};
			var result = task.Execute ();
			Debug.Assert (result);
		}
	}
}
