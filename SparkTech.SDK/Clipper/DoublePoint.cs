namespace SparkTech.SDK.Clipper
{
    public struct DoublePoint
    {
        #region Fields

        public double X;

        public double Y;

        #endregion

        #region Constructors and Destructors

        public DoublePoint(double x = 0, double y = 0)
        {
            this.X = x;
            this.Y = y;
        }

        public DoublePoint(DoublePoint dp)
        {
            this.X = dp.X;
            this.Y = dp.Y;
        }

        public DoublePoint(IntPoint ip)
        {
            this.X = ip.X;
            this.Y = ip.Y;
        }

        #endregion
    };
}