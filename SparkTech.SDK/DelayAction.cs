namespace SparkTech.SDK
{
    using System;
    using System.Collections.Generic;

    using SparkTech.SDK.Logging;

    public static class DelayAction
    {
        private static readonly List<Action> OnStartHandlers = new List<Action>();

        public static event Action OnGameStart
        {
            add
            {
                if (Game.State == GameState.Running)
                {
                    RunCallback(value);
                }
                else
                {
                    OnStartHandlers.Add(value);
                }
            }
            remove
            {
                OnStartHandlers.Remove(value);
            }
        }

        private static void RunCallback(Action callback)
        {
            try
            {
                callback();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        static DelayAction()
        {
            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (OnStartHandlers.Count != 0 && Game.State == GameState.Running)
            {
                OnStartHandlers.ForEach(RunCallback);
                OnStartHandlers.Clear();
            }

            if (DelayActions.Count == 0)
            {
                return;
            }

            var time = Game.Time;

            for (var i = DelayActions.Count - 1; i >= 0; i--)
            {
                var item = DelayActions[i];

                if (item.ExecuteTime > time)
                    break;

                DelayActions.RemoveAt(i);
                RunCallback(item.Callback);
            }
        }

        private static readonly List<DelayActionEntry> DelayActions = new List<DelayActionEntry>();

        private static readonly IComparer<DelayActionEntry> DelayActionComparer = new DelayActionEntry.Comparer();

        public static void Add(float time, Action action)
        {
            DelayActions.Add(new DelayActionEntry(time, action));
            DelayActions.Sort(DelayActionComparer);
        }

        public static void OnLeagueThread(Action action)
        {
            void Callback(EventArgs args)
            {
                Game.OnUpdate -= Callback;
                action();
            }

            Game.OnUpdate += Callback;
        }

        private class DelayActionEntry
        {
            public readonly float ExecuteTime;

            public readonly Action Callback;

            public DelayActionEntry(float time, Action callback)
            {
                this.ExecuteTime = Game.Time + time;
                this.Callback = callback;
            }

            public class Comparer : IComparer<DelayActionEntry>
            {
                public int Compare(DelayActionEntry x, DelayActionEntry y)
                {
                    var t1 = x.ExecuteTime;
                    var t2 = y.ExecuteTime;

                    return t1.CompareTo(t2);
                }
            }
        }
    }
}