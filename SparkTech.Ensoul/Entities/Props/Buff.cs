namespace SparkTech.Ensoul.Entities.Props
{
    using SparkTech.SDK.Entities;

    public class Buff : PropWrapper<EnsoulSharp.BuffInstance>, IBuff
    {
        public Buff(EnsoulSharp.BuffInstance buff) : base(buff)
        { }

        public BuffType Type => (BuffType)this.Prop.Type;
        public IGameObject Caster => ObjectManager.GetById(this.Prop.Caster.NetworkId);
        public int Count => this.Prop.Count;
        public float StartTime => this.Prop.StartTime;
        public float EndTime => this.Prop.EndTime;
        public bool IsActive => this.Prop.IsActive;
        public bool IsPositive => this.Prop.IsPositive;
        public bool IsValid => this.Prop.IsValid;
        public string Name => this.Prop.Name;
    }
}