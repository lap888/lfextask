using Dapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoyo.Core.Expand;
using Yoyo.IServices.Response;
using Yoyo.IServices.Utils;

namespace Yoyo.Service.Member
{
    /// <summary>
    /// 团队操作
    /// </summary>
    public class Teams : IServices.IMember.ITeams
    {
        private readonly Entity.SqlContext SqlContext;
        public Teams(Entity.SqlContext sql)
        {
            this.SqlContext = sql;
        }

        /// <summary>
        /// 更新团队人数
        /// </summary>
        /// <param name="Uid">用户ID</param>
        /// <param name="Number">变更人员数量(默认+1)</param>
        /// <returns></returns>
        public async Task<bool> UpdateTeamPersonnel(long Uid, int Number = 1)
        {
            if (Number == 0) { return true; }
            var TeamUsers = await this.GetRelation(Uid);
            if (String.IsNullOrWhiteSpace(TeamUsers.TopologyString)) { return true; }
            String Sql = $"UPDATE `user_ext` SET `teamCount`=`teamCount`+{Number},`updateTime`='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' WHERE (`teamCount`+{Number}>=0) AND `userId` IN ({TeamUsers.TopologyString})";
            return (await this.SqlContext.Dapper.ExecuteAsync(Sql)) > 0;
        }

        /// <summary>
        /// 更新团队果核
        /// </summary>
        /// <param name="Uid">用户ID</param>
        /// <param name="Quantity">更新数量(默认+1)</param>
        /// <returns></returns>
        public async Task<bool> UpdateTeamKernel(long Uid, decimal Quantity = 1)
        {
            if (Quantity == 0) { return true; }
            var TeamUsers = await this.GetRelation(Uid);
            if (String.IsNullOrWhiteSpace(TeamUsers.TopologyString)) { return true; }
            String Sql = $"UPDATE `user_ext` SET `teamCandyH`=`teamCandyH`+{Quantity},`updateTime`='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' WHERE (`teamCandyH`+{Quantity}>=0) AND `userId` IN ({TeamUsers.TopologyString})";
            return (await this.SqlContext.Dapper.ExecuteAsync(Sql)) > 0;
        }

        /// <summary>
        /// 更新团队的直推人数
        /// </summary>
        /// <param name="Uid">用户ID(不传则更新所有用户)</param>
        /// <param name="Status">计入直推人数的用户状态(默认:已认证)</param>
        /// <returns></returns>
        public async Task<bool> UpdateTeamDirectPersonnel(long? Uid, Entity.Utils.MemberAuthStatus? Status = Entity.Utils.MemberAuthStatus.CERTIFIED)
        {
            StringBuilder Sql = new StringBuilder();
            Sql.AppendLine("UPDATE `user_ext` AS `E` ");
            Sql.AppendLine("INNER JOIN (");
            Sql.AppendLine("SELECT `R`.`ParentId` AS `Uid`,COUNT(`R`.`ParentId`) AS `Total` ");
            Sql.AppendLine("FROM `user` AS `U` ");
            Sql.AppendLine("LEFT JOIN `yoyo_member_relation` AS `R` ");
            Sql.AppendLine("ON `U`.`id`=`R`.`MemberId` ");
            Sql.AppendLine("WHERE 1=1 ");

            //==============================插入条件==============================//
            if (null != Status) { Sql.AppendLine($"AND `U`.`auditState`={(int)Status} "); }
            if (null != Uid) { Sql.AppendLine($" AND `R`.`ParentId`={Uid} "); }
            //==============================结束插入==============================//

            Sql.AppendLine("GROUP BY `R`.`ParentId`");
            Sql.AppendLine(") AS `T` ");
            Sql.AppendLine("ON `E`.`userId`=`T`.`Uid` ");
            Sql.AppendLine("SET ");
            Sql.AppendLine("`E`.`authCount`=`T`.`Total`,");
            Sql.AppendLine($"`E`.`updateTime`='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'");

            return (await this.SqlContext.Dapper.ExecuteAsync(Sql.ToString())) > 0;
        }

        /// <summary>
        /// 获取用户拓扑关系
        /// </summary>
        /// <param name="Uid"></param>
        /// <returns></returns>
        public async Task<RspMemberRelation> GetRelation(long Uid)
        {
            Entity.Models.MemberRelation relation = await this.SqlContext.MemberRelation.FirstOrDefaultAsync(o => o.MemberId == Uid);
            if (null == relation)
            {
                //==============尝试修复推荐关系==============//
                long? ParentId = await SqlContext.Dapper.QueryFirstOrDefaultAsync<long?>($"SELECT `MemberId` AS `MemberId` FROM `yoyo_member_relation` WHERE `MemberId`=(SELECT `id` FROM `user` WHERE `mobile`=(SELECT `inviterMobile` FROM `user` WHERE `id`={Uid}))");
                if (ParentId == null) { ParentId = 0; }
                return await SetRelation(Uid, ParentId.Value);
            }
            return new RspMemberRelation
            {
                MemberId = relation.MemberId,
                ParentId = relation.ParentId,
                RelationLevel = relation.RelationLevel,
                CreateTime = relation.CreateTime,
                TopologyString = relation.Topology,
                Topology = String.IsNullOrWhiteSpace(relation.Topology) ? new List<long>() : relation.Topology.Split(",").ToList().ConvertAll(x => x.ToLong())
            };
        }

        /// <summary>
        /// 设置用户拓扑关系
        /// </summary>
        /// <param name="Uid"></param>
        /// <param name="PUid"></param>
        /// <returns></returns>
        public async Task<RspMemberRelation> SetRelation(long Uid, long PUid)
        {
            Entity.Models.MemberRelation relation = await this.SqlContext.MemberRelation.FirstOrDefaultAsync(o => o.MemberId == Uid);
            if (null != relation)
            {
                return new RspMemberRelation
                {
                    MemberId = relation.MemberId,
                    ParentId = relation.ParentId,
                    RelationLevel = relation.RelationLevel,
                    CreateTime = relation.CreateTime,
                    TopologyString = relation.Topology,
                    Topology = String.IsNullOrWhiteSpace(relation.Topology) ? new List<long>() : relation.Topology.Split(",").ToList().ConvertAll(x => x.ToLong())
                };
            }
            RspMemberRelation parentRelation = new RspMemberRelation();

            if (PUid != 0)
            {
                parentRelation = await this.GetRelation(PUid);
                parentRelation.Topology.Add(PUid);
                parentRelation.Topology.Remove(0);
            }


            try
            {
                relation = this.SqlContext.MemberRelation.Add(new Entity.Models.MemberRelation
                {
                    MemberId = Uid,
                    ParentId = PUid,
                    RelationLevel = parentRelation.Topology.Count + 1,
                    CreateTime = DateTime.Now,
                    Topology = string.Join(",", parentRelation.Topology)
                }).Entity;

                if (this.SqlContext.SaveChanges() < 1) { ServiceCode.USER_SET_RELATION_FAIL.Throw(); }
            }
            catch (Exception ex) { ServiceCode.USER_SET_RELATION_FAIL.Throw(ex); }

            return new RspMemberRelation
            {
                MemberId = relation.MemberId,
                ParentId = relation.ParentId,
                RelationLevel = relation.RelationLevel,
                CreateTime = relation.CreateTime,
                TopologyString = relation.Topology,
                Topology = String.IsNullOrWhiteSpace(relation.Topology) ? new List<long>() : relation.Topology.Split(",").ToList().ConvertAll(x => x.ToLong())
            };
        }
    }
}
