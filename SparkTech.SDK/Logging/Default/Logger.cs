namespace SparkTech.SDK.Logging.Default
{
    using System;

    public sealed class DefaultLogger : ILogger
    {
        public void Write(string msg, LogLevel level)
        {
            // TODO this is some temp garbage

            Console.ForegroundColor = level switch
            {
                LogLevel.Error => ConsoleColor.DarkYellow,
                LogLevel.Warn => ConsoleColor.Yellow,
                _ => ConsoleColor.White
            };

            Console.WriteLine();
            Console.WriteLine(msg);
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}