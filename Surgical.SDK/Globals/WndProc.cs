namespace Surgical.SDK
{
    using System;

    using SharpDX;

    using Surgical.SDK.API;
    using Surgical.SDK.EventData;

    public static class UserInput
    {
        private static IUserInputAPI r;

        internal static void Initialize(IUserInputAPI userInput)
        {
            r = userInput;

            userInput.WndProc = args => OnWndProc.SafeInvoke(args);
        }

        public static event Action<WndProcEventArgs> OnWndProc;

        public static Vector2 CursorPosition => r.CursorPosition;
    }
}