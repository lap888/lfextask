using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Yoyo.IServices.IMember
{
    /// <summary>
    /// 团队操作
    /// </summary>
    public interface ITeams
    {
        /// <summary>
        /// 更新团队人数
        /// </summary>
        /// <param name="Uid">用户ID</param>
        /// <param name="Number">变更人员数量(默认+1)</param>
        /// <returns></returns>
        Task<bool> UpdateTeamPersonnel(long Uid, int Number = 1);

        /// <summary>
        /// 更新团队果核
        /// </summary>
        /// <param name="Uid">用户ID</param>
        /// <param name="Quantity">更新数量(默认+1)</param>
        /// <returns></returns>
        Task<bool> UpdateTeamKernel(long Uid, decimal Quantity = 1);

        /// <summary>
        /// 更新某个团队的直推人数
        /// </summary>
        /// <param name="Uid">用户ID(不传或NULL则更新所有用户)</param>
        /// <param name="Status">计入直推人数的用户状态(默认:已认证)</param>
        /// <returns></returns>
        Task<bool> UpdateTeamDirectPersonnel(long? Uid, Entity.Utils.MemberAuthStatus? Status = Entity.Utils.MemberAuthStatus.CERTIFIED);

        /// <summary>
        /// 获取用户推荐关系
        /// </summary>
        /// <param name="Uid">用户ID</param>
        /// <returns></returns>
        Task<Response.RspMemberRelation> GetRelation(long Uid);

        /// <summary>
        /// 设置用户推荐关系
        /// </summary>
        /// <param name="Uid">用户ID</param>
        /// <param name="PUid">父级推荐人ID</param>
        /// <returns></returns>
        Task<Response.RspMemberRelation> SetRelation(long Uid, long PUid);
    }
}