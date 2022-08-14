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
along with this program. If not, see<https://www.gnu.org/licenses/>.*/

using Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NpcGenerator
{
    public class TraitChooser
    {
        public TraitChooser(List<Trait> traits, IRandom random)
        {
            m_remainingTraits = new List<Trait>(traits);
            m_random = random;
            foreach (Trait trait in traits)
            {
                m_remainingWeight += trait.Weight;
            }
        }

        public string[] Choose(int count, out List<BonusSelection> bonusSelections)
        {
            bonusSelections = new List<BonusSelection>();
            if (count == 0)
            {
                return Array.Empty<string>();
            }

            Trace.Assert(m_remainingTraits.Count >= count, "Not enough traits for all selections.");

            //Number of selected traits is actually variable, with a maximum of SelectionCount. If a hidden trait is selected,
            //it consumes a selection count but is not added to the selected list.
            List<string> selected = new List<string>();
            for (int i = 0; i < count; i++)
            {
                int randomSelection = m_random.Int(0, m_remainingWeight);
                int selectedIndex = -1;
                int weightCount = 0;
                for (int j = 0; j < m_remainingTraits.Count; ++j)
                {
                    weightCount += m_remainingTraits[j].Weight;
                    if (randomSelection < weightCount)
                    {
                        selectedIndex = j;
                        break;
                    }
                }
                Trace.Assert(selectedIndex >= 0, "Failed to choose a trait.");
                Trait trait = m_remainingTraits[selectedIndex];
                if (!trait.IsHidden)
                {
                    selected.Add(trait.Name);
                }
                if (trait.BonusSelection != null)
                {
                    bonusSelections.Add(trait.BonusSelection);
                }

                m_remainingWeight -= m_remainingTraits[selectedIndex].Weight;
                m_remainingTraits.RemoveAt(selectedIndex);
            }

            return selected.ToArray();
        }

        private readonly IRandom m_random;
        private readonly List<Trait> m_remainingTraits = new List<Trait>();
        private int m_remainingWeight = 0;
    }
}
