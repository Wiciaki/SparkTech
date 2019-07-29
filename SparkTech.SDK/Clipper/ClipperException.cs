namespace SparkTech.SDK.Clipper
{
    using System;

    internal class ClipperException : Exception
    {
        #region Constructors and Destructors

        public ClipperException(string description)
            : base(description)
        {
        }

        #endregion
    }
}