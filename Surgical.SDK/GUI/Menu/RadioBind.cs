namespace Surgical.SDK.GUI.Menu
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Surgical.SDK.EventData;

    public class RadioBind
    {
        private bool block;

        private readonly List<MenuItem> items = new List<MenuItem>();

        public void Add(MenuItem item)
        {
            if (!(item is MenuBool))
            {
                throw new ArgumentException("Added something different than MenuBool to a RadioBind!");
            }

            this.items.Add(item);

            item.BeforeValueChange += args => this.BeforeValueChange(item, args);
        }

        private void BeforeValueChange(MenuItem item, BeforeValueChangeEventArgs args)
        {
            if (this.block)
            {
                return;
            }

            this.block = true;

            foreach (var i in this.items.OfType<MenuBool>())
            {
                i.Value = i.Id == item.Id;
            }

            this.block = false;
        }
    }
}