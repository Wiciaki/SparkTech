namespace SparkTech.SDK.Entities
{
    using System;

    [Flags]
    public enum SpellState
    {
        Ready = 0x0,
        NoSpell = 0x2,
        NotLearned = 0x4,
        Disabled = 0x8,
        Unknown = 0xA,
        Surpressed = 0x10,
        Cooldown = 0x20,
        NoMana = 0x40
    }
}