using System;

namespace Frog.Util.Log
{
    public class ConsoleLogger : AbstractLogger
    {
        public ConsoleLogger(string loggerName) : base(loggerName)
        {
        }

        public ConsoleLogger(string loggerName, string level) : base(loggerName, level)
        {
        }

        protected override void outputLogToTarget(string message, string triggerLevel)
        {
            Console.WriteLine(message);
        }
    }
}
