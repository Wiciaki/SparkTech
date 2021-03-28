namespace SparkTech.SDK.Logging
{
    using System;
    using System.IO;

    public sealed class FileLogger : ILogger
    {
        private readonly string path;

        public FileLogger()
        {
            this.path = Folder.Logs.GetFile($"log {DateTime.Now:dd-M-HH.mm}.txt");
        }

        public void Write(string msg, LogLevel level)
        {
            char lvl;
            
            switch (level)
            {
                case LogLevel.Debug:
                    lvl = 'I';
                    break;
                case LogLevel.Warn:
                    lvl = 'W';
                    break;
                default:
                    lvl = 'E';
                    break;
            };

            File.AppendAllText(this.path, $"{DateTime.Now:hh:mm:ss:fff} {lvl} {msg}\n");
        }
    }
}