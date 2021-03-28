namespace SparkTech.SDK.API
{
    using System;

    using SharpDX;

    using SparkTech.SDK.Entities;
    using SparkTech.SDK.EventData;

    public interface IGameFragment
    {
        void SendChat(string text);

        void ShowChat(string text);

        void SendEmote(Emote emote);

        void ShowPing(PingCategory category, IGameObject target, bool playSound);

        void ShowPing(PingCategory category, Vector2 target, bool playSound);

        void SendPing(PingCategory category, IGameObject target);

        void SendPing(PingCategory category, Vector2 target);

        bool IsShopOpen();

        bool IsChatOpen();

        Vector2 WorldToScreen(Vector3 pos);

        Vector2 WorldToMinimap(Vector3 pos);

        Vector3 ScreenToWorld(Vector2 pos);

        Vector3 Cursor { get; }

        Matrix ProjectionMatrix { get; }

        Matrix ViewMatrix { get; }

        GameState State { get; }

        GameMap Map { get; }

        float Time { get; }

        float Latency { get; }

        Action<EventArgs> Update { get; set; }

        Action<_EndEventArgs> End { get; set; }

        Action<_AfkEventArgs> Afk { get; set; }

        Action<NotifyEventArgs> Notify { get; set; }

        Action<_PingEventArgs> Ping { get; set; }

        Action<_InputEventArgs> Input { get; set; }

        Action<_ChatEventArgs> Chat { get; set; }
    }
}