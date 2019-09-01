namespace SparkTech.SDK.Logging
{
    using System;

    using SparkTech.SDK.API;

    public static class Log
    {
        private static ILogger l;

        internal static void Initialize(ILogger logger)
        {
            l = logger;

            AppDomain.CurrentDomain.UnhandledException += (sender, args) => { Error(args.ExceptionObject); };

            Info("Started logger");
            Info("Platform name: " + Platform.PlatformName);
        }

        public static void Error(object obj)
        {
            Error(obj?.ToString());
        }

        public static void Warn(object obj)
        {
            Warn(obj?.ToString());
        }

        public static void Info(object obj)
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
            if (msg != null)
            {
                l.Write(msg, level);
            }
        }
    }
}