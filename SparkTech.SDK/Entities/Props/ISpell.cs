namespace SparkTech.SDK.Entities
{
    public interface ISpell
    {
        int Level { get; }
        bool Learned { get; }
        float CooldownExpires { get; }
        int NumericalDisplay { get; }
        int Ammo { get; }
        SpellToggleState ToggleState { get; }
        float Cooldown { get; }
        float[] TooltipVars { get; }
        int IconUsed { get; }
        string Name { get; }
        float ManaCost { get; }
        ISpellData SpellData { get; }
        SpellSlot Slot { get; }
        SpellState State { get; }
    }
}