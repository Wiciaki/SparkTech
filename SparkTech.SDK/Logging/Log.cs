namespace SparkTech.SDK.Logging
{
    public static class Log
    {
        internal static ILogger Logger = new FileLogger();

        internal static LogLevel LogLevel;

        public static void Error(object obj)
        {
            Error(obj?.ToString());
        }

        public static void Warn(object obj)
        {
            Warn(obj?.ToString());
        }

        public static void Debug(object obj)
        {
            Info(obj?.ToString());
        }

        public static void Error(string msg)
        {
            Write(msg, LogLevel.Error);
        }

        public static void Warn(string msg)
        {
            Write(msg, LogLevel.Warn);
        }

        public static void Info(string msg)
        {
            Write(msg, LogLevel.Debug);
        }

        private static void Write(string msg, LogLevel level)
        {
            if (msg != null && level >= LogLevel)
            {
                Logger.Write(msg, level);
            }
        }
    }
}