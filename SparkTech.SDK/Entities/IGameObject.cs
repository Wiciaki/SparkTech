namespace SparkTech.SDK.Entities
{
    using SharpDX;

    public interface IGameObject
    {
        int Id { get; }
        GameObjectTeam Team { get; }
        GameObjectType Type { get; }
        string Name { get; }
        bool IsDead { get; }
        bool IsValid { get; }
        float BoundingRadius { get; }
        BoundingBox BoundingBox { get; }
        Vector3 Position { get; }
        Vector3 PreviousPosition { get; }
    }
}