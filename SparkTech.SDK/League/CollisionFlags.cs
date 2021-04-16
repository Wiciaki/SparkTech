namespace SparkTech.SDK.League
{
    using System;

    [Flags]
    public enum CollisionFlags
    {
        None = 0,
        Grass = 1,
        Wall = 2,
        Building = 64, // 0x00000040
        Prop = 128, // 0x00000080
        GlobalVision = 256, // 0x00000100
    }
}