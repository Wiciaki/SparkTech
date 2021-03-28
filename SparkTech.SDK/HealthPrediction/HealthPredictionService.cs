namespace SparkTech.SDK.HealthPrediction
{
    using SparkTech.SDK.Entities;
    using SparkTech.SDK.Modules;

    public static class HealthPredictionService
    {
        #region Static Fields

        public static readonly Picker<IHealthPredition> Picker = new Picker<IHealthPredition>(new PortedPrediction());

        #endregion

        #region Public Methods and Operators

        public static float PredictHealth(this IUnit unit, float time)
        {
            return Picker.Current.Predict(unit, time);
        }

        #endregion
    }
}