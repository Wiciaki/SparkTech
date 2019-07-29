//  -------------------------------------------------------------------
//
//  Last updated: 21/08/2017
//  Created: 27/07/2017
//
//  Copyright (c) Entropy, 2017 - 2017
//
//  ObjectCache.cs is a part of SparkTech
//
//  SparkTech is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//  SparkTech is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//  You should have received a copy of the GNU General Public License
//  along with SparkTech. If not, see <http://www.gnu.org/licenses/>.
//
//  -------------------------------------------------------------------

namespace SparkTech.SDK.Entities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using SparkTech.SDK.Logging;
    using SparkTech.SDK.Misc;
    using SparkTech.SDK.Platform.API;

    public static class ObjectManager
    {

        private static readonly Dictionary<Type, ObjectCacheEntry> Container = new Dictionary<Type, ObjectCacheEntry>
                                                                               {
                                                                                   [typeof(IGameObject)] =
                                                                                       new ObjectCacheEntry(
                                                                                           new HashSet<IGameObject>(
                                                                                               new GameObjectComparer<
                                                                                                   IGameObject>()))
                                                                               };

        public static event Action<IGameObject> OnCreate;

        public static event Action<IGameObject> OnDelete;

        internal static void Initialize(IObjectManager mgr)
        {
            Log.Info("ObjectManager - starting!");

            Player = mgr.GetPlayer();

            PlayerId = Player.Id();

            playerTeam = Player.Team();

            mgr.Create = HandleCreate;
            mgr.Delete = HandleDelete;

            Array.ForEach(mgr.GetUnits(), HandleCreate);
        }

        public static int PlayerId { get; private set; }

        public static IPlayer Player { get; private set; }

        private static GameObjectTeam playerTeam;

        public static HashSet<TGameObject> Get<TGameObject>() where TGameObject : IGameObject
        {
            if (Container.TryGetValue(typeof(TGameObject), out var entry))
            {
                return (HashSet<TGameObject>)entry.HashSet;
            }

            Log.Info($"ObjectCache - Adding {typeof(TGameObject).Name}!");

            var hashset = Container[typeof(IGameObject)].HashSet.OfType<TGameObject>().ToHashSet(new GameObjectComparer<TGameObject>());

            Container.Add(typeof(TGameObject), new ObjectCacheEntry(hashset));

            return hashset;
        }

        public static bool IsAlly(this IGameObject o)
        {
            return o.Team() == playerTeam;
        }

        public static bool IsEnemy(this IGameObject o)
        {
            return !o.IsAlly();
        }

        public static IGameObject GetById(int id)
        {
            return Get<IGameObject>().FirstOrDefault(o => o.Id() == id);
        }

        private static void HandleCreate(IGameObject sender)
        {
            ProcessItem(sender, true);
        }

        private static void HandleDelete(IGameObject sender)
        {
            ProcessItem(sender, false);
        }

        private static void ProcessItem(IGameObject sender, bool @new)
        {
            var senderType = sender.GetType();

            foreach (var (key, value) in Container)
            {
                if (key.IsAssignableFrom(senderType))
                {
                    value.Process(sender, @new);
                }
            }
        }

        private class ObjectCacheEntry
        {
            #region Fields

            public readonly IEnumerable HashSet;

            private readonly MethodInfo add;

            private readonly MethodInfo remove;

            private readonly object[] args;

            #endregion

            #region Constructors and Destructors

            public ObjectCacheEntry(IEnumerable hashSet)
            {
                var type = hashSet.GetType();

                this.add = type.GetMethod("Add");

                this.remove = type.GetMethod("Remove");

                this.args = new object[1];

                this.HashSet = hashSet;
            }

            #endregion

            #region Public Methods and Operators

            public void Process(IGameObject o, bool @new)
            {
                this.args[0] = o;

                if (@new)
                {
                    Process(this.add, OnCreate);
                }
                else
                {
                    Process(this.remove, OnDelete);
                }

                void Process(MethodBase method, Action<IGameObject> evt)
                {
                    method.Invoke(this.HashSet, this.args);

                    evt.SafeInvoke(o);
                }
            }

            #endregion
        }
    }
}