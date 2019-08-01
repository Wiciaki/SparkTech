namespace SparkTech.SDK.Entities
{
    using System;

    using SparkTech.SDK.Misc;
    using SparkTech.SDK.Platform.API;

    public static class EntityEvents
    {
        public static event Action<OnDamageEventArgs> OnDamage;

        internal static void Initialize(IEntityEvents events)
        {
            events.Damage = OnDamage.SafeInvoke;
        }
    }
}