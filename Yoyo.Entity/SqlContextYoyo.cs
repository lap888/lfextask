using System;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Yoyo.Entity.Models;
using Yoyo.Entity.Models.luckdraw;

namespace Yoyo.Entity
{
    /// <summary>
    /// 数据库操作上下文
    /// </summary>
    public partial class SqlContextYoyo : DbContext
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private readonly String ConnectionString;
        /// <summary>
        /// 数据库
        /// </summary>
        private IDbConnection DbConnection;
        /// <summary>
        /// 数据库上下文
        /// </summary>
        /// <param name="options"></param>
        public SqlContextYoyo(DbContextOptions<SqlContextYoyo> options) : base(options)
        {
            try
            {
                this.ConnectionString = options.GetExtension<Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal.MySqlOptionsExtension>().ConnectionString;
            }
            catch (Exception)
            {
                this.ConnectionString = String.Empty;
            }
        }
        /// <summary>
        /// 启用Dapper
        /// </summary>
        public IDbConnection Dapper
        {
            get
            {
                if (null == DbConnection) { DbConnection = this.DapperConnection; }
                return DbConnection;
            }
        }
        /// <summary>
        /// 获取一个新的Dapper数据库连接
        /// </summary>
        public IDbConnection DapperConnection { get => new MySql.Data.MySqlClient.MySqlConnection(this.ConnectionString); }

        /// <summary>
        /// 股权账户
        /// </summary>
        public virtual DbSet<UserAccountEquity> UserAccountEquity { get; set; }
        /// <summary>
        /// 股权记录
        /// </summary>
        public virtual DbSet<UserAccountEquityRecord> UserAccountEquityRecord { get; set; }
        /// <summary>
        /// 用户信息拓展表
        /// </summary>
        public virtual DbSet<UserExt> UserExt { get; set; }
        /// <summary>
        /// 用户关系表
        /// </summary>
        public virtual DbSet<MemberRelation> MemberRelation { get; set; }
        /// <summary>
        /// 系统Banner
        /// </summary>
        public virtual DbSet<SysBanner> SysBanner { get; set; }
        /// <summary>
        /// 用户表
        /// </summary>
        public virtual DbSet<User> User { get; set; }
        /// <summary>
        /// 用户广告点击表
        /// </summary>
        public virtual DbSet<AdClick> AdClick { get; set; }
        /// <summary>
        /// 每日分红数据表
        /// </summary>
        public virtual DbSet<EverydayDividend> EverydayDividend { get; set; }
        /// <summary>
        /// 用户邀请排行榜
        /// </summary>
        public virtual DbSet<MemberInviteRanking> MemberInviteRanking { get; set; }
        /// <summary>
        /// 用户任务记录表
        /// </summary>
        public virtual DbSet<MemberDailyTask> MemberDailyTask { get; set; }
        /// <summary>
        /// 系统任务记录表
        /// </summary>
        public virtual DbSet<SystemTask> SystemTask { get; set; }
        /// <summary>
        /// 用户活跃
        /// </summary>
        public virtual DbSet<MemberActive> MemberActive { get; set; }
        /// <summary>
        /// 用户活跃贡献值
        /// </summary>
        public virtual DbSet<MemberDevote> MemberDevote { get; set; }
        /// <summary>
        /// 用户收购量
        /// </summary>
        public virtual DbSet<MemberDuplicate> MemberDuplicate { get; set; }

        public virtual DbSet<Yoyo_Luckydraw_Prize> Yoyo_Luckydraw_Prize { get; set; }
        public virtual DbSet<Yoyo_Luckydraw_Round> Yoyo_Luckydraw_Round { get; set; }
        public virtual DbSet<Yoyo_Luckydraw_User> Yoyo_Luckydraw_User { get; set; }
        public virtual DbSet<Yoyo_Luckydraw_DefaultUser> Yoyo_Luckydraw_DefaultUser { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserAccountEquity>(entity =>
            {
                entity.HasKey(e => e.AccountId);

                entity.ToTable("user_account_equity");

                entity.HasIndex(e => e.UserId)
                    .HasName("FK_UserId")
                    .IsUnique();

                entity.Property(e => e.AccountId).HasColumnType("bigint(20)");

                entity.Property(e => e.Balance).HasColumnType("decimal(18,5)");

                entity.Property(e => e.Expenses).HasColumnType("decimal(18,5)");

                entity.Property(e => e.Frozen).HasColumnType("decimal(18,5)");

                entity.Property(e => e.ModifyTime).HasColumnType("datetime");

                entity.Property(e => e.Revenue).HasColumnType("decimal(18,5)");

                entity.Property(e => e.UserId).HasColumnType("bigint(20)");
            });

            modelBuilder.Entity<UserAccountEquityRecord>(entity =>
            {
                entity.HasKey(e => e.RecordId);

                entity.ToTable("user_account_equity_record");

                entity.HasIndex(e => e.AccountId)
                    .HasName("FK_AccountId");

                entity.HasIndex(e => e.ModifyTime)
                    .HasName("FK_ModifyTime");

                entity.HasIndex(e => e.ModifyType)
                    .HasName("FK_ModifyType");

                entity.HasIndex(e => new { e.AccountId, e.ModifyType })
                    .HasName("NORMAL_AID_MT");

                entity.HasIndex(e => new { e.ModifyType, e.AccountId })
                    .HasName("NORMAL_MT_AID");

                entity.Property(e => e.RecordId).HasColumnType("bigint(20)");

                entity.Property(e => e.AccountId).HasColumnType("bigint(20)");

                entity.Property(e => e.Incurred).HasColumnType("decimal(12,5)");

                entity.Property(e => e.ModifyDesc)
                    .IsRequired()
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.ModifyTime).HasColumnType("datetime");

                entity.Property(e => e.ModifyType).HasColumnType("int(11)");

                entity.Property(e => e.PostChange).HasColumnType("decimal(12,5)");

                entity.Property(e => e.PreChange).HasColumnType("decimal(12,5)");
            });

            modelBuilder.Entity<MemberDuplicate>(entity =>
            {
                entity.ToTable("yoyo_member_duplicate");

                entity.HasIndex(e => new { e.Date, e.UserId })
                    .HasName("UK_Date_UserId")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Duplicate).HasColumnType("decimal(18,5)");

                entity.Property(e => e.UserId).HasColumnType("bigint(20)");
            });

            modelBuilder.Entity<MemberDevote>(entity =>
            {
                entity.ToTable("yoyo_member_devote");

                entity.HasIndex(e => new { e.UserId, e.DevoteDate })
                    .HasName("UK_UserId_DevoteDate")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Devote).HasColumnType("decimal(18,0)");

                entity.Property(e => e.DevoteDate).HasColumnType("date");

                entity.Property(e => e.UserId).HasColumnType("bigint(20)");
            });

            modelBuilder.Entity<UserExt>(entity =>
            {
                entity.ToTable("user_ext");

                entity.HasIndex(e => e.UserId)
                    .HasName("user_unique")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AuthCount)
                    .HasColumnName("authCount")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.BigCandyH)
                    .HasColumnName("bigCandyH")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("createTime")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.LittleCandyH)
                    .HasColumnName("littleCandyH")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.TeamCandyH)
                    .HasColumnName("teamCandyH")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.TeamCount)
                    .HasColumnName("teamCount")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.TeamStart)
                    .HasColumnName("teamStart")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("updateTime")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.UserId)
                    .HasColumnName("userId")
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("'0'");
            });

            modelBuilder.Entity<MemberRelation>(entity =>
            {
                entity.HasKey(e => e.MemberId);

                entity.ToTable("yoyo_member_relation");

                entity.HasIndex(e => e.ParentId)
                    .HasName("FK_ParentId");

                entity.Property(e => e.MemberId).HasColumnType("bigint(20)");

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.RelationLevel).HasColumnType("int(11)");

                entity.Property(e => e.ParentId).HasColumnType("bigint(20)");

                entity.Property(e => e.Topology)
                    .IsRequired()
                    .HasColumnType("text");
            });

            modelBuilder.Entity<SysBanner>(entity =>
            {
                entity.ToTable("sys_banner");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("createdAt")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ImageUrl)
                    .IsRequired()
                    .HasColumnName("imageUrl")
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Params)
                    .IsRequired()
                    .HasColumnName("params")
                    .HasColumnType("text");

                entity.Property(e => e.Queue)
                    .HasColumnName("queue")
                    .HasColumnType("tinyint(3)");

                entity.Property(e => e.Source)
                    .HasColumnName("source")
                    .HasColumnType("tinyint(3)");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("tinyint(3)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasColumnType("tinyint(3)");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.HasIndex(e => e.AuditState)
                    .HasName("audit_state");

                entity.HasIndex(e => e.Ctime)
                    .HasName("ctime");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.Mobile)
                    .HasName("mobile_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.Rcode)
                    .HasName("rcode_2");

                entity.HasIndex(e => new { e.InviterMobile, e.Mobile, e.Status })
                    .HasName("inviter_mobile");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Alipay)
                    .HasColumnName("alipay")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.AlipayUid)
                    .HasColumnName("alipayUid")
                    .HasColumnType("varchar(32)");

                entity.Property(e => e.AuditState)
                    .HasColumnName("auditState")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AvatarUrl)
                    .IsRequired()
                    .HasColumnName("avatarUrl")
                    .HasColumnType("varchar(225)")
                    .HasDefaultValueSql("'images/avatar/default/1.png'");

                entity.Property(e => e.CCount)
                    .HasColumnName("cCount")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.CandyNum)
                    .HasColumnName("candyNum")
                    .HasColumnType("decimal(18,8)")
                    .HasDefaultValueSql("'0.00000000'");

                entity.Property(e => e.CandyP)
                    .HasColumnName("candyP")
                    .HasColumnType("decimal(18,2)")
                    .HasDefaultValueSql("'0.00'");

                entity.Property(e => e.CnadyDoAt)
                    .HasColumnName("cnadyDoAt")
                    .HasColumnType("datetime");

                entity.Property(e => e.ContryCode)
                    .HasColumnName("contryCode")
                    .HasColumnType("varchar(8)");

                entity.Property(e => e.Ctime)
                    .HasColumnName("ctime")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.FreezeCandyNum)
                    .HasColumnName("freezeCandyNum")
                    .HasColumnType("decimal(18,8)")
                    .HasDefaultValueSql("'0.00000000'");

                entity.Property(e => e.Golds)
                    .HasColumnName("golds")
                    .HasColumnType("decimal(18,8)")
                    .HasDefaultValueSql("'0.00000000'");

                entity.Property(e => e.InviterMobile)
                    .HasColumnName("inviterMobile")
                    .HasColumnType("varchar(11)");

                entity.Property(e => e.Level)
                    .HasColumnName("level")
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasColumnType("varchar(11)");

                entity.Property(e => e.MonthlyTradeCount)
                    .HasColumnName("monthlyTradeCount")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Password)
                    .HasColumnName("password")
                    .HasColumnType("varchar(80)");

                entity.Property(e => e.PasswordSalt)
                    .HasColumnName("passwordSalt")
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.Rcode)
                    .HasColumnName("rcode")
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.TodayAvaiableGolds)
                    .HasColumnName("todayAvaiableGolds")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'5'");

                entity.Property(e => e.TradePwd)
                    .HasColumnName("tradePwd")
                    .HasColumnType("varchar(80)");

                entity.Property(e => e.Utime)
                    .HasColumnName("utime")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.Uuid)
                    .HasColumnName("uuid")
                    .HasColumnType("varchar(100)");
            });

            modelBuilder.Entity<AdClick>(entity =>
            {
                entity.ToTable("yoyo_ad_click");

                entity.HasIndex(e => new { e.ClickId, e.ClickDate })
                    .HasName("FK_ClickId_ClickDate")
                    .IsUnique();

                entity.HasIndex(e => new { e.UserId, e.ClickDate })
                    .HasName("FK_UserId,_ClickDate");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.AdId).HasColumnType("int(11)");

                entity.Property(e => e.CandyP).HasColumnType("decimal(18,2)");

                entity.Property(e => e.ClickDate).HasColumnType("date");

                entity.Property(e => e.ClickId)
                    .IsRequired()
                    .HasColumnType("varchar(64)");

                entity.Property(e => e.ClickTime).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnType("bigint(20)");
            });

            modelBuilder.Entity<EverydayDividend>(entity =>
            {
                entity.HasKey(e => e.DividendDate);

                entity.ToTable("yoyo_everyday_dividend");

                entity.Property(e => e.DividendDate).HasColumnType("date");

                entity.Property(e => e.CandyFee).HasColumnType("decimal(18,6)");

                entity.Property(e => e.People1).HasColumnType("int(11)");

                entity.Property(e => e.People2).HasColumnType("int(11)");

                entity.Property(e => e.People3).HasColumnType("int(11)");

                entity.Property(e => e.People4).HasColumnType("int(11)");

                entity.Property(e => e.People5).HasColumnType("int(11)");

                entity.Property(e => e.Star1).HasColumnType("decimal(18,6)");

                entity.Property(e => e.Star2).HasColumnType("decimal(18,6)");

                entity.Property(e => e.Star3).HasColumnType("decimal(18,6)");

                entity.Property(e => e.Star4).HasColumnType("decimal(18,6)");

                entity.Property(e => e.Star5).HasColumnType("decimal(18,6)");
            });

            modelBuilder.Entity<MemberInviteRanking>(entity =>
            {
                entity.ToTable("yoyo_member_invite_ranking");

                entity.HasIndex(e => new { e.Phase, e.UserId })
                    .HasName("FK_Phase_UserId")
                    .IsUnique();

                entity.HasIndex(e => e.InviteTotal)
                    .HasName("FK_InviteTotal");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.InviteDate).HasColumnType("date");

                entity.Property(e => e.InviteToday).HasColumnType("int(11)");

                entity.Property(e => e.InviteTotal).HasColumnType("int(11)");

                entity.Property(e => e.Phase).HasColumnType("int(11)");

                entity.Property(e => e.UserId).HasColumnType("bigint(20)");
            });

            modelBuilder.Entity<MemberDailyTask>(entity =>
            {
                entity.ToTable("yoyo_member_daily_task");

                entity.HasIndex(e => new { e.UserId, e.TaskId })
                    .HasName("UK_UserTask")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Carry).HasColumnType("int(11)");

                entity.Property(e => e.CompleteDate).HasColumnType("date");

                entity.Property(e => e.Completed).HasColumnType("int(11)");

                entity.Property(e => e.TaskId).HasColumnType("int(11)");

                entity.Property(e => e.UserId).HasColumnType("bigint(20)");
            });

            modelBuilder.Entity<SystemTask>(entity =>
            {
                entity.ToTable("yoyo_system_task");

                entity.Property(e => e.Id).HasColumnType("int(11)");

                entity.Property(e => e.Aims).HasColumnType("int(11)");

                entity.Property(e => e.Reward).HasColumnType("decimal(18,2)");

                entity.Property(e => e.Devote).HasColumnType("decimal(18,2)");

                entity.Property(e => e.Status).HasColumnType("int(11)");

                entity.Property(e => e.TaskDesc)
                    .IsRequired()
                    .HasColumnType("varchar(1024)");

                entity.Property(e => e.TaskTitle)
                    .IsRequired()
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Unit)
                    .IsRequired()
                    .HasColumnType("varchar(8)");

                entity.Property(e => e.TaskType).HasColumnType("int(11)");

                entity.Property(e => e.Sort).HasColumnType("int(11)");
            });

            modelBuilder.Entity<MemberActive>(entity =>
            {
                entity.ToTable("yoyo_member_active");

                entity.HasIndex(e => e.ActiveTime)
                    .HasName("NORMAL_ACTIVE");

                entity.HasIndex(e => e.UserId)
                    .HasName("UNIQUE_USERID")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.ActiveTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.JPushId)
                    .IsRequired()
                    .HasColumnName("JPushId")
                    .HasColumnType("varchar(64)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Remark)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.UserId).HasColumnType("bigint(20)");
            });

            modelBuilder.Entity<Yoyo_Luckydraw_User>()
                         .HasOne(x => x.Yoyo_Luckydraw_Round)
                        .WithMany()
                        .HasForeignKey(x => x.RoundId);

            modelBuilder.Entity<Yoyo_Luckydraw_User>()
                        .HasOne(x => x.User)
                       .WithMany()
                       .HasForeignKey(x => x.UserId);
            modelBuilder.Entity<Yoyo_Luckydraw_Round>()
                        .HasOne(x => x.Yoyo_Luckydraw_Prize)
                       .WithMany()
                       .HasForeignKey(x => x.PrizeId);

        }
    }
}
