using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Yoyo.IPlugins;
using Yoyo.IPlugins.Request;
using Yoyo.Plugin.RealVerify;
using Yoyo.Plugin.RealVerify.Models;

namespace XUnitTest
{
    public class RealVerify
    {
        private readonly IServiceProvider ServiceProvider;
        public RealVerify()
        {
            Config config = new Config()
            {
                ClientName = "RealVerify",
                AccessKey = "LTAI4Fh748kfpY6mYWptXbqX",
                AccessSecret = "CnbnP6taECcu30nxE2OGNb8UbDvTwX",
                ApiUrl = "https://saf.cn-shanghai.aliyuncs.com"
            };
            IServiceCollection services = new ServiceCollection();
            services.AddHttpClient(config.ClientName);
            services.AddScoped<IRealVerify, AlipayVerify>();

            ServiceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public async void Test()
        {

            String ttt = null;

            var ss =  ttt.Equals("11");

            var VerifySub = ServiceProvider.GetService<IRealVerify>();

            //var Result = await VerifySub.Execute(new ReqRealVerifyInitiate()
            //{
            //    SceneId = "1000000095",
            //    OuterOrderNo = "19219875432159701",
            //    BizCode = "FACE",
            //    CertName = "张永",
            //    CertNo = "130182198701070913",
            //    ReturnUrl = "http://www.baidu.com/"
            //});

            List<ReqRealVerifyQuery> list = new List<ReqRealVerifyQuery>();

            list.Add(new ReqRealVerifyQuery()
            {
                CertifyId = "5918411718143f259e397ca859cbea13",
                SceneId = "1000000095"
            });

            list.Add(new ReqRealVerifyQuery()
            {
                CertifyId = "960047b0684f260bf58f9bd44904f4b5",
                SceneId = "1000000095"
            });

            list.Add(new ReqRealVerifyQuery()
            {
                CertifyId = "c0025dc746cc02e55095bacb9384629f",
                SceneId = "1000000095"
            });

            list.Add(new ReqRealVerifyQuery()
            {
                CertifyId = "de0b884f87166d5ed19f5f4aafd8d4d9",
                SceneId = "1000000095"
            });

            list.Add(new ReqRealVerifyQuery()
            {
                CertifyId = "76ba5ec9b2c35b574bc9dd5082a25d95",
                SceneId = "1000000095"
            });

            list.Add(new ReqRealVerifyQuery()
            {
                CertifyId = "e6b39413f785547b6d5f021b3e0dad2a",
                SceneId = "1000000095"
            });

            foreach (var item in list)
            {
                var Rult = await VerifySub.Execute(item);
            }

            return;
        }
    }
}
