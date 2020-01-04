namespace Surgical.SDK.API
{
    using System;

    using SharpDX;

    using Surgical.SDK.EventData;

    public interface IUserInputAPI
    {
        Action<WndProcEventArgs>? WndProcess { get; set; }

        Point CursorPosition { get; }
    }
}