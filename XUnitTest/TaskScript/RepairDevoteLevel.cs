using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Linq;

namespace XUnitTest.TaskScript
{
    public class RepairDevoteLevel
    {
        private readonly IServiceProvider ServiceProvider;
        public readonly IConfiguration Configuration;

        public RepairDevoteLevel()
        {
            CommServiceProvider commService = new CommServiceProvider();
            Configuration = commService.GetConfiguration();
            ServiceProvider = commService.GetServiceProvider();
        }

        [Fact]
        public void Repair()
        {
            Yoyo.Entity.SqlContext SqlContext = this.ServiceProvider.GetService<Yoyo.Entity.SqlContext>();

            List<Yoyo.IServices.Utils.SystemUserLevel> Settings = this.ServiceProvider.GetRequiredService<IOptionsMonitor<List<Yoyo.IServices.Utils.SystemUserLevel>>>().CurrentValue;
            StringBuilder Sql = new StringBuilder();
            Sql.AppendLine("UPDATE `user` SET golds=0,`level`='lv0';");
            Sql.AppendLine("UPDATE `user` SET golds=50 WHERE auditState=2;");
            Sql.AppendLine("UPDATE `user` AS u INNER JOIN `user_ext` AS e ON u.id=e.userId SET u.golds=u.golds+e.authCount*50;");
            Sql.AppendLine($"UPDATE `user` SET `level`={Level(Settings)}");
            Sql.AppendLine("");
            Sql.AppendLine("SELECT `level`,COUNT(`level`) AS Total FROM `user` GROUP BY `level`;");
            String ExecuteSql = Sql.ToString();

        }

        public string Level(List<Yoyo.IServices.Utils.SystemUserLevel> req)
        {
            StringBuilder Sql = new StringBuilder();
            Sql.AppendLine("(CASE");
            foreach (var item in req.Where(o => o.Claim > 0).OrderByDescending(o => o.Claim))
            {
                Sql.AppendLine($"WHEN `golds` >= {item.Claim} THEN '{item.Level}'");
            }
            Sql.AppendLine("ELSE 'lv0'");
            Sql.AppendLine("END);");
            var SqlString = Sql.ToString();
            return SqlString;
        }
    }
}
