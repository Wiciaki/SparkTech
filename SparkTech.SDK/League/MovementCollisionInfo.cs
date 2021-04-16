namespace SparkTech.SDK.League
{
    using SharpDX;

    public struct MovementCollisionInfo
    {
        public Vector2 CollisionPosition { get; }

        public float CollisionTime { get; }

        public MovementCollisionInfo(Vector2 position, float time)
        {
            this.CollisionPosition = position;
            this.CollisionTime = time;
        }
    }
}