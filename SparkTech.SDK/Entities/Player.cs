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

        public static void IssueOrder(GameObjectOrder order, Vector3 target)
        {
            Fragment.IssueOrder(order, target);
        }

        public static void IssueOrder(GameObjectOrder order, IAttackable target)
        {
            Fragment.IssueOrder(order, target);
        }

        public static void UpdateChargedSpell(SpellSlot slot, Vector3 target, bool releaseCast)
        {
            Fragment.UpdateChargedSpell(slot, target, releaseCast);
        }

        //bool CastSpell(SpellCastInput args);

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