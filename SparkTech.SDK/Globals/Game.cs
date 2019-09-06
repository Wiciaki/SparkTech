namespace SparkTech.SDK
{
    using System;

    using SharpDX;

    using SparkTech.SDK.API.Fragments;

    public static class Game
    {
        private static IGame api;

        internal static void Initialize(IGame game)
        {
            api = game;

            OnWndProc = args => OnWndProc.SafeInvoke(args);
        }

        public static event Action<WndProcEventArgs> OnWndProc; 

        public static Vector2 CursorPosition => api.CursorPosition;

        public static float Time => api.Time;

        public static float Ping => api.Ping;

        public static void ChatShow(string text)
        {
            api.ChatShow(text);
        }

        public static void ChatPrint(string text)
        {
            api.ChatPrint(text);
        }

        public static bool IsChatOpen()
        {
            return api.IsChatOpen();
        }

        public static bool IsShopOpen()
        {
            return api.IsShopOpen();
        }

        public static Vector2 WorldToScreen(this Vector3 pos)
        {
            return api.WorldToScreen(pos);
        }

        public static Vector2 WorldToMinimap(this Vector3 pos)
        {
            return api.WorldToMinimap(pos);
        }

        public static Vector3 ScreenToWorld(this Vector3 pos)
        {
            return api.ScreenToWorld(pos);
        }
    }
}