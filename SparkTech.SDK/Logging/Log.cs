namespace SparkTech.SDK.Logging
{
    using SparkTech.SDK.Platform;

    public static class Log
    {
        public static void Error(object obj)
        {
            if (obj != null)
            {
                Error(obj.ToString());
            }
        }

        public static void Warn(object obj)
        {
            if (obj != null)
            {
                Warn(obj.ToString());
            }
        }

        public static void Info(object obj)
        {
            if (obj != null)
            {
                Info(obj.ToString());
            }
        }

        public static void Error(string msg)
        {
            VendorSetup.Logger.Write(msg, LogLevel.Error);
        }

        public static void Warn(string msg)
        {
            VendorSetup.Logger.Write(msg, LogLevel.Warn);
        }

        public static void Info(string msg)
        {
            VendorSetup.Logger.Write(msg, LogLevel.Info);
        }
    }
}