namespace SparkTech.SDK.Game
{
    using System;

    using SharpDX;

    using SparkTech.SDK.Logging;
    using SparkTech.SDK.Platform.API;

    public static class GameEvents
    {
        public static event Action<WndProcEventArgs> OnWndProc;

        public static Point GetCursorPosition() => api.GetCursorPosition();

        private static IGameEvents api;

        internal static void Initialize(IGameEvents o)
        {
            api = o;

            o.OnWndProc = args => OnWndProc.SafeInvoke(args);
        }
    }
}