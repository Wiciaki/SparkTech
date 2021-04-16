namespace SparkTech.SDK.League
{
    using SharpDX;

    public class NavMeshCell
    {
        public int X { get; }

        public int Y { get; }

        public Vector3 WorldPosition { get; }

        internal NavMeshCell(int x, int y)
        {
            this.X = x;
            this.Y = y;

            this.WorldPosition = Game.NavMesh.GridToWorld(x, y);
        }

        public CollisionFlags CollisionFlags
        {
            get => Game.NavMesh.GetCollisionFlags(this.WorldPosition.X, this.WorldPosition.Y);
            set => Game.NavMesh.SetCollisionFlags(value, this.WorldPosition.X, this.WorldPosition.Y);
        }
    }
}