using Aliyun.OSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using ZipSharp.ZipIndex;

namespace ZipSharpTest
{
    internal class OssPartTest
    {

        private readonly string bucket = "";

        private OssClient CreateClient()
        {
            var client = new OssClient("https://oss-cn-hangzhou.aliyuncs.com", "", "");
            return client;
        }

        [InlineData("TestFile\\a.zip", "Test/Sub/a.zip")]
        public void ZipUpload(string zipFileName, string key)
        {
            var client = CreateClient();
            try
            {
                var zipInfo = ZipFileInfo.GetIndexs(zipFileName, null);

                Assert.NotNull(zipInfo);

                var bytes = ZipFileInfo.Ser(zipInfo);

                ObjectMetadata meta = new ObjectMetadata();
                meta.UserMetadata.Add("ZipIndex", Convert.ToBase64String(bytes));

                client.PutObject(bucket, key, zipFileName, meta);

            }
            catch (Exception ex)
            {
            }

        }

        [InlineData("Test/Sub/a.zip", "report.json")]
        public void ZipDownload(string key, string file)
        {
            var client = CreateClient();
            try
            {
                var oldMeta = client.GetObjectMetadata(bucket, key);

                if (oldMeta.UserMetadata.TryGetValue("ZipIndex", out string index))
                {
                    var zipInfo = ZipFileInfo.Des(Convert.FromBase64String(index));
                    Assert.NotNull(zipInfo);
                    var fileInfo = zipInfo.Files.FirstOrDefault(f => f.Key == file).Value;
                    Assert.NotNull(fileInfo);

                    var getObjectRequest = new GetObjectRequest(bucket, key);

                    getObjectRequest.SetRange(fileInfo.Offset + fileInfo.HeaderSize, fileInfo.CompressedSize64);
                   
                    var obj = client.GetObject(getObjectRequest);
                    using var fileStream = new FileStream(file, FileMode.OpenOrCreate);
                    Assert.True(obj.Content.Length == fileInfo.CompressedSize64);
                    Compressor.Decompress(obj.Content, fileStream);
                    Assert.True(fileStream.Length == fileInfo.UncompressedSize64);
                }



            }
            catch (Exception ex)
            {
            }

        }
    }
}
