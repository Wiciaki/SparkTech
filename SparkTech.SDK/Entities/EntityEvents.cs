namespace SparkTech.SDK.Entities
{
    using System;

    using SparkTech.SDK.API.Fragments;

    using SparkTech.SDK.Entities.EventArgs;

    public static class EntityEvents
    {
        public static event Action<DamageEventArgs> OnDamage;

        public static event Action<PlayAnimationEventArgs> OnPlayAnimation;

        public static event Action<ProcessCastEventArgs> OnProcessSpellCast;

        public static event Action<ProcessCastEventArgs> OnDoCast;

        public static event Action<NewPathEventArgs> OnNewPath;

        public static event Action<IssueOrderEventArgs> OnIssueOrder;

        public static event Action<TeleportEventArgs> OnTeleport;

        public static event Action<AggroEventArgs> OnAggro;

        public static event Action<SwapItemEventArgs> OnSwapItem;

        public static event Action<PlaceItemInSlotEventArgs> OnPlaceItemInSlot;

        public static event Action<RemoveItemEventArgs> OnRemoveItem;

        public static event Action<BuffUpdateEventArgs> OnBuffAdd;

        public static event Action<BuffUpdateEventArgs> OnBuffRemove;

        public static event Action<BuffUpdateEventArgs> OnBuffUpdateCount;

        public static event Action<LevelUpEventArgs> OnLevelUp;

        public static event Action<PauseAnimationEventArgs> OnPauseAnimation;

        public static event Action<TargetEventArgs> OnTarget;

        public static event Action<CastSpellEventArgs> OnSpellbookCastSpell;
        
        public static event Action<StopCastEventArgs> OnSpellbookStopCast;
        
        public static event Action<UpdateChargedSpellEventArgs> OnSpellbookUpdateChargedSpell;

        internal static void Initialize(IEntityEvents events)
        {
            events.Damage = args => OnDamage.SafeInvoke(args);
        }
    }
}