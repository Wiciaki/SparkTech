namespace SparkTech.SDK.Entities
{
    using SharpDX;

    using SparkTech.SDK.API;

    public class Humanizer : IPlayerFragment
    {
        internal static bool IsEnabled;

        internal static int Apm;

        private static readonly IPlayerFragment Fragment;

        static Humanizer()
        {
            Fragment = Platform.CoreFragment?.GetPlayerFragment() ?? throw Platform.FragmentException();
        }

        public bool IssueOrder(GameObjectOrder order, Vector3 target)
        {
            return Fragment.IssueOrder(order, target);
        }

        public bool IssueOrder(GameObjectOrder order, IAttackable target)
        {
            return Fragment.IssueOrder(order, target);
        }
        
        public bool CastSpell(SpellSlot slot)
        {
            return Fragment.CastSpell(slot);
        }

        public bool CastSpell(SpellSlot slot, IGameObject target)
        {
            return Fragment.CastSpell(slot, target);
        }

        public bool CastSpell(SpellSlot slot, Vector3 position)
        {
            return Fragment.CastSpell(slot, position);
        }

        public bool CastSpell(SpellSlot slot, Vector3 startPosition, Vector3 endPosition)
        {
            return Fragment.CastSpell(slot, startPosition, endPosition);
        }

        public bool UpdateChargedSpell(SpellSlot slot, Vector3 target, bool releaseCast)
        {
            return Fragment.UpdateChargedSpell(slot, target, releaseCast);
        }

        public BuyItemResult BuyItem(ItemId itemId)
        {
            return Fragment.BuyItem(itemId);
        }

        public void SellItem(int slot)
        {
            Fragment.SellItem(slot);
        }

        public void LevelSpell(SpellSlot slot)
        {
            Fragment.LevelSpell(slot);
        }

        public void EvolveSpell(SpellSlot slot)
        {
            Fragment.EvolveSpell(slot);
        }

        /*
         *         private static readonly Random Random = new Random();

        private static Menu menu;

        private float commandT;

        internal static void Initialize(Menu m)
        {
            m.Add(new MenuBool("enable", true));
            m.Add(new MenuInt("apm", 200, 600, 400));

            menu = m;
        }

        private static float GetDelay()
        {
            var apm = menu["apm"].GetValue<int>();
            var t = apm / 60000f;

            Console.WriteLine(t);

            return 0f;
        }

        public bool Execute(Func<bool> func)
        {
            var time = Game.Time;

            if (this.commandT > time && menu["enable"].GetValue<bool>() || !func())
            {
                return false;
            }

            this.commandT = time + GetDelay();
            return true;
        }
        */
    }
}