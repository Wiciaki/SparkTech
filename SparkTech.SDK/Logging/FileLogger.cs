namespace SparkTech.SDK.Logging
{
    using System;
    using System.IO;

    using SparkTech.SDK.Security;

    public sealed class FileLogger : ILogger
    {
        private readonly string path;

        public FileLogger()
        {
            this.path = Folder.Logs.GetFile($"log {DateTime.Now:dd-M-HH.mm}.txt");
        }

        public void Write(string msg, LogLevel level)
        {
            var lvl = level switch
            {
                LogLevel.Debug => "I",
                LogLevel.Warn => "W",
                _ => "E"
            };

            File.AppendAllText(this.path, $"{DateTime.Now:hh:mm:ss:fff} {lvl} {msg}\n");
        }
    }
}