namespace SparkTech.SDK.Champion
{
    using SparkTech.SDK.Entities;
    using SparkTech.SDK.Modules;

    public interface IChampion : IModule
    {
        float GetHealthIndicatorDamage(IHero hero);

        // GetSequence
    }
}