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

        public static Point CursorPosition => Fragment.CursorPosition;

        static UserInput()
        {
            Fragment = Platform.UserInputFragment ?? throw Platform.APIException("UserInputAPI");

            Fragment.WndProcess = args => OnWndProc.SafeInvoke(args);
        }
    }
}