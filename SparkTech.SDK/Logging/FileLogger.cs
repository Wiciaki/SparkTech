namespace SparkTech.SDK.Logging
{
    using System.IO;

    using SparkTech.SDK.Security;

    public class FileLogger : ILogger
    {
        private readonly string path;

        public FileLogger()
        {
            this.path = Folder.Root.GetFile("log.txt");
        }

        public void Write(string msg, LogLevel level)
        {
            File.AppendAllText(this.path, level + " | " + msg + "\n");
        }
    }
}