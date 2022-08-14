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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NpcGenerator;
using System;
using System.Collections.Generic;
using System.Reflection;

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
            Trait trait2 = new Trait(TRAIT2_NAME, 1, isHidden: false);
            original.Add(trait1);
            original.Add(trait2);

            TraitCategory copy = original.DeepCopyWithReplacements(new List<Replacement>());

            Assert.AreEqual(NAME, copy.Name, "Assigned the wrong name");
            Assert.AreEqual(DEFAULT_SELECTIONS, copy.DefaultSelectionCount, "Assigned the wrong default selections");
            Assert.AreEqual(TRAIT1_NAME, copy.GetTrait(TRAIT1_NAME).Name, TRAIT1_NAME + " was not copied correctly");
            Assert.AreEqual(TRAIT2_NAME, copy.GetTrait(TRAIT2_NAME).Name, TRAIT2_NAME + " was not copied correctly");
        }

        [TestMethod]
        public void DeepCopyWithReplacements()
        {
            const string NAME = "Colour";
            const int DEFAULT_SELECTIONS = 3;
            TraitCategory original = new TraitCategory(NAME, DEFAULT_SELECTIONS);
            const string TRAIT1_NAME = "Blue";
            const string TRAIT2_NAME = "Green";
            Trait trait1 = new Trait(TRAIT1_NAME, 1, isHidden: true);
            Trait trait2 = new Trait(TRAIT2_NAME, 1, isHidden: false);
            original.Add(trait1);
            original.Add(trait2);

            const string IRRELEVANT_TRAIT_ORIGINAL_NAME = "Lion";
            TraitCategory irreleventCategory = new TraitCategory("Animal", 1);
            Trait irreleventTrait = new Trait(IRRELEVANT_TRAIT_ORIGINAL_NAME, 1, isHidden: false);
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
            TraitCategory category = new TraitCategory("Colour", 1);
            Trait trait = new Trait(TRAIT_NAME, 1, isHidden: true);
            category.Add(trait);

            Assert.AreEqual(trait, category.GetTrait(TRAIT_NAME), TRAIT_NAME + " was not found in the copy");
        }

        [TestMethod]
        public void GetTraitThatDoesNotExist()
        {
            const string TRAIT_NAME = "Blue";
            TraitCategory category = new TraitCategory("Colour", 1);
            Trait trait = new Trait(TRAIT_NAME, 1, isHidden: true);
            category.Add(trait);

            Trait foundTrait = category.GetTrait("Purple");

            Assert.IsNull(foundTrait, "Getting a trait that does not exist should return null but is not");
        }

        [TestMethod]
        public void ReplaceReferences()
        {
            TraitCategory oldCategory = new TraitCategory("Animal", 1);

            const string TRAIT1_NAME = "Blue";
            TraitCategory category = new TraitCategory("Colour", 1);
            Trait trait1 = new Trait(TRAIT1_NAME, 1, isHidden: true);
            BonusSelection bonusSelection1 = new BonusSelection(category, 1);
            trait1.BonusSelection = bonusSelection1;
            category.Add(trait1);

            const string TRAIT2_NAME = "Green";
            Trait trait2 = new Trait(TRAIT2_NAME, 1, isHidden: false);
            BonusSelection bonusSelection2 = new BonusSelection(oldCategory, 1);
            trait2.BonusSelection = bonusSelection2;
            category.Add(trait2);

            TraitCategory newCategory = new TraitCategory("Terrain", 1);

            Dictionary<TraitCategory, TraitCategory> replacements = new Dictionary<TraitCategory, TraitCategory>
            {
                [oldCategory] = newCategory
            };
            category.ReplaceTraitReferences(replacements);

            Assert.AreEqual(category, trait1.BonusSelection.TraitCategory, "Category was incorrectly replaced.");
            Assert.AreEqual(newCategory, trait2.BonusSelection.TraitCategory, "Category was not replaced.");
        }

        [TestMethod]
        public void CreateTraitChooser()
        {
            const string TRAIT_NAME = "Blue";
            TraitCategory category = new TraitCategory("Colour", 1);
            Trait trait = new Trait(TRAIT_NAME, 1, isHidden: true);
            category.Add(trait);

            TraitChooser chooser = category.CreateTraitChooser(new MockRandom());

            Assert.IsNotNull(chooser, "Chooser is null, which should be impossible");
        }

        [TestMethod]
        public void GetTraitNamesWhenEmpty()
        {
            TraitCategory category = new TraitCategory("Colour", 1);

            string[] names = category.GetTraitNames();

            Assert.IsNotNull(names);
        }

        [TestMethod]
        public void GetSingleTraitName()
        {
            const string TRAIT_NAME = "Blue";
            TraitCategory category = new TraitCategory("Colour", 1);
            Trait trait = new Trait(TRAIT_NAME, 1, isHidden: true);
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
            TraitCategory category = new TraitCategory("Colour", 1);
            Trait trait0 = new Trait(TRAIT_NAME0, 1, isHidden: true);
            Trait trait1 = new Trait(TRAIT_NAME1, 1, isHidden: true);
            category.Add(trait0);
            category.Add(trait1);

            string[] names = category.GetTraitNames();

            Assert.AreEqual(2, names.Length, "Wrong number of traits names");
            int trait0Index = Array.FindIndex(names, name => name == TRAIT_NAME0);
            Assert.IsTrue(trait0Index > -1, TRAIT_NAME0 + " is not found in trait name list");
            int trait1Index = Array.FindIndex(names, name => name == TRAIT_NAME1);
            Assert.IsTrue(trait1Index > -1, TRAIT_NAME1 + " is not found in trait name list");
        }
    }
}
