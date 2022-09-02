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
            DefaultSelectionCount = selectionCount;
        }

        public TraitCategory DeepCopyWithReplacements(IReadOnlyList<Replacement> replacements)
        {
            TraitCategory copy = (TraitCategory)MemberwiseClone();
            IReadOnlyList<Replacement> replacementsForThisCategory = ReplacementsForThisCategory(replacements);
            copy.m_traits = new List<Trait>(m_traits.Count);
            for (int i = 0; i < m_traits.Count; ++i)
            {
                Trait originalTrait = m_traits[i];
                Trait replacementTrait = ReplacementTrait(originalTrait, replacementsForThisCategory);
                copy.m_traits.Add(replacementTrait);
            }
            return copy;
        }

        private static Trait ReplacementTrait(Trait originalTrait, IReadOnlyList<Replacement> replacements)
        {
            string replacementTraitName = ReplacementNameForTrait(replacements, originalTrait);
            Trait newTrait = originalTrait.DeepCopyWithRename(replacementTraitName);
            return newTrait;
        }

        private static string ReplacementNameForTrait(IReadOnlyList<Replacement> replacements, Trait trait)
        {
            Replacement replacement = ListUtil.Find(replacements, replacement => replacement.OriginalTrait == trait);
            return replacement is null ? trait.Name : replacement.ReplacementTraitName;
        }

        private IReadOnlyList<Replacement> ReplacementsForThisCategory(IReadOnlyList<Replacement> replacements)
        {
            List<Replacement> replacementsForThis = new List<Replacement>();
            foreach(Replacement replacement in replacements)
            {
                if (replacement.Category.Name == Name)
                {
                    replacementsForThis.Add(replacement);
                }
            }
            return replacementsForThis;
        }

        public void Add(Trait trait)
        {
            if (trait is null)
            {
                throw new ArgumentNullException(nameof(trait));
            }

            m_traits.Add(trait);
        }

        public void Set(Requirement requirement)
        {
            m_requirement = requirement;
        }

        public bool IsUnlockedFor(Npc npc)
        {
            if (npc is null)
            {
                throw new ArgumentNullException(nameof(npc));
            }

            bool isUnlocked = m_requirement is null || m_requirement.IsUnlockedFor(npc);
            return isUnlocked;
        }

        public TraitChooser CreateTraitChooser(IRandom random)
        {
            return new TraitChooser(m_traits, random);
        }

        public Trait GetTrait(string name)
        {
            return m_traits.Find(trait => trait.Name == name);
        }

        public bool HasTrait(string name)
        {
            return m_traits.FindIndex(trait => trait.Name == name) > -1;
        }

        public string[] GetTraitNames()
        {
            Trait[] traits = m_traits.ToArray();
            string[] names = new string[traits.Length];
            for (int i = 0; i < traits.Length; ++i)
            {
                names[i] = traits[i].Name;
            }
            return names;
        }

        public void ReplaceTraitReferences(Dictionary<TraitCategory,TraitCategory> originalsToReplacements)
        {
            foreach (Trait trait in m_traits)
            {
                if (trait.BonusSelection != null)
                {
                    TraitCategory original = trait.BonusSelection.TraitCategory;
                    bool found = originalsToReplacements.TryGetValue(original, out TraitCategory replacement);
                    if (found)
                    {
                        trait.BonusSelection = trait.BonusSelection.ShallowCopyWithNewCategory(replacement);
                    }
                }
            }
        }

        public string Name
        {
            get;
            private set;
        }

        public int DefaultSelectionCount
        {
            get;
            private set;
        }

        private Requirement m_requirement;
        private List<Trait> m_traits = new List<Trait>();
    }
}