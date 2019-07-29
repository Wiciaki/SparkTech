namespace SparkTech.SDK.Clipper
{
    using System.Collections.Generic;

    public class ClipperBase
    {
        internal const double horizontal = -3.4E+38;

        internal const int Skip = -2;

        internal const int Unassigned = -1;

        internal const double tolerance = 1.0E-20;

        internal static bool near_zero(double val)
        {
            return val > -tolerance && val < tolerance;
        }

#if use_int32
    public const cInt loRange = 0x7FFF;
    public const cInt hiRange = 0x7FFF;
#else
        public const long loRange = 0x3FFFFFFF;
        public const long hiRange = 0x3FFFFFFFFFFFFFFFL;
#endif

        internal LocalMinima m_MinimaList;

        internal LocalMinima m_CurrentLM;

        internal List<List<TEdge>> m_edges = new List<List<TEdge>>();

        internal Scanbeam m_Scanbeam;

        internal List<OutRec> m_PolyOuts;

        internal TEdge m_ActiveEdges;

        internal bool m_UseFullRange;

        internal bool m_HasOpenPaths;

        //------------------------------------------------------------------------------

        public bool PreserveCollinear { get; set; }

        //------------------------------------------------------------------------------

        public void Swap(ref long val1, ref long val2)
        {
            var tmp = val1;
            val1 = val2;
            val2 = tmp;
        }

        //------------------------------------------------------------------------------

        internal static bool IsHorizontal(TEdge e)
        {
            return e.Delta.Y == 0;
        }

        //------------------------------------------------------------------------------

        internal bool PointIsVertex(IntPoint pt, OutPt pp)
        {
            var pp2 = pp;
            do
            {
                if (pp2.Pt == pt) return true;

                pp2 = pp2.Next;
            }
            while (pp2 != pp);

            return false;
        }

        //------------------------------------------------------------------------------

        internal bool PointOnLineSegment(IntPoint pt, IntPoint linePt1, IntPoint linePt2, bool UseFullRange)
        {
            if (UseFullRange)
            {
                return pt.X == linePt1.X && pt.Y == linePt1.Y || pt.X == linePt2.X && pt.Y == linePt2.Y
                                                              || pt.X > linePt1.X == pt.X < linePt2.X && pt.Y > linePt1.Y == pt.Y < linePt2.Y
                                                                                                      && Int128.Int128Mul(pt.X - linePt1.X, linePt2.Y - linePt1.Y)
                                                                                                      == Int128.Int128Mul(linePt2.X - linePt1.X, pt.Y - linePt1.Y);
            }
            else
            {
                return pt.X == linePt1.X && pt.Y == linePt1.Y || pt.X == linePt2.X && pt.Y == linePt2.Y
                                                              || pt.X > linePt1.X == pt.X < linePt2.X && pt.Y > linePt1.Y == pt.Y < linePt2.Y
                                                                                                      && (pt.X - linePt1.X) * (linePt2.Y - linePt1.Y) == (linePt2.X - linePt1.X) * (pt.Y - linePt1.Y);
            }
        }

        //------------------------------------------------------------------------------

        internal bool PointOnPolygon(IntPoint pt, OutPt pp, bool UseFullRange)
        {
            var pp2 = pp;
            while (true)
            {
                if (this.PointOnLineSegment(pt, pp2.Pt, pp2.Next.Pt, UseFullRange)) return true;

                pp2 = pp2.Next;
                if (pp2 == pp) break;
            }

            return false;
        }

        //------------------------------------------------------------------------------

        internal static bool SlopesEqual(TEdge e1, TEdge e2, bool UseFullRange)
        {
            if (UseFullRange)
            {
                return Int128.Int128Mul(e1.Delta.Y, e2.Delta.X) == Int128.Int128Mul(e1.Delta.X, e2.Delta.Y);
            }
            else
            {
                return (long)e1.Delta.Y * e2.Delta.X == (long)e1.Delta.X * e2.Delta.Y;
            }
        }

        //------------------------------------------------------------------------------

        internal static bool SlopesEqual(IntPoint pt1, IntPoint pt2, IntPoint pt3, bool UseFullRange)
        {
            if (UseFullRange)
            {
                return Int128.Int128Mul(pt1.Y - pt2.Y, pt2.X - pt3.X) == Int128.Int128Mul(pt1.X - pt2.X, pt2.Y - pt3.Y);
            }
            else
            {
                return (long)(pt1.Y - pt2.Y) * (pt2.X - pt3.X) - (long)(pt1.X - pt2.X) * (pt2.Y - pt3.Y) == 0;
            }
        }

        //------------------------------------------------------------------------------

        internal static bool SlopesEqual(IntPoint pt1, IntPoint pt2, IntPoint pt3, IntPoint pt4, bool UseFullRange)
        {
            if (UseFullRange)
            {
                return Int128.Int128Mul(pt1.Y - pt2.Y, pt3.X - pt4.X) == Int128.Int128Mul(pt1.X - pt2.X, pt3.Y - pt4.Y);
            }
            else
            {
                return (long)(pt1.Y - pt2.Y) * (pt3.X - pt4.X) - (long)(pt1.X - pt2.X) * (pt3.Y - pt4.Y) == 0;
            }
        }

        //------------------------------------------------------------------------------

        internal ClipperBase() //constructor (nb: no external instantiation)
        {
            this.m_MinimaList = null;
            this.m_CurrentLM = null;
            this.m_UseFullRange = false;
            this.m_HasOpenPaths = false;
        }

        //------------------------------------------------------------------------------

        public virtual void Clear()
        {
            this.DisposeLocalMinimaList();
            for (var i = 0; i < this.m_edges.Count; ++i)
            {
                for (var j = 0; j < this.m_edges[i].Count; ++j) this.m_edges[i][j] = null;

                this.m_edges[i].Clear();
            }

            this.m_edges.Clear();
            this.m_UseFullRange = false;
            this.m_HasOpenPaths = false;
        }

        //------------------------------------------------------------------------------

        private void DisposeLocalMinimaList()
        {
            while (this.m_MinimaList != null)
            {
                var tmpLm = this.m_MinimaList.Next;
                this.m_MinimaList = null;
                this.m_MinimaList = tmpLm;
            }

            this.m_CurrentLM = null;
        }

        //------------------------------------------------------------------------------

        private void RangeTest(IntPoint Pt, ref bool useFullRange)
        {
            if (useFullRange)
            {
                if (Pt.X > hiRange || Pt.Y > hiRange || -Pt.X > hiRange || -Pt.Y > hiRange)
                    throw new ClipperException("Coordinate outside allowed range");
            }
            else if (Pt.X > loRange || Pt.Y > loRange || -Pt.X > loRange || -Pt.Y > loRange)
            {
                useFullRange = true;
                this.RangeTest(Pt, ref useFullRange);
            }
        }

        //------------------------------------------------------------------------------

        private void InitEdge(TEdge e, TEdge eNext, TEdge ePrev, IntPoint pt)
        {
            e.Next = eNext;
            e.Prev = ePrev;
            e.Curr = pt;
            e.OutIdx = Unassigned;
        }

        //------------------------------------------------------------------------------

        private void InitEdge2(TEdge e, PolyType polyType)
        {
            if (e.Curr.Y >= e.Next.Curr.Y)
            {
                e.Bot = e.Curr;
                e.Top = e.Next.Curr;
            }
            else
            {
                e.Top = e.Curr;
                e.Bot = e.Next.Curr;
            }
            this.SetDx(e);
            e.PolyTyp = polyType;
        }

        //------------------------------------------------------------------------------

        private TEdge FindNextLocMin(TEdge E)
        {
            TEdge E2;
            for (;;)
            {
                while (E.Bot != E.Prev.Bot || E.Curr == E.Top) E = E.Next;

                if (E.Dx != horizontal && E.Prev.Dx != horizontal) break;

                while (E.Prev.Dx == horizontal) E = E.Prev;

                E2 = E;
                while (E.Dx == horizontal) E = E.Next;

                if (E.Top.Y == E.Prev.Bot.Y) continue; //ie just an intermediate horz.

                if (E2.Prev.Bot.X < E.Bot.X) E = E2;
                break;
            }

            return E;
        }

        //------------------------------------------------------------------------------

        private TEdge ProcessBound(TEdge E, bool LeftBoundIsForward)
        {
            TEdge EStart, Result = E;
            TEdge Horz;

            if (Result.OutIdx == Skip)
            {
                //check if there are edges beyond the skip edge in the bound and if so
                //create another LocMin and calling ProcessBound once more ...
                E = Result;
                if (LeftBoundIsForward)
                {
                    while (E.Top.Y == E.Next.Bot.Y) E = E.Next;
                    while (E != Result && E.Dx == horizontal) E = E.Prev;
                }
                else
                {
                    while (E.Top.Y == E.Prev.Bot.Y) E = E.Prev;
                    while (E != Result && E.Dx == horizontal) E = E.Next;
                }

                if (E == Result)
                {
                    if (LeftBoundIsForward) Result = E.Next;
                    else Result = E.Prev;
                }
                else
                {
                    //there are more edges in the bound beyond result starting with E
                    if (LeftBoundIsForward) E = Result.Next;
                    else E = Result.Prev;
                    var locMin = new LocalMinima();
                    locMin.Next = null;
                    locMin.Y = E.Bot.Y;
                    locMin.LeftBound = null;
                    locMin.RightBound = E;
                    E.WindDelta = 0;
                    Result = this.ProcessBound(E, LeftBoundIsForward);
                    this.InsertLocalMinima(locMin);
                }
                return Result;
            }

            if (E.Dx == horizontal)
            {
                //We need to be careful with open paths because this may not be a
                //true local minima (ie E may be following a skip edge).
                //Also, consecutive horz. edges may start heading left before going right.
                if (LeftBoundIsForward) EStart = E.Prev;
                else EStart = E.Next;
                if (EStart.Dx == horizontal) //ie an adjoining horizontal skip edge
                {
                    if (EStart.Bot.X != E.Bot.X && EStart.Top.X != E.Bot.X) this.ReverseHorizontal(E);
                }
                else if (EStart.Bot.X != E.Bot.X) this.ReverseHorizontal(E);
            }

            EStart = E;
            if (LeftBoundIsForward)
            {
                while (Result.Top.Y == Result.Next.Bot.Y && Result.Next.OutIdx != Skip) Result = Result.Next;

                if (Result.Dx == horizontal && Result.Next.OutIdx != Skip)
                {
                    //nb: at the top of a bound, horizontals are added to the bound
                    //only when the preceding edge attaches to the horizontal's left vertex
                    //unless a Skip edge is encountered when that becomes the top divide
                    Horz = Result;
                    while (Horz.Prev.Dx == horizontal) Horz = Horz.Prev;

                    if (Horz.Prev.Top.X > Result.Next.Top.X) Result = Horz.Prev;
                }

                while (E != Result)
                {
                    E.NextInLML = E.Next;
                    if (E.Dx == horizontal && E != EStart && E.Bot.X != E.Prev.Top.X) this.ReverseHorizontal(E);
                    E = E.Next;
                }

                if (E.Dx == horizontal && E != EStart && E.Bot.X != E.Prev.Top.X) this.ReverseHorizontal(E);
                Result = Result.Next; //move to the edge just beyond current bound
            }
            else
            {
                while (Result.Top.Y == Result.Prev.Bot.Y && Result.Prev.OutIdx != Skip) Result = Result.Prev;

                if (Result.Dx == horizontal && Result.Prev.OutIdx != Skip)
                {
                    Horz = Result;
                    while (Horz.Next.Dx == horizontal) Horz = Horz.Next;

                    if (Horz.Next.Top.X == Result.Prev.Top.X || Horz.Next.Top.X > Result.Prev.Top.X) Result = Horz.Next;
                }

                while (E != Result)
                {
                    E.NextInLML = E.Prev;
                    if (E.Dx == horizontal && E != EStart && E.Bot.X != E.Next.Top.X) this.ReverseHorizontal(E);
                    E = E.Prev;
                }

                if (E.Dx == horizontal && E != EStart && E.Bot.X != E.Next.Top.X) this.ReverseHorizontal(E);
                Result = Result.Prev; //move to the edge just beyond current bound
            }

            return Result;
        }

        //------------------------------------------------------------------------------

        public bool AddPath(List<IntPoint> pg, PolyType polyType, bool Closed)
        {
#if use_lines
            if (!Closed && polyType == PolyType.ptClip)
                throw new ClipperException("AddPath: Open paths must be subject.");
#else
      if (!Closed)
        throw new ClipperException("AddPath: Open paths have been disabled.");
#endif

            var highI = (int)pg.Count - 1;
            if (Closed) while (highI > 0 && pg[highI] == pg[0]) --highI;

            while (highI > 0 && pg[highI] == pg[highI - 1]) --highI;

            if (Closed && highI < 2 || !Closed && highI < 1) return false;

            //create a new edge array ...
            var edges = new List<TEdge>(highI + 1);
            for (var i = 0; i <= highI; i++) edges.Add(new TEdge());

            var IsFlat = true;

            //1. Basic (first) edge initialization ...
            edges[1].Curr = pg[1];
            this.RangeTest(pg[0], ref this.m_UseFullRange);
            this.RangeTest(pg[highI], ref this.m_UseFullRange);
            this.InitEdge(edges[0], edges[1], edges[highI], pg[0]);
            this.InitEdge(edges[highI], edges[0], edges[highI - 1], pg[highI]);
            for (var i = highI - 1; i >= 1; --i)
            {
                this.RangeTest(pg[i], ref this.m_UseFullRange);
                this.InitEdge(edges[i], edges[i + 1], edges[i - 1], pg[i]);
            }

            var eStart = edges[0];

            //2. Remove duplicate vertices, and (when closed) collinear edges ...
            TEdge E = eStart, eLoopStop = eStart;
            for (;;)
            {
                //nb: allows matching start and end points when not Closed ...
                if (E.Curr == E.Next.Curr && (Closed || E.Next != eStart))
                {
                    if (E == E.Next) break;

                    if (E == eStart) eStart = E.Next;
                    E = this.RemoveEdge(E);
                    eLoopStop = E;
                    continue;
                }

                if (E.Prev == E.Next) break; //only two vertices
                else if (Closed && SlopesEqual(E.Prev.Curr, E.Curr, E.Next.Curr, this.m_UseFullRange)
                                && (!this.PreserveCollinear || !this.Pt2IsBetweenPt1AndPt3(E.Prev.Curr, E.Curr, E.Next.Curr)))
                {
                    //Collinear edges are allowed for open paths but in closed paths
                    //the default is to merge adjacent collinear edges into a single edge.
                    //However, if the PreserveCollinear property is enabled, only overlapping
                    //collinear edges (ie spikes) will be removed from closed paths.
                    if (E == eStart) eStart = E.Next;
                    E = this.RemoveEdge(E);
                    E = E.Prev;
                    eLoopStop = E;
                    continue;
                }

                E = E.Next;
                if (E == eLoopStop || !Closed && E.Next == eStart) break;
            }

            if (!Closed && E == E.Next || Closed && E.Prev == E.Next) return false;

            if (!Closed)
            {
                this.m_HasOpenPaths = true;
                eStart.Prev.OutIdx = Skip;
            }

            //3. Do second stage of edge initialization ...
            E = eStart;
            do
            {
                this.InitEdge2(E, polyType);
                E = E.Next;
                if (IsFlat && E.Curr.Y != eStart.Curr.Y) IsFlat = false;
            }
            while (E != eStart);

            //4. Finally, add edge bounds to LocalMinima list ...

            //Totally flat paths must be handled differently when adding them
            //to LocalMinima list to avoid endless loops etc ...
            if (IsFlat)
            {
                if (Closed) return false;

                E.Prev.OutIdx = Skip;
                var locMin = new LocalMinima();
                locMin.Next = null;
                locMin.Y = E.Bot.Y;
                locMin.LeftBound = null;
                locMin.RightBound = E;
                locMin.RightBound.Side = EdgeSide.esRight;
                locMin.RightBound.WindDelta = 0;
                for (;;)
                {
                    if (E.Bot.X != E.Prev.Top.X) this.ReverseHorizontal(E);
                    if (E.Next.OutIdx == Skip) break;

                    E.NextInLML = E.Next;
                    E = E.Next;
                }

                this.InsertLocalMinima(locMin);
                this.m_edges.Add(edges);
                return true;
            }

            this.m_edges.Add(edges);
            bool leftBoundIsForward;
            TEdge EMin = null;

            //workaround to avoid an endless loop in the while loop below when
            //open paths have matching start and end points ...
            if (E.Prev.Bot == E.Prev.Top) E = E.Next;

            for (;;)
            {
                E = this.FindNextLocMin(E);
                if (E == EMin) break;
                else if (EMin == null) EMin = E;

                //E and E.Prev now share a local minima (left aligned if horizontal).
                //Compare their slopes to find which starts which bound ...
                var locMin = new LocalMinima();
                locMin.Next = null;
                locMin.Y = E.Bot.Y;
                if (E.Dx < E.Prev.Dx)
                {
                    locMin.LeftBound = E.Prev;
                    locMin.RightBound = E;
                    leftBoundIsForward = false; //Q.nextInLML = Q.prev
                }
                else
                {
                    locMin.LeftBound = E;
                    locMin.RightBound = E.Prev;
                    leftBoundIsForward = true; //Q.nextInLML = Q.next
                }
                locMin.LeftBound.Side = EdgeSide.esLeft;
                locMin.RightBound.Side = EdgeSide.esRight;

                if (!Closed) locMin.LeftBound.WindDelta = 0;
                else if (locMin.LeftBound.Next == locMin.RightBound) locMin.LeftBound.WindDelta = -1;
                else locMin.LeftBound.WindDelta = 1;
                locMin.RightBound.WindDelta = -locMin.LeftBound.WindDelta;

                E = this.ProcessBound(locMin.LeftBound, leftBoundIsForward);
                if (E.OutIdx == Skip) E = this.ProcessBound(E, leftBoundIsForward);

                var E2 = this.ProcessBound(locMin.RightBound, !leftBoundIsForward);
                if (E2.OutIdx == Skip) E2 = this.ProcessBound(E2, !leftBoundIsForward);

                if (locMin.LeftBound.OutIdx == Skip) locMin.LeftBound = null;
                else if (locMin.RightBound.OutIdx == Skip) locMin.RightBound = null;
                this.InsertLocalMinima(locMin);
                if (!leftBoundIsForward) E = E2;
            }

            return true;
        }

        //------------------------------------------------------------------------------

        public bool AddPaths(List<List<IntPoint>> ppg, PolyType polyType, bool closed)
        {
            var result = false;
            for (var i = 0; i < ppg.Count; ++i) if (this.AddPath(ppg[i], polyType, closed)) result = true;

            return result;
        }

        //------------------------------------------------------------------------------

        internal bool Pt2IsBetweenPt1AndPt3(IntPoint pt1, IntPoint pt2, IntPoint pt3)
        {
            if (pt1 == pt3 || pt1 == pt2 || pt3 == pt2) return false;
            else if (pt1.X != pt3.X) return pt2.X > pt1.X == pt2.X < pt3.X;
            else return pt2.Y > pt1.Y == pt2.Y < pt3.Y;
        }

        //------------------------------------------------------------------------------

        private TEdge RemoveEdge(TEdge e)
        {
            //removes e from double_linked_list (but without removing from memory)
            e.Prev.Next = e.Next;
            e.Next.Prev = e.Prev;
            var result = e.Next;
            e.Prev = null; //flag as removed (see ClipperBase.Clear)
            return result;
        }

        //------------------------------------------------------------------------------

        private void SetDx(TEdge e)
        {
            e.Delta.X = e.Top.X - e.Bot.X;
            e.Delta.Y = e.Top.Y - e.Bot.Y;
            if (e.Delta.Y == 0) e.Dx = horizontal;
            else e.Dx = (double)e.Delta.X / e.Delta.Y;
        }

        //---------------------------------------------------------------------------

        private void InsertLocalMinima(LocalMinima newLm)
        {
            if (this.m_MinimaList == null) this.m_MinimaList = newLm;
            else if (newLm.Y >= this.m_MinimaList.Y)
            {
                newLm.Next = this.m_MinimaList;
                this.m_MinimaList = newLm;
            }
            else
            {
                var tmpLm = this.m_MinimaList;
                while (tmpLm.Next != null && newLm.Y < tmpLm.Next.Y) tmpLm = tmpLm.Next;

                newLm.Next = tmpLm.Next;
                tmpLm.Next = newLm;
            }
        }

        //------------------------------------------------------------------------------

        internal bool PopLocalMinima(long Y, out LocalMinima current)
        {
            current = this.m_CurrentLM;
            if (this.m_CurrentLM != null && this.m_CurrentLM.Y == Y)
            {
                this.m_CurrentLM = this.m_CurrentLM.Next;
                return true;
            }

            return false;
        }

        //------------------------------------------------------------------------------

        private void ReverseHorizontal(TEdge e)
        {
            //swap horizontal edges' top and bottom x's so they follow the natural
            //progression of the bounds - ie so their xbots will align with the
            //adjoining lower edge. [Helpful in the ProcessHorizontal() method.]
            this.Swap(ref e.Top.X, ref e.Bot.X);
#if use_xyz
      Swap(ref e.Top.Z, ref e.Bot.Z);
#endif
        }

        //------------------------------------------------------------------------------

        internal virtual void Reset()
        {
            this.m_CurrentLM = this.m_MinimaList;
            if (this.m_CurrentLM == null) return; //ie nothing to process

            //reset all edges ...
            this.m_Scanbeam = null;
            var lm = this.m_MinimaList;
            while (lm != null)
            {
                this.InsertScanbeam(lm.Y);
                var e = lm.LeftBound;
                if (e != null)
                {
                    e.Curr = e.Bot;
                    e.OutIdx = Unassigned;
                }
                e = lm.RightBound;
                if (e != null)
                {
                    e.Curr = e.Bot;
                    e.OutIdx = Unassigned;
                }
                lm = lm.Next;
            }

            this.m_ActiveEdges = null;
        }

        //------------------------------------------------------------------------------

        public static IntRect GetBounds(List<List<IntPoint>> paths)
        {
            int i = 0, cnt = paths.Count;
            while (i < cnt && paths[i].Count == 0) i++;

            if (i == cnt) return new IntRect(0, 0, 0, 0);

            var result = new IntRect();
            result.left = paths[i][0].X;
            result.right = result.left;
            result.top = paths[i][0].Y;
            result.bottom = result.top;
            for (; i < cnt; i++)
            for (var j = 0; j < paths[i].Count; j++)
            {
                if (paths[i][j].X < result.left) result.left = paths[i][j].X;
                else if (paths[i][j].X > result.right) result.right = paths[i][j].X;
                if (paths[i][j].Y < result.top) result.top = paths[i][j].Y;
                else if (paths[i][j].Y > result.bottom) result.bottom = paths[i][j].Y;
            }

            return result;
        }

        //------------------------------------------------------------------------------

        internal void InsertScanbeam(long Y)
        {
            //single-linked list: sorted descending, ignoring dups.
            if (this.m_Scanbeam == null)
            {
                this.m_Scanbeam = new Scanbeam();
                this.m_Scanbeam.Next = null;
                this.m_Scanbeam.Y = Y;
            }
            else if (Y > this.m_Scanbeam.Y)
            {
                var newSb = new Scanbeam();
                newSb.Y = Y;
                newSb.Next = this.m_Scanbeam;
                this.m_Scanbeam = newSb;
            }
            else
            {
                var sb2 = this.m_Scanbeam;
                while (sb2.Next != null && Y <= sb2.Next.Y) sb2 = sb2.Next;

                if (Y == sb2.Y) return; //ie ignores duplicates

                var newSb = new Scanbeam();
                newSb.Y = Y;
                newSb.Next = sb2.Next;
                sb2.Next = newSb;
            }
        }

        //------------------------------------------------------------------------------

        internal bool PopScanbeam(out long Y)
        {
            if (this.m_Scanbeam == null)
            {
                Y = 0;
                return false;
            }

            Y = this.m_Scanbeam.Y;
            this.m_Scanbeam = this.m_Scanbeam.Next;
            return true;
        }

        //------------------------------------------------------------------------------

        internal bool LocalMinimaPending()
        {
            return this.m_CurrentLM != null;
        }

        //------------------------------------------------------------------------------

        internal OutRec CreateOutRec()
        {
            var result = new OutRec();
            result.Idx = Unassigned;
            result.IsHole = false;
            result.IsOpen = false;
            result.FirstLeft = null;
            result.Pts = null;
            result.BottomPt = null;
            result.PolyNode = null;
            this.m_PolyOuts.Add(result);
            result.Idx = this.m_PolyOuts.Count - 1;
            return result;
        }

        //------------------------------------------------------------------------------

        internal void DisposeOutRec(int index)
        {
            var outRec = this.m_PolyOuts[index];
            outRec.Pts = null;
            outRec = null;
            this.m_PolyOuts[index] = null;
        }

        //------------------------------------------------------------------------------

        internal void UpdateEdgeIntoAEL(ref TEdge e)
        {
            if (e.NextInLML == null) throw new ClipperException("UpdateEdgeIntoAEL: invalid call");

            var AelPrev = e.PrevInAEL;
            var AelNext = e.NextInAEL;
            e.NextInLML.OutIdx = e.OutIdx;
            if (AelPrev != null) AelPrev.NextInAEL = e.NextInLML;
            else this.m_ActiveEdges = e.NextInLML;
            if (AelNext != null) AelNext.PrevInAEL = e.NextInLML;
            e.NextInLML.Side = e.Side;
            e.NextInLML.WindDelta = e.WindDelta;
            e.NextInLML.WindCnt = e.WindCnt;
            e.NextInLML.WindCnt2 = e.WindCnt2;
            e = e.NextInLML;
            e.Curr = e.Bot;
            e.PrevInAEL = AelPrev;
            e.NextInAEL = AelNext;
            if (!IsHorizontal(e)) this.InsertScanbeam(e.Top.Y);
        }

        //------------------------------------------------------------------------------

        internal void SwapPositionsInAEL(TEdge edge1, TEdge edge2)
        {
            //check that one or other edge hasn't already been removed from AEL ...
            if (edge1.NextInAEL == edge1.PrevInAEL || edge2.NextInAEL == edge2.PrevInAEL) return;

            if (edge1.NextInAEL == edge2)
            {
                var next = edge2.NextInAEL;
                if (next != null) next.PrevInAEL = edge1;
                var prev = edge1.PrevInAEL;
                if (prev != null) prev.NextInAEL = edge2;
                edge2.PrevInAEL = prev;
                edge2.NextInAEL = edge1;
                edge1.PrevInAEL = edge2;
                edge1.NextInAEL = next;
            }
            else if (edge2.NextInAEL == edge1)
            {
                var next = edge1.NextInAEL;
                if (next != null) next.PrevInAEL = edge2;
                var prev = edge2.PrevInAEL;
                if (prev != null) prev.NextInAEL = edge1;
                edge1.PrevInAEL = prev;
                edge1.NextInAEL = edge2;
                edge2.PrevInAEL = edge1;
                edge2.NextInAEL = next;
            }
            else
            {
                var next = edge1.NextInAEL;
                var prev = edge1.PrevInAEL;
                edge1.NextInAEL = edge2.NextInAEL;
                if (edge1.NextInAEL != null) edge1.NextInAEL.PrevInAEL = edge1;
                edge1.PrevInAEL = edge2.PrevInAEL;
                if (edge1.PrevInAEL != null) edge1.PrevInAEL.NextInAEL = edge1;
                edge2.NextInAEL = next;
                if (edge2.NextInAEL != null) edge2.NextInAEL.PrevInAEL = edge2;
                edge2.PrevInAEL = prev;
                if (edge2.PrevInAEL != null) edge2.PrevInAEL.NextInAEL = edge2;
            }

            if (edge1.PrevInAEL == null) this.m_ActiveEdges = edge1;
            else if (edge2.PrevInAEL == null) this.m_ActiveEdges = edge2;
        }

        //------------------------------------------------------------------------------

        internal void DeleteFromAEL(TEdge e)
        {
            var AelPrev = e.PrevInAEL;
            var AelNext = e.NextInAEL;
            if (AelPrev == null && AelNext == null && e != this.m_ActiveEdges) return; //already deleted

            if (AelPrev != null) AelPrev.NextInAEL = AelNext;
            else this.m_ActiveEdges = AelNext;
            if (AelNext != null) AelNext.PrevInAEL = AelPrev;
            e.NextInAEL = null;
            e.PrevInAEL = null;
        }

        //------------------------------------------------------------------------------
    }
}