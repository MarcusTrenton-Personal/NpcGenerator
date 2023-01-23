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
using System.Text.RegularExpressions;

namespace Services
{
    public static class ParamUtil
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
                throw new ArgumentException(name + " must be 0 or greater");
            }
        }

        public static void VerifyPositiveNumber(string name, int value)
        {
            if (value <= 0)
            {
                throw new ArgumentException(name + " must be greater than 0");
            }
        }

        public static void VerifyDictionaryKeyExists<T,U>(string dictionaryName, in IDictionary<T,U> dictionary, string keyName, T key)
        {
            if (dictionary is null)
            {
                throw new ArgumentNullException(dictionaryName);
            }

            bool elementExists = dictionary.ContainsKey(key);
            if (!elementExists)
            {
                throw new ArgumentException(keyName + " was not found among the " + dictionaryName);
            }
        }

        public static void VerifyDictionaryValuesNotNull<T, U>(string dictionaryName, in IDictionary<T, U> dictionary)
        {
            if (dictionary is null)
            {
                throw new ArgumentNullException(dictionaryName);
            }

            foreach (var keyValuePair in dictionary)
            {
                if (keyValuePair.Key is null)
                {
                    throw new ArgumentException("Null key in " + dictionaryName);
                }

                if (keyValuePair.Value is null)
                {
                    throw new ArgumentException("Null value in " + dictionaryName);
                }
            }
        }

        public static void VerifyMatchesPattern(string name, string value, string pattern, string errorMessage)
        {
            Regex regex = new Regex(pattern);
            bool isMatch = regex.IsMatch(value);
            if (!isMatch)
            {
                throw new ArgumentException(errorMessage, name);
            }    
        }

        public static void VerifyMatchesPattern(string name, string value, Regex regex, string errorMessage)
        {
            bool isMatch = regex.IsMatch(value);
            if (!isMatch)
            {
                throw new ArgumentException(errorMessage, name);
            }
        }

        public static void VerifyInRange(string name, int value, int minInclusive, int maxInclusive)
        {
            if (minInclusive > maxInclusive)
            {
                throw new ArgumentException("Invalid range where " + nameof(minInclusive) + " " + minInclusive + " > " + nameof(maxInclusive) + 
                    " of " + maxInclusive);
            }
            if (value < minInclusive || value > maxInclusive)
            {
                throw new ArgumentException(name + "is not in the inclusive range " + minInclusive + " to " + maxInclusive);
            }
        }
    }
}
