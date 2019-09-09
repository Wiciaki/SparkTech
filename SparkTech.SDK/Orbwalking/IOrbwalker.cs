namespace SparkTech.SDK.Orbwalking
{
    using System;

    using SparkTech.SDK.Entities;
    using SparkTech.SDK.Modules;

    public interface IOrbwalker : IModule
    {
        float LastAutoAttackStartTime { get; }

        bool IsAttacking { get; }

        //float LastOrderTime { get; }

        //Mode GetMode();

        Action<BeforeAttackEventArgs> BeforeAttack { get; set; }

        Action<IAttackable> AfterAttack { get; set; }
    }
}