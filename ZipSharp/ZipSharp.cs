using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZipSharp
{


    public enum CompressMethod
    {
        Stored = 0,
        Deflated=1,
        Bzip2,
        Aes,
        Zstd,
    }

    public class ZipLibABI
    {
        [DllImport("zip", EntryPoint = "extract_file")]
        public static extern int ExtractFile(string srcName, string targetPath);

        [DllImport("zip", EntryPoint = "compress_dir")]
        public static extern int CompressDir(string srcPath, string targetName,CompressMethod method=CompressMethod.Deflated);
    }
}
