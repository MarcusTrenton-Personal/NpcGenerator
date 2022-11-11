/*Copyright(C) 2022 Marcus Trenton, marcus.trenton@gmail.com

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <https://www.gnu.org/licenses/>.*/

using System;
using System.Collections.Generic;

namespace Services
{
    public class ParamUtil
    {
        public static void VerifyHasContent(string name, string value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(name);
            }
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(name + " is just whitespace");
            }
        }

        public static void VerifyNotNull(string name, object value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(name);
            }
        }

        public static void VerifyElementsAreNotNull<T>(string name, in IEnumerable<T> values) where T: notnull
        {
            if (values is null)
            {
                throw new ArgumentNullException(name);
            }
            foreach (T value in values)
            {
                if (value is null)
                {
                    throw new ArgumentException(name + " has a null element");
                }
            }
        }

        public static void VerifyWholeNumber(string name, int value)
        {
            if (value < 0)
            {
                throw new ArgumentException(name + " must 0 or greater");
            }
        }

        public static void VerifyInternalDictionaryKeyExists<T,U>(string dictionaryName, in IDictionary<T,U> dictionary, string keyName, T key)
        {
            bool dictionaryExists = dictionary != null;
            if (!dictionaryExists)
            {
                throw new InvalidOperationException(dictionaryName + " not initialized");
            }

            bool elementExists = dictionary.ContainsKey(key);
            if (!elementExists)
            {
                throw new ArgumentException(keyName + " was not found among the " + dictionaryName);
            }
        }
    }
}
