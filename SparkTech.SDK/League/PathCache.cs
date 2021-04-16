namespace SparkTech.SDK.League
{
    using System.Collections.Generic;
    using System.Linq;

    using SharpDX;

    using SparkTech.SDK.Entities;
    using SparkTech.SDK.EventData;

    public static class PathCache
    {
        private static readonly Dictionary<int, List<CachedPath>> Storage = new Dictionary<int, List<CachedPath>>();

        static PathCache() => EntityEvents.OnNewPath += OnNewPath;

        private static void OnNewPath(NewPathEventArgs args)
        {
            var sender = args.Source as IHero;

            if (sender == null || !sender.IsValid)
            {
                return;
            }

            var id = args.SourceId;

            if (!Storage.ContainsKey(id))
            {
                Storage.Add(id, new List<CachedPath>());
            }

            var list = Storage[id];
            list.Add(new CachedPath(args.Path.ToList<Vector3>().ToVector2()));

            if (list.Count > 50)
            {
                list.RemoveRange(0, 40);
            }
        }

        public static CachedPath GetPath(IUnit unit)
        {
            return unit != null && Storage.TryGetValue(unit.Id, out var result) ? result.LastOrDefault() : new CachedPath();
        }

        public class CachedPath
        {
            public List<Vector2> Path { get; }
            public float Time { get; }

            public CachedPath(List<Vector2> path) : this() => this.Path = path;
            public CachedPath() => this.Time = Game.Time;
        }
    }
}
