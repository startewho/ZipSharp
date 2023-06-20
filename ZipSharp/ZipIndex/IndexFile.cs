using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using MessagePack;

namespace ZipSharp.ZipIndex
{
    [MessagePackObject]
    public class IndexFile
    {
        /// <summary>
        /// Name of the file as stored in the zip.
        /// </summary>
        [Key(0)]
        public string Name { get; set; }


        /// <summary>
        /// Size of compressed data, excluding ZIP headers.
        /// </summary>
        [Key(1)]
        public long CompressedSize64 { get; set; }


        /// <summary>
        /// Size of the Uncompressed data.
        /// </summary>
        [Key(2)]
        public long UncompressedSize64 { get; set; }


        /// <summary>
        /// Offset where file data header starts.
        /// </summary>
        [Key(3)]
        public long Offset { get; set; }


        /// <summary>
        /// CRC of the uncompressed data.
        /// </summary>
        [Key(4)]
        public long CRC32 { get; set; }


        /// <summary>
        /// The index of file in the zip file.
        /// </summary>
        [Key(5)]
        public long ZipFileIndex { get; set; }

        /// <summary>
        /// Storage method.
        /// </summary>
        [Key(6)]
        public ushort Method { get; set; }  // Storage method.

        /// <summary>
        /// General purpose bit flag.
        /// </summary>
        [Key(7)]
        public ushort Flags { get; set; }


        /// <summary>
        /// Custom data.
        /// </summary>
        [Key(8)]
        public Dictionary<string, string> Custom { get; set; }


        public static byte[] Ser(Dictionary<string, IndexFile> dic)
        {
            byte[] bytes = MessagePackSerializer.Serialize(dic);
            return bytes;
        }


        public static Dictionary<string, IndexFile> Des(byte[] bytes)
        {
            var dict= MessagePackSerializer.Deserialize<Dictionary<string, IndexFile>>(bytes);
            return dict;
        }


        /// <summary>
        /// 获取IndexFile字典
        /// </summary>
        /// <param name="zipFileName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static Dictionary<string, IndexFile> GetIndexs(string zipFileName, string password, Encoding encoding = null)
        {
            var dict = new Dictionary<string, IndexFile>();
            using Stream inputStream = File.Open(zipFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var zipFile = new ZipFile(inputStream, false, StringCodec.Default);
            if (string.IsNullOrEmpty(password))
            {
                zipFile.Password = password;
            }
            foreach (var item in zipFile)
            {
                var entry = (ZipEntry)item;
                if (entry.IsFile)
                {
                    var indexFile = new IndexFile()
                    {
                        Name = entry.Name,
                        Offset = entry.Offset,
                        CompressedSize64 = entry.CompressedSize,
                        UncompressedSize64 = entry.Size,
                        CRC32 = entry.Crc,
                        ZipFileIndex = entry.ZipFileIndex,
                        Method = (ushort)entry.CompressionMethod,
                        Flags = (ushort)entry.Flags,
                    };
                    dict.TryAdd(entry.Name, indexFile);
                }
            }
            return dict;
        }

        public static Stream GetIndexFileStream(string zipFileName, string password, IndexFile indexFile, Encoding encoding = null)
        {
            Stream inputStream = File.Open(zipFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            return GetIndexFileStream(inputStream, password, indexFile, encoding);
        }

        /// <summary>
        /// 根据IndexFile获取原文件
        /// </summary>
        /// <param name="zipFileStream"></param>
        /// <param name="password"></param>
        /// <param name="indexFile"></param>
        /// <returns></returns>
        public static Stream GetIndexFileStream(Stream zipFileStream, string password, IndexFile indexFile, Encoding encoding = null)
        {
            using var zipFile = new ZipFile(zipFileStream, true, StringCodec.FromEncoding(encoding));
            if (string.IsNullOrEmpty(password))
            {
                zipFile.Password = password;
            }
            var entry = new ZipEntry(indexFile.Name)
            {
                Offset = indexFile.Offset,
                CompressedSize = indexFile.CompressedSize64,
                Size = indexFile.UncompressedSize64,
                Crc = indexFile.CRC32,
                ZipFileIndex = indexFile.ZipFileIndex,
                CompressionMethod = (CompressionMethod)indexFile.Method,
                Flags = indexFile.Flags,
            };
            return zipFile.GetInputStream(entry);
        }


    }
}
