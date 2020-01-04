namespace Surgical.SDK.API
{
    using System;

    using SharpDX;
    using SharpDX.Direct3D9;

    public interface IRenderAPI
    {
        Size2 Resolution { get; }

        Device Device { get; }

        Action BeginScene { get; set; }

        Action Draw { get; set; }

        Action EndScene { get; set; }

        Action LostDevice { get; set; }
        
        Action ResetDevice { get; set; }

        Action SetRenderTarget { get; set; }
    }
}