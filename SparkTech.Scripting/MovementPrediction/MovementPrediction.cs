namespace SparkTech.MovementPrediction
{
    using SparkTech.MovementPrediction.Default;

    public static class MovementPrediction
    {
        #region Static Fields

        private static readonly IModulePicker<IMovementPrediction> Picker = EntropySetup.CreatePicker<IMovementPrediction, DefaultMovementPrediction>();

        #endregion

        #region Constructors and Destructors

        static MovementPrediction()
        {
            //Adding default prediction
            Picker.Add<DefaultMovementPrediction>("Default");
        }

        #endregion

        #region Public Methods and Operators

        public static PredictionOutput GetPrediction(AIBaseClient target)
        {
            return Picker.Current.Predict(target);
        }

        public static PredictionOutput GetPrediction(PredictionInput input)
        {
            return Picker.Current.Predict(input);
        }

        #endregion
    }
}