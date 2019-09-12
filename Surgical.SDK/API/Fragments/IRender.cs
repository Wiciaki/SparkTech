namespace Surgical.SDK.API.Fragments
{
    using System;

    using SharpDX;
    using SharpDX.Direct3D9;

    public interface IRender
    {
        Size2 Resolution();

        Device Device { get; }

        Action BeginScene { get; set; }

        Action Draw { get; set; }

        Action EndScene { get; set; }

        Action LostDevice { get; set; }
        
        Action ResetDevice { get; set; }

        Action SetRenderTarget { get; set; }

        Matrix Projection { get; }

        Matrix View { get; }
    }
}