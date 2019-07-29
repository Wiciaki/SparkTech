#define use_lines

//using System.Text;          //for Int128.AsString() & StringBuilder
//using System.IO;            //debugging with streamReader & StreamWriter
//using System.Windows.Forms; //debugging to clipboard

namespace SparkTech.SDK.Clipper
{
    #region Using Directives

#if use_int32
  using cInt = Int32;
#else
#endif
    using System;
    using System.Collections.Generic;

    using Path = System.Collections.Generic.List<IntPoint>;
    using Paths = System.Collections.Generic.List<System.Collections.Generic.List<IntPoint>>;

    #endregion

    public class Clipper : ClipperBase
    {
        //InitOptions that can be passed to the constructor ...
        public const int ioReverseSolution = 1;

        public const int ioStrictlySimple = 2;

        public const int ioPreserveCollinear = 4;

        private ClipType m_ClipType;

        private Maxima m_Maxima;

        private TEdge m_SortedEdges;

        private readonly List<IntersectNode> m_IntersectList;

        private readonly IComparer<IntersectNode> m_IntersectNodeComparer;

        private bool m_ExecuteLocked;

        private PolyFillType m_ClipFillType;

        private PolyFillType m_SubjFillType;

        private readonly List<Join> m_Joins;

        private readonly List<Join> m_GhostJoins;

        private bool m_UsingPolyTree;
#if use_xyz
      public delegate void ZFillCallback(IntPoint bot1, IntPoint top1,
        IntPoint bot2, IntPoint top2, ref IntPoint pt);
      public ZFillCallback ZFillFunction { get; set; }
#endif
        public Clipper(int InitOptions = 0)
            : base() //constructor
        {
            this.m_Scanbeam = null;
            this.m_Maxima = null;
            this.m_ActiveEdges = null;
            this.m_SortedEdges = null;
            this.m_IntersectList = new List<IntersectNode>();
            this.m_IntersectNodeComparer = new MyIntersectNodeSort();
            this.m_ExecuteLocked = false;
            this.m_UsingPolyTree = false;
            this.m_PolyOuts = new List<OutRec>();
            this.m_Joins = new List<Join>();
            this.m_GhostJoins = new List<Join>();
            this.ReverseSolution = (ioReverseSolution & InitOptions) != 0;
            this.StrictlySimple = (ioStrictlySimple & InitOptions) != 0;
            this.PreserveCollinear = (ioPreserveCollinear & InitOptions) != 0;
#if use_xyz
          ZFillFunction = null;
#endif
        }

        //------------------------------------------------------------------------------

        private void InsertMaxima(long X)
        {
            //double-linked list: sorted ascending, ignoring dups.
            var newMax = new Maxima();
            newMax.X = X;
            if (this.m_Maxima == null)
            {
                this.m_Maxima = newMax;
                this.m_Maxima.Next = null;
                this.m_Maxima.Prev = null;
            }
            else if (X < this.m_Maxima.X)
            {
                newMax.Next = this.m_Maxima;
                newMax.Prev = null;
                this.m_Maxima = newMax;
            }
            else
            {
                var m = this.m_Maxima;
                while (m.Next != null && X >= m.Next.X) m = m.Next;

                if (X == m.X) return; //ie ignores duplicates (& CG to clean up newMax)

                //insert newMax between m and m.Next ...
                newMax.Next = m.Next;
                newMax.Prev = m;
                if (m.Next != null) m.Next.Prev = newMax;
                m.Next = newMax;
            }
        }

        //------------------------------------------------------------------------------

        public bool ReverseSolution { get; set; }

        //------------------------------------------------------------------------------

        public bool StrictlySimple { get; set; }

        //------------------------------------------------------------------------------

        public bool Execute(ClipType clipType, Paths solution, PolyFillType FillType = PolyFillType.pftEvenOdd)
        {
            return this.Execute(clipType, solution, FillType, FillType);
        }

        //------------------------------------------------------------------------------

        public bool Execute(ClipType clipType, PolyTree polytree, PolyFillType FillType = PolyFillType.pftEvenOdd)
        {
            return this.Execute(clipType, polytree, FillType, FillType);
        }

        //------------------------------------------------------------------------------

        public bool Execute(ClipType clipType, Paths solution, PolyFillType subjFillType, PolyFillType clipFillType)
        {
            if (this.m_ExecuteLocked) return false;

            if (this.m_HasOpenPaths)
            {
                throw new ClipperException("Error: PolyTree struct is needed for open path clipping.");
            }

            this.m_ExecuteLocked = true;
            solution.Clear();
            this.m_SubjFillType = subjFillType;
            this.m_ClipFillType = clipFillType;
            this.m_ClipType = clipType;
            this.m_UsingPolyTree = false;
            bool succeeded;
            try
            {
                succeeded = this.ExecuteInternal();

                //build the return polygons ...
                if (succeeded) this.BuildResult(solution);
            }
            finally
            {
                this.DisposeAllPolyPts();
                this.m_ExecuteLocked = false;
            }
            return succeeded;
        }

        //------------------------------------------------------------------------------

        public bool Execute(ClipType clipType, PolyTree polytree, PolyFillType subjFillType, PolyFillType clipFillType)
        {
            if (this.m_ExecuteLocked) return false;

            this.m_ExecuteLocked = true;
            this.m_SubjFillType = subjFillType;
            this.m_ClipFillType = clipFillType;
            this.m_ClipType = clipType;
            this.m_UsingPolyTree = true;
            bool succeeded;
            try
            {
                succeeded = this.ExecuteInternal();

                //build the return polygons ...
                if (succeeded) this.BuildResult2(polytree);
            }
            finally
            {
                this.DisposeAllPolyPts();
                this.m_ExecuteLocked = false;
            }
            return succeeded;
        }

        //------------------------------------------------------------------------------

        internal void FixHoleLinkage(OutRec outRec)
        {
            //skip if an outermost polygon or
            //already already points to the correct FirstLeft ...
            if (outRec.FirstLeft == null
                || outRec.IsHole != outRec.FirstLeft.IsHole && outRec.FirstLeft.Pts != null) return;

            var orfl = outRec.FirstLeft;
            while (orfl != null && (orfl.IsHole == outRec.IsHole || orfl.Pts == null)) orfl = orfl.FirstLeft;

            outRec.FirstLeft = orfl;
        }

        //------------------------------------------------------------------------------

        private bool ExecuteInternal()
        {
            try
            {
                this.Reset();
                this.m_SortedEdges = null;
                this.m_Maxima = null;

                long botY, topY;
                if (!this.PopScanbeam(out botY)) return false;

                this.InsertLocalMinimaIntoAEL(botY);
                while (this.PopScanbeam(out topY) || this.LocalMinimaPending())
                {
                    this.ProcessHorizontals();
                    this.m_GhostJoins.Clear();
                    if (!this.ProcessIntersections(topY)) return false;

                    this.ProcessEdgesAtTopOfScanbeam(topY);
                    botY = topY;
                    this.InsertLocalMinimaIntoAEL(botY);
                }

                //fix orientations ...
                foreach (var outRec in this.m_PolyOuts)
                {
                    if (outRec.Pts == null || outRec.IsOpen) continue;

                    if ((outRec.IsHole ^ this.ReverseSolution) == this.Area(outRec) > 0)
                        this.ReversePolyPtLinks(outRec.Pts);
                }

                this.JoinCommonEdges();

                foreach (var outRec in this.m_PolyOuts)
                {
                    if (outRec.Pts == null) continue;
                    else if (outRec.IsOpen) this.FixupOutPolyline(outRec);
                    else this.FixupOutPolygon(outRec);
                }

                if (this.StrictlySimple) this.DoSimplePolygons();
                return true;
            }

            //catch { return false; }
            finally
            {
                this.m_Joins.Clear();
                this.m_GhostJoins.Clear();
            }
        }

        //------------------------------------------------------------------------------

        private void DisposeAllPolyPts()
        {
            for (var i = 0; i < this.m_PolyOuts.Count; ++i) this.DisposeOutRec(i);

            this.m_PolyOuts.Clear();
        }

        //------------------------------------------------------------------------------

        private void AddJoin(OutPt Op1, OutPt Op2, IntPoint OffPt)
        {
            var j = new Join();
            j.OutPt1 = Op1;
            j.OutPt2 = Op2;
            j.OffPt = OffPt;
            this.m_Joins.Add(j);
        }

        //------------------------------------------------------------------------------

        private void AddGhostJoin(OutPt Op, IntPoint OffPt)
        {
            var j = new Join();
            j.OutPt1 = Op;
            j.OffPt = OffPt;
            this.m_GhostJoins.Add(j);
        }

        //------------------------------------------------------------------------------

#if use_xyz
      internal void SetZ(ref IntPoint pt, TEdge e1, TEdge e2)
      {
        if (pt.Z != 0 || ZFillFunction == null) return;
        else if (pt == e1.Bot) pt.Z = e1.Bot.Z;
        else if (pt == e1.Top) pt.Z = e1.Top.Z;
        else if (pt == e2.Bot) pt.Z = e2.Bot.Z;
        else if (pt == e2.Top) pt.Z = e2.Top.Z;
        else ZFillFunction(e1.Bot, e1.Top, e2.Bot, e2.Top, ref pt);
      }
      //------------------------------------------------------------------------------
#endif

        private void InsertLocalMinimaIntoAEL(long botY)
        {
            LocalMinima lm;
            while (this.PopLocalMinima(botY, out lm))
            {
                var lb = lm.LeftBound;
                var rb = lm.RightBound;

                OutPt Op1 = null;
                if (lb == null)
                {
                    this.InsertEdgeIntoAEL(rb, null);
                    this.SetWindingCount(rb);
                    if (this.IsContributing(rb)) Op1 = this.AddOutPt(rb, rb.Bot);
                }
                else if (rb == null)
                {
                    this.InsertEdgeIntoAEL(lb, null);
                    this.SetWindingCount(lb);
                    if (this.IsContributing(lb)) Op1 = this.AddOutPt(lb, lb.Bot);
                    this.InsertScanbeam(lb.Top.Y);
                }
                else
                {
                    this.InsertEdgeIntoAEL(lb, null);
                    this.InsertEdgeIntoAEL(rb, lb);
                    this.SetWindingCount(lb);
                    rb.WindCnt = lb.WindCnt;
                    rb.WindCnt2 = lb.WindCnt2;
                    if (this.IsContributing(lb)) Op1 = this.AddLocalMinPoly(lb, rb, lb.Bot);
                    this.InsertScanbeam(lb.Top.Y);
                }

                if (rb != null)
                {
                    if (IsHorizontal(rb))
                    {
                        if (rb.NextInLML != null) this.InsertScanbeam(rb.NextInLML.Top.Y);
                        this.AddEdgeToSEL(rb);
                    }
                    else this.InsertScanbeam(rb.Top.Y);
                }

                if (lb == null || rb == null) continue;

                //if output polygons share an Edge with a horizontal rb, they'll need joining later ...
                if (Op1 != null && IsHorizontal(rb) && this.m_GhostJoins.Count > 0 && rb.WindDelta != 0)
                {
                    for (var i = 0; i < this.m_GhostJoins.Count; i++)
                    {
                        //if the horizontal Rb and a 'ghost' horizontal overlap, then convert
                        //the 'ghost' join to a real join ready for later ...
                        var j = this.m_GhostJoins[i];
                        if (this.HorzSegmentsOverlap(j.OutPt1.Pt.X, j.OffPt.X, rb.Bot.X, rb.Top.X))
                            this.AddJoin(j.OutPt1, Op1, j.OffPt);
                    }
                }

                if (lb.OutIdx >= 0 && lb.PrevInAEL != null && lb.PrevInAEL.Curr.X == lb.Bot.X
                    && lb.PrevInAEL.OutIdx >= 0
                    && SlopesEqual(lb.PrevInAEL.Curr, lb.PrevInAEL.Top, lb.Curr, lb.Top, this.m_UseFullRange)
                    && lb.WindDelta != 0 && lb.PrevInAEL.WindDelta != 0)
                {
                    var Op2 = this.AddOutPt(lb.PrevInAEL, lb.Bot);
                    this.AddJoin(Op1, Op2, lb.Top);
                }

                if (lb.NextInAEL != rb)
                {
                    if (rb.OutIdx >= 0 && rb.PrevInAEL.OutIdx >= 0
                        && SlopesEqual(rb.PrevInAEL.Curr, rb.PrevInAEL.Top, rb.Curr, rb.Top, this.m_UseFullRange)
                        && rb.WindDelta != 0 && rb.PrevInAEL.WindDelta != 0)
                    {
                        var Op2 = this.AddOutPt(rb.PrevInAEL, rb.Bot);
                        this.AddJoin(Op1, Op2, rb.Top);
                    }

                    var e = lb.NextInAEL;
                    if (e != null)
                    {
                        while (e != rb)
                        {
                            //nb: For calculating winding counts etc, IntersectEdges() assumes
                            //that param1 will be to the right of param2 ABOVE the intersection ...
                            this.IntersectEdges(rb, e, lb.Curr); //order important here
                            e = e.NextInAEL;
                        }
                    }
                }
            }
        }

        //------------------------------------------------------------------------------

        private void InsertEdgeIntoAEL(TEdge edge, TEdge startEdge)
        {
            if (this.m_ActiveEdges == null)
            {
                edge.PrevInAEL = null;
                edge.NextInAEL = null;
                this.m_ActiveEdges = edge;
            }
            else if (startEdge == null && this.E2InsertsBeforeE1(this.m_ActiveEdges, edge))
            {
                edge.PrevInAEL = null;
                edge.NextInAEL = this.m_ActiveEdges;
                this.m_ActiveEdges.PrevInAEL = edge;
                this.m_ActiveEdges = edge;
            }
            else
            {
                if (startEdge == null) startEdge = this.m_ActiveEdges;
                while (startEdge.NextInAEL != null && !this.E2InsertsBeforeE1(startEdge.NextInAEL, edge))
                    startEdge = startEdge.NextInAEL;

                edge.NextInAEL = startEdge.NextInAEL;
                if (startEdge.NextInAEL != null) startEdge.NextInAEL.PrevInAEL = edge;
                edge.PrevInAEL = startEdge;
                startEdge.NextInAEL = edge;
            }
        }

        //----------------------------------------------------------------------

        private bool E2InsertsBeforeE1(TEdge e1, TEdge e2)
        {
            if (e2.Curr.X == e1.Curr.X)
            {
                if (e2.Top.Y > e1.Top.Y) return e2.Top.X < TopX(e1, e2.Top.Y);
                else return e1.Top.X > TopX(e2, e1.Top.Y);
            }
            else return e2.Curr.X < e1.Curr.X;
        }

        //------------------------------------------------------------------------------

        private bool IsEvenOddFillType(TEdge edge)
        {
            if (edge.PolyTyp == PolyType.ptSubject) return this.m_SubjFillType == PolyFillType.pftEvenOdd;
            else return this.m_ClipFillType == PolyFillType.pftEvenOdd;
        }

        //------------------------------------------------------------------------------

        private bool IsEvenOddAltFillType(TEdge edge)
        {
            if (edge.PolyTyp == PolyType.ptSubject) return this.m_ClipFillType == PolyFillType.pftEvenOdd;
            else return this.m_SubjFillType == PolyFillType.pftEvenOdd;
        }

        //------------------------------------------------------------------------------

        private bool IsContributing(TEdge edge)
        {
            PolyFillType pft, pft2;
            if (edge.PolyTyp == PolyType.ptSubject)
            {
                pft = this.m_SubjFillType;
                pft2 = this.m_ClipFillType;
            }
            else
            {
                pft = this.m_ClipFillType;
                pft2 = this.m_SubjFillType;
            }

            switch (pft)
            {
                case PolyFillType.pftEvenOdd:

                    //return false if a subj line has been flagged as inside a subj polygon
                    if (edge.WindDelta == 0 && edge.WindCnt != 1) return false;

                    break;
                case PolyFillType.pftNonZero:
                    if (Math.Abs(edge.WindCnt) != 1) return false;

                    break;
                case PolyFillType.pftPositive:
                    if (edge.WindCnt != 1) return false;

                    break;
                default: //PolyFillType.pftNegative
                    if (edge.WindCnt != -1) return false;

                    break;
            }

            switch (this.m_ClipType)
            {
                case ClipType.ctIntersection:
                    switch (pft2)
                    {
                        case PolyFillType.pftEvenOdd:
                        case PolyFillType.pftNonZero: return edge.WindCnt2 != 0;
                        case PolyFillType.pftPositive: return edge.WindCnt2 > 0;
                        default: return edge.WindCnt2 < 0;
                    }
                case ClipType.ctUnion:
                    switch (pft2)
                    {
                        case PolyFillType.pftEvenOdd:
                        case PolyFillType.pftNonZero: return edge.WindCnt2 == 0;
                        case PolyFillType.pftPositive: return edge.WindCnt2 <= 0;
                        default: return edge.WindCnt2 >= 0;
                    }
                case ClipType.ctDifference:
                    if (edge.PolyTyp == PolyType.ptSubject)
                    {
                        switch (pft2)
                        {
                            case PolyFillType.pftEvenOdd:
                            case PolyFillType.pftNonZero: return edge.WindCnt2 == 0;
                            case PolyFillType.pftPositive: return edge.WindCnt2 <= 0;
                            default: return edge.WindCnt2 >= 0;
                        }
                    }
                    else
                    {
                        switch (pft2)
                        {
                            case PolyFillType.pftEvenOdd:
                            case PolyFillType.pftNonZero: return edge.WindCnt2 != 0;
                            case PolyFillType.pftPositive: return edge.WindCnt2 > 0;
                            default: return edge.WindCnt2 < 0;
                        }
                    }
                case ClipType.ctXor:
                    if (edge.WindDelta == 0) //XOr always contributing unless open
                    {
                        switch (pft2)
                        {
                            case PolyFillType.pftEvenOdd:
                            case PolyFillType.pftNonZero: return edge.WindCnt2 == 0;
                            case PolyFillType.pftPositive: return edge.WindCnt2 <= 0;
                            default: return edge.WindCnt2 >= 0;
                        }
                    }
                    else return true;
            }

            return true;
        }

        //------------------------------------------------------------------------------

        private void SetWindingCount(TEdge edge)
        {
            var e = edge.PrevInAEL;

            //find the edge of the same polytype that immediately preceeds 'edge' in AEL
            while (e != null && (e.PolyTyp != edge.PolyTyp || e.WindDelta == 0)) e = e.PrevInAEL;

            if (e == null)
            {
                PolyFillType pft;
                pft = edge.PolyTyp == PolyType.ptSubject ? this.m_SubjFillType : this.m_ClipFillType;
                if (edge.WindDelta == 0) edge.WindCnt = pft == PolyFillType.pftNegative ? -1 : 1;
                else edge.WindCnt = edge.WindDelta;
                edge.WindCnt2 = 0;
                e = this.m_ActiveEdges; //ie get ready to calc WindCnt2
            }
            else if (edge.WindDelta == 0 && this.m_ClipType != ClipType.ctUnion)
            {
                edge.WindCnt = 1;
                edge.WindCnt2 = e.WindCnt2;
                e = e.NextInAEL; //ie get ready to calc WindCnt2
            }
            else if (this.IsEvenOddFillType(edge))
            {
                //EvenOdd filling ...
                if (edge.WindDelta == 0)
                {
                    //are we inside a subj polygon ...
                    var Inside = true;
                    var e2 = e.PrevInAEL;
                    while (e2 != null)
                    {
                        if (e2.PolyTyp == e.PolyTyp && e2.WindDelta != 0) Inside = !Inside;
                        e2 = e2.PrevInAEL;
                    }

                    edge.WindCnt = Inside ? 0 : 1;
                }
                else edge.WindCnt = edge.WindDelta;

                edge.WindCnt2 = e.WindCnt2;
                e = e.NextInAEL; //ie get ready to calc WindCnt2
            }
            else
            {
                //nonZero, Positive or Negative filling ...
                if (e.WindCnt * e.WindDelta < 0)
                {
                    //prev edge is 'decreasing' WindCount (WC) toward zero
                    //so we're outside the previous polygon ...
                    if (Math.Abs(e.WindCnt) > 1)
                    {
                        //outside prev poly but still inside another.
                        //when reversing direction of prev poly use the same WC
                        if (e.WindDelta * edge.WindDelta < 0) edge.WindCnt = e.WindCnt;

                        //otherwise continue to 'decrease' WC ...
                        else edge.WindCnt = e.WindCnt + edge.WindDelta;
                    }
                    else

                        //now outside all polys of same polytype so set own WC ...
                        edge.WindCnt = edge.WindDelta == 0 ? 1 : edge.WindDelta;
                }
                else
                {
                    //prev edge is 'increasing' WindCount (WC) away from zero
                    //so we're inside the previous polygon ...
                    if (edge.WindDelta == 0) edge.WindCnt = e.WindCnt < 0 ? e.WindCnt - 1 : e.WindCnt + 1;

                    //if wind direction is reversing prev then use same WC
                    else if (e.WindDelta * edge.WindDelta < 0) edge.WindCnt = e.WindCnt;

                    //otherwise add to WC ...
                    else edge.WindCnt = e.WindCnt + edge.WindDelta;
                }
                edge.WindCnt2 = e.WindCnt2;
                e = e.NextInAEL; //ie get ready to calc WindCnt2
            }

            //update WindCnt2 ...
            if (this.IsEvenOddAltFillType(edge))
            {
                //EvenOdd filling ...
                while (e != edge)
                {
                    if (e.WindDelta != 0) edge.WindCnt2 = edge.WindCnt2 == 0 ? 1 : 0;
                    e = e.NextInAEL;
                }
            }
            else
            {
                //nonZero, Positive or Negative filling ...
                while (e != edge)
                {
                    edge.WindCnt2 += e.WindDelta;
                    e = e.NextInAEL;
                }
            }
        }

        //------------------------------------------------------------------------------

        private void AddEdgeToSEL(TEdge edge)
        {
            //SEL pointers in PEdge are use to build transient lists of horizontal edges.
            //However, since we don't need to worry about processing order, all additions
            //are made to the front of the list ...
            if (this.m_SortedEdges == null)
            {
                this.m_SortedEdges = edge;
                edge.PrevInSEL = null;
                edge.NextInSEL = null;
            }
            else
            {
                edge.NextInSEL = this.m_SortedEdges;
                edge.PrevInSEL = null;
                this.m_SortedEdges.PrevInSEL = edge;
                this.m_SortedEdges = edge;
            }
        }

        //------------------------------------------------------------------------------

        internal bool PopEdgeFromSEL(out TEdge e)
        {
            //Pop edge from front of SEL (ie SEL is a FILO list)
            e = this.m_SortedEdges;
            if (e == null) return false;

            var oldE = e;
            this.m_SortedEdges = e.NextInSEL;
            if (this.m_SortedEdges != null) this.m_SortedEdges.PrevInSEL = null;
            oldE.NextInSEL = null;
            oldE.PrevInSEL = null;
            return true;
        }

        //------------------------------------------------------------------------------

        private void CopyAELToSEL()
        {
            var e = this.m_ActiveEdges;
            this.m_SortedEdges = e;
            while (e != null)
            {
                e.PrevInSEL = e.PrevInAEL;
                e.NextInSEL = e.NextInAEL;
                e = e.NextInAEL;
            }
        }

        //------------------------------------------------------------------------------

        private void SwapPositionsInSEL(TEdge edge1, TEdge edge2)
        {
            if (edge1.NextInSEL == null && edge1.PrevInSEL == null) return;
            if (edge2.NextInSEL == null && edge2.PrevInSEL == null) return;

            if (edge1.NextInSEL == edge2)
            {
                var next = edge2.NextInSEL;
                if (next != null) next.PrevInSEL = edge1;
                var prev = edge1.PrevInSEL;
                if (prev != null) prev.NextInSEL = edge2;
                edge2.PrevInSEL = prev;
                edge2.NextInSEL = edge1;
                edge1.PrevInSEL = edge2;
                edge1.NextInSEL = next;
            }
            else if (edge2.NextInSEL == edge1)
            {
                var next = edge1.NextInSEL;
                if (next != null) next.PrevInSEL = edge2;
                var prev = edge2.PrevInSEL;
                if (prev != null) prev.NextInSEL = edge1;
                edge1.PrevInSEL = prev;
                edge1.NextInSEL = edge2;
                edge2.PrevInSEL = edge1;
                edge2.NextInSEL = next;
            }
            else
            {
                var next = edge1.NextInSEL;
                var prev = edge1.PrevInSEL;
                edge1.NextInSEL = edge2.NextInSEL;
                if (edge1.NextInSEL != null) edge1.NextInSEL.PrevInSEL = edge1;
                edge1.PrevInSEL = edge2.PrevInSEL;
                if (edge1.PrevInSEL != null) edge1.PrevInSEL.NextInSEL = edge1;
                edge2.NextInSEL = next;
                if (edge2.NextInSEL != null) edge2.NextInSEL.PrevInSEL = edge2;
                edge2.PrevInSEL = prev;
                if (edge2.PrevInSEL != null) edge2.PrevInSEL.NextInSEL = edge2;
            }

            if (edge1.PrevInSEL == null) this.m_SortedEdges = edge1;
            else if (edge2.PrevInSEL == null) this.m_SortedEdges = edge2;
        }

        //------------------------------------------------------------------------------

        private void AddLocalMaxPoly(TEdge e1, TEdge e2, IntPoint pt)
        {
            this.AddOutPt(e1, pt);
            if (e2.WindDelta == 0) this.AddOutPt(e2, pt);
            if (e1.OutIdx == e2.OutIdx)
            {
                e1.OutIdx = Unassigned;
                e2.OutIdx = Unassigned;
            }
            else if (e1.OutIdx < e2.OutIdx) this.AppendPolygon(e1, e2);
            else this.AppendPolygon(e2, e1);
        }

        //------------------------------------------------------------------------------

        private OutPt AddLocalMinPoly(TEdge e1, TEdge e2, IntPoint pt)
        {
            OutPt result;
            TEdge e, prevE;
            if (IsHorizontal(e2) || e1.Dx > e2.Dx)
            {
                result = this.AddOutPt(e1, pt);
                e2.OutIdx = e1.OutIdx;
                e1.Side = EdgeSide.esLeft;
                e2.Side = EdgeSide.esRight;
                e = e1;
                if (e.PrevInAEL == e2) prevE = e2.PrevInAEL;
                else prevE = e.PrevInAEL;
            }
            else
            {
                result = this.AddOutPt(e2, pt);
                e1.OutIdx = e2.OutIdx;
                e1.Side = EdgeSide.esRight;
                e2.Side = EdgeSide.esLeft;
                e = e2;
                if (e.PrevInAEL == e1) prevE = e1.PrevInAEL;
                else prevE = e.PrevInAEL;
            }

            if (prevE != null && prevE.OutIdx >= 0 && prevE.Top.Y < pt.Y && e.Top.Y < pt.Y)
            {
                var xPrev = TopX(prevE, pt.Y);
                var xE = TopX(e, pt.Y);
                if (xPrev == xE && e.WindDelta != 0 && prevE.WindDelta != 0 && SlopesEqual(
                        new IntPoint(xPrev, pt.Y),
                        prevE.Top,
                        new IntPoint(xE, pt.Y),
                        e.Top,
                        this.m_UseFullRange))
                {
                    var outPt = this.AddOutPt(prevE, pt);
                    this.AddJoin(result, outPt, e.Top);
                }
            }
            return result;
        }

        //------------------------------------------------------------------------------

        private OutPt AddOutPt(TEdge e, IntPoint pt)
        {
            if (e.OutIdx < 0)
            {
                var outRec = this.CreateOutRec();
                outRec.IsOpen = e.WindDelta == 0;
                var newOp = new OutPt();
                outRec.Pts = newOp;
                newOp.Idx = outRec.Idx;
                newOp.Pt = pt;
                newOp.Next = newOp;
                newOp.Prev = newOp;
                if (!outRec.IsOpen) this.SetHoleState(e, outRec);
                e.OutIdx = outRec.Idx; //nb: do this after SetZ !
                return newOp;
            }
            else
            {
                var outRec = this.m_PolyOuts[e.OutIdx];

                //OutRec.Pts is the 'Left-most' point & OutRec.Pts.Prev is the 'Right-most'
                var op = outRec.Pts;
                var ToFront = e.Side == EdgeSide.esLeft;
                if (ToFront && pt == op.Pt) return op;
                else if (!ToFront && pt == op.Prev.Pt) return op.Prev;

                var newOp = new OutPt();
                newOp.Idx = outRec.Idx;
                newOp.Pt = pt;
                newOp.Next = op;
                newOp.Prev = op.Prev;
                newOp.Prev.Next = newOp;
                op.Prev = newOp;
                if (ToFront) outRec.Pts = newOp;
                return newOp;
            }
        }

        //------------------------------------------------------------------------------

        private OutPt GetLastOutPt(TEdge e)
        {
            var outRec = this.m_PolyOuts[e.OutIdx];
            if (e.Side == EdgeSide.esLeft) return outRec.Pts;
            else return outRec.Pts.Prev;
        }

        //------------------------------------------------------------------------------

        internal void SwapPoints(ref IntPoint pt1, ref IntPoint pt2)
        {
            var tmp = new IntPoint(pt1);
            pt1 = pt2;
            pt2 = tmp;
        }

        //------------------------------------------------------------------------------

        private bool HorzSegmentsOverlap(long seg1a, long seg1b, long seg2a, long seg2b)
        {
            if (seg1a > seg1b) this.Swap(ref seg1a, ref seg1b);
            if (seg2a > seg2b) this.Swap(ref seg2a, ref seg2b);
            return seg1a < seg2b && seg2a < seg1b;
        }

        //------------------------------------------------------------------------------

        private void SetHoleState(TEdge e, OutRec outRec)
        {
            var e2 = e.PrevInAEL;
            TEdge eTmp = null;
            while (e2 != null)
            {
                if (e2.OutIdx >= 0 && e2.WindDelta != 0)
                {
                    if (eTmp == null) eTmp = e2;
                    else if (eTmp.OutIdx == e2.OutIdx) eTmp = null; //paired
                }
                e2 = e2.PrevInAEL;
            }

            if (eTmp == null)
            {
                outRec.FirstLeft = null;
                outRec.IsHole = false;
            }
            else
            {
                outRec.FirstLeft = this.m_PolyOuts[eTmp.OutIdx];
                outRec.IsHole = !outRec.FirstLeft.IsHole;
            }
        }

        //------------------------------------------------------------------------------

        private double GetDx(IntPoint pt1, IntPoint pt2)
        {
            if (pt1.Y == pt2.Y) return horizontal;
            else return (double)(pt2.X - pt1.X) / (pt2.Y - pt1.Y);
        }

        //---------------------------------------------------------------------------

        private bool FirstIsBottomPt(OutPt btmPt1, OutPt btmPt2)
        {
            var p = btmPt1.Prev;
            while (p.Pt == btmPt1.Pt && p != btmPt1) p = p.Prev;

            var dx1p = Math.Abs(this.GetDx(btmPt1.Pt, p.Pt));
            p = btmPt1.Next;
            while (p.Pt == btmPt1.Pt && p != btmPt1) p = p.Next;

            var dx1n = Math.Abs(this.GetDx(btmPt1.Pt, p.Pt));

            p = btmPt2.Prev;
            while (p.Pt == btmPt2.Pt && p != btmPt2) p = p.Prev;

            var dx2p = Math.Abs(this.GetDx(btmPt2.Pt, p.Pt));
            p = btmPt2.Next;
            while (p.Pt == btmPt2.Pt && p != btmPt2) p = p.Next;

            var dx2n = Math.Abs(this.GetDx(btmPt2.Pt, p.Pt));

            if (Math.Max(dx1p, dx1n) == Math.Max(dx2p, dx2n) && Math.Min(dx1p, dx1n) == Math.Min(dx2p, dx2n))
                return this.Area(btmPt1) > 0; //if otherwise identical use orientation
            else return dx1p >= dx2p && dx1p >= dx2n || dx1n >= dx2p && dx1n >= dx2n;
        }

        //------------------------------------------------------------------------------

        private OutPt GetBottomPt(OutPt pp)
        {
            OutPt dups = null;
            var p = pp.Next;
            while (p != pp)
            {
                if (p.Pt.Y > pp.Pt.Y)
                {
                    pp = p;
                    dups = null;
                }
                else if (p.Pt.Y == pp.Pt.Y && p.Pt.X <= pp.Pt.X)
                {
                    if (p.Pt.X < pp.Pt.X)
                    {
                        dups = null;
                        pp = p;
                    }
                    else
                    {
                        if (p.Next != pp && p.Prev != pp) dups = p;
                    }
                }
                p = p.Next;
            }

            if (dups != null)
            {
                //there appears to be at least 2 vertices at bottomPt so ...
                while (dups != p)
                {
                    if (!this.FirstIsBottomPt(p, dups)) pp = dups;
                    dups = dups.Next;
                    while (dups.Pt != pp.Pt) dups = dups.Next;
                }
            }

            return pp;
        }

        //------------------------------------------------------------------------------

        private OutRec GetLowermostRec(OutRec outRec1, OutRec outRec2)
        {
            //work out which polygon fragment has the correct hole state ...
            if (outRec1.BottomPt == null) outRec1.BottomPt = this.GetBottomPt(outRec1.Pts);
            if (outRec2.BottomPt == null) outRec2.BottomPt = this.GetBottomPt(outRec2.Pts);
            var bPt1 = outRec1.BottomPt;
            var bPt2 = outRec2.BottomPt;
            if (bPt1.Pt.Y > bPt2.Pt.Y) return outRec1;
            else if (bPt1.Pt.Y < bPt2.Pt.Y) return outRec2;
            else if (bPt1.Pt.X < bPt2.Pt.X) return outRec1;
            else if (bPt1.Pt.X > bPt2.Pt.X) return outRec2;
            else if (bPt1.Next == bPt1) return outRec2;
            else if (bPt2.Next == bPt2) return outRec1;
            else if (this.FirstIsBottomPt(bPt1, bPt2)) return outRec1;
            else return outRec2;
        }

        //------------------------------------------------------------------------------

        private bool OutRec1RightOfOutRec2(OutRec outRec1, OutRec outRec2)
        {
            do
            {
                outRec1 = outRec1.FirstLeft;
                if (outRec1 == outRec2) return true;
            }
            while (outRec1 != null);

            return false;
        }

        //------------------------------------------------------------------------------

        private OutRec GetOutRec(int idx)
        {
            var outrec = this.m_PolyOuts[idx];
            while (outrec != this.m_PolyOuts[outrec.Idx]) outrec = this.m_PolyOuts[outrec.Idx];

            return outrec;
        }

        //------------------------------------------------------------------------------

        private void AppendPolygon(TEdge e1, TEdge e2)
        {
            var outRec1 = this.m_PolyOuts[e1.OutIdx];
            var outRec2 = this.m_PolyOuts[e2.OutIdx];

            OutRec holeStateRec;
            if (this.OutRec1RightOfOutRec2(outRec1, outRec2)) holeStateRec = outRec2;
            else if (this.OutRec1RightOfOutRec2(outRec2, outRec1)) holeStateRec = outRec1;
            else holeStateRec = this.GetLowermostRec(outRec1, outRec2);

            //get the start and ends of both output polygons and
            //join E2 poly onto E1 poly and delete pointers to E2 ...
            var p1_lft = outRec1.Pts;
            var p1_rt = p1_lft.Prev;
            var p2_lft = outRec2.Pts;
            var p2_rt = p2_lft.Prev;

            //join e2 poly onto e1 poly and delete pointers to e2 ...
            if (e1.Side == EdgeSide.esLeft)
            {
                if (e2.Side == EdgeSide.esLeft)
                {
                    //z y x a b c
                    this.ReversePolyPtLinks(p2_lft);
                    p2_lft.Next = p1_lft;
                    p1_lft.Prev = p2_lft;
                    p1_rt.Next = p2_rt;
                    p2_rt.Prev = p1_rt;
                    outRec1.Pts = p2_rt;
                }
                else
                {
                    //x y z a b c
                    p2_rt.Next = p1_lft;
                    p1_lft.Prev = p2_rt;
                    p2_lft.Prev = p1_rt;
                    p1_rt.Next = p2_lft;
                    outRec1.Pts = p2_lft;
                }
            }
            else
            {
                if (e2.Side == EdgeSide.esRight)
                {
                    //a b c z y x
                    this.ReversePolyPtLinks(p2_lft);
                    p1_rt.Next = p2_rt;
                    p2_rt.Prev = p1_rt;
                    p2_lft.Next = p1_lft;
                    p1_lft.Prev = p2_lft;
                }
                else
                {
                    //a b c x y z
                    p1_rt.Next = p2_lft;
                    p2_lft.Prev = p1_rt;
                    p1_lft.Prev = p2_rt;
                    p2_rt.Next = p1_lft;
                }
            }

            outRec1.BottomPt = null;
            if (holeStateRec == outRec2)
            {
                if (outRec2.FirstLeft != outRec1) outRec1.FirstLeft = outRec2.FirstLeft;
                outRec1.IsHole = outRec2.IsHole;
            }
            outRec2.Pts = null;
            outRec2.BottomPt = null;

            outRec2.FirstLeft = outRec1;

            var OKIdx = e1.OutIdx;
            var ObsoleteIdx = e2.OutIdx;

            e1.OutIdx = Unassigned; //nb: safe because we only get here via AddLocalMaxPoly
            e2.OutIdx = Unassigned;

            var e = this.m_ActiveEdges;
            while (e != null)
            {
                if (e.OutIdx == ObsoleteIdx)
                {
                    e.OutIdx = OKIdx;
                    e.Side = e1.Side;
                    break;
                }

                e = e.NextInAEL;
            }

            outRec2.Idx = outRec1.Idx;
        }

        //------------------------------------------------------------------------------

        private void ReversePolyPtLinks(OutPt pp)
        {
            if (pp == null) return;

            OutPt pp1;
            OutPt pp2;
            pp1 = pp;
            do
            {
                pp2 = pp1.Next;
                pp1.Next = pp1.Prev;
                pp1.Prev = pp2;
                pp1 = pp2;
            }
            while (pp1 != pp);
        }

        //------------------------------------------------------------------------------

        private static void SwapSides(TEdge edge1, TEdge edge2)
        {
            var side = edge1.Side;
            edge1.Side = edge2.Side;
            edge2.Side = side;
        }

        //------------------------------------------------------------------------------

        private static void SwapPolyIndexes(TEdge edge1, TEdge edge2)
        {
            var outIdx = edge1.OutIdx;
            edge1.OutIdx = edge2.OutIdx;
            edge2.OutIdx = outIdx;
        }

        //------------------------------------------------------------------------------

        private void IntersectEdges(TEdge e1, TEdge e2, IntPoint pt)
        {
            //e1 will be to the left of e2 BELOW the intersection. Therefore e1 is before
            //e2 in AEL except when e1 is being inserted at the intersection point ...

            var e1Contributing = e1.OutIdx >= 0;
            var e2Contributing = e2.OutIdx >= 0;

#if use_xyz
          SetZ(ref pt, e1, e2);
#endif

#if use_lines

            //if either edge is on an OPEN path ...
            if (e1.WindDelta == 0 || e2.WindDelta == 0)
            {
                //ignore subject-subject open path intersections UNLESS they
                //are both open paths, AND they are both 'contributing maximas' ...
                if (e1.WindDelta == 0 && e2.WindDelta == 0) return;

                //if intersecting a subj line with a subj poly ...
                else if (e1.PolyTyp == e2.PolyTyp && e1.WindDelta != e2.WindDelta
                         && this.m_ClipType == ClipType.ctUnion)
                {
                    if (e1.WindDelta == 0)
                    {
                        if (e2Contributing)
                        {
                            this.AddOutPt(e1, pt);
                            if (e1Contributing) e1.OutIdx = Unassigned;
                        }
                    }
                    else
                    {
                        if (e1Contributing)
                        {
                            this.AddOutPt(e2, pt);
                            if (e2Contributing) e2.OutIdx = Unassigned;
                        }
                    }
                }
                else if (e1.PolyTyp != e2.PolyTyp)
                {
                    if (e1.WindDelta == 0 && Math.Abs(e2.WindCnt) == 1
                        && (this.m_ClipType != ClipType.ctUnion || e2.WindCnt2 == 0))
                    {
                        this.AddOutPt(e1, pt);
                        if (e1Contributing) e1.OutIdx = Unassigned;
                    }
                    else if (e2.WindDelta == 0 && Math.Abs(e1.WindCnt) == 1
                             && (this.m_ClipType != ClipType.ctUnion || e1.WindCnt2 == 0))
                    {
                        this.AddOutPt(e2, pt);
                        if (e2Contributing) e2.OutIdx = Unassigned;
                    }
                }

                return;
            }
#endif

            //update winding counts...
            //assumes that e1 will be to the Right of e2 ABOVE the intersection
            if (e1.PolyTyp == e2.PolyTyp)
            {
                if (this.IsEvenOddFillType(e1))
                {
                    var oldE1WindCnt = e1.WindCnt;
                    e1.WindCnt = e2.WindCnt;
                    e2.WindCnt = oldE1WindCnt;
                }
                else
                {
                    if (e1.WindCnt + e2.WindDelta == 0) e1.WindCnt = -e1.WindCnt;
                    else e1.WindCnt += e2.WindDelta;
                    if (e2.WindCnt - e1.WindDelta == 0) e2.WindCnt = -e2.WindCnt;
                    else e2.WindCnt -= e1.WindDelta;
                }
            }
            else
            {
                if (!this.IsEvenOddFillType(e2)) e1.WindCnt2 += e2.WindDelta;
                else e1.WindCnt2 = e1.WindCnt2 == 0 ? 1 : 0;
                if (!this.IsEvenOddFillType(e1)) e2.WindCnt2 -= e1.WindDelta;
                else e2.WindCnt2 = e2.WindCnt2 == 0 ? 1 : 0;
            }

            PolyFillType e1FillType, e2FillType, e1FillType2, e2FillType2;
            if (e1.PolyTyp == PolyType.ptSubject)
            {
                e1FillType = this.m_SubjFillType;
                e1FillType2 = this.m_ClipFillType;
            }
            else
            {
                e1FillType = this.m_ClipFillType;
                e1FillType2 = this.m_SubjFillType;
            }
            if (e2.PolyTyp == PolyType.ptSubject)
            {
                e2FillType = this.m_SubjFillType;
                e2FillType2 = this.m_ClipFillType;
            }
            else
            {
                e2FillType = this.m_ClipFillType;
                e2FillType2 = this.m_SubjFillType;
            }

            int e1Wc, e2Wc;
            switch (e1FillType)
            {
                case PolyFillType.pftPositive:
                    e1Wc = e1.WindCnt;
                    break;
                case PolyFillType.pftNegative:
                    e1Wc = -e1.WindCnt;
                    break;
                default:
                    e1Wc = Math.Abs(e1.WindCnt);
                    break;
            }
            switch (e2FillType)
            {
                case PolyFillType.pftPositive:
                    e2Wc = e2.WindCnt;
                    break;
                case PolyFillType.pftNegative:
                    e2Wc = -e2.WindCnt;
                    break;
                default:
                    e2Wc = Math.Abs(e2.WindCnt);
                    break;
            }

            if (e1Contributing && e2Contributing)
            {
                if (e1Wc != 0 && e1Wc != 1 || e2Wc != 0 && e2Wc != 1
                    || e1.PolyTyp != e2.PolyTyp && this.m_ClipType != ClipType.ctXor) this.AddLocalMaxPoly(e1, e2, pt);
                else
                {
                    this.AddOutPt(e1, pt);
                    this.AddOutPt(e2, pt);
                    SwapSides(e1, e2);
                    SwapPolyIndexes(e1, e2);
                }
            }
            else if (e1Contributing)
            {
                if (e2Wc == 0 || e2Wc == 1)
                {
                    this.AddOutPt(e1, pt);
                    SwapSides(e1, e2);
                    SwapPolyIndexes(e1, e2);
                }
            }
            else if (e2Contributing)
            {
                if (e1Wc == 0 || e1Wc == 1)
                {
                    this.AddOutPt(e2, pt);
                    SwapSides(e1, e2);
                    SwapPolyIndexes(e1, e2);
                }
            }
            else if ((e1Wc == 0 || e1Wc == 1) && (e2Wc == 0 || e2Wc == 1))
            {
                //neither edge is currently contributing ...
                long e1Wc2, e2Wc2;
                switch (e1FillType2)
                {
                    case PolyFillType.pftPositive:
                        e1Wc2 = e1.WindCnt2;
                        break;
                    case PolyFillType.pftNegative:
                        e1Wc2 = -e1.WindCnt2;
                        break;
                    default:
                        e1Wc2 = Math.Abs(e1.WindCnt2);
                        break;
                }
                switch (e2FillType2)
                {
                    case PolyFillType.pftPositive:
                        e2Wc2 = e2.WindCnt2;
                        break;
                    case PolyFillType.pftNegative:
                        e2Wc2 = -e2.WindCnt2;
                        break;
                    default:
                        e2Wc2 = Math.Abs(e2.WindCnt2);
                        break;
                }

                if (e1.PolyTyp != e2.PolyTyp) this.AddLocalMinPoly(e1, e2, pt);
                else if (e1Wc == 1 && e2Wc == 1)
                {
                    switch (this.m_ClipType)
                    {
                        case ClipType.ctIntersection:
                            if (e1Wc2 > 0 && e2Wc2 > 0) this.AddLocalMinPoly(e1, e2, pt);
                            break;
                        case ClipType.ctUnion:
                            if (e1Wc2 <= 0 && e2Wc2 <= 0) this.AddLocalMinPoly(e1, e2, pt);
                            break;
                        case ClipType.ctDifference:
                            if (e1.PolyTyp == PolyType.ptClip && e1Wc2 > 0 && e2Wc2 > 0
                                || e1.PolyTyp == PolyType.ptSubject && e1Wc2 <= 0 && e2Wc2 <= 0)
                                this.AddLocalMinPoly(e1, e2, pt);
                            break;
                        case ClipType.ctXor:
                            this.AddLocalMinPoly(e1, e2, pt);
                            break;
                    }
                }
                else SwapSides(e1, e2);
            }
        }

        //------------------------------------------------------------------------------

        private void DeleteFromSEL(TEdge e)
        {
            var SelPrev = e.PrevInSEL;
            var SelNext = e.NextInSEL;
            if (SelPrev == null && SelNext == null && e != this.m_SortedEdges) return; //already deleted

            if (SelPrev != null) SelPrev.NextInSEL = SelNext;
            else this.m_SortedEdges = SelNext;
            if (SelNext != null) SelNext.PrevInSEL = SelPrev;
            e.NextInSEL = null;
            e.PrevInSEL = null;
        }

        //------------------------------------------------------------------------------

        private void ProcessHorizontals()
        {
            TEdge horzEdge; //m_SortedEdges;
            while (this.PopEdgeFromSEL(out horzEdge)) this.ProcessHorizontal(horzEdge);
        }

        //------------------------------------------------------------------------------

        private void GetHorzDirection(TEdge HorzEdge, out Direction Dir, out long Left, out long Right)
        {
            if (HorzEdge.Bot.X < HorzEdge.Top.X)
            {
                Left = HorzEdge.Bot.X;
                Right = HorzEdge.Top.X;
                Dir = Direction.dLeftToRight;
            }
            else
            {
                Left = HorzEdge.Top.X;
                Right = HorzEdge.Bot.X;
                Dir = Direction.dRightToLeft;
            }
        }

        //------------------------------------------------------------------------

        private void ProcessHorizontal(TEdge horzEdge)
        {
            Direction dir;
            long horzLeft, horzRight;
            var IsOpen = horzEdge.WindDelta == 0;

            this.GetHorzDirection(horzEdge, out dir, out horzLeft, out horzRight);

            TEdge eLastHorz = horzEdge, eMaxPair = null;
            while (eLastHorz.NextInLML != null && IsHorizontal(eLastHorz.NextInLML)) eLastHorz = eLastHorz.NextInLML;

            if (eLastHorz.NextInLML == null) eMaxPair = this.GetMaximaPair(eLastHorz);

            var currMax = this.m_Maxima;
            if (currMax != null)
            {
                //get the first maxima in range (X) ...
                if (dir == Direction.dLeftToRight)
                {
                    while (currMax != null && currMax.X <= horzEdge.Bot.X) currMax = currMax.Next;

                    if (currMax != null && currMax.X >= eLastHorz.Top.X) currMax = null;
                }
                else
                {
                    while (currMax.Next != null && currMax.Next.X < horzEdge.Bot.X) currMax = currMax.Next;

                    if (currMax.X <= eLastHorz.Top.X) currMax = null;
                }
            }

            OutPt op1 = null;
            for (;;) //loop through consec. horizontal edges
            {
                var IsLastHorz = horzEdge == eLastHorz;
                var e = this.GetNextInAEL(horzEdge, dir);
                while (e != null)
                {
                    //this code block inserts extra coords into horizontal edges (in output
                    //polygons) whereever maxima touch these horizontal edges. This helps
                    //'simplifying' polygons (ie if the Simplify property is set).
                    if (currMax != null)
                    {
                        if (dir == Direction.dLeftToRight)
                        {
                            while (currMax != null && currMax.X < e.Curr.X)
                            {
                                if (horzEdge.OutIdx >= 0 && !IsOpen)
                                    this.AddOutPt(horzEdge, new IntPoint(currMax.X, horzEdge.Bot.Y));
                                currMax = currMax.Next;
                            }
                        }
                        else
                        {
                            while (currMax != null && currMax.X > e.Curr.X)
                            {
                                if (horzEdge.OutIdx >= 0 && !IsOpen)
                                    this.AddOutPt(horzEdge, new IntPoint(currMax.X, horzEdge.Bot.Y));
                                currMax = currMax.Prev;
                            }
                        }
                    }

                    ;

                    if (dir == Direction.dLeftToRight && e.Curr.X > horzRight
                        || dir == Direction.dRightToLeft && e.Curr.X < horzLeft) break;

                    //Also break if we've got to the end of an intermediate horizontal edge ...
                    //nb: Smaller Dx's are to the right of larger Dx's ABOVE the horizontal.
                    if (e.Curr.X == horzEdge.Top.X && horzEdge.NextInLML != null && e.Dx < horzEdge.NextInLML.Dx) break;

                    if (horzEdge.OutIdx >= 0 && !IsOpen) //note: may be done multiple times
                    {
#if use_xyz
                  if (dir == Direction.dLeftToRight) SetZ(ref e.Curr, horzEdge, e);
                  else SetZ(ref e.Curr, e, horzEdge);
#endif

                        op1 = this.AddOutPt(horzEdge, e.Curr);
                        var eNextHorz = this.m_SortedEdges;
                        while (eNextHorz != null)
                        {
                            if (eNextHorz.OutIdx >= 0 && this.HorzSegmentsOverlap(
                                    horzEdge.Bot.X,
                                    horzEdge.Top.X,
                                    eNextHorz.Bot.X,
                                    eNextHorz.Top.X))
                            {
                                var op2 = this.GetLastOutPt(eNextHorz);
                                this.AddJoin(op2, op1, eNextHorz.Top);
                            }
                            eNextHorz = eNextHorz.NextInSEL;
                        }

                        this.AddGhostJoin(op1, horzEdge.Bot);
                    }

                    //OK, so far we're still in range of the horizontal Edge  but make sure
                    //we're at the last of consec. horizontals when matching with eMaxPair
                    if (e == eMaxPair && IsLastHorz)
                    {
                        if (horzEdge.OutIdx >= 0) this.AddLocalMaxPoly(horzEdge, eMaxPair, horzEdge.Top);
                        this.DeleteFromAEL(horzEdge);
                        this.DeleteFromAEL(eMaxPair);
                        return;
                    }

                    if (dir == Direction.dLeftToRight)
                    {
                        var Pt = new IntPoint(e.Curr.X, horzEdge.Curr.Y);
                        this.IntersectEdges(horzEdge, e, Pt);
                    }
                    else
                    {
                        var Pt = new IntPoint(e.Curr.X, horzEdge.Curr.Y);
                        this.IntersectEdges(e, horzEdge, Pt);
                    }
                    var eNext = this.GetNextInAEL(e, dir);
                    this.SwapPositionsInAEL(horzEdge, e);
                    e = eNext;
                } //end while(e != null)

                //Break out of loop if HorzEdge.NextInLML is not also horizontal ...
                if (horzEdge.NextInLML == null || !IsHorizontal(horzEdge.NextInLML)) break;

                this.UpdateEdgeIntoAEL(ref horzEdge);
                if (horzEdge.OutIdx >= 0) this.AddOutPt(horzEdge, horzEdge.Bot);
                this.GetHorzDirection(horzEdge, out dir, out horzLeft, out horzRight);
            } //end for (;;)

            if (horzEdge.OutIdx >= 0 && op1 == null)
            {
                op1 = this.GetLastOutPt(horzEdge);
                var eNextHorz = this.m_SortedEdges;
                while (eNextHorz != null)
                {
                    if (eNextHorz.OutIdx >= 0 && this.HorzSegmentsOverlap(
                            horzEdge.Bot.X,
                            horzEdge.Top.X,
                            eNextHorz.Bot.X,
                            eNextHorz.Top.X))
                    {
                        var op2 = this.GetLastOutPt(eNextHorz);
                        this.AddJoin(op2, op1, eNextHorz.Top);
                    }
                    eNextHorz = eNextHorz.NextInSEL;
                }

                this.AddGhostJoin(op1, horzEdge.Top);
            }

            if (horzEdge.NextInLML != null)
            {
                if (horzEdge.OutIdx >= 0)
                {
                    op1 = this.AddOutPt(horzEdge, horzEdge.Top);

                    this.UpdateEdgeIntoAEL(ref horzEdge);
                    if (horzEdge.WindDelta == 0) return;

                    //nb: HorzEdge is no longer horizontal here
                    var ePrev = horzEdge.PrevInAEL;
                    var eNext = horzEdge.NextInAEL;
                    if (ePrev != null && ePrev.Curr.X == horzEdge.Bot.X && ePrev.Curr.Y == horzEdge.Bot.Y
                        && ePrev.WindDelta != 0 && ePrev.OutIdx >= 0 && ePrev.Curr.Y > ePrev.Top.Y
                        && SlopesEqual(horzEdge, ePrev, this.m_UseFullRange))
                    {
                        var op2 = this.AddOutPt(ePrev, horzEdge.Bot);
                        this.AddJoin(op1, op2, horzEdge.Top);
                    }
                    else if (eNext != null && eNext.Curr.X == horzEdge.Bot.X && eNext.Curr.Y == horzEdge.Bot.Y
                             && eNext.WindDelta != 0 && eNext.OutIdx >= 0 && eNext.Curr.Y > eNext.Top.Y
                             && SlopesEqual(horzEdge, eNext, this.m_UseFullRange))
                    {
                        var op2 = this.AddOutPt(eNext, horzEdge.Bot);
                        this.AddJoin(op1, op2, horzEdge.Top);
                    }
                }
                else this.UpdateEdgeIntoAEL(ref horzEdge);
            }
            else
            {
                if (horzEdge.OutIdx >= 0) this.AddOutPt(horzEdge, horzEdge.Top);
                this.DeleteFromAEL(horzEdge);
            }
        }

        //------------------------------------------------------------------------------

        private TEdge GetNextInAEL(TEdge e, Direction Direction)
        {
            return Direction == Direction.dLeftToRight ? e.NextInAEL : e.PrevInAEL;
        }

        //------------------------------------------------------------------------------

        private bool IsMinima(TEdge e)
        {
            return e != null && e.Prev.NextInLML != e && e.Next.NextInLML != e;
        }

        //------------------------------------------------------------------------------

        private bool IsMaxima(TEdge e, double Y)
        {
            return e != null && e.Top.Y == Y && e.NextInLML == null;
        }

        //------------------------------------------------------------------------------

        private bool IsIntermediate(TEdge e, double Y)
        {
            return e.Top.Y == Y && e.NextInLML != null;
        }

        //------------------------------------------------------------------------------

        internal TEdge GetMaximaPair(TEdge e)
        {
            if (e.Next.Top == e.Top && e.Next.NextInLML == null) return e.Next;
            else if (e.Prev.Top == e.Top && e.Prev.NextInLML == null) return e.Prev;
            else return null;
        }

        //------------------------------------------------------------------------------

        internal TEdge GetMaximaPairEx(TEdge e)
        {
            //as above but returns null if MaxPair isn't in AEL (unless it's horizontal)
            var result = this.GetMaximaPair(e);
            if (result == null || result.OutIdx == Skip
                || result.NextInAEL == result.PrevInAEL && !IsHorizontal(result)) return null;

            return result;
        }

        //------------------------------------------------------------------------------

        private bool ProcessIntersections(long topY)
        {
            if (this.m_ActiveEdges == null) return true;

            try
            {
                this.BuildIntersectList(topY);
                if (this.m_IntersectList.Count == 0) return true;
                if (this.m_IntersectList.Count == 1 || this.FixupIntersectionOrder()) this.ProcessIntersectList();
                else return false;
            }
            catch
            {
                this.m_SortedEdges = null;
                this.m_IntersectList.Clear();
                throw new ClipperException("ProcessIntersections error");
            }

            this.m_SortedEdges = null;
            return true;
        }

        //------------------------------------------------------------------------------

        private void BuildIntersectList(long topY)
        {
            if (this.m_ActiveEdges == null) return;

            //prepare for sorting ...
            var e = this.m_ActiveEdges;
            this.m_SortedEdges = e;
            while (e != null)
            {
                e.PrevInSEL = e.PrevInAEL;
                e.NextInSEL = e.NextInAEL;
                e.Curr.X = TopX(e, topY);
                e = e.NextInAEL;
            }

            //bubblesort ...
            var isModified = true;
            while (isModified && this.m_SortedEdges != null)
            {
                isModified = false;
                e = this.m_SortedEdges;
                while (e.NextInSEL != null)
                {
                    var eNext = e.NextInSEL;
                    IntPoint pt;
                    if (e.Curr.X > eNext.Curr.X)
                    {
                        this.IntersectPoint(e, eNext, out pt);
                        if (pt.Y < topY) pt = new IntPoint(TopX(e, topY), topY);
                        var newNode = new IntersectNode();
                        newNode.Edge1 = e;
                        newNode.Edge2 = eNext;
                        newNode.Pt = pt;
                        this.m_IntersectList.Add(newNode);

                        this.SwapPositionsInSEL(e, eNext);
                        isModified = true;
                    }
                    else e = eNext;
                }

                if (e.PrevInSEL != null) e.PrevInSEL.NextInSEL = null;
                else break;
            }

            this.m_SortedEdges = null;
        }

        //------------------------------------------------------------------------------

        private bool EdgesAdjacent(IntersectNode inode)
        {
            return inode.Edge1.NextInSEL == inode.Edge2 || inode.Edge1.PrevInSEL == inode.Edge2;
        }

        //------------------------------------------------------------------------------

        private static int IntersectNodeSort(IntersectNode node1, IntersectNode node2)
        {
            //the following typecast is safe because the differences in Pt.Y will
            //be limited to the height of the scanbeam.
            return (int)(node2.Pt.Y - node1.Pt.Y);
        }

        //------------------------------------------------------------------------------

        private bool FixupIntersectionOrder()
        {
            //pre-condition: intersections are sorted bottom-most first.
            //Now it's crucial that intersections are made only between adjacent edges,
            //so to ensure this the order of intersections may need adjusting ...
            this.m_IntersectList.Sort(this.m_IntersectNodeComparer);

            this.CopyAELToSEL();
            var cnt = this.m_IntersectList.Count;
            for (var i = 0; i < cnt; i++)
            {
                if (!this.EdgesAdjacent(this.m_IntersectList[i]))
                {
                    var j = i + 1;
                    while (j < cnt && !this.EdgesAdjacent(this.m_IntersectList[j])) j++;

                    if (j == cnt) return false;

                    var tmp = this.m_IntersectList[i];
                    this.m_IntersectList[i] = this.m_IntersectList[j];
                    this.m_IntersectList[j] = tmp;
                }

                this.SwapPositionsInSEL(this.m_IntersectList[i].Edge1, this.m_IntersectList[i].Edge2);
            }

            return true;
        }

        //------------------------------------------------------------------------------

        private void ProcessIntersectList()
        {
            for (var i = 0; i < this.m_IntersectList.Count; i++)
            {
                var iNode = this.m_IntersectList[i];
                {
                    this.IntersectEdges(iNode.Edge1, iNode.Edge2, iNode.Pt);
                    this.SwapPositionsInAEL(iNode.Edge1, iNode.Edge2);
                }
            }

            this.m_IntersectList.Clear();
        }

        //------------------------------------------------------------------------------

        internal static long Round(double value)
        {
            return value < 0 ? (long)(value - 0.5) : (long)(value + 0.5);
        }

        //------------------------------------------------------------------------------

        private static long TopX(TEdge edge, long currentY)
        {
            if (currentY == edge.Top.Y) return edge.Top.X;

            return edge.Bot.X + Round(edge.Dx * (currentY - edge.Bot.Y));
        }

        //------------------------------------------------------------------------------

        private void IntersectPoint(TEdge edge1, TEdge edge2, out IntPoint ip)
        {
            ip = new IntPoint();
            double b1, b2;

            //nb: with very large coordinate values, it's possible for SlopesEqual() to
            //return false but for the edge.Dx value be equal due to double precision rounding.
            if (edge1.Dx == edge2.Dx)
            {
                ip.Y = edge1.Curr.Y;
                ip.X = TopX(edge1, ip.Y);
                return;
            }

            if (edge1.Delta.X == 0)
            {
                ip.X = edge1.Bot.X;
                if (IsHorizontal(edge2)) ip.Y = edge2.Bot.Y;
                else
                {
                    b2 = edge2.Bot.Y - edge2.Bot.X / edge2.Dx;
                    ip.Y = Round(ip.X / edge2.Dx + b2);
                }
            }
            else if (edge2.Delta.X == 0)
            {
                ip.X = edge2.Bot.X;
                if (IsHorizontal(edge1)) ip.Y = edge1.Bot.Y;
                else
                {
                    b1 = edge1.Bot.Y - edge1.Bot.X / edge1.Dx;
                    ip.Y = Round(ip.X / edge1.Dx + b1);
                }
            }
            else
            {
                b1 = edge1.Bot.X - edge1.Bot.Y * edge1.Dx;
                b2 = edge2.Bot.X - edge2.Bot.Y * edge2.Dx;
                var q = (b2 - b1) / (edge1.Dx - edge2.Dx);
                ip.Y = Round(q);
                if (Math.Abs(edge1.Dx) < Math.Abs(edge2.Dx)) ip.X = Round(edge1.Dx * q + b1);
                else ip.X = Round(edge2.Dx * q + b2);
            }

            if (ip.Y < edge1.Top.Y || ip.Y < edge2.Top.Y)
            {
                if (edge1.Top.Y > edge2.Top.Y) ip.Y = edge1.Top.Y;
                else ip.Y = edge2.Top.Y;
                if (Math.Abs(edge1.Dx) < Math.Abs(edge2.Dx)) ip.X = TopX(edge1, ip.Y);
                else ip.X = TopX(edge2, ip.Y);
            }

            //finally, don't allow 'ip' to be BELOW curr.Y (ie bottom of scanbeam) ...
            if (ip.Y > edge1.Curr.Y)
            {
                ip.Y = edge1.Curr.Y;

                //better to use the more vertical edge to derive X ...
                if (Math.Abs(edge1.Dx) > Math.Abs(edge2.Dx)) ip.X = TopX(edge2, ip.Y);
                else ip.X = TopX(edge1, ip.Y);
            }
        }

        //------------------------------------------------------------------------------

        private void ProcessEdgesAtTopOfScanbeam(long topY)
        {
            var e = this.m_ActiveEdges;
            while (e != null)
            {
                //1. process maxima, treating them as if they're 'bent' horizontal edges,
                //   but exclude maxima with horizontal edges. nb: e can't be a horizontal.
                var IsMaximaEdge = this.IsMaxima(e, topY);

                if (IsMaximaEdge)
                {
                    var eMaxPair = this.GetMaximaPairEx(e);
                    IsMaximaEdge = eMaxPair == null || !IsHorizontal(eMaxPair);
                }

                if (IsMaximaEdge)
                {
                    if (this.StrictlySimple) this.InsertMaxima(e.Top.X);
                    var ePrev = e.PrevInAEL;
                    this.DoMaxima(e);
                    if (ePrev == null) e = this.m_ActiveEdges;
                    else e = ePrev.NextInAEL;
                }
                else
                {
                    //2. promote horizontal edges, otherwise update Curr.X and Curr.Y ...
                    if (this.IsIntermediate(e, topY) && IsHorizontal(e.NextInLML))
                    {
                        this.UpdateEdgeIntoAEL(ref e);
                        if (e.OutIdx >= 0) this.AddOutPt(e, e.Bot);
                        this.AddEdgeToSEL(e);
                    }
                    else
                    {
                        e.Curr.X = TopX(e, topY);
                        e.Curr.Y = topY;
#if use_xyz
              if (e.Top.Y == topY) e.Curr.Z = e.Top.Z;
              else if (e.Bot.Y == topY) e.Curr.Z = e.Bot.Z;
              else e.Curr.Z = 0;
#endif
                    }

                    //When StrictlySimple and 'e' is being touched by another edge, then
                    //make sure both edges have a vertex here ...
                    if (this.StrictlySimple)
                    {
                        var ePrev = e.PrevInAEL;
                        if (e.OutIdx >= 0 && e.WindDelta != 0 && ePrev != null && ePrev.OutIdx >= 0
                            && ePrev.Curr.X == e.Curr.X && ePrev.WindDelta != 0)
                        {
                            var ip = new IntPoint(e.Curr);
#if use_xyz
                SetZ(ref ip, ePrev, e);
#endif
                            var op = this.AddOutPt(ePrev, ip);
                            var op2 = this.AddOutPt(e, ip);
                            this.AddJoin(op, op2, ip); //StrictlySimple (type-3) join
                        }
                    }

                    e = e.NextInAEL;
                }
            }

            //3. Process horizontals at the Top of the scanbeam ...
            this.ProcessHorizontals();
            this.m_Maxima = null;

            //4. Promote intermediate vertices ...
            e = this.m_ActiveEdges;
            while (e != null)
            {
                if (this.IsIntermediate(e, topY))
                {
                    OutPt op = null;
                    if (e.OutIdx >= 0) op = this.AddOutPt(e, e.Top);
                    this.UpdateEdgeIntoAEL(ref e);

                    //if output polygons share an edge, they'll need joining later ...
                    var ePrev = e.PrevInAEL;
                    var eNext = e.NextInAEL;
                    if (ePrev != null && ePrev.Curr.X == e.Bot.X && ePrev.Curr.Y == e.Bot.Y && op != null
                        && ePrev.OutIdx >= 0 && ePrev.Curr.Y > ePrev.Top.Y
                        && SlopesEqual(e.Curr, e.Top, ePrev.Curr, ePrev.Top, this.m_UseFullRange) && e.WindDelta != 0
                        && ePrev.WindDelta != 0)
                    {
                        var op2 = this.AddOutPt(ePrev, e.Bot);
                        this.AddJoin(op, op2, e.Top);
                    }
                    else if (eNext != null && eNext.Curr.X == e.Bot.X && eNext.Curr.Y == e.Bot.Y && op != null
                             && eNext.OutIdx >= 0 && eNext.Curr.Y > eNext.Top.Y
                             && SlopesEqual(e.Curr, e.Top, eNext.Curr, eNext.Top, this.m_UseFullRange)
                             && e.WindDelta != 0 && eNext.WindDelta != 0)
                    {
                        var op2 = this.AddOutPt(eNext, e.Bot);
                        this.AddJoin(op, op2, e.Top);
                    }
                }
                e = e.NextInAEL;
            }
        }

        //------------------------------------------------------------------------------

        private void DoMaxima(TEdge e)
        {
            var eMaxPair = this.GetMaximaPairEx(e);
            if (eMaxPair == null)
            {
                if (e.OutIdx >= 0) this.AddOutPt(e, e.Top);
                this.DeleteFromAEL(e);
                return;
            }

            var eNext = e.NextInAEL;
            while (eNext != null && eNext != eMaxPair)
            {
                this.IntersectEdges(e, eNext, e.Top);
                this.SwapPositionsInAEL(e, eNext);
                eNext = e.NextInAEL;
            }

            if (e.OutIdx == Unassigned && eMaxPair.OutIdx == Unassigned)
            {
                this.DeleteFromAEL(e);
                this.DeleteFromAEL(eMaxPair);
            }
            else if (e.OutIdx >= 0 && eMaxPair.OutIdx >= 0)
            {
                if (e.OutIdx >= 0) this.AddLocalMaxPoly(e, eMaxPair, e.Top);
                this.DeleteFromAEL(e);
                this.DeleteFromAEL(eMaxPair);
            }
#if use_lines
            else if (e.WindDelta == 0)
            {
                if (e.OutIdx >= 0)
                {
                    this.AddOutPt(e, e.Top);
                    e.OutIdx = Unassigned;
                }
                this.DeleteFromAEL(e);

                if (eMaxPair.OutIdx >= 0)
                {
                    this.AddOutPt(eMaxPair, e.Top);
                    eMaxPair.OutIdx = Unassigned;
                }
                this.DeleteFromAEL(eMaxPair);
            }
#endif
            else throw new ClipperException("DoMaxima error");
        }

        //------------------------------------------------------------------------------

        public static void ReversePaths(Paths polys)
        {
            foreach (var poly in polys)
            {
                poly.Reverse();
            }
        }

        //------------------------------------------------------------------------------

        public static bool Orientation(Path poly)
        {
            return Area(poly) >= 0;
        }

        //------------------------------------------------------------------------------

        private int PointCount(OutPt pts)
        {
            if (pts == null) return 0;

            var result = 0;
            var p = pts;
            do
            {
                result++;
                p = p.Next;
            }
            while (p != pts);

            return result;
        }

        //------------------------------------------------------------------------------

        private void BuildResult(Paths polyg)
        {
            polyg.Clear();
            polyg.Capacity = this.m_PolyOuts.Count;
            for (var i = 0; i < this.m_PolyOuts.Count; i++)
            {
                var outRec = this.m_PolyOuts[i];
                if (outRec.Pts == null) continue;

                var p = outRec.Pts.Prev;
                var cnt = this.PointCount(p);
                if (cnt < 2) continue;

                var pg = new Path(cnt);
                for (var j = 0; j < cnt; j++)
                {
                    pg.Add(p.Pt);
                    p = p.Prev;
                }

                polyg.Add(pg);
            }
        }

        //------------------------------------------------------------------------------

        private void BuildResult2(PolyTree polytree)
        {
            polytree.Clear();

            //add each output polygon/contour to polytree ...
            polytree.m_AllPolys.Capacity = this.m_PolyOuts.Count;
            for (var i = 0; i < this.m_PolyOuts.Count; i++)
            {
                var outRec = this.m_PolyOuts[i];
                var cnt = this.PointCount(outRec.Pts);
                if (outRec.IsOpen && cnt < 2 || !outRec.IsOpen && cnt < 3) continue;

                this.FixHoleLinkage(outRec);
                var pn = new PolyNode();
                polytree.m_AllPolys.Add(pn);
                outRec.PolyNode = pn;
                pn.m_polygon.Capacity = cnt;
                var op = outRec.Pts.Prev;
                for (var j = 0; j < cnt; j++)
                {
                    pn.m_polygon.Add(op.Pt);
                    op = op.Prev;
                }
            }

            //fixup PolyNode links etc ...
            polytree.m_Childs.Capacity = this.m_PolyOuts.Count;
            for (var i = 0; i < this.m_PolyOuts.Count; i++)
            {
                var outRec = this.m_PolyOuts[i];
                if (outRec.PolyNode == null) continue;
                else if (outRec.IsOpen)
                {
                    outRec.PolyNode.IsOpen = true;
                    polytree.AddChild(outRec.PolyNode);
                }
                else if (outRec.FirstLeft != null && outRec.FirstLeft.PolyNode != null)
                    outRec.FirstLeft.PolyNode.AddChild(outRec.PolyNode);
                else polytree.AddChild(outRec.PolyNode);
            }
        }

        //------------------------------------------------------------------------------

        private void FixupOutPolyline(OutRec outrec)
        {
            var pp = outrec.Pts;
            var lastPP = pp.Prev;
            while (pp != lastPP)
            {
                pp = pp.Next;
                if (pp.Pt == pp.Prev.Pt)
                {
                    if (pp == lastPP) lastPP = pp.Prev;
                    var tmpPP = pp.Prev;
                    tmpPP.Next = pp.Next;
                    pp.Next.Prev = tmpPP;
                    pp = tmpPP;
                }
            }

            if (pp == pp.Prev) outrec.Pts = null;
        }

        //------------------------------------------------------------------------------

        private void FixupOutPolygon(OutRec outRec)
        {
            //FixupOutPolygon() - removes duplicate points and simplifies consecutive
            //parallel edges by removing the middle vertex.
            OutPt lastOK = null;
            outRec.BottomPt = null;
            var pp = outRec.Pts;
            var preserveCol = this.PreserveCollinear || this.StrictlySimple;
            for (;;)
            {
                if (pp.Prev == pp || pp.Prev == pp.Next)
                {
                    outRec.Pts = null;
                    return;
                }

                //test for duplicate points and collinear edges ...
                if (pp.Pt == pp.Next.Pt || pp.Pt == pp.Prev.Pt
                    || SlopesEqual(pp.Prev.Pt, pp.Pt, pp.Next.Pt, this.m_UseFullRange)
                    && (!preserveCol || !this.Pt2IsBetweenPt1AndPt3(pp.Prev.Pt, pp.Pt, pp.Next.Pt)))
                {
                    lastOK = null;
                    pp.Prev.Next = pp.Next;
                    pp.Next.Prev = pp.Prev;
                    pp = pp.Prev;
                }
                else if (pp == lastOK) break;
                else
                {
                    if (lastOK == null) lastOK = pp;
                    pp = pp.Next;
                }
            }

            outRec.Pts = pp;
        }

        //------------------------------------------------------------------------------

        private OutPt DupOutPt(OutPt outPt, bool InsertAfter)
        {
            var result = new OutPt();
            result.Pt = outPt.Pt;
            result.Idx = outPt.Idx;
            if (InsertAfter)
            {
                result.Next = outPt.Next;
                result.Prev = outPt;
                outPt.Next.Prev = result;
                outPt.Next = result;
            }
            else
            {
                result.Prev = outPt.Prev;
                result.Next = outPt;
                outPt.Prev.Next = result;
                outPt.Prev = result;
            }
            return result;
        }

        //------------------------------------------------------------------------------

        private bool GetOverlap(long a1, long a2, long b1, long b2, out long Left, out long Right)
        {
            if (a1 < a2)
            {
                if (b1 < b2)
                {
                    Left = Math.Max(a1, b1);
                    Right = Math.Min(a2, b2);
                }
                else
                {
                    Left = Math.Max(a1, b2);
                    Right = Math.Min(a2, b1);
                }
            }
            else
            {
                if (b1 < b2)
                {
                    Left = Math.Max(a2, b1);
                    Right = Math.Min(a1, b2);
                }
                else
                {
                    Left = Math.Max(a2, b2);
                    Right = Math.Min(a1, b1);
                }
            }
            return Left < Right;
        }

        //------------------------------------------------------------------------------

        private bool JoinHorz(OutPt op1, OutPt op1b, OutPt op2, OutPt op2b, IntPoint Pt, bool DiscardLeft)
        {
            var Dir1 = op1.Pt.X > op1b.Pt.X ? Direction.dRightToLeft : Direction.dLeftToRight;
            var Dir2 = op2.Pt.X > op2b.Pt.X ? Direction.dRightToLeft : Direction.dLeftToRight;
            if (Dir1 == Dir2) return false;

            //When DiscardLeft, we want Op1b to be on the Left of Op1, otherwise we
            //want Op1b to be on the Right. (And likewise with Op2 and Op2b.)
            //So, to facilitate this while inserting Op1b and Op2b ...
            //when DiscardLeft, make sure we're AT or RIGHT of Pt before adding Op1b,
            //otherwise make sure we're AT or LEFT of Pt. (Likewise with Op2b.)
            if (Dir1 == Direction.dLeftToRight)
            {
                while (op1.Next.Pt.X <= Pt.X && op1.Next.Pt.X >= op1.Pt.X && op1.Next.Pt.Y == Pt.Y) op1 = op1.Next;

                if (DiscardLeft && op1.Pt.X != Pt.X) op1 = op1.Next;
                op1b = this.DupOutPt(op1, !DiscardLeft);
                if (op1b.Pt != Pt)
                {
                    op1 = op1b;
                    op1.Pt = Pt;
                    op1b = this.DupOutPt(op1, !DiscardLeft);
                }
            }
            else
            {
                while (op1.Next.Pt.X >= Pt.X && op1.Next.Pt.X <= op1.Pt.X && op1.Next.Pt.Y == Pt.Y) op1 = op1.Next;

                if (!DiscardLeft && op1.Pt.X != Pt.X) op1 = op1.Next;
                op1b = this.DupOutPt(op1, DiscardLeft);
                if (op1b.Pt != Pt)
                {
                    op1 = op1b;
                    op1.Pt = Pt;
                    op1b = this.DupOutPt(op1, DiscardLeft);
                }
            }

            if (Dir2 == Direction.dLeftToRight)
            {
                while (op2.Next.Pt.X <= Pt.X && op2.Next.Pt.X >= op2.Pt.X && op2.Next.Pt.Y == Pt.Y) op2 = op2.Next;

                if (DiscardLeft && op2.Pt.X != Pt.X) op2 = op2.Next;
                op2b = this.DupOutPt(op2, !DiscardLeft);
                if (op2b.Pt != Pt)
                {
                    op2 = op2b;
                    op2.Pt = Pt;
                    op2b = this.DupOutPt(op2, !DiscardLeft);
                }
                ;
            }
            else
            {
                while (op2.Next.Pt.X >= Pt.X && op2.Next.Pt.X <= op2.Pt.X && op2.Next.Pt.Y == Pt.Y) op2 = op2.Next;

                if (!DiscardLeft && op2.Pt.X != Pt.X) op2 = op2.Next;
                op2b = this.DupOutPt(op2, DiscardLeft);
                if (op2b.Pt != Pt)
                {
                    op2 = op2b;
                    op2.Pt = Pt;
                    op2b = this.DupOutPt(op2, DiscardLeft);
                }
                ;
            }

            ;

            if (Dir1 == Direction.dLeftToRight == DiscardLeft)
            {
                op1.Prev = op2;
                op2.Next = op1;
                op1b.Next = op2b;
                op2b.Prev = op1b;
            }
            else
            {
                op1.Next = op2;
                op2.Prev = op1;
                op1b.Prev = op2b;
                op2b.Next = op1b;
            }
            return true;
        }

        //------------------------------------------------------------------------------

        private bool JoinPoints(Join j, OutRec outRec1, OutRec outRec2)
        {
            OutPt op1 = j.OutPt1, op1b;
            OutPt op2 = j.OutPt2, op2b;

            //There are 3 kinds of joins for output polygons ...
            //1. Horizontal joins where Join.OutPt1 & Join.OutPt2 are vertices anywhere
            //along (horizontal) collinear edges (& Join.OffPt is on the same horizontal).
            //2. Non-horizontal joins where Join.OutPt1 & Join.OutPt2 are at the same
            //location at the Bottom of the overlapping segment (& Join.OffPt is above).
            //3. StrictlySimple joins where edges touch but are not collinear and where
            //Join.OutPt1, Join.OutPt2 & Join.OffPt all share the same point.
            var isHorizontal = j.OutPt1.Pt.Y == j.OffPt.Y;

            if (isHorizontal && j.OffPt == j.OutPt1.Pt && j.OffPt == j.OutPt2.Pt)
            {
                //Strictly Simple join ...
                if (outRec1 != outRec2) return false;

                op1b = j.OutPt1.Next;
                while (op1b != op1 && op1b.Pt == j.OffPt) op1b = op1b.Next;

                var reverse1 = op1b.Pt.Y > j.OffPt.Y;
                op2b = j.OutPt2.Next;
                while (op2b != op2 && op2b.Pt == j.OffPt) op2b = op2b.Next;

                var reverse2 = op2b.Pt.Y > j.OffPt.Y;
                if (reverse1 == reverse2) return false;

                if (reverse1)
                {
                    op1b = this.DupOutPt(op1, false);
                    op2b = this.DupOutPt(op2, true);
                    op1.Prev = op2;
                    op2.Next = op1;
                    op1b.Next = op2b;
                    op2b.Prev = op1b;
                    j.OutPt1 = op1;
                    j.OutPt2 = op1b;
                    return true;
                }
                else
                {
                    op1b = this.DupOutPt(op1, true);
                    op2b = this.DupOutPt(op2, false);
                    op1.Next = op2;
                    op2.Prev = op1;
                    op1b.Prev = op2b;
                    op2b.Next = op1b;
                    j.OutPt1 = op1;
                    j.OutPt2 = op1b;
                    return true;
                }
            }
            else if (isHorizontal)
            {
                //treat horizontal joins differently to non-horizontal joins since with
                //them we're not yet sure where the overlapping is. OutPt1.Pt & OutPt2.Pt
                //may be anywhere along the horizontal edge.
                op1b = op1;
                while (op1.Prev.Pt.Y == op1.Pt.Y && op1.Prev != op1b && op1.Prev != op2) op1 = op1.Prev;
                while (op1b.Next.Pt.Y == op1b.Pt.Y && op1b.Next != op1 && op1b.Next != op2) op1b = op1b.Next;

                if (op1b.Next == op1 || op1b.Next == op2) return false; //a flat 'polygon'

                op2b = op2;
                while (op2.Prev.Pt.Y == op2.Pt.Y && op2.Prev != op2b && op2.Prev != op1b) op2 = op2.Prev;
                while (op2b.Next.Pt.Y == op2b.Pt.Y && op2b.Next != op2 && op2b.Next != op1) op2b = op2b.Next;

                if (op2b.Next == op2 || op2b.Next == op1) return false; //a flat 'polygon'

                long Left, Right;

                //Op1 -. Op1b & Op2 -. Op2b are the extremites of the horizontal edges
                if (!this.GetOverlap(op1.Pt.X, op1b.Pt.X, op2.Pt.X, op2b.Pt.X, out Left, out Right)) return false;

                //DiscardLeftSide: when overlapping edges are joined, a spike will created
                //which needs to be cleaned up. However, we don't want Op1 or Op2 caught up
                //on the discard Side as either may still be needed for other joins ...
                IntPoint Pt;
                bool DiscardLeftSide;
                if (op1.Pt.X >= Left && op1.Pt.X <= Right)
                {
                    Pt = op1.Pt;
                    DiscardLeftSide = op1.Pt.X > op1b.Pt.X;
                }
                else if (op2.Pt.X >= Left && op2.Pt.X <= Right)
                {
                    Pt = op2.Pt;
                    DiscardLeftSide = op2.Pt.X > op2b.Pt.X;
                }
                else if (op1b.Pt.X >= Left && op1b.Pt.X <= Right)
                {
                    Pt = op1b.Pt;
                    DiscardLeftSide = op1b.Pt.X > op1.Pt.X;
                }
                else
                {
                    Pt = op2b.Pt;
                    DiscardLeftSide = op2b.Pt.X > op2.Pt.X;
                }
                j.OutPt1 = op1;
                j.OutPt2 = op2;
                return this.JoinHorz(op1, op1b, op2, op2b, Pt, DiscardLeftSide);
            }
            else
            {
                //nb: For non-horizontal joins ...
                //    1. Jr.OutPt1.Pt.Y == Jr.OutPt2.Pt.Y
                //    2. Jr.OutPt1.Pt > Jr.OffPt.Y

                //make sure the polygons are correctly oriented ...
                op1b = op1.Next;
                while (op1b.Pt == op1.Pt && op1b != op1) op1b = op1b.Next;

                var Reverse1 = op1b.Pt.Y > op1.Pt.Y || !SlopesEqual(op1.Pt, op1b.Pt, j.OffPt, this.m_UseFullRange);
                if (Reverse1)
                {
                    op1b = op1.Prev;
                    while (op1b.Pt == op1.Pt && op1b != op1) op1b = op1b.Prev;

                    if (op1b.Pt.Y > op1.Pt.Y || !SlopesEqual(op1.Pt, op1b.Pt, j.OffPt, this.m_UseFullRange))
                        return false;
                }

                ;
                op2b = op2.Next;
                while (op2b.Pt == op2.Pt && op2b != op2) op2b = op2b.Next;

                var Reverse2 = op2b.Pt.Y > op2.Pt.Y || !SlopesEqual(op2.Pt, op2b.Pt, j.OffPt, this.m_UseFullRange);
                if (Reverse2)
                {
                    op2b = op2.Prev;
                    while (op2b.Pt == op2.Pt && op2b != op2) op2b = op2b.Prev;

                    if (op2b.Pt.Y > op2.Pt.Y || !SlopesEqual(op2.Pt, op2b.Pt, j.OffPt, this.m_UseFullRange))
                        return false;
                }

                if (op1b == op1 || op2b == op2 || op1b == op2b || outRec1 == outRec2 && Reverse1 == Reverse2)
                    return false;

                if (Reverse1)
                {
                    op1b = this.DupOutPt(op1, false);
                    op2b = this.DupOutPt(op2, true);
                    op1.Prev = op2;
                    op2.Next = op1;
                    op1b.Next = op2b;
                    op2b.Prev = op1b;
                    j.OutPt1 = op1;
                    j.OutPt2 = op1b;
                    return true;
                }
                else
                {
                    op1b = this.DupOutPt(op1, true);
                    op2b = this.DupOutPt(op2, false);
                    op1.Next = op2;
                    op2.Prev = op1;
                    op1b.Prev = op2b;
                    op2b.Next = op1b;
                    j.OutPt1 = op1;
                    j.OutPt2 = op1b;
                    return true;
                }
            }
        }

        //----------------------------------------------------------------------

        public static int PointInPolygon(IntPoint pt, Path path)
        {
            //returns 0 if false, +1 if true, -1 if pt ON polygon boundary
            //See "The Point in Polygon Problem for Arbitrary Polygons" by Hormann & Agathos
            //http://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.88.5498&rep=rep1&type=pdf
            int result = 0, cnt = path.Count;
            if (cnt < 3) return 0;

            var ip = path[0];
            for (var i = 1; i <= cnt; ++i)
            {
                var ipNext = i == cnt ? path[0] : path[i];
                if (ipNext.Y == pt.Y)
                {
                    if (ipNext.X == pt.X || ip.Y == pt.Y && ipNext.X > pt.X == ip.X < pt.X) return -1;
                }

                if (ip.Y < pt.Y != ipNext.Y < pt.Y)
                {
                    if (ip.X >= pt.X)
                    {
                        if (ipNext.X > pt.X) result = 1 - result;
                        else
                        {
                            var d = (double)(ip.X - pt.X) * (ipNext.Y - pt.Y)
                                    - (double)(ipNext.X - pt.X) * (ip.Y - pt.Y);
                            if (d == 0) return -1;
                            else if (d > 0 == ipNext.Y > ip.Y) result = 1 - result;
                        }
                    }
                    else
                    {
                        if (ipNext.X > pt.X)
                        {
                            var d = (double)(ip.X - pt.X) * (ipNext.Y - pt.Y)
                                    - (double)(ipNext.X - pt.X) * (ip.Y - pt.Y);
                            if (d == 0) return -1;
                            else if (d > 0 == ipNext.Y > ip.Y) result = 1 - result;
                        }
                    }
                }

                ip = ipNext;
            }

            return result;
        }

        //------------------------------------------------------------------------------

        //See "The Point in Polygon Problem for Arbitrary Polygons" by Hormann & Agathos
        //http://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.88.5498&rep=rep1&type=pdf
        private static int PointInPolygon(IntPoint pt, OutPt op)
        {
            //returns 0 if false, +1 if true, -1 if pt ON polygon boundary
            var result = 0;
            var startOp = op;
            long ptx = pt.X, pty = pt.Y;
            long poly0x = op.Pt.X, poly0y = op.Pt.Y;
            do
            {
                op = op.Next;
                long poly1x = op.Pt.X, poly1y = op.Pt.Y;

                if (poly1y == pty)
                {
                    if (poly1x == ptx || poly0y == pty && poly1x > ptx == poly0x < ptx) return -1;
                }

                if (poly0y < pty != poly1y < pty)
                {
                    if (poly0x >= ptx)
                    {
                        if (poly1x > ptx) result = 1 - result;
                        else
                        {
                            var d = (double)(poly0x - ptx) * (poly1y - pty) - (double)(poly1x - ptx) * (poly0y - pty);
                            if (d == 0) return -1;

                            if (d > 0 == poly1y > poly0y) result = 1 - result;
                        }
                    }
                    else
                    {
                        if (poly1x > ptx)
                        {
                            var d = (double)(poly0x - ptx) * (poly1y - pty) - (double)(poly1x - ptx) * (poly0y - pty);
                            if (d == 0) return -1;

                            if (d > 0 == poly1y > poly0y) result = 1 - result;
                        }
                    }
                }

                poly0x = poly1x;
                poly0y = poly1y;
            }
            while (startOp != op);

            return result;
        }

        //------------------------------------------------------------------------------

        private static bool Poly2ContainsPoly1(OutPt outPt1, OutPt outPt2)
        {
            var op = outPt1;
            do
            {
                //nb: PointInPolygon returns 0 if false, +1 if true, -1 if pt on polygon
                var res = PointInPolygon(op.Pt, outPt2);
                if (res >= 0) return res > 0;

                op = op.Next;
            }
            while (op != outPt1);

            return true;
        }

        //----------------------------------------------------------------------

        private void FixupFirstLefts1(OutRec OldOutRec, OutRec NewOutRec)
        {
            foreach (var outRec in this.m_PolyOuts)
            {
                var firstLeft = ParseFirstLeft(outRec.FirstLeft);
                if (outRec.Pts != null && firstLeft == OldOutRec)
                {
                    if (Poly2ContainsPoly1(outRec.Pts, NewOutRec.Pts)) outRec.FirstLeft = NewOutRec;
                }
            }
        }

        //----------------------------------------------------------------------

        private void FixupFirstLefts2(OutRec innerOutRec, OutRec outerOutRec)
        {
            //A polygon has split into two such that one is now the inner of the other.
            //It's possible that these polygons now wrap around other polygons, so check
            //every polygon that's also contained by OuterOutRec's FirstLeft container
            //(including nil) to see if they've become inner to the new inner polygon ...
            var orfl = outerOutRec.FirstLeft;
            foreach (var outRec in this.m_PolyOuts)
            {
                if (outRec.Pts == null || outRec == outerOutRec || outRec == innerOutRec) continue;

                var firstLeft = ParseFirstLeft(outRec.FirstLeft);
                if (firstLeft != orfl && firstLeft != innerOutRec && firstLeft != outerOutRec) continue;

                if (Poly2ContainsPoly1(outRec.Pts, innerOutRec.Pts)) outRec.FirstLeft = innerOutRec;
                else if (Poly2ContainsPoly1(outRec.Pts, outerOutRec.Pts)) outRec.FirstLeft = outerOutRec;
                else if (outRec.FirstLeft == innerOutRec || outRec.FirstLeft == outerOutRec) outRec.FirstLeft = orfl;
            }
        }

        //----------------------------------------------------------------------

        private void FixupFirstLefts3(OutRec OldOutRec, OutRec NewOutRec)
        {
            //same as FixupFirstLefts1 but doesn't call Poly2ContainsPoly1()
            foreach (var outRec in this.m_PolyOuts)
            {
                var firstLeft = ParseFirstLeft(outRec.FirstLeft);
                if (outRec.Pts != null && firstLeft == OldOutRec) outRec.FirstLeft = NewOutRec;
            }
        }

        //----------------------------------------------------------------------

        private static OutRec ParseFirstLeft(OutRec FirstLeft)
        {
            while (FirstLeft != null && FirstLeft.Pts == null) FirstLeft = FirstLeft.FirstLeft;

            return FirstLeft;
        }

        //------------------------------------------------------------------------------

        private void JoinCommonEdges()
        {
            for (var i = 0; i < this.m_Joins.Count; i++)
            {
                var join = this.m_Joins[i];

                var outRec1 = this.GetOutRec(join.OutPt1.Idx);
                var outRec2 = this.GetOutRec(join.OutPt2.Idx);

                if (outRec1.Pts == null || outRec2.Pts == null) continue;
                if (outRec1.IsOpen || outRec2.IsOpen) continue;

                //get the polygon fragment with the correct hole state (FirstLeft)
                //before calling JoinPoints() ...
                OutRec holeStateRec;
                if (outRec1 == outRec2) holeStateRec = outRec1;
                else if (this.OutRec1RightOfOutRec2(outRec1, outRec2)) holeStateRec = outRec2;
                else if (this.OutRec1RightOfOutRec2(outRec2, outRec1)) holeStateRec = outRec1;
                else holeStateRec = this.GetLowermostRec(outRec1, outRec2);

                if (!this.JoinPoints(join, outRec1, outRec2)) continue;

                if (outRec1 == outRec2)
                {
                    //instead of joining two polygons, we've just created a new one by
                    //splitting one polygon into two.
                    outRec1.Pts = join.OutPt1;
                    outRec1.BottomPt = null;
                    outRec2 = this.CreateOutRec();
                    outRec2.Pts = join.OutPt2;

                    //update all OutRec2.Pts Idx's ...
                    this.UpdateOutPtIdxs(outRec2);

                    if (Poly2ContainsPoly1(outRec2.Pts, outRec1.Pts))
                    {
                        //outRec1 contains outRec2 ...
                        outRec2.IsHole = !outRec1.IsHole;
                        outRec2.FirstLeft = outRec1;

                        if (this.m_UsingPolyTree) this.FixupFirstLefts2(outRec2, outRec1);

                        if ((outRec2.IsHole ^ this.ReverseSolution) == this.Area(outRec2) > 0)
                            this.ReversePolyPtLinks(outRec2.Pts);
                    }
                    else if (Poly2ContainsPoly1(outRec1.Pts, outRec2.Pts))
                    {
                        //outRec2 contains outRec1 ...
                        outRec2.IsHole = outRec1.IsHole;
                        outRec1.IsHole = !outRec2.IsHole;
                        outRec2.FirstLeft = outRec1.FirstLeft;
                        outRec1.FirstLeft = outRec2;

                        if (this.m_UsingPolyTree) this.FixupFirstLefts2(outRec1, outRec2);

                        if ((outRec1.IsHole ^ this.ReverseSolution) == this.Area(outRec1) > 0)
                            this.ReversePolyPtLinks(outRec1.Pts);
                    }
                    else
                    {
                        //the 2 polygons are completely separate ...
                        outRec2.IsHole = outRec1.IsHole;
                        outRec2.FirstLeft = outRec1.FirstLeft;

                        //fixup FirstLeft pointers that may need reassigning to OutRec2
                        if (this.m_UsingPolyTree) this.FixupFirstLefts1(outRec1, outRec2);
                    }
                }
                else
                {
                    //joined 2 polygons together ...

                    outRec2.Pts = null;
                    outRec2.BottomPt = null;
                    outRec2.Idx = outRec1.Idx;

                    outRec1.IsHole = holeStateRec.IsHole;
                    if (holeStateRec == outRec2) outRec1.FirstLeft = outRec2.FirstLeft;
                    outRec2.FirstLeft = outRec1;

                    //fixup FirstLeft pointers that may need reassigning to OutRec1
                    if (this.m_UsingPolyTree) this.FixupFirstLefts3(outRec2, outRec1);
                }
            }
        }

        //------------------------------------------------------------------------------

        private void UpdateOutPtIdxs(OutRec outrec)
        {
            var op = outrec.Pts;
            do
            {
                op.Idx = outrec.Idx;
                op = op.Prev;
            }
            while (op != outrec.Pts);
        }

        //------------------------------------------------------------------------------

        private void DoSimplePolygons()
        {
            var i = 0;
            while (i < this.m_PolyOuts.Count)
            {
                var outrec = this.m_PolyOuts[i++];
                var op = outrec.Pts;
                if (op == null || outrec.IsOpen) continue;

                do //for each Pt in Polygon until duplicate found do ...
                {
                    var op2 = op.Next;
                    while (op2 != outrec.Pts)
                    {
                        if (op.Pt == op2.Pt && op2.Next != op && op2.Prev != op)
                        {
                            //split the polygon into two ...
                            var op3 = op.Prev;
                            var op4 = op2.Prev;
                            op.Prev = op4;
                            op4.Next = op;
                            op2.Prev = op3;
                            op3.Next = op2;

                            outrec.Pts = op;
                            var outrec2 = this.CreateOutRec();
                            outrec2.Pts = op2;
                            this.UpdateOutPtIdxs(outrec2);
                            if (Poly2ContainsPoly1(outrec2.Pts, outrec.Pts))
                            {
                                //OutRec2 is contained by OutRec1 ...
                                outrec2.IsHole = !outrec.IsHole;
                                outrec2.FirstLeft = outrec;
                                if (this.m_UsingPolyTree) this.FixupFirstLefts2(outrec2, outrec);
                            }
                            else if (Poly2ContainsPoly1(outrec.Pts, outrec2.Pts))
                            {
                                //OutRec1 is contained by OutRec2 ...
                                outrec2.IsHole = outrec.IsHole;
                                outrec.IsHole = !outrec2.IsHole;
                                outrec2.FirstLeft = outrec.FirstLeft;
                                outrec.FirstLeft = outrec2;
                                if (this.m_UsingPolyTree) this.FixupFirstLefts2(outrec, outrec2);
                            }
                            else
                            {
                                //the 2 polygons are separate ...
                                outrec2.IsHole = outrec.IsHole;
                                outrec2.FirstLeft = outrec.FirstLeft;
                                if (this.m_UsingPolyTree) this.FixupFirstLefts1(outrec, outrec2);
                            }
                            op2 = op; //ie get ready for the next iteration
                        }
                        op2 = op2.Next;
                    }

                    op = op.Next;
                }
                while (op != outrec.Pts);
            }
        }

        //------------------------------------------------------------------------------

        public static double Area(Path poly)
        {
            var cnt = (int)poly.Count;
            if (cnt < 3) return 0;

            double a = 0;
            for (int i = 0, j = cnt - 1; i < cnt; ++i)
            {
                a += ((double)poly[j].X + poly[i].X) * ((double)poly[j].Y - poly[i].Y);
                j = i;
            }

            return -a * 0.5;
        }

        //------------------------------------------------------------------------------

        internal double Area(OutRec outRec)
        {
            return this.Area(outRec.Pts);
        }

        //------------------------------------------------------------------------------

        internal double Area(OutPt op)
        {
            var opFirst = op;
            if (op == null) return 0;

            double a = 0;
            do
            {
                a = a + (double)(op.Prev.Pt.X + op.Pt.X) * (double)(op.Prev.Pt.Y - op.Pt.Y);
                op = op.Next;
            }
            while (op != opFirst);

            return a * 0.5;
        }

        //------------------------------------------------------------------------------
        // SimplifyPolygon functions ...
        // Convert self-intersecting polygons into simple polygons
        //------------------------------------------------------------------------------

        public static Paths SimplifyPolygon(Path poly, PolyFillType fillType = PolyFillType.pftEvenOdd)
        {
            var result = new Paths();
            var c = new Clipper();
            c.StrictlySimple = true;
            c.AddPath(poly, PolyType.ptSubject, true);
            c.Execute(ClipType.ctUnion, result, fillType, fillType);
            return result;
        }

        //------------------------------------------------------------------------------

        public static Paths SimplifyPolygons(Paths polys, PolyFillType fillType = PolyFillType.pftEvenOdd)
        {
            var result = new Paths();
            var c = new Clipper();
            c.StrictlySimple = true;
            c.AddPaths(polys, PolyType.ptSubject, true);
            c.Execute(ClipType.ctUnion, result, fillType, fillType);
            return result;
        }

        //------------------------------------------------------------------------------

        private static double DistanceSqrd(IntPoint pt1, IntPoint pt2)
        {
            var dx = (double)pt1.X - pt2.X;
            var dy = (double)pt1.Y - pt2.Y;
            return dx * dx + dy * dy;
        }

        //------------------------------------------------------------------------------

        private static double DistanceFromLineSqrd(IntPoint pt, IntPoint ln1, IntPoint ln2)
        {
            //The equation of a line in general form (Ax + By + C = 0)
            //given 2 points (x¹,y¹) & (x²,y²) is ...
            //(y¹ - y²)x + (x² - x¹)y + (y² - y¹)x¹ - (x² - x¹)y¹ = 0
            //A = (y¹ - y²); B = (x² - x¹); C = (y² - y¹)x¹ - (x² - x¹)y¹
            //perpendicular distance of point (x³,y³) = (Ax³ + By³ + C)/Sqrt(A² + B²)
            //see http://en.wikipedia.org/wiki/Perpendicular_distance
            double A = ln1.Y - ln2.Y;
            double B = ln2.X - ln1.X;
            var C = A * ln1.X + B * ln1.Y;
            C = A * pt.X + B * pt.Y - C;
            return C * C / (A * A + B * B);
        }

        //---------------------------------------------------------------------------

        private static bool SlopesNearCollinear(IntPoint pt1, IntPoint pt2, IntPoint pt3, double distSqrd)
        {
            //this function is more accurate when the point that's GEOMETRICALLY
            //between the other 2 points is the one that's tested for distance.
            //nb: with 'spikes', either pt1 or pt3 is geometrically between the other pts
            if (Math.Abs(pt1.X - pt2.X) > Math.Abs(pt1.Y - pt2.Y))
            {
                if (pt1.X > pt2.X == pt1.X < pt3.X) return DistanceFromLineSqrd(pt1, pt2, pt3) < distSqrd;
                else if (pt2.X > pt1.X == pt2.X < pt3.X) return DistanceFromLineSqrd(pt2, pt1, pt3) < distSqrd;
                else return DistanceFromLineSqrd(pt3, pt1, pt2) < distSqrd;
            }
            else
            {
                if (pt1.Y > pt2.Y == pt1.Y < pt3.Y) return DistanceFromLineSqrd(pt1, pt2, pt3) < distSqrd;
                else if (pt2.Y > pt1.Y == pt2.Y < pt3.Y) return DistanceFromLineSqrd(pt2, pt1, pt3) < distSqrd;
                else return DistanceFromLineSqrd(pt3, pt1, pt2) < distSqrd;
            }
        }

        //------------------------------------------------------------------------------

        private static bool PointsAreClose(IntPoint pt1, IntPoint pt2, double distSqrd)
        {
            var dx = (double)pt1.X - pt2.X;
            var dy = (double)pt1.Y - pt2.Y;
            return dx * dx + dy * dy <= distSqrd;
        }

        //------------------------------------------------------------------------------

        private static OutPt ExcludeOp(OutPt op)
        {
            var result = op.Prev;
            result.Next = op.Next;
            op.Next.Prev = result;
            result.Idx = 0;
            return result;
        }

        //------------------------------------------------------------------------------

        public static Path CleanPolygon(Path path, double distance = 1.415)
        {
            //distance = proximity in units/pixels below which vertices will be stripped.
            //Default ~= sqrt(2) so when adjacent vertices or semi-adjacent vertices have
            //both x & y coords within 1 unit, then the second vertex will be stripped.

            var cnt = path.Count;

            if (cnt == 0) return new Path();

            var outPts = new OutPt[cnt];
            for (var i = 0; i < cnt; ++i) outPts[i] = new OutPt();

            for (var i = 0; i < cnt; ++i)
            {
                outPts[i].Pt = path[i];
                outPts[i].Next = outPts[(i + 1) % cnt];
                outPts[i].Next.Prev = outPts[i];
                outPts[i].Idx = 0;
            }

            var distSqrd = distance * distance;
            var op = outPts[0];
            while (op.Idx == 0 && op.Next != op.Prev)
                if (PointsAreClose(op.Pt, op.Prev.Pt, distSqrd))
                {
                    op = ExcludeOp(op);
                    cnt--;
                }
                else if (PointsAreClose(op.Prev.Pt, op.Next.Pt, distSqrd))
                {
                    ExcludeOp(op.Next);
                    op = ExcludeOp(op);
                    cnt -= 2;
                }
                else if (SlopesNearCollinear(op.Prev.Pt, op.Pt, op.Next.Pt, distSqrd))
                {
                    op = ExcludeOp(op);
                    cnt--;
                }
                else
                {
                    op.Idx = 1;
                    op = op.Next;
                }

            if (cnt < 3) cnt = 0;
            var result = new Path(cnt);
            for (var i = 0; i < cnt; ++i)
            {
                result.Add(op.Pt);
                op = op.Next;
            }

            outPts = null;
            return result;
        }

        //------------------------------------------------------------------------------

        public static Paths CleanPolygons(Paths polys, double distance = 1.415)
        {
            var result = new Paths(polys.Count);
            for (var i = 0; i < polys.Count; i++) result.Add(CleanPolygon(polys[i], distance));

            return result;
        }

        //------------------------------------------------------------------------------

        internal static Paths Minkowski(Path pattern, Path path, bool IsSum, bool IsClosed)
        {
            var delta = IsClosed ? 1 : 0;
            var polyCnt = pattern.Count;
            var pathCnt = path.Count;
            var result = new Paths(pathCnt);
            if (IsSum)
            {
                for (var i = 0; i < pathCnt; i++)
                {
                    var p = new Path(polyCnt);
                    foreach (var ip in pattern)
                    {
                        p.Add(new IntPoint(path[i].X + ip.X, path[i].Y + ip.Y));
                    }

                    result.Add(p);
                }
            }
            else
            {
                for (var i = 0; i < pathCnt; i++)
                {
                    var p = new Path(polyCnt);
                    foreach (var ip in pattern)
                    {
                        p.Add(new IntPoint(path[i].X - ip.X, path[i].Y - ip.Y));
                    }

                    result.Add(p);
                }
            }

            var quads = new Paths((pathCnt + delta) * (polyCnt + 1));
            for (var i = 0; i < pathCnt - 1 + delta; i++)
                for (var j = 0; j < polyCnt; j++)
                {
                    var quad = new Path(4);
                    quad.Add(result[i % pathCnt][j % polyCnt]);
                    quad.Add(result[(i + 1) % pathCnt][j % polyCnt]);
                    quad.Add(result[(i + 1) % pathCnt][(j + 1) % polyCnt]);
                    quad.Add(result[i % pathCnt][(j + 1) % polyCnt]);
                    if (!Orientation(quad)) quad.Reverse();
                    quads.Add(quad);
                }

            return quads;
        }

        //------------------------------------------------------------------------------

        public static Paths MinkowskiSum(Path pattern, Path path, bool pathIsClosed)
        {
            var paths = Minkowski(pattern, path, true, pathIsClosed);
            var c = new Clipper();
            c.AddPaths(paths, PolyType.ptSubject, true);
            c.Execute(ClipType.ctUnion, paths, PolyFillType.pftNonZero, PolyFillType.pftNonZero);
            return paths;
        }

        //------------------------------------------------------------------------------

        private static Path TranslatePath(Path path, IntPoint delta)
        {
            var outPath = new Path(path.Count);
            for (var i = 0; i < path.Count; i++) outPath.Add(new IntPoint(path[i].X + delta.X, path[i].Y + delta.Y));

            return outPath;
        }

        //------------------------------------------------------------------------------

        public static Paths MinkowskiSum(Path pattern, Paths paths, bool pathIsClosed)
        {
            var solution = new Paths();
            var c = new Clipper();
            for (var i = 0; i < paths.Count; ++i)
            {
                var tmp = Minkowski(pattern, paths[i], true, pathIsClosed);
                c.AddPaths(tmp, PolyType.ptSubject, true);
                if (pathIsClosed)
                {
                    var path = TranslatePath(paths[i], pattern[0]);
                    c.AddPath(path, PolyType.ptClip, true);
                }
            }

            c.Execute(ClipType.ctUnion, solution, PolyFillType.pftNonZero, PolyFillType.pftNonZero);
            return solution;
        }

        //------------------------------------------------------------------------------

        public static Paths MinkowskiDiff(Path poly1, Path poly2)
        {
            var paths = Minkowski(poly1, poly2, false, true);
            var c = new Clipper();
            c.AddPaths(paths, PolyType.ptSubject, true);
            c.Execute(ClipType.ctUnion, paths, PolyFillType.pftNonZero, PolyFillType.pftNonZero);
            return paths;
        }

        //------------------------------------------------------------------------------

        internal enum NodeType
        {
            ntAny,

            ntOpen,

            ntClosed
        };

        public static Paths PolyTreeToPaths(PolyTree polytree)
        {
            var result = new Paths();
            result.Capacity = polytree.Total;
            AddPolyNodeToPaths(polytree, NodeType.ntAny, result);
            return result;
        }

        //------------------------------------------------------------------------------

        internal static void AddPolyNodeToPaths(PolyNode polynode, NodeType nt, Paths paths)
        {
            var match = true;
            switch (nt)
            {
                case NodeType.ntOpen: return;
                case NodeType.ntClosed:
                    match = !polynode.IsOpen;
                    break;
                default: break;
            }

            if (polynode.m_polygon.Count > 0 && match) paths.Add(polynode.m_polygon);
            foreach (var pn in polynode.Childs)
            {
                AddPolyNodeToPaths(pn, nt, paths);
            }
        }

        //------------------------------------------------------------------------------

        public static Paths OpenPathsFromPolyTree(PolyTree polytree)
        {
            var result = new Paths();
            result.Capacity = polytree.ChildCount;
            for (var i = 0; i < polytree.ChildCount; i++)
                if (polytree.Childs[i].IsOpen) result.Add(polytree.Childs[i].m_polygon);

            return result;
        }

        //------------------------------------------------------------------------------

        public static Paths ClosedPathsFromPolyTree(PolyTree polytree)
        {
            var result = new Paths();
            result.Capacity = polytree.Total;
            AddPolyNodeToPaths(polytree, NodeType.ntClosed, result);
            return result;
        }

        //------------------------------------------------------------------------------
    } //end Clipper

    //------------------------------------------------------------------------------
} //end ClipperLib namespace