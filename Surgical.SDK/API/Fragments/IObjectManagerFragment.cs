﻿namespace Surgical.SDK.API.Fragments
{
    using System;

    using Surgical.SDK.Entities;

    public interface IObjectManagerFragment
    {
        IGameObject[] GetUnits();

        IHero GetPlayer();

        Action<IGameObject> Create { get; set; }

        Action<IGameObject> Delete { get; set; }
    }
}