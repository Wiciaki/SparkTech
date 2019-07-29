namespace SparkTech.SDK.Entities
{
    public interface IGameObject
    {
        int Id();

        GameObjectTeam Team();

        string Name();

        bool IsDead();

        bool IsVisible();

        float BoundingRadius();

        float PositionX();
        float PositionY();
        float PositionZ();

        float OrientationX();
        float OrientationY();
        float OrientationZ();

        // needed?
        bool IsValid();
    }
}