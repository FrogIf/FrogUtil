namespace Frog.Util.Log
{
    public interface ILogger
    {
        void debug(string message, params object[] args);

        void info(string message, params object[] args);

        void warn(string message, params object[] args);

        void error(string message, params object[] args);

    }
}
