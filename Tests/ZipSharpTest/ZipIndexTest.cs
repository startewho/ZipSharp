
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;
using ZipSharp.ZipIndex;
namespace ZipSharpTest;

public class ZipIndexTest
{
    [Theory]
    [InlineData("TestFile\\a.zip", "", "border.json", 13630)]
    [InlineData("TestFile\\a.zip", "", "sub/sub子路径/border.json", 13630)]
    public void ZipIndexFile(string zipFileName, string password, string fileName, long fileSize)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Encoding encoding = Encoding.UTF8;
        if (HasChinese(fileName))
        {
            encoding = Encoding.GetEncoding("GB2312");
        }

        var zipInfo = ZipFileInfo.GetIndexs(zipFileName, password, encoding);

        Assert.NotNull(zipInfo);

        var entryPair = zipInfo.Files.First();

        Assert.NotNull(entryPair);

        try
        {
            var stream = IndexFile.GetIndexFileStream(zipFileName, password, zipInfo.Method, entryPair.Value, encoding);

            using (var memory = new MemoryStream())
            {
                stream.CopyTo(memory);
                memory.Flush();
                stream.Close();
                Assert.True(memory.Length == fileSize);
            }
        }
        catch (Exception ex)
        {
            throw ex;
            throw;
        }


    }

    [Theory]
    [InlineData("TestFile\\a.zip")]
    public void ZipIndexSerDic(string zipFileName)
    {
        var zipInfo = ZipFileInfo.GetIndexs(zipFileName, null);

        Assert.NotNull(zipInfo);

        var bytes = ZipFileInfo.Ser(zipInfo);

        Assert.NotNull(bytes);

        var desInfo = ZipFileInfo.Des(bytes);

        Assert.NotNull(desInfo);

        Assert.Equal(zipInfo.Method, desInfo.Method);
        Assert.Equal(zipInfo.Files.Count, desInfo.Files.Count);
    }


    [Theory]
    [InlineData("TestFile\\a.zip")]
    public void PartDecompress(string zipFileName)
    {
        var zipInfo = ZipFileInfo.GetIndexs(zipFileName, null);
        Assert.NotNull(zipInfo);
        using (var fileStream = new FileStream(zipFileName, FileMode.Open))
        {
            foreach (var info in zipInfo.Files)
            {
                var file = info.Value;
                var filePath = info.Key;
                var folder = Directory.GetParent(filePath).FullName;
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                fileStream.Position = file.Offset + 26;
                var fileNameLength = ReadLEUshort(fileStream); //2byte
                var commentLength = ReadLEUshort(fileStream); //2byte
                fileStream.Position = file.Offset + 30 + fileNameLength + commentLength;
                var buffers = new byte[file.CompressedSize64];
                using (var inStream = new MemoryStream())
                {
                    var read = fileStream.ReadAtLeast(buffers, (int)file.CompressedSize64);
                    if (read >= file.CompressedSize64)
                    {
                        inStream.Write(buffers);
                        using (var outStream = new FileStream(filePath, FileMode.OpenOrCreate))
                        {
                            inStream.Position = 0;
                            Compressor.Decompress(inStream, outStream);
                            Assert.True(file.UncompressedSize64 == outStream.Length);
                        }
                    }
                }
            }
        }
    }


    [Theory]
    [InlineData("TestFile\\a.zip")]
    public void PartParellDecompress(string zipFileName)
    {
        var zipInfo = ZipFileInfo.GetIndexs(zipFileName, null);
        Assert.NotNull(zipInfo);

        Parallel.ForEach(zipInfo.Files, info =>
        {
            using (var fileStream = new FileStream(zipFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {

                var file = info.Value;
                var filePath = info.Key;
                var folder = Directory.GetParent(filePath).FullName;
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                fileStream.Position = file.Offset + 26;
                var fileNameLength = ReadLEUshort(fileStream); //2byte
                var commentLength = ReadLEUshort(fileStream); //2byte
                fileStream.Position = file.Offset + 30 + fileNameLength + commentLength;
                var buffers = new byte[file.CompressedSize64];
                using (var inStream = new MemoryStream())
                {
                    var read = fileStream.ReadAtLeast(buffers, (int)file.CompressedSize64);
                    if (read >= file.CompressedSize64)
                    {
                        inStream.Write(buffers);
                        using (var outStream = new FileStream(filePath, FileMode.OpenOrCreate))
                        {
                            inStream.Position = 0;
                            Compressor.Decompress(inStream, outStream);
                            Assert.True(file.UncompressedSize64 == outStream.Length);
                        }
                    }
                }

            }
        }
        );

    }

    static ushort ReadLEUshort(Stream baseStream)
{
    int data1 = baseStream.ReadByte();

    if (data1 < 0)
    {
        throw new EndOfStreamException("End of stream");
    }

    int data2 = baseStream.ReadByte();

    if (data2 < 0)
    {
        throw new EndOfStreamException("End of stream");
    }

    return unchecked((ushort)((ushort)data1 | (ushort)(data2 << 8)));
}


public static bool HasChinese(string str)
{
    return Regex.IsMatch(str, @"[\u4e00-\u9fa5]");
}
}