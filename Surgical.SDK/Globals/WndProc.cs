namespace Surgical.SDK
{
    using System;

    using SharpDX;

    using Surgical.SDK.API;
    using Surgical.SDK.EventData;

    public static class WndProc
    {
        private static IWndProc w;

        internal static void Initialize(IWndProc wndProc)
        {
            w = wndProc;

            wndProc.WndProc = args => OnWndProc.SafeInvoke(args);
        }

        public static event Action<WndProcEventArgs> OnWndProc;

        public static Vector2 CursorPosition => w.CursorPosition;
    }
}