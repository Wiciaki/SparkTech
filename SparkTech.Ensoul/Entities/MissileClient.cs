namespace SparkTech.Ensoul.Entities
{
    using SharpDX;

    using SparkTech.SDK.Entities;

    public class MissileClient<TEntity> : GameObject<TEntity>, IMissile where TEntity : EnsoulSharp.MissileClient
    {
        public MissileClient(TEntity entity) : base(entity)
        { }

        public IUnit Caster => ObjectManager.GetById<IUnit>(this.Entity.SpellCaster.NetworkId);
        public Vector3 StartPosition => this.Entity.StartPosition;
        public Vector3 EndPosition => this.Entity.EndPosition;
        public IGameObject Target => ObjectManager.GetById(this.Entity.Target.NetworkId);
        public SpellSlot Slot => (SpellSlot)this.Entity.Slot;
        public bool IsComplete => this.Entity.IsComplete;
        public float Width => this.Entity.Width;
        public float Speed => this.Entity.Speed;
        public bool IsDestroyed => this.Entity.IsDestroyed;
        public bool IsVisible => this.Entity.IsVisible;
    }
}
