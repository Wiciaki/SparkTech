namespace SparkTech.SDK.GUI.Menu
{
    using System;

    public class BeforeValueChangeEventArgs : BlockableEventArgs
    {
        private readonly object oldValue, newValue;

        public readonly Type Type;

        public T OldValue<T>()
        {
            return (T)this.oldValue;
        }

        public T NewValue<T>()
        {
            return (T)this.newValue;
        }

        public bool OfType<T>()
        {
            return typeof(T).IsAssignableFrom(this.Type);
        }

        private BeforeValueChangeEventArgs(Type type, object oldValue, object newValue)
        {
            this.Type = type;

            this.oldValue = oldValue;

            this.newValue = newValue;
        }

        public static BeforeValueChangeEventArgs Create<T>(T oldValue, T newValue)
        {
            return new BeforeValueChangeEventArgs(typeof(T), oldValue, newValue);
        }
    }
}