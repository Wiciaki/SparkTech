namespace SparkTech.Ensoul.Fragments
{
    using System;

    using EnsoulSharp;

    using SDK.API;
    using SDK.EventData;

    using SparkTech.Ensoul.Entities;

    public class EntityEventsFragment : IEntityEventsFragment
    {
        public EntityEventsFragment()
        {
            AIBaseClient.OnPlayAnimation += (o, args) =>
            {
                var arg = new PlayAnimationEventArgs(o.NetworkId, args.Animation);
                this.PlayAnimation(arg);
                if (arg.IsBlocked) { args.Process = false; }
            };
            AIBaseClient.OnProcessSpellCast += (o, args) => this.ProcessSpellCast(new ProcessSpellCastEventArgs(o.NetworkId, EntityConverter.Convert(args.SData), args.Level, args.Start, args.To, args.End, EntityConverter.T(args.Target?.NetworkId), args.MissileNetworkId, args.CastTime, args.TotalTime, (SDK.Entities.SpellSlot)args.Slot));
            AIBaseClient.OnDoCast += (o, args) => this.DoCast(new ProcessSpellCastEventArgs(o.NetworkId, EntityConverter.Convert(args.SData), args.Level, args.Start, args.To, args.End, EntityConverter.T(args.Target?.NetworkId), args.MissileNetworkId, args.CastTime, args.TotalTime, (SDK.Entities.SpellSlot)args.Slot));
            AIBaseClient.OnNewPath += (o, args) => this.NewPath(new NewPathEventArgs(o.NetworkId, args.Path, args.IsDash, args.Speed));
            AIBaseClient.OnIssueOrder += (o, args) =>
            {
                var arg = new IssueOrderEventArgs(o.NetworkId, (SDK.Entities.GameObjectOrder)args.Order, args.TargetPosition, EntityConverter.T(args.Target?.NetworkId), args.IsAttackMove, args.IsPetCommand);
                this.IssueOrder(arg);
                if (arg.IsBlocked) { args.Process = false; };
            };
            AttackableUnit.OnTeleport += (o, args) => this.Teleport(new TeleportEventArgs(o.NetworkId, args.RecallType, args.RecallName));
            AIBaseClient.OnAggro += (o, args) => this.Aggro(new AggroEventArgs(o.NetworkId, args.NetworkId));
            //AIBaseClient.OnSwapItem += args => this.SwapItem();
            //AIBaseClient.OnPlaceItem += args => this.PlaceItemInSlot();
            //AIBaseClient.OnRemoveItem += args => this.RemoveItem();
            AIBaseClient.OnBuffAdd += (o, args) => this.BuffAdd(new BuffUpdateEventArgs(o.NetworkId, EntityConverter.Convert(args.Buff)));
            AIBaseClient.OnBuffRemove += (o, args) => this.BuffRemove(new BuffUpdateEventArgs(o.NetworkId, EntityConverter.Convert(args.Buff)));
            //EnsoulSharp.AIBaseClient.O += args => this.BuffUpdateCount();
            AIHeroClient.OnLevelUp += (o, args) => this.LevelUp(new LevelUpEventArgs(o.NetworkId, args.Level));
            //AIBaseClient.OnAggro += args => this.PauseAnimation();
            //AIBaseClient.OnAggro += args => this.Target();
            Spellbook.OnCastSpell += (o, args) =>
            {
                var arg = new CastSpellEventArgs(EntityConverter.Convert(o), args.StartPosition, args.EndPosition, EntityConverter.T(args.Target?.NetworkId), (SDK.Entities.SpellSlot)args.Slot);
                this.SpellbookCastSpell(arg);
                if (arg.IsBlocked) { args.Process = false; };
            };
            Spellbook.OnStopCast += (o, args) => this.SpellbookStopCast(new StopCastEventArgs(EntityConverter.Convert(o), args.MissileNetworkId, args.KeepAnimationPlaying, args.HasBeenCast, args.SpellStopCancelled, args.DestroyMissile, args.SpellCastId));
            Spellbook.OnUpdateChargedSpell += (o, args) =>
            {
                var arg = new UpdateChargedSpellEventArgs(EntityConverter.Convert(o), (SDK.Entities.SpellSlot)args.Slot, args.Position, args.ReleaseCast);
                this.SpellbookUpdateChargedSpell(arg);
                if (arg.IsBlocked) { args.Process = false; };
            };
            //EnsoulSharp.AIBaseClient.On += args => this.EnterLocalVisiblityClient();
        }

        public Action<PlayAnimationEventArgs> PlayAnimation { get; set; }
        public Action<ProcessSpellCastEventArgs> ProcessSpellCast { get; set; }
        public Action<ProcessSpellCastEventArgs> DoCast { get; set; }
        public Action<NewPathEventArgs> NewPath { get; set; }
        public Action<IssueOrderEventArgs> IssueOrder { get; set; }
        public Action<TeleportEventArgs> Teleport { get; set; }
        public Action<AggroEventArgs> Aggro { get; set; }
        public Action<BuffUpdateEventArgs> BuffAdd { get; set; }
        public Action<BuffUpdateEventArgs> BuffRemove { get; set; }
        public Action<LevelUpEventArgs> LevelUp { get; set; }
        public Action<CastSpellEventArgs> SpellbookCastSpell { get; set; }
        public Action<StopCastEventArgs> SpellbookStopCast { get; set; }
        public Action<UpdateChargedSpellEventArgs> SpellbookUpdateChargedSpell { get; set; }
    }
}