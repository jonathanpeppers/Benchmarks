# Benchmarks

Initial benchmarks for Mono.Cecil:

```
// * Summary *

BenchmarkDotNet=v0.11.3, OS=Windows 10.0.17763.134 (1809/October2018Update/Redstone5)
Intel Core i7-7600U CPU 2.80GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
  [Host]     : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.3190.0
  DefaultJob : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.3190.0


   Method |      Mean |    Error |    StdDev |
--------- |----------:|---------:|----------:|
 Defaults |  74.45 ms | 1.026 ms | 0.8564 ms |
 InMemory | 114.33 ms | 2.251 ms | 2.1054 ms |

// * Hints *
Outliers
  Cecil.Defaults: Default -> 2 outliers were removed
  Cecil.InMemory: Default -> 1 outlier  was  removed

// * Legends *
  Mean   : Arithmetic mean of all measurements
  Error  : Half of 99.9% confidence interval
  StdDev : Standard deviation of all measurements
  1 ms   : 1 Millisecond (0.001 sec)

// ***** BenchmarkRunner: End *****
Run time: 00:00:33 (33.18 sec), executed benchmarks: 2
```