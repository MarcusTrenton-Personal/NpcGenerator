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

namespace NpcGenerator
{
    public class TraitCategory : IGuardedByRequirement
    {
        public TraitCategory(string name) : this(name, outputName: name, selectionCount: 1)
        {
        }

        public TraitCategory(string name, int selectionCount) : this(name, outputName: name, selectionCount: selectionCount)
        {
        }

        public TraitCategory(string name, string outputName, int selectionCount)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (outputName is null)
            {
                throw new ArgumentNullException(nameof(outputName));
            }
            if (selectionCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(selectionCount), "Must be 0 or greater");
            }

            Name = name;
            OutputName = outputName;
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

        public TraitChooser CreateTraitChooser(IRandom random)
        {
            return new TraitChooser(m_traits, Name, random);
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

        public HashSet<string> BonusSelectionCategoryNames()
        {
            HashSet<string> categories = new HashSet<string>();
            foreach (Trait trait in m_traits)
            {
                if (trait.BonusSelection != null)
                {
                    categories.Add(trait.BonusSelection.CategoryName);
                }
            }

            return categories;
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

        public HashSet<string> DependentCategoryNames()
        {
            HashSet<string> dependentCategories = TraitDependenciesOnCategories();
            dependentCategories.Remove(Name); //Mutually exclusive traits in the same category are not dependencies for inter-category ordering.

            HashSet<string> localCategoryDependencies = m_requirement == null ? new HashSet<string>() : m_requirement.DependentCategoryNames();
            dependentCategories.UnionWith(localCategoryDependencies);

            return dependentCategories;
        }

        public bool HasIntraCategoryTraitDependencies()
        {
            HashSet<string> dependentCategories = TraitDependenciesOnCategories();
            bool selfDedependency = dependentCategories.Contains(Name);
            return selfDedependency;
        }

        private HashSet<string> TraitDependenciesOnCategories()
        {
            HashSet<string> dependentCategories = new HashSet<string>();
            foreach (Trait trait in m_traits)
            {
                HashSet<string> traitDependentCategories = trait.DependentCategoryNames();
                dependentCategories.UnionWith(traitDependentCategories);
            }
            return dependentCategories;
        }

        public string Name { get; private set; }

        public string OutputName { get; private set; }

        public int DefaultSelectionCount { get; private set; }

        private List<Trait> m_traits = new List<Trait>();
        private Requirement m_requirement;
    }
}