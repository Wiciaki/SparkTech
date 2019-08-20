namespace SparkTech.SDK.Logging
{
    using System;

    public sealed class ConsoleLogger : ILogger
    {
        public void Write(string msg, LogLevel level)
        {
            Console.Write($"[{DateTime.Now:hh:mm:ss}] ");

            Console.ForegroundColor = level switch
            {
                LogLevel.Error => ConsoleColor.DarkYellow,
                LogLevel.Warn => ConsoleColor.Yellow,
                _ => ConsoleColor.White
            };

            Console.WriteLine(msg);

            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}