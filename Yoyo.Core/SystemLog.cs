using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Filter;
using log4net.Repository;

namespace Yoyo.Core
{
    /// <summary>
    /// 系统日志
    /// </summary>
    public static class SystemLog
    {
        #region 异常日志
        /// <summary>
        /// 调试日志
        /// </summary>
        /// <param name="msg">信息</param>
        public static void Debug(object msg)
        {
            FileLogs.Debug(msg);
        }
        /// <summary>
        /// 调试日志
        /// </summary>
        /// <param name="msg">信息</param>
        /// <param name="ex">异常信息</param>
        public static void Debug(object msg, Exception ex)
        {
            FileLogs.Debug(msg, ex);
        }
        #endregion

        #region 错误日志
        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="msg">信息</param>
        /// <remarks>
        /// 可捕捉的及预料范围内的需要记录的日志信息，在error文件夹内
        /// </remarks>
        public static void Error(object msg)
        {
            FileLogs.Error(msg);
        }
        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="msg">信息</param>
        /// <param name="ex">异常信息</param>
        /// <remarks>
        /// 可捕捉的及预料范围内的需要记录的日志信息，在error文件夹内
        /// </remarks>
        public static void Error(object msg, Exception ex)
        {
            FileLogs.Error(msg, ex);
        }
        #endregion

        #region 数据日志
        /// <summary>
        /// 数据日志
        /// </summary>
        /// <param name="msg">信息</param>
        /// <remarks>
        /// 数据的通讯日志，信息记录日志等，在info文件夹内
        /// </remarks>
        public static void Info(object msg)
        {
            FileLogs.Info(msg);
        }
        /// <summary>
        /// 数据日志
        /// </summary>
        /// <param name="msg">信息</param>
        /// <param name="ex">异常信息</param>
        /// <remarks>
        /// 数据的通讯日志，信息记录日志等，在info文件夹内
        /// </remarks>
        public static void Info(object msg, Exception ex)
        {
            FileLogs.Info(msg, ex);
        }
        #endregion

        #region 警告日志
        /// <summary>
        /// 警告日志
        /// </summary>
        /// <param name="msg">信息</param>
        public static void Warn(object msg)
        {
            FileLogs.Warn(msg);
        }
        /// <summary>
        /// 警告日志
        /// </summary>
        /// <param name="msg">信息</param>
        /// <param name="ex">异常信息</param>
        public static void Warn(object msg, Exception ex)
        {
            FileLogs.Warn(msg, ex);
        }
        #endregion

        #region 定时任务日志
        /// <summary>
        /// 定时任务日志
        /// </summary>
        /// <param name="msg">信息</param>
        /// <remarks>
        /// 用于定时任务执行时的信息记录，存入jobs文件夹内。
        /// </remarks>
        public static void Jobs(object msg)
        {
            FileLogs.Fatal(msg);
        }
        /// <summary>
        /// 定时任务日志
        /// </summary>
        /// <param name="msg">信息</param>
        /// <param name="ex">异常信息</param>
        /// <remarks>
        /// 用于定时任务执行时的信息记录，存入jobs文件夹内。
        /// </remarks>
        public static void Jobs(object msg, Exception ex)
        {
            FileLogs.Fatal(msg, ex);
        }
        #endregion

        #region 日志容器配置
        const String fileLogFormat = "%n记录时间：%date %n日志级别：%-5level %n类及方法：%logger  %n描述：%message %newline";
        const String fileLogBlock = "%newline==================================== Log ====================================%newline";
        static readonly ILoggerRepository fileLogger = LogManager.CreateRepository("Yoyo_FileLogs");
        static Boolean fileLoggerActive;
        #endregion

        #region 文件日志容器
        static ILog FileLogs
        {
            get
            {
                if (!fileLoggerActive) { FileLoggerConfigure(); }
                return LogManager.GetLogger(fileLogger.Name, GetMethodFullName());
            }
        }
        #endregion

        #region 文件日志基本配置
        static void FileLoggerConfigure()
        {
            Dictionary<String, Level> levels = new Dictionary<string, Level>
            {
                { "debug", Level.Debug },   //调试日志
                { "warn", Level.Warn },     //警告日志
                { "info", Level.Info },     //通讯日志
                { "error", Level.Error },   //错误日志
                { "jobs", Level.Fatal }    //定时任务执行日志
            };
            foreach (var item in levels)
            {
                BasicConfigurator.Configure(fileLogger, GetFileAppender(item.Value, item.Key));
            }
            fileLoggerActive = true;
        }
        #endregion

        #region 设置获取日志级别类型格式
        static RollingFileAppender GetFileAppender(Level level, String pathName)
        {
            var layout = new log4net.Layout.PatternLayout(fileLogBlock + fileLogFormat);
            var fileappend = new RollingFileAppender
            {
                LockingModel = new FileAppender.MinimalLock(),
                AppendToFile = true,
                StaticLogFileName = false,
                ImmediateFlush = true,
                RollingStyle = RollingFileAppender.RollingMode.Date,
                File = Path.Combine("logs", $"{pathName}{Path.DirectorySeparatorChar}"),
                DatePattern = $"yyyy-MM-dd'.log'",
                Name = $"{level.Name}RollingFileAppender",
                Layout = layout
            };
            LevelRangeFilter LevelRange = new LevelRangeFilter { LevelMax = level, LevelMin = level };
            LevelRange.ActivateOptions();
            fileappend.AddFilter(LevelRange);
            fileappend.ActivateOptions();
            return fileappend;
        }
        #endregion

        #region 获取方法全名
        static string GetMethodFullName()
        {
            try
            {
                int depth = 2;
                StackTrace st = new StackTrace();
                int maxFrames = st.GetFrames().Length;
                StackFrame sf;
                string methodName, className;
                Type classType;
                do
                {
                    sf = st.GetFrame(depth++);
                    classType = sf.GetMethod().DeclaringType;
                    className = classType.ToString();
                } while (className.EndsWith("Exception", StringComparison.CurrentCulture) && depth < maxFrames);
                methodName = sf.GetMethod().Name;
                return className + "." + methodName;
            }
            catch (Exception ex)
            {
                if (!fileLoggerActive) { FileLoggerConfigure(); }
                LogManager.GetLogger(fileLogger.Name, $"Yoyo.Core.{nameof(GetMethodFullName)}").Fatal(ex);
                return string.Empty;
            }
        }
        #endregion
    }
}
