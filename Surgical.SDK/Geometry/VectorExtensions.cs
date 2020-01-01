//namespace Surgical.SDK.Geometry
//{
//    public static class VectorExtensions
//    {
//        public static bool IsInside(this Vector2 pos, Vector2 boxStart, Size size)
//        {
//            return pos.X >= boxStart.X && pos.Y >= boxStart.Y && pos.X < boxStart.X + size.Width && pos.Y < boxStart.Y + size.Height;
//        }

//        public static Vector2 Extend(this Vector2 source, Vector2 target, float range)
//        {
//            return source + target * (target - source).Normalized();
//        }

//        public static Vector3 Extend(this Vector3 source, Vector3 target, float range)
//        {
//            return source + target * (target - source).Normalized();
//        }

//        public static Vector3 Extend(this IGameObject source, IGameObject target, float range)
//        {
//            return Extend(source.Position(), target.Position(), range);
//        }

//        public static Vector2 Midpoint(this Vector2 vec1, Vector2 vec2)
//        {
//            return new Vector2((vec1.X + vec2.X) / 2, (vec2.Y + vec2.Y) / 2);
//        }

//        public static Vector3 Midpoint(this Vector3 vec1, Vector3 vec2)
//        {
//            return new Vector3((vec1.X + vec2.X) / 2, (vec1.Y + vec2.Y) / 2, (vec1.Z + vec2.Z) / 2);
//        }

//        public static Vector3 Midpoint(this IGameObject start, IGameObject end)
//        {
//            return Midpoint(start.Position(), end.Position());
//        }

//        public static Vector2 Normalized(this Vector2 vector)
//        {
//            return Vector2.Normalize(vector);
//        }

//        public static Vector3 Normalized(this Vector3 vector)
//        {
//            return Vector3.Normalize(vector);
//        }

//        public static Vector2 PerpendicularAntiClockwise(this Vector2 vector)
//        {
//            return new Vector2(vector.Y, -vector.X);
//        }

//        public static Vector3 PerpendicularAntiClockwise(this Vector3 vector)
//        {
//            var vec2 = vector.To2D().PerpendicularAntiClockwise();
//            return new Vector3(vec2.X, vector.Y, vec2.Y);
//        }

//        public static Vector2 PerpendicularClockwise(this Vector2 vector)
//        {
//            return new Vector2(-vector.Y, vector.X);
//        }

//        public static Vector3 PerpendicularClockwise(this Vector3 vector)
//        {
//            var vec2 = vector.To2D().PerpendicularClockwise();
//            return new Vector3(vec2.X, vector.Y, vec2.Y);
//        }

//        public static float ToDegrees(this float radians)
//        {
//            return radians * 180f * (float)Math.PI;
//        }

//        public static float ToRadians(this float degrees)
//        {
//            return degrees * (float)Math.PI / 180f;
//        }

//        public static Vector3 To3D(this Vector2 vector2)
//        {
//            return new Vector3(vector2.X, NavGrid.GetHeightForPosition(vector2), vector2.Y);
//        }

//        public static Vector2 To2D(this Vector3 vector3)
//        {
//            return new Vector2(vector3.X, vector3.Z);
//        }

//        public static Vector3 ToFlat3D(this Vector2 vector2)
//        {
//            return new Vector3(vector2.X, 0, vector2.Y);
//        }

//        public static Vector3 ToFlat3D(this Vector3 vector3)
//        {
//            return new Vector3(vector3.X, 0, vector3.Y);
//        }

//        public static bool IsBuilding(this Vector2 vector2)
//        {
//            return new NavGridCell(vector2).CollFlags.HasFlag(CollisionFlags.Building);
//        }

//        public static bool IsBuilding(this Vector3 vector3)
//        {
//            return new NavGridCell(vector3).CollFlags.HasFlag(CollisionFlags.Building);
//        }

//        public static bool IsGrass(this Vector2 vector2)
//        {
//            return new NavGridCell(vector2).CollFlags.HasFlag(CollisionFlags.Grass);
//        }

//        public static bool IsGrass(this Vector3 vector3)
//        {
//            return new NavGridCell(vector3).CollFlags.HasFlag(CollisionFlags.Grass);
//        }

//        public static bool IsWall(this Vector2 vector2)
//        {
//            return new NavGridCell(vector2).CollFlags.HasFlag(CollisionFlags.Wall | CollisionFlags.Building);
//        }

//        public static bool IsWall(this Vector3 vector3)
//        {
//            return new NavGridCell(vector3).CollFlags.HasFlag(CollisionFlags.Wall | CollisionFlags.Building);
//        }

//        public static float Distance(this Vector2 pos1, Vector2 pos2)
//        {
//            return Vector2.Distance(pos1, pos2);
//        }

//        public static float Distance(this Vector3 pos1, Vector3 pos2)
//        {
//            return Vector3.Distance(pos1, pos2);
//        }

//        public static float Distance(this Vector3 pos, IGameObject o)
//        {
//            return Vector3.Distance(pos, o.Position());
//        }

//        public static float Distance(this IGameObject from, Vector3 to)
//        {
//            return Vector3.Distance(from.Position(), to);
//        }

//        public static float Distance(this IGameObject o1, IGameObject o2)
//        {
//            return Vector3.Distance(o1.Position(), o2.Position());
//        }

//        public static bool IsInRange(this Vector3 pos1, Vector3 pos2, float range)
//        {
//            return pos1.Distance(pos2) <= range;
//        }

//        public static bool IsInRange(this IGameObject o1, IGameObject o2, float range)
//        {
//            return IsInRange(o1.Position(), o2.Position(), range);
//        }

//        public static bool IsInRange(this IGameObject source, Vector3 target, float range)
//        {
//            return IsInRange(source.Position(), target, range);
//        }

//        public static bool IsInRange(this Vector3 source, IGameObject target, float range)
//        {
//            return IsInRange(source, target.Position(), range);
//        }
//    }
//}