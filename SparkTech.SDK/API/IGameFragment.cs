namespace SparkTech.SDK.API
{
    using System;

    using SharpDX;

    using SparkTech.SDK.Entities;

    public interface IGameFragment
    {
        void SendChat(string text);

        void ShowChat(string text);

        void SendEmote(Emote emote);

        void SendPing(PingCategory category, IGameObject target);

        void SendPing(PingCategory category, Vector2 target);

        bool IsShopOpen();

        bool IsChatOpen();

        bool IsScoreboardOpen();

        Vector2 WorldToScreen(Vector3 pos);

        Vector2 WorldToMinimap(Vector3 pos);

        Vector3 ScreenToWorld(Vector2 pos);

        Vector3 Cursor { get; }

        Matrix ProjectionMatrix { get; }

        Matrix ViewMatrix { get; }

        GameState State { get; }

        GameMap Map { get; }

        float Time { get; }

        float Ping { get; }

        Action<EventArgs> Update { get; set; }
    }
}