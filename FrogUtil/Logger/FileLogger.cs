using Frog.Util.File;
using System;
using System.IO;
using System.Text;

namespace Frog.Util.Log
{
    public class FileLogger : AbstractLogger
    {
        private readonly StreamWriter writer;

        public FileLogger(string loggerName, string level, string outputPath, string logFileName) : base(loggerName, level)
        {
            if (System.IO.File.Exists(outputPath + "/" + logFileName))
            {
                FileUtil.renameFile(outputPath, logFileName, logFileName + "." + DateTime.Now.Ticks.ToString());
            }
            else if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            writer = new StreamWriter(outputPath + "/" + logFileName);
            writer.AutoFlush = true;
        }

        protected override void outputLogToTarget(string message, string triggerLevel)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            sb.Append(loggerName);
            sb.Append('-');
            sb.Append(triggerLevel);
            sb.Append("]:");
            sb.Append(message);
            writer.WriteLine(sb.ToString());
        }

        public void Dispose()
        {
            this.writer.Close();
        }
    }
}
