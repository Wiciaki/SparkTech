namespace SparkTech.SDK.MovementPrediction
{
    using SparkTech.SDK.Entities;
    using SparkTech.SDK.Modules;
    using SparkTech.SDK.MovementPrediction.Default;
    using SparkTech.SDK.Platform;

    public static class MovementPrediction
    {
        #region Static Fields

        private static readonly IModulePicker<IMovementPrediction> Picker = SdkSetup.CreatePicker<IMovementPrediction, DefaultMovementPrediction>();

        #endregion

        #region Constructors and Destructors

        static MovementPrediction()
        {
            Picker.Add<DefaultMovementPrediction>("Default");
        }

        #endregion

        #region Public Methods and Operators

        public static PredictionOutput GetPrediction(PredictionInput input)
        {
            return Picker.Current.Predict(input);
        }

        #endregion
    }
}