namespace SparkTech.SDK.Platform.API
{
    using System;

    using SharpDX;
    using SharpDX.Direct3D9;

    public interface IRender
    {
        Size2 Resolution();

        Device GetDevice();

        Action BeginScene { get; set; }

        Action Draw { get; set; }

        Action EndScene { get; set; }

        Action LostDevice { get; set; }
        
        Action ResetDevice { get; set; }

        Vector2 WorldToScreen(Vector3 pos);

        Vector2 WorldToMinimap(Vector3 pos);

        Vector3 ScreenToWorld(Vector3 pos);

        Matrix ProjectionMatrix();

        Matrix ViewMatrix();
    }
}