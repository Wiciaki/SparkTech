//  -------------------------------------------------------------------
//
//  Last updated: 21/08/2017
//  Created: 01/08/2017
//
//  Copyright (c) Entropy, 2017 - 2017
//
//  MathOperations.cs is a part of SparkTech
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

namespace SparkTech.SDK.Util
{
    using System;

    public static class MathOperations
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Returns the given number raised to the power of two.
        /// </summary>
        public static int Pow(this int number)
        {
            return number * number;
        }

        /// <summary>
        ///     Returns the given number raised to the power of two.
        /// </summary>
        public static uint Pow(this uint number)
        {
            return number * number;
        }

        /// <summary>
        ///     Returns the given number raised to the power of two.
        /// </summary>
        public static double Pow(this double number)
        {
            return number * number;
        }

        /// <summary>
        ///     Returns the given number raised to the power of two.
        /// </summary>
        public static float Pow(this float number)
        {
            return number * number;
        }

        /// <summary>
        ///     Returns the square root of the given number.
        /// </summary>
        public static double Sqrt(this int number)
        {
            return Math.Sqrt(number);
        }

        /// <summary>
        ///     Returns the square root of the given number.
        /// </summary>
        public static double Sqrt(this uint number)
        {
            return Math.Sqrt(number);
        }

        /// <summary>
        ///     Returns the square root of the given number.
        /// </summary>
        public static double Sqrt(this double number)
        {
            return Math.Sqrt(number);
        }

        /// <summary>
        ///     Returns the square root of the given number.
        /// </summary>
        public static double Sqrt(this float number)
        {
            return Math.Sqrt(number);
        }

        public static float ToSeconds(this int ticks)
        {
            return ticks / 1000f;
        }

        public static int ToTicks(this float seconds)
        {
            return (int)(seconds * 1000f);
        }

        #endregion
    }
}