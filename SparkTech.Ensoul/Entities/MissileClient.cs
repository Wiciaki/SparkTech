namespace SparkTech.Ensoul.Entities
{
    using SharpDX;

    using SparkTech.SDK.Entities;

    public class MissileClient<TEntity> : GameObject<TEntity>, IMissile where TEntity : EnsoulSharp.MissileClient
    {
        public MissileClient(TEntity entity) : base(entity)
        { }

        public IUnit Caster
        {
            get
            {
                var caster = this.Entity.SpellCaster;

                return caster == null ? null : ObjectManager.GetById<IUnit>(caster.NetworkId);
            }
        }

        public IGameObject Target
        {
            get
            {
                var target = this.Entity.Target;

                return target == null ? null : ObjectManager.GetById(target.NetworkId);
            }
        }

        public Vector3 StartPosition => this.Entity.StartPosition;
        public Vector3 EndPosition => this.Entity.EndPosition;
        public SpellSlot Slot => (SpellSlot)this.Entity.Slot;
        public bool IsComplete => this.Entity.IsComplete;
        public float Width => this.Entity.Width;
        public float Speed => this.Entity.Speed;
        public bool IsDestroyed => this.Entity.IsDestroyed;
        public bool IsVisible => this.Entity.IsVisible;
    }
}
