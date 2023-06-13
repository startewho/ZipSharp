using System.Runtime.InteropServices;

namespace ZipSharp
{

    /// <summary>
    /// CompressMethod
    /// </summary>
    public enum CompressMethod
    {
        Stored = 0,
        Deflated=1,
        Bzip2,
        Aes,
        Zstd,
    }

    public partial class ZipLibABI
    {
        [LibraryImport("zip", EntryPoint = "extract_file",StringMarshalling = StringMarshalling.Utf8)]
        public  static partial int ExtractFile(string srcName, string targetPath);

        [LibraryImport("zip", EntryPoint = "compress_dir",StringMarshalling =StringMarshalling.Utf8)]
        public static partial  int CompressDir(string srcPath, string targetName,CompressMethod method=CompressMethod.Deflated);
    }
}
