namespace SparkTech.SDK.Logging
{
    using System.IO;

    using SparkTech.SDK.Security;

    public class FileLogger : ILogger
    {
        private readonly string path;

        public FileLogger()
        {
            path = Folder.RootFolder.GetFile("log.txt");
        }

        public void Write(string msg, LogLevel level)
        {
            File.AppendAllText(path, level + " | " +msg + "\n");
        }
    }
}