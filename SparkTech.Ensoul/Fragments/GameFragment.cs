﻿namespace SparkTech.Ensoul.Fragments
{
    using System;

    using EnsoulSharp;

    using SharpDX;

    using SDK.API;
    using SDK.Entities;

    using Game = EnsoulSharp.Game;
    using SparkTech.SDK.League;

    public class GameFragment : IGameFragment
    {
        public GameFragment() => Game.OnUpdate += args => this.Update(args);

        public INavMesh GetNavMesh() => new NavMeshFragment();
        public Action<EventArgs> Update { get; set; }

        public Matrix ProjectionMatrix => Drawing.Projection;
        public Matrix ViewMatrix => Drawing.View;
        public SDK.League.GameState State => (SDK.League.GameState)Game.State;
        public GameMap Map => (GameMap)Game.MapId;
        public float Time => Game.Time;
        public int Ping => Game.Ping;
        public int FPS => Game.FPS;
        public Vector3 CursorPos => Game.CursorPos;
        public string Version => Game.Version;

        public bool IsChatOpen() => MenuGUI.IsChatOpen;
        public bool IsShopOpen() => MenuGUI.IsShopOpen;
        public bool IsScoreboardOpen() => MenuGUI.IsScoreboardOpen;

        public void Say(string text) => Game.Say(text, false);
        public void Print(string text) => Game.Print(text);

        public Vector3 ScreenToWorld(Vector2 pos) => Drawing.ScreenToWorld(pos);
        public Vector2 WorldToMinimap(Vector3 pos) => Drawing.WorldToMinimap(pos);
        public Vector2 WorldToScreen(Vector3 pos) => Drawing.WorldToScreen(pos);

        public void SendEmote(Emote emote) => Game.SendEmote((EmoteId)emote);

        public void SendPing(SDK.League.PingCategory category, IGameObject target)
        {
            Game.SendPing((EnsoulSharp.PingCategory)category, EnsoulSharp.ObjectManager.GetUnitByNetworkId<GameObject>(target.Id));
        }

        public void SendPing(SDK.League.PingCategory category, Vector2 target)
        {
            Game.SendPing((EnsoulSharp.PingCategory)category, target);
        }

        public void SendSummonerEmote(SDK.League.SummonerEmoteSlot slot)
        {
            Game.SendSummonerEmote((EnsoulSharp.SummonerEmoteSlot)slot);
        }

        public void SendMasteryBadge()
        {
            Game.SendMasteryBadge();
        }
    }
}