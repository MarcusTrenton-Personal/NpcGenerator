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

using Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NpcGenerator
{
    public class TraitChooser
    {
        public TraitChooser(in IReadOnlyList<Trait> traits, in string originalCategory, in IRandom random, in Npc npc)
        {
            ParamUtil.VerifyElementsAreNotNull(nameof(traits), traits);
            ParamUtil.VerifyHasContent(nameof(originalCategory), originalCategory);
            ParamUtil.VerifyNotNull(nameof(random), random);
            ParamUtil.VerifyNotNull(nameof(npc), npc);

            m_remainingTraits = new List<Trait>(traits);
            m_originalCategory = originalCategory;
            m_random = random;
            m_npc = npc;
        }

        private List<Trait> CalculateValidTraits(out int totalWeight)
        {
            List<Trait> validTraits = new List<Trait>();
            totalWeight = 0;
            foreach (Trait trait in m_remainingTraits)
            {
                bool isValid = trait.IsUnlockedFor(m_npc);
                if (isValid)
                {
                    validTraits.Add(trait);
                    totalWeight += trait.Weight;
                }
            }

            return validTraits;
        }

        public Npc.Trait[] Choose(int count, out IReadOnlyList<BonusSelection> bonusSelectionsReadonly)
        {
            List<BonusSelection> bonusSelections = new List<BonusSelection>();
            bonusSelectionsReadonly = bonusSelections.AsReadOnly();
            if (count == 0)
            {
                return Array.Empty<Npc.Trait>();
            }
            if (m_remainingTraits.Count < count)
            {
                throw new TooFewTraitsException(requested: count, available: m_remainingTraits.Count);
            }

            List<Trait> validTraits = CalculateValidTraits(out int remainingWeight);
            if (validTraits.Count < count)
            {
                throw new TooFewTraitsPassRequirementsException(requested: count, available: validTraits.Count);
            }

            List<Npc.Trait> selected = new List<Npc.Trait>();
            for (int i = 0; i < count; i++)
            {
                if (remainingWeight == 0)
                {
                    throw new NoRemainingWeightException();
                }

                int randomSelection = m_random.Int(0, remainingWeight);
                int selectedIndex = -1;
                int weightCount = 0;
                for (int j = 0; j < validTraits.Count; ++j)
                {
                    weightCount += validTraits[j].Weight;
                    if (randomSelection < weightCount)
                    {
                        selectedIndex = j;
                        break;
                    }
                }

                if (selectedIndex < 0)
                {
                    throw new FailedToChooseTrait();
                }

                Trait trait = validTraits[selectedIndex];
                selected.Add(new Npc.Trait(trait.Name, m_originalCategory, trait.IsHidden, trait.OriginalName));
                if (trait.BonusSelection != null)
                {
                    bonusSelections.Add(trait.BonusSelection);
                }

                remainingWeight -= validTraits[selectedIndex].Weight;
                validTraits.RemoveAt(selectedIndex);
            }

            m_remainingTraits.RemoveAll(trait => IsTraitSelected(trait, selected));

            return selected.ToArray();
        }

        private bool IsTraitSelected(Trait trait, List<Npc.Trait> selected)
        {
            bool isFound = selected.Find(select => select.OriginalName == trait.OriginalName) != null;
            return isFound;
        }

        private readonly IRandom m_random;
        private readonly string m_originalCategory;
        private readonly List<Trait> m_remainingTraits = new List<Trait>();
        private Npc m_npc;
    }

    public class TooFewTraitsException : ArgumentException
    {
        public TooFewTraitsException(int requested, int available)
        {
            Requested = requested;
            Available = available;
        }

        public int Requested { get; private set; }
        public int Available { get; private set; }
    }

    public class TooFewTraitsPassRequirementsException : ArgumentException
    {
        public TooFewTraitsPassRequirementsException(int requested, int available)
        {
            Requested = requested;
            Available = available;
        }

        public int Requested { get; private set; }
        public int Available { get; private set; }
    }

    public class NoRemainingWeightException : InvalidOperationException
    {
        public NoRemainingWeightException()
        {
        }
    }

    public class FailedToChooseTrait : InvalidOperationException
    {
        public FailedToChooseTrait()
        {
        }
    }
}
