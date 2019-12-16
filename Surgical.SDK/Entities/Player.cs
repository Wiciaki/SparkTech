namespace Surgical.SDK.Entities
{
    using SharpDX;

    using Surgical.SDK.API.Fragments;

    public static class Player
    {
        private static IPlayerFragment player;

        internal static void Initialize(IPlayerFragment fragment)
        {
            player = fragment;
        }

        public static void IssueOrder(GameObjectOrder order, Vector3 target)
        {
            player.IssueOrder(order, target);
        }

        public static void IssueOrder(GameObjectOrder order, IGameObject target)
        {
            player.IssueOrder(order, target);
        }

        public static void UpdateChargedSpell(SpellSlot slot, Vector3 target)
        {
            player.UpdateChargedSpell(slot, target);
        }

        //bool CastSpell(SpellCastInput args);

        public static void LevelSpell(SpellSlot slot)
        {
            player.LevelSpell(slot);
        }

        public static void EvolveSpell(SpellSlot slot)
        {
            player.EvolveSpell(slot);
        }
    }
}