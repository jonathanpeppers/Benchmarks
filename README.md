# Benchmarks

Comparison of EmbeddedResource usage SRM vs Mono.Cecil:
```
// * Summary *

BenchmarkDotNet=v0.11.3, OS=Windows 10.0.17763.253 (1809/October2018Update/Redstone5)
Intel Core i7-7600U CPU 2.80GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
  [Host]     : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.3260.0
  DefaultJob : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.3260.0


                     Method |     Mean |     Error |    StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
--------------------------- |---------:|----------:|----------:|------------:|------------:|------------:|--------------------:|
 System.Reflection.Metadata | 11.67 ms | 0.1618 ms | 0.1434 ms |   1984.3750 |    656.2500 |    656.2500 |             5.52 MB |
                 Mono.Cecil | 34.04 ms | 0.4388 ms | 0.4105 ms |   2066.6667 |   1600.0000 |    933.3333 |            39.95 MB |

// * Hints *
Outliers
  Cecil.System.Reflection.Metadata: Default -> 1 outlier  was  removed

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
Run time: 00:00:43 (43.61 sec), executed benchmarks: 2
```