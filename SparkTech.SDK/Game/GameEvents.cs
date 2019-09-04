namespace SparkTech.SDK.Game
{
    using System;

    using SharpDX;

    using SparkTech.SDK.Logging;

    public static class GameEvents
    {
        public static event Action<WndProcEventArgs> OnWndProc;
    }
}