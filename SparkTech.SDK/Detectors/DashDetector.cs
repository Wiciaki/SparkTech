namespace SparkTech.SDK.Detectors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SparkTech.SDK.Entities;
    using SparkTech.SDK.EventData;
    using SparkTech.SDK.League;

    public static class DashDetector
    {
        static DashDetector() => EntityEvents.OnNewPath += OnNewPath;

        private static readonly Dictionary<int, DashEventArgs> Storage = new Dictionary<int, DashEventArgs>();

        public static event Action<DashEventArgs> OnDash;

        public static DashEventArgs GetDash(this IUnit unit)
        {
            return unit is IHero hero && Storage.TryGetValue(hero.Id, out var args) && args.EndTime >= Game.Time && hero.Path.Length > 0 ? args : null;
        }

        public static bool IsDashing(this IUnit unit)
        {
            return unit.GetDash() != null;
        }

        private static void OnNewPath(NewPathEventArgs args)
        {
            if (!args.IsDash)
            {
                Storage.Remove(args.SourceId);
                return;
            }

            if (args.Source is IHero hero)
            {
                var startPos = hero.Position;
                var endPos = args.Path.Last();
                var startTime = Game.Time;
                var endTime = startTime + startPos.Distance(endPos) / args.Speed;

                var dash = new DashEventArgs(args.SourceId, args.Path, args.Speed, startPos, endPos, startTime, endTime);
                
                Storage[args.SourceId] = dash;
                OnDash?.SafeInvoke(dash);
            }
        }
    }
}