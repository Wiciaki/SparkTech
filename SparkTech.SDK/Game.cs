namespace SparkTech.SDK
{
    using System;

    using SharpDX;

    using SparkTech.SDK.API;
    using SparkTech.SDK.Entities;
    using SparkTech.SDK.EventData;

    public static class Game
    {
        private static readonly IGameFragment Fragment;

        static Game()
        {
            Fragment = Platform.CoreFragment?.GetGameFragment() ?? throw Platform.FragmentException();

            Fragment.Update = args => OnUpdate.SafeInvoke(args);
            Fragment.Notify = args => OnNotify.SafeInvoke(args);
            Fragment.End = args => OnEnd.SafeInvoke(args);
            Fragment.Afk = args => OnAfk.SafeInvoke(args);
            Fragment.Ping = args => OnPing.SafeInvoke(args);
            Fragment.Input = args => OnInput.SafeInvoke(args);
            Fragment.Chat = args => OnChat.SafeInvoke(args);
        }

        public static event Action<EventArgs> OnUpdate;

        public static event Action<_EndEventArgs> OnEnd;

        public static event Action<_AfkEventArgs> OnAfk;

        public static event Action<NotifyEventArgs> OnNotify;

        public static event Action<_PingEventArgs> OnPing;

        public static event Action<_InputEventArgs> OnInput;

        public static event Action<_ChatEventArgs> OnChat;

        public static GameState State => Fragment.State;

        public static GameMap Map => Fragment.Map;

        public static Vector3 Cursor => Fragment.Cursor;

        public static float Time => Fragment.Time;

        public static float Ping => Fragment.Latency;

        public static Matrix ProjectionMatrix => Fragment.ProjectionMatrix;

        public static Matrix ViewMatrix => Fragment.ViewMatrix;

        public static void SendChat(string text)
        { 
            Fragment.SendChat(text);
        }

        public static void ShowChat(string text)
        {
            Fragment.ShowChat(text);
        }

        public static bool IsChatOpen()
        {
            return Fragment.IsChatOpen();
        }

        public static bool IsShopOpen()
        {
            return Fragment.IsShopOpen();
        }

        public static Vector2 WorldToScreen(this Vector3 pos)
        {
            return Fragment.WorldToScreen(pos);
        }

        public static Vector2 WorldToMinimap(this Vector3 pos)
        {
            return Fragment.WorldToMinimap(pos);
        }

        public static Vector3 ScreenToWorld(this Vector2 pos)
        {
            return Fragment.ScreenToWorld(pos);
        }

        public static void SendEmote(Emote emote)
        {
            Fragment.SendEmote(emote);
        }

        public static void SendPing(PingCategory category, IGameObject target)
        {
            Fragment.SendPing(category, target);
        }

        public static void SendPing(PingCategory category, Vector2 targetPos)
        {
            Fragment.SendPing(category, targetPos);
        }

        public static void ShowPing(PingCategory category, IGameObject target, bool playSound)
        {
            Fragment.ShowPing(category, target, playSound);
        }

        public static void ShowPing(PingCategory category, Vector2 targetPos, bool playSound)
        {
            Fragment.ShowPing(category, targetPos, playSound);
        }
    }
}