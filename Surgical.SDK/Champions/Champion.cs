﻿namespace Surgical.SDK.Champions
{
    using System;
    using System.Reflection;

    using Surgical.SDK.Entities;
    using Surgical.SDK.GUI.Menu;

    public abstract class Champion
    {
        protected static readonly Menu Menu;

        public virtual float GetHealthIndicatorDamage(IHero hero)
        {
            // 
            return 0;
        }

        internal static void Initialize()
        {
            var champName = ObjectManager.Player.CharName;

            var type = Assembly.GetAssembly(typeof(Champion)).GetType("" + champName);

            if (!type.IsSubclassOf(typeof(Champion)))
            {
                return;
            }

            var champ = (Champion)Activator.CreateInstance(type);
        }
    }
}