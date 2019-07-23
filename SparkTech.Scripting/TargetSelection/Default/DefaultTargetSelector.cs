namespace SparkTech.TargetSelector.Default
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SparkTech.TargetSelector.Default.Weights;
    using SparkTech.UI.Menu;
    using SparkTech.UI.Menu.Values;
    using SparkTech.Utils;

    internal sealed class DefaultTargetSelector : ITargetSelector
    {
        ModuleMenu IEntropyModule.Menu => Menu;

        private static readonly ModuleMenu Menu;

        private static readonly List<Weight> Weights;

        private static readonly IEqualityComparer<AIHeroClient> EqualityComparer;

        static DefaultTargetSelector()
        {
            EqualityComparer = new GameObjectComparer();
            Weights = new List<Weight>();

            Menu = new ModuleMenu("TargetSelector")
                   {
                       new MenuLabel("Notice")
                   };

            // TODO: other weights
            Register<DistanceWeight>();

            void Register<TWeight>() where TWeight : Weight, new()
            {
                var weight = new TWeight();
                Weights.Add(weight);

                foreach (var component in weight.CreateItems())
                {
                    Menu.Add(component);
                }
            }
        }

        AIHeroClient ITargetSelector.SelectTarget(IEnumerable<AIHeroClient> heroes)
        {
            var enemies = heroes.Where(hero => hero.IsEnemy()).ToList();

            if (enemies.Count == 0)
            {
                return null;
            }

            var sortedEnemies = enemies.ToArray();
            var weightCollection = new Dictionary<AIHeroClient, int>(enemies.Count, EqualityComparer);
            enemies.ForEach(enemy => weightCollection.Add(enemy, 0));

            foreach (var weight in Weights)
            {
                var w = weight.GetWeight();

                if (w == 0)
                {
                    continue;
                }

                Array.Sort(sortedEnemies, weight);

                for (var i = weightCollection.Count; i > 0; --i)
                {
                    weightCollection[sortedEnemies[i]] += i * w;
                }
            }

            return weightCollection.OrderByDescending(pair => pair.Value).Select(pair => pair.Key).First();
        }

        void IEntropyModule.Release()
        {

        }
    }
}