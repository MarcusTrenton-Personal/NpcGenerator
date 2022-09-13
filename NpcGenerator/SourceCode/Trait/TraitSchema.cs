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
    public class TraitSchema
    {
        public void Add(TraitCategory traitCategory)
        {
            m_categories.Add(traitCategory);
        }

        public void Add(ReplacementSearch replacement)
        {
            m_replacements.Add(replacement);
        }

        public IReadOnlyList<TraitCategory> GetTraitCategories()
        {
            return m_categories;
        }

        public IReadOnlyList<ReplacementSearch> GetReplacementSearches()
        {
            return m_replacements;
        }

        public bool HasTrait(TraitId traitId)
        {
            if (traitId is null)
            {
                throw new ArgumentNullException(nameof(traitId));
            }

            TraitCategory category = m_categories.Find(category => category.Name == traitId.CategoryName);
            if (category is null)
            {
                return false;
            }

            bool hasTrait = category.HasTrait(traitId.TraitName);
            return hasTrait;
        }

        public bool HasCircularRequirements(out List<Dependency> categoryRequirementcycle)
        {
            categoryRequirementcycle = null;
            Digraph<string> requirementGraph = CalculateRequirementGraph();
            bool hasCycle = requirementGraph.HasCycle(out List<string> digraphCategoryCycle);
            if (hasCycle)
            {
                categoryRequirementcycle = DetailDependencyCycle(digraphCategoryCycle);
            }
            return hasCycle;
        }

        private Digraph<string> CalculateRequirementGraph()
        {
            Digraph<string> requirementGraph = new Digraph<string>();

            foreach (TraitCategory category in m_categories)
            {
                requirementGraph.AddNode(category.Name);

                //Direct dependencies via Requirement
                HashSet<string> dependencies = category.DependentCategoryNames();
                foreach (string dependency in dependencies)
                {
                    requirementGraph.AddEdge(dependency, category.Name);
                }

                //Indirect dependency via BonusSelection into a Required trait's category
                foreach (TraitCategory candidateBonusCategory in m_categories)
                {
                    if (candidateBonusCategory == category)
                    {
                        continue;
                    }

                    HashSet<string> bonusCategories = candidateBonusCategory.BonusSelectionCategoryNames();
                    foreach (string bonusCategory in bonusCategories)
                    {
                        bool doesCandidateCategoryHaveBonusSelectionIntoDependency = dependencies.Contains(bonusCategory);
                        if (doesCandidateCategoryHaveBonusSelectionIntoDependency)
                        {
                            //This is tricky, so let's walk through an example.
                            //If category A has a dependency on B and C has a bonus selection into B then A has a depenendcy on C too.
                            //Not B has depedency on C.
                            //B and C can have bonus selections with each other without wrecking the trait selection algorithm.
                            //The algorithm can just repeats trait selection in B and C until all bonus selections are done.
                            //But, trait selection in A cannot happen until B and C are finished trait selection, thus A depends on C.
                            requirementGraph.AddEdge(candidateBonusCategory.Name, category.Name);
                        }
                    }
                }
            }

            return requirementGraph;
        }

        //Expand the cycle to clarify bonus selection dependencies.
        //If category A requires category B and category C has a bonus selection into B then the digraph has A depend on C and B.
        //A cycle could contain ... -> C -> A -> ... hiding the relationship be A, B, and C.
        //This method detects such cases and makes them plain, so ... -> C -> B -> A -> ...
        //Resulting user-facing error messages will be much clearer.
        //Parameter cycle will have a repeat string to complete the loop: A, B, C, A. This first and last values are always the same.
        private List<Dependency> DetailDependencyCycle(List<string> cycle) 
        {
            List<Dependency> dependencies = new List<Dependency>();
            for (int i = 0; i < cycle.Count - 1; ++i)
            {
                TraitCategory successorCategory = m_categories.Find(category => category.Name == cycle[i+1]);

                //Detect easy case of direct requirement
                HashSet<string> successorDependencies = successorCategory.DependentCategoryNames();
                bool isDirectDependency = successorDependencies.Contains(cycle[i]);
                if (isDirectDependency)
                {
                    dependencies.Add(new Dependency(
                        originalCategory: cycle[i], 
                        dependentCategory: cycle[i+1], 
                        dependencyType: Dependency.Type.Requirement));
                    continue;
                }

                //Detect hard case of indirect bonus selection
                TraitCategory originalCategory = m_categories.Find(category => category.Name == cycle[i]);
                HashSet<string> bonusCategories = originalCategory.BonusSelectionCategoryNames();
                foreach (string bonusCategory in bonusCategories)
                {
                    bool isIndirectDependency = successorDependencies.Contains(bonusCategory);
                    if (isIndirectDependency)
                    {
                        dependencies.Add(new Dependency(
                            originalCategory: cycle[i],
                            dependentCategory: bonusCategory,
                            dependencyType: Dependency.Type.BonusSelection));
                        dependencies.Add(new Dependency(
                            originalCategory: bonusCategory,
                            dependentCategory: cycle[i+1],
                            dependencyType: Dependency.Type.Requirement));
                        break;
                    }
                }
            }

            return dependencies;
        }

        public class Dependency
        {
            public enum Type
            {
                Requirement,
                BonusSelection
            }

            public Dependency(string originalCategory, string dependentCategory, Type dependencyType)
            {
                OriginalCategory = originalCategory;
                DependentCategory = dependentCategory;
                DependencyType = dependencyType;
            }

            public string OriginalCategory { get; private set; }
            public string DependentCategory { get; private set; }
            public Type DependencyType { get; private set; }
        }

        private readonly List<TraitCategory> m_categories = new List<TraitCategory>();
        private readonly List<ReplacementSearch> m_replacements = new List<ReplacementSearch>();
    }
}
