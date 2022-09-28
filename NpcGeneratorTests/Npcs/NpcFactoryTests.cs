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
    public class NpcFactoryTests
    {
        [TestMethod]
        public void CreateSingleNpc()
        {
            const string CATEGORY = "Colour";
            const string TRAIT = "Blue";
            TraitCategory colourCategory = new TraitCategory(CATEGORY);
            colourCategory.Add(new Trait(TRAIT));

            TraitSchema schema = new TraitSchema();
            schema.Add(colourCategory);

            NpcGroup npcGroup = NpcFactory.Create(schema, 1, new List<Replacement>(), m_random);
            
            Assert.IsNotNull(npcGroup, "Failed to create an npc using a valid schema");
            Assert.AreEqual(1, npcGroup.NpcCount, "Wrong number of npcs created.");

            Npc npc = npcGroup.GetNpcAtIndex(0);
            string[] traits = npc.GetTraitsOfCategory(CATEGORY);
            Assert.AreEqual(1, traits.Length, "Wrong number of traits in category");
            Assert.AreEqual(TRAIT, traits[0], "Npc created with incorrect trait in category");
        }

        [TestMethod]
        public void CreateMultipleNpcs()
        {
            const string CATEGORY = "Colour";
            const string TRAIT = "Blue";
            TraitCategory colourCategory = new TraitCategory(CATEGORY);
            colourCategory.Add(new Trait(TRAIT));

            TraitSchema schema = new TraitSchema();
            schema.Add(colourCategory);

            NpcGroup npcGroup = NpcFactory.Create(schema, 2, new List<Replacement>(), m_random);

            Assert.IsNotNull(npcGroup, "Failed to create an npc using a valid schema");
            Assert.AreEqual(2, npcGroup.NpcCount, "Wrong number of npcs created.");

            Npc npc0 = npcGroup.GetNpcAtIndex(0);
            string[] traits0 = npc0.GetTraitsOfCategory(CATEGORY);
            Assert.AreEqual(1, traits0.Length, "Wrong number of traits in category");
            Assert.AreEqual(TRAIT, traits0[0], "Npc created with incorrect trait in category");

            Npc npc1 = npcGroup.GetNpcAtIndex(0);
            string[] traits1 = npc1.GetTraitsOfCategory(CATEGORY);
            Assert.AreEqual(1, traits1.Length, "Wrong number of traits in category");
            Assert.AreEqual(TRAIT, traits1[0], "Npc created with incorrect trait in category");
        }

        [TestMethod]
        public void CreateZeroNpcs()
        {
            TraitCategory colourCategory = new TraitCategory("Colour");
            colourCategory.Add(new Trait("Blue"));

            TraitSchema schema = new TraitSchema();
            schema.Add(colourCategory);

            NpcGroup npcGroup = NpcFactory.Create(schema, 0, new List<Replacement>(), m_random);

            Assert.IsNotNull(npcGroup, "Failed to create an npc using a valid schema");
            Assert.AreEqual(0, npcGroup.NpcCount, "Wrong number of npcs created.");
        }

        [TestMethod]
        public void OneCategoryTwoSelections()
        {
            const string CATEGORY = "Colour";
            const string TRAIT0 = "Blue";
            const string TRAIT1 = "Green";
            TraitCategory colourCategory = new TraitCategory(CATEGORY, 2);
            colourCategory.Add(new Trait(TRAIT0));
            colourCategory.Add(new Trait(TRAIT1));

            TraitSchema schema = new TraitSchema();
            schema.Add(colourCategory);

            NpcGroup npcGroup = NpcFactory.Create(schema, 1, new List<Replacement>(), m_random);

            Assert.IsNotNull(npcGroup, "Failed to create an npc using a valid schema");
            Assert.AreEqual(1, npcGroup.NpcCount, "Wrong number of npcs created.");

            Npc npc = npcGroup.GetNpcAtIndex(0);
            string[] traits = npc.GetTraitsOfCategory(CATEGORY);
            Assert.AreEqual(2, traits.Length, "Wrong number of traits in category");
            bool foundTrait0 = Array.FindIndex(traits, trait => trait == TRAIT0) > -1;
            Assert.IsTrue(foundTrait0, "Npc created with incorrect trait in category");
            bool foundTrait1 = Array.FindIndex(traits, trait => trait == TRAIT1) > -1;
            Assert.IsTrue(foundTrait1, "Npc created with incorrect trait in category");
        }

        [TestMethod]
        public void TwoCategoriesOneSelectionEach()
        {
            const string CATEGORY0 = "Colour";
            const string CATEGORY0_TRAIT = "Blue";
            TraitCategory colourCategory = new TraitCategory(CATEGORY0, 1);
            colourCategory.Add(new Trait(CATEGORY0_TRAIT));

            const string CATEGORY1 = "Animal";
            const string CATEGORY1_TRAIT = "Bear";
            TraitCategory animalCategory = new TraitCategory(CATEGORY1, 1);
            animalCategory.Add(new Trait(CATEGORY1_TRAIT));

            TraitSchema schema = new TraitSchema();
            schema.Add(colourCategory);
            schema.Add(animalCategory);

            NpcGroup npcGroup = NpcFactory.Create(schema, 1, new List<Replacement>(), m_random);

            Assert.IsNotNull(npcGroup, "Failed to create an npc using a valid schema");
            Assert.AreEqual(1, npcGroup.NpcCount, "Wrong number of npcs created.");

            Npc npc = npcGroup.GetNpcAtIndex(0);
            string[] traits0 = npc.GetTraitsOfCategory(CATEGORY0);
            Assert.AreEqual(1, traits0.Length, "Wrong number of traits in category");
            Assert.AreEqual(CATEGORY0_TRAIT, traits0[0], "Category has the wrong trait");

            string[] traits1 = npc.GetTraitsOfCategory(CATEGORY1);
            Assert.AreEqual(1, traits1.Length, "Wrong number of traits in category");
            Assert.AreEqual(CATEGORY1_TRAIT, traits1[0], "Category has the wrong trait");
        }

        [TestMethod]
        public void CategoryWith2TraitsAndCategoryWith1Trait()
        {
            const string CATEGORY0 = "Colour";
            const string CATEGORY0_TRAIT0 = "Blue";
            const string CATEGORY0_TRAIT1 = "Green";
            TraitCategory colourCategory = new TraitCategory(CATEGORY0, 2);
            colourCategory.Add(new Trait(CATEGORY0_TRAIT0));
            colourCategory.Add(new Trait(CATEGORY0_TRAIT1));

            const string CATEGORY1 = "Animal";
            const string CATEGORY1_TRAIT = "Bear";
            TraitCategory animalCategory = new TraitCategory(CATEGORY1, 1);
            animalCategory.Add(new Trait(CATEGORY1_TRAIT));

            TraitSchema schema = new TraitSchema();
            schema.Add(colourCategory);
            schema.Add(animalCategory);

            NpcGroup npcGroup = NpcFactory.Create(schema, 1, new List<Replacement>(), m_random);

            Assert.IsNotNull(npcGroup, "Failed to create an npc using a valid schema");
            Assert.AreEqual(1, npcGroup.NpcCount, "Wrong number of npcs created.");

            Npc npc = npcGroup.GetNpcAtIndex(0);
            string[] traits0 = npc.GetTraitsOfCategory(CATEGORY0);
            Assert.AreEqual(2, traits0.Length, "Wrong number of traits in category");
            bool foundTrait0 = Array.FindIndex(traits0, trait => trait == CATEGORY0_TRAIT0) > -1;
            Assert.IsTrue(foundTrait0, "Npc created with incorrect trait in category");
            bool foundTrait1 = Array.FindIndex(traits0, trait => trait == CATEGORY0_TRAIT1) > -1;
            Assert.IsTrue(foundTrait1, "Npc created with incorrect trait in category");

            string[] traits1 = npc.GetTraitsOfCategory(CATEGORY1);
            Assert.AreEqual(1, traits1.Length, "Wrong number of traits in category");
            Assert.AreEqual(CATEGORY1_TRAIT, traits1[0], "Category has the wrong trait");
        }

        [TestMethod]
        public void NullSchema()
        {
            bool threwException = false;
            try
            {
                NpcGroup npcGroup = NpcFactory.Create(null, 1, new List<Replacement>(), m_random);
            }
            catch (Exception)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Null schema did not throw exception.");
        }

        [TestMethod]
        public void NegativeNpcCount()
        {
            const string CATEGORY = "Colour";
            const string TRAIT = "Blue";
            TraitCategory colourCategory = new TraitCategory(CATEGORY);
            colourCategory.Add(new Trait(TRAIT));

            TraitSchema schema = new TraitSchema();
            schema.Add(colourCategory);

            bool threwException = false;
            try
            {
                NpcGroup npcGroup = NpcFactory.Create(schema, -1, new List<Replacement>(), m_random);
            }
            catch (Exception)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Null schema did not throw exception.");
        }

        [TestMethod]
        public void NullReplacements()
        {
            const string CATEGORY = "Colour";
            const string TRAIT = "Blue";
            TraitCategory colourCategory = new TraitCategory(CATEGORY);
            colourCategory.Add(new Trait(TRAIT));

            TraitSchema schema = new TraitSchema();
            schema.Add(colourCategory);

            bool threwException = false;
            try
            {
                NpcGroup npcGroup = NpcFactory.Create(schema, 1, null, m_random);
            }
            catch (Exception)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Null schema did not throw exception.");
        }

        [TestMethod]
        public void SchemaWithoutCategories()
        {
            TraitSchema schema = new TraitSchema();

            NpcGroup npcGroup = NpcFactory.Create(schema, 1, new List<Replacement>(), m_random);

            Assert.IsNotNull(npcGroup, "Failed to create an npc using a valid schema");
            Assert.AreEqual(1, npcGroup.NpcCount, "Wrong number of npcs created.");

            Npc npc = npcGroup.GetNpcAtIndex(0);
            Assert.IsNotNull(npc, "Npc should be traitless instead of null");
        }

        [TestMethod]
        public void HiddenTrait()
        {
            const string CATEGORY = "Colour";
            const string TRAIT = "Blue";
            TraitCategory colourCategory = new TraitCategory(CATEGORY);
            colourCategory.Add(new Trait(TRAIT, 1, isHidden: true));

            TraitSchema schema = new TraitSchema();
            schema.Add(colourCategory);

            NpcGroup npcGroup = NpcFactory.Create(schema, 1, new List<Replacement>(), m_random);

            Assert.IsNotNull(npcGroup, "Failed to create an npc using a valid schema");
            Assert.AreEqual(1, npcGroup.NpcCount, "Wrong number of npcs created.");

            Npc npc = npcGroup.GetNpcAtIndex(0);
            string[] traits = npc.GetTraitsOfCategory(CATEGORY);
            Assert.AreEqual(0, traits.Length, "Wrong number of traits in category");
        }

        [TestMethod]
        public void BonusSelectionForMissingCategory()
        {
            TraitCategory colourCategory = new TraitCategory("Colour");
            Trait trait = new Trait("Blue")
            {
                BonusSelection = new BonusSelection("NotInSchema", selectionCount: 1)
            };
            colourCategory.Add(trait);

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(colourCategory);

            bool threwException = false;
            try
            {
                NpcGroup npcGroup = NpcFactory.Create(traitSchema, 1, new List<Replacement>(), m_random);
            }
            catch (Exception)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Bonus selection of category not in schema did not cause exception.");
        }

        [TestMethod]
        public void BonusSelectionExceedsTraits()
        {
            TraitCategory colourCategory = new TraitCategory("Colour", 1);
            Trait trait = new Trait("Blue")
            {
                BonusSelection = new BonusSelection(colourCategory.Name, 1)
            };
            colourCategory.Add(trait);

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(colourCategory);

            bool threwException = false;
            try
            {
                NpcGroup npcGroup = NpcFactory.Create(traitSchema, 1, new List<Replacement>(), m_random);
            }
            catch (Exception)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Bonus selection in a category with no unpicked traits did not cause exception.");
        }

        [TestMethod]
        public void SingleNpcIntraCategoryBonusSelection()
        {
            const string BLUE = "Blue";
            const string GREEN = "Green";

            TraitCategory colourCategory = new TraitCategory("Colour", 1);
            Trait blue = new Trait(BLUE)
            {
                BonusSelection = new BonusSelection(colourCategory.Name, 1)
            };
            colourCategory.Add(blue);
            Trait green = new Trait(GREEN);
            colourCategory.Add(green);

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(colourCategory);

            //Mock random always selects the first trait in a category, causing bonus selection.
            NpcGroup npcGroup = NpcFactory.Create(traitSchema, 1, new List<Replacement>(), m_random);
            string[] traits = npcGroup.GetNpcAtIndex(0).GetTraitsOfCategory(colourCategory.Name);

            Assert.AreEqual(2, traits.Length, "Bonus selection did not occur.");
            Assert.IsTrue(traits[0] == BLUE || traits[1] == BLUE, "Both traits were not selected");
            Assert.IsTrue(traits[0] == GREEN || traits[1] == GREEN, "Both traits were not selected");
        }

        [TestMethod]
        public void MultipleNpcIntraCategoryBonusSelectionDoesNotCauseException()
        {
            const string BLUE = "Blue";
            const string GREEN = "Green";

            TraitCategory colourCategory = new TraitCategory("Colour", 1);
            Trait blue = new Trait(BLUE)
            {
                BonusSelection = new BonusSelection(colourCategory.Name, 1)
            };
            colourCategory.Add(blue);
            Trait green = new Trait(GREEN);
            colourCategory.Add(green);

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(colourCategory);

            const int NPC_COUNT = 3;
            //Mock random always selects the first trait in a category, causing bonus selection.
            NpcGroup npcGroup = NpcFactory.Create(traitSchema, NPC_COUNT, new List<Replacement>(), m_random);

            Assert.AreEqual(NPC_COUNT, npcGroup.NpcCount, "Wrong number of NPCs generated");
        }

        [TestMethod]
        public void InterCategoryBonusSelection()
        {
            const string BLUE = "Blue";
            const string BEAR = "Bear";

            TraitCategory animalCategory = new TraitCategory("Animal", 0);
            animalCategory.Add(new Trait(BEAR));

            TraitCategory colourCategory = new TraitCategory("Colour", 1);
            Trait blue = new Trait(BLUE)
            {
                BonusSelection = new BonusSelection(animalCategory.Name, 1)
            };
            colourCategory.Add(blue);

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(colourCategory);
            traitSchema.Add(animalCategory);

            NpcGroup npcGroup = NpcFactory.Create(traitSchema, 1, new List<Replacement>(), m_random);
            Npc npc = npcGroup.GetNpcAtIndex(0);

            string[] colours = npc.GetTraitsOfCategory(colourCategory.Name);
            Assert.AreEqual(1, colours.Length, "Normal selection did not occur.");
            Assert.AreEqual(BLUE, colours[0], "Wrong trait was selected");

            string[] animals = npc.GetTraitsOfCategory(animalCategory.Name);
            Assert.AreEqual(1, animals.Length, "Bonus selection did not occur.");
            Assert.AreEqual(BEAR, animals[0], "Wrong trait was selected");
        }

        [TestMethod]
        public void SingleNpcWithReplacement()
        {
            const string CATEGORY = "Colour";
            TraitCategory colourCategory = new TraitCategory(CATEGORY);
            Trait trait = new Trait("Blue");
            colourCategory.Add(trait);

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(colourCategory);

            const string REPLACEMENT_COLOUR = "Red";
            Replacement replacement = new Replacement(trait, REPLACEMENT_COLOUR, colourCategory);
            NpcGroup npcGroup = NpcFactory.Create(traitSchema, 1, new List<Replacement>() { replacement }, m_random);

            Assert.AreEqual(1, npcGroup.NpcCount, "Wrong number of npcs created.");
            string colourTrait = npcGroup.GetNpcAtIndex(0).GetTraitsOfCategory(CATEGORY)[0];
            Assert.AreEqual(REPLACEMENT_COLOUR, colourTrait, "Replacement was not honoured");
        }

        [TestMethod]
        public void MultipleNpcsWithReplacements()
        {
            const string CATEGORY = "Colour";
            TraitCategory colourCategory = new TraitCategory(CATEGORY);
            Trait trait = new Trait("Blue");
            colourCategory.Add(trait);

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(colourCategory);

            const string REPLACEMENT_COLOUR = "Red";
            Replacement replacement = new Replacement(trait, REPLACEMENT_COLOUR, colourCategory);
            NpcGroup npcGroup = NpcFactory.Create(traitSchema, 2, new List<Replacement>() { replacement }, m_random);

            Assert.AreEqual(2, npcGroup.NpcCount, "Wrong number of npcs created.");
            string colourTrait0 = npcGroup.GetNpcAtIndex(0).GetTraitsOfCategory(CATEGORY)[0];
            Assert.AreEqual(REPLACEMENT_COLOUR, colourTrait0, "Replacement was not honoured");
            string colourTrait1 = npcGroup.GetNpcAtIndex(1).GetTraitsOfCategory(CATEGORY)[0];
            Assert.AreEqual(REPLACEMENT_COLOUR, colourTrait1, "Replacement was not honoured");
        }

        [TestMethod]
        public void NpcFailsCategoryRequirement()
        {
            const string DEPENDENCY_CATEGORY = "Animal";
            TraitCategory dependencyCategory = new TraitCategory(DEPENDENCY_CATEGORY);
            Trait dependentTrait0 = new Trait("Bear", 1);
            dependencyCategory.Add(dependentTrait0);
            const string IMPOSSIBLE_TRAIT = "Rhino";
            Trait dependentTrait1 = new Trait(IMPOSSIBLE_TRAIT, 0);
            dependencyCategory.Add(dependentTrait1);

            const string LOCKED_CATEGORY = "Colour";
            TraitCategory lockedCategory = new TraitCategory(LOCKED_CATEGORY);
            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait hasImpossibleTrait = new NpcHasTrait(new TraitId(DEPENDENCY_CATEGORY, IMPOSSIBLE_TRAIT), npcHolder);
            lockedCategory.Set(new Requirement(hasImpossibleTrait, npcHolder));
            Trait lockedTrait = new Trait("Blue");
            lockedCategory.Add(lockedTrait);

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(dependencyCategory);
            traitSchema.Add(lockedCategory);

            NpcGroup npcGroup = NpcFactory.Create(traitSchema, 1, new List<Replacement>(), m_random);

            Assert.AreEqual(1, npcGroup.NpcCount, "Wrong number of npcs created.");
            Npc npc = npcGroup.GetNpcAtIndex(0);
            string[] lockedTraits = npc.GetTraitsOfCategory(LOCKED_CATEGORY);
            Assert.AreEqual(0, lockedTraits.Length, "Requirement was not honoured");
        }

        [TestMethod]
        public void NpcPassesSingleCategoryRequirement()
        {
            const string DEPENDENCY_CATEGORY = "Animal";
            TraitCategory dependencyCategory = new TraitCategory(DEPENDENCY_CATEGORY);
            const string DEPENDENCY_TRAIT = "Rhino";
            Trait dependentTrait = new Trait(DEPENDENCY_TRAIT);
            dependencyCategory.Add(dependentTrait);

            const string LOCKED_CATEGORY = "Colour";
            TraitCategory lockedCategory = new TraitCategory(LOCKED_CATEGORY);
            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait hasTrait = new NpcHasTrait(new TraitId(DEPENDENCY_CATEGORY, DEPENDENCY_TRAIT), npcHolder);
            lockedCategory.Set(new Requirement(hasTrait, npcHolder));
            const string LOCKED_TRAIT = "Blue";
            Trait trait = new Trait(LOCKED_TRAIT);
            lockedCategory.Add(trait);

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(dependencyCategory);
            traitSchema.Add(lockedCategory);

            NpcGroup npcGroup = NpcFactory.Create(traitSchema, 1, new List<Replacement>(), m_random);

            Assert.AreEqual(1, npcGroup.NpcCount, "Wrong number of npcs created.");
            Npc npc = npcGroup.GetNpcAtIndex(0);
            string[] lockedTraits = npc.GetTraitsOfCategory(LOCKED_CATEGORY);
            Assert.AreEqual(1, lockedTraits.Length, "Requirement was not honoured");
            Assert.AreEqual(LOCKED_TRAIT, lockedTraits[0], "Requirement was not honoured");
        }

        [TestMethod]
        public void NpcPassesChainedCategoryRequirements()
        {
            const string DEPENDENCY_CATEGORY = "Animal";
            TraitCategory dependencyCategory = new TraitCategory(DEPENDENCY_CATEGORY);
            const string DEPENDENCY_TRAIT = "Rhino";
            Trait dependentTrait = new Trait(DEPENDENCY_TRAIT);
            dependencyCategory.Add(dependentTrait);

            const string LOCKED_CATEGORY0 = "Colour";
            TraitCategory lockedCategory0 = new TraitCategory(LOCKED_CATEGORY0);
            NpcHolder npcHolder0 = new NpcHolder();
            NpcHasTrait hasTrait0 = new NpcHasTrait(new TraitId(DEPENDENCY_CATEGORY, DEPENDENCY_TRAIT), npcHolder0);
            lockedCategory0.Set(new Requirement(hasTrait0, npcHolder0));
            const string LOCKED_TRAIT0 = "Blue";
            Trait lockedTrait0 = new Trait(LOCKED_TRAIT0);
            lockedCategory0.Add(lockedTrait0);

            const string LOCKED_CATEGORY1 = "Size";
            TraitCategory lockedCategory1 = new TraitCategory(LOCKED_CATEGORY1);
            NpcHolder npcHolder1 = new NpcHolder();
            NpcHasTrait hasTrait1 = new NpcHasTrait(new TraitId(LOCKED_CATEGORY0, LOCKED_TRAIT0), npcHolder1);
            lockedCategory1.Set(new Requirement(hasTrait1, npcHolder1));
            const string LOCKED_TRAIT1 = "Small";
            Trait lockedTrait1 = new Trait(LOCKED_TRAIT1);
            lockedCategory1.Add(lockedTrait1);

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(dependencyCategory);
            traitSchema.Add(lockedCategory0);
            traitSchema.Add(lockedCategory1);

            NpcGroup npcGroup = NpcFactory.Create(traitSchema, 1, new List<Replacement>(), m_random);

            Assert.AreEqual(1, npcGroup.NpcCount, "Wrong number of npcs created.");
            Npc npc = npcGroup.GetNpcAtIndex(0);
            
            string[] locked0Traits = npc.GetTraitsOfCategory(LOCKED_CATEGORY0);
            Assert.AreEqual(1, locked0Traits.Length, "Requirement was not honoured");
            Assert.AreEqual(LOCKED_TRAIT0, locked0Traits[0], "Requirement was not honoured");

            string[] locked1Traits = npc.GetTraitsOfCategory(LOCKED_CATEGORY1);
            Assert.AreEqual(1, locked1Traits.Length, "Requirement was not honoured");
            Assert.AreEqual(LOCKED_TRAIT1, locked1Traits[0], "Requirement was not honoured");
        }

        [TestMethod]
        public void NpcPassesRequirementDueToReplacement()
        {
            const string DEPENDENCY_CATEGORY = "Animal";
            TraitCategory dependencyCategory = new TraitCategory(DEPENDENCY_CATEGORY);
            const string ORIGINAL_TRAIT = "Bear";
            Trait originalTrait = new Trait(ORIGINAL_TRAIT);
            dependencyCategory.Add(originalTrait);
            const string DEPENDENCY_TRAIT = "Rhino";
            Trait dependentTrait = new Trait(DEPENDENCY_TRAIT);
            dependencyCategory.Add(dependentTrait);

            const string LOCKED_CATEGORY = "Colour";
            TraitCategory lockedCategory = new TraitCategory(LOCKED_CATEGORY);
            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait hasTrait = new NpcHasTrait(new TraitId(DEPENDENCY_CATEGORY, DEPENDENCY_TRAIT), npcHolder);
            lockedCategory.Set(new Requirement(hasTrait, npcHolder));
            const string LOCKED_TRAIT = "Blue";
            Trait trait = new Trait(LOCKED_TRAIT);
            lockedCategory.Add(trait);

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(dependencyCategory);
            traitSchema.Add(lockedCategory);

            Replacement replacement = new Replacement(originalTrait, DEPENDENCY_TRAIT, dependencyCategory);
            NpcGroup npcGroup = NpcFactory.Create(traitSchema, 1, new List<Replacement>() { replacement }, m_random);

            Assert.AreEqual(1, npcGroup.NpcCount, "Wrong number of npcs created.");
            Npc npc = npcGroup.GetNpcAtIndex(0);
            string[] lockedTraits = npc.GetTraitsOfCategory(LOCKED_CATEGORY);
            Assert.AreEqual(1, lockedTraits.Length, "Requirement was not honoured");
            Assert.AreEqual(LOCKED_TRAIT, lockedTraits[0], "Requirement was not honoured");
        }

        [TestMethod]
        public void NpcFailsRequirementDueToReplacement()
        {
            const string DEPENDENCY_CATEGORY = "Animal";
            TraitCategory dependencyCategory = new TraitCategory(DEPENDENCY_CATEGORY);
            const string ORIGINAL_TRAIT = "Bear";
            Trait originalTrait = new Trait(ORIGINAL_TRAIT);
            dependencyCategory.Add(originalTrait);
            const string OTHER_TRAIT = "Rhino";
            Trait dependentTrait = new Trait(OTHER_TRAIT);
            dependencyCategory.Add(dependentTrait);

            const string LOCKED_CATEGORY = "Colour";
            TraitCategory lockedCategory = new TraitCategory(LOCKED_CATEGORY);
            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait hasTrait = new NpcHasTrait(new TraitId(DEPENDENCY_CATEGORY, ORIGINAL_TRAIT), npcHolder);
            lockedCategory.Set(new Requirement(hasTrait, npcHolder));
            const string LOCKED_TRAIT = "Blue";
            Trait trait = new Trait(LOCKED_TRAIT);
            lockedCategory.Add(trait);

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(dependencyCategory);
            traitSchema.Add(lockedCategory);

            Replacement replacement = new Replacement(originalTrait, OTHER_TRAIT, dependencyCategory);
            NpcGroup npcGroup = NpcFactory.Create(traitSchema, 1, new List<Replacement>() { replacement }, m_random);

            Assert.AreEqual(1, npcGroup.NpcCount, "Wrong number of npcs created.");
            Npc npc = npcGroup.GetNpcAtIndex(0);
            string[] lockedTraits = npc.GetTraitsOfCategory(LOCKED_CATEGORY);
            Assert.AreEqual(0, lockedTraits.Length, "Requirement was not honoured");
        }

        [TestMethod]
        public void NpcPassesRequirementDueToBonusSelection()
        {
            const string BONUS_GIVING_CATEGORY = "Size";
            TraitCategory bonusGivingCategory = new TraitCategory(BONUS_GIVING_CATEGORY);
            const string BONUS_GIVING_TRAIT = "Small";
            Trait bonusGivingTrait = new Trait(BONUS_GIVING_TRAIT);
            const string DEPENDENCY_CATEGORY = "Animal";
            bonusGivingTrait.BonusSelection = new BonusSelection(DEPENDENCY_CATEGORY, 1);
            bonusGivingCategory.Add(bonusGivingTrait);

            TraitCategory dependencyCategory = new TraitCategory(DEPENDENCY_CATEGORY, 0);
            const string DEPENDENCY_TRAIT = "Rhino";
            Trait dependentTrait = new Trait(DEPENDENCY_TRAIT);
            dependencyCategory.Add(dependentTrait);

            const string LOCKED_CATEGORY = "Colour";
            TraitCategory lockedCategory = new TraitCategory(LOCKED_CATEGORY);
            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait hasTrait = new NpcHasTrait(new TraitId(DEPENDENCY_CATEGORY, DEPENDENCY_TRAIT), npcHolder);
            lockedCategory.Set(new Requirement(hasTrait, npcHolder));
            const string LOCKED_TRAIT = "Blue";
            Trait trait = new Trait(LOCKED_TRAIT);
            lockedCategory.Add(trait);

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(lockedCategory);
            traitSchema.Add(dependencyCategory);
            traitSchema.Add(bonusGivingCategory);

            NpcGroup npcGroup = NpcFactory.Create(traitSchema, 1, new List<Replacement>(), m_random);

            Assert.AreEqual(1, npcGroup.NpcCount, "Wrong number of npcs created.");
            Npc npc = npcGroup.GetNpcAtIndex(0);
            string[] lockedTraits = npc.GetTraitsOfCategory(LOCKED_CATEGORY);
            Assert.AreEqual(1, lockedTraits.Length, "Requirement was not honoured");
            Assert.AreEqual(LOCKED_TRAIT, lockedTraits[0], "Requirement was not honoured");
        }

        [TestMethod]
        public void NpcFailsRequirementDueToPingPongingBonusSelections()
        {
            const string CATEGORY0 = "Size";
            const string CATEGORY1 = "Animal";

            TraitCategory category0 = new TraitCategory(CATEGORY0);
            
            const string C0T0 = "Small";
            Trait c0t0 = new Trait(C0T0);
            c0t0.BonusSelection = new BonusSelection(CATEGORY1, 1);
            category0.Add(c0t0);

            const string C0T1 = "Large";
            Trait c0t1 = new Trait(C0T1);
            c0t0.BonusSelection = new BonusSelection(CATEGORY1, 1);
            category0.Add(c0t0);

            TraitCategory category1 = new TraitCategory(CATEGORY1, 0);
            
            const string C1T0 = "Rhino";
            Trait c1t0 = new Trait(C1T0);
            c1t0.BonusSelection = new BonusSelection(CATEGORY0, 1);
            category1.Add(c1t0);

            const string C1T1 = "Bear";
            Trait c1t1 = new Trait(C1T1);
            category1.Add(c1t1);

            const string LOCKED_CATEGORY = "Colour";
            TraitCategory lockedCategory = new TraitCategory(LOCKED_CATEGORY);
            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait hasTrait = new NpcHasTrait(new TraitId(CATEGORY1, C1T1), npcHolder);
            LogicalNone logicalNone = new LogicalNone();
            logicalNone.Add(hasTrait);
            lockedCategory.Set(new Requirement(logicalNone, npcHolder));
            const string LOCKED_TRAIT = "Blue";
            Trait trait = new Trait(LOCKED_TRAIT);
            lockedCategory.Add(trait);

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(lockedCategory);
            traitSchema.Add(category1);
            traitSchema.Add(category0);

            NpcGroup npcGroup = NpcFactory.Create(traitSchema, 1, new List<Replacement>(), m_random);

            Assert.AreEqual(1, npcGroup.NpcCount, "Wrong number of npcs created.");
            Npc npc = npcGroup.GetNpcAtIndex(0);
            string[] lockedTraits = npc.GetTraitsOfCategory(LOCKED_CATEGORY);
            Assert.AreEqual(0, lockedTraits.Length, "Requirement was not honoured");
        }

        [TestMethod]
        public void BonusSelectionIntoLockedCategory()
        {
            const string DEPENDENCY_CATEGORY = "Animal";
            TraitCategory dependencyCategory = new TraitCategory(DEPENDENCY_CATEGORY, 0);
            const string DEPENDENCY_TRAIT = "Bear";
            Trait dependencyTrait = new Trait(DEPENDENCY_TRAIT);
            dependencyCategory.Add(dependencyTrait);

            const string LOCKED_CATEGORY = "Colour";
            TraitCategory lockedCategory = new TraitCategory(LOCKED_CATEGORY);
            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait hasTrait = new NpcHasTrait(new TraitId(DEPENDENCY_CATEGORY, DEPENDENCY_TRAIT), npcHolder);
            lockedCategory.Set(new Requirement(hasTrait, npcHolder));
            const string LOCKED_TRAIT = "Blue";
            Trait lockedTrait = new Trait(LOCKED_TRAIT);
            lockedCategory.Add(lockedTrait);

            const string SOURCE_CATEGORY = "Building";
            TraitCategory sourceCategory = new TraitCategory(SOURCE_CATEGORY);
            const string SOURCE_TRAIT = "School";
            Trait sourceTrait = new Trait(SOURCE_TRAIT);
            sourceTrait.BonusSelection = new BonusSelection(LOCKED_CATEGORY, 1);
            sourceCategory.Add(sourceTrait);

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(dependencyCategory);
            traitSchema.Add(lockedCategory);
            traitSchema.Add(sourceCategory);

            NpcGroup npcGroup = NpcFactory.Create(traitSchema, 1, new List<Replacement>(), m_random);

            Assert.AreEqual(1, npcGroup.NpcCount, "Wrong number of npcs created.");
            Npc npc = npcGroup.GetNpcAtIndex(0);
            string[] lockedTraits = npc.GetTraitsOfCategory(LOCKED_CATEGORY);
            Assert.AreEqual(0, lockedTraits.Length, "Requirement was not honoured");
            string[] dependencyTraits = npc.GetTraitsOfCategory(DEPENDENCY_CATEGORY);
            Assert.AreEqual(0, dependencyTraits.Length, "Wrong number of dependency traits");
            string[] sourceTraits = npc.GetTraitsOfCategory(SOURCE_CATEGORY);
            Assert.AreEqual(1, sourceTraits.Length, "Wrong number of source traits");
            Assert.AreEqual(SOURCE_TRAIT, sourceTraits[0], "Wrong source trait");
        }

        private readonly MockRandom m_random = new MockRandom();
    }
}
