namespace SparkTech.SDK.Platform.API
{
    using System;

    using SharpDX;

    public interface IUserInteraction
    {
        Action<WndProcEventArgs> OnWndProc { get; set; }

        Point CursorPosition();
    }
}