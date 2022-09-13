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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NpcGenerator;
using Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    [TestClass]
    public class TraitSchemaTests
    {
        [TestMethod]
        public void EmptyTraitCategories()
        {
            TraitSchema traitSchema = new TraitSchema();
            IReadOnlyList<TraitCategory> categories = traitSchema.GetTraitCategories();

            Assert.IsNotNull(categories, "Categories are somehow null.");
            Assert.AreEqual(0, categories.Count, "Categories are not empty");
        }

        [TestMethod]
        public void SingleTraitCategory()
        {
            TraitSchema traitSchema = new TraitSchema();
            TraitCategory category = new TraitCategory("Colour", 1);
            traitSchema.Add(category);

            IReadOnlyList<TraitCategory> categories = traitSchema.GetTraitCategories();

            Assert.IsNotNull(categories, "Categories are somehow null.");
            Assert.AreEqual(1, categories.Count, "Categories count is incorrect");
            Assert.AreEqual(category, categories[0], "Wrong category stored");
        }

        [TestMethod]
        public void MultipleTraitCategories()
        {
            TraitSchema traitSchema = new TraitSchema();
            TraitCategory category0 = new TraitCategory("Colour", 1);
            traitSchema.Add(category0);
            TraitCategory category1 = new TraitCategory("Animal", 1);
            traitSchema.Add(category1);

            IReadOnlyList<TraitCategory> categories = traitSchema.GetTraitCategories();

            Assert.IsNotNull(categories, "Categories are somehow null.");
            Assert.AreEqual(2, categories.Count, "Categories count is incorrect");
            Assert.AreEqual(category0, categories[0], "Wrong category stored");
            Assert.AreEqual(category1, categories[1], "Wrong category stored");
        }

        [TestMethod]
        public void EmptyReplacements()
        {
            TraitSchema traitSchema = new TraitSchema();
            IReadOnlyList<ReplacementSearch> replacements = traitSchema.GetReplacementSearches();

            Assert.IsNotNull(replacements, "Replacements are somehow null.");
            Assert.AreEqual(0, replacements.Count, "Replacements are not empty");
        }

        [TestMethod]
        public void SingleReplacement()
        {
            TraitSchema traitSchema = new TraitSchema();
            TraitCategory category = new TraitCategory("Colour", 1);
            Trait trait = new Trait("Red");
            ReplacementSearch replacement = new ReplacementSearch(trait, category);
            traitSchema.Add(replacement);

            IReadOnlyList<ReplacementSearch> replacements = traitSchema.GetReplacementSearches();

            Assert.IsNotNull(replacements, "Replacements are somehow null.");
            Assert.AreEqual(1, replacements.Count, "Replacements are not empty");
            Assert.AreEqual(replacement, replacements[0], "Altered ReplacementSearch was stored");
        }

        [TestMethod]
        public void MultipleReplacements()
        {
            TraitSchema traitSchema = new TraitSchema();
            TraitCategory category = new TraitCategory("Colour", 1);
            Trait trait0 = new Trait("Green");
            Trait trait1 = new Trait("Red");

            ReplacementSearch replacement0 = new ReplacementSearch(trait0, category);
            traitSchema.Add(replacement0);
            ReplacementSearch replacement1 = new ReplacementSearch(trait1, category);
            traitSchema.Add(replacement1);

            IReadOnlyList<ReplacementSearch> replacements = traitSchema.GetReplacementSearches();

            Assert.IsNotNull(replacements, "Replacements are somehow null.");
            Assert.AreEqual(2, replacements.Count, "Replacements are not empty");
            Assert.AreEqual(replacement0, replacements[0], "Altered ReplacementSearch was stored");
            Assert.AreEqual(replacement1, replacements[1], "Altered ReplacementSearch was stored");
        }

        [TestMethod]
        public void HasTraitNull()
        {
            TraitSchema traitSchema = new TraitSchema();

            bool threwException = false;
            try
            {
                bool isFound = traitSchema.HasTrait(null);
            }
            catch (ArgumentNullException)
            {
                threwException = true;
            }
            
            Assert.IsTrue(threwException, "Failed to throw an ArgumentNullException for a null parameter");
        }

        [TestMethod]
        public void HasTraitNotFoundDueToEmptyCategories()
        {
            TraitSchema traitSchema = new TraitSchema();

            bool isFound = traitSchema.HasTrait(new TraitId("Animal", "Bear"));

            Assert.IsFalse(isFound, "Incorrectly found a trait in an empty schema");
        }

        [TestMethod]
        public void HasTraitNotFoundDueToTraitName()
        {
            const string CATEGORY = "Animal";

            Trait trait = new Trait("Bear");

            TraitCategory category = new TraitCategory(CATEGORY, 1);
            category.Add(trait);

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(category);

            bool isFound = traitSchema.HasTrait(new TraitId(CATEGORY, "Velociraptor"));

            Assert.IsFalse(isFound, "Incorrectly found a trait");
        }

        [TestMethod]
        public void HasTraitNotFoundDueToCategoryName()
        {
            const string TRAIT = "Black";

            Trait trait = new Trait(TRAIT);

            TraitCategory category = new TraitCategory("Race", 1);
            category.Add(trait);

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(category);

            bool isFound = traitSchema.HasTrait(new TraitId("Hair Colour", TRAIT));

            Assert.IsFalse(isFound, "Incorrectly found a trait");
        }

        [TestMethod]
        public void HasTraitFound()
        {
            const string CATEGORY = "Animal";
            const string TRAIT = "Bear";

            Trait trait = new Trait(TRAIT);

            TraitCategory category = new TraitCategory(CATEGORY, 1);
            category.Add(trait);

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(category);

            bool isFound = traitSchema.HasTrait(new TraitId(CATEGORY, TRAIT));

            Assert.IsTrue(isFound, "Failed to find trait that was in schema");
        }

        [TestMethod]
        public void HasTraitNotFoundDueToCase()
        {
            const string CATEGORY = "Animal";

            Trait trait = new Trait("Bear");

            TraitCategory category = new TraitCategory(CATEGORY, 1);
            category.Add(trait);

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(category);

            bool isFound = traitSchema.HasTrait(new TraitId(CATEGORY, "bear"));

            Assert.IsFalse(isFound, "Incorrectly found a trait");
        }

        [TestMethod]
        public void HasTraitAfterTraitAdded()
        {
            const string CATEGORY = "Animal";
            const string TRAIT = "Bear";

            Trait redHerringTrait = new Trait("Velociraptor");

            TraitCategory category = new TraitCategory(CATEGORY, 1);
            category.Add(redHerringTrait);

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(category);

            bool isFoundInitially = traitSchema.HasTrait(new TraitId(CATEGORY, TRAIT));
            Assert.IsFalse(isFoundInitially, "Incorrectly found a trait");

            Trait trait = new Trait(TRAIT);
            category.Add(trait);

            bool isFoundSubsequent = traitSchema.HasTrait(new TraitId(CATEGORY, TRAIT));
            Assert.IsTrue(isFoundSubsequent, "Failed to find trait that was in schema");
        }

        [TestMethod]
        public void HasCircularRequirementsForEmptySchema()
        {
            TraitSchema traitSchema = new TraitSchema();

            bool hasCircularRequirements = traitSchema.HasCircularRequirements(out List<TraitSchema.Dependency> cycle);

            Assert.IsFalse(hasCircularRequirements, "Incorrectly detected cycle in empty schema");
            Assert.IsNull(cycle, "Cycle list should be null when there is no cycle");
        }

        [TestMethod]
        public void HasCircularRequirementsForIsolatedCategories()
        {
            TraitSchema traitSchema = new TraitSchema();
            TraitCategory category0 = new TraitCategory("Animal", 1);
            traitSchema.Add(category0);
            TraitCategory category1 = new TraitCategory("Colour", 1);
            traitSchema.Add(category1);
            TraitCategory category2 = new TraitCategory("Age", 1);
            traitSchema.Add(category2);

            bool hasCircularRequirements = traitSchema.HasCircularRequirements(out List<TraitSchema.Dependency> cycle);

            Assert.IsFalse(hasCircularRequirements, "Incorrectly detected cycle in schema of isolated categories");
            Assert.IsNull(cycle, "Cycle list should be null when there is no cycle");
        }

        [TestMethod]
        public void HasCircularRequirementsForCategoriesWithBonusSelections()
        {
            TraitSchema traitSchema = new TraitSchema();

            TraitCategory category0 = new TraitCategory("Animal", 1);
            traitSchema.Add(category0);

            TraitCategory category1 = new TraitCategory("Colour", 1);
            traitSchema.Add(category1);

            Trait c0t0 = new Trait("Bear");
            category0.Add(c0t0);
            Trait c0t1 = new Trait("Rhino")
            {
                BonusSelection = new BonusSelection(category1.Name, 1)
            };
            category0.Add(c0t1);

            Trait c1t0 = new Trait("Blue");
            category1.Add(c1t0);
            Trait c1t1 = new Trait("Red")
            {
                BonusSelection = new BonusSelection(category0.Name, 1)
            };
            category1.Add(c1t1);

            bool hasCircularRequirements = traitSchema.HasCircularRequirements(out List<TraitSchema.Dependency> cycle);

            Assert.IsFalse(hasCircularRequirements, "Incorrectly detected cycle in schema of categories connected by only bonus selections");
            Assert.IsNull(cycle, "Cycle list should be null when there is no cycle");
        }

        [TestMethod]
        public void HasCircularRequirementsNotForCategoriesWithNonCircularRequirements()
        {
            TraitSchema traitSchema = new TraitSchema();
            TraitCategory category0 = new TraitCategory("Animal", 1);
            traitSchema.Add(category0);
            TraitCategory category1 = new TraitCategory("Colour", 1);
            traitSchema.Add(category1);
            TraitCategory category2 = new TraitCategory("Age", 1);
            traitSchema.Add(category2);

            Trait trait0 = new Trait("Bear");
            category0.Add(trait0);
            Trait trait1 = new Trait("Blue");
            category1.Add(trait1);
            Trait trait2 = new Trait("Young");
            category2.Add(trait2);

            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait hasTrait0 = new NpcHasTrait(new TraitId(category0.Name, trait0.Name), npcHolder);
            NpcHasTrait hasTrait1 = new NpcHasTrait(new TraitId(category1.Name, trait1.Name), npcHolder);
            LogicalAll logicalAll = new LogicalAll();
            logicalAll.Add(hasTrait0);
            logicalAll.Add(hasTrait1);
            Requirement requirement = new Requirement(logicalAll, npcHolder);
            category2.Set(requirement);

            bool hasCircularRequirements = traitSchema.HasCircularRequirements(out List<TraitSchema.Dependency> cycle);

            Assert.IsFalse(hasCircularRequirements, "Incorrectly detected cycle in schema of categories with non-circular requirements");
            Assert.IsNull(cycle, "Cycle list should be null when there is no cycle");
        }

        [TestMethod]
        public void HasCircularRequirementsNotForCategoriesWithNonCircularRequirementsAndBonusSelections()
        {
            TraitSchema traitSchema = new TraitSchema();
            TraitCategory category0 = new TraitCategory("Animal", 1);
            traitSchema.Add(category0);
            TraitCategory category1 = new TraitCategory("Colour", 1);
            traitSchema.Add(category1);
            TraitCategory category2 = new TraitCategory("Age", 1);
            traitSchema.Add(category2);

            Trait c0t0 = new Trait("Bear");
            category0.Add(c0t0);
            Trait c1t0 = new Trait("Blue");
            category1.Add(c1t0);
            Trait c2t0 = new Trait("Young");
            category2.Add(c2t0);

            Trait c0t1 = new Trait("Rhino")
            {
                BonusSelection = new BonusSelection(category1.Name, 1)
            };
            category0.Add(c0t1);
            Trait c1t1 = new Trait("Red")
            {
                BonusSelection = new BonusSelection(category0.Name, 1)
            };
            category1.Add(c1t1);


            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait hasTrait0 = new NpcHasTrait(new TraitId(category0.Name, c0t0.Name), npcHolder);
            NpcHasTrait hasTrait1 = new NpcHasTrait(new TraitId(category1.Name, c1t0.Name), npcHolder);
            LogicalAll logicalAll = new LogicalAll();
            logicalAll.Add(hasTrait0);
            logicalAll.Add(hasTrait1);
            Requirement requirement = new Requirement(logicalAll, npcHolder);
            category2.Set(requirement);

            bool hasCircularRequirements = traitSchema.HasCircularRequirements(out List<TraitSchema.Dependency> cycle);

            Assert.IsFalse(hasCircularRequirements, 
                "Incorrectly detected cycle in schema of categories with bonus selection and non-circular requirements");
            Assert.IsNull(cycle, "Cycle list should be null when there is no cycle");
        }

        [TestMethod]
        public void HasCircularRequirementsWithTwoCategoriesDependentOnEachOther()
        {
            TraitSchema traitSchema = new TraitSchema();
            TraitCategory category0 = new TraitCategory("Animal", 1);
            traitSchema.Add(category0);
            TraitCategory category1 = new TraitCategory("Colour", 1);
            traitSchema.Add(category1);

            Trait trait0 = new Trait("Bear");
            category0.Add(trait0);
            Trait trait1 = new Trait("Blue");
            category1.Add(trait1);

            NpcHolder npcHolder0 = new NpcHolder();
            NpcHasTrait hasTrait1 = new NpcHasTrait(new TraitId(category1.Name, trait1.Name), npcHolder0);
            Requirement requirement0 = new Requirement(hasTrait1, npcHolder0);
            category0.Set(requirement0);

            NpcHolder npcHolder1 = new NpcHolder();
            NpcHasTrait hasTrait0 = new NpcHasTrait(new TraitId(category0.Name, trait0.Name), npcHolder1);
            Requirement requirement1 = new Requirement(hasTrait0, npcHolder1);
            category1.Set(requirement1);

            bool hasCircularRequirements = traitSchema.HasCircularRequirements(out List<TraitSchema.Dependency> cycle);

            Assert.IsTrue(hasCircularRequirements, "Failed to detected cycle in schema");
            Assert.AreEqual(2, cycle.Count, "Cycle list has wrong number of elements");

            foreach (TraitSchema.Dependency dep in cycle)
            {
                Assert.AreEqual(TraitSchema.Dependency.Type.Requirement, dep.DependencyType, "Wrong dependency type");

                if (dep.OriginalCategory == category0.Name)
                {
                    Assert.AreEqual(category1.Name, dep.DependentCategory, "Wrong dependent category");
                }
                else
                {
                    Assert.AreEqual(dep.OriginalCategory, category1.Name, "Incorrect OriginalCategory name in Dependency");
                    Assert.AreEqual(category0.Name, dep.DependentCategory, "Wrong dependent category");
                }
            }
        }

        [TestMethod]
        public void HasCircularRequirementsDueToBonusSelection()
        {
            TraitSchema traitSchema = new TraitSchema();
            TraitCategory category0 = new TraitCategory("Animal", 1);
            traitSchema.Add(category0);
            TraitCategory category1 = new TraitCategory("Building", 1);
            traitSchema.Add(category1);
            TraitCategory category2 = new TraitCategory("Colour", 1);
            traitSchema.Add(category2);

            Trait c0t0 = new Trait("Bear");
            category0.Add(c0t0);
            Trait c1t0 = new Trait("School");
            category1.Add(c1t0);
            Trait c2t0 = new Trait("Blue");
            category2.Add(c2t0);

            Trait c0t1 = new Trait("Rhino")
            {
                BonusSelection = new BonusSelection(category1.Name, 1)
            };
            category0.Add(c0t1);


            NpcHolder npcHolder0 = new NpcHolder();
            NpcHasTrait hasTrait2 = new NpcHasTrait(new TraitId(category2.Name, c2t0.Name), npcHolder0);
            Requirement requirement0 = new Requirement(hasTrait2, npcHolder0);
            category0.Set(requirement0);

            NpcHolder npcHolder2 = new NpcHolder();
            NpcHasTrait hasTrait1 = new NpcHasTrait(new TraitId(category1.Name, c0t0.Name), npcHolder2);
            Requirement requirement2 = new Requirement(hasTrait1, npcHolder2);
            category2.Set(requirement2);

            bool hasCircularRequirements = traitSchema.HasCircularRequirements(out List<TraitSchema.Dependency> cycle);

            Assert.IsTrue(hasCircularRequirements, "Failed to detected cycle in schema");
            Assert.AreEqual(3, cycle.Count, "Cycle list has wrong number of elements");

            foreach (TraitSchema.Dependency dep in cycle)
            {
                if (dep.OriginalCategory == category0.Name)
                {
                    Assert.AreEqual(TraitSchema.Dependency.Type.BonusSelection, dep.DependencyType, "Wrong dependency type");
                    Assert.AreEqual(category1.Name, dep.DependentCategory, "Wrong dependent category");
                }
                else if (dep.OriginalCategory == category1.Name)
                {
                    Assert.AreEqual(TraitSchema.Dependency.Type.Requirement, dep.DependencyType, "Wrong dependency type");
                    Assert.AreEqual(category2.Name, dep.DependentCategory, "Wrong dependent category");
                }
                else
                {
                    Assert.AreEqual(category2.Name, dep.OriginalCategory, "Incorrect OriginalCategory name in Dependency");
                    Assert.AreEqual(TraitSchema.Dependency.Type.Requirement, dep.DependencyType, "Wrong dependency type");
                    Assert.AreEqual(category0.Name, dep.DependentCategory, "Wrong dependent category");

                }
            }
        }
    }
}
