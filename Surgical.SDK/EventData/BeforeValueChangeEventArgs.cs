namespace Surgical.SDK.EventData
{
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

        public T OldValue<T>()
        {
            return (T)this.oldValue;
        }

        public T NewValue<T>()
        {
            return (T)this.newValue;
        }

        public static BeforeValueChangeEventArgs Create<T>(T oldValue, T newValue)
        {
            return new BeforeValueChangeEventArgs(oldValue, newValue);
        }
    }
}