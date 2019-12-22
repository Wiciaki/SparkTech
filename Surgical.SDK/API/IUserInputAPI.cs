namespace Surgical.SDK.API
{
    using System;

    using SharpDX;

    using Surgical.SDK.EventData;

    public interface IUserInputAPI
    {
        Action<WndProcEventArgs> WndProc { get; set; }

        Vector2 CursorPosition { get; }
    }
}