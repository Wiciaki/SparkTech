namespace SparkTech.SDK.Entities
{
    public interface IBuff
    {
        BuffType Type { get; }

        IGameObject Caster { get; }

        int Count { get; }

        float StartTime { get; }

        float EndTime { get; }

        bool IsActive { get; }

        bool IsPositive { get; }

        bool IsValid { get; }

        string Name { get; }
    }
}