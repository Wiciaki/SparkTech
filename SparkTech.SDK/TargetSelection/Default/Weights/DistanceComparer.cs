namespace SparkTech.SDK.TargetSelection.Default.Weights
{
    using System;

    internal class DistanceWeight : Weight
    {
        protected override int GetDefaultWeight() => 2;

        protected override IComparable GetComparable(AIHeroClient target) => target.DistanceToPlayer();
    }
}