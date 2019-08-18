namespace SparkTech.SDK
{
    using System;
    using System.Linq;

    using SparkTech.SDK.Logging;

    public static class EventExtensions
    {
        public static void SafeInvoke(this Action e)
        {
            if (e == null)
            {
                return;
            }

            foreach (var callback in e.GetInvocationList().Cast<Action>())
            {
                try
                {
                    callback();
                }
                catch (Exception ex)
                {
                    ex.Log();
                }
            }
        }

        public static void SafeInvoke<T>(this Action<T> e, T arg)
        {
            if (e == null)
            {
                return;
            }

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