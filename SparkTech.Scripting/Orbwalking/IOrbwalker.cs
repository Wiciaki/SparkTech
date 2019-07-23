namespace SparkTech.Orbwalking
{
    using System;

    public interface IOrbwalker : IModule
    {
        float LastAttackStartTime { get; }

        bool IsAttacking { get; }

        float LastOrderTime { get; }

        Mode GetMode();

        event Action<BeforeAttackEventArgs> BeforeAttack;

        event Action<AttackableUnit> AfterAttack;
    }
}