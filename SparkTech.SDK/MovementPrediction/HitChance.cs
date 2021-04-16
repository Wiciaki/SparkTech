namespace SparkTech.SDK.MovementPrediction
{
    public enum HitChance
    {
        None = -2, // 0xFFFFFFFE
        Collision = -1, // 0xFFFFFFFF
        OutOfRange = 0,
        Low = 1,
        Medium = 2,
        High = 3,
        VeryHigh = 4,
        Immobile = 5,
        Dash = 6,
    }
}