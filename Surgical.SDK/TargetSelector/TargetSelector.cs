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

            this.weights = Weight.CreateWeights(this.Menu);
        }

        IHero ITargetSelector.GetTarget(IEnumerable<IHero> heroes)
        {
            // experimental, lets see how this performs
            var enemies = heroes.Where(hero => hero.IsEnemy()).ToArray();

            switch (enemies.Length)
            {
                case 0:
                    return null;
                case 1:
                    return enemies[0];
            }

            var dictionary = enemies.ToDictionary(e => e, e => 0, new EntityComparer<IHero>());
            var query = from w in this.weights let v = w.Value where v > 0 select (w, v);

            foreach (var (weight, value) in query)
            {
                Array.Sort(enemies, weight);

                for (var i = dictionary.Count; i >= 0; --i)
                {
                    dictionary[enemies[i]] += i * value;
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