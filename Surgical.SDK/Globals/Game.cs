namespace Surgical.SDK
{
    using System;

    using SharpDX;

    using Surgical.SDK.API.Fragments;
    using Surgical.SDK.Entities;
    using Surgical.SDK.EventData;

    public static class Game
    {
        private static IGameFragment game;

        internal static void Initialize(IGameFragment fragment)
        {
            game = fragment;

            fragment.Update = args => OnUpdate.SafeInvoke(args);
            fragment.Notify = args => OnNotify.SafeInvoke(args);
            fragment.Start = args => OnStart.SafeInvoke(args);
            fragment.End = args => OnEnd.SafeInvoke(args);
            fragment.Afk = args => OnAfk.SafeInvoke(args);
            fragment.Ping = args => OnPing.SafeInvoke(args);
            fragment.Input = args => OnInput.SafeInvoke(args);
            fragment.Chat = args => OnChat.SafeInvoke(args);
        }

        public static event Action<EventArgs> OnUpdate;

        public static event Action<EventArgs> OnStart;

        public static event Action<EndEventArgs> OnEnd;

        public static event Action<AfkEventArgs> OnAfk;

        public static event Action<NotifyEventArgs> OnNotify;

        public static event Action<PingEventArgs> OnPing;

        public static event Action<InputEventArgs> OnInput;

        public static event Action<ChatEventArgs> OnChat;

        public static Vector3 CursorPosition => game.CursorPosition;

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

        public static Vector3 ScreenToWorld(this Vector2 pos)
        {
            return game.ScreenToWorld(pos);
        }

        public static void SendEmote(Emote emote)
        {
            game.SendEmote(emote);
        }

        public static void SendPing(PingCategory category, IGameObject target)
        {
            game.SendPing(category, target);
        }

        public static void SendPing(PingCategory category, Vector2 target)
        {
            game.SendPing(category, target);
        }

        public static void ShowPing(PingCategory category, IGameObject target, bool playSound)
        {
            game.ShowPing(category, target, playSound);
        }

        public static void ShowPing(PingCategory category, Vector2 target, bool playSound)
        {
            game.ShowPing(category, target, playSound);
        }
    }
}