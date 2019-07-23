namespace SparkTech.Utils
{
    using System;

    using SharpDX;

    public static class Geometry
    {
        #region Public Methods and Operators

        public static IntersectionResult Intersection(
            this Vector2 lineSegment1Start,
            Vector2 lineSegment1End,
            Vector2 lineSegment2Start,
            Vector2 lineSegment2End)
        {
            double deltaACy = lineSegment1Start.Y - lineSegment2Start.Y;
            double deltaDCx = lineSegment2End.X - lineSegment2Start.X;
            double deltaACx = lineSegment1Start.X - lineSegment2Start.X;
            double deltaDCy = lineSegment2End.Y - lineSegment2Start.Y;
            double deltaBAx = lineSegment1End.X - lineSegment1Start.X;
            double deltaBAy = lineSegment1End.Y - lineSegment1Start.Y;

            var denominator = deltaBAx * deltaDCy - deltaBAy * deltaDCx;
            var numerator = deltaACy * deltaDCx - deltaACx * deltaDCy;

            if (Math.Abs(denominator) < float.Epsilon)
            {
                if (Math.Abs(numerator) < float.Epsilon)
                {
                    // collinear. Potentially infinite intersection points.
                    // Check and return one of them.
                    if (lineSegment1Start.X >= lineSegment2Start.X && lineSegment1Start.X <= lineSegment2End.X)
                    {
                        return new IntersectionResult(true, lineSegment1Start);
                    }

                    if (lineSegment2Start.X >= lineSegment1Start.X && lineSegment2Start.X <= lineSegment1End.X)
                    {
                        return new IntersectionResult(true, lineSegment2Start);
                    }

                    return default;
                }

                // parallel
                return default;
            }

            var r = numerator / denominator;
            if (r < 0 || r > 1)
            {
                return default;
            }

            var s = (deltaACy * deltaBAx - deltaACx * deltaBAy) / denominator;
            if (s < 0 || s > 1)
            {
                return default;
            }

            return new IntersectionResult(
                true,
                new Vector2((float)(lineSegment1Start.X + r * deltaBAx), (float)(lineSegment1Start.Y + r * deltaBAy)));
        }

        public static Vector2 Perpendicular(this Vector2 vector2, int offset = 0)
        {
            return offset == 0 ? new Vector2(-vector2.Y, vector2.X) : new Vector2(vector2.Y, -vector2.X);
        }

        public static float Polar(this Vector2 v)
        {
            if (Math.Abs(v.X) < float.Epsilon)
            {
                if (v.Y > 0)
                {
                    return 90;
                }

                return v.Y < 0 ? 270 : 0;
            }

            var theta = Math.Atan(v.Y / v.X) * Math.PI / 180;

            if (v.X < 0)
            {
                theta += 180;
            }
            if (theta < 0)
            {
                theta += 360;
            }

            return (float)theta;
        }

        public static ProjectionInfo ProjectOn(this Vector2 point, Vector2 segmentStart, Vector2 segmentEnd)
        {
            var cx = point.X;
            var cy = point.Y;
            var ax = segmentStart.X;
            var ay = segmentStart.Y;
            var bx = segmentEnd.X;
            var by = segmentEnd.Y;
            var rL = ((cx - ax) * (bx - ax) + (cy - ay) * (by - ay))
                     / ((float)Math.Pow(bx - ax, 2) + (float)Math.Pow(by - ay, 2));
            var pointLine = new Vector2(ax + rL * (bx - ax), ay + rL * (by - ay));
            float rS;
            if (rL < 0)
            {
                rS = 0;
            }
            else if (rL > 1)
            {
                rS = 1;
            }
            else
            {
                rS = rL;
            }

            var isOnSegment = Math.Abs(rS - rL) < float.Epsilon;
            var pointSegment = isOnSegment ? pointLine : new Vector2(ax + rS * (bx - ax), ay + rS * (by - ay));
            return new ProjectionInfo(isOnSegment, pointSegment, pointLine);
        }

        public static Vector2 Rotated(this Vector2 vector2, float angle)
        {
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);

            return new Vector2((float)(vector2.X * cos - vector2.Y * sin), (float)(vector2.Y * cos + vector2.X * sin));
        }

        #endregion

        /// <summary>
        ///     Struct IntersectionResult
        /// </summary>
        public struct IntersectionResult
        {
            #region Fields

            /// <summary>
            ///     Returns if both of the points intersect.
            /// </summary>
            public bool Intersects;

            /// <summary>
            ///     Intersection point
            /// </summary>
            public Vector2 Point;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="IntersectionResult" /> struct.
            ///     Constructor for Intersection Result
            /// </summary>
            /// <param name="intersects">
            ///     Intersection of input
            /// </param>
            /// <param name="point">
            ///     Intersection Point
            /// </param>
            public IntersectionResult(bool intersects = false, Vector2 point = default)
            {
                this.Intersects = intersects;
                this.Point = point;
            }

            #endregion
        }

        public struct MovementCollisionInfo
        {
            #region Fields

            public Vector2 CollisionPosition;

            public float CollisionTime;

            #endregion

            #region Constructors and Destructors

            internal MovementCollisionInfo(float collisionTime, Vector2 collisionPosition)
            {
                this.CollisionTime = collisionTime;
                this.CollisionPosition = collisionPosition;
            }

            #endregion

            #region Public Indexers

            public object this[int i] => i == 0 ? this.CollisionTime : (object)this.CollisionPosition;

            #endregion
        }

        public struct ProjectionInfo
        {
            #region Fields

            /// <summary>
            ///     Returns if the point is on the segment
            /// </summary>
            public bool IsOnSegment;

            /// <summary>
            ///     Line point
            /// </summary>
            public Vector2 LinePoint;

            /// <summary>
            ///     Segment point
            /// </summary>
            public Vector2 SegmentPoint;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="ProjectionInfo" /> struct.
            /// </summary>
            /// <param name="isOnSegment">
            ///     Is on Segment
            /// </param>
            /// <param name="segmentPoint">
            ///     Segment point
            /// </param>
            /// <param name="linePoint">
            ///     Line point
            /// </param>
            internal ProjectionInfo(bool isOnSegment, Vector2 segmentPoint, Vector2 linePoint)
            {
                this.IsOnSegment = isOnSegment;
                this.SegmentPoint = segmentPoint;
                this.LinePoint = linePoint;
            }

            #endregion
        }
    }
}