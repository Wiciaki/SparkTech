namespace SparkTech.SDK.Clipper
{
    using System;
    using System.Collections.Generic;

    public class ClipperOffset
    {
        #region Constants

        private const double def_arc_tolerance = 0.25;

        private const double two_pi = Math.PI * 2;

        #endregion

        #region Fields

        private readonly List<DoublePoint> m_normals = new List<DoublePoint>();

        private readonly PolyNode m_polyNodes = new PolyNode();

        private double m_delta, m_sinA, m_sin, m_cos;

        private List<IntPoint> m_destPoly;

        private List<List<IntPoint>> m_destPolys;

        private IntPoint m_lowest;

        private double m_miterLim, m_StepsPerRad;

        private List<IntPoint> m_srcPoly;

        #endregion

        #region Constructors and Destructors

        public ClipperOffset(double miterLimit = 2.0, double arcTolerance = def_arc_tolerance)
        {
            this.MiterLimit = miterLimit;
            this.ArcTolerance = arcTolerance;
            this.m_lowest.X = -1;
        }

        #endregion

        #region Public Properties

        public double ArcTolerance { get; set; }

        public double MiterLimit { get; set; }

        #endregion

        #region Public Methods and Operators

        //------------------------------------------------------------------------------

        public void AddPath(List<IntPoint> path, JoinType joinType, EndType endType)
        {
            var highI = path.Count - 1;
            if (highI < 0) return;

            var newNode = new PolyNode();
            newNode.m_jointype = joinType;
            newNode.m_endtype = endType;

            //strip duplicate points from path and also get index to the lowest point ...
            if (endType == EndType.etClosedLine || endType == EndType.etClosedPolygon)
                while (highI > 0 && path[0] == path[highI]) highI--;

            newNode.m_polygon.Capacity = highI + 1;
            newNode.m_polygon.Add(path[0]);
            int j = 0, k = 0;
            for (var i = 1; i <= highI; i++)
                if (newNode.m_polygon[j] != path[i])
                {
                    j++;
                    newNode.m_polygon.Add(path[i]);
                    if (path[i].Y > newNode.m_polygon[k].Y
                        || path[i].Y == newNode.m_polygon[k].Y && path[i].X < newNode.m_polygon[k].X) k = j;
                }

            if (endType == EndType.etClosedPolygon && j < 2) return;

            this.m_polyNodes.AddChild(newNode);

            //if this path's lowest pt is lower than all the others then update m_lowest
            if (endType != EndType.etClosedPolygon) return;

            if (this.m_lowest.X < 0) this.m_lowest = new IntPoint(this.m_polyNodes.ChildCount - 1, k);
            else
            {
                var ip = this.m_polyNodes.Childs[(int)this.m_lowest.X].m_polygon[(int)this.m_lowest.Y];
                if (newNode.m_polygon[k].Y > ip.Y || newNode.m_polygon[k].Y == ip.Y && newNode.m_polygon[k].X < ip.X)
                    this.m_lowest = new IntPoint(this.m_polyNodes.ChildCount - 1, k);
            }
        }

        //------------------------------------------------------------------------------

        public void AddPaths(List<List<IntPoint>> paths, JoinType joinType, EndType endType)
        {
            foreach (var p in paths)
            {
                this.AddPath(p, joinType, endType);
            }
        }

        //------------------------------------------------------------------------------

        public void Clear()
        {
            this.m_polyNodes.Childs.Clear();
            this.m_lowest.X = -1;
        }

        //------------------------------------------------------------------------------

        public void Execute(ref List<List<IntPoint>> solution, double delta)
        {
            solution.Clear();
            this.FixOrientations();
            this.DoOffset(delta);

            //now clean up 'corners' ...
            var clpr = new Clipper();
            clpr.AddPaths(this.m_destPolys, PolyType.ptSubject, true);
            if (delta > 0)
            {
                clpr.Execute(ClipType.ctUnion, solution, PolyFillType.pftPositive, PolyFillType.pftPositive);
            }
            else
            {
                var r = ClipperBase.GetBounds(this.m_destPolys);
                var outer = new List<IntPoint>(4);

                outer.Add(new IntPoint(r.left - 10, r.bottom + 10));
                outer.Add(new IntPoint(r.right + 10, r.bottom + 10));
                outer.Add(new IntPoint(r.right + 10, r.top - 10));
                outer.Add(new IntPoint(r.left - 10, r.top - 10));

                clpr.AddPath(outer, PolyType.ptSubject, true);
                clpr.ReverseSolution = true;
                clpr.Execute(ClipType.ctUnion, solution, PolyFillType.pftNegative, PolyFillType.pftNegative);
                if (solution.Count > 0) solution.RemoveAt(0);
            }
        }

        //------------------------------------------------------------------------------

        public void Execute(ref PolyTree solution, double delta)
        {
            solution.Clear();
            this.FixOrientations();
            this.DoOffset(delta);

            //now clean up 'corners' ...
            var clpr = new Clipper();
            clpr.AddPaths(this.m_destPolys, PolyType.ptSubject, true);
            if (delta > 0)
            {
                clpr.Execute(ClipType.ctUnion, solution, PolyFillType.pftPositive, PolyFillType.pftPositive);
            }
            else
            {
                var r = ClipperBase.GetBounds(this.m_destPolys);
                var outer = new List<IntPoint>(4);

                outer.Add(new IntPoint(r.left - 10, r.bottom + 10));
                outer.Add(new IntPoint(r.right + 10, r.bottom + 10));
                outer.Add(new IntPoint(r.right + 10, r.top - 10));
                outer.Add(new IntPoint(r.left - 10, r.top - 10));

                clpr.AddPath(outer, PolyType.ptSubject, true);
                clpr.ReverseSolution = true;
                clpr.Execute(ClipType.ctUnion, solution, PolyFillType.pftNegative, PolyFillType.pftNegative);

                //remove the outer PolyNode rectangle ...
                if (solution.ChildCount == 1 && solution.Childs[0].ChildCount > 0)
                {
                    var outerNode = solution.Childs[0];
                    solution.Childs.Capacity = outerNode.ChildCount;
                    solution.Childs[0] = outerNode.Childs[0];
                    solution.Childs[0].m_Parent = solution;
                    for (var i = 1; i < outerNode.ChildCount; i++) solution.AddChild(outerNode.Childs[i]);
                }
                else solution.Clear();
            }
        }

        #endregion

        #region Methods

        //------------------------------------------------------------------------------

        internal static DoublePoint GetUnitNormal(IntPoint pt1, IntPoint pt2)
        {
            double dx = pt2.X - pt1.X;
            double dy = pt2.Y - pt1.Y;
            if (dx == 0 && dy == 0) return new DoublePoint();

            var f = 1 * 1.0 / Math.Sqrt(dx * dx + dy * dy);
            dx *= f;
            dy *= f;

            return new DoublePoint(dy, -dx);
        }

        //------------------------------------------------------------------------------

        internal static long Round(double value)
        {
            return value < 0 ? (long)(value - 0.5) : (long)(value + 0.5);
        }

        //------------------------------------------------------------------------------

        internal void DoMiter(int j, int k, double r)
        {
            var q = this.m_delta / r;
            this.m_destPoly.Add(
                new IntPoint(
                    Round(this.m_srcPoly[j].X + (this.m_normals[k].X + this.m_normals[j].X) * q),
                    Round(this.m_srcPoly[j].Y + (this.m_normals[k].Y + this.m_normals[j].Y) * q)));
        }

        //------------------------------------------------------------------------------

        internal void DoRound(int j, int k)
        {
            var a = Math.Atan2(
                this.m_sinA,
                this.m_normals[k].X * this.m_normals[j].X + this.m_normals[k].Y * this.m_normals[j].Y);
            var steps = Math.Max((int)Round(this.m_StepsPerRad * Math.Abs(a)), 1);

            double X = this.m_normals[k].X, Y = this.m_normals[k].Y, X2;
            for (var i = 0; i < steps; ++i)
            {
                this.m_destPoly.Add(
                    new IntPoint(
                        Round(this.m_srcPoly[j].X + X * this.m_delta),
                        Round(this.m_srcPoly[j].Y + Y * this.m_delta)));
                X2 = X;
                X = X * this.m_cos - this.m_sin * Y;
                Y = X2 * this.m_sin + Y * this.m_cos;
            }

            this.m_destPoly.Add(
                new IntPoint(
                    Round(this.m_srcPoly[j].X + this.m_normals[j].X * this.m_delta),
                    Round(this.m_srcPoly[j].Y + this.m_normals[j].Y * this.m_delta)));
        }

        //------------------------------------------------------------------------------

        internal void DoSquare(int j, int k)
        {
            var dx = Math.Tan(
                Math.Atan2(
                    this.m_sinA,
                    this.m_normals[k].X * this.m_normals[j].X + this.m_normals[k].Y * this.m_normals[j].Y) / 4);
            this.m_destPoly.Add(
                new IntPoint(
                    Round(this.m_srcPoly[j].X + this.m_delta * (this.m_normals[k].X - this.m_normals[k].Y * dx)),
                    Round(this.m_srcPoly[j].Y + this.m_delta * (this.m_normals[k].Y + this.m_normals[k].X * dx))));
            this.m_destPoly.Add(
                new IntPoint(
                    Round(this.m_srcPoly[j].X + this.m_delta * (this.m_normals[j].X + this.m_normals[j].Y * dx)),
                    Round(this.m_srcPoly[j].Y + this.m_delta * (this.m_normals[j].Y - this.m_normals[j].X * dx))));
        }

        //------------------------------------------------------------------------------

        private void DoOffset(double delta)
        {
            this.m_destPolys = new List<List<IntPoint>>();
            this.m_delta = delta;

            //if Zero offset, just copy any CLOSED polygons to m_p and return ...
            if (ClipperBase.near_zero(delta))
            {
                this.m_destPolys.Capacity = this.m_polyNodes.ChildCount;
                for (var i = 0; i < this.m_polyNodes.ChildCount; i++)
                {
                    var node = this.m_polyNodes.Childs[i];
                    if (node.m_endtype == EndType.etClosedPolygon) this.m_destPolys.Add(node.m_polygon);
                }

                return;
            }

            //see offset_triginometry3.svg in the documentation folder ...
            if (this.MiterLimit > 2) this.m_miterLim = 2 / (this.MiterLimit * this.MiterLimit);
            else this.m_miterLim = 0.5;

            double y;
            if (this.ArcTolerance <= 0.0) y = def_arc_tolerance;
            else if (this.ArcTolerance > Math.Abs(delta) * def_arc_tolerance) y = Math.Abs(delta) * def_arc_tolerance;
            else y = this.ArcTolerance;

            //see offset_triginometry2.svg in the documentation folder ...
            var steps = Math.PI / Math.Acos(1 - y / Math.Abs(delta));
            this.m_sin = Math.Sin(two_pi / steps);
            this.m_cos = Math.Cos(two_pi / steps);
            this.m_StepsPerRad = steps / two_pi;
            if (delta < 0.0) this.m_sin = -this.m_sin;

            this.m_destPolys.Capacity = this.m_polyNodes.ChildCount * 2;
            for (var i = 0; i < this.m_polyNodes.ChildCount; i++)
            {
                var node = this.m_polyNodes.Childs[i];
                this.m_srcPoly = node.m_polygon;

                var len = this.m_srcPoly.Count;

                if (len == 0 || delta <= 0 && (len < 3 || node.m_endtype != EndType.etClosedPolygon)) continue;

                this.m_destPoly = new List<IntPoint>();

                if (len == 1)
                {
                    if (node.m_jointype == JoinType.jtRound)
                    {
                        double X = 1.0, Y = 0.0;
                        for (var j = 1; j <= steps; j++)
                        {
                            this.m_destPoly.Add(
                                new IntPoint(
                                    Round(this.m_srcPoly[0].X + X * delta),
                                    Round(this.m_srcPoly[0].Y + Y * delta)));
                            var X2 = X;
                            X = X * this.m_cos - this.m_sin * Y;
                            Y = X2 * this.m_sin + Y * this.m_cos;
                        }
                    }
                    else
                    {
                        double X = -1.0, Y = -1.0;
                        for (var j = 0; j < 4; ++j)
                        {
                            this.m_destPoly.Add(
                                new IntPoint(
                                    Round(this.m_srcPoly[0].X + X * delta),
                                    Round(this.m_srcPoly[0].Y + Y * delta)));
                            if (X < 0) X = 1;
                            else if (Y < 0) Y = 1;
                            else X = -1;
                        }
                    }

                    this.m_destPolys.Add(this.m_destPoly);
                    continue;
                }

                //build m_normals ...
                this.m_normals.Clear();
                this.m_normals.Capacity = len;
                for (var j = 0; j < len - 1; j++)
                    this.m_normals.Add(GetUnitNormal(this.m_srcPoly[j], this.m_srcPoly[j + 1]));

                if (node.m_endtype == EndType.etClosedLine || node.m_endtype == EndType.etClosedPolygon)
                    this.m_normals.Add(GetUnitNormal(this.m_srcPoly[len - 1], this.m_srcPoly[0]));
                else this.m_normals.Add(new DoublePoint(this.m_normals[len - 2]));

                if (node.m_endtype == EndType.etClosedPolygon)
                {
                    var k = len - 1;
                    for (var j = 0; j < len; j++) this.OffsetPoint(j, ref k, node.m_jointype);

                    this.m_destPolys.Add(this.m_destPoly);
                }
                else if (node.m_endtype == EndType.etClosedLine)
                {
                    var k = len - 1;
                    for (var j = 0; j < len; j++) this.OffsetPoint(j, ref k, node.m_jointype);

                    this.m_destPolys.Add(this.m_destPoly);
                    this.m_destPoly = new List<IntPoint>();

                    //re-build m_normals ...
                    var n = this.m_normals[len - 1];
                    for (var j = len - 1; j > 0; j--)
                        this.m_normals[j] = new DoublePoint(-this.m_normals[j - 1].X, -this.m_normals[j - 1].Y);

                    this.m_normals[0] = new DoublePoint(-n.X, -n.Y);
                    k = 0;
                    for (var j = len - 1; j >= 0; j--) this.OffsetPoint(j, ref k, node.m_jointype);

                    this.m_destPolys.Add(this.m_destPoly);
                }
                else
                {
                    var k = 0;
                    for (var j = 1; j < len - 1; ++j) this.OffsetPoint(j, ref k, node.m_jointype);

                    IntPoint pt1;
                    if (node.m_endtype == EndType.etOpenButt)
                    {
                        var j = len - 1;
                        pt1 = new IntPoint(
                            (long)Round(this.m_srcPoly[j].X + this.m_normals[j].X * delta),
                            (long)Round(this.m_srcPoly[j].Y + this.m_normals[j].Y * delta));
                        this.m_destPoly.Add(pt1);
                        pt1 = new IntPoint(
                            (long)Round(this.m_srcPoly[j].X - this.m_normals[j].X * delta),
                            (long)Round(this.m_srcPoly[j].Y - this.m_normals[j].Y * delta));
                        this.m_destPoly.Add(pt1);
                    }
                    else
                    {
                        var j = len - 1;
                        k = len - 2;
                        this.m_sinA = 0;
                        this.m_normals[j] = new DoublePoint(-this.m_normals[j].X, -this.m_normals[j].Y);
                        if (node.m_endtype == EndType.etOpenSquare) this.DoSquare(j, k);
                        else this.DoRound(j, k);
                    }

                    //re-build m_normals ...
                    for (var j = len - 1; j > 0; j--)
                        this.m_normals[j] = new DoublePoint(-this.m_normals[j - 1].X, -this.m_normals[j - 1].Y);

                    this.m_normals[0] = new DoublePoint(-this.m_normals[1].X, -this.m_normals[1].Y);

                    k = len - 1;
                    for (var j = k - 1; j > 0; --j) this.OffsetPoint(j, ref k, node.m_jointype);

                    if (node.m_endtype == EndType.etOpenButt)
                    {
                        pt1 = new IntPoint(
                            (long)Round(this.m_srcPoly[0].X - this.m_normals[0].X * delta),
                            (long)Round(this.m_srcPoly[0].Y - this.m_normals[0].Y * delta));
                        this.m_destPoly.Add(pt1);
                        pt1 = new IntPoint(
                            (long)Round(this.m_srcPoly[0].X + this.m_normals[0].X * delta),
                            (long)Round(this.m_srcPoly[0].Y + this.m_normals[0].Y * delta));
                        this.m_destPoly.Add(pt1);
                    }
                    else
                    {
                        k = 1;
                        this.m_sinA = 0;
                        if (node.m_endtype == EndType.etOpenSquare) this.DoSquare(0, 1);
                        else this.DoRound(0, 1);
                    }
                    this.m_destPolys.Add(this.m_destPoly);
                }
            }
        }

        //------------------------------------------------------------------------------

        private void FixOrientations()
        {
            //fixup orientations of all closed paths if the orientation of the
            //closed path with the lowermost vertex is wrong ...
            if (this.m_lowest.X >= 0 && !Clipper.Orientation(this.m_polyNodes.Childs[(int)this.m_lowest.X].m_polygon))
            {
                for (var i = 0; i < this.m_polyNodes.ChildCount; i++)
                {
                    var node = this.m_polyNodes.Childs[i];
                    if (node.m_endtype == EndType.etClosedPolygon || node.m_endtype == EndType.etClosedLine
                        && Clipper.Orientation(node.m_polygon)) node.m_polygon.Reverse();
                }
            }
            else
            {
                for (var i = 0; i < this.m_polyNodes.ChildCount; i++)
                {
                    var node = this.m_polyNodes.Childs[i];
                    if (node.m_endtype == EndType.etClosedLine && !Clipper.Orientation(node.m_polygon))
                        node.m_polygon.Reverse();
                }
            }
        }

        //------------------------------------------------------------------------------

        private void OffsetPoint(int j, ref int k, JoinType jointype)
        {
            //cross product ...
            this.m_sinA = this.m_normals[k].X * this.m_normals[j].Y - this.m_normals[j].X * this.m_normals[k].Y;

            if (Math.Abs(this.m_sinA * this.m_delta) < 1.0)
            {
                //dot product ...
                var cosA = this.m_normals[k].X * this.m_normals[j].X + this.m_normals[j].Y * this.m_normals[k].Y;
                if (cosA > 0) // angle ==> 0 degrees
                {
                    this.m_destPoly.Add(
                        new IntPoint(
                            Round(this.m_srcPoly[j].X + this.m_normals[k].X * this.m_delta),
                            Round(this.m_srcPoly[j].Y + this.m_normals[k].Y * this.m_delta)));
                    return;
                }

                //else angle ==> 180 degrees
            }
            else if (this.m_sinA > 1.0) this.m_sinA = 1.0;
            else if (this.m_sinA < -1.0) this.m_sinA = -1.0;

            if (this.m_sinA * this.m_delta < 0)
            {
                this.m_destPoly.Add(
                    new IntPoint(
                        Round(this.m_srcPoly[j].X + this.m_normals[k].X * this.m_delta),
                        Round(this.m_srcPoly[j].Y + this.m_normals[k].Y * this.m_delta)));
                this.m_destPoly.Add(this.m_srcPoly[j]);
                this.m_destPoly.Add(
                    new IntPoint(
                        Round(this.m_srcPoly[j].X + this.m_normals[j].X * this.m_delta),
                        Round(this.m_srcPoly[j].Y + this.m_normals[j].Y * this.m_delta)));
            }
            else
            {
                switch (jointype)
                {
                    case JoinType.jtMiter:
                    {
                        var r = 1 + (this.m_normals[j].X * this.m_normals[k].X
                                     + this.m_normals[j].Y * this.m_normals[k].Y);
                        if (r >= this.m_miterLim) this.DoMiter(j, k, r);
                        else this.DoSquare(j, k);
                        break;
                    }
                    case JoinType.jtSquare:
                        this.DoSquare(j, k);
                        break;
                    case JoinType.jtRound:
                        this.DoRound(j, k);
                        break;
                }
            }

            k = j;
        }

        #endregion

        //------------------------------------------------------------------------------
    }
}