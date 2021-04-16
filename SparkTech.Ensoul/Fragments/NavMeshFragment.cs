namespace SparkTech.Ensoul.Fragments
{
    using SharpDX;

    using SparkTech.SDK.League;

    using NavMesh = EnsoulSharp.NavMesh;

    public class NavMeshFragment : INavMesh
    {
        public CollisionFlags GetCollisionFlags(float x, float y) => (CollisionFlags)NavMesh.GetCollisionFlags(x, y);
        public float GetHeightForPosition(float x, float y) => NavMesh.GetHeightForPosition(x, y);
        public Vector3 GridToWorld(int x, int y) => NavMesh.GridToWorld(x, y);
        public bool IsWallOfType(float x, float y, CollisionFlags flags, float radius) => NavMesh.IsWallOfType(x, y, (EnsoulSharp.CollisionFlags)flags, radius);
        public bool IsWater(float x, float y) => NavMesh.IsWater(x, y);
        public void SetCollisionFlags(CollisionFlags flags, float x, float y) => NavMesh.SetCollisionFlags((EnsoulSharp.CollisionFlags)flags, x, y);
        public Vector2 WorldToGrid(float x, float y) => NavMesh.WorldToGrid(x, y);
    }
}