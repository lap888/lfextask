using CSRedis;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Yoyo.Core.Expand;

namespace XUnitTest
{
    public class RepairTeamData
    {
        private readonly IServiceProvider ServiceProvider;
        CSRedisClient RedisCache = null;
        public RepairTeamData()
        {
            CommServiceProvider comm = new CommServiceProvider();
            ServiceProvider = comm.GetServiceProvider();
        }


        /// <summary>
        /// 修复推荐关系
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task RepairLfexUP()
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                int SqlCountRun = 3000;
                Yoyo.IServices.IMember.ITeams Team = this.ServiceProvider.GetService<Yoyo.IServices.IMember.ITeams>();
                Yoyo.Entity.SqlContext SqlContext = this.ServiceProvider.GetService<Yoyo.Entity.SqlContext>();

                List<MemberRelation> RelationList = new List<MemberRelation>();
                Dictionary<long, List<long>> RelationTmp = new Dictionary<long, List<long>>();

                #region 修复推荐关系
                //==============================修复推荐关系==============================//
                List<UserInviteInfo> UserInfos = SqlContext.Dapper.Query<UserInviteInfo>("SELECT U.id AS UserId,IFNULL(UP.UpId,1) AS InviteUserId FROM `user` AS U LEFT JOIN (SELECT mobile AS UpTel,id AS UpId FROM `user`) AS UP ON U.inviterMobile=UP.UpTel WHERE U.id>1 ORDER BY id", null, null, true, 3000).ToList();

                RelationList.Add(new MemberRelation()
                {
                    MemberId = 1,
                    ParentId = 0,
                    RelationLevel = 1,
                    Topology = "",
                    CreateTime = DateTime.Now
                });
                RelationTmp.Add(1, new List<long>());

                foreach (var item in UserInfos)
                {
                    List<long> Topology = new List<long>();
                    if (RelationTmp.ContainsKey(item.InviteUserId))
                    {
                        Topology.Add(item.InviteUserId);
                        Topology = Topology.Concat(RelationTmp[item.InviteUserId]).ToList();
                    }
                    else
                    {
                        List<long> T = new List<long>();
                        var Tmp = GetUp(UserInfos, item.InviteUserId, T);
                        Topology = Topology.Concat(Tmp).ToList();
                    }
                    var TmpToplogy = Topology.OrderBy(o => o).ToList();
                    RelationTmp.Add(item.UserId, TmpToplogy);

                    MemberRelation Relation = new MemberRelation()
                    {
                        MemberId = item.UserId,
                        ParentId = item.InviteUserId,
                        RelationLevel = Topology.Count + 1,
                        Topology = string.Join(",", TmpToplogy),
                        CreateTime = DateTime.Now
                    };
                    RelationList.Add(Relation);
                }

                //==============================清空推荐关系==============================//
                await SqlContext.Dapper.ExecuteAsync("TRUNCATE TABLE `yoyo_member_relation`");

                int RunCount = 0;
                var RelationSqlTitle = "INSERT INTO `yoyo_member_relation`(`MemberId`, `ParentId`,`RelationLevel`, `Topology`, `CreateTime`) VALUES ";
                StringBuilder RelationSql = new StringBuilder();

                foreach (var item in RelationList)
                {

                    if (RunCount == SqlCountRun)
                    {
                        await SqlContext.Dapper.ExecuteAsync(RelationSqlTitle + RelationSql.ToString().TrimEnd(','), null, null, 3000);
                        RelationSql.Clear();
                        RunCount = 0;
                    }
                    RelationSql.Append($"\r\n({item.MemberId}, {item.ParentId},{item.RelationLevel}, '{item.Topology}', NOW()),");
                    RunCount++;
                }
                await SqlContext.Dapper.ExecuteAsync(RelationSqlTitle + RelationSql.ToString().TrimEnd(','), null, null, 3000);
                #endregion
                stopwatch.Stop();
                var TotalTime = stopwatch.Elapsed.TotalMinutes;
                System.Console.WriteLine(TotalTime);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex);
                throw;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Repair()
        {

            //============stopwatch 开始断点，结束断点。
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                int SqlCountRun = 3000;
                Yoyo.IServices.IMember.ITeams Team = this.ServiceProvider.GetService<Yoyo.IServices.IMember.ITeams>();
                Yoyo.Entity.SqlContext SqlContext = this.ServiceProvider.GetService<Yoyo.Entity.SqlContext>();

                List<MemberRelation> RelationList = (await SqlContext.Dapper.QueryAsync<MemberRelation>($"select * from `yoyo_member_relation`")).ToList();

                #region 修复团队数据
                //==============================团队数据重置==============================//
                await SqlContext.Dapper.ExecuteAsync("UPDATE `user_ext` SET `teamCount`=0,`teamCandyH`=0,`authCount`=0");

                //==============================修复直推用户==============================//
                await Team.UpdateTeamDirectPersonnel(null, Yoyo.Entity.Utils.MemberAuthStatus.CERTIFIED);

                //==============================修复团队数据==============================//
                var UserRelation = RelationList.OrderBy(o => o.MemberId).ToList();
                var Users = SqlContext.Dapper.Query<UserTmpInfo>("SELECT u.id,m.`Balance` FROM `user` AS u left join (select Balance,UserId userId from `user_account_wallet` where `CoinType`='LF') AS m ON u.id=m.userId WHERE u.`auditState`=2 AND u.`status`=0", null, null, true, 3000).ToList();
                int SqlCount = 0;
                StringBuilder TeamRepairSql = new StringBuilder();
                foreach (var item in UserRelation)
                {
                    if (String.IsNullOrWhiteSpace(item.Topology)) { continue; }
                    var tmpUserInfo = Users.FirstOrDefault(t => t.Id == item.MemberId);

                    String TmpSql = "UPDATE `user_ext` SET `teamCount`=`teamCount`+1,`teamCandyH`=(`teamCandyH`+{0}),`updateTime`=NOW() WHERE `userId` IN ({1})";
                    var SqlE = String.Format(TmpSql, tmpUserInfo?.Balance ?? 0, item.Topology);
                    SqlCount++;
                    if (SqlCount == SqlCountRun)
                    {
                        var SqlStringE = TeamRepairSql.ToString();
                        await SqlContext.Dapper.ExecuteAsync(SqlStringE, null, null, 3000);
                        TeamRepairSql.Clear();
                        SqlCount = 0;
                    }
                    TeamRepairSql.AppendLine(SqlE + ";");
                }
                await SqlContext.Dapper.ExecuteAsync(TeamRepairSql.ToString(), null, null, 3000);
                #endregion

                #region 修复星级大小区数据
                var TableName = "lfex_member_dividend_tmp_use";

                //==============================创建基础数据表==============================//
                StringBuilder StarSql = new StringBuilder();
                StarSql.AppendLine("DROP TABLE IF EXISTS `lfex_member_ext_tmp`;");
                StarSql.AppendLine("CREATE TABLE `lfex_member_ext_tmp` (");
                StarSql.AppendLine("  `Id` bigint(20) NOT NULL,");
                StarSql.AppendLine("  `UserID` bigint(20) NOT NULL,");
                StarSql.AppendLine("  `ParentId` bigint(20) NOT NULL,");
                StarSql.AppendLine("  `teamStart` int(11) NOT NULL,");
                StarSql.AppendLine("  `teamCount` int(11) NOT NULL,");
                StarSql.AppendLine("  `authCount` int(11) NOT NULL,");
                StarSql.AppendLine("  `teamCandyH` DECIMAL(11,4) NOT NULL,");
                StarSql.AppendLine("  `bigCandyH` DECIMAL(11,4) NOT NULL,");
                StarSql.AppendLine("  `littleCandyH` DECIMAL(11,4) NOT NULL,");
                StarSql.AppendLine("  PRIMARY KEY (`UserID`),");
                StarSql.AppendLine("	KEY `FK_Id` (`Id`) USING BTREE,");
                StarSql.AppendLine("  KEY `FK_ParentId` (`ParentId`) USING BTREE,");
                StarSql.AppendLine("  KEY `FK_teamCandyH` (`teamCandyH`) USING BTREE");
                StarSql.AppendLine(") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;");
                StarSql.AppendLine("");
                //==============================拷贝基础数据==============================//
                StarSql.AppendLine("TRUNCATE TABLE lfex_member_ext_tmp;");
                StarSql.AppendLine("INSERT INTO lfex_member_ext_tmp SELECT ");
                StarSql.AppendLine("A.Id,A.UserID,R.ParentId,0 AS teamStart,A.teamCount,A.authCount,A.teamCandyH,0 AS bigCandyH,0 AS littleCandyH FROM (");
                StarSql.AppendLine("SELECT (@i:=@i+1) AS Id,ext.userId AS UserID,ext.authCount,ext.teamCount,ext.teamCandyH FROM user_ext AS ext,(SELECT @i:=0) AS Ids ORDER BY ext.teamCandyH DESC) AS A");
                StarSql.AppendLine("INNER JOIN (SELECT MemberId,ParentId FROM yoyo_member_relation) AS R ON A.userId=R.MemberId");
                StarSql.AppendLine("ORDER BY A.Id;");
                StarSql.AppendLine("");
                //==============================创建分红基础表==============================//
                StarSql.AppendLine($"DROP TABLE IF EXISTS `{TableName}`;");
                StarSql.AppendLine($"CREATE TABLE `{TableName}` (");
                StarSql.AppendLine("  `UserId` bigint(20) NOT NULL,");
                StarSql.AppendLine("  `teamStart` int(11) NOT NULL DEFAULT '0',");
                StarSql.AppendLine("  `teamCandyH` DECIMAL(11,4) NOT NULL DEFAULT '0',");
                StarSql.AppendLine("  `bigCandyH` DECIMAL(11,4) NOT NULL DEFAULT '0',");
                StarSql.AppendLine("  `littleCandyH` DECIMAL(11,4) NOT NULL DEFAULT '0',");
                StarSql.AppendLine("  PRIMARY KEY (`UserId`)");
                StarSql.AppendLine(") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;");
                //==============================更新星级和大小区数据==============================//
                StarSql.AppendLine($"TRUNCATE TABLE {TableName};");
                StarSql.AppendLine($"INSERT INTO {TableName} SELECT ");
                StarSql.AppendLine("Tmp.UserId,");
                StarSql.AppendLine("(CASE");
                StarSql.AppendLine("  WHEN Tmp.authCount>=20 AND Tmp.teamCandyH>=250000 AND IF(Tmp.bigCandyH>Tmp.littleCandyH,Tmp.littleCandyH,Tmp.bigCandyH)>=50000 THEN 4");
                StarSql.AppendLine("  WHEN Tmp.authCount>=20 AND Tmp.teamCandyH>=100000 AND IF(Tmp.bigCandyH>Tmp.littleCandyH,Tmp.littleCandyH,Tmp.bigCandyH)>=20000 THEN 3");
                StarSql.AppendLine("  WHEN Tmp.authCount>=20 AND Tmp.teamCandyH>=20000 AND IF(Tmp.bigCandyH>Tmp.littleCandyH,Tmp.littleCandyH,Tmp.bigCandyH)>=5000 THEN 2");
                StarSql.AppendLine("  WHEN Tmp.authCount>=20 AND Tmp.teamCandyH>=500 THEN 1");
                StarSql.AppendLine("  ELSE 0");
                StarSql.AppendLine("END)AS teamStart,");
                StarSql.AppendLine("Tmp.teamCandyH,");
                StarSql.AppendLine("IF(Tmp.bigCandyH<Tmp.littleCandyH,Tmp.littleCandyH,Tmp.bigCandyH) AS bigCandyH,");
                StarSql.AppendLine("IF(Tmp.bigCandyH>Tmp.littleCandyH,Tmp.littleCandyH,Tmp.bigCandyH) AS littleCandyH");
                StarSql.AppendLine("FROM (");
                StarSql.AppendLine("SELECT ");
                StarSql.AppendLine("A.UserID,A.authCount,A.teamCandyH,");
                StarSql.AppendLine("IFNULL(B.BigCandyH,0) AS bigCandyH,");
                StarSql.AppendLine("IF(A.teamCandyH-IFNULL(B.BigCandyH,0)<0,0,A.teamCandyH-IFNULL(B.BigCandyH,0)) AS littleCandyH");
                StarSql.AppendLine("FROM lfex_member_ext_tmp AS A LEFT JOIN (");
                StarSql.AppendLine("SELECT A.ParentId AS UserID,SUM(A.teamCandyH) AS BigCandyH FROM (SELECT * FROM lfex_member_ext_tmp WHERE teamCandyH>0) AS A");
                StarSql.AppendLine("  WHERE (");
                StarSql.AppendLine("    SELECT COUNT(1) FROM (SELECT * FROM lfex_member_ext_tmp WHERE teamCandyH>0) AS B");
                StarSql.AppendLine("    WHERE B.ParentId=A.ParentId AND B.Id<=A.Id");
                StarSql.AppendLine("   )<2 ");
                StarSql.AppendLine(" GROUP BY A.ParentId) AS B ON A.UserID=B.UserID");
                StarSql.AppendLine(")AS Tmp;");
                StarSql.AppendLine("");
                //==============================执行SQL语句==============================//
                var SqlString = StarSql.ToString();
                await SqlContext.Dapper.ExecuteAsync(SqlString, null, null, 3000);
                #endregion

                //==============================更新数据==============================//
                await SqlContext.Dapper.ExecuteAsync("DROP TABLE IF EXISTS `lfex_member_ext_tmp`;");
                await SqlContext.Dapper.ExecuteAsync($"UPDATE user_ext AS E INNER JOIN `{TableName}` AS T ON E.userId=T.UserID SET E.bigCandyH=T.bigCandyH,E.littleCandyH=T.littleCandyH,E.teamStart=T.teamStart,E.updateTime=NOW();", null, null, 3000);

                //==============================删除基础数据表==============================//
                await SqlContext.Dapper.ExecuteAsync($"DROP TABLE IF EXISTS `{TableName}`;");


                stopwatch.Stop();
                var TotalTime = stopwatch.Elapsed.TotalMinutes;
                System.Console.WriteLine(TotalTime);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                var zzz = ex;
            }
        }

        public class StarUser
        {
            public int UserId { get; set; }
            public int TeamStart { get; set; }
            public int TeamCandyH { get; set; }
            public int BigCandyH { get; set; }
            public int LittleCandyH { get; set; }
        }

        public class StarUserRelation
        {
            public long UserId { get; set; }
            public long ParentId { get; set; }
            public int TeamStart { get; set; }
            public string Topology { get; set; }
            public List<long> UserRelation { get; set; }
        }

        public List<long> GetUp(List<UserInviteInfo> users, long id, List<long> vs)
        {
            var user = users.FirstOrDefault(o => o.UserId == id);
            if (null != user && !vs.Contains(user.UserId))
            {
                vs.Add(user.UserId);
                GetUp(users, user.InviteUserId, vs);
            }
            return vs;
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

        public class MemberTaskInfo
        {
            public int MinningId { get; set; }
            public long UserId { get; set; }
            public int Total { get; set; }
        }

        public partial class MemberRelation
        {
            /// <summary>
            /// 会员ID
            /// </summary>
            public long MemberId { get; set; }
            /// <summary>
            /// 父级ID
            /// </summary>
            public long ParentId { get; set; }
            /// <summary>
            /// 关系层级
            /// </summary>
            public int RelationLevel { get; set; }
            /// <summary>
            /// 拓扑关系
            /// </summary>
            public string Topology { get; set; }
            /// <summary>
            /// 创建时间
            /// </summary>
            public DateTime CreateTime { get; set; }
        }

        public class UserInviteInfo
        {
            public long UserId { get; set; }

            public long InviteUserId { get; set; }
        }

        public class UserTmpInfo
        {

            public int Id { get; set; }
            public decimal Balance { get; set; }
        }
    }
}
