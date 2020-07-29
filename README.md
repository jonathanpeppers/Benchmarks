# Benchmarks

An empty `BenchmarkDotNet` project, mostly throwaway benchmarks.

Different branches have benchmarks for different thingies.

## Results for the `<Hash/>` MSBuild task

    BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18362.959 (1903/May2019Update/19H1)
    Intel Core i9-9900K CPU 3.60GHz (Coffee Lake), 1 CPU, 16 logical and 8 physical cores
      [Host]     : .NET Framework 4.8 (4.8.4180.0), X64 RyuJIT
      DefaultJob : .NET Framework 4.8 (4.8.4180.0), X64 RyuJIT
    
    
    | Method |     Mean |    Error |   StdDev |   Gen 0 |  Gen 1 | Gen 2 | Allocated |
    |------- |---------:|---------:|---------:|--------:|-------:|------:|----------:|
    |  Hash2 | 85.57 us | 0.364 us | 0.340 us | 14.6484 | 1.5869 |     - |  90.88 KB |
    |  Hash1 | 90.27 us | 0.374 us | 0.313 us | 25.3906 | 5.0049 |     - | 157.18 KB |
    
    // * Hints *
    Outliers
      HashBenchmark.Hash2: Default -> 3 outliers were detected (84.91 us..85.00 us)
      HashBenchmark.Hash1: Default -> 2 outliers were removed (91.34 us, 91.40 us)
    
    // * Legends *
      Mean      : Arithmetic mean of all measurements
      Error     : Half of 99.9% confidence interval
      StdDev    : Standard deviation of all measurements
      Gen 0     : GC Generation 0 collects per 1000 operations
      Gen 1     : GC Generation 1 collects per 1000 operations
      Gen 2     : GC Generation 2 collects per 1000 operations
      Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
      1 us      : 1 Microsecond (0.000001 sec)
