namespace SparkTech.Ensoul.Entities.Props
{
    using SparkTech.SDK.Entities;

    public class SpellData : PropWrapper<EnsoulSharp.SpellData>, ISpellData
    {
        public SpellData(EnsoulSharp.SpellData data) : base(data)
        { }

        public string Name => this.Prop.Name;
        public float CooldownTime => this.Prop.CooldownTime;
        public bool UseMinimapTargeting => this.Prop.UseMinimapTargeting;
        public bool IsToggleSpell => this.Prop.IsToggleSpell;
        public float CastRange => this.Prop.CastRange;
        public float CastRangeDisplayOverride => this.Prop.CastRangeDisplayOverride;
        public float CastRadius => this.Prop.CastRadius;
        public float CastRadiusSecondary => this.Prop.CastRadiusSecondary;
        public float CastConeAngle => this.Prop.CastConeAngle;
        public float CastConeDistance => this.Prop.CastConeDistance;
        public float CastFrame => this.Prop.CastFrame;
        public SpellDataCastType CastType => (SpellDataCastType)this.Prop.CastType;
        public float MissileSpeed => this.Prop.MissileSpeed;
        public float LineWidth => this.Prop.LineWidth;
        public float LineDragLength => this.Prop.LineDragLength;
        public SpellDataTargetType TargetingType => (SpellDataTargetType)this.Prop.TargetingType;
        public float[] CooldownTimeArray => this.Prop.CooldownTimeArray;
        public float[] CastRangeArray => this.Prop.CastRangeArray;
        public float[] CastRangeDisplayOverrideArray => this.Prop.CastRangeDisplayOverrideArray;
        public float[] CastRadiusArray => this.Prop.CastRadiusArray;
        public float[] CastRadiusSecondaryArray => this.Prop.CastRadiusSecondaryArray;
        public float[] ManaArray => this.Prop.ManaArray;
    }
}