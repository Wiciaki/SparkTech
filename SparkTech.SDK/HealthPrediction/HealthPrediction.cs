namespace SparkTech.SDK.HealthPrediction
{
    using SparkTech.SDK.Entities;
    using SparkTech.SDK.HealthPrediction.Default;
    using SparkTech.SDK.Modules;
    using SparkTech.SDK.Platform;

    public static class HealthPrediction
    {
        #region Static Fields

        private static readonly IModulePicker<IHealthPredition> Picker;

        static HealthPrediction()
        {
            Picker = SdkSetup.CreatePicker<IHealthPredition, DefaultHealthPrediction>();
        }

        #endregion

        #region Public Methods and Operators

        public static void Add<T>(string moduleName)
            where T : IHealthPredition, new()
        {
            Picker.Add<T>(moduleName);
        }

        public static float PredictHealth(this IAIBase unit, float time)
        {
            return Picker.Current.Predict(unit, time);
        }

        #endregion
    }
}