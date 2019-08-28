namespace SparkTech.SDK.Game
{
    using System;

    using SharpDX;

    using SparkTech.SDK.Logging;
    using SparkTech.SDK.Platform.API;

    public static class GameEvents
    {
        public static event Action<WndProcEventArgs> OnWndProc;
    }
}