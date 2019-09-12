namespace Surgical.SDK.HealthPrediction
{
    using Surgical.SDK.Entities;
    using Surgical.SDK.Modules;

    public static class HealthPrediction
    {
        #region Static Fields

        private static readonly IModulePicker<IHealthPredition> Picker;

        static HealthPrediction()
        {
            //Picker = SdkSetup.CreatePicker<IHealthPredition, DefaultHealthPrediction>();
        }

        #endregion

        #region Public Methods and Operators

        public static void Add<T>(string moduleName)
            where T : IHealthPredition, new()
        {
            Picker.Add<T>(moduleName);
        }

        public static float PredictHealth(this IUnit unit, float time)
        {
            return Picker.Current.Predict(unit, time);
        }

        #endregion
    }
}