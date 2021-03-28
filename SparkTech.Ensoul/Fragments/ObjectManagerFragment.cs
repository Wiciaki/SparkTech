namespace SparkTech.Ensoul.Fragments
{
    using System;
    using System.Linq;

    using SDK.API;
    using SDK.Entities;

    using Entities;

    public class ObjectManagerFragment : IObjectManagerFragment
    {
        public ObjectManagerFragment()
        {
            EnsoulSharp.GameObject.OnCreate += (o, args) => this.Create(Convert(o));
            EnsoulSharp.GameObject.OnDelete += (o, args) =>
            {
                var unit = ObjectManager.GetById(o.NetworkId);

                if (unit != null)
                {
                    this.Delete(unit);
                }
            };
        }

        public Action<IGameObject> Create { get; set; }

        public Action<IGameObject> Delete { get; set; }

        public int GetPlayerId()
        {
            return EnsoulSharp.ObjectManager.Player.NetworkId;
        }

        public IGameObject[] GetUnits()
        {
            return EnsoulSharp.ObjectManager.Get<EnsoulSharp.GameObject>().Select(Convert).ToArray();
        }

        internal static IGameObject Convert(EnsoulSharp.GameObject o)
        {
            if (o is EnsoulSharp.AIHeroClient hero)
            {
                return new AIHeroClient<EnsoulSharp.AIHeroClient>(hero);
            }

            if (o is EnsoulSharp.AIMinionClient minion)
            {
                return new AIMinionClient<EnsoulSharp.AIMinionClient>(minion);
            }

            if (o is EnsoulSharp.AITurretClient turret)
            {
                return new AITurretClient<EnsoulSharp.AITurretClient>(turret);
            }

            if (o is EnsoulSharp.AIBaseClient unit)
            {
                return new AIBaseClient<EnsoulSharp.AIBaseClient>(unit);
            }

            if (o is EnsoulSharp.BuildingClient building)
            {
                return new AIBuilding<EnsoulSharp.BuildingClient>(building);
            }

            if (o is EnsoulSharp.AttackableUnit attackable)
            {
                return new AttackableUnit<EnsoulSharp.AttackableUnit>(attackable);
            }

            if (o is EnsoulSharp.MissileClient missile)
            {
                return new MissileClient<EnsoulSharp.MissileClient>(missile);
            }

            return new GameObject<EnsoulSharp.GameObject>(o);
        }
    }
}