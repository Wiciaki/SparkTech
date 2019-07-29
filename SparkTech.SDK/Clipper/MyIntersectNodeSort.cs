namespace SparkTech.SDK.Clipper
{
    using System.Collections.Generic;

    public class MyIntersectNodeSort : IComparer<IntersectNode>
    {
        #region Public Methods and Operators

        public int Compare(IntersectNode node1, IntersectNode node2)
        {
            var i = node2.Pt.Y - node1.Pt.Y;
            if (i > 0) return 1;
            else if (i < 0) return -1;
            else return 0;
        }

        #endregion
    }
}