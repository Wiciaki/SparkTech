﻿namespace Surgical.SDK.API.Fragments
{
    using System;

    using SharpDX;

    using Surgical.SDK.Entities;
    using Surgical.SDK.EventData;

    public interface IGame
    {
        Vector3 CursorPosition { get; }

        Vector2 CursorPosition2D { get; }

        void SendChat(string text);

        void ShowChat(string text);

        void SendEmote(Emote emote);

        void ShowPing(PingCategory pingCategory, IGameObject target, bool playSound);

        void ShowPing(PingCategory pingCategory, Vector2 target, bool playSound);

        void SendPing(PingCategory pingCategory, IGameObject target);

        void SendPing(PingCategory pingCategory, Vector2 target);

        bool IsShopOpen();

        bool IsChatOpen();

        Vector2 WorldToScreen(Vector3 pos);

        Vector2 WorldToMinimap(Vector3 pos);

        Vector3 ScreenToWorld(Vector3 pos);

        GameMode Mode { get; }

        GameType Type { get; }

        GameMap Map { get; }

        float Time { get; }

        float ClockTime { get; }

        float Latency { get; }

        string Region { get; }

        long Id { get; }

        Action<WndProcEventArgs> WndProc { get; set; }

        Action<EventArgs> Update { get; set; }

        Action<EventArgs> Start { get; set; }

        Action<EndEventArgs> End { get; set; }

        Action<AfkEventArgs> Afk { get; set; }

        Action<NotifyEventArgs> Notify { get; set; }

        Action<PingEventArgs> Ping { get; set; }

        Action<InputEventArgs> Input { get; set; }

        Action<ChatEventArgs> Chat { get; set; }
    }
}