namespace SparkTech.SDK.Orbwalker
{
    using System;

    using SparkTech.SDK.EventData;
    using SparkTech.SDK.Modules;

    public interface IOrbwalker : IModule
    {
        float LastAutoAttackStartTime { get; }

        bool IsAttacking { get; }

        //float LastOrderTime { get; }

        Action<BeforeAttackEventArgs> BeforeAttack { get; set; }

        Action<AfterAttackEventArgs> AfterAttack { get; set; }
    }
}