namespace SparkTech.SDK
{
    using System;

    public abstract class BlockableEventArgs : EventArgs
    {
        public bool IsBlocked { get; private set; }

        public void Block()
        {
            this.IsBlocked = true;
        }
    }
}