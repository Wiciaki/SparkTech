﻿namespace SparkTech.SDK.GUI.Menu
{
    using System;

    using Newtonsoft.Json.Linq;

    using SharpDX;

    using SparkTech.SDK.EventData;

    public abstract class MenuItem
    {
        public string Id { get; }

        public event Action<BeforeValueChangeEventArgs> BeforeValueChange;

        public bool IsVisible { get; set; } = true;

        public Size2 Size { get; private set; }

        private bool save;

        protected MenuItem(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Empty MenuItem Id provided!");
            }

            this.Id = id;
        }

        protected abstract Size2 GetSize();

        protected internal void UpdateSize()
        {
            this.Size = this.GetSize();
        }

        protected internal abstract void OnEndScene(Point point, int width);

        protected internal virtual void OnWndProc(Point point, int width, WndProcEventArgs args)
        { }

        protected internal virtual JToken GetToken() => null;

        protected internal virtual void SetToken(JToken token)
        { }

        protected internal virtual bool ConsumeSaveToken()
        {
            var s = this.save;
            this.save = false;
            return s;
        }

        protected internal virtual void SetTranslations(Translations t)
        { }

        protected internal virtual bool InsideExpandableArea(Point point, int width) => false;

        private IMenuValue<T> MenuValue<T>()
        {
            return this as IMenuValue<T> ?? throw new InvalidOperationException($"\"{this.Id}\" doesn't implement IMenuValue<{typeof(T).Name}>!");
        }

        public T GetValue<T>()
        {
            return this.MenuValue<T>().Value;
        }

        public void SetValue<T>(T value)
        {
            this.MenuValue<T>().Value = value;
        }

        protected bool UpdateValue<T>(T @new, bool notifyOnly = false)
        {
            var t = this.MenuValue<T>();

            if (this.BeforeValueChange != null)
            {
                var args = BeforeValueChangeEventArgs.Create(t.Value, @new);

                this.BeforeValueChange.SafeInvoke(args);

                if (args.IsBlocked)
                {
                    return false;
                }
            }

            if (!notifyOnly)
            {
                this.save = true;
            }

            return true;
        }
    }
}