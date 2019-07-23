namespace SparkTech.HealthPrediction
{
    using SparkTech.HealthPrediction.Default;

    public static class HealthPrediction
    {
        #region Static Fields

        private static readonly IModulePicker<IHealthPredition> Picker;

        static HealthPrediction()
        {
            Picker = EntropySetup.CreatePicker<IHealthPredition, DefaultHealthPrediction>();
        }

        #endregion

        #region Public Methods and Operators

        public static void Add<T>(string moduleName)
            where T : IHealthPredition, new()
        {
            Picker.Add<T>(moduleName);
        }

        public static float GetHealthIn(this AIBaseClient unit, float time)
        {
            return Picker.Current.PredictHealth(unit, time);
        }

        #endregion
    }
}