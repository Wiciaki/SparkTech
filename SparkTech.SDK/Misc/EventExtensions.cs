namespace SparkTech.SDK.Misc
{
    using System;
    using System.Linq;

    using SparkTech.SDK.Logging;

    public static class EventExtensions
    {
        public static void SafeInvoke<T>(this Action<T> e, T arg)
        {
            foreach (var callback in e.GetInvocationList().Cast<Action<T>>())
            {
                try
                {
                    callback(arg);
                }
                catch (Exception ex)
                {
                    ex.Log();
                }
            }
        }
    }
}