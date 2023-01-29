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
        public TraitCategory(string name) : this(name, outputName: name, selectionCount: 1, isHidden: false)
        {
        }

        public TraitCategory(string name, int selectionCount) : this(name, outputName: name, selectionCount: selectionCount, isHidden: false)
        {
        }

        public TraitCategory(string name, string outputName, int selectionCount) : 
            this(name, outputName: outputName, selectionCount: selectionCount, isHidden: false)
        {
        }

        public TraitCategory(string name, string outputName, int selectionCount, bool isHidden)
        {
            ParamUtil.VerifyHasContent(nameof(name), name);
            ParamUtil.VerifyHasContent(nameof(outputName), outputName);
            ParamUtil.VerifyWholeNumber(nameof(selectionCount), selectionCount);

            Name = name;
            OutputName = outputName;
            DefaultSelectionCount = selectionCount;
            IsHidden = isHidden;
        }

        public TraitCategory DeepCopyWithReplacements(in IReadOnlyList<Replacement> replacements)
        {
            ParamUtil.VerifyElementsAreNotNull(nameof(replacements), replacements);

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
            Replacement replacement = CollectionUtil.Find(replacements, replacement => replacement.OriginalTrait == trait);
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

        public void Add(in Trait trait)
        {
            ParamUtil.VerifyNotNull(nameof(trait), trait);

            m_traits.Add(trait);
        }

        public TraitChooser CreateTraitChooser(in IRandom random, in Npc npc)
        {
            ParamUtil.VerifyNotNull(nameof(random), random);
            ParamUtil.VerifyNotNull(nameof(npc), npc);

            return new TraitChooser(m_traits, Name, random, npc);
        }

        public Trait GetTrait(string name)
        {
            return m_traits.Find(trait => trait.Name == name);
        }

        public Trait GetTraitWithOriginalName(string originalName)
        {
            return m_traits.Find(trait => trait.OriginalName == originalName);
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

        public string[] GetTraitNames(Sort sortCriteria)
        {
            string[] result = sortCriteria switch
            {
                Sort.Alphabetical => GetTraitNamesSortedAlphabetically(),
                Sort.Weight => GetTraitNamesSortedByWeight(),
                Sort.Given => GetTraitNames(),
                _ => throw new InvalidOperationException("Missing implementation for sorting by " + sortCriteria)
            };
            return result;
        }

        private string[] GetTraitNamesSortedAlphabetically()
        {
            string[] names = GetTraitNames();
            Array.Sort(names);
            return names;
        }

        private class TraitWeightComparer : IComparer<Trait>
        {
            public int Compare(Trait a, Trait b)
            {
                if (a.Weight > b.Weight)
                {
                    return -1;
                }
                else if (a.Weight < b.Weight)
                {
                    return 1;
                }
                return 0;
            }
        }

        private string[] GetTraitNamesSortedByWeight()
        {
            Trait[] traits = m_traits.ToArray();
            Array.Sort(traits, new TraitWeightComparer());
            string[] names = new string[traits.Length];
            for (int i = 0; i < traits.Length; ++i)
            {
                names[i] = traits[i].Name;
            }
            return names;
        }

        public IReadOnlyList<Trait> GetTraits()
        {
            return m_traits;
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

        public void Set(in Requirement requirement)
        {
            m_requirement = requirement;
        }

        public bool HasRequirement()
        {
            return m_requirement != null;
        }

        public bool IsUnlockedFor(in Npc npc)
        {
            ParamUtil.VerifyNotNull(nameof(npc), npc);

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
        public bool IsHidden { get; private set; }

        private List<Trait> m_traits = new List<Trait>();
        private Requirement m_requirement;
    }
}