using System;
using System.Text;

namespace Frog.Util.Log
{
    public abstract class AbstractLogger : ILogger
    {
        public const string LEVEL_DEBUG = "debug";

        public const string LEVEL_INFO = "info";

        public const string LEVEL_WARN = "warn";

        public const string LEVEL_ERROR = "error";

        /// <summary>
        /// 日志级别, 0 - debug, 1 - info, 2 - warn, 3 - error, 默认为1(info)
        /// </summary>
        private int loggerLevel = 1;

        protected readonly string loggerName;

        public AbstractLogger(string loggerName)
        {
            this.loggerName = loggerName;
        }

        public AbstractLogger(string loggerName, string level)
        {
            if (level == null)
            {
                throw new ArgumentException("logger level can't be null.");
            }

            level = level.ToLower();

            switch (level)
            {
                case LEVEL_DEBUG:
                    loggerLevel = 0;
                    break;
                case LEVEL_INFO:
                    loggerLevel = 1;
                    break;
                case LEVEL_WARN:
                    loggerLevel = 2;
                    break;
                case LEVEL_ERROR:
                    loggerLevel = 3;
                    break;
                default:
                    throw new ArgumentException("unrecognizable level : " + level);
            }
            this.loggerName = loggerName;

        }


        public void debug(string message, params object[] args)
        {
            if (this.loggerLevel < 1)
            {
                formatAndOutput(message, LEVEL_DEBUG, args);
            }
        }

        public void info(string message, params object[] args)
        {
            if (this.loggerLevel < 2)
            {
                formatAndOutput(message, LEVEL_INFO, args);
            }
        }

        public void warn(string message, params object[] args)
        {
            if (this.loggerLevel < 3)
            {
                formatAndOutput(message, LEVEL_WARN, args);
            }
        }

        public void error(string message, params object[] args)
        {
            formatAndOutput(message, LEVEL_ERROR, args);
        }

        private void formatAndOutput(string message, string triggerLevel, params object[] args)
        {
            StringBuilder sb = new StringBuilder();
            char[] chars = message.ToCharArray();
            int argIndex = 0;
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == '{' && i < chars.Length - 1 && chars[i + 1] == '}')
                {
                    sb.Append(args[argIndex]);
                    argIndex++;
                    i++;
                }
                else
                {
                    sb.Append(chars[i]);
                }
            }
            this.outputLogToTarget(sb.ToString(), triggerLevel);
        }

        protected abstract void outputLogToTarget(string message, string triggerLevel);
    }
}
