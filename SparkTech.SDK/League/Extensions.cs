namespace SparkTech.SDK.League
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SharpDX;

    using SparkTech.SDK.Entities;
    using SparkTech.SDK.Rendering;

    public static class Extensions
    {
        public static Vector2 ToVector2(this Vector3 vector) => new Vector2(vector.X, vector.Y);

        public static List<Vector2> ToVector2(this List<Vector3> path)
        {
            return path.ConvertAll(ToVector2);
        }

        public static Vector3 ToVector3(this Vector2 vector, float? value = null)
        {
            return new Vector3(vector.X, vector.Y, value ?? Game.NavMesh.GetHeightForPosition(vector));
        }

        public static Vector3 SetZ(this Vector3 vector, float? value = null)
        {
            vector.Z = value ?? Game.CursorPos.Z;
            return vector;
        }

        public static bool IsValid(this Vector2 vector) => vector != Vector2.Zero;

        public static bool IsValid(this Vector3 vector) => vector != Vector3.Zero;

        public static Vector2 Normalized(this Vector2 vector)
        {
            vector.Normalize();
            return vector;
        }

        public static Vector3 Normalized(this Vector3 vector)
        {
            vector.Normalize();
            return vector;
        }

        public static float Distance(this Vector2 from, Vector2 to)
        {
            return Vector2.Distance(from, to);
        }

        public static float Distance(this Vector3 from, Vector3 to)
        {
            return Vector3.Distance(from, to);
        }

        public static float Distance(this Vector3 from, IGameObject to)
        {
            return from.Distance(to.Position);
        }

        public static float Distance(this IGameObject from, Vector3 to)
        {
            return from.Position.Distance(to);
        }

        public static float Distance(this IGameObject from, IGameObject to)
        {
            return from.Position.Distance(to.Position);
        }

        public static float AngleBetween(this Vector2 vector, Vector2 target)
        {
            var result = vector.Polar() - target.Polar();
            
            if (result < 0f)
            {
                result += 360f;
            }
            if (result > 180f)
            {
                result = 360f - result;
            }

            return result;
        }

        public static float Polar(this Vector2 vector)
        {
            if (MathUtil.IsZero(vector.X))
            {
                if (MathUtil.IsZero(vector.Y))
                {
                    return 0f;
                }

                return vector.Y > 0f ? 90f : 270f;
            }

            float result = (float)(Math.Atan(vector.Y / vector.X) * (180d / Math.PI));
            
            if (vector.X < 0f)
            {
                result += 180f;
            }
            if (result < 0f)
            {
                result += 360f;
            }

            return result;
        }

        public static Vector2 Extend(this Vector2 vector, Vector2 target, float distance)
        {
            return vector + (target - vector).Normalized() * distance;
        }

        public static Vector3 Extend(this Vector3 vector, Vector3 target, float distance)
        {
            return vector + (target - vector).Normalized() * distance;
        }

        public static Vector2 Rotated(this Vector2 vector, float angle)
        {
            var sin = (float)Math.Sin(angle);
            var cos = (float)Math.Cos(angle);

            return new Vector2(vector.X * cos - vector.Y * sin, vector.Y * cos + vector.X * sin);
        }

        [Obsolete]
        public static Vector3 Rotated(this Vector3 vector, float angle)
        {
            var sin = (float)Math.Sin(angle);
            var cos = (float)Math.Cos(angle);

            return new Vector3(vector.X * cos - vector.Y * sin, vector.Y * cos + vector.X * sin, vector.Z);
        }

        public static float PathLength(this IList<Vector3> path)
        {
            var result = 0f;

            for (int i = 0; i < path.Count - 1; ++i)
            {
                result += path[i].Distance(path[i + 1]);
            }

            return result;
        }

        [Obsolete]
        public static float PathLength(this IList<Vector2> path)
        {
            var result = 0f;

            for (int i = 0; i < path.Count - 1; ++i)
            {
                result += path[i].Distance(path[i + 1]);
            }

            return result;
        }

        public static List<Vector2> CutPath(this List<Vector2> path, float distance)
        {
            var results = new List<Vector2>();

            if (distance < 0f)
            {
                path[0] += distance * (path[1] - path[0]).Normalized();
                return path;
            }
            
            for (var i = 0; i < path.Count - 1; i++)
            {
                var num = path[i].Distance(path[i + 1]);

                if (num > distance)
                {
                    results.Add(path[i] + distance * (path[i + 1] - path[i]).Normalized());
                    
                    for (var j = i + 1; j < path.Count; j++)
                    {
                        results.Add(path[j]);
                    }

                    break;
                }

                distance -= num;
            }

            if (results.Count == 0)
            {
                results.Add(path.LastOrDefault());
            }

            return results;
        }

        public static float CrossProduct(this Vector2 self, Vector2 other)
        {
            return other.Y * self.X - other.X * self.Y;
        }

        public static bool IsOnMiniMap(this Vector3 vector, float radius = 0f)
        {
            var minimap = Game.WorldToMinimap(vector);
            return minimap.X + radius > 0f && minimap.Y + radius > 0f;
        }

        public static bool IsOnScreen(this Vector3 vector, float radius = 0f)
        {
            var screen = Game.WorldToScreen(vector);
            return screen.X + radius >= 0f && screen.X - radius <= Render.Width && screen.Y + radius >= 0f && screen.Y - radius <= Render.Height;
        }

        public static bool IsUnderAllyTurret(this Vector3 position)
        {
            return ObjectManager.Get<ITurret>().Any(turret => turret.IsAlly() && turret.Distance(position) < 950f && !turret.IsDead && turret.Health > 1f);
        }

        public static bool IsUnderEnemyTurret(this Vector3 position)
        {
            return ObjectManager.Get<ITurret>().Any(turret => turret.IsEnemy() && turret.Distance(position) < 950f && !turret.IsDead && turret.Health > 1f);
        }

        public static ProjectionInfo ProjectOn(this Vector2 point, Vector2 segmentStart, Vector2 segmentEnd)
        {
            double x1 = (double)point.X;
            float y1 = point.Y;
            float x2 = segmentStart.X;
            float y2 = segmentStart.Y;
            float x3 = segmentEnd.X;
            float y3 = segmentEnd.Y;
            double num1 = (double)x2;
            float num2 = (float)(((x1 - num1) * ((double)x3 - (double)x2) + ((double)y1 - (double)y2) * ((double)y3 - (double)y2)) / (Math.Pow((double)x3 - (double)x2, 2.0) + Math.Pow((double)y3 - (double)y2, 2.0)));
            Vector2 linePoint = new Vector2(x2 + num2 * (x3 - x2), y2 + num2 * (y3 - y2));
            float num3 = (double)num2 >= 0.0 ? ((double)num2 <= 1.0 ? num2 : 1f) : 0.0f;
            int num4 = num3.CompareTo(num2) == 0 ? 1 : 0;
            return new ProjectionInfo(num4 != 0, num4 != 0 ? linePoint : new Vector2(x2 + num3 * (x3 - x2), y2 + num3 * (y3 - y2)), linePoint);
        }

        public static float Distance(this Vector2 point, Vector2 segmentStart, Vector2 segmentEnd, bool onlyIfOnSegment = false)
        {
            var projectionInfo = point.ProjectOn(segmentStart, segmentEnd);
            return !projectionInfo.IsOnSegment && onlyIfOnSegment ? float.MaxValue : Vector2.Distance(projectionInfo.SegmentPoint, point);
        }

        public static MovementCollisionInfo VectorMovementCollision(this Vector2 pointStartA, Vector2 pointEndA, float pointVelocityA, Vector2 pointB, float pointVelocityB, float delay = 0f)
        {
            return new[] { pointStartA, pointEndA }.VectorMovementCollision(
                pointVelocityA,
                pointB,
                pointVelocityB,
                delay);
        }

        public static MovementCollisionInfo VectorMovementCollision(this Vector2[] pointA, float pointVelocityA, Vector2 pointB, float pointVelocityB, float delay = 0f)
        {
            if (pointA.Length < 1)
            {
                return default(MovementCollisionInfo);
            }

            float sP1X = pointA[0].X,
                  sP1Y = pointA[0].Y,
                  eP1X = pointA[1].X,
                  eP1Y = pointA[1].Y,
                  sP2X = pointB.X,
                  sP2Y = pointB.Y;

            float d = eP1X - sP1X, e = eP1Y - sP1Y;
            float dist = (float)Math.Sqrt((d * d) + (e * e)), t1 = float.NaN;
            float s = Math.Abs(dist) > float.Epsilon ? pointVelocityA * d / dist : 0,
                  k = (Math.Abs(dist) > float.Epsilon) ? pointVelocityA * e / dist : 0f;

            float r = sP2X - sP1X, j = sP2Y - sP1Y;
            var c = (r * r) + (j * j);

            if (dist > 0f)
            {
                if (Math.Abs(pointVelocityA - float.MaxValue) < float.Epsilon)
                {
                    var t = dist / pointVelocityA;
                    t1 = pointVelocityB * t >= 0f ? t : float.NaN;
                }
                else if (Math.Abs(pointVelocityB - float.MaxValue) < float.Epsilon)
                {
                    t1 = 0f;
                }
                else
                {
                    float a = (s * s) + (k * k) - (pointVelocityB * pointVelocityB), b = (-r * s) - (j * k);

                    if (Math.Abs(a) < float.Epsilon)
                    {
                        if (Math.Abs(b) < float.Epsilon)
                        {
                            t1 = (Math.Abs(c) < float.Epsilon) ? 0f : float.NaN;
                        }
                        else
                        {
                            var t = -c / (2 * b);
                            t1 = (pointVelocityB * t >= 0f) ? t : float.NaN;
                        }
                    }
                    else
                    {
                        var sqr = (b * b) - (a * c);
                        if (sqr >= 0)
                        {
                            var nom = (float)Math.Sqrt(sqr);
                            var t = (-nom - b) / a;
                            t1 = pointVelocityB * t >= 0f ? t : float.NaN;
                            t = (nom - b) / a;
                            var t2 = (pointVelocityB * t >= 0f) ? t : float.NaN;

                            if (!float.IsNaN(t2) && !float.IsNaN(t1))
                            {
                                if (t1 >= delay && t2 >= delay)
                                {
                                    t1 = Math.Min(t1, t2);
                                }
                                else if (t2 >= delay)
                                {
                                    t1 = t2;
                                }
                            }
                        }
                    }
                }
            }
            else if (Math.Abs(dist) < float.Epsilon)
            {
                t1 = 0f;
            }

            return new MovementCollisionInfo((!float.IsNaN(t1)) ? new Vector2(sP1X + (s * t1), sP1Y + (k * t1)) : default(Vector2), t1);
        }
    }
}