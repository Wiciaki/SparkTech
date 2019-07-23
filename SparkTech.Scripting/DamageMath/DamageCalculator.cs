namespace SparkTech.DamageCalculator
{
    using SparkTech.DamageCalculator.Default;
    using SparkTech.Enumerations;

    public static class DamageCalculator
    {
        #region Static Fields

        public static readonly IModulePicker<IDamageCalculator> Picker;

        #endregion

        #region Constructors and Destructors

        static DamageCalculator()
        {
            Picker = EntropySetup.CreatePicker<IDamageCalculator>();
            Picker.Add<DefaultDamageCalculator>("Default");
            Picker.Start();
        }

        #endregion

        #region Public Methods and Operators

        public static float CalculateDamageOnUnit(AIBaseClient attacker, AIBaseClient target, float rawDamage, DamageType damageType)
        {
            return Picker.Current.CalculateDamageOnUnit(attacker, target, rawDamage, damageType);
        }

        #endregion

    }
}
