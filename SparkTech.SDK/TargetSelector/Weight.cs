namespace SparkTech.SDK.TargetSelector
{
    using System;
    using System.Collections.Generic;

    using SparkTech.SDK.Entities;
    using SparkTech.SDK.GUI.Menu;

    public abstract class Weight : IComparer<IHero>
    {
        private readonly MenuInt item;

        public int Value => this.item.Value;

        protected Weight(string id, int defaultValue)
        {
            this.item = new MenuInt(id, 0, 10, defaultValue);
        }

        protected abstract IComparable GetComparable(IHero target);

        public static List<Weight> CreateWeights(Menu menu)
        {
            TWeight I<TWeight>() where TWeight : Weight, new()
            {
                var weight = new TWeight();
                menu.Add(weight.item);
                return weight;
            }

            return new List<Weight> { I<PlayerDistanceWeight>(), I<MouseDistanceWeight>(), I<DealsMostDmgWeight>(), I<TimeToKillWeight>() };
        }

        public int Compare(IHero x, IHero y)
        {
            var left = this.GetComparable(x);
            var right = this.GetComparable(y);

            return left.CompareTo(right);
        }

        private class PlayerDistanceWeight : Weight
        {
            public PlayerDistanceWeight() : base("distancePlayer", 3)
            { }

            protected override IComparable GetComparable(IHero target)
            {
                return ObjectManager.Player.Distance(target);
            }
        }

        private class MouseDistanceWeight : Weight
        {
            public MouseDistanceWeight() : base("distanceMouse", 2)
            { }

            protected override IComparable GetComparable(IHero target)
            {
                return float.MaxValue - Game.Cursor.Distance(target);
            }
        }

        private class DealsMostDmgWeight : Weight
        {
            public DealsMostDmgWeight() : base("dealsMostDmg", 8)
            { }

            protected override IComparable GetComparable(IHero target)
            {
                return target.TotalAttackDamage + target.TotalMagicalDamage;
            }
        }

        private class TimeToKillWeight : Weight
        {
            public TimeToKillWeight() : base("timeToKill", 6)
            { }

            protected override IComparable GetComparable(IHero target)
            {
                return target.Health;
            }
        }
    }
}