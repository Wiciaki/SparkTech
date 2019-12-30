namespace Surgical.SDK.Entities
{
    using SharpDX;

    using Surgical.SDK.API.Fragments;

    public static class Player
    {
        private static readonly IPlayerFragment Fragment;

        static Player()
        {
            Fragment = Platform.CoreFragment?.GetPlayerFragment() ?? throw Platform.FragmentException();
        }

        public static void IssueOrder(GameObjectOrder order, Vector3 target)
        {
            Fragment.IssueOrder(order, target);
        }

        public static void IssueOrder(GameObjectOrder order, IGameObject target)
        {
            Fragment.IssueOrder(order, target);
        }

        public static void UpdateChargedSpell(SpellSlot slot, Vector3 target)
        {
            Fragment.UpdateChargedSpell(slot, target);
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