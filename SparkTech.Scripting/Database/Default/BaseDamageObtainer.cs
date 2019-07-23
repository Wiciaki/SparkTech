namespace SparkTech.Spells.Default
{
    using Newtonsoft.Json.Linq;

    internal delegate float BaseDamageObtainer(
        JObject spell,
        AIHeroClient attacker,
        AIBaseClient target,
        float? healthSimulated);
}