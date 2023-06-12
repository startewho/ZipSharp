using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using IronCompress;
using System.IO.Compression;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;
using Cysharp.Diagnostics;
using System.Diagnostics;
using System.Collections.Generic;

namespace ZstdSharp.Benchmark
{
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class Benchmark
    {

        private readonly string folder = @"E:\Bin\dbeaver\";
        private readonly string zipSharp = "zipsharp.zip";
        private readonly string zipSystem = "zipsystem.zip";
        private readonly string zipZstd = "zipzstd.zip";
        [GlobalSetup]
        public void Setup()
        {

        }

        [BenchmarkCategory("Compress"), Benchmark(Baseline = true)]
        public void CompressZipSharp()
        {

            if (File.Exists(zipSharp))
            {
                File.Delete(zipSharp);
            }
            var zip = new FastZip();
            zip.CreateZip(zipSharp, folder, true, string.Empty, string.Empty);
           
        }

        [BenchmarkCategory("Compress"), Benchmark]
        public void CompressZipSystem()
        {

            if (File.Exists(zipSystem))
            {
                File.Delete(zipSystem);
            }
            System.IO.Compression.ZipFile.CreateFromDirectory(folder, zipSystem);

        }

        [BenchmarkCategory("Compress"), Benchmark]
        public void CompressZipZstd()
        {
            if (File.Exists(zipZstd))
            {
                File.Delete(zipZstd);
            }
            ZipSharp.ZipLibABI.CompressDir(folder, zipZstd,ZipSharp.CompressMethod.Zstd);
        }


        //[BenchmarkCategory("Decompress"), Benchmark(Baseline = true)]
        //public void DecompressZipSharp()
        //{

        //}

        //[BenchmarkCategory("Decompress"), Benchmark]
        //public void DecompressZipSystem()
        //{

        //}

        //[BenchmarkCategory("Decompress"), Benchmark]
        //public void DecompressZipZstd()
        //{

        //}
    }
}
