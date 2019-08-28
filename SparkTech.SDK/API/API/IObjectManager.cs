namespace SparkTech.SDK.Platform.API
{
    using System;

    using SparkTech.SDK.Entities;

    public interface IObjectManager
    {
        IGameObject[] GetUnits();

        IPlayer GetPlayer();

        Action<IGameObject> Create { get; set; }

        Action<IGameObject> Delete { get; set; }
    }
}