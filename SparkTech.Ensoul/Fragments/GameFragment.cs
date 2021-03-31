namespace SparkTech.Ensoul.Fragments
{
    using System;

    using EnsoulSharp;

    using SharpDX;

    using SDK;
    using SDK.API;
    using SDK.Entities;

    using Game = EnsoulSharp.Game;

    public class GameFragment : IGameFragment // TODO: unbreak this
    {
        public GameFragment()
        {
            Game.OnUpdate += args => this.Update(args);
            //Game.OnNotify += args => this.Notify(new NotifyEventArgs(null, (GameEvent)args.EventId));
        }

        public Matrix ProjectionMatrix => Drawing.Projection;
        public Matrix ViewMatrix => Drawing.View;
        public SDK.GameState State => (SDK.GameState)Game.State;
        public GameMap Map => (GameMap)Game.MapId;
        public float Time => Game.Time;
        public float Ping => Game.Ping;
        public Vector3 Cursor => Game.CursorPos;

        public Action<EventArgs> Update { get; set; }

        public bool IsChatOpen() => MenuGUI.IsChatOpen;

        public bool IsShopOpen() => MenuGUI.IsShopOpen;

        public bool IsScoreboardOpen() => MenuGUI.IsScoreboardOpen;

        public void SendChat(string text) => Game.Say(text, false);

        public void SendEmote(Emote emote) => Game.SendEmote((EmoteId)emote);

        public void SendPing(SDK.PingCategory category, IGameObject target)
        {
            Game.SendPing((EnsoulSharp.PingCategory)category, EnsoulSharp.ObjectManager.GetUnitByNetworkId<GameObject>(target.Id));
        }

        public void SendPing(SDK.PingCategory category, Vector2 target)
        {
            Game.SendPing((EnsoulSharp.PingCategory)category, target);
        }

        public void ShowChat(string text) => Game.Print(text);

        public Vector3 ScreenToWorld(Vector2 pos) => Drawing.ScreenToWorld(pos);

        public Vector2 WorldToMinimap(Vector3 pos) => Drawing.WorldToMinimap(pos);

        public Vector2 WorldToScreen(Vector3 pos) => Drawing.WorldToScreen(pos);
    }
}