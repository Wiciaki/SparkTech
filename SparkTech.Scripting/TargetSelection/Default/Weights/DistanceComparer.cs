namespace SparkTech.TargetSelector.Default.Weights
{
    using System;

    using SparkTech.Utils;

    internal class DistanceWeight : Weight
    {
        protected override int GetDefaultWeight() => 2;

        protected override IComparable GetComparable(AIHeroClient target) => target.DistanceToPlayer();
    }
}