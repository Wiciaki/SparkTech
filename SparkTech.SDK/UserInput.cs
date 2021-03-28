﻿namespace SparkTech.SDK
{
    using System;

    using SharpDX;

    using SparkTech.SDK.API;
    using SparkTech.SDK.EventData;

    public static class UserInput
    {
        private static readonly IUserInputAPI Fragment;

        public static event Action<WndProcEventArgs> OnWndProc;

        public static Point Cursor2D { get; private set; }

        static UserInput()
        {
            Fragment = Platform.UserInputFragment ?? throw Platform.APIException("UserInputAPI");

            Fragment.WndProcess = args =>
            {
                if (args.Message == WindowsMessages.MOUSEMOVE)
                {
                    var x = (short)args.LParam;
                    var y = (short)(args.LParam >> 16);

                    Cursor2D = new Point(x, y);
                }

                OnWndProc.SafeInvoke(args);
            };
        }
    }
}