# Benchmarks

Initial benchmarks for Mono.Cecil vs System.Reflection.Metadata (Windows):
```
// * Summary *

BenchmarkDotNet=v0.11.3, OS=Windows 10.0.17763.316 (1809/October2018Update/Redstone5)
Intel Core i7-7600U CPU 2.80GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
  [Host]     : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.3324.0
  DefaultJob : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.3324.0


                     Method |      Mean |    Error |   StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
--------------------------- |----------:|---------:|---------:|------------:|------------:|------------:|--------------------:|
 System.Reflection.Metadata |  81.00 ms | 1.564 ms | 2.192 ms |  13333.3333 |   1000.0000 |    666.6667 |            28.18 MB |
                 Mono.Cecil | 100.33 ms | 1.994 ms | 2.296 ms |  10600.0000 |   2800.0000 |   1400.0000 |             62.6 MB |

// * Hints *
Outliers
  Cecil.System.Reflection.Metadata: Default -> 2 outliers were detected

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
Run time: 00:00:37 (37.39 sec), executed benchmarks: 2
```

Results on MacOS:
```
// * Summary *

BenchmarkDotNet=v0.11.3, OS=macOS Mojave 10.14.2 (18C54) [Darwin 18.2.0]
Intel Core i7-6567U CPU 3.30GHz (Skylake), 1 CPU, 4 logical and 2 physical cores
  [Host]     : Mono 5.18.0.271 (2018-08/7ad18718865 Fri), 64bit
  DefaultJob : Mono 5.18.0.271 (2018-08/7ad18718865 Fri), 64bit


                     Method |     Mean |    Error |   StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
--------------------------- |---------:|---------:|---------:|------------:|------------:|------------:|--------------------:|
 System.Reflection.Metadata | 111.1 ms | 1.303 ms | 1.219 ms |   1400.0000 |   1400.0000 |   1400.0000 |                   - |
                 Mono.Cecil | 133.5 ms | 2.623 ms | 3.845 ms |   3000.0000 |   3000.0000 |   3000.0000 |                   - |

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
Run time: 00:00:21 (21.38 sec), executed benchmarks: 2
```