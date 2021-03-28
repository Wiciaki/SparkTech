namespace SparkTech.SDK.Logging
{
    using System;

    public sealed class ConsoleLogger : ILogger
    {
        public void Write(string msg, LogLevel level)
        {
            ConsoleColor color;

            switch (level)
            {
                case LogLevel.Error:
                    color = ConsoleColor.DarkYellow;
                    break;
                case LogLevel.Warn:
                    color = ConsoleColor.Yellow;
                    break;
                default:
                    color = ConsoleColor.White;
                    break;
            }

            Console.Write($"[{DateTime.Now:hh:mm:ss}] ");

            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}