namespace Surgical.SDK.TargetSelector
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json.Linq;

    using Surgical.SDK.Entities;
    using Surgical.SDK.GUI.Menu;
    using Surgical.SDK.Modules;
    using Surgical.SDK.Properties;

    public class TargetSelector : ITargetSelector
    {
        public Menu Menu { get; }

        private readonly List<Weight> weights;

        public TargetSelector()
        {
            this.Menu = new Menu("TargetSelector");

            this.weights = Weight.GetWeights(this.Menu);
        }

        IHero ITargetSelector.GetTarget(IEnumerable<IHero> targets)
        {
            // experimental, lets see how this performs
            var enemies = targets.Where(hero => hero.IsEnemy()).ToArray();

            switch (enemies.Length)
            {
                case 0:
                    return null;
                case 1:
                    return enemies[0];
            }

            var dictionary = enemies.ToDictionary(e => e, e => 0, new EntityComparer<IHero>());
            var query = from w in this.weights let i = w.Importance where i > 0 select (w, i);

            foreach (var (weight, importance) in query)
            {
                Array.Sort(enemies, weight);

                for (var i = dictionary.Count; i >= 0; --i)
                {
                    dictionary[enemies[i]] += i * importance;
                }
            }

            var result = dictionary.OrderByDescending(pair => pair.Value).First();

            return result.Key;
        }

        public JObject GetTranslations()
        {
            return JObject.Parse(Resources.TargetSelector);
        }

        void IModule.Start()
        {

        }

        void IModule.Pause()
        {

        }
    }
}