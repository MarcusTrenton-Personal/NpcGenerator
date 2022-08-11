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
along with this program.If not, see<https://www.gnu.org/licenses/>.*/

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NpcGenerator
{
    public class Npc
    {
        public const char CSV_SEPARATOR = ',';
        public const char MULTI_TRAIT_SEPARATOR = '&';

        public void Add(string category, string[] traits)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }
            if (category.Length == 0)
            {
                throw new ArgumentException("string must not be null or empty", nameof(category));
            }
            if (traits == null)
            {
                throw new ArgumentNullException(nameof(traits));
            }
            foreach (var trait in traits)
            {
                if (string.IsNullOrEmpty(trait))
                {
                    throw new ArgumentException("string array elements must not be null or empty", nameof(traits));
                }
            }

            bool categoryExists = m_traitsByCategory.ContainsKey(category);
            List<string> traitsList;
            if (categoryExists)
            {
                traitsList = m_traitsByCategory[category];
            }
            else
            {
                m_traitsByCategory[category] = new List<string>();
                traitsList = m_traitsByCategory[category];
            }
            traitsList.AddRange(traits);
        }

        public string[] GetTraitsOfCategory(string category)
        {
            bool success = m_traitsByCategory.TryGetValue(category, out List<string> traits);
            if (success)
            {
                return traits.ToArray();
            }
            else
            {
                return Array.Empty<string>();
            }
        }

        private readonly Dictionary<string,List<string>> m_traitsByCategory = new Dictionary<string, List<string>>();
    }
}