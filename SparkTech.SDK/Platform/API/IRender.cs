namespace SparkTech.SDK.Platform.API
{
    using System;

    using SharpDX;
    using SharpDX.Direct3D9;

    public interface IRender
    {
        int Width();

        int Height();

        Device GetDevice();

        Action Render { get; set; }

        Action BeginScene { get; set; }

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