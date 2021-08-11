using System.Collections.Generic;

namespace Frog.Util.Log
{
    public class LoggerFactory
    {

        private static string level = "info";

        private static LoggerWrapper rootLogger = new LoggerWrapper(new ConsoleLogger("root", level));

        private static Dictionary<string, ILogger> loggerMap = new Dictionary<string, ILogger>();

        public static ILogger GetLogger(string loggerName)
        {
            ILogger logger = null;
            loggerMap.TryGetValue(loggerName, out logger);
            if (logger == null)
            {
                return rootLogger;
            }
            else
            {
                return logger;
            }
        }

        public static void register(string loggerName, ILogger logger)
        {
            register(loggerName, logger, true);
        }

        /// <summary>
        /// 注册logger到LoggerFactory
        /// </summary>
        /// <param name="loggerName">日志名, 如果已经存在, 则覆盖</param>
        /// <param name="logger">日志对象</param>
        /// <param name="additivity">是否追加, true - 则同时输出到rootLogger, false - 不输出到rootLogger</param>
        public static void register(string loggerName, ILogger logger, bool additivity)
        {
            if (additivity)
            {
                ChainLogger chainLogger = new ChainLogger();
                chainLogger.addLogger(logger);
                chainLogger.addLogger(rootLogger);
                loggerMap.Add(loggerName, chainLogger);
            }
            else
            {
                loggerMap.Add(loggerName, logger);
            }
        }

        public static void setRootLogger(ILogger logger)
        {
            rootLogger.changeLogger(logger);
        }

        public static ILogger GetRootLogger()
        {
            return rootLogger;
        }


        private class LoggerWrapper : ILogger
        {

            private volatile ILogger logger;

            public LoggerWrapper(ILogger logger)
            {
                this.logger = logger;
            }

            public void changeLogger(ILogger logger)
            {
                this.logger = logger;
            }

            public void debug(string message, params object[] args)
            {
                logger.debug(message, args);
            }

            public void error(string message, params object[] args)
            {
                logger.error(message, args);
            }

            public void info(string message, params object[] args)
            {
                logger.info(message, args);
            }

            public void warn(string message, params object[] args)
            {
                logger.warn(message, args);
            }
        }

    }
}
