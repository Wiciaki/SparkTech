﻿namespace Surgical.SDK.TargetSelector.Default
{
    using System;

    using Surgical.SDK.Entities;

    internal class DistanceWeight : Weight
    {
        protected override int GetDefaultWeight() => 2;

        protected override IComparable GetComparable(IHero target)
        {
            return 0; //ObjectManager.Player.Distance(target);
        }
    }
}