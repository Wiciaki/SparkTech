namespace SparkTech.TickOperations
{
    using System;
    using System.Linq;

    using SparkTech.Utils;

    public static class GameTick
    {
        internal static int TicksPerSecond = 25;

        public static event GameUpdateDelegate OnUpdate;

        static GameTick()
        {
            Game.OnUpdate += GameOnUpdate;
        }

        private static float lastExecuteTime;

        private static void GameOnUpdate(EntropyEventArgs args)
        {
            if (OnUpdate == null)
            {
                return;
            }

            var time = Game.ClockTime;

            if (time - lastExecuteTime < 1f / TicksPerSecond)
            {
                return;
            }

            lastExecuteTime = time;

            foreach (var callback in OnUpdate.GetInvocationList().Cast<GameUpdateDelegate>().Select(d => new Action(() => d(args))))
            {
                callback.TryExecute("Couldn't invoke a GameTick.OnUpdate callback!");
            }
        }
    }
}