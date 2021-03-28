namespace SparkTech.SDK
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using SparkTech.SDK.Logging;

    internal static class InternalEx
    {
        public static void SafeInvoke<T>(this Action<T> evt, T args)
        {
            if (evt == null)
            {
                return;
            }

            foreach (var callback in evt.GetInvocationList().Cast<Action<T>>())
            {
                try
                {
                    callback(args);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
        }

        public static void Trigger(this Type type)
        {
            RuntimeHelpers.RunClassConstructor(type.TypeHandle);
        }
    }
}