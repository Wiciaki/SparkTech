namespace Surgical.SDK.Entities
{
    public interface ISpellbook
    {
        bool IsCastingSpell { get; }

        SpellSlot SelectedSpellSlot { get; }

        SpellSlot ActiveSpellSlot { get; }

        IUnit Owner { get; }

        bool HasSpellCaster { get; }

        ISpellData[] SpellDatas { get; }

        bool IsAutoAttacking { get; }

        bool IsCharging { get; }

        bool IsChanneling { get; }

        // bool SpellWasCast { get; } ???

        float CastTime { get; }

        bool IsStopped { get; }

        ISpellData GetSpellData(SpellSlot slot);

        SpellState CanUseSpell(SpellSlot slot);
    }
}