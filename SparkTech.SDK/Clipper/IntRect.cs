namespace SparkTech.SDK.Clipper
{
    public struct IntRect
    {
        #region Fields

        public long bottom;

        public long left;

        public long right;

        public long top;

        #endregion

        #region Constructors and Destructors

        public IntRect(long l, long t, long r, long b)
        {
            this.left = l;
            this.top = t;
            this.right = r;
            this.bottom = b;
        }

        public IntRect(IntRect ir)
        {
            this.left = ir.left;
            this.top = ir.top;
            this.right = ir.right;
            this.bottom = ir.bottom;
        }

        #endregion
    }
}