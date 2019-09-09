namespace SparkTech.SDK
{
    using System;

    using SharpDX;

    using SparkTech.SDK.API.Fragments;
    using SparkTech.SDK.EventArgs;

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
        }

        public static event Action<WndProcEventArgs> OnWndProc; 

        public static event Action<System.EventArgs> OnUpdate;

        public static event Action<System.EventArgs> OnStart;

        public static event Action<System.EventArgs> OnEnd;

        public static event Action<NotifyEventArgs> OnNotify;

        public static Vector2 CursorPosition => game.CursorPosition;

        public static float Time => game.Time;

        public static float Ping => game.Ping;

        public static void ChatShow(string text)
        {
            game.ChatShow(text);
        }

        public static void ChatPrint(string text)
        {
            game.ChatPrint(text);
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
    }
}