namespace SparkTech.SDK.Entities
{
    using System;

    using SparkTech.SDK.EventData;

    public static class EntityEvents
    {
        public static event Action<NotifyEventArgs> OnNotify;

        public static event Action<PlayAnimationEventArgs> OnPlayAnimation;

        public static event Action<ProcessSpellCastEventArgs> OnProcessSpellCast;

        public static event Action<ProcessSpellCastEventArgs> OnDoCast;

        public static event Action<NewPathEventArgs> OnNewPath;

        public static event Action<IssueOrderEventArgs> OnIssueOrder;

        public static event Action<TeleportEventArgs> OnTeleport;

        public static event Action<AggroEventArgs> OnAggro;

        public static event Action<BuffUpdateEventArgs> OnBuffAdd;

        public static event Action<BuffUpdateEventArgs> OnBuffRemove;

        public static event Action<LevelUpEventArgs> OnLevelUp;

        public static event Action<CastSpellEventArgs> OnSpellbookCastSpell;
        
        public static event Action<StopCastEventArgs> OnSpellbookStopCast;
        
        public static event Action<UpdateChargedSpellEventArgs> OnSpellbookUpdateChargedSpell;

        public static event Action<PropertyChangeEventArgs<int>> OnIntegerPropertyChange;

        public static event Action<PropertyChangeEventArgs<float>> OnFloatPropertyChange;

        static EntityEvents()
        {
            var fragment = Platform.CoreFragment?.GetEntityEventsFragment() ?? throw Platform.FragmentException();

            fragment.Notify = args => OnNotify.SafeInvoke(args);
            fragment.PlayAnimation = args => OnPlayAnimation.SafeInvoke(args);
            fragment.ProcessSpellCast = args => OnProcessSpellCast.SafeInvoke(args);
            fragment.DoCast = args => OnDoCast.SafeInvoke(args);
            fragment.NewPath = args => OnNewPath.SafeInvoke(args);
            fragment.IssueOrder = args => OnIssueOrder.SafeInvoke(args);
            fragment.Teleport = args => OnTeleport.SafeInvoke(args);
            fragment.Aggro = args => OnAggro.SafeInvoke(args);
            fragment.BuffAdd = args => OnBuffAdd.SafeInvoke(args);
            fragment.BuffRemove = args => OnBuffRemove.SafeInvoke(args);
            fragment.LevelUp = args => OnLevelUp.SafeInvoke(args);
            fragment.SpellbookCastSpell = args => OnSpellbookCastSpell.SafeInvoke(args);
            fragment.SpellbookStopCast = args => OnSpellbookStopCast.SafeInvoke(args);
            fragment.SpellbookUpdateChargedSpell = args => OnSpellbookUpdateChargedSpell.SafeInvoke(args);
            fragment.OnIntegerPropertyChange = args => OnIntegerPropertyChange.SafeInvoke(args);
            fragment.OnFloatPropertyChange = args => OnFloatPropertyChange.SafeInvoke(args);
        }
    }
}