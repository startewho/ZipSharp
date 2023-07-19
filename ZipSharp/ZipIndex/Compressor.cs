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
        public static long Compress(Stream inStream,Stream outStream)
        {
            DeflateCompressor compressor = new DeflateCompressor(); 
            compressor.Compress(inStream,outStream);
            return outStream.Length;
        }
        public static long Decompress(Stream inStream,Stream outStream)
        {
            DeflateCompressor compressor = new DeflateCompressor();
            compressor.Decompress(inStream, outStream);
            return outStream.Length;
        }
    }
}
