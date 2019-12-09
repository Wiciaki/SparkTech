namespace Surgical.SDK.MovementPrediction
{
    using Surgical.SDK.Modules;

    public static class MovementPredictionService
    {
        #region Static Fields

        public static readonly Picker<IMovementPrediction> Picker = new Picker<IMovementPrediction>(new MovementPrediction());

        #endregion

        #region Public Methods and Operators

        public static PredictionOutput Predict(this PredictionInput input)
        {
            return Picker.Current.Predict(input);
        }

        #endregion
    }
}