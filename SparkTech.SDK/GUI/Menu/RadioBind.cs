namespace SparkTech.SDK.GUI.Menu
{
    using System.Collections;
    using System.Collections.Generic;

    public sealed class RadioBind : IEnumerable
    {
        private readonly List<MenuItem> items = new List<MenuItem>();

        private bool updating;

        public void Add<T>(T item) where T : MenuItem, IMenuValue<bool>
        {
            if (this.items.Exists(m => m.GetValue<bool>()))
            {
                item.SetValue(false);
            }

            if (this.items.Count == 0)
            {
                item.SetValue(true);
            }

            this.items.Add(item);

            item.BeforeValueChange += args =>
            {
                if (this.updating)
                {
                    return;
                }

                if (item.GetValue<bool>())
                {
                    args.Block();
                    return;
                }

                this.updating = true;

                foreach (var i in this.items)
                {
                    i.SetValue(i.Id == item.Id);
                }

                this.updating = false;
            };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.items.GetEnumerator();
        }
    }
}