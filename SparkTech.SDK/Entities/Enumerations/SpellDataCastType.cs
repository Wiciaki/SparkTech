namespace SparkTech.SDK.Entities
{
    public enum SpellDataCastType
    {
        Unknown = -1, // 0xFFFFFFFF
        Instant = 0,
        Missile = 1,
        ArcMissile = 3,
        CircleMissile = 4,
        CircleMissileSynchronized = 7,
        Spline = 8,
        TrackAndContinuePastTarget = 9,
    }
}
