namespace SparkTech.SDK.EventArgs
{
    public abstract class BeforeValueChangeEventArgs : BlockableEventArgs
    {
        private BeforeValueChange<T> Cast<T>()
        {
            return (BeforeValueChange<T>)this;
        }

        public bool Is<T>()
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

        public static BeforeValueChangeEventArgs Create<T>(T oldValue, T newValue)
        {
            return new BeforeValueChange<T>(oldValue, newValue);
        }

        private class BeforeValueChange<T> : BeforeValueChangeEventArgs
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