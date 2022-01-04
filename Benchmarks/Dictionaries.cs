
using System.Collections.Concurrent;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace Benchmarks;

/*
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1415 (21H1/May2021Update)
Intel Core i9-9900K CPU 3.60GHz (Coffee Lake), 1 CPU, 16 logical and 8 physical cores
.NET SDK=6.0.100

|                         Method |     Mean |     Error |    StdDev |  Gen 0 |  Gen 1 | Allocated |
|------------------------------- |---------:|----------:|----------:|-------:|-------:|----------:|
|      ConcurrentDictionary_Same | 1.578 us | 0.0207 us | 0.0194 us | 0.1202 |      - |   1,016 B |
|                Dictionary_Same | 2.147 us | 0.0144 us | 0.0128 us | 0.0229 |      - |     216 B |
|           Dictionary_Different | 4.820 us | 0.0411 us | 0.0343 us | 1.5564 | 0.0458 |  13,072 B |
| ConcurrentDictionary_Different | 9.803 us | 0.1827 us | 0.1955 us | 2.6550 | 0.1526 |  22,312 B |
*/

[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[MemoryDiagnoser]
public class Dictionaries
{
    [Benchmark]
    public void Dictionary_Different()
    {
        string text;
        var dict = new Dictionary<string, string>(StringComparer.Ordinal);
        for (int i = 0 ; i < 100; i++)
        {
            text = i.ToString();
            lock (dict)
            {
                if (!dict.TryGetValue(text, out _))
                {
                    dict.Add(text, text);
                }
            }
        }
    }

    [Benchmark]
    public void Dictionary_Same()
    {
        string text = "TEST";
        var dict = new Dictionary<string, string>(StringComparer.Ordinal);
        for (int i = 0 ; i < 100; i++)
        {
            lock (dict)
            {
                if (!dict.TryGetValue(text, out _))
                {
                    dict.Add(text, text);
                }
            }
        }
    }

    [Benchmark]
    public void ConcurrentDictionary_Different()
    {
        string text;
        var dict = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);
        for (int i = 0 ; i < 100; i++)
        {
            text = i.ToString();
            dict.GetOrAdd(text, text);
        }
    }

    [Benchmark]
    public void ConcurrentDictionary_Same()
    {
        string text = "TEST";
        var dict = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);
        for (int i = 0 ; i < 100; i++)
        {
            dict.GetOrAdd(text, text);
        }
    }
}