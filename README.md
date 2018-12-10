# Benchmarks

Initial benchmarks for Mono.Cecil vs System.Reflection.Metadata:

```
// * Summary *

BenchmarkDotNet=v0.11.3, OS=Windows 10.0.17763.134 (1809/October2018Update/Redstone5)
Intel Core i7-7600U CPU 2.80GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
  [Host]     : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.3190.0
  DefaultJob : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.3190.0


                                              Method |      Mean |     Error |    StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
---------------------------------------------------- |----------:|----------:|----------:|------------:|------------:|------------:|--------------------:|
 'System.Reflection.Metadata with default settings.' |  50.81 ms |  1.051 ms |  1.366 ms |   8363.6364 |           - |           - |            16.91 MB |
                 'Mono.Cecil with default settings.' | 489.72 ms |  8.764 ms |  7.318 ms |  16000.0000 |  11000.0000 |   3000.0000 |           103.41 MB |
     'Mono.Cecil with ReadingMode.Deferred setting.' | 502.03 ms | 10.815 ms | 10.116 ms |  16000.0000 |  11000.0000 |   3000.0000 |           103.41 MB |
            'Mono.Cecil with InMemory=True setting.' | 538.11 ms | 10.681 ms | 11.429 ms |  18000.0000 |  11000.0000 |   3000.0000 |           157.73 MB |

// * Hints *
Outliers
  Cecil.'System.Reflection.Metadata with default settings.': Default -> 3 outliers were removed
  Cecil.'Mono.Cecil with default settings.': Default                 -> 2 outliers were removed

// * Legends *
  Mean                : Arithmetic mean of all measurements
  Error               : Half of 99.9% confidence interval
  StdDev              : Standard deviation of all measurements
  Gen 0/1k Op         : GC Generation 0 collects per 1k Operations
  Gen 1/1k Op         : GC Generation 1 collects per 1k Operations
  Gen 2/1k Op         : GC Generation 2 collects per 1k Operations
  Allocated Memory/Op : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
  1 ms                : 1 Millisecond (0.001 sec)

// ***** BenchmarkRunner: End *****
Run time: 00:01:12 (72.56 sec), executed benchmarks: 4
```