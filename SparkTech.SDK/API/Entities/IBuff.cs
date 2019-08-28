namespace SparkTech.SDK.Entities.Buffs
{
    public interface IBuff
    {
        BuffType Type();

        IGameObject CasterId();

        int Count();

        string DisplayName();

        float StartTime();

        float EndTime();

        bool IsActive();

        bool IsValid();

        string Name();

        string SourceName();
    }
}