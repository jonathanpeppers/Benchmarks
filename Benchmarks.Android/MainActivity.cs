using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using System;
using System.IO;

namespace Benchmarks.Android
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            FindViewById<Button>(Resource.Id.go).Click += OnClick;
        }

        void OnClick(object sender, EventArgs e)
        {
            var logger = new Logger();
            var config = new ManualConfig { ArtifactsPath = Path.GetTempPath() };
            config.AddLogger(logger);
            var summary = BenchmarkRunner.Run<PropertyChangedExtensions>(config.WithOptions(ConfigOptions.DisableLogFile));
            FindViewById<TextView>(Resource.Id.results).Text = logger.ToString();
        }

        class Logger : ILogger
        {
            StringWriter writer = new StringWriter();

            public string Id => "In-Memory Logger";

            public int Priority => 1;

            public void Flush() => writer.Flush();

            public void Write(LogKind logKind, string text) => writer.Write(text);

            public void WriteLine() => writer.WriteLine();

            public void WriteLine(LogKind logKind, string text) => writer.WriteLine(text);

            public override string ToString() => writer.ToString();
        }
    }
}