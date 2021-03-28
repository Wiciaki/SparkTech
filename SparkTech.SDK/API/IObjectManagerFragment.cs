namespace SparkTech.SDK.API
{
    using System;

    using SparkTech.SDK.Entities;

    public interface IObjectManagerFragment
    {
        IGameObject[] GetUnits();

        int GetPlayerId();

        Action<IGameObject> Create { get; set; }

        Action<IGameObject> Delete { get; set; }
    }
}