namespace SparkTech.SDK.MovementPrediction
{
    /*
    //TODO Plz sparky make this dynamic
    public class CustomCollisionObject
    {
        #region Constructors and Destructors

        public CustomCollisionObject(GameObject instance, Vector3 cPosition)
        {
            this.CastTime = Game.TickCount;
            this.Instance = instance;
            this.CastedPosition = cPosition;
        }

        #endregion

        #region Public Properties

        public Vector3 CastedPosition { get; set; }

        public float CastTime { get; set; }

        public GameObject Instance { get; set; }

        #endregion
    }

    public static class Collision
    {
        #region Static Fields

        public static List<CustomCollisionObject> CustomCollisionObjects = new List<CustomCollisionObject>();

        //TODO Plz sparky make this better
        public static List<string> CustomCollisionObjectsNames = new List<string>
                                                                 {
                                                                     "YasuoWMovingWall"
                                                                 };

        #endregion

        #region Constructors and Destructors

        static Collision()
        {
            AIBaseClient.OnProcessSpellCast += AIBaseClient_OnProcessSpellCast;
            GameObject.OnDelete += GameObject_OnDelete;
        }

        #endregion

        #region Public Methods and Operators

        public static List<AIBaseClient> GetCollision(List<Vector3> positions, PredictionInput input)
        {
            var result = new List<AIBaseClient>();

            foreach (var position in positions)
            {
                
                //if (input.CollisionObjects.HasFlag(CollisionableObjects.Minions))
                //{
                //    foreach (var minion in ObjectCache.Get<AIMinion>().Where(
                //        minion => minion.IsValidTarget(
                //            Math.Min(input.Range + input.Radius + 100, 2000),
                //            checkRangeFrom: input.RangeCheckFrom)))
                //    {
                //        input.Target = minion;
                //        var minionPrediction = SDK.MovementPrediction.Instance.GetPrediction(input, false, false);
                //        if (((Vector2)minionPrediction.UnitPosition).DistanceSquared(
                //            (Vector2)input.From,
                //            (Vector2)position,
                //            true) <= Math.Pow(input.Radius + 15 + minion.BoundingRadius, 2))
                //        {
                //            result.Add(minion);
                //        }
                //    }
                //}
                
                
                //if (input.CollisionObjects.HasFlag(CollisionableObjects.Heroes))
                //{
                //    foreach (var hero in ObjectCache.Get<AIHeroClient>().Where(
                //        hero => hero.IsValidTarget(
                //            Math.Min(input.Range + input.Radius + 100, 2000),
                //            checkRangeFrom: input.RangeCheckFrom)))
                //    {
                //        input.Target = hero;
                //        var prediction = SDK.MovementPrediction.Instance.GetPrediction(input, false, false);
                //        if (((Vector2)prediction.UnitPosition).DistanceSquared(
                //            (Vector2)input.From,
                //            (Vector2)position,
                //            false) <= Math.Pow(input.Radius + 50 + hero.BoundingRadius, 2))
                //        {
                //            result.Add(hero);
                //        }
                //    }
                //}
                
                if (input.CollisionObjects.HasFlag(CollisionableObjects.Walls))
                {
                }

                if (!input.CollisionObjects.HasFlag(CollisionableObjects.YasuoWall))
                {
                    continue;
                }

                //TODO Plz sparky make this better
                if (CustomCollisionObjects.Any())
                {
                    foreach (var customCollisionObject in CustomCollisionObjects)
                    {
                        //TODO Make this dynamic
                        //Yasuo wall's duration is 4 secs
                        if (Game.TickCount - customCollisionObject.CastTime > 4000)
                        {
                            //TODO Maybe put all this into the object ?
                            var level = customCollisionObject.Instance.Name.Substring(
                                customCollisionObject.Instance.Name.Length - 6,
                                1);
                            var wallWidth = 300 + 50 * Convert.ToInt32(level);
                            var wallDirection =
                                (customCollisionObject.Instance.Position.To2D()
                                 - customCollisionObject.CastedPosition.To2D()).Normalized().Perpendicular();
                            var wallStart = customCollisionObject.Instance.Position.To2D()
                                            + wallWidth / 2f * wallDirection;
                            var wallEnd = wallStart - wallWidth * wallDirection;

                            if (!wallStart.Intersection(wallEnd, (Vector2)position, (Vector2)input.From).Intersects)
                            {
                                continue;
                            }

                            var t = Game.TickCount
                                    + (wallStart.Intersection(wallEnd, (Vector2)position, (Vector2)input.From).Point.To3D()
                                           .Distance(input.From) / input.Speed + input.Delay) * 1000;

                            if (t < customCollisionObject.CastTime + 4000)
                            {
                                result.Add(LocalPlayer.Instance);
                            }
                        }
                    }
                }
            }

            return result.Distinct().ToList();
        }

        #endregion

        #region Methods

        private static void AIBaseClient_OnProcessSpellCast(AIBaseClientCastEventArgs args)
        {
            if (!args.Caster.IsValid || !args.Caster.IsEnemy()
                || !CustomCollisionObjectsNames.Contains(args.SpellData.Name))
            {
                return;
            }

            //TODO make this better
            var wall = ObjectManager.Get<GameObject>().FirstOrDefault(
                x => x.IsValid && Regex.IsMatch(x.Name, "_w_windwall_enemy_0.\\.troy", RegexOptions.IgnoreCase));

            if (wall != null)
            {
                //TODO Server pos
                CustomCollisionObjects.Add(new CustomCollisionObject(wall, args.Caster.Position));
            }
        }

        private static void GameObject_OnDelete(GameObjectDeleteEventArgs args)
        {
            if (CustomCollisionObjects.Any())
            {
                if (args.Sender.IsEnemy())
                {
                    //TODO Proper delete the custom object collision
                }
            }
        }

        #endregion
    }*/
}