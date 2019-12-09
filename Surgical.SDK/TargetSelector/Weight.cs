namespace Surgical.SDK.TargetSelector
{
    using System;
    using System.Collections.Generic;

    using Surgical.SDK.Entities;
    using Surgical.SDK.GUI.Menu;

    internal abstract class Weight : IComparer<IHero>
    {
        private readonly MenuInt item;

        protected Weight(string id, int defaultValue)
        {
            this.item = new MenuInt(id, 0, 10, defaultValue);
        }

        public static List<Weight> GetWeights(Menu menu)
        {
            return new List<Weight> { I<PlayerDistanceWeight>(), I<MouseDistanceWeight>(), I<DealsMostDmgWeight>(), I<TimeToKillWeight>() };

            TWeight I<TWeight>() where TWeight : Weight, new()
            {
                var weight = new TWeight();

                menu.Add(weight.item);

                return weight;
            }
        }

        public int Importance => this.item.Value;

        protected abstract IComparable GetComparable(IHero target);

        int IComparer<IHero>.Compare(IHero x, IHero y)
        {
            var left = this.GetComparable(x);
            var right = this.GetComparable(y);

            return left.CompareTo(right);
        }

        private class PlayerDistanceWeight : Weight
        {
            public PlayerDistanceWeight() : base("distancePlayer", 3)
            {

            }

            protected override IComparable GetComparable(IHero target)
            {
                return ObjectManager.Player.Distance(target);
            }
        }

        private class MouseDistanceWeight : Weight
        {
            public MouseDistanceWeight() : base("distanceMouse", 2)
            {

            }

            protected override IComparable GetComparable(IHero target)
            {
                return Game.CursorPosition.Distance(target);
            }
        }

        private class DealsMostDmgWeight : Weight
        {
            public DealsMostDmgWeight() : base("dealsMostDmg", 8)
            {

            }

            protected override IComparable GetComparable(IHero target)
            {
                return 0;
                //return target.AP + target.AD;
            }
        }

        private class TimeToKillWeight : Weight
        {
            public TimeToKillWeight() : base("timeToKill", 6)
            {

            }

            protected override IComparable GetComparable(IHero target)
            {
                return 0;
            }
        }
    }
}