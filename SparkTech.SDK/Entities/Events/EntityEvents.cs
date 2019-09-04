namespace SparkTech.SDK.Entities.Events
{
    using System;

    using SparkTech.SDK.API.Fragments;

    public static class EntityEvents
    {
        public static event Action<OnDamageEventArgs> OnDamage;

        internal static void Initialize(IEntityEvents events)
        {
            events.Damage = args => OnDamage.SafeInvoke(args);
        }
    }
}