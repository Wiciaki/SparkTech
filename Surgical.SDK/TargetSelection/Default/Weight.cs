namespace Surgical.SDK.TargetSelection.Default
{
    using System;
    using System.Collections.Generic;

    using Surgical.SDK.Entities;
    using Surgical.SDK.GUI.Menu;

    internal abstract class Weight : IComparer<IHero>
    {
        private MenuInt item;

        protected internal virtual IEnumerable<MenuItem> CreateItems()
        {
            yield return this.item = new MenuInt(this.GetId(), this.GetDefaultWeight(), 0, 20);
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