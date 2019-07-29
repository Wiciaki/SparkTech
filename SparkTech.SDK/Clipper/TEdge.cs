namespace SparkTech.SDK.Clipper
{
    internal class TEdge
    {
        #region Fields

        internal IntPoint Bot;

        internal IntPoint Curr; //current (updated for every new scanbeam)

        internal IntPoint Delta;

        internal double Dx;

        internal TEdge Next;

        internal TEdge NextInAEL;

        internal TEdge NextInLML;

        internal TEdge NextInSEL;

        internal int OutIdx;

        internal PolyType PolyTyp;

        internal TEdge Prev;

        internal TEdge PrevInAEL;

        internal TEdge PrevInSEL;

        internal EdgeSide Side; //side only refers to current side of solution poly

        internal IntPoint Top;

        internal int WindCnt;

        internal int WindCnt2; //winding count of the opposite polytype

        internal int WindDelta; //1 or -1 depending on winding direction

        #endregion
    };
}