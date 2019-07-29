namespace SparkTech.SDK.Logging
{
    using System;

    public static class Extensions
    {
        public static void Log(this Exception ex)
        {
            Logging.Log.Error(ex.ToString());
        }
    }
}