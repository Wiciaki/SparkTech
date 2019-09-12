namespace Surgical.SDK
{
    using System;
    using System.Collections.Generic;

    using Surgical.SDK.Logging;

    public static class DelayAction
    {
        static DelayAction()
        {
            Game.OnUpdate += OnUpdate;
        }

        private static readonly List<DelayActionEntry> DelayActions = new List<DelayActionEntry>();

        private static void OnUpdate(System.EventArgs _)
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
        
        public static void OnUpdate(Action action)
        {
            void Callback(System.EventArgs _)
            {
                Game.OnUpdate -= Callback;

                action();
            }

            Game.OnUpdate += Callback;
        }

        private class DelayActionEntry
        {
            public class Comparer : IComparer<DelayActionEntry>
            {
                int IComparer<DelayActionEntry>.Compare(DelayActionEntry x, DelayActionEntry y)
                {
                    return y.ExecuteTime.CompareTo(x.ExecuteTime);
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