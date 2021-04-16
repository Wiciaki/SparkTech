namespace SparkTech.SDK.League
{
    using SharpDX;

    public sealed class NavMesh : INavMesh
    {
        private readonly INavMesh fragment;

        internal NavMesh(INavMesh fragment)
        {
            this.fragment = fragment;
        }

        public NavMeshCell GetCell(Vector2 position)
        {
            return this.GetCell((int)position.X, (int)position.Y);
        }

        public NavMeshCell GetCell(int x, int y)
        {
            return new NavMeshCell(x, y);
        }

        public CollisionFlags GetCollisionFlags(Vector3 position)
        {
            return this.GetCollisionFlags(position.X, position.Y);
        }

        public CollisionFlags GetCollisionFlags(float x, float y)
        {
            return this.fragment.GetCollisionFlags(x, y);
        }

        public float GetHeightForPosition(Vector2 position)
        {
            return this.GetHeightForPosition(position.X, position.Y);
        }

        public float GetHeightForPosition(float x, float y)
        {
            return this.fragment.GetHeightForPosition(x, y);
        }

        public Vector3 GridToWorld(Vector2 position)
        {
            return this.GridToWorld((int)position.X, (int)position.Y);
        }

        public Vector3 GridToWorld(int x, int y)
        {
            return this.fragment.GridToWorld(x, y);
        }

        public bool IsWallOfType(Vector3 position, CollisionFlags flags, float radius)
        {
            return this.IsWallOfType(position.X, position.Y, flags, radius);
        }

        public bool IsWallOfType(float x, float y, CollisionFlags flags, float radius)
        {
            return this.fragment.IsWallOfType(x, y, flags, radius);
        }

        public bool IsWater(NavMeshCell cell)
        {
            return this.IsWater(cell.X, cell.Y);
        }

        public bool IsWater(Vector3 position)
        {
            return this.IsWater(position.X, position.Y);
        }

        public bool IsWater(float x, float y)
        {
            return this.fragment.IsWater(x, y);
        }

        public void SetCollisionFlags(CollisionFlags flags, Vector3 position)
        {
            this.SetCollisionFlags(flags, position.X, position.Y);
        }

        public void SetCollisionFlags(CollisionFlags flags, float x, float y)
        {
            this.fragment.SetCollisionFlags(flags, x, y);
        }

        public Vector2 WorldToGrid(Vector3 position)
        {
            return this.WorldToGrid(position.X, position.Y);
        }

        public Vector2 WorldToGrid(float x, float y)
        {
            return this.fragment.WorldToGrid(x, y);
        }
    }
}