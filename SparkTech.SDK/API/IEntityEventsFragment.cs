﻿namespace SparkTech.SDK.API
{
    using System;

    using SparkTech.SDK.EventData;

    public interface IEntityEventsFragment
    {
        //Action<DamageEventArgs> Damage { get; set; }

        Action<PlayAnimationEventArgs> PlayAnimation { get; set; }

        Action<ProcessSpellCastEventArgs> ProcessSpellCast { get; set; }

        Action<ProcessSpellCastEventArgs> DoCast { get; set; }

        Action<NewPathEventArgs> NewPath { get; set; }

        Action<IssueOrderEventArgs> IssueOrder { get; set; }

        Action<TeleportEventArgs> Teleport { get; set; }

        Action<AggroEventArgs> Aggro { get; set; }

        //Action<SwapItemEventArgs> SwapItem { get; set; }

        //Action<PlaceItemInSlotEventArgs> PlaceItemInSlot { get; set; }

        //Action<RemoveItemEventArgs> RemoveItem { get; set; }

        Action<BuffUpdateEventArgs> BuffAdd { get; set; }

        Action<BuffUpdateEventArgs> BuffRemove { get; set; }

        //Action<BuffUpdateEventArgs> BuffUpdateCount { get; set; }

        Action<LevelUpEventArgs> LevelUp { get; set; }

        //Action<PauseAnimationEventArgs> PauseAnimation { get; set; }

        //Action<TargetEventArgs> Target { get; set; }

        Action<CastSpellEventArgs> SpellbookCastSpell { get; set; }
        
        Action<StopCastEventArgs> SpellbookStopCast { get; set; }
        
        Action<UpdateChargedSpellEventArgs> SpellbookUpdateChargedSpell { get; set; }
    }
}