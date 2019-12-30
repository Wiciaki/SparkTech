namespace Surgical.SDK.Champion
{
    using Surgical.SDK.Entities;
    using Surgical.SDK.Modules;

    public interface IChampion : IModule
    {
        float GetHealthIndicatorDamage(IHero hero);
    }
}