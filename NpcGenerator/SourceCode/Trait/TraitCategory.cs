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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;

namespace NpcGenerator
{
    public class TraitCategory
    {
        public TraitCategory(string name, int selectionCount)
        {
            if (selectionCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(selectionCount), "Must be 0 or greater");
            }

            Name = name;
            SelectionCount = selectionCount;
        }

        public void Add(Trait trait)
        {
            if (trait == null)
            {
                throw new ArgumentNullException(nameof(trait));
            }

            m_traits.Add(trait);
            m_totalWeight += trait.Weight;
        }

        public string[] Choose()
        {
            if (SelectionCount == 0)
            {
                return Array.Empty<string>();
            }

            if (m_traits.Count == 0)
            {
                throw new InvalidOperationException("Cannot choose trait from empty Trait Group " + Name);
            }

            Trace.Assert(m_traits.Count >= SelectionCount, "Not enough traits for all selections.");
            //If there's multiple selections, make a copy of the list so that selected traits can be removed.
            List<Trait> remainingTraitChoices = SelectionCount == 1 ? m_traits : new List<Trait>(m_traits);
            int remainingWeight = m_totalWeight;

            string[] selected = new string[SelectionCount];
            for (int i = 0; i < selected.Length; i++)
            {
                int randomSelection = RandomNumberGenerator.GetInt32(0, remainingWeight) + 1;
                int selectedIndex = -1;
                int weightCount = 0;
                for (int j = 0; j < remainingTraitChoices.Count; ++j)
                {
                    weightCount += remainingTraitChoices[j].Weight;
                    if (randomSelection <= weightCount)
                    {
                        selectedIndex = j;
                        break;
                    }
                }
                Trace.Assert(selectedIndex >= 0, "Failed to choose a trait.");
                selected[i] = remainingTraitChoices[selectedIndex].Name;

                if (i + 1 < selected.Length)
                {
                    remainingWeight -= remainingTraitChoices[selectedIndex].Weight;
                    remainingTraitChoices.RemoveAt(selectedIndex);
                }
            }
            
            return selected;
        }

        public string Name
        {
            get;
            private set;
        }

        public int SelectionCount
        {
            get;
            private set;
        }

        private readonly List<Trait> m_traits = new List<Trait>();
        private int m_totalWeight;
    }
}