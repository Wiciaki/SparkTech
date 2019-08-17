﻿namespace SparkTech.SDK.TargetSelection.Default
{
    using System;
    using System.Collections.Generic;

    using SparkTech.SDK.Entities;
    using SparkTech.SDK.GUI.Menu;
    using SparkTech.SDK.GUI.Menu.Items;

    internal abstract class Weight : IComparer<IHero>
    {
        private MenuSlider item;

        protected internal virtual IEnumerable<MenuItem> CreateItems()
        {
            yield return this.item = new MenuSlider(this.GetId(), this.GetDefaultWeight(), 0, 20);
        }

        internal int GetWeight() => this.item.Value;

        protected virtual string GetId()
        {
            return this.GetType().Name.Replace("Weight", string.Empty);
        }

        protected abstract int GetDefaultWeight();

        protected abstract IComparable GetComparable(IHero target);

        int IComparer<IHero>.Compare(IHero x, IHero y)
        {
            return this.GetComparable(x).CompareTo(this.GetComparable(y));
        }
    }
}