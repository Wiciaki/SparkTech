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

        internal static void Initialize(IEntityEventsFragment fragment)
        {
            fragment.Damage = args => OnDamage.SafeInvoke(args);
            fragment.PlayAnimation = args => OnPlayAnimation.SafeInvoke(args);
            fragment.ProcessSpellCast = args => OnProcessSpellCast.SafeInvoke(args);
            fragment.DoCast = args => OnDoCast.SafeInvoke(args);
            fragment.NewPath = args => OnNewPath.SafeInvoke(args);
            fragment.IssueOrder = args => OnIssueOrder.SafeInvoke(args);
            fragment.Teleport = args => OnTeleport.SafeInvoke(args);
            fragment.Aggro = args => OnAggro.SafeInvoke(args);
            fragment.SwapItem = args => OnSwapItem.SafeInvoke(args);
            fragment.PlaceItemInSlot = args => OnPlaceItemInSlot.SafeInvoke(args);
            fragment.RemoveItem = args => OnRemoveItem.SafeInvoke(args);
            fragment.BuffAdd = args => OnBuffAdd.SafeInvoke(args);
            fragment.BuffRemove = args => OnBuffRemove.SafeInvoke(args);
            fragment.BuffUpdateCount = args => OnBuffUpdateCount.SafeInvoke(args);
            fragment.LevelUp = args => OnLevelUp.SafeInvoke(args);
            fragment.PauseAnimation = args => OnPauseAnimation.SafeInvoke(args);
            fragment.Target = args => OnTarget.SafeInvoke(args);
            fragment.SpellbookCastSpell = args => OnSpellbookCastSpell.SafeInvoke(args);
            fragment.SpellbookStopCast = args => OnSpellbookStopCast.SafeInvoke(args);
            fragment.SpellbookUpdateChargedSpell = args => OnSpellbookUpdateChargedSpell.SafeInvoke(args);
        }
    }
}