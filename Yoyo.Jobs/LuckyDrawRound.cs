// using Microsoft.Extensions.DependencyInjection;
// using Quartz;
// using System;
// using System.Collections.Generic;
// using System.Text;
// using System.Threading.Tasks;
// using Dapper;
// using System.Diagnostics;
// using System.Data;
// using System.Linq;
// using Microsoft.Extensions.Options;
// using Yoyo.IServices.Response;
// using Yoyo.Entity;
// using Yoyo.Entity.Models.luckdraw;
// using Microsoft.EntityFrameworkCore;
// using Yoyo.Entity.Models;
// using Org.BouncyCastle.Crypto.Tls;

// namespace Yoyo.Jobs
// {
//     public class LuckyDrawRound : IJob
//     {
//         private static bool runService = false;
//         private readonly IServiceProvider ServiceProvider;

//         public LuckyDrawRound(IServiceProvider service)
//         {
//             this.ServiceProvider = service;
//         }
//         public void Execute(IJobExecutionContext context)
//         {
//             if (!runService)
//             {

//                 using (var service = this.ServiceProvider.CreateScope())
//                 {
//                     Entity.SqlContext _context = service.ServiceProvider.GetRequiredService<Entity.SqlContext>();
//                     try
//                     {

//                         runService = true;
//                         Stopwatch stopwatch = new Stopwatch();
//                         stopwatch.Start();

//                         var rounds = _context.Yoyo_Luckydraw_Round.Where(x => x.Status == RoundStatus.Waiting && x.OpenTime <= DateTime.Now).ToList();
//                         var maxLevelRound = _context.Yoyo_Luckydraw_Round.AsNoTracking().OrderByDescending(x => x.Level).FirstOrDefault();
//                         var defaultUsers = _context.Yoyo_Luckydraw_DefaultUser.AsNoTracking().ToList();
//                         int level = 0;
//                         if (maxLevelRound != null)
//                         {
//                             level = maxLevelRound.Level;
//                         }
//                         var levelIndex = 0;
//                         rounds.ForEach(x =>
//                         {
//                             x.Status = RoundStatus.Ending;
//                             x.UpdatedTime = DateTime.Now;
//                             if (x.OpenTime <= DateTime.Now)
//                             {
//                                 //可以开奖了
//                                 if (x.WinnerType == WinnerType.Default)
//                                 {
//                                     //默认取default表里的用户Id
//                                     var ran = new Random();
//                                     var index = (ran.Next(0, defaultUsers.Count - 1));
//                                     var winUserExist = _context.Yoyo_Luckydraw_User.FirstOrDefault(u => u.UserId == defaultUsers[index].UserId);
//                                     if (winUserExist == null)
//                                     {
//                                         _context.Yoyo_Luckydraw_User.Add(new Yoyo_Luckydraw_User
//                                         {
//                                             CandyCount = ran.Next(1, x.MaxNumber),
//                                             IsWin = true,
//                                             RoundId = x.Id,
//                                             UserId = defaultUsers[index].UserId,
//                                             CreatedTime = DateTime.Now
//                                         });
//                                     }
//                                     else
//                                     {
//                                         winUserExist.IsWin = true;
//                                     }
//                                 }
//                                 else
//                                 {
//                                     //随机取糖果参与的用户
//                                     var users = _context.Yoyo_Luckydraw_User.Where(u => u.RoundId == x.Id).ToList();
//                                     Dictionary<int, long> dic = new Dictionary<int, long>();
//                                     int candyIndex = 1;
//                                     users.ForEach(uu =>
//                                     {
//                                         for (int i = 0; i < uu.CandyCount; i++)
//                                         {
//                                             dic.Add(candyIndex, uu.UserId);
//                                             candyIndex += 1;
//                                         }
//                                     });
//                                     var ran = new Random();
//                                     var winIndex = (ran.Next(1, candyIndex - 1));
//                                     var winUser = users.FirstOrDefault(uu => uu.UserId == dic[winIndex]);
//                                     winUser.IsWin = true;
//                                 }

//                                 if (x.AutoNext)
//                                 {
//                                     //自动开启下一轮
//                                     var nextRound = new Yoyo_Luckydraw_Round
//                                     {
//                                         CreatedTime = DateTime.Now,
//                                         Level = level + levelIndex + 1,
//                                         NeedRoundNumber = x.NeedRoundNumber,
//                                         AutoNext = x.AutoNext,
//                                         DelayHour = x.DelayHour,
//                                         WinnerType = x.WinnerType,
//                                         Status = RoundStatus.Rounding,
//                                         MaxNumber = x.MaxNumber,
//                                         UpdatedTime = DateTime.Now,
//                                         CurrentRoundNumber = 0,
//                                         PrizeId = x.PrizeId
//                                     };
//                                     _context.Yoyo_Luckydraw_Round.Add(nextRound);
//                                 }
//                                 levelIndex = levelIndex + 1;
//                             }
//                             _context.SaveChanges();
//                         });

//                         runService = false;
//                         stopwatch.Stop();
//                         Core.SystemLog.Jobs($"定时更新夺宝状态 执行完成,执行时间:{stopwatch.Elapsed.TotalSeconds}秒");

//                     }
//                     catch (Exception ex)
//                     {
//                         runService = false;
//                         Core.SystemLog.Jobs("定时更新夺宝状态错误", ex);
//                     }
//                 }

//             }

//         }

//         Task IJob.Execute(IJobExecutionContext context)
//         {
//             throw new NotImplementedException();
//         }
//     }
// }

