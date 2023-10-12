using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Diagnostics;
using Yoyo.Entity.Models;
using System.Collections.Generic;
using System.Linq;

namespace Yoyo.Jobs
{
    public class UpdateTeamStar : IJob
    {
        private readonly IServiceProvider ServiceProvider;

        public UpdateTeamStar(IServiceProvider service)
        {
            this.ServiceProvider = service;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            using (var service = this.ServiceProvider.CreateScope())
            {
                try
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    Entity.SqlContext SqlContext = service.ServiceProvider.GetRequiredService<Entity.SqlContext>();
                    String TableName = $"lfex_member_star_now";

                    StringBuilder Sql = new StringBuilder();
                    //==============================创建基础数据表==============================//
                    Sql.AppendLine("DROP TABLE IF EXISTS `lfex_member_ext_tmp`;");
                    Sql.AppendLine("CREATE TABLE `lfex_member_ext_tmp` (");
                    Sql.AppendLine("  `Id` bigint(20) NOT NULL,");
                    Sql.AppendLine("  `UserID` bigint(20) NOT NULL,");
                    Sql.AppendLine("  `ParentId` bigint(20) NOT NULL,");
                    Sql.AppendLine("  `teamStart` int(11) NOT NULL,");
                    Sql.AppendLine("  `teamCount` int(11) NOT NULL,");
                    Sql.AppendLine("  `authCount` int(11) NOT NULL,");
                    Sql.AppendLine("  `teamCandyH` DECIMAL(11,4) NOT NULL,");
                    Sql.AppendLine("  `bigCandyH` DECIMAL(11,4) NOT NULL,");
                    Sql.AppendLine("  `littleCandyH` DECIMAL(11,4) NOT NULL,");
                    Sql.AppendLine("  PRIMARY KEY (`UserID`),");
                    Sql.AppendLine("	KEY `FK_Id` (`Id`) USING BTREE,");
                    Sql.AppendLine("  KEY `FK_ParentId` (`ParentId`) USING BTREE,");
                    Sql.AppendLine("  KEY `FK_teamCandyH` (`teamCandyH`) USING BTREE");
                    Sql.AppendLine(") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;");
                    Sql.AppendLine("");
                    //==============================拷贝基础数据==============================//
                    Sql.AppendLine("TRUNCATE TABLE lfex_member_ext_tmp;");
                    Sql.AppendLine("INSERT INTO lfex_member_ext_tmp SELECT ");
                    Sql.AppendLine("A.Id,A.UserID,R.ParentId,0 AS teamStart,A.teamCount,A.authCount,A.teamCandyH,0 AS bigCandyH,0 AS littleCandyH FROM (");
                    Sql.AppendLine("SELECT (@i:=@i+1) AS Id,ext.userId AS UserID,ext.authCount,ext.teamCount,ext.teamCandyH FROM user_ext AS ext,(SELECT @i:=0) AS Ids ORDER BY ext.teamCandyH DESC) AS A");
                    Sql.AppendLine("INNER JOIN (SELECT MemberId,ParentId FROM yoyo_member_relation) AS R ON A.userId=R.MemberId");
                    Sql.AppendLine("ORDER BY A.Id;");
                    Sql.AppendLine("");
                    //==============================创建分红基础表==============================//
                    Sql.AppendLine($"DROP TABLE IF EXISTS `{TableName}`;");
                    Sql.AppendLine($"CREATE TABLE `{TableName}` (");
                    Sql.AppendLine("  `UserId` bigint(20) NOT NULL,");
                    Sql.AppendLine("  `teamStart` int(11) NOT NULL DEFAULT '0',");
                    Sql.AppendLine("  `teamCandyH` DECIMAL(11,4) NOT NULL DEFAULT '0',");
                    Sql.AppendLine("  `bigCandyH` DECIMAL(11,4) NOT NULL DEFAULT '0',");
                    Sql.AppendLine("  `littleCandyH` DECIMAL(11,4) NOT NULL DEFAULT '0',");
                    Sql.AppendLine("  PRIMARY KEY (`UserId`)");
                    Sql.AppendLine(") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;");
                    //==============================写入分红数据==============================//
                    Sql.AppendLine($"TRUNCATE TABLE {TableName};");
                    Sql.AppendLine($"INSERT INTO {TableName} SELECT ");
                    Sql.AppendLine("Tmp.UserId,");
                    Sql.AppendLine("(CASE");
                    Sql.AppendLine("  WHEN Tmp.authCount>=20 AND Tmp.teamCandyH>=100000 AND IF(Tmp.bigCandyH>Tmp.littleCandyH,Tmp.littleCandyH,Tmp.bigCandyH)>=25000 THEN 4");
                    Sql.AppendLine("  WHEN Tmp.authCount>=20 AND Tmp.teamCandyH>=8000 AND IF(Tmp.bigCandyH>Tmp.littleCandyH,Tmp.littleCandyH,Tmp.bigCandyH)>=2000 THEN 3");
                    Sql.AppendLine("  WHEN Tmp.authCount>=20 AND Tmp.teamCandyH>=2000 AND IF(Tmp.bigCandyH>Tmp.littleCandyH,Tmp.littleCandyH,Tmp.bigCandyH)>=500 THEN 2");
                    Sql.AppendLine("  WHEN Tmp.authCount>=20 AND Tmp.teamCandyH>=500 THEN 1");
                    Sql.AppendLine("  ELSE 0");
                    Sql.AppendLine("END)AS teamStart,");
                    Sql.AppendLine("Tmp.teamCandyH,");
                    Sql.AppendLine("IF(Tmp.bigCandyH<Tmp.littleCandyH,Tmp.littleCandyH,Tmp.bigCandyH) AS bigCandyH,");
                    Sql.AppendLine("IF(Tmp.bigCandyH>Tmp.littleCandyH,Tmp.littleCandyH,Tmp.bigCandyH) AS littleCandyH");
                    Sql.AppendLine("FROM (");
                    Sql.AppendLine("SELECT ");
                    Sql.AppendLine("A.UserID,A.authCount,A.teamCandyH,");
                    Sql.AppendLine("IFNULL(B.BigCandyH,0) AS bigCandyH,");
                    Sql.AppendLine("IF(A.teamCandyH-IFNULL(B.BigCandyH,0)<0,0,A.teamCandyH-IFNULL(B.BigCandyH,0)) AS littleCandyH");
                    Sql.AppendLine("FROM lfex_member_ext_tmp AS A LEFT JOIN (");
                    Sql.AppendLine("SELECT A.ParentId AS UserID,SUM(A.teamCandyH) AS BigCandyH FROM (SELECT * FROM lfex_member_ext_tmp WHERE teamCandyH>0) AS A");
                    Sql.AppendLine("  WHERE (");
                    Sql.AppendLine("    SELECT COUNT(1) FROM (SELECT * FROM lfex_member_ext_tmp WHERE teamCandyH>0) AS B");
                    Sql.AppendLine("    WHERE B.ParentId=A.ParentId AND B.Id<=A.Id");
                    Sql.AppendLine("   )<2 ");
                    Sql.AppendLine(" GROUP BY A.ParentId) AS B ON A.UserID=B.UserID");
                    Sql.AppendLine(")AS Tmp;");
                    Sql.AppendLine("");
                    //==============================删除基础数据表==============================//
                    Sql.AppendLine("DROP TABLE IF EXISTS `lfex_member_ext_tmp`;");
                    //==============================执行SQL语句==============================//
                    var SqlString = Sql.ToString();
                    await SqlContext.Dapper.ExecuteAsync(SqlString, null, null, 1200);
                    //==============================修改正式表内SQL语句==============================//
                    SqlContext.Dapper.Execute($"UPDATE user_ext AS E INNER JOIN `{TableName}` AS T ON E.userId=T.UserID SET E.bigCandyH=T.bigCandyH,E.littleCandyH=T.littleCandyH,E.teamStart=T.teamStart,E.updateTime=NOW();", null, null, 1200);
                    stopwatch.Stop();
                    Core.SystemLog.Jobs($"定时更新大区小区及星级 执行完成,执行时间:{stopwatch.Elapsed.TotalSeconds}秒");
                }
                catch (Exception ex)
                {
                    Core.SystemLog.Jobs("定时更新大区小区及星级 发生错误", ex);
                }
            }
        }

        public class UserTmpInfo
        {
            public int Id { get; set; }
            public decimal Balance { get; set; }
        }

        /// <summary>
        /// 团队星级
        /// </summary>
        public class TeamStar
        {
            /// <summary>
            /// 用户ID
            /// </summary>
            public long UserId { get; set; }
            /// <summary>
            /// 团队星级
            /// </summary>
            public int TeamStart { get; set; }
            /// <summary>
            /// 直推星级
            /// </summary>
            public int StartLevel { get; set; }
            /// <summary>
            /// 直推星级数量
            /// </summary>
            public int StartCount { get; set; }
        }
    }
}
