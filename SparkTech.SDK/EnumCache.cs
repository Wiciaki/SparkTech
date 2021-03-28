//  -------------------------------------------------------------------
//
//  Last updated: 21/08/2017
//  Created: 26/07/2017
//
//  Copyright (c) Entropy, 2017 - 2017
//
//  EnumCache.cs is a part of SparkTech
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

namespace SparkTech.SDK
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    ///     Exposes the enumeration for fast access.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type to be cached.</typeparam>
    [SuppressMessage(
        "ReSharper",
        "StaticMemberInGenericType",
        Justification = "The members differ for every type param provided, therefore the suppression is fine.")]
    public static class EnumCache<TEnum> where TEnum : struct, IConvertible
    {
        #region Static Fields

        /// <summary>
        ///     The amount of all the values in an enumeration.
        /// </summary>
        public static int Count => Values.Count;

        /// <summary>
        ///     The names of the constants in the enumeration.
        /// </summary>
        public static readonly List<string> Names;

        /// <summary>
        ///     The enumeration values represented by a list.
        /// </summary>
        public static readonly List<TEnum> Values;

        /// <summary>
        ///     Contains the descriptions of the enum members.
        /// </summary>
        private static readonly Dictionary<TEnum, string> Descriptions;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="EnumCache{TEnum}" /> class.
        /// </summary>
        /// <exception cref="InvalidOperationException"><see cref="TEnum" /> is not of an enumerable type.</exception>
        static EnumCache()
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new InvalidOperationException("TEnum must be of an enumerable type!");
            }

            Values = ((TEnum[])Enum.GetValues(typeof(TEnum))).ToList();

            Names = Values.ConvertAll(@enum => @enum.ToString(CultureInfo.InvariantCulture));

            Descriptions = Values.ToDictionary(
                e => e,
                e => ((DescriptionAttribute)typeof(TEnum).GetMember(e.ToString(CultureInfo.InvariantCulture)).Single().GetCustomAttributes(typeof(DescriptionAttribute), false)
                             .SingleOrDefault())?.Description);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Retrieves a description from the specified <see cref="TEnum" />.
        ///     <para>This returns null if the item has no description.</para>
        /// </summary>
        /// <param name="item">The value to have its description read.</param>
        /// <returns></returns>
        public static string Description(TEnum item)
        {
            return Descriptions[item];
        }

        /// <summary>
        ///     Gets the equivalent <see cref="TEnum" /> value.
        /// </summary>
        /// <param name="value">The string to be parsed.</param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException">The value couldn't be parsed.</exception>
        public static TEnum Parse(string value)
        {
            return Values[Names.IndexOf(value)];
        }

        /// <summary>
        ///     Gets the equivalent <see cref="TEnum" /> value, or the default one, when parsing failed.
        /// </summary>
        /// <param name="value">The string to be parsed.</param>
        /// <param name="result">The parsing result</param>
        /// <returns></returns>
        public static bool TryParse(string value, out TEnum result)
        {
            var i = Names.IndexOf(value);

            if (i == -1)
            {
                result = default(TEnum);

                return false;
            }

            result = Values[i];

            return true;
        }

        #endregion
    }
}