namespace SparkTech.SDK.Entities
{
    using SharpDX;

    public interface IGameObject
    {
        int Id();

        GameObjectTeam Team();

        string Name();

        bool IsDead();

        bool IsVisible();

        float BoundingRadius();

        Vector3 Position();

        Vector3 Orientation();

        // needed?
        bool IsValid();
    }
}