namespace SparkTech.SDK.GUI.Menu
{
    using System;

    using SparkTech.SDK.Misc;

    public class BeforeValueChangeEventArgs : BlockableEventArgs
    {
        public readonly Type Type;

        private readonly object oldValue, newValue;

        public T OldValue<T>()
        {
            return (T)this.oldValue;
        }

        public T NewValue<T>()
        {
            return (T)this.newValue;
        }

        public bool ValueIs<T>()
        {
            return this.newValue is T;
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