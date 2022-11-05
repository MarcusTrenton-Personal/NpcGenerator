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
    public class TraitTests
    {
        [TestMethod]
        public void ValueConstructor()
        {
            const string NAME = "Blue";
            const int WEIGHT = 1;
            const bool IS_HIDDEN = false;
            Trait trait = new Trait(NAME, WEIGHT, IS_HIDDEN);

            Assert.AreEqual(NAME, trait.Name, "Wrong name was stored");
            Assert.AreEqual(WEIGHT, trait.Weight, "Wrong weight was stored");
            Assert.AreEqual(IS_HIDDEN, trait.IsHidden, "Wrong IsHidden was stored");
            Assert.IsNull(trait.BonusSelection, "BonusSelection is not null despite not be specified");
        }

        [TestMethod]
        public void DeepCopyWithRename()
        {
            Trait original = new Trait("Blue");
            TraitCategory category = new TraitCategory("Animal", 1);
            original.BonusSelection = new BonusSelection(category.Name, 1);

            const string COPY_NAME = "Green";
            Trait copy = original.DeepCopyWithRename(COPY_NAME);

            Assert.IsFalse(ReferenceEquals(original, copy), "Original and copy are the same object, which is not a copy");
            Assert.AreEqual(COPY_NAME, copy.Name, "Name was not renamed.");
            Assert.AreEqual(original.Weight, copy.Weight, "Weight was not copied correctly");
            Assert.AreEqual(original.IsHidden, copy.IsHidden, "IsHidden was not copied correctly");
            Assert.IsFalse(ReferenceEquals(original.BonusSelection, copy.BonusSelection), 
                "Original and copy BonusSelection are the same object, which is not a deep copy");
            Assert.AreEqual(original.BonusSelection.SelectionCount, copy.BonusSelection.SelectionCount,
                "BonusSelection SelectionCount was not copied");
            Assert.AreEqual(original.BonusSelection.CategoryName, copy.BonusSelection.CategoryName, 
                "BonusSelection TraitCategory was not copied");
        }

        [TestMethod]
        public void SetIsUnlockedWithNullSetRequirement()
        {
            Trait trait = new Trait("Blue");
            trait.Set(requirement: null);

            bool isUnlocked = trait.IsUnlockedFor(new Npc());

            Assert.IsTrue(isUnlocked, "Null requirement should always unlock trait for every Npc");
        }

        [TestMethod]
        public void IsUnlockedWithoutSettingRequirement()
        {
            Trait trait = new Trait("Blue");

            bool isUnlocked = trait.IsUnlockedFor(new Npc());

            Assert.IsTrue(isUnlocked, "Null requirement should always unlock trait for every Npc");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void IsUnlockedForNullNpc()
        {
            Trait trait = new Trait("Blue");

            trait.IsUnlockedFor(npc: null);
        }

        [TestMethod]
        public void IsUnlockedForNpcTrue()
        {
            const string CATEGORY = "Colour";
            const string TRAIT = "Blue";

            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait expression = new NpcHasTrait(new TraitId(CATEGORY, TRAIT), npcHolder);
            Requirement requirement = new Requirement(expression, npcHolder);

            Trait trait = new Trait("Bear");
            trait.Set(requirement);

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT, CATEGORY) });
            bool isUnlocked = trait.IsUnlockedFor(npc);

            Assert.IsTrue(isUnlocked, "Failed to unlock trait despite meeting the requirements");
        }

        [TestMethod]
        public void IsUnlockedForNpcFalse()
        {
            const string CATEGORY = "Colour";

            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait expression = new NpcHasTrait(new TraitId(CATEGORY, "Blue"), npcHolder);
            Requirement requirement = new Requirement(expression, npcHolder);

            Trait trait = new Trait("Bear");
            trait.Set(requirement);

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait("Red", CATEGORY) });
            bool isUnlocked = trait.IsUnlockedFor(npc);

            Assert.IsFalse(isUnlocked, "Incorrectly unlocked trait despite meeting the requirements");
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

            Trait trait = new Trait("Bear");
            trait.Set(requirement);

            Npc npcWithDesiredTrait = new Npc();
            npcWithDesiredTrait.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(DESIRED_TRAIT, CATEGORY) });
            bool isUnlockedForNpcWithDesiredTrait = trait.IsUnlockedFor(npcWithDesiredTrait);
            Assert.IsTrue(isUnlockedForNpcWithDesiredTrait, "Failed to unlock trait despite meeting the requirements");

            Npc npcWithOtherTrait = new Npc();
            npcWithOtherTrait.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(OTHER_TRAIT, CATEGORY) });
            bool isUnlockedForNpcWithOtherTrait = trait.IsUnlockedFor(npcWithOtherTrait);
            Assert.IsFalse(isUnlockedForNpcWithOtherTrait, "Incorrectly unlocked trait despite meeting the requirements");
        }

        [TestMethod]
        public void IsUnlockedAfterChangingRequirements()
        {
            const string CATEGORY = "Colour";
            const string INITIAL_TRAIT = "Blue";
            const string SUBSEQUENT_TRAIT = "Orange";

            Trait trait = new Trait("Bear");
            Npc npcWithInitialTrait = new Npc();
            npcWithInitialTrait.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(INITIAL_TRAIT, CATEGORY) });

            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait initialExpression = new NpcHasTrait(new TraitId(CATEGORY, INITIAL_TRAIT), npcHolder);
            Requirement initialRequirement = new Requirement(initialExpression, npcHolder);
            trait.Set(initialRequirement);

            bool isInitialRequirementUnlockedForNpc = trait.IsUnlockedFor(npcWithInitialTrait);
            Assert.IsTrue(isInitialRequirementUnlockedForNpc, "Failed to unlock trait despite meeting the requirements");

            NpcHasTrait subsequentExpression = new NpcHasTrait(new TraitId(CATEGORY, SUBSEQUENT_TRAIT), npcHolder);
            Requirement subsequentRequirement = new Requirement(subsequentExpression, npcHolder);
            trait.Set(subsequentRequirement);

            bool isSubsequentRequirementUnlockedForNpc = trait.IsUnlockedFor(npcWithInitialTrait);
            Assert.IsFalse(isSubsequentRequirementUnlockedForNpc, "Incorrectly unlocked trait despite meeting the requirements");
        }

        [TestMethod]
        public void DependentCategoryNamesEmpty()
        {
            Trait trait = new Trait("Bear");
            HashSet<string> dependentCategoryNames = trait.DependentCategoryNames();

            Assert.AreEqual(0, dependentCategoryNames.Count, "Incorrect number of dependencies");
        }

        [TestMethod]
        public void DependentCategoryNamesSingle()
        {
            const string DEPENDENT_CATEGORY = "Colour";

            Trait trait = new Trait("Bear");

            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait npcHasTrait = new NpcHasTrait(new TraitId(DEPENDENT_CATEGORY, "Blue"), npcHolder);
            Requirement req = new Requirement(npcHasTrait, npcHolder);
            trait.Set(req);

            HashSet<string> dependentCategoryNames = trait.DependentCategoryNames();

            Assert.AreEqual(1, dependentCategoryNames.Count, "Incorrect number of dependencies");
            foreach (string dependency in dependentCategoryNames)
            {
                Assert.AreEqual(DEPENDENT_CATEGORY, dependency, "Wrong trait dependency");
            }
        }

        [TestMethod]
        public void DependentCategoryNamesChangeWithRequirements()
        {
            const string DEPENDENT_CATEGORY = "Colour";

            Trait trait = new Trait("Bear");

            HashSet<string> dependentCategoryNames0 = trait.DependentCategoryNames();
            Assert.AreEqual(0, dependentCategoryNames0.Count, "Incorrect number of dependencies");

            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait npcHasTrait = new NpcHasTrait(new TraitId(DEPENDENT_CATEGORY, "Blue"), npcHolder);
            Requirement req = new Requirement(npcHasTrait, npcHolder);
            trait.Set(req);

            HashSet<string> dependentCategoryNames1 = trait.DependentCategoryNames();

            Assert.AreEqual(1, dependentCategoryNames1.Count, "Incorrect number of dependencies");
            foreach (string dependency in dependentCategoryNames1)
            {
                Assert.AreEqual(DEPENDENT_CATEGORY, dependency, "Wrong trait dependency");
            }
        }

        [TestMethod]
        public void DependentCategoryNamesMultipleIndependent()
        {
            const string DEPENDENT_CATEGORY0 = "Colour";
            const string DEPENDENT_CATEGORY1 = "Hair";

            Trait trait = new Trait("Bear");

            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait npcHasTrait0 = new NpcHasTrait(new TraitId(DEPENDENT_CATEGORY0, "Blue"), npcHolder);
            NpcHasTrait npcHasTrait1 = new NpcHasTrait(new TraitId(DEPENDENT_CATEGORY1, "Blue"), npcHolder);
            LogicalAny expression = new LogicalAny();
            expression.Add(npcHasTrait0);
            expression.Add(npcHasTrait1);
            Requirement req = new Requirement(expression, npcHolder);
            trait.Set(req);

            HashSet<string> dependentCategoryNames = trait.DependentCategoryNames();

            Assert.AreEqual(2, dependentCategoryNames.Count, "Incorrect number of dependencies");
            Assert.IsTrue(dependentCategoryNames.Contains(DEPENDENT_CATEGORY0), "Dependencies does not contain expected element");
            Assert.IsTrue(dependentCategoryNames.Contains(DEPENDENT_CATEGORY1), "Dependencies does not contain expected element");
        }

        [TestMethod]
        public void DependentCategoryNamesMultipleOverlapping()
        {
            const string DEPENDENT_CATEGORY0 = "Colour";
            const string DEPENDENT_CATEGORY1 = "Hair";

            Trait trait = new Trait("Bear");

            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait npcHasTrait0 = new NpcHasTrait(new TraitId(DEPENDENT_CATEGORY0, "Blue"), npcHolder);
            NpcHasTrait npcHasTrait1 = new NpcHasTrait(new TraitId(DEPENDENT_CATEGORY1, "Blue"), npcHolder);
            NpcHasTrait npcHasTrait2 = new NpcHasTrait(new TraitId(DEPENDENT_CATEGORY1, "Green"), npcHolder);
            LogicalAny expression = new LogicalAny();
            expression.Add(npcHasTrait0);
            expression.Add(npcHasTrait1);
            expression.Add(npcHasTrait2);
            Requirement req = new Requirement(expression, npcHolder);
            trait.Set(req);

            HashSet<string> dependentCategoryNames = trait.DependentCategoryNames();

            Assert.AreEqual(2, dependentCategoryNames.Count, "Incorrect number of dependencies");
            Assert.IsTrue(dependentCategoryNames.Contains(DEPENDENT_CATEGORY0), "Dependencies does not contain expected element");
            Assert.IsTrue(dependentCategoryNames.Contains(DEPENDENT_CATEGORY1), "Dependencies does not contain expected element");
        }
    }
}
