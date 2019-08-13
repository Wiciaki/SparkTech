namespace SparkTech.SDK.TargetSelection.Default.Weights
{
    using System;

    using SparkTech.SDK.Entities;

    internal class DistanceWeight : Weight
    {
        protected override int GetDefaultWeight() => 2;

        protected override IComparable GetComparable(IHero target)
        {
            return target.DistanceToPlayer();
        }
    }
}