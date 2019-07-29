//  -------------------------------------------------------------------
//
//  Last updated: 21/08/2017
//  Created: 19/08/2017
//
//  Copyright (c) Entropy, 2017 - 2017
//
//  DelayAction.cs is a part of SparkTech
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

namespace SparkTech.SDK.TickOperations
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using SparkTech.SDK.Util;

    public class DelayAction
    {
        #region Static Fields

        private static readonly SortedSet<DelayAction> DelayActions = new SortedSet<DelayAction>(new DelayActionComparer());

        #endregion

        #region Fields

        private readonly Action action;

        private readonly float time;

        #endregion

        #region Constructors and Destructors

        static DelayAction()
        {
            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EntropyEventArgs args)
        {
            if (DelayActions.Count == 0)
            {
                return;
            }

            DelayAction current;

            while ((current = DelayActions.Min).time <= args.ExecuteTime)
            {
                if (!DelayActions.Remove(current))
                {
                    throw new Exception("wtf");
                }

                current.action.TryExecute("Couldn't invoke a DelayAction callback!");
            }
        }

        private DelayAction(float time, Action action)
        {
            this.time = time + Game.ClockTime;

            this.action = action;
        }

        #endregion

        #region Public Methods and Operators

        public static void Queue(float time, Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof (action));
            }

            if (time <= 0f)
            {
                throw new ArgumentException("time <= 0f");
            }

            DelayActions.Add(new DelayAction(time, action));
        }

        public static void Queue(int time, Action action)
        {
            Queue(time.ToSeconds(), action);
        }

        public static void InOnUpdate(Action action)
        {
            void OnUpdate(EntropyEventArgs args)
            {
                Game.OnUpdate -= OnUpdate;

                action.TryExecute("Couldn't invoke the \"InOnUpdate\" action.");
            }

            Game.OnUpdate += OnUpdate;
        }


        #endregion

        [SuppressMessage(
            "ReSharper",
            "PossibleNullReferenceException",
            Justification = "DelayAction objects are only ever created internally and will never be null.")]
        private sealed class DelayActionComparer : IComparer<DelayAction>
        {
            #region Explicit Interface Methods

            int IComparer<DelayAction>.Compare(DelayAction x, DelayAction y)
            {
                return x.time.CompareTo(y.time);
            }

            #endregion
        }
    }
}