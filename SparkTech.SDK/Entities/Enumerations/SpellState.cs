namespace SparkTech.SDK.Entities
{
    public enum SpellState
    {
        Ready = 0,
        Surpressed = 8,
        Unknown = 10, // 0x0000000A
        NotLearned = 12, // 0x0000000C
        Disabled = 24, // 0x00000018
        Cooldown = 32, // 0x00000020
        NoMana = 96, // 0x00000060
    }
}