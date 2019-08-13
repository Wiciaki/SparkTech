namespace SparkTech.SDK.GUI.Menu
{
    using System;
    using System.Drawing;

    using Newtonsoft.Json.Linq;

    using SparkTech.SDK.Game;

    public abstract class MenuItem
    {
        public readonly string Id;

        protected MenuItem(string id)
        {
            this.Id = id;

            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Empty MenuItem id provided!");
            }
        }

        public bool IsVisible { get; set; } = true;

        protected internal virtual void UpdateSize()
        {
            this.Size = this.GetSize();
        }

        public Size Size { get; private set; }

        protected abstract Size GetSize();

        protected internal abstract void OnEndScene(Point point, int groupWidth);

        protected internal virtual void OnWndProc(Point point, int groupWidth, WndProcEventArgs args)
        { }

        protected internal virtual bool ShouldSave() => false;

        protected internal virtual JToken GetToken() => null;

        protected internal virtual void SetToken(JToken token)
        { }

        public T GetValue<T>() => this.MenuValueCast<T>().Value;

        public void SetValue<T>(T value) => this.MenuValueCast<T>().Value = value;

        private IMenuValue<T> MenuValueCast<T>()
        {
            return this as IMenuValue<T> ?? throw new InvalidOperationException($"\"{this.Id}\" doesn't implement IMenuValue<{typeof(T).Name}>!");
        }
    }
}