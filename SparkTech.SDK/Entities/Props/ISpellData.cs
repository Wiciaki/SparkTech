namespace SparkTech.SDK.Entities
{
    public interface ISpellData
    {
        string Name { get; }
        float CooldownTime { get; }
        bool UseMinimapTargeting { get; }
        bool IsToggleSpell { get; }
        float CastRange { get; }
        float CastRangeDisplayOverride { get; }
        float CastRadius { get; }
        float CastRadiusSecondary { get; }
        float CastConeAngle { get; }
        float CastConeDistance { get; }
        float CastFrame { get; }
        SpellDataCastType CastType { get; }
        float MissileSpeed { get; }
        float LineWidth { get; }
        float LineDragLength { get; }
        SpellDataTargetType TargetingType { get; }
        float[] CooldownTimeArray { get; }
        float[] CastRangeArray { get; }
        float[] CastRangeDisplayOverrideArray { get; }
        float[] CastRadiusArray { get; }
        float[] CastRadiusSecondaryArray { get; }
        float[] ManaArray { get; }
    }
}