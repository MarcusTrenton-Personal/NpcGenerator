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

        public Npc()
        {
        }

        public Npc(IReadOnlyList<TraitCategory> categories)
        {
            Dictionary<TraitCategory, TraitChooser> chooserForCategory = new Dictionary<TraitCategory, TraitChooser>();
            Dictionary<TraitCategory, int> selectionsPerCategory = new Dictionary<TraitCategory, int>();
            foreach (TraitCategory category in categories)
            {
                selectionsPerCategory[category] = category.DefaultSelectionCount;
                chooserForCategory[category] = category.CreateTraitChooser();
            }

            while (selectionsPerCategory.Count > 0)
            {
                GetElementOf(selectionsPerCategory, out TraitCategory category, out int count);
                TraitChooser chooser = chooserForCategory[category];
                string[] traits = chooser.Choose(count, out List<BonusSelection> bonusSelections);
                AddTrait(category: category.Name, traits: traits);

                selectionsPerCategory.Remove(category);

                foreach (BonusSelection bonusSelection in bonusSelections)
                {
                    selectionsPerCategory[bonusSelection.TraitCategory] = bonusSelection.SelectionCount;
                }
            }
        }

        private static void GetElementOf(Dictionary<TraitCategory, int> selectionsPerCategory, out TraitCategory category, out int count)
        {
            Dictionary<TraitCategory, int>.Enumerator enumerator = selectionsPerCategory.GetEnumerator();
            enumerator.MoveNext();
            KeyValuePair<TraitCategory, int> current = enumerator.Current;
            category = current.Key;
            count = current.Value;
        }

        public void AddTrait(string category, string[] traits)
        {
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

        public string[] ToStringArrayByCategory(IList<string> categoryOrder)
        {
            List<string> traitsPerCategory = new List<string>();
            foreach (string category in categoryOrder)
            {
                bool found = m_traitsByCategory.TryGetValue(category, out List<string> traits);
                if (found)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    CombineTraits(traits, stringBuilder);
                    traitsPerCategory.Add(stringBuilder.ToString());
                }
            }
            return traitsPerCategory.ToArray();
        }

        public void ToCsvRow(StringBuilder stringBuilder, IList<string> categoryOrder)
        {
            if (stringBuilder == null)
            {
                throw new ArgumentNullException(nameof(stringBuilder));
            }
            for (int i = 0; i < categoryOrder.Count; ++i)
            {
                bool found = m_traitsByCategory.TryGetValue(categoryOrder[i], out List<string> traits);
                if (found)
                {
                    CombineTraits(traits, stringBuilder);
                }
                if (i + 1 < m_traitsByCategory.Count)
                {
                    stringBuilder.Append(CSV_SEPARATOR);
                }
            }
        }

        public void ToJsonObject(JsonWriter writer, IList<string> categoryOrder)
        {
            writer.WriteStartObject();
            
            for(int i = 0; i < categoryOrder.Count; ++i)
            {
                bool found = m_traitsByCategory.TryGetValue(categoryOrder[i], out List<string> traits);
                if (found)
                {
                    writer.WritePropertyName(categoryOrder[i]);
                    writer.WriteStartArray();
                    for(int j = 0; j < traits.Count; ++j)
                    {
                        writer.WriteValue(traits[j]);
                    }
                    writer.WriteEndArray();
                }
            }

            writer.WriteEndObject();
        }

        private static void CombineTraits(List<string> traits, StringBuilder stringBuilder)
        {
            for (int i = 0; i < traits.Count; ++i)
            {
                stringBuilder.Append(traits[i]);
                if (i + 1 < traits.Count)
                {
                    stringBuilder.Append(" " + MULTI_TRAIT_SEPARATOR + " ");
                }
            }
        }

        private readonly Dictionary<string,List<string>> m_traitsByCategory = new Dictionary<string, List<string>>();
    }
}