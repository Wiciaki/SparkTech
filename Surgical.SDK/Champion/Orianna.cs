﻿namespace Surgical.SDK.Champion
{
    using Newtonsoft.Json.Linq;

    using Surgical.SDK.Entities;
    using Surgical.SDK.GUI.Menu;

    public class Orianna : IChampion
    {
        public void Start()
        {
            
        }

        public void Pause()
        {

        }

        public Menu Menu { get; } = new Menu("surgical");

        public JObject? GetTranslations()
        {
            return null;
        }

        public float GetHealthIndicatorDamage(IHero hero)
        {
            return 0f;
        }
    }
}