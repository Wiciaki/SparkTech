namespace SparkTech.SDK.API.Fragments
{
    using System;

    using SharpDX;

    public interface IGame
    {
        Vector2 CursorPosition { get; }

        void ChatShow(string text);

        void ChatPrint(string text);

        bool IsShopOpen();

        bool IsChatOpen();

        Vector2 WorldToScreen(Vector3 pos);

        Vector2 WorldToMinimap(Vector3 pos);

        Vector3 ScreenToWorld(Vector3 pos);

        float Time { get; }

        float Ping { get; }

        Action<WndProcEventArgs> WndProc { get; set; }
    }
}