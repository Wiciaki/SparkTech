namespace SparkTech.SDK.API.Fragments
{
    using System;

    using SharpDX;

    public interface IUserInteraction
    {
        Action<WndProcEventArgs> OnWndProc { get; set; }

        Point CursorPosition();
    }
}