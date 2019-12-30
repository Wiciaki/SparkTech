namespace Surgical.SDK.Entities
{
    using System.Collections.Generic;

    public class EntityComparer<T> : IEqualityComparer<T> where T : IGameObject
    {
        public bool Equals(T x, T y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return obj.Id;
        }
    }
}