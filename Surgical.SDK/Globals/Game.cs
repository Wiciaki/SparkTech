﻿namespace Surgical.SDK
{
    using System;
    using System.Linq;

    using SharpDX;

    using Surgical.SDK.API.Fragments;
    using Surgical.SDK.Entities;
    using Surgical.SDK.EventData;
    using Surgical.SDK.Logging;

    public static class Game
    {
        private static IGame game;

        internal static void Initialize(IGame g)
        {
            game = g;

            g.WndProc = args => OnWndProc.SafeInvoke(args);
            g.Update = args => OnUpdate.SafeInvoke(args);
            g.Notify = args => OnNotify.SafeInvoke(args);
            g.Start = args => OnStart.SafeInvoke(args);
            g.End = args => OnEnd.SafeInvoke(args);
            g.Afk = args => OnAfk.SafeInvoke(args);
            g.Ping = args => OnPing.SafeInvoke(args);
            g.Input = args => OnInput.SafeInvoke(args);
            g.Chat = args => OnChat.SafeInvoke(args);
        }

        public static event Action<WndProcEventArgs> OnWndProc; 

        public static event Action<EventArgs> OnUpdate;

        public static event Action<EventArgs> OnStart;

        public static event Action<EndEventArgs> OnEnd;

        public static event Action<AfkEventArgs> OnAfk;

        public static event Action<NotifyEventArgs> OnNotify;

        public static event Action<PingEventArgs> OnPing;

        public static event Action<InputEventArgs> OnInput;

        public static event Action<ChatEventArgs> OnChat;

        public static Vector3 CursorPosition => game.CursorPosition;

        public static Vector2 CursorPosition2D => game.CursorPosition2D;

        public static GameMode Mode => game.Mode;

        public static GameType Type => game.Type;

        public static GameMap Map => game.Map;

        public static float Time => game.Time;

        public static float ClockTime => game.ClockTime;

        public static float Latency => game.Latency;

        public static long Id => game.Id;

        public static string Region => game.Region;

        public static void SendChat(string text)
        { 
            game.SendChat(text);
        }

        public static void ShowChat(string text)
        {
            game.ShowChat(text);
        }

        public static bool IsChatOpen()
        {
            return game.IsChatOpen();
        }

        public static bool IsShopOpen()
        {
            return game.IsShopOpen();
        }

        public static Vector2 WorldToScreen(this Vector3 pos)
        {
            return game.WorldToScreen(pos);
        }

        public static Vector2 WorldToMinimap(this Vector3 pos)
        {
            return game.WorldToMinimap(pos);
        }

        public static Vector3 ScreenToWorld(this Vector3 pos)
        {
            return game.ScreenToWorld(pos);
        }

        public static void SendEmote(Emote emote)
        {
            game.SendEmote(emote);
        }

        public static void SendPing(PingCategory pingCategory, IGameObject target)
        {
            game.SendPing(pingCategory, target);
        }

        public static void SendPing(PingCategory pingCategory, Vector2 target)
        {
            game.SendPing(pingCategory, target);
        }

        public static void ShowPing(PingCategory pingCategory, IGameObject target, bool playSound)
        {
            game.ShowPing(pingCategory, target, playSound);
        }

        public static void ShowPing(PingCategory pingCategory, Vector2 target, bool playSound)
        {
            game.ShowPing(pingCategory, target, playSound);
        }

        internal static void SafeInvoke<T>(this Action<T> evt, T args)
        {
            if (evt == null)
            {
                return;
            }

            foreach (var callback in evt.GetInvocationList().Cast<Action<T>>())
            {
                try
                {
                    callback(args);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
        }
    }
}