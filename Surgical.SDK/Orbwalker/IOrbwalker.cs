namespace Surgical.SDK.Orbwalker
{
    using System;

    using Surgical.SDK.EventData;
    using Surgical.SDK.Modules;

    public interface IOrbwalker : IModule
    {
        float LastAutoAttackStartTime { get; }

        bool IsAttacking { get; }

        //float LastOrderTime { get; }

        Action<BeforeAttackEventArgs> BeforeAttack { get; set; }

        Action<AfterAttackEventArgs> AfterAttack { get; set; }
    }
}