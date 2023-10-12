using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTest
{
    public class COSTest
    {
        private Yoyo.Plugin.IQCloud.Models.Config config;
        private readonly IServiceProvider ServiceProvider;
        public COSTest()
        {
            config = new Yoyo.Plugin.IQCloud.Models.Config()
            {
                SecretId = "AKIDtcu0qRQVgeLrBHPZovpV1RdbNyLWrQ4W",
                SecretKey = "X6TbtxnPp3qb8QUSmzyOmws0s8qNddSB",
                StsSecretId = "AKID6EE6ptceyMafDbJywFOMAnGHtOSNjj4B",
                StsSecretKey = "BIHCuUt12vcIoJix45yKXaTAi9RaMyRk",
                Bucket = "yoyoba-1254396143",
                Region = "ap-beijing",
                BucketDomain = "https://yoyoba-1254396143.cos.ap-beijing.myqcloud.com",
                ClientName = "YoyoCOS"
            };
            IServiceCollection services = new ServiceCollection();
            services.AddHttpClient(config.ClientName);
            services.AddScoped<Yoyo.IPlugins.IQCloudPlugin, Yoyo.Plugin.IQCloud.QCloudPlugin>();
            ServiceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public async void COS()
        {
            Yoyo.IPlugins.IQCloudPlugin QCloud = ServiceProvider.GetService<Yoyo.IPlugins.IQCloudPlugin>();

            String FileName = "yoyoba_excel.xlsx";
            String FilePath = "E:\\" + FileName;
            FileStream fileStream = File.OpenRead(FilePath);

            string authstr = await QCloud.PutAuthStr("/yoyoba_excel.xlsx");

            bool rult = await QCloud.PutObject("/yoyoba_excel.xlsx", fileStream);
            fileStream.Close();

            Stream stream = await QCloud.GetObject("/yoyoba_excel.xlsx");

            // 把 Stream 转换成 byte[] 
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);

            // 把 byte[] 写入文件 
            FileStream fs = new FileStream("E:\\test.xlsx", FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(bytes);
            bw.Close();
            fs.Close();

            bool isdel = await QCloud.DelObject("/yoyoba_excel.xlsx");

            return;
        }


        [Fact]
        public async void PutObject()
        {
            IHttpClientFactory factory = ServiceProvider.GetService<IHttpClientFactory>();
            HttpClient client = factory.CreateClient(config.ClientName);

            List<Int64> QKeyTime = new List<Int64>()
            {
                GetTimeStamp(DateTime.Now),
                GetTimeStamp(DateTime.Now.AddMinutes(30))
            };
            String FileName = "yoyoba_excel.xlsx";
            String FilePath = "E:\\" + FileName;
            String q_key_time = String.Join(";", QKeyTime);
            String q_sign_algorithm = "sha1";
            String SingKey = HMACSHA1(config.SecretKey, q_key_time);
            // String ApiUrl = "static-1301093304.cos.ap-beijing.myqcloud.com";
            String HttpStr = $"put\n/{FileName}\n\nhost=static-1301093304.cos.ap-beijing.myqcloud.com\n";
            String SignStr = $"{q_sign_algorithm}\n{q_key_time}\n{SHA1HASH(HttpStr)}\n";
            String Sign = HMACSHA1(SingKey, SignStr);
            FileStream fileStream = File.OpenRead(FilePath);

            String Url = $"https://static-1301093304.cos.ap-beijing.myqcloud.com/{FileName}?q-sign-algorithm={q_sign_algorithm}&q-ak={config.SecretId}&q-sign-time={q_key_time}&q-key-time={q_key_time}&q-header-list=host&q-url-param-list=&q-signature={Sign}";

            StreamContent content = new StreamContent(fileStream);
            HttpResponseMessage response = await client.PutAsync(Url, content);
            String ResultStr = await response.Content.ReadAsStringAsync();



            return;
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        private Int64 GetTimeStamp(DateTime dt)
        {
            TimeSpan ts = dt - new DateTime(1970, 1, 1, 8, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }

        /// <summary>
        /// HMACSHA1加密
        /// </summary>
        /// <param name="key"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        private static String HMACSHA1(String key, String body)
        {
            byte[] btkey = Encoding.UTF8.GetBytes(key);
            byte[] btbody = Encoding.UTF8.GetBytes(body);
            using (HMACSHA1 hmac = new HMACSHA1(btkey))
            {
                return String.Join("", hmac.ComputeHash(btbody).ToList().Select(b => b.ToString("x2")).ToArray());
            }
        }

        /// <summary>
        /// SHA1加密
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        private static String SHA1HASH(String body)
        {
            byte[] btbody = Encoding.UTF8.GetBytes(body);
            using (SHA1 sha1 = new SHA1CryptoServiceProvider())
            {
                return String.Join("", sha1.ComputeHash(btbody).ToList().Select(b => b.ToString("x2")).ToArray());
            }
        }


    }




}
