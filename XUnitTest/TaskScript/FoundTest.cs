using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Yoyo.Core.Expand;

namespace XUnitTest.TaskScript
{
    public class FoundTest
    {
        public IServiceProvider ServiceProvider;
        public FoundTest()
        {
            CommServiceProvider comm = new CommServiceProvider();
            ServiceProvider = comm.GetServiceProvider();
        }

        [Fact]
        public async Task SystemTransfer()
        {
            Yoyo.IServices.IMember.ISubscribe subscribe = this.ServiceProvider.GetService<Yoyo.IServices.IMember.ISubscribe>();
            await subscribe.SubscribeMemberRegist((new Yoyo.IServices.Request.ReqTeamSubscribeInfo
            {
                MemberId = 1,
                ParentId = 0
            }).ToJson());

        }
    }
}