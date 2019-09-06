namespace SparkTech.SDK.Entities
{
    public interface ISpellbook
    {
        bool IsCastingSpell { get; }

        SpellSlot SelectedSpellSlot { get; }

        SpellSlot ActiveSpellSlot { get; }

        int OwnerId { get; }

        bool HasSpellCaster { get; }

        ISpell[] Spells { get; }

        bool IsAutoAttacking { get; }

        bool IsCharging { get; }

        bool IsChanneling { get; }

        // bool SpellWasCast(); ???

        float CastTime { get; }

        bool IsStopped { get; }

        ISpell GetSpell(SpellSlot slot);

        SpellState CanUseSpell(SpellSlot slot);
    }
}