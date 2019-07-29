namespace SparkTech.SDK.Clipper
{
    internal class OutRec
    {
        #region Fields

        internal OutPt BottomPt;

        internal OutRec FirstLeft; //see comments in clipper.pas

        internal int Idx;

        internal bool IsHole;

        internal bool IsOpen;

        internal PolyNode PolyNode;

        internal OutPt Pts;

        #endregion
    };
}