namespace SparkTech.SDK.Platform.API
{
    using System;

    using SharpDX;

    public interface IGameEvents
    {
        Action<WndProcEventArgs> OnWndProc { get; set; }

        Point GetCursorPosition();
    }
}