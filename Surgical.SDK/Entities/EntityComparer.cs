namespace Surgical.SDK.Entities
{
    using System.Collections.Generic;

    public class EntityComparer<T> : IEqualityComparer<T> where T : IGameObject
    {
        public bool Equals(T x, T y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            return x != null && y != null && this.GetHashCode(x) == this.GetHashCode(y);
        }

        public int GetHashCode(T obj)
        {
            return obj.Id;
        }
    }
}