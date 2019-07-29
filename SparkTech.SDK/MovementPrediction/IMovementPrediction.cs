namespace SparkTech.SDK.MovementPrediction
{
    using SparkTech.SDK.Modules;

    /// <summary>
    ///     The movement prediction can predict a units future path
    /// </summary>
    public interface IMovementPrediction : IModule
    {
        #region Public Methods and Operators

        PredictionOutput Predict(PredictionInput predInput);

        #endregion
    }
}