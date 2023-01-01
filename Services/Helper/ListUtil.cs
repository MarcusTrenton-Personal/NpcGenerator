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
    public static class ListUtil
    {
        public static IReadOnlyList<T> ConvertAll<T,U>(in IReadOnlyList<U> list, in Func<U, T> converter) 
            where T : notnull 
            where U : notnull
        {
            ParamUtil.VerifyNotNull(nameof(list), list);
            ParamUtil.VerifyNotNull(nameof(converter), converter);

            List<T> result = new List<T>();
            foreach(U item in list)
            {
                result.Add(converter(item));
            }
            return result.AsReadOnly();
        }

        public const int INDEX_NOT_FOUND = -1;
        public static int IndexOf<T>(in IReadOnlyList<T> list, in Predicate<T> test) where T : notnull
        {
            ParamUtil.VerifyNotNull(nameof(list), list);
            ParamUtil.VerifyNotNull(nameof(test), test);

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

        public static List<T> DistinctPreserveOrder<T>(in IReadOnlyList<T> list) where T : notnull
        {
            ParamUtil.VerifyElementsAreNotNull(nameof(list), list);

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

        public static List<T> FindAll<T>(in IReadOnlyList<T> list, in Predicate<T> test) where T : notnull
        {
            ParamUtil.VerifyNotNull(nameof(list), list);
            ParamUtil.VerifyNotNull(nameof(test), test);

            List<T> result = new List<T>();
            foreach (T element in list)
            {
                if (test(element))
                {
                    result.Add(element);
                }
            }

            return result;
        }
    }
}
