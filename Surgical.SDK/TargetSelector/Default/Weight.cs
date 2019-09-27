namespace Surgical.SDK.TargetSelector.Default
{
    using System;
    using System.Collections.Generic;

    using Surgical.SDK.Entities;
    using Surgical.SDK.GUI.Menu;

    public abstract class Weight : IComparer<IHero>
    {
        private MenuInt item;

        protected internal virtual IEnumerable<MenuItem> CreateItems()
        {
            yield return this.item = new MenuInt(this.GetId(), 0, 20, this.GetDefaultWeight());
        }

        public int Importance => this.item.Value;

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