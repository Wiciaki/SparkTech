namespace SparkTech.SDK.API
{
    using System;

    using SparkTech.SDK.EventData;

    public interface IEntityEventsFragment
    {
        Action<NotifyEventArgs> Notify { get; set; }

        Action<PlayAnimationEventArgs> PlayAnimation { get; set; }

        Action<ProcessSpellCastEventArgs> ProcessSpellCast { get; set; }

        Action<ProcessSpellCastEventArgs> DoCast { get; set; }

        Action<NewPathEventArgs> NewPath { get; set; }

        Action<IssueOrderEventArgs> IssueOrder { get; set; }

        Action<TeleportEventArgs> Teleport { get; set; }

        Action<AggroEventArgs> Aggro { get; set; }

        Action<BuffUpdateEventArgs> BuffAdd { get; set; }

        Action<BuffUpdateEventArgs> BuffRemove { get; set; }

        Action<LevelUpEventArgs> LevelUp { get; set; }

        Action<CastSpellEventArgs> SpellbookCastSpell { get; set; }
        
        Action<StopCastEventArgs> SpellbookStopCast { get; set; }
        
        Action<UpdateChargedSpellEventArgs> SpellbookUpdateChargedSpell { get; set; }

        Action<PropertyChangeEventArgs<int>> OnIntegerPropertyChange { get; set; }

        Action<PropertyChangeEventArgs<float>> OnFloatPropertyChange { get; set; }
    }
}