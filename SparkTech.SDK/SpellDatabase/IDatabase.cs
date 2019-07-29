namespace SparkTech.SDK.SpellDatabase
{
    using SparkTech.SDK.Enumerations;

    public interface IDatabase : IEntropyModule
    {
        Version Version { get; }

        event Action Updated;

        string GetName(SpellLink spell);

        string GetKey(SpellLink spell);

        float[] GetRange(SpellLink spell);

        float[] GetCooldown(SpellLink spell);

        float[] GetCost(SpellLink spell);

        int GetMaxRank(SpellLink spell);

        float AutoAttackDamage(AIBaseClient attacker, AttackableUnit target, float? healthSimulated = null);

        float SpellDamage(Spell spell, AIBaseClient target, DamageStage stage, float? healthSimulated = null);

        float SummonerSpellDamage(AIHeroClient attacker, AIBaseClient target, SummonerSpell spell, DamageStage stage);
    }
}