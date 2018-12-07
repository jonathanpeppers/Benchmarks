# Benchmarks

Initial benchmarks for Mono.Cecil:

```
// * Summary *

BenchmarkDotNet=v0.11.3, OS=Windows 10.0.17763.134 (1809/October2018Update/Redstone5)
Intel Core i7-7600U CPU 2.80GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
  [Host]     : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.3190.0
  DefaultJob : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.3190.0


   Method |     Mean |     Error |    StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
--------- |---------:|----------:|----------:|------------:|------------:|------------:|--------------------:|
 Defaults | 518.8 ms | 10.306 ms | 15.425 ms |  16000.0000 |  11000.0000 |   3000.0000 |           103.41 MB |
 InMemory | 541.5 ms |  7.212 ms |  6.746 ms |  16000.0000 |  11000.0000 |   3000.0000 |           157.74 MB |

// * Hints *
Outliers
  Cecil.Defaults: Default -> 4 outliers were removed

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
Run time: 00:00:39 (39.12 sec), executed benchmarks: 2
```