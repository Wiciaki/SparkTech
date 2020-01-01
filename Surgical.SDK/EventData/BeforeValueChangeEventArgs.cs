namespace Surgical.SDK.EventData
{
    using System;

    public class BeforeValueChangeEventArgs : BlockableEventArgs
    {
        private readonly object oldValue, newValue;

        private BeforeValueChangeEventArgs(object oldValue, object newValue)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        public bool ValueIs<T>()
        {
            return this.oldValue is T && this.newValue is T;
        }

        public T GetOldValue<T>()
        {
            return (T)this.oldValue;
        }

        public T GetNewValue<T>()
        {
            return (T)this.newValue;
        }

        public static BeforeValueChangeEventArgs Create<T>(T oldValue, T newValue)
        {
            if (oldValue == null)
            {
                throw new ArgumentNullException(nameof(oldValue));
            }

            if (newValue == null)
            {
                throw new ArgumentNullException(nameof(newValue));
            }

            return new BeforeValueChangeEventArgs(oldValue, newValue);
        }
    }
}