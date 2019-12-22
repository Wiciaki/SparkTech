namespace Surgical.SDK.GUI.Menu
{
    using System.Collections.Generic;

    public class RadioBind
    {
        private bool block;

        private readonly List<MenuBool> items = new List<MenuBool>();

        public void Add(MenuBool item)
        {
            this.items.Add(item);

            item.BeforeValueChange += _ => this.BeforeValueChange(item);
        }

        private void BeforeValueChange(MenuItem item)
        {
            if (this.block)
            {
                return;
            }

            this.block = true;

            foreach (var i in this.items)
            {
                i.Value = i.Id == item.Id;
            }

            this.block = false;
        }
    }
}