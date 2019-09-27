namespace Surgical.SDK.HealthPrediction
{
    using Surgical.SDK.Entities;
    using Surgical.SDK.Modules;

    public static class HealthPrediction
    {
        #region Static Fields

        internal static IModulePicker<IHealthPredition> Picker;

        #endregion

        #region Public Methods and Operators

        public static float PredictHealth(this IUnit unit, float time)
        {
            return Picker.Current.Predict(unit, time);
        }

        #endregion
    }
}