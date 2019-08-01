namespace SparkTech.SDK.Platform.API
{
    using System;

    using SparkTech.SDK.Entities;
    using SparkTech.SDK.Entities.Spells;

    public interface ISpellbook
    {
        Action<SpellbookCastSpellEventArgs> CastSpell { get; set; }

        Action<SpellbookStopCastEventArgs> StopCast { get; set; }

        Action<SpellbookUpdateChargedSpell> UpdateChargedSpell { get; set; }

        bool IsCastingSpell();

        SpellSlot SelectedSpellSlot();

        SpellSlot ActiveSpellSlot();

        int OwnerId();

        bool HasSpellCaster();

        Spell[] Spells();

        bool IsAutoAttacking();

        bool IsCharging();

        bool IsChanneling();

        // bool SpellWasCast(); ???

        float CastTime();

        bool IsStopped();

        Spell GetSpell(SpellSlot slot);

        SpellState CanUseSpell(SpellSlot slot);
    }
}