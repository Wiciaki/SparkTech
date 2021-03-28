namespace SparkTech.SDK.Logging
{
    public interface ILogger
    {
        void Write(string msg, LogLevel level);
    }
}