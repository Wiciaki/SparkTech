namespace SparkTech.SDK
{
    public abstract class BlockableEventArgs
    {
        public bool Process { get; private set; } = true;

        public void Block()
        {
            this.Process = false;
        }
    }
}