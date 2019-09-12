namespace Surgical.SDK.EventData
{
    public abstract class BeforeMenuValueChangeEventArgs : BlockableEventArgs
    {
        private BeforeValueChange<T> Cast<T>()
        {
            return (BeforeValueChange<T>)this;
        }

        public bool ValueIs<T>()
        {
            return this is BeforeValueChange<T>;
        }

        public T OldValue<T>()
        {
            return this.Cast<T>().OldT;
        }

        public T NewValue<T>()
        {
            return this.Cast<T>().NewT;
        }

        public static BeforeMenuValueChangeEventArgs Create<T>(T oldValue, T newValue)
        {
            return new BeforeValueChange<T>(oldValue, newValue);
        }

        private class BeforeValueChange<T> : BeforeMenuValueChangeEventArgs
        {
            public readonly T OldT, NewT;

            public BeforeValueChange(T oldValue, T newValue)
            {
                this.OldT = oldValue;

                this.NewT = newValue;
            }
        }
    }
}