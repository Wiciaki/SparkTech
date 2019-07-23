namespace SparkTech.TargetSelector.Default
{
    using System;
    using System.Collections.Generic;

    using SparkTech.UI.Menu;
    using SparkTech.UI.Menu.Values;

    internal abstract class Weight : IComparer<AIHeroClient>
    {
        private MenuSlider item;

        protected internal virtual IEnumerable<MenuComponent> CreateItems()
        {
            yield return this.item = new MenuSlider(this.GetId(), this.GetDefaultWeight(), 0, 20);
        }

        internal int GetWeight() => this.item.Value;

        protected virtual string GetId()
        {
            return this.GetType().Name.Replace("Weight", string.Empty);
        }

        protected abstract int GetDefaultWeight();

        protected abstract IComparable GetComparable(AIHeroClient target);

        int IComparer<AIHeroClient>.Compare(AIHeroClient x, AIHeroClient y)
        {
            return this.GetComparable(x).CompareTo(this.GetComparable(y));
        }
    }
}