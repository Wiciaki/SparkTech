namespace SparkTech.SDK.DamageLibrary
{
    using Entities;
    using Modules;

    public interface IDamageLibrary : IModule
    {
        float GetAutoAttackDamage(IUnit source, IUnit target, bool includePassive);

        float GetSpellDamage(IHero source, IUnit target, SpellSlot slot, int stage);
    }
}