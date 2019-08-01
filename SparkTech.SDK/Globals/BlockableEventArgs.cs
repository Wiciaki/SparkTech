namespace SparkTech.SDK
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