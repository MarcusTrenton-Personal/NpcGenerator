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

namespace Tests
{
    [TestClass]
    public class TraitCategoryTests
    {
        [TestMethod]
        public void ValueConstructor()
        {
            const string NAME = "Colour";
            const int DEFAULT_SELECTIONS = 3;
            TraitCategory category = new TraitCategory(NAME, DEFAULT_SELECTIONS);

            Assert.AreEqual(NAME, category.Name, "Assigned the wrong name");
            Assert.AreEqual(DEFAULT_SELECTIONS, category.DefaultSelectionCount, "Assigned the wrong default selections");
        }

        [TestMethod]
        public void DeepCopyWithEmptyReplacements()
        {
            const string NAME = "Colour";
            const int DEFAULT_SELECTIONS = 3;
            TraitCategory original = new TraitCategory(NAME, DEFAULT_SELECTIONS);
            const string TRAIT1_NAME = "Blue";
            const string TRAIT2_NAME = "Green";
            Trait trait1 = new Trait(TRAIT1_NAME, 1, isHidden: true);
            Trait trait2 = new Trait(TRAIT2_NAME, 1, isHidden: false)
            {
                BonusSelection = new BonusSelection("Animal", 1)
            };
            original.Add(trait1);
            original.Add(trait2);

            TraitCategory copy = original.DeepCopyWithReplacements(new List<Replacement>());

            Assert.AreEqual(NAME, copy.Name, "Assigned the wrong name");
            Assert.AreEqual(DEFAULT_SELECTIONS, copy.DefaultSelectionCount, "Assigned the wrong default selections");
            
            Trait copy1 = copy.GetTrait(TRAIT1_NAME);
            Assert.AreEqual(TRAIT1_NAME, copy1.Name, TRAIT1_NAME + " was not copied correctly");
            Assert.AreEqual(trait1.Weight, copy1.Weight, TRAIT1_NAME + " was not copied correctly");
            Assert.AreEqual(trait1.IsHidden, copy1.IsHidden, TRAIT1_NAME + " was not copied correctly");
            Assert.AreEqual(trait1.BonusSelection, copy1.BonusSelection, TRAIT1_NAME + " was not copied correctly");

            Trait copy2 = copy.GetTrait(TRAIT2_NAME);
            Assert.AreEqual(TRAIT2_NAME, copy2.Name, TRAIT2_NAME + " was not copied correctly");
            Assert.AreEqual(trait2.Weight, copy2.Weight, TRAIT2_NAME + " was not copied correctly");
            Assert.AreEqual(trait2.IsHidden, copy2.IsHidden, TRAIT2_NAME + " was not copied correctly");
            Assert.AreEqual(trait2.BonusSelection.CategoryName, copy2.BonusSelection.CategoryName, 
                TRAIT2_NAME + " was not copied correctly");
            Assert.AreEqual(trait2.BonusSelection.SelectionCount, copy2.BonusSelection.SelectionCount, 
                TRAIT2_NAME + " was not copied correctly");
        }

        [TestMethod]
        public void DeepCopyWithReplacements()
        {
            const string NAME = "Colour";
            const int DEFAULT_SELECTIONS = 3;
            TraitCategory original = new TraitCategory(NAME, DEFAULT_SELECTIONS);
            const string TRAIT1_NAME = "Blue";
            const string TRAIT2_NAME = "Green";
            Trait trait1 = new Trait(TRAIT1_NAME);
            Trait trait2 = new Trait(TRAIT2_NAME);
            original.Add(trait1);
            original.Add(trait2);

            const string IRRELEVANT_TRAIT_ORIGINAL_NAME = "Lion";
            TraitCategory irreleventCategory = new TraitCategory("Animal", 1);
            Trait irreleventTrait = new Trait(IRRELEVANT_TRAIT_ORIGINAL_NAME);
            irreleventCategory.Add(irreleventTrait);

            const string TRAIT1_REPLACEMENT = "Red";
            const string IRRELEVANT_TRAIT_REPLACEMENT = "Elephant";
            List<Replacement> replacements = new List<Replacement>
            {
                new Replacement(trait1, TRAIT1_REPLACEMENT, original),
                new Replacement(irreleventTrait, IRRELEVANT_TRAIT_REPLACEMENT, irreleventCategory)
            };

            TraitCategory copy = original.DeepCopyWithReplacements(replacements);

            Assert.AreEqual(NAME, copy.Name, "Assigned the wrong name");
            Assert.AreEqual(DEFAULT_SELECTIONS, copy.DefaultSelectionCount, "Assigned the wrong default selections");

            Trait trait1WithOldName  = copy.GetTrait(TRAIT1_NAME);
            Assert.IsNull(trait1WithOldName, "Trait with old name was not removed");

            Trait trait1Replacement = copy.GetTrait(TRAIT1_REPLACEMENT);
            Assert.IsFalse(ReferenceEquals(trait1, trait1Replacement), "Deep copy did not copy trait.");
            Assert.IsNotNull(trait1Replacement, "Cannot find the replacement trait");
            Assert.AreEqual(TRAIT1_REPLACEMENT, trait1Replacement.Name, "Replacement trait has the wrong name.");

            Trait trait2Copy = copy.GetTrait(TRAIT2_NAME);
            Assert.IsFalse(ReferenceEquals(trait2, trait2Copy), "Deep copy did not copy trait.");
            Assert.IsNotNull(trait2Copy, "Cannot find the unchanged trait");
            Assert.AreEqual(TRAIT2_NAME, trait2Copy.Name, TRAIT2_NAME + " was not found in the copy");

            Assert.AreEqual(IRRELEVANT_TRAIT_ORIGINAL_NAME, irreleventCategory.GetTrait(IRRELEVANT_TRAIT_ORIGINAL_NAME).Name, 
                "Irrelevent category was changed when it shouldn't.");
        }

        [TestMethod]
        public void AddAndGetTrait()
        {
            const string TRAIT_NAME = "Blue";
            TraitCategory category = new TraitCategory("Colour");
            Trait trait = new Trait(TRAIT_NAME);
            category.Add(trait);

            Assert.AreEqual(trait, category.GetTrait(TRAIT_NAME), TRAIT_NAME + " was not found in the copy");
        }

        [TestMethod]
        public void GetTraitThatDoesNotExist()
        {
            const string TRAIT_NAME = "Blue";
            TraitCategory category = new TraitCategory("Colour");
            Trait trait = new Trait(TRAIT_NAME);
            category.Add(trait);

            Trait foundTrait = category.GetTrait("Purple");

            Assert.IsNull(foundTrait, "Getting a trait that does not exist should return null but is not");
        }

        [TestMethod]
        public void GetEmptyBonusSelectionCategories()
        {
            TraitCategory category = new TraitCategory("Animal");

            HashSet<string> bonusSelectionCategories = category.BonusSelectionCategoryNames();

            Assert.AreEqual(0, bonusSelectionCategories.Count, "Wrong number of BonusSelectionCategories");
        }

        [TestMethod]
        public void GetSingleBonusSelectionCategory()
        {
            TraitCategory category0 = new TraitCategory("Animal");
            TraitCategory category1 = new TraitCategory("Colour");

            Trait trait0 = new Trait("Bear");
            Trait trait1 = new Trait("Velociraptor")
            {
                BonusSelection = new BonusSelection(category1.Name, 1)
            };
            Trait trait2 = new Trait("Tyrannosaurus Rex")
            {
                BonusSelection = new BonusSelection(category1.Name, 1)
            };
            category0.Add(trait0);
            category0.Add(trait1);
            category0.Add(trait2);

            HashSet<string> bonusSelectionCategories = category0.BonusSelectionCategoryNames();

            Assert.AreEqual(1, bonusSelectionCategories.Count, "Wrong number of BonusSelectionCategories");
            Assert.IsTrue(bonusSelectionCategories.Contains(category1.Name), "Wrong bonus selection category");
        }

        [TestMethod]
        public void GetSingleSelfBonusSelectionCategory()
        {
            TraitCategory category = new TraitCategory("Animal");

            Trait trait0 = new Trait("Bear");
            Trait trait1 = new Trait("Velociraptor")
            {
                BonusSelection = new BonusSelection(category.Name, 1)
            };
            Trait trait2 = new Trait("Tyrannosaurus Rex")
            {
                BonusSelection = new BonusSelection(category.Name, 1)
            };
            category.Add(trait0);
            category.Add(trait1);
            category.Add(trait2);

            HashSet<string> bonusSelectionCategories = category.BonusSelectionCategoryNames();

            Assert.AreEqual(1, bonusSelectionCategories.Count, "Wrong number of BonusSelectionCategories");
            Assert.IsTrue(bonusSelectionCategories.Contains(category.Name), "Wrong bonus selection category");
        }

        [TestMethod]
        public void GetMultipleBonusSelectionCategory()
        {
            TraitCategory category0 = new TraitCategory("Animal");
            TraitCategory category1 = new TraitCategory("Colour");
            TraitCategory category2 = new TraitCategory("Shape");

            Trait trait0 = new Trait("Bear");
            Trait trait1 = new Trait("Velociraptor")
            {
                BonusSelection = new BonusSelection(category1.Name, 1)
            };
            Trait trait2 = new Trait("Tyrannosaurus Rex")
            {
                BonusSelection = new BonusSelection(category1.Name, 1)
            };
            Trait trait3 = new Trait("Ankylosaurus");
            trait2.BonusSelection = new BonusSelection(category2.Name, 1);
            category0.Add(trait0);
            category0.Add(trait1);
            category0.Add(trait2);
            category0.Add(trait3);

            HashSet<string> bonusSelectionCategories = category0.BonusSelectionCategoryNames();

            Assert.AreEqual(2, bonusSelectionCategories.Count, "Wrong number of BonusSelectionCategories");
            Assert.IsTrue(bonusSelectionCategories.Contains(category1.Name), "Wrong bonus selection category");
            Assert.IsTrue(bonusSelectionCategories.Contains(category2.Name), "Wrong bonus selection category");
        }

        [TestMethod]
        public void CreateTraitChooser()
        {
            const string TRAIT_NAME = "Blue";
            TraitCategory category = new TraitCategory("Colour");
            Trait trait = new Trait(TRAIT_NAME);
            category.Add(trait);

            TraitChooser chooser = category.CreateTraitChooser(new MockRandom());

            Assert.IsNotNull(chooser, "Chooser is null, which should be impossible");
        }

        [TestMethod]
        public void GetTraitNamesWhenEmpty()
        {
            TraitCategory category = new TraitCategory("Colour");

            string[] names = category.GetTraitNames();

            Assert.IsNotNull(names, "Incorrectly found trait names in an empty categories");
        }

        [TestMethod]
        public void GetSingleTraitName()
        {
            const string TRAIT_NAME = "Blue";
            TraitCategory category = new TraitCategory("Colour");
            Trait trait = new Trait(TRAIT_NAME);
            category.Add(trait);

            string[] names = category.GetTraitNames();

            Assert.AreEqual(1, names.Length, "Wrong number of traits names");
            Assert.AreEqual(TRAIT_NAME, names[0], "Wrong trait name");
        }

        [TestMethod]
        public void GetMultipleTraitNames()
        {
            const string TRAIT_NAME0 = "Blue";
            const string TRAIT_NAME1 = "Red";
            TraitCategory category = new TraitCategory("Colour");
            Trait trait0 = new Trait(TRAIT_NAME0);
            Trait trait1 = new Trait(TRAIT_NAME1);
            category.Add(trait0);
            category.Add(trait1);

            string[] names = category.GetTraitNames();

            Assert.AreEqual(2, names.Length, "Wrong number of traits names");
            int trait0Index = Array.FindIndex(names, name => name == TRAIT_NAME0);
            Assert.IsTrue(trait0Index > -1, TRAIT_NAME0 + " is not found in trait name list");
            int trait1Index = Array.FindIndex(names, name => name == TRAIT_NAME1);
            Assert.IsTrue(trait1Index > -1, TRAIT_NAME1 + " is not found in trait name list");
        }

        [TestMethod]
        public void SetIsUnlockedWithNullSetRequirement()
        {
            TraitCategory category = new TraitCategory("Colour");
            category.Set(requirement: null);

            bool isUnlocked = category.IsUnlockedFor(new Npc());

            Assert.IsTrue(isUnlocked, "Null requirement should always unlock category for every Npc");
        }

        [TestMethod]
        public void IsUnlockedWithoutSettingRequirement()
        {
            TraitCategory category = new TraitCategory("Colour");

            bool isUnlocked = category.IsUnlockedFor(new Npc());

            Assert.IsTrue(isUnlocked, "Null requirement should always unlock category for every Npc");
        }

        [TestMethod]
        public void IsUnlockedForNullNpc()
        {
            TraitCategory category = new TraitCategory("Colour");

            bool threwException = false;
            try
            {
                bool isUnlocked = category.IsUnlockedFor(npc: null);
            }
            catch (ArgumentNullException)
            {
                threwException = true;
            }
            
            Assert.IsTrue(threwException, "Test whether a null Npc unlocks a category should throw ArgumentNullException");
        }

        [TestMethod]
        public void IsUnlockedForNpcTrue()
        {
            const string CATEGORY = "Colour";
            const string TRAIT = "Blue";

            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait expression = new NpcHasTrait(new TraitId(CATEGORY, TRAIT), npcHolder);
            Requirement requirement = new Requirement(expression, npcHolder);

            TraitCategory category = new TraitCategory("Animal");
            category.Set(requirement);

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT) });
            bool isUnlocked = category.IsUnlockedFor(npc);

            Assert.IsTrue(isUnlocked, "Failed to unlock category despite meeting the requirements");
        }

        [TestMethod]
        public void IsUnlockedForNpcFalse()
        {
            const string CATEGORY = "Colour";

            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait expression = new NpcHasTrait(new TraitId(CATEGORY, "Blue"), npcHolder);
            Requirement requirement = new Requirement(expression, npcHolder);

            TraitCategory category = new TraitCategory("Animal");
            category.Set(requirement);

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait("Red") });
            bool isUnlocked = category.IsUnlockedFor(npc);

            Assert.IsFalse(isUnlocked, "Incorrectly unlocked category despite meeting the requirements");
        }

        [TestMethod]
        public void IsUnlockedForTwoDifferentNpcs()
        {
            const string CATEGORY = "Colour";
            const string DESIRED_TRAIT = "Blue";
            const string OTHER_TRAIT = "Orange";

            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait expression = new NpcHasTrait(new TraitId(CATEGORY, DESIRED_TRAIT), npcHolder);
            Requirement requirement = new Requirement(expression, npcHolder);

            TraitCategory category = new TraitCategory("Animal");
            category.Set(requirement);

            Npc npcWithDesiredTrait = new Npc();
            npcWithDesiredTrait.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(DESIRED_TRAIT) });
            bool isUnlockedForNpcWithDesiredTrait = category.IsUnlockedFor(npcWithDesiredTrait);
            Assert.IsTrue(isUnlockedForNpcWithDesiredTrait, "Failed to unlock category despite meeting the requirements");

            Npc npcWithOtherTrait = new Npc();
            npcWithOtherTrait.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(OTHER_TRAIT) });
            bool isUnlockedForNpcWithOtherTrait = category.IsUnlockedFor(npcWithOtherTrait);
            Assert.IsFalse(isUnlockedForNpcWithOtherTrait, "Incorrectly unlocked category despite meeting the requirements");
        }

        [TestMethod]
        public void IsUnlockedAfterChangingRequirements()
        {
            const string CATEGORY = "Colour";
            const string INITIAL_TRAIT = "Blue";
            const string SUBSEQUENT_TRAIT = "Orange";

            TraitCategory category = new TraitCategory("Animal");
            Npc npcWithInitialTrait = new Npc();
            npcWithInitialTrait.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(INITIAL_TRAIT) });

            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait initialExpression = new NpcHasTrait(new TraitId(CATEGORY, INITIAL_TRAIT), npcHolder);
            Requirement initialRequirement = new Requirement(initialExpression, npcHolder);
            category.Set(initialRequirement);

            bool isInitialRequirementUnlockedForNpc = category.IsUnlockedFor(npcWithInitialTrait);
            Assert.IsTrue(isInitialRequirementUnlockedForNpc, "Failed to unlock category despite meeting the requirements");

            NpcHasTrait subsequentExpression = new NpcHasTrait(new TraitId(CATEGORY, SUBSEQUENT_TRAIT), npcHolder);
            Requirement subsequentRequirement = new Requirement(subsequentExpression, npcHolder);
            category.Set(subsequentRequirement);

            bool isSubsequentRequirementUnlockedForNpc = category.IsUnlockedFor(npcWithInitialTrait);
            Assert.IsFalse(isSubsequentRequirementUnlockedForNpc, "Incorrectly unlocked category despite meeting the requirements");
        }

        [TestMethod]
        public void HasTraitNull()
        {
            TraitCategory category = new TraitCategory("Animal");

            bool hasTrait = category.HasTrait(name: null);
            
            Assert.IsFalse(hasTrait, "Found a null trait which should not be possible");
        }

        [TestMethod]
        public void HasTraitFound()
        {
            const string TRAIT = "Bear";
            TraitCategory category = new TraitCategory("Animal");
            category.Add(new Trait(TRAIT));

            bool hasTrait = category.HasTrait(TRAIT);

            Assert.IsTrue(hasTrait, "Did not find a trait that should have been found");
        }

        [TestMethod]
        public void HasTraitNotFound()
        {
            TraitCategory category = new TraitCategory("Animal");
            category.Add(new Trait("Bear"));

            bool hasTrait = category.HasTrait("Lion");

            Assert.IsFalse(hasTrait, "Found a trait that did not exist");
        }

        [TestMethod]
        public void HasTraitDifferentCase()
        {
            TraitCategory category = new TraitCategory("Animal");
            category.Add(new Trait("Bear"));

            bool hasTrait = category.HasTrait("bear");

            Assert.IsFalse(hasTrait, "Found a trait incorrectly ignoring case");
        }

        [TestMethod]
        public void HasTraitBeforeAndAfterTraitAdded()
        {
            const string TRAIT = "Bear";
            TraitCategory category = new TraitCategory("Animal");
            bool initialHasTrait = category.HasTrait(TRAIT);

            Assert.IsFalse(initialHasTrait, "Found a trait that did not exist");

            category.Add(new Trait(TRAIT));
            bool subsequentHasTrait = category.HasTrait(TRAIT);

            Assert.IsTrue(subsequentHasTrait, "Did not find a trait that should have been found");
        }

        [TestMethod]
        public void DependentCategoryNamesEmpty()
        {
            TraitCategory category = new TraitCategory("Animal");
            HashSet<string> dependentCategoryNames = category.DependentCategoryNames();

            Assert.AreEqual(0, dependentCategoryNames.Count, "Incorrect number of dependencies");
        }

        [TestMethod]
        public void DependentCategoryNamesSingle()
        {
            const string DEPENDENT_CATEGORY = "Colour";

            TraitCategory category = new TraitCategory("Animal");

            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait npcHasTrait = new NpcHasTrait(new TraitId(DEPENDENT_CATEGORY, "Blue"), npcHolder);
            Requirement req = new Requirement(npcHasTrait, npcHolder);
            category.Set(req);

            HashSet<string> dependentCategoryNames = category.DependentCategoryNames();

            Assert.AreEqual(1, dependentCategoryNames.Count, "Incorrect number of dependencies");
            foreach (string dependency in dependentCategoryNames)
            {
                Assert.AreEqual(DEPENDENT_CATEGORY, dependency, "Wrong category dependency");
            }
        }

        [TestMethod]
        public void DependentCategoryNamesChangeWithRequirements()
        {
            const string DEPENDENT_CATEGORY = "Colour";

            TraitCategory category = new TraitCategory("Animal");

            HashSet<string> dependentCategoryNames0 = category.DependentCategoryNames();
            Assert.AreEqual(0, dependentCategoryNames0.Count, "Incorrect number of dependencies");

            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait npcHasTrait = new NpcHasTrait(new TraitId(DEPENDENT_CATEGORY, "Blue"), npcHolder);
            Requirement req = new Requirement(npcHasTrait, npcHolder);
            category.Set(req);

            HashSet<string> dependentCategoryNames1 = category.DependentCategoryNames();

            Assert.AreEqual(1, dependentCategoryNames1.Count, "Incorrect number of dependencies");
            foreach (string dependency in dependentCategoryNames1)
            {
                Assert.AreEqual(DEPENDENT_CATEGORY, dependency, "Wrong category dependency");
            }
        }

        [TestMethod]
        public void DependentCategoryNamesMultipleIndependent()
        {
            const string DEPENDENT_CATEGORY0 = "Colour";
            const string DEPENDENT_CATEGORY1 = "Hair";

            TraitCategory category = new TraitCategory("Animal");

            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait npcHasTrait0 = new NpcHasTrait(new TraitId(DEPENDENT_CATEGORY0, "Blue"), npcHolder);
            NpcHasTrait npcHasTrait1 = new NpcHasTrait(new TraitId(DEPENDENT_CATEGORY1, "Blue"), npcHolder);
            LogicalAny expression = new LogicalAny();
            expression.Add(npcHasTrait0);
            expression.Add(npcHasTrait1);
            Requirement req = new Requirement(expression, npcHolder);
            category.Set(req);

            HashSet<string> dependentCategoryNames = category.DependentCategoryNames();

            Assert.AreEqual(2, dependentCategoryNames.Count, "Incorrect number of dependencies");
            SortedList<string, string> alphabeticalCategoryDependencies = new SortedList<string, string>();
            foreach (string dep in dependentCategoryNames)
            {
                alphabeticalCategoryDependencies.Add(dep, dep);
            }

            Assert.AreEqual(DEPENDENT_CATEGORY0, alphabeticalCategoryDependencies.Values[0], "Wrong dependency");
            Assert.AreEqual(DEPENDENT_CATEGORY1, alphabeticalCategoryDependencies.Values[1], "Wrong dependency");
        }

        [TestMethod]
        public void DependentCategoryNamesMultipleOverlapping()
        {
            const string DEPENDENT_CATEGORY0 = "Colour";
            const string DEPENDENT_CATEGORY1 = "Hair";

            TraitCategory category = new TraitCategory("Animal");

            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait npcHasTrait0 = new NpcHasTrait(new TraitId(DEPENDENT_CATEGORY0, "Blue"), npcHolder);
            NpcHasTrait npcHasTrait1 = new NpcHasTrait(new TraitId(DEPENDENT_CATEGORY1, "BLue"), npcHolder);
            NpcHasTrait npcHasTrait2 = new NpcHasTrait(new TraitId(DEPENDENT_CATEGORY1, "Green"), npcHolder);
            LogicalAny expression = new LogicalAny();
            expression.Add(npcHasTrait0);
            expression.Add(npcHasTrait1);
            expression.Add(npcHasTrait2);
            Requirement req = new Requirement(expression, npcHolder);
            category.Set(req);

            HashSet<string> dependentCategoryNames = category.DependentCategoryNames();

            Assert.AreEqual(2, dependentCategoryNames.Count, "Incorrect number of dependencies");
            SortedList<string, string> alphabeticalCategoryDependencies = new SortedList<string, string>();
            foreach (string dep in dependentCategoryNames)
            {
                alphabeticalCategoryDependencies.Add(dep, dep);
            }

            Assert.AreEqual(DEPENDENT_CATEGORY0, alphabeticalCategoryDependencies.Values[0], "Wrong dependency");
            Assert.AreEqual(DEPENDENT_CATEGORY1, alphabeticalCategoryDependencies.Values[1], "Wrong dependency");
        }
    }
}
