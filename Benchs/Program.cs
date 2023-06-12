using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ZstdSharp.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {

            var summary = BenchmarkRunner.Run<Benchmark>();
           // var stopwatch=new Stopwatch();
           //var bench=new Benchmark();
           // stopwatch.Start();
           // bench.CompressZipSharp();
           // stopwatch.Stop();
           // Console.WriteLine($"使用ZipSharp; {stopwatch.Elapsed}");

           // stopwatch.Start();
           // bench.CompressZipSystem();
           // stopwatch.Stop();
           // Console.WriteLine($"使用ZipSystem; {stopwatch.Elapsed}");

           // stopwatch.Start();
           // bench.CompressZipZstd();
           // stopwatch.Stop();
           // Console.WriteLine($"使用ZipZstd; {stopwatch.Elapsed}");

        }
    }
}
