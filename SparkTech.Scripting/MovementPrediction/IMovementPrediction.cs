//  -------------------------------------------------------------------
//
//  Last updated: 21/08/2017
//  Created: 26/07/2017
//
//  Copyright (c) Entropy, 2017 - 2017
//
//  IMovementPrediction.cs is a part of SparkTech
//
//  license soonTM
//
//  -------------------------------------------------------------------

namespace SparkTech.MovementPrediction
{
    using SparkTech.Spells;

    /// <summary>
    ///     The movement prediction can predict a units future path
    /// </summary>
    public interface IMovementPrediction : IEntropyModule
    {
        #region Public Methods and Operators

        PredictionOutput Predict(PredictionInput predInput);

        PredictionOutput Predict(AIBaseClient target);

        #endregion
    }
}