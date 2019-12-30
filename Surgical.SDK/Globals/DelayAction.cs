namespace Surgical.SDK
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Surgical.SDK.Logging;

    public static class DelayAction
    {
        public static void OnLeagueThread(Action action)
        {
            void Callback(EventArgs _)
            {
                Game.OnUpdate -= Callback;
                action();
            }

            Game.OnUpdate += Callback;
        }

        static DelayAction()
        {
            Game.OnUpdate += OnUpdate;
        }

        private static readonly List<DelayActionEntry> DelayActions = new List<DelayActionEntry>();

        private static void OnUpdate(EventArgs _)
        {
            if (DelayActions.Count == 0)
            {
                return;
            }

            var time = Game.Time;

            while (true)
            {
                var i = DelayActions.Count - 1;
                var item = DelayActions[i];

                if (item.ExecuteTime > time)
                {
                    break;
                }

                DelayActions.RemoveAt(i);

                item.Callback();

                if (i == 0)
                {
                    break;
                }
            }
        }

        private static readonly IComparer<DelayActionEntry> DelayActionComparer = new DelayActionEntry.Comparer();

        public static void Add(float time, Action action)
        {
            DelayActions.Add(new DelayActionEntry(time, action));
            DelayActions.Sort(DelayActionComparer);
        }

        private class DelayActionEntry
        {
            public class Comparer : IComparer<DelayActionEntry>
            {
                [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
                int IComparer<DelayActionEntry>.Compare(DelayActionEntry x, DelayActionEntry y)
                {
                    var e1 = x.ExecuteTime;
                    var e2 = y.ExecuteTime;

                    return e1.CompareTo(e2);
                }
            }

            public readonly float ExecuteTime;

            private readonly Action action;

            public void Callback()
            {
                try
                {
                    this.action();
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }

            public DelayActionEntry(float time, Action action)
            {
                this.ExecuteTime = Game.Time + time;
                this.action = action;
            }
        }
    }
}