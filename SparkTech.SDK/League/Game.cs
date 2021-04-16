namespace SparkTech.SDK.League
{
    using System;

    using SharpDX;

    using SparkTech.SDK.API;
    using SparkTech.SDK.Entities;

    public static class Game
    {
        private static readonly IGameFragment Fragment;

        public static NavMesh NavMesh { get; }

        static Game()
        {
            Fragment = Platform.CoreFragment?.GetGameFragment() ?? throw Platform.FragmentException();
            Fragment.Update = args => OnUpdate.SafeInvoke(args);

            NavMesh = new NavMesh(Fragment.GetNavMesh());
        }

        public static event Action<EventArgs> OnUpdate;

        public static GameState State => Fragment.State;

        public static GameMap Map => Fragment.Map;

        public static Vector3 CursorPos => Fragment.CursorPos;

        public static float Time => Fragment.Time;

        public static int Ping => Fragment.Ping;

        public static Matrix ProjectionMatrix => Fragment.ProjectionMatrix;

        public static Matrix ViewMatrix => Fragment.ViewMatrix;

        public static void SendChat(string text)
        {
            Fragment.Say(text);
        }

        public static void ShowChat(string text)
        {
            Fragment.Print(text);
        }

        public static bool IsChatOpen()
        {
            return Fragment.IsChatOpen();
        }

        public static bool IsShopOpen()
        {
            return Fragment.IsShopOpen();
        }

        public static bool IsScoreboardOpen()
        {
            return Fragment.IsScoreboardOpen();
        }

        public static Vector2 WorldToScreen(Vector3 pos)
        {
            return Fragment.WorldToScreen(pos);
        }

        public static Vector2 WorldToMinimap(Vector3 pos)
        {
            return Fragment.WorldToMinimap(pos);
        }

        public static Vector3 ScreenToWorld(Vector2 pos)
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
    }
}