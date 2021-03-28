namespace SparkTech.Ensoul.Entities
{
    using SharpDX;

    using SDK.Entities;

    public class GameObject<TEntity> : IGameObject where TEntity : EnsoulSharp.GameObject
    {
        protected readonly TEntity Entity;

        public GameObject(TEntity entity)
        {
            this.Entity = entity;
        }

        public int Id => this.Entity.NetworkId;

        public GameObjectTeam Team => (GameObjectTeam)this.Entity.Team;

        public string Name => this.Entity.Name;

        public bool IsDead => this.Entity.IsDead;

        public float BoundingRadius => this.Entity.BoundingRadius;

        public Vector3 Position => this.Entity.Position;

        public bool IsValid => this.Entity.IsValid;
    }
}