namespace Surgical.SDK
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Surgical.SDK.Logging;

    internal static class InternalUtils
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