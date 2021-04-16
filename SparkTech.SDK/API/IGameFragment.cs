namespace SparkTech.SDK.API
{
    using System;

    using SharpDX;

    using SparkTech.SDK.Entities;
    using SparkTech.SDK.League;

    public interface IGameFragment
    {
        INavMesh GetNavMesh();

        void Say(string text);

        void Print(string text);

        void SendEmote(Emote emote);

        void SendSummonerEmote(SummonerEmoteSlot slot);

        void SendMasteryBadge();

        void SendPing(PingCategory category, IGameObject target);

        void SendPing(PingCategory category, Vector2 target);

        bool IsShopOpen();

        bool IsChatOpen();

        bool IsScoreboardOpen();

        Vector2 WorldToScreen(Vector3 pos);

        Vector2 WorldToMinimap(Vector3 pos);

        Vector3 ScreenToWorld(Vector2 pos);

        Vector3 CursorPos { get; }

        Matrix ProjectionMatrix { get; }

        Matrix ViewMatrix { get; }

        GameState State { get; }

        GameMap Map { get; }

        float Time { get; }

        int Ping { get; }

        int FPS { get; }

        string Version { get; }

        Action<EventArgs> Update { get; set; }
    }
}