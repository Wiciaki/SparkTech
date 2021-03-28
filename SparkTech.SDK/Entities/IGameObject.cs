namespace SparkTech.SDK.Entities
{
    using SharpDX;

    public interface IGameObject
    {
        int Id { get; }
        GameObjectTeam Team { get; }
        string Name { get; }
        bool IsDead { get; }
        bool IsValid { get; }
        float BoundingRadius { get; }
        Vector3 Position { get; }
    }
}