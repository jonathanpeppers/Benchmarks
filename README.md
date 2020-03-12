# Benchmarks

Benchmarks for `ToHexString` equivalents.

These came from various implementations:

* https://stackoverflow.com/questions/311165/how-do-you-convert-a-byte-array-to-a-hexadecimal-string-and-vice-versa/24343727#24343727
* https://github.com/xamarin/xamarin-android/blob/aaea55ff99ae033682dc8eef0817ddbe007aa542/src/Xamarin.Android.Build.Tasks/Utilities/Files.cs#L404-L415
* https://github.com/xamarin/Xamarin.Forms/blob/abe544ed2b1fe9c38ea1295aff1a03d1149e1f8f/Xamarin.Forms.Platform.Tizen/TizenPlatformServices.cs#L161

```
BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18362
Intel Core i9-9900K CPU 3.60GHz (Coffee Lake), 1 CPU, 16 logical and 8 physical cores
  [Host]     : .NET Framework 4.8 (4.8.4121.0), X86 LegacyJIT
  DefaultJob : .NET Framework 4.8 (4.8.4121.0), X86 LegacyJIT


|              Method |     Mean |    Error |   StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------------- |---------:|---------:|---------:|-------:|------:|------:|----------:|
| StackOverflowUnsafe | 42.41 ns | 0.228 ns | 0.213 ns | 0.0297 |     - |     - |     156 B |
|       StackOverflow | 46.89 ns | 0.152 ns | 0.118 ns | 0.0297 |     - |     - |     156 B |
|          TizenToHex | 58.35 ns | 0.427 ns | 0.399 ns | 0.0297 |     - |     - |     156 B |
|       XAToHexString | 59.84 ns | 0.121 ns | 0.101 ns | 0.0297 |     - |     - |     156 B |
```

Results on MacOS:

TODO