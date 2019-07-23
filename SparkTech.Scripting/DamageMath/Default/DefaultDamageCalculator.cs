namespace SparkTech.DamageCalculator.Default
{
    using System;

    using SparkTech.Enumerations;

    internal class DefaultDamageCalculator : IDamageCalculator
    {
        #region Public Methods and Operators

        public void Release()
        {

        }

        #endregion

        #region Explicit Interface Methods

        float IDamageCalculator.CalculateDamageOnUnit(
            AIBaseClient attacker,
            AIBaseClient target,
            float rawDamage,
            DamageType damageType)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
