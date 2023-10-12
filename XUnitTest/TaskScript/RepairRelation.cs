using CSRedis;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Yoyo.Core.Expand;

namespace XUnitTest
{
    public class RepairRelation
    {
        private readonly IServiceProvider ServiceProvider;
        public RepairRelation()
        {
            CommServiceProvider comm = new CommServiceProvider();
            ServiceProvider = comm.GetServiceProvider();
        }


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

                List<MemberRelation> RelationList = new List<MemberRelation>();
                Dictionary<long, List<long>> RelationTmp = new Dictionary<long, List<long>>();

                #region 修复推荐关系
                //==============================修复推荐关系==============================//
                List<UserInviteInfo> UserInfos = SqlContext.Dapper.Query<UserInviteInfo>("SELECT U.id AS UserId,IFNULL(UP.UpId,2) AS InviteUserId FROM `user` AS U LEFT JOIN (SELECT mobile AS UpTel,id AS UpId FROM `user`) AS UP ON U.inviterMobile=UP.UpTel WHERE U.id>1 ORDER BY id", null, null, true, 3000).ToList();

                RelationList.Add(new MemberRelation()
                {
                    MemberId = 0,
                    ParentId = 0,
                    RelationLevel = 1,
                    Topology = "",
                    CreateTime = DateTime.Now
                });
                RelationTmp.Add(0, new List<long>());

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
                    Topology.Remove(0);
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

                #region 修复团队数据
                //==============================团队数据重置==============================//
                await SqlContext.Dapper.ExecuteAsync("UPDATE `user_ext` SET `teamCount`=0,`teamCandyH`=0,`authCount`=0");

                //==============================修复直推用户==============================//
                await Team.UpdateTeamDirectPersonnel(null, Yoyo.Entity.Utils.MemberAuthStatus.CERTIFIED);

                //==============================修复团队数据==============================//
                var UserRelation = RelationList.OrderBy(o => o.MemberId).ToList();
                var Users = SqlContext.Dapper.Query<long>("SELECT u.id FROM `user` AS u INNER JOIN (SELECT userId FROM minnings WHERE  TO_DAYS(endTime)-TO_DAYS(NOW())>0 AND `status`=1  GROUP BY userId) AS m ON u.id=m.userId WHERE u.`auditState`=2 AND u.`status`=0;", null, null, true, 3000).ToList();
                int SqlCount = 0;
                StringBuilder TeamRepairSql = new StringBuilder();
                foreach (var item in UserRelation)
                {
                    if (String.IsNullOrWhiteSpace(item.Topology)) { continue; }
                    String TmpSql = "UPDATE `user_ext` SET `teamCount`=`teamCount`+1,{0}`updateTime`=NOW() WHERE `userId` IN ({1})";
                    String TeamCandy = "";
                    if (Users.Contains(item.MemberId)) { TeamCandy = "`teamCandyH`=`teamCandyH`+1,"; }
                    var SqlE = String.Format(TmpSql, TeamCandy, item.Topology);
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

                #region 修复任务数据
                //==============================任务数据修复==============================//
                List<MemberTaskInfo> UserTasks = SqlContext.Dapper.Query<MemberTaskInfo>("SELECT COUNT(id) AS Total,userId AS UserId,minningId AS MinningId FROM `minnings` WHERE TO_DAYS(endTime)-TO_DAYS(NOW())>0 AND `status`=1 AND source<=1 AND minningId NOT IN (0,6,16) GROUP BY userId,minningId HAVING Total>0 AND Total<11 ORDER BY userId", null, null, true, 3000).ToList();

                int TaskSqlCount = 0;
                StringBuilder TaskSql = new StringBuilder();
                String FormatTaskSql = "UPDATE `user_ext` SET `teamCandyH`=(`teamCandyH`+{0}),updateTime=NOW() WHERE `userId` IN ({1});";
                foreach (var item in UserTasks.GroupBy(o => o.UserId).Select(e => e.Key))
                {
                    string UserRelationTopology = RelationList.FirstOrDefault(e => e.MemberId == item)?.Topology;
                    if (String.IsNullOrWhiteSpace(UserRelationTopology)) { continue; }
                    List<MemberTaskInfo> UserTask = UserTasks.Where(o => o.UserId == item).ToList();
                    decimal TeamCandyH = 0M;
                    foreach (var User in UserTask)
                    {
                        switch (User.MinningId)
                        {
                            case 0: TeamCandyH += User.Total * 1M; break;
                            case 1: TeamCandyH += User.Total * 2.3M; break;
                            case 2: TeamCandyH += User.Total * 24; break;
                            case 3: TeamCandyH += User.Total * 75; break;
                            case 4: TeamCandyH += User.Total * 460; break;
                            case 5: TeamCandyH += User.Total * 182; break;
                            case 6: TeamCandyH += User.Total * 1M; break;
                            case 7: TeamCandyH += User.Total * 1.1M; break;
                            case 8: TeamCandyH += User.Total * 1400; break;
                            case 9: TeamCandyH += User.Total * 4733; break;
                            case 10: TeamCandyH += User.Total * 11.6M; break;
                            case 11: TeamCandyH += User.Total * 2M; break;
                            case 12: TeamCandyH += User.Total * 5M; break;
                            case 13: TeamCandyH += User.Total * 15M; break;
                            case 14: TeamCandyH += User.Total * 45M; break;
                            case 15: TeamCandyH += User.Total * 0.2M; break;
                            case 16: TeamCandyH += User.Total * 0.4M; break;
                            case 17: TeamCandyH += User.Total * 0.4M; break;
                            case 18: TeamCandyH += User.Total * 10M; break;
                            case 100: TeamCandyH += User.Total * 0.4M; break;
                            case 101: TeamCandyH += User.Total * 4.1M; break;
                            case 102: TeamCandyH += User.Total * 16.6M; break;
                            case 103: TeamCandyH += User.Total * 42M; break;
                            case 104: TeamCandyH += User.Total * 166M; break;
                            case 105: TeamCandyH += User.Total * 460M; break;

                            default: break;
                        }
                    }
                    if (TeamCandyH > 0)
                    {
                        TaskSql.AppendLine(String.Format(FormatTaskSql, TeamCandyH, UserRelationTopology));
                        TaskSqlCount++;
                        if (TaskSqlCount == SqlCountRun)
                        {
                            await SqlContext.Dapper.ExecuteAsync(TaskSql.ToString(), null, null, 3000);
                            TaskSql.Clear();
                            TaskSqlCount = 0;
                        }
                    }
                }
                await SqlContext.Dapper.ExecuteAsync(TaskSql.ToString(), null, null, 3000);
                #endregion

                #region 修复星级大小区数据


                var TableName = "yoyo_member_dividend_tmp_use";

                //==============================创建基础数据表==============================//
                StringBuilder StarSql = new StringBuilder();
                StarSql.AppendLine("DROP TABLE IF EXISTS `yoyo_member_ext_tmp`;");
                StarSql.AppendLine("CREATE TABLE `yoyo_member_ext_tmp` (");
                StarSql.AppendLine("  `Id` bigint(20) NOT NULL,");
                StarSql.AppendLine("  `UserID` bigint(20) NOT NULL,");
                StarSql.AppendLine("  `ParentId` bigint(20) NOT NULL,");
                StarSql.AppendLine("  `teamStart` int(11) NOT NULL,");
                StarSql.AppendLine("  `teamCount` int(11) NOT NULL,");
                StarSql.AppendLine("  `authCount` int(11) NOT NULL,");
                StarSql.AppendLine("  `teamCandyH` int(11) NOT NULL,");
                StarSql.AppendLine("  `bigCandyH` int(11) NOT NULL,");
                StarSql.AppendLine("  `littleCandyH` int(11) NOT NULL,");
                StarSql.AppendLine("  PRIMARY KEY (`UserID`),");
                StarSql.AppendLine("	KEY `FK_Id` (`Id`) USING BTREE,");
                StarSql.AppendLine("  KEY `FK_ParentId` (`ParentId`) USING BTREE,");
                StarSql.AppendLine("  KEY `FK_teamCandyH` (`teamCandyH`) USING BTREE");
                StarSql.AppendLine(") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;");
                StarSql.AppendLine("");
                //==============================拷贝基础数据==============================//
                StarSql.AppendLine("TRUNCATE TABLE yoyo_member_ext_tmp;");
                StarSql.AppendLine("INSERT INTO yoyo_member_ext_tmp SELECT ");
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
                StarSql.AppendLine("  `teamCandyH` int(11) NOT NULL DEFAULT '0',");
                StarSql.AppendLine("  `bigCandyH` int(11) NOT NULL DEFAULT '0',");
                StarSql.AppendLine("  `littleCandyH` int(11) NOT NULL DEFAULT '0',");
                StarSql.AppendLine("  PRIMARY KEY (`UserId`)");
                StarSql.AppendLine(") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;");
                //==============================更新星级和大小区数据==============================//
                StarSql.AppendLine($"TRUNCATE TABLE {TableName};");
                StarSql.AppendLine($"INSERT INTO {TableName} SELECT ");
                StarSql.AppendLine("Tmp.UserId,");
                StarSql.AppendLine("(CASE");
                StarSql.AppendLine("  WHEN Tmp.authCount>=20 AND Tmp.teamCandyH>=1000000 AND IF(Tmp.bigCandyH>Tmp.littleCandyH,Tmp.littleCandyH,Tmp.bigCandyH)>=250000 THEN 5");
                StarSql.AppendLine("  WHEN Tmp.authCount>=20 AND Tmp.teamCandyH>=100000 AND IF(Tmp.bigCandyH>Tmp.littleCandyH,Tmp.littleCandyH,Tmp.bigCandyH)>=25000 THEN 4");
                StarSql.AppendLine("  WHEN Tmp.authCount>=20 AND Tmp.teamCandyH>=8000 AND IF(Tmp.bigCandyH>Tmp.littleCandyH,Tmp.littleCandyH,Tmp.bigCandyH)>=2000 THEN 3");
                StarSql.AppendLine("  WHEN Tmp.authCount>=20 AND Tmp.teamCandyH>=2000 AND IF(Tmp.bigCandyH>Tmp.littleCandyH,Tmp.littleCandyH,Tmp.bigCandyH)>=400 THEN 2");
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
                StarSql.AppendLine("FROM yoyo_member_ext_tmp AS A LEFT JOIN (");
                StarSql.AppendLine("SELECT A.ParentId AS UserID,SUM(A.teamCandyH) AS BigCandyH FROM (SELECT * FROM yoyo_member_ext_tmp WHERE teamCandyH>0) AS A");
                StarSql.AppendLine("  WHERE (");
                StarSql.AppendLine("    SELECT COUNT(1) FROM (SELECT * FROM yoyo_member_ext_tmp WHERE teamCandyH>0) AS B");
                StarSql.AppendLine("    WHERE B.ParentId=A.ParentId AND B.Id<A.Id");
                StarSql.AppendLine("   )<2 ");
                StarSql.AppendLine(" GROUP BY A.ParentId) AS B ON A.UserID=B.UserID");
                StarSql.AppendLine(")AS Tmp;");
                StarSql.AppendLine("");
                //==============================执行SQL语句==============================//
                var SqlString = StarSql.ToString();
                await SqlContext.Dapper.ExecuteAsync(SqlString, null, null, 3000);
                #endregion

                //==============================更新数据==============================//
                await SqlContext.Dapper.ExecuteAsync("DROP TABLE IF EXISTS `yoyo_member_ext_tmp`;");
                await SqlContext.Dapper.ExecuteAsync($"UPDATE user_ext AS E INNER JOIN `{TableName}` AS T ON E.userId=T.UserID SET E.bigCandyH=T.bigCandyH,E.littleCandyH=T.littleCandyH,E.teamStart=T.teamStart,E.updateTime=NOW();", null, null, 3000);

                //==============================删除基础数据表==============================//
                await SqlContext.Dapper.ExecuteAsync($"DROP TABLE IF EXISTS `{TableName}`;");

                //==============================重新计算等级==============================//


                stopwatch.Stop();
                var TotalTime = stopwatch.Elapsed.TotalMinutes;
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
    }
}
