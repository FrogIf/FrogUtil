using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frog.Util.Log
{
    public class ChainLogger : ILogger
    {
        private LinkedList<ILogger> loggers = new LinkedList<ILogger>();

        public void addLogger(ILogger logger)
        {
            loggers.AddLast(logger);
        }

        public void debug(string message, params object[] args)
        {
            LinkedList<ILogger>.Enumerator enumerator = loggers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.debug(message, args);
            }
        }

        public void error(string message, params object[] args)
        {
            LinkedList<ILogger>.Enumerator enumerator = loggers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.error(message, args);
            }
        }

        public void info(string message, params object[] args)
        {
            LinkedList<ILogger>.Enumerator enumerator = loggers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.info(message, args);
            }
        }

        public void warn(string message, params object[] args)
        {
            LinkedList<ILogger>.Enumerator enumerator = loggers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.warn(message, args);
            }
        }
    }
}
