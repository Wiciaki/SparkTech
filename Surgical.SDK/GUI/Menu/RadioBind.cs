namespace Surgical.SDK.GUI.Menu
{
    using System.Collections;
    using System.Collections.Generic;

    public sealed class RadioBind : IEnumerable, IResumable
    {
        private bool block, working;

        private readonly List<MenuBool> items = new List<MenuBool>();

        public void Add(MenuBool item)
        {
            this.items.Add(item);

            item.BeforeValueChange += _ => this.BeforeValueChange(item);
        }

        private void BeforeValueChange(MenuItem item)
        {
            if (this.block || !this.working)
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

        public void Start()
        {
            this.working = true;
        }

        public void Pause()
        {
            this.working = false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.items.GetEnumerator();
        }
    }
}