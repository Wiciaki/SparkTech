namespace SparkTech.SDK.Entities
{
    using EventData;

    public interface ISpellbook
    {
        ISpell[] Spells { get; }

        IUnit Owner { get; }

        ProcessSpellCastEventArgs ActiveSpell { get; }

        bool IsStopped { get; }
        bool IsCastingSpell { get; }
        bool IsWindingUp { get; }
        bool IsAutoAttack { get; }
        bool IsCharging { get; }
        bool IsChanneling { get; }
        bool SpellWasCast { get; }

        float CastTime { get; }
        float CastEndTime { get; }

        ISpell GetSpell(SpellSlot slot);

        SpellState CanUseSpell(SpellSlot slot);
    }
}