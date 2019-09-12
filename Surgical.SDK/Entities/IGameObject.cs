namespace Surgical.SDK.Entities
{
    using SharpDX;

    public interface IGameObject
    {
        int Id { get; }

        GameObjectTeam Team { get; }

        string Name { get; }

        bool IsDead { get; }

        bool IsVisible { get; }

        float BoundingRadius { get; }

        Vector3 Position { get; }

        Vector3 Orientation { get; }

        // needed?
        bool IsValid { get; }
    }
}