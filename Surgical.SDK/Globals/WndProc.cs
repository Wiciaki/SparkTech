namespace Surgical.SDK
{
    using System;

    using SharpDX;

    using Surgical.SDK.API;
    using Surgical.SDK.EventData;

    public static class UserInput
    {
        private static readonly IUserInputAPI Fragment;

        public static event Action<WndProcEventArgs>? OnWndProc;

        public static Vector2 CursorPosition => Fragment.CursorPosition;

        static UserInput()
        {
            Fragment = Platform.UserInputFragment ?? throw new InvalidOperationException("Attempted to use UserInputAPI when it wasn't present!");

            Fragment.WndProcess = args => OnWndProc.SafeInvoke(args);
        }
    }
}