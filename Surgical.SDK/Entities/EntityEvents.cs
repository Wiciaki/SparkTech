namespace Surgical.SDK.Entities
{
    using System;

    using Surgical.SDK.API.Fragments;
    using Surgical.SDK.EventData;

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
            events.PlayAnimation = args => OnPlayAnimation.SafeInvoke(args);
            events.ProcessSpellCast = args => OnProcessSpellCast.SafeInvoke(args);
            events.DoCast = args => OnDoCast.SafeInvoke(args);
            events.NewPath = args => OnNewPath.SafeInvoke(args);
            events.IssueOrder = args => OnIssueOrder.SafeInvoke(args);
            events.Teleport = args => OnTeleport.SafeInvoke(args);
            events.Aggro = args => OnAggro.SafeInvoke(args);
            events.SwapItem = args => OnSwapItem.SafeInvoke(args);
            events.PlaceItemInSlot = args => OnPlaceItemInSlot.SafeInvoke(args);
            events.RemoveItem = args => OnRemoveItem.SafeInvoke(args);
            events.BuffAdd = args => OnBuffAdd.SafeInvoke(args);
            events.BuffRemove = args => OnBuffRemove.SafeInvoke(args);
            events.BuffUpdateCount = args => OnBuffUpdateCount.SafeInvoke(args);
            events.LevelUp = args => OnLevelUp.SafeInvoke(args);
            events.PauseAnimation = args => OnPauseAnimation.SafeInvoke(args);
            events.Target = args => OnTarget.SafeInvoke(args);
            events.SpellbookCastSpell = args => OnSpellbookCastSpell.SafeInvoke(args);
            events.SpellbookStopCast = args => OnSpellbookStopCast.SafeInvoke(args);
            events.SpellbookUpdateChargedSpell = args => OnSpellbookUpdateChargedSpell.SafeInvoke(args);
        }
    }
}