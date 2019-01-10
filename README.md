# Benchmarks

Initial benchmarks for Mono.Cecil vs System.Reflection.Metadata (Windows):
```
// * Summary *

BenchmarkDotNet=v0.11.3, OS=Windows 10.0.17763.194 (1809/October2018Update/Redstone5)
Intel Core i7-7600U CPU 2.80GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
  [Host]     : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.3260.0
  DefaultJob : .NET Framework 4.7.2 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.3260.0


                                              Method |      Mean |     Error |    StdDev |    Median | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
---------------------------------------------------- |----------:|----------:|----------:|----------:|------------:|------------:|------------:|--------------------:|
 'System.Reflection.Metadata with default settings.' |  50.98 ms |  2.187 ms |  3.998 ms |  49.31 ms |   7444.4444 |           - |           - |             15.1 MB |
                 'Mono.Cecil with default settings.' | 538.70 ms | 10.666 ms |  9.977 ms | 538.74 ms |  16000.0000 |  11000.0000 |   3000.0000 |           103.41 MB |
     'Mono.Cecil with ReadingMode.Deferred setting.' | 543.68 ms | 10.796 ms | 12.432 ms | 543.74 ms |  16000.0000 |  11000.0000 |   3000.0000 |           103.41 MB |
            'Mono.Cecil with InMemory=True setting.' | 565.58 ms | 11.257 ms | 14.236 ms | 563.15 ms |  16000.0000 |  11000.0000 |   3000.0000 |           157.73 MB |

// * Hints *
Outliers
  Cecil.'System.Reflection.Metadata with default settings.': Default -> 7 outliers were removed
  Cecil.'Mono.Cecil with default settings.': Default                 -> 1 outlier  was  removed

// * Legends *
  Mean                : Arithmetic mean of all measurements
  Error               : Half of 99.9% confidence interval
  StdDev              : Standard deviation of all measurements
  Median              : Value separating the higher half of all measurements (50th percentile)
  Gen 0/1k Op         : GC Generation 0 collects per 1k Operations
  Gen 1/1k Op         : GC Generation 1 collects per 1k Operations
  Gen 2/1k Op         : GC Generation 2 collects per 1k Operations
  Allocated Memory/Op : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
  1 ms                : 1 Millisecond (0.001 sec)

// ***** BenchmarkRunner: End *****
Run time: 00:01:26 (86.94 sec), executed benchmarks: 4
```

Results on MacOS:
```
// * Summary *

BenchmarkDotNet=v0.11.3, OS=macOS Mojave 10.14.2 (18C54) [Darwin 18.2.0]
Intel Core i7-6567U CPU 3.30GHz (Skylake), 1 CPU, 4 logical and 2 physical cores
  [Host]     : Mono 5.18.0.162 (2018-08/bc9d709e704 Fri), 64bit
  DefaultJob : Mono 5.18.0.162 (2018-08/bc9d709e704 Fri), 64bit


                                              Method |      Mean |      Error |     StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
---------------------------------------------------- |----------:|-----------:|-----------:|------------:|------------:|------------:|--------------------:|
 'System.Reflection.Metadata with default settings.' |  95.54 ms |  0.8713 ms |  0.7724 ms |   4500.0000 |           - |           - |                   - |
     'Mono.Cecil with ReadingMode.Deferred setting.' | 425.76 ms |  8.2878 ms |  9.8660 ms |  26000.0000 |   5000.0000 |   5000.0000 |                   - |
                 'Mono.Cecil with default settings.' | 436.69 ms | 16.3162 ms | 16.7555 ms |  28000.0000 |   6000.0000 |   6000.0000 |                   - |
            'Mono.Cecil with InMemory=True setting.' | 465.90 ms |  4.8805 ms |  4.3265 ms |  26000.0000 |   6000.0000 |   6000.0000 |                   - |

// * Hints *
Outliers
  Cecil.'System.Reflection.Metadata with default settings.': Default -> 1 outlier  was  removed
  Cecil.'Mono.Cecil with ReadingMode.Deferred setting.': Default     -> 2 outliers were removed, 4 outliers were detected

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
Run time: 00:00:58 (58.73 sec), executed benchmarks: 4
```