namespace SparkTech.SDK.API.Fragments
{
    using System;

    using SharpDX;

    using SparkTech.SDK.EventArgs;

    public interface IUserInteraction
    {
        Action<WndProcEventArgs> OnWndProc { get; set; }

        Point CursorPosition();
    }
}