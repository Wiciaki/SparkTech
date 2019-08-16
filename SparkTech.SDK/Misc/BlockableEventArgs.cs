namespace SparkTech.SDK.Misc
{
    public abstract class BlockableEventArgs
    {
        public bool IsBlocked { get; private set; }

        public void Block()
        {
            this.IsBlocked = true;
        }
    }
}