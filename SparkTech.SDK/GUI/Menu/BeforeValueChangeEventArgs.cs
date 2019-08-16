namespace SparkTech.SDK.GUI.Menu
{
    using SparkTech.SDK.Misc;

    public class BeforeValueChangeEventArgs : BlockableEventArgs
    {
        private readonly object oldValue, newValue;

        public T GetOldValue<T>()
        {
            return (T)this.oldValue;
        }

        public T GetNewValue<T>()
        {
            return (T)this.newValue;
        }

        public BeforeValueChangeEventArgs(object oldValue, object newValue)
        {
            this.oldValue = oldValue;

            this.newValue = newValue;
        }
    }
}