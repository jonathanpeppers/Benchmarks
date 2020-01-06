# Benchmarks

Benchmarkings for improvements around Xamarin.Forms `Binding`:
```
BenchmarkDotNet=v0.11.3, OS=Windows 10.0.18362
Intel Core i9-9900K CPU 3.60GHz, 1 CPU, 16 logical and 8 physical cores
  [Host]     : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.8.4075.0
  DefaultJob : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.8.4075.0


         Method |       Mean |      Error |     StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
--------------- |-----------:|-----------:|-----------:|------------:|------------:|------------:|--------------------:|
    CtorSingle2 |   122.6 ns |  0.5047 ns |  0.4721 ns |      0.0701 |           - |           - |               369 B |
    CtorSingle1 |   156.3 ns |  0.5229 ns |  0.4636 ns |      0.0801 |           - |           - |               421 B |
  CtorMultiple2 |   337.7 ns |  2.5286 ns |  2.3652 ns |      0.1631 |           - |           - |               857 B |
  CtorMultiple1 |   435.4 ns |  1.0472 ns |  0.9796 ns |      0.1936 |           - |           - |              1017 B |
         Clone2 |   667.0 ns |  2.5739 ns |  2.0095 ns |      0.3262 |      0.0010 |           - |              1715 B |
         Clone1 |   890.6 ns |  9.1459 ns |  8.5551 ns |      0.3872 |      0.0010 |           - |              2035 B |
   ApplySingle2 | 1,512.4 ns |  9.0686 ns |  7.0802 ns |      0.0992 |           - |           - |               521 B |
   ApplySingle1 | 1,655.3 ns | 31.1960 ns | 35.9253 ns |      0.1087 |           - |           - |               573 B |
 ApplyMultiple2 | 4,501.1 ns | 21.8014 ns | 20.3930 ns |      0.2594 |           - |           - |              1394 B |
 ApplyMultiple1 | 4,510.7 ns | 25.4709 ns | 22.5793 ns |      0.2899 |           - |           - |              1554 B |
```

Results on MacOS:
```
BenchmarkDotNet=v0.11.3, OS=macOS Mojave 10.14.6 (18G95) [Darwin 18.7.0]
Intel Core i7-6567U CPU 3.30GHz (Skylake), 1 CPU, 4 logical and 2 physical cores
  [Host]     : Mono 6.6.0.155 (2019-08/296a9afdb24 Thu), 64bit 
  DefaultJob : Mono 6.6.0.155 (2019-08/296a9afdb24 Thu), 64bit 
         Method |        Mean |      Error |     StdDev |      Median | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
--------------- |------------:|-----------:|-----------:|------------:|------------:|------------:|------------:|--------------------:|
    CtorSingle2 |    355.9 ns |   2.600 ns |   2.432 ns |    356.1 ns |      0.1950 |           - |           - |                   - |
    CtorSingle1 |    447.0 ns |  28.825 ns |  29.601 ns |    432.4 ns |      0.2203 |           - |           - |                   - |
  CtorMultiple2 |  1,316.6 ns |   7.092 ns |   6.287 ns |  1,316.6 ns |      0.4292 |           - |           - |                   - |
  CtorMultiple1 |  1,611.0 ns |  77.914 ns | 109.225 ns |  1,560.9 ns |      0.4997 |           - |           - |                   - |
         Clone2 |  2,670.6 ns |  38.075 ns |  33.753 ns |  2,655.7 ns |      0.8583 |           - |           - |                   - |
         Clone1 |  3,154.3 ns |  15.010 ns |  14.040 ns |  3,151.7 ns |      0.9995 |           - |           - |                   - |
   ApplySingle2 |  8,065.5 ns |  87.537 ns |  77.599 ns |  8,071.3 ns |      0.3052 |           - |           - |                   - |
   ApplySingle1 |  8,555.2 ns | 183.373 ns | 508.126 ns |  8,268.9 ns |      0.3357 |           - |           - |                   - |
 ApplyMultiple2 | 28,933.1 ns | 252.589 ns | 236.272 ns | 28,937.1 ns |      0.8850 |           - |           - |                   - |
 ApplyMultiple1 | 29,388.9 ns | 269.263 ns | 251.868 ns | 29,296.0 ns |      0.9766 |           - |           - |                   - |
```