using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyCompressor;

namespace ZipSharp.ZipIndex
{
    public class Compressor
    {
        private readonly static Lazy<ICompressor> deflat = new Lazy<ICompressor>(new DeflateCompressor());
        private readonly static Lazy<ICompressor> gzip = new Lazy<ICompressor>(new GZipCompressor());
        private readonly static Lazy<ICompressor> brotli = new Lazy<ICompressor>(new BrotliCompressor());
        private readonly static Lazy<ICompressor> zstd = new Lazy<ICompressor>(new ZstdCompressor());


        private static ICompressor GetCompressor(CompressMethod compressMethod)
        {
            switch (compressMethod)
            {
                case CompressMethod.Stored:
                    break;
                case CompressMethod.Deflated:
                    return deflat.Value;
                case CompressMethod.Bzip2:
                    break;
                case CompressMethod.Aes:
                    break;
                case CompressMethod.Zstd:
                    return zstd.Value;
            }
            return deflat.Value;
        }
        public static long Compress(Stream inStream, Stream outStream, CompressMethod compressMethod)
        {
            var compressor = GetCompressor(compressMethod);

            compressor.Compress(inStream, outStream);
            return outStream.Length;
        }
        public static long Decompress(Stream inStream, Stream outStream, CompressMethod compressMethod=CompressMethod.Deflated)
        {
            var compressor = GetCompressor(compressMethod);
            compressor.Decompress(inStream, outStream);
            return outStream.Length;
        }
    }
}
