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
|  GrendelStaticArray | 39.90 ns | 0.188 ns | 0.167 ns | 0.0297 |     - |     - |     156 B |
| StackOverflowUnsafe | 41.56 ns | 0.413 ns | 0.344 ns | 0.0297 |     - |     - |     156 B |
|       StackOverflow | 46.32 ns | 0.708 ns | 0.662 ns | 0.0297 |     - |     - |     156 B |
|          TizenToHex | 56.30 ns | 0.386 ns | 0.361 ns | 0.0297 |     - |     - |     156 B |
|       XAToHexString | 57.92 ns | 0.868 ns | 0.812 ns | 0.0297 |     - |     - |     156 B |
```

Results on MacOS:

TODO