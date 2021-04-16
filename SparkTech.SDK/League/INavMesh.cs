namespace SparkTech.SDK.League
{
    using SharpDX;

    public interface INavMesh
    {
        CollisionFlags GetCollisionFlags(float x, float y);

        void SetCollisionFlags(CollisionFlags flags, float x, float y);

        bool IsWallOfType(float x, float y, CollisionFlags flags, float radius);

        bool IsWater(float x, float y);

        Vector3 GridToWorld(int x, int y);

        Vector2 WorldToGrid(float x, float y);

        float GetHeightForPosition(float x, float y);
    }
}