
using System.Text;
using System.Text.RegularExpressions;
using Xunit;
using ZipSharp.ZipIndex;
namespace ZipSharpTest
{
    public class ZipIndexTest
    {
        [Theory]
        [InlineData("TestFile\\a.zip","", "border.json", 13630)]
        [InlineData("TestFile\\a.zip", "", "sub/sub子路径/border.json", 13630)]
        public void ZipIndexFile(string zipFileName,string password,string fileName,long fileSize)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding encoding = Encoding.UTF8;
            if (HasChinese(fileName))
            {
                encoding = Encoding.GetEncoding("GB2312");
            }

            var dict = IndexFile.GetIndexs(zipFileName, password,encoding);

            Assert.NotNull(dict);

            var entryPair = dict.First();

            Assert.NotNull(entryPair);

            try
            {
                var stream = IndexFile.GetIndexFileStream(zipFileName, password, entryPair.Value, encoding);
                
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
            var dict = IndexFile.GetIndexs(zipFileName,null);

            Assert.NotNull(dict);

            var bytes = IndexFile.Ser(dict);
            
            Assert.NotNull(bytes);

            var desDic = IndexFile.Des(bytes);

            Assert.NotNull(desDic); 

            Assert.Equal(dict.Count,desDic.Count);
        }



        public static bool HasChinese(string str)
        {
            return Regex.IsMatch(str, @"[\u4e00-\u9fa5]");
        }

    }
}