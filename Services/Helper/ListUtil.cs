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
using System.Text;

namespace Services
{
    public static class ListUtil
    {
        public static T Find<T>(IReadOnlyList<T> list, Predicate<T> test) where T : notnull
        {
            if (list is null)
            {
                throw new ArgumentNullException(nameof(list));
            }
            if (test is null)
            {
                throw new ArgumentNullException(nameof(test));
            }

            foreach (T item in list)
            {
                if (test(item))
                {
                    return item;
                }
            }
            return default;
        }

        public static bool IsNullOrEmpty<T>(IReadOnlyList<T> list) where T : notnull
        {
            return list is null || list.Count == 0;
        }

        public static IReadOnlyList<T> ConvertAll<T,U>(IReadOnlyList<U> list, Func<U, T> converter) 
            where T : notnull 
            where U : notnull
        {
            if (list is null)
            {
                throw new ArgumentNullException(nameof(list));
            }
            if (converter is null)
            {
                throw new ArgumentNullException(nameof(converter));
            }

            List<T> result = new List<T>();
            foreach(U item in list)
            {
                result.Add(converter(item));
            }
            return result.AsReadOnly();
        }

        public const int INDEX_NOT_FOUND = -1;
        public static int IndexOf<T>(IReadOnlyList<T> list, Predicate<T> test) where T : notnull
        {
            if (list is null)
            {
                throw new ArgumentNullException(nameof(list));
            }
            if (test is null)
            {
                throw new ArgumentNullException(nameof(test));
            }

            for (int i = 0; i < list.Count; ++i)
            {
                bool isFound = test(list[i]);
                if (isFound)
                {
                    return i;
                }
            }
            return INDEX_NOT_FOUND;
        }

        public static List<T> DistinctPreserveOrder<T>(IReadOnlyList<T> list) where T : notnull
        {
            if (list is null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            List<T> result = new List<T>();
            bool[] processed = new bool[list.Count];
            for (int i = 0; i < processed.Length; ++i)
            {
                processed[i] = false;
            }

            for (int i = 0; i < list.Count; ++i)
            {
                if (!processed[i])
                {
                    result.Add(list[i]);
                    processed[i] = true;
                    for (int j = i + 1; j < list.Count; ++j)
                    {
                        if (!processed[j])
                        {
                            processed[j] = list[i].Equals(list[j]);
                        }
                    }
                }
            }
            return result;
        }
    }
}
