namespace SparkTech.SDK.Entities
{
    using SharpDX;

    using SparkTech.SDK.API;

    public static class Player
    {
        private static readonly IPlayerFragment Fragment;

        static Player()
        {
            Fragment = Platform.CoreFragment?.GetPlayerFragment() ?? throw Platform.FragmentException();
        }

        public static int SpellTrainingPoints => Fragment.SpellTrainingPoints;

        public static bool IssueOrder(GameObjectOrder order, Vector3 target)
        {
            return Fragment.IssueOrder(order, target);
        }

        public static bool IssueOrder(GameObjectOrder order, IAttackable target)
        {
            return Fragment.IssueOrder(order, target);
        }
        
        public static bool CastSpell(SpellSlot slot)
        {
            return Fragment.CastSpell(slot);
        }

        public static bool CastSpell(SpellSlot slot, IGameObject target)
        {
            return Fragment.CastSpell(slot, target);
        }

        public static bool CastSpell(SpellSlot slot, Vector3 target)
        {
            return Fragment.CastSpell(slot, target);
        }

        public static bool UpdateChargedSpell(SpellSlot slot, Vector3 target, bool releaseCast)
        {
            return Fragment.UpdateChargedSpell(slot, target, releaseCast);
        }

        public static void LevelSpell(SpellSlot slot)
        {
            Fragment.LevelSpell(slot);
        }

        public static void EvolveSpell(SpellSlot slot)
        {
            Fragment.EvolveSpell(slot);
        }
    }
}