namespace SparkTech.SDK
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using SparkTech.SDK.Logging;

    internal static class InternalEx
    {
        internal static void SafeInvoke<T>(this Action<T> evt, T args)
        {
            if (evt == null || !Platform.IsLoaded)
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

        internal static void SafeInvoke(this Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        internal static void Trigger(this Type type)
        {
            RuntimeHelpers.RunClassConstructor(type.TypeHandle);
        }
    }
}