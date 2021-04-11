namespace SparkTech.SDK.Entities
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using SharpDX;

    using SparkTech.SDK.API;

    public class Humanizer : IPlayerFragment
    {
        private static readonly IPlayerFragment Fragment;

        private static readonly Random Random;

        static Humanizer()
        {
            Fragment = Platform.CoreFragment?.GetPlayerFragment() ?? throw Platform.FragmentException();
            Random = new Random();
        }

        internal static bool IsEnabled { get; set; }

        internal static int Apm { get; set; }

        private float commandT;

        private static float GetDelay()
        {
            var t = 60f / Apm;
            var multi = Random.Next(8, 12) / 10f;

            return t * multi;
        }

        public static async Task<int> Benchmark()
        {
            var humanizer = new Humanizer();
            var counter = 0;

            Action<EventArgs> benchmark = delegate
            {
                if (humanizer.CanRun())
                {
                    counter++;
                }
            };

            var stopwatch = Stopwatch.StartNew();

            Game.OnUpdate += benchmark;
            await Task.Delay(60 * 1000);
            Game.OnUpdate -= benchmark;

            stopwatch.Stop();

            return counter;
        }

        public T CanRun<T>(Func<T> func)
        {
            var time = Game.Time;

            if (IsEnabled && this.commandT > time)
            {
                return default(T);
            }

            var result = func();

            if (!result.Equals(default(T)))
            {
                this.commandT = time + GetDelay();
            }

            return result;
        }

        public bool CanRun()
        {
            return this.CanRun(() => true);
        }

        public bool IssueOrder(GameObjectOrder order, Vector3 target)
        {
            return this.CanRun(() => Fragment.IssueOrder(order, target));
        }

        public bool IssueOrder(GameObjectOrder order, IAttackable target)
        {
            return this.CanRun(() => Fragment.IssueOrder(order, target));
        }
        
        public bool CastSpell(SpellSlot slot)
        {
            return this.CanRun(() => Fragment.CastSpell(slot));
        }

        public bool CastSpell(SpellSlot slot, IGameObject target)
        {
            return this.CanRun(() => Fragment.CastSpell(slot, target));
        }

        public bool CastSpell(SpellSlot slot, Vector3 position)
        {
            return this.CanRun(() => Fragment.CastSpell(slot, position));
        }

        public bool CastSpell(SpellSlot slot, Vector3 startPosition, Vector3 endPosition)
        {
            return this.CanRun(() => Fragment.CastSpell(slot, startPosition, endPosition));
        }

        public bool UpdateChargedSpell(SpellSlot slot, Vector3 target, bool releaseCast)
        {
            return this.CanRun(() => Fragment.UpdateChargedSpell(slot, target, releaseCast));
        }

        public BuyItemResult BuyItem(ItemId itemId)
        {
            return this.CanRun(() => Fragment.BuyItem(itemId));
        }

        public void SellItem(int slot)
        {
            if (this.CanRun())
            {
                Fragment.SellItem(slot);
            }
        }

        public void LevelSpell(SpellSlot slot)
        {
            if (this.CanRun())
            {
                Fragment.LevelSpell(slot);
            }
        }

        public void EvolveSpell(SpellSlot slot)
        {
            if (this.CanRun())
            {
                Fragment.EvolveSpell(slot);
            }
        }
    }
}