namespace Surgical.SDK.Logging
{
    using System;

    public static class Log
    {
        private static readonly ILogger Logger;

        static Log()
        {
            Logger = Platform.PlatformLogger ?? new FileLogger();

            AppDomain.CurrentDomain.UnhandledException += (sender, args) => { Error(args.ExceptionObject); };

            Info("Started logger");
            Info("Platform name: " + Platform.Name);
        }

        public static void Error(object? obj)
        {
            Error(obj?.ToString());
        }

        public static void Warn(object? obj)
        {
            Warn(obj?.ToString());
        }

        public static void Info(object? obj)
        {
            Info(obj?.ToString());
        }

        public static void Error(string? msg)
        {
            Write(msg, LogLevel.Error);
        }

        public static void Warn(string? msg)
        {
            Write(msg, LogLevel.Warn);
        }

        public static void Info(string? msg)
        {
            Write(msg, LogLevel.Debug);
        }

        private static void Write(string? msg, LogLevel level)
        {
            if (msg != null)
            {
                Logger.Write(msg, level);
            }
        }
    }
}