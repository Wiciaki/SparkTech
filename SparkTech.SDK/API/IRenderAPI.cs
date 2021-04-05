namespace SparkTech.SDK.API
{
    using System;

    using SharpDX.Direct3D9;

    public interface IRenderAPI
    {
        int Width { get; }

        int Height { get; }

        Device Device { get; }

        Action BeginScene { get; set; }

        Action Draw { get; set; }

        Action EndScene { get; set; }

        Action LostDevice { get; set; }
        
        Action ResetDevice { get; set; }
    }
}