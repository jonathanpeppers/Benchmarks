# Benchmarks

Initial benchmarks for Mono.Cecil:

```
// * Summary *

BenchmarkDotNet=v0.11.3, OS=Windows 10.0.17763.134 (1809/October2018Update/Redstone5)
Intel Core i7-7600U CPU 2.80GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
  [Host]     : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.3190.0
  DefaultJob : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.3190.0


   Method |     Mean |     Error |    StdDev |
--------- |---------:|----------:|----------:|
 Defaults | 495.7 ms |  5.646 ms |  5.005 ms |
 InMemory | 537.9 ms | 10.245 ms | 11.388 ms |

// * Hints *
Outliers
  Cecil.Defaults: Default -> 1 outlier  was  removed

// * Legends *
  Mean   : Arithmetic mean of all measurements
  Error  : Half of 99.9% confidence interval
  StdDev : Standard deviation of all measurements
  1 ms   : 1 Millisecond (0.001 sec)

// ***** BenchmarkRunner: End *****
Run time: 00:00:32 (32.43 sec), executed benchmarks: 2
```