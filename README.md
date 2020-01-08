# Benchmarks

Initial benchmarks for Assembly.GetName().Name vs Assembly.FullName and Substring:
```
BenchmarkDotNet=v0.11.3, OS=Windows 10.0.18362
Intel Core i9-9900K CPU 3.60GHz, 1 CPU, 16 logical and 8 physical cores
  [Host]     : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.8.4075.0
  DefaultJob : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.8.4075.0


          Method |      Mean |     Error |    StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
---------------- |----------:|----------:|----------:|------------:|------------:|------------:|--------------------:|
      Substring2 |  92.05 ns | 0.1823 ns | 0.1616 ns |      0.0168 |           - |           - |                88 B |
       Substring | 130.91 ns | 0.7452 ns | 0.6971 ns |      0.0167 |           - |           - |                88 B |
      Substring3 | 196.83 ns | 0.5788 ns | 0.5414 ns |      0.0167 |           - |           - |                88 B |
 AssemblyGetName | 949.68 ns | 1.9551 ns | 1.8288 ns |      0.0896 |           - |           - |               473 B |
```

Results on MacOS:
```
TODO
```