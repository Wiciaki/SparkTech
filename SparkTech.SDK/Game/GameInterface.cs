namespace SparkTech.SDK.Game
{
    using SharpDX;

    using SparkTech.SDK.Platform.API;

    public static class GameInterface
    {
        private static IGameInterface api;

        public static void ChatShow(string text) => api.ChatShow(text);

        public static void ChatPrint(string text) => api.ChatPrint(text);

        //public static Point CursorPosition() => api.CursorPosition();

        public static float Time() => 0f;

        public static bool IsChatOpen() => api.IsChatOpen();

        public static bool IsShopOpen() => api.IsShopOpen();

        public static Vector2 WorldToScreen(this Vector3 pos) => api.WorldToScreen(pos);

        public static Vector2 WorldToMinimap(this Vector3 pos) => api.WorldToMinimap(pos);

        public static Vector3 ScreenToWorld(this Vector3 pos) => api.ScreenToWorld(pos);

        internal static void Initialize(IGameInterface game)
        {
            api = game;
        }
    }
}