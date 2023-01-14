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

namespace Tests.NpcFactoryTests.Create
{
    [TestClass]
    public class NpcFactoryCreateTests
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
            Npc.Trait[] traits = npc.GetTraitsOfCategory(CATEGORY);
            Assert.AreEqual(1, traits.Length, "Wrong number of traits in category");
            Assert.AreEqual(TRAIT, traits[0].Name, "Npc created with incorrect trait");
            Assert.AreEqual(TRAIT, traits[0].OriginalName, "Npc created with incorrect trait original name");
            Assert.IsFalse(traits[0].IsHidden, "Npc created with incorrect trait in category");
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
            Npc.Trait[] traits0 = npc0.GetTraitsOfCategory(CATEGORY);
            Assert.AreEqual(1, traits0.Length, "Wrong number of traits in category");
            Assert.AreEqual(TRAIT, traits0[0].Name, "Npc created with incorrect trait in category");
            Assert.IsFalse(traits0[0].IsHidden, "Npc created with incorrect trait in category");

            Npc npc1 = npcGroup.GetNpcAtIndex(0);
            Npc.Trait[] traits1 = npc1.GetTraitsOfCategory(CATEGORY);
            Assert.AreEqual(1, traits1.Length, "Wrong number of traits in category");
            Assert.AreEqual(TRAIT, traits1[0].Name, "Npc created with incorrect trait in category");
            Assert.IsFalse(traits1[0].IsHidden, "Npc created with incorrect trait in category");
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
            Npc.Trait[] traits = npc.GetTraitsOfCategory(CATEGORY);
            Assert.AreEqual(2, traits.Length, "Wrong number of traits in category");
            bool foundTrait0 = Array.FindIndex(traits, trait => trait.Name == TRAIT0) > -1;
            Assert.IsTrue(foundTrait0, "Npc created with incorrect trait in category");
            bool foundTrait1 = Array.FindIndex(traits, trait => trait.Name == TRAIT1) > -1;
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
            Npc.Trait[] traits0 = npc.GetTraitsOfCategory(CATEGORY0);
            Assert.AreEqual(1, traits0.Length, "Wrong number of traits in category");
            Assert.AreEqual(CATEGORY0_TRAIT, traits0[0].Name, "Category has the wrong trait");
            Assert.IsFalse(traits0[0].IsHidden, "Npc created with incorrect trait in category");

            Npc.Trait[] traits1 = npc.GetTraitsOfCategory(CATEGORY1);
            Assert.AreEqual(1, traits1.Length, "Wrong number of traits in category");
            Assert.AreEqual(CATEGORY1_TRAIT, traits1[0].Name, "Category has the wrong trait");
            Assert.IsFalse(traits1[0].IsHidden, "Npc created with incorrect trait in category");
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
            Npc.Trait[] traits0 = npc.GetTraitsOfCategory(CATEGORY0);
            Assert.AreEqual(2, traits0.Length, "Wrong number of traits in category");
            bool foundTrait0 = Array.FindIndex(traits0, trait => trait.Name == CATEGORY0_TRAIT0) > -1;
            Assert.IsTrue(foundTrait0, "Npc created with incorrect trait in category");
            bool foundTrait1 = Array.FindIndex(traits0, trait => trait.Name == CATEGORY0_TRAIT1) > -1;
            Assert.IsTrue(foundTrait1, "Npc created with incorrect trait in category");

            Npc.Trait[] traits1 = npc.GetTraitsOfCategory(CATEGORY1);
            Assert.AreEqual(1, traits1.Length, "Wrong number of traits in category");
            Assert.AreEqual(CATEGORY1_TRAIT, traits1[0].Name, "Category has the wrong trait");
            Assert.IsFalse(traits1[0].IsHidden, "Npc created with incorrect trait in category");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void NullSchema()
        {
            NpcFactory.Create(null, 1, new List<Replacement>(), m_random);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void NegativeNpcCount()
        {
            const string CATEGORY = "Colour";
            const string TRAIT = "Blue";
            TraitCategory colourCategory = new TraitCategory(CATEGORY);
            colourCategory.Add(new Trait(TRAIT));

            TraitSchema schema = new TraitSchema();
            schema.Add(colourCategory);

            NpcFactory.Create(schema, -1, new List<Replacement>(), m_random);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void NullReplacements()
        {
            const string CATEGORY = "Colour";
            const string TRAIT = "Blue";
            TraitCategory colourCategory = new TraitCategory(CATEGORY);
            colourCategory.Add(new Trait(TRAIT));

            TraitSchema schema = new TraitSchema();
            schema.Add(colourCategory);

            NpcFactory.Create(schema, 1, null, m_random);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void NullReplacementElement()
        {
            const string CATEGORY = "Colour";
            const string TRAIT = "Blue";
            TraitCategory colourCategory = new TraitCategory(CATEGORY);
            colourCategory.Add(new Trait(TRAIT));

            TraitSchema schema = new TraitSchema();
            schema.Add(colourCategory);

            NpcFactory.Create(schema, 1, new List<Replacement>() { null }, m_random);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void NullRandom()
        {
            const string CATEGORY = "Colour";
            const string TRAIT = "Blue";
            TraitCategory colourCategory = new TraitCategory(CATEGORY);
            colourCategory.Add(new Trait(TRAIT));

            TraitSchema schema = new TraitSchema();
            schema.Add(colourCategory);

            NpcFactory.Create(schema, 1, new List<Replacement>(), random: null);
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
            Npc.Trait[] traits = npc.GetTraitsOfCategory(CATEGORY);
            Assert.AreEqual(1, traits.Length, "Wrong number of traits in category");
            Assert.AreEqual(TRAIT, traits[0].Name, "Wrong trait in category");
            Assert.IsTrue(traits[0].IsHidden, "Trait should be hidden is not");
        }

        [TestMethod]
        public void BonusSelectionForMissingCategory()
        {
            const string MISSING_CATEGORY = "NotInSchema";
            TraitCategory colourCategory = new TraitCategory("Colour");
            Trait trait = new Trait("Blue")
            {
                BonusSelection = new BonusSelection(MISSING_CATEGORY, selectionCount: 1)
            };
            colourCategory.Add(trait);

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(colourCategory);

            bool threwException = false;
            try
            {
                NpcGroup npcGroup = NpcFactory.Create(traitSchema, 1, new List<Replacement>(), m_random);
            }
            catch (MissingBonusSelectionCategory exception)
            {
                Assert.AreEqual(MISSING_CATEGORY, exception.Category, "Incorrect missing category for bonus selection");
                threwException = true;
            }

            Assert.IsTrue(threwException, "Bonus selection of category not in schema did not cause exception.");
        }

        [TestMethod]
        public void BonusSelectionExceedsTraits()
        {
            const string CATEGORY = "Colour";
            TraitCategory colourCategory = new TraitCategory(CATEGORY, 1);
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
            catch (TooFewTraitsInCategoryException exception)
            {
                Assert.AreEqual(CATEGORY, exception.Category, "Incorrect category");
                Assert.AreEqual(1, exception.Requested, "Wrong number of requested traits");
                Assert.AreEqual(0, exception.Available, "Wrong number of available traits");
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
            Npc.Trait[] traits = npcGroup.GetNpcAtIndex(0).GetTraitsOfCategory(colourCategory.Name);

            Assert.AreEqual(2, traits.Length, "Bonus selection did not occur.");
            Assert.IsTrue(traits[0].Name == BLUE || traits[1].Name == BLUE, "Both traits were not selected");
            Assert.IsTrue(traits[0].Name == GREEN || traits[1].Name == GREEN, "Both traits were not selected");
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

            Npc.Trait[] colours = npc.GetTraitsOfCategory(colourCategory.Name);
            Assert.AreEqual(1, colours.Length, "Normal selection did not occur.");
            Assert.AreEqual(BLUE, colours[0].Name, "Wrong trait was selected");
            Assert.IsFalse(colours[0].IsHidden, "Npc created with incorrect trait in category");

            Npc.Trait[] animals = npc.GetTraitsOfCategory(animalCategory.Name);
            Assert.AreEqual(1, animals.Length, "Bonus selection did not occur.");
            Assert.AreEqual(BEAR, animals[0].Name, "Wrong trait was selected");
            Assert.IsFalse(animals[0].IsHidden, "Npc created with incorrect trait in category");
        }

        [TestMethod]
        public void SingleNpcWithReplacement()
        {
            const string CATEGORY = "Colour";
            const string ORIGINAL_NAME = "Blue";
            TraitCategory colourCategory = new TraitCategory(CATEGORY);
            Trait trait = new Trait(ORIGINAL_NAME);
            colourCategory.Add(trait);

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(colourCategory);

            const string REPLACEMENT_COLOUR = "Red";
            Replacement replacement = new Replacement(trait, REPLACEMENT_COLOUR, colourCategory);
            NpcGroup npcGroup = NpcFactory.Create(traitSchema, 1, new List<Replacement>() { replacement }, m_random);

            Assert.AreEqual(1, npcGroup.NpcCount, "Wrong number of npcs created.");
            Npc.Trait npcTrait = npcGroup.GetNpcAtIndex(0).GetTraitsOfCategory(CATEGORY)[0];
            Assert.AreEqual(REPLACEMENT_COLOUR, npcTrait.Name, "Replacement was not honoured");
            Assert.AreEqual(ORIGINAL_NAME, npcTrait.OriginalName, "Replacement original name not recored");
        }

        [TestMethod]
        public void MultipleNpcsWithReplacements()
        {
            const string CATEGORY = "Colour";
            const string ORIGINAL_NAME = "Blue";
            TraitCategory colourCategory = new TraitCategory(CATEGORY);
            Trait trait = new Trait(ORIGINAL_NAME);
            colourCategory.Add(trait);

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(colourCategory);

            const string REPLACEMENT_COLOUR = "Red";
            Replacement replacement = new Replacement(trait, REPLACEMENT_COLOUR, colourCategory);
            NpcGroup npcGroup = NpcFactory.Create(traitSchema, 2, new List<Replacement>() { replacement }, m_random);

            Assert.AreEqual(2, npcGroup.NpcCount, "Wrong number of npcs created.");
            Npc.Trait npcTrait0 = npcGroup.GetNpcAtIndex(0).GetTraitsOfCategory(CATEGORY)[0];
            Assert.AreEqual(REPLACEMENT_COLOUR, npcTrait0.Name, "Replacement was not honoured");
            Assert.AreEqual(ORIGINAL_NAME, npcTrait0.OriginalName, "Replacement original name not recored");
            Npc.Trait npcTrait1 = npcGroup.GetNpcAtIndex(1).GetTraitsOfCategory(CATEGORY)[0];
            Assert.AreEqual(REPLACEMENT_COLOUR, npcTrait1.Name, "Replacement was not honoured");
            Assert.AreEqual(ORIGINAL_NAME, npcTrait1.OriginalName, "Replacement original name not recored");
        }

        [TestMethod]
        public void ReplacementCollidesWithExistingTraitBothSelected()
        {
            const string CATEGORY = "Colour";
            const string ORIGINAL_COLOUR = "Blue";
            const string REPLACEMENT_COLOUR = "Red";
            TraitCategory colourCategory = new TraitCategory(CATEGORY, 2);
            Trait trait0 = new Trait(ORIGINAL_COLOUR);
            Trait trait1 = new Trait(REPLACEMENT_COLOUR);
            colourCategory.Add(trait0);
            colourCategory.Add(trait1);

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(colourCategory);

            Replacement replacement = new Replacement(trait0, REPLACEMENT_COLOUR, colourCategory);
            NpcGroup npcGroup = NpcFactory.Create(traitSchema, 1, new List<Replacement>() { replacement }, m_random);

            Assert.AreEqual(1, npcGroup.NpcCount, "Wrong number of npcs created.");
            Npc.Trait npcTrait = npcGroup.GetNpcAtIndex(0).GetTraitsOfCategory(CATEGORY)[0];
            Assert.AreEqual(REPLACEMENT_COLOUR, npcTrait.Name, "Replacement was not honoured");
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
            Npc.Trait[] lockedTraits = npc.GetTraitsOfCategory(LOCKED_CATEGORY);
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
            Npc.Trait[] lockedTraits = npc.GetTraitsOfCategory(LOCKED_CATEGORY);
            Assert.AreEqual(1, lockedTraits.Length, "Requirement was not honoured");
            Assert.AreEqual(LOCKED_TRAIT, lockedTraits[0].Name, "Requirement was not honoured");
            Assert.IsFalse(lockedTraits[0].IsHidden, "Npc created with incorrect trait in category");
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
            
            Npc.Trait[] locked0Traits = npc.GetTraitsOfCategory(LOCKED_CATEGORY0);
            Assert.AreEqual(1, locked0Traits.Length, "Requirement was not honoured");
            Assert.AreEqual(LOCKED_TRAIT0, locked0Traits[0].Name, "Requirement was not honoured");
            Assert.IsFalse(locked0Traits[0].IsHidden, "Npc created with incorrect trait in category");

            Npc.Trait[] locked1Traits = npc.GetTraitsOfCategory(LOCKED_CATEGORY1);
            Assert.AreEqual(1, locked1Traits.Length, "Requirement was not honoured");
            Assert.AreEqual(LOCKED_TRAIT1, locked1Traits[0].Name, "Requirement was not honoured");
            Assert.IsFalse(locked0Traits[0].IsHidden, "Npc created with incorrect trait in category");
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
            Npc.Trait[] lockedTraits = npc.GetTraitsOfCategory(LOCKED_CATEGORY);
            Assert.AreEqual(1, lockedTraits.Length, "Requirement was not honoured");
            Assert.AreEqual(LOCKED_TRAIT, lockedTraits[0].Name, "Requirement was not honoured");
            Assert.IsFalse(lockedTraits[0].IsHidden, "Npc created with incorrect trait in category");
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
            Npc.Trait[] lockedTraits = npc.GetTraitsOfCategory(LOCKED_CATEGORY);
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
            Npc.Trait[] lockedTraits = npc.GetTraitsOfCategory(LOCKED_CATEGORY);
            Assert.AreEqual(1, lockedTraits.Length, "Requirement was not honoured");
            Assert.AreEqual(LOCKED_TRAIT, lockedTraits[0].Name, "Requirement was not honoured");
            Assert.IsFalse(lockedTraits[0].IsHidden, "Npc created with incorrect trait in category");
        }

        [TestMethod]
        public void NpcFailsRequirementDueToPingPongingBonusSelections()
        {
            const string CATEGORY0 = "Size";
            const string CATEGORY1 = "Animal";

            TraitCategory category0 = new TraitCategory(CATEGORY0);
            
            const string C0T0 = "Small";
            Trait c0t0 = new Trait(C0T0)
            {
                BonusSelection = new BonusSelection(CATEGORY1, 1)
            };
            category0.Add(c0t0);

            const string C0T1 = "Large";
            Trait c0t1 = new Trait(C0T1)
            {
                BonusSelection = new BonusSelection(CATEGORY1, 1)
            };
            category0.Add(c0t1);

            TraitCategory category1 = new TraitCategory(CATEGORY1, 0);
            
            const string C1T0 = "Rhino";
            Trait c1t0 = new Trait(C1T0)
            {
                BonusSelection = new BonusSelection(CATEGORY0, 1)
            };
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
            Npc.Trait[] lockedTraits = npc.GetTraitsOfCategory(LOCKED_CATEGORY);
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
            Trait sourceTrait = new Trait(SOURCE_TRAIT)
            {
                BonusSelection = new BonusSelection(LOCKED_CATEGORY, 1)
            };
            sourceCategory.Add(sourceTrait);

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(dependencyCategory);
            traitSchema.Add(lockedCategory);
            traitSchema.Add(sourceCategory);

            NpcGroup npcGroup = NpcFactory.Create(traitSchema, 1, new List<Replacement>(), m_random);

            Assert.AreEqual(1, npcGroup.NpcCount, "Wrong number of npcs created.");
            Npc npc = npcGroup.GetNpcAtIndex(0);
            Npc.Trait[] lockedTraits = npc.GetTraitsOfCategory(LOCKED_CATEGORY);
            Assert.AreEqual(0, lockedTraits.Length, "Requirement was not honoured");
            Npc.Trait[] dependencyTraits = npc.GetTraitsOfCategory(DEPENDENCY_CATEGORY);
            Assert.AreEqual(0, dependencyTraits.Length, "Wrong number of dependency traits");
            Npc.Trait[] sourceTraits = npc.GetTraitsOfCategory(SOURCE_CATEGORY);
            Assert.AreEqual(1, sourceTraits.Length, "Wrong number of source traits");
            Assert.AreEqual(SOURCE_TRAIT, sourceTraits[0].Name, "Wrong source trait");
            Assert.IsFalse(sourceTraits[0].IsHidden, "Npc created with incorrect trait in category");
        }

        [TestMethod]
        public void IsolatedOutputCategoryName()
        {
            const string CATEGORY_NAME = "Young Fame";
            const string OUTPUT_CATEGORY_NAME = "Fame";
            const string TRAIT = "Social Media";
            TraitCategory category = new TraitCategory(CATEGORY_NAME, OUTPUT_CATEGORY_NAME, 1);
            category.Add(new Trait(TRAIT));

            TraitSchema schema = new TraitSchema();
            schema.Add(category);

            NpcGroup npcGroup = NpcFactory.Create(schema, 1, new List<Replacement>(), m_random);

            Assert.IsNotNull(npcGroup, "Failed to create an npc using a valid schema");
            Assert.AreEqual(1, npcGroup.NpcCount, "Wrong number of npcs created.");

            Npc npc = npcGroup.GetNpcAtIndex(0);
            Npc.Trait[] traitsOfOriginalName = npc.GetTraitsOfCategory(CATEGORY_NAME);
            Assert.AreEqual(0, traitsOfOriginalName.Length, "Npc traits were added to the original category name instead of the output name");
            Npc.Trait[] traitsOfOutputName = npc.GetTraitsOfCategory(OUTPUT_CATEGORY_NAME);
            Assert.AreEqual(1, traitsOfOutputName.Length, "Npc traits were not added the category output name");
            Assert.AreEqual(TRAIT, traitsOfOutputName[0].Name, "Wrong trait name added to npc category");
        }

        [TestMethod]
        public void SharedOutputCategoryName()
        {
            const string CATEGORY0_NAME = "Young Fame";
            const string CATEGORY1_NAME = "Old Fame";
            const string SHARED_OUTPUT_CATEGORY_NAME = "Fame";
            const string C0_TRAIT = "Social Media";
            const string C1_TRAIT = "Radio";
            TraitCategory youngFameCategory = new TraitCategory(CATEGORY0_NAME, SHARED_OUTPUT_CATEGORY_NAME, 1);
            youngFameCategory.Add(new Trait(C0_TRAIT));
            TraitCategory oldFameCategory = new TraitCategory(CATEGORY1_NAME, SHARED_OUTPUT_CATEGORY_NAME, 1);
            oldFameCategory.Add(new Trait(C1_TRAIT));

            TraitSchema schema = new TraitSchema();
            schema.Add(youngFameCategory);
            schema.Add(oldFameCategory);

            NpcGroup npcGroup = NpcFactory.Create(schema, 1, new List<Replacement>(), m_random);

            Assert.IsNotNull(npcGroup, "Failed to create an npc using a valid schema");
            Assert.AreEqual(1, npcGroup.NpcCount, "Wrong number of npcs created.");

            Npc npc = npcGroup.GetNpcAtIndex(0);
            Npc.Trait[] traitsOfOriginalYoungFameName = npc.GetTraitsOfCategory(CATEGORY0_NAME);
            Assert.AreEqual(0, traitsOfOriginalYoungFameName.Length,
                "Npc traits were added to the original category name instead of the output name");
            Npc.Trait[] traitsOfOriginalOldFameName = npc.GetTraitsOfCategory(CATEGORY1_NAME);
            Assert.AreEqual(0, traitsOfOriginalOldFameName.Length,
                "Npc traits were added to the original category name instead of the output name");
            Npc.Trait[] traitsOfOutputName = npc.GetTraitsOfCategory(SHARED_OUTPUT_CATEGORY_NAME);
            Assert.AreEqual(2, traitsOfOutputName.Length, "Npc traits were not added the category output name");

            Npc.Trait npcTrait0 = Array.Find(traitsOfOutputName, trait => trait.Name == C0_TRAIT);
            Assert.IsNotNull(npcTrait0, "Trait was not added to the output category");

            Npc.Trait npcTrait1 = Array.Find(traitsOfOutputName, trait => trait.Name == C1_TRAIT);
            Assert.IsNotNull(npcTrait1, "Trait was not added to the output category");
        }

        [TestMethod]
        public void CategoryOutputNameIsAnotherCategoriesName()
        {
            const string CATEGORY0_NAME = "Young Fame";
            const string CATEGORY1_NAME_AND_CATEGORY0_OUTPUT_NAME = "Fame";
            const string C0_TRAIT = "Social Media";
            const string C1_TRAIT = "Radio";
            TraitCategory youngFameCategory = new TraitCategory(CATEGORY0_NAME, CATEGORY1_NAME_AND_CATEGORY0_OUTPUT_NAME, 1);
            youngFameCategory.Add(new Trait(C0_TRAIT));
            TraitCategory fameCategory = new TraitCategory(CATEGORY1_NAME_AND_CATEGORY0_OUTPUT_NAME);
            fameCategory.Add(new Trait(C1_TRAIT));

            TraitSchema schema = new TraitSchema();
            schema.Add(youngFameCategory);
            schema.Add(fameCategory);

            NpcGroup npcGroup = NpcFactory.Create(schema, 1, new List<Replacement>(), m_random);

            Assert.IsNotNull(npcGroup, "Failed to create an npc using a valid schema");
            Assert.AreEqual(1, npcGroup.NpcCount, "Wrong number of npcs created.");

            Npc npc = npcGroup.GetNpcAtIndex(0);
            Npc.Trait[] traitsOfOriginalYoungFameName = npc.GetTraitsOfCategory(CATEGORY0_NAME);
            Assert.AreEqual(0, traitsOfOriginalYoungFameName.Length,
                "Npc traits were added to the original category name instead of the output name");
            Npc.Trait[] traitsOfOutputName = npc.GetTraitsOfCategory(CATEGORY1_NAME_AND_CATEGORY0_OUTPUT_NAME);
            Assert.AreEqual(2, traitsOfOutputName.Length, "Npc traits were not added the category output name");

            Npc.Trait npcTrait0 = Array.Find(traitsOfOutputName, trait => trait.Name == C0_TRAIT);
            Assert.IsNotNull(npcTrait0, "Trait was not added to the output category");

            Npc.Trait npcTrait1 = Array.Find(traitsOfOutputName, trait => trait.Name == C1_TRAIT);
            Assert.IsNotNull(npcTrait1, "Trait was not added to the output category");
        }

        [TestMethod]
        public void TwoCategoriesAddSameTraitToOutputCategory()
        {
            const string CATEGORY0_NAME = "Young Fame";
            const string CATEGORY1_NAME = "Old Fame";
            const string SHARED_OUTPUT_CATEGORY_NAME = "Fame";
            const string TRAIT = "Social Media";
            TraitCategory youngFameCategory = new TraitCategory(CATEGORY0_NAME, SHARED_OUTPUT_CATEGORY_NAME, 1);
            youngFameCategory.Add(new Trait(TRAIT));
            TraitCategory oldFameCategory = new TraitCategory(CATEGORY1_NAME, SHARED_OUTPUT_CATEGORY_NAME, 1);
            oldFameCategory.Add(new Trait(TRAIT));

            TraitSchema schema = new TraitSchema();
            schema.Add(youngFameCategory);
            schema.Add(oldFameCategory);

            NpcGroup npcGroup = NpcFactory.Create(schema, 1, new List<Replacement>(), m_random);

            Assert.IsNotNull(npcGroup, "Failed to create an npc using a valid schema");
            Assert.AreEqual(1, npcGroup.NpcCount, "Wrong number of npcs created.");

            Npc npc = npcGroup.GetNpcAtIndex(0);
            Npc.Trait[] traitsOfOriginalYoungFameName = npc.GetTraitsOfCategory(CATEGORY0_NAME);
            Assert.AreEqual(0, traitsOfOriginalYoungFameName.Length,
                "Npc traits were added to the original category name instead of the output name");
            Npc.Trait[] traitsOfOriginalOldFameName = npc.GetTraitsOfCategory(CATEGORY1_NAME);
            Assert.AreEqual(0, traitsOfOriginalOldFameName.Length,
                "Npc traits were added to the original category name instead of the output name");
            Npc.Trait[] traitsOfOutputName = npc.GetTraitsOfCategory(SHARED_OUTPUT_CATEGORY_NAME);
            Assert.AreEqual(2, traitsOfOutputName.Length, "Npc traits were not added the category output name");
            Npc.Trait npcTrait0 = Array.Find(traitsOfOutputName, trait => trait.Name == TRAIT && trait.OriginalCategory == CATEGORY0_NAME);
            Assert.IsNotNull(npcTrait0, "Trait was not added to the output category");
            Npc.Trait npcTrait1 = Array.Find(traitsOfOutputName, trait => trait.Name == TRAIT && trait.OriginalCategory == CATEGORY1_NAME);
            Assert.IsNotNull(npcTrait1, "Trait was not added to the output category");
        }

        [TestMethod]
        public void NpcPassesTraitRequirements()
        {
            const string REQUIRED_CATEGORY = "Animal";
            const string REQUIRED_TRAIT = "Bear";
            const string GUARDED_CATEGORY = "Colour";
            const string GUARDED_TRAIT = "Blue";
            
            TraitCategory requiredCategory = new TraitCategory(REQUIRED_CATEGORY);
            requiredCategory.Add(new Trait(REQUIRED_TRAIT));
            
            TraitCategory guardedCategory = new TraitCategory(GUARDED_CATEGORY);
            Trait guardedTrait = new Trait(GUARDED_TRAIT);
            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait npcHasTrait = new NpcHasTrait(new TraitId(REQUIRED_CATEGORY, REQUIRED_TRAIT), npcHolder);
            Requirement req = new Requirement(npcHasTrait, npcHolder);
            guardedTrait.Set(req);
            guardedCategory.Add(guardedTrait);

            TraitSchema schema = new TraitSchema();
            schema.Add(requiredCategory);
            schema.Add(guardedCategory);

            NpcGroup npcGroup = NpcFactory.Create(schema, 1, new List<Replacement>(), m_random);

            Assert.IsNotNull(npcGroup, "Failed to create an npc using a valid schema");
            Assert.AreEqual(1, npcGroup.NpcCount, "Wrong number of npcs created.");

            Npc npc = npcGroup.GetNpcAtIndex(0);

            Npc.Trait[] traitsOfRequiredCategory = npc.GetTraitsOfCategory(REQUIRED_CATEGORY);
            Assert.AreEqual(1, traitsOfRequiredCategory.Length, "Wrong number of traits in category");
            Assert.AreEqual(REQUIRED_TRAIT, traitsOfRequiredCategory[0].Name, "Wrong trait in category");

            Npc.Trait[] traitsOfGuardedCategory = npc.GetTraitsOfCategory(GUARDED_CATEGORY);
            Assert.AreEqual(1, traitsOfGuardedCategory.Length, "Wrong number of traits in category");
            Assert.AreEqual(GUARDED_TRAIT, traitsOfGuardedCategory[0].Name, "Wrong trait in category");
        }

        [TestMethod]
        public void NpcFailsTraitRequirements()
        {
            const string REQUIRED_CATEGORY = "Animal";
            const string REQUIRED_TRAIT = "Bear";
            const string GUARDED_CATEGORY = "Colour";
            const string GUARDED_TRAIT = "Blue";
            const string NONGUARDED_TRAIT = "Red";

            TraitCategory requiredCategory = new TraitCategory(REQUIRED_CATEGORY, 0);
            requiredCategory.Add(new Trait(REQUIRED_TRAIT));

            TraitCategory guardedCategory = new TraitCategory(GUARDED_CATEGORY);
            
            Trait guardedTrait = new Trait(GUARDED_TRAIT);
            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait npcHasTrait = new NpcHasTrait(new TraitId(REQUIRED_CATEGORY, REQUIRED_TRAIT), npcHolder);
            Requirement req = new Requirement(npcHasTrait, npcHolder);
            guardedTrait.Set(req);
            guardedCategory.Add(guardedTrait);

            Trait nonGuardedTrait = new Trait(NONGUARDED_TRAIT);
            guardedCategory.Add(nonGuardedTrait);

            TraitSchema schema = new TraitSchema();
            schema.Add(requiredCategory);
            schema.Add(guardedCategory);

            NpcGroup npcGroup = NpcFactory.Create(schema, 1, new List<Replacement>(), m_random);

            Assert.IsNotNull(npcGroup, "Failed to create an npc using a valid schema");
            Assert.AreEqual(1, npcGroup.NpcCount, "Wrong number of npcs created.");

            Npc npc = npcGroup.GetNpcAtIndex(0);

            Npc.Trait[] traitsOfRequiredCategory = npc.GetTraitsOfCategory(REQUIRED_CATEGORY);
            Assert.AreEqual(0, traitsOfRequiredCategory.Length, "Wrong number of traits in category");

            Npc.Trait[] traitsOfGuardedCategory = npc.GetTraitsOfCategory(GUARDED_CATEGORY);
            Assert.AreEqual(1, traitsOfGuardedCategory.Length, "Wrong number of traits in category");
            Assert.AreEqual(NONGUARDED_TRAIT, traitsOfGuardedCategory[0].Name, "Wrong trait in category");
        }

        [TestMethod, ExpectedException(typeof(TooFewTraitsPassTraitRequirementsException))]
        public void NpcFailsTraitRequirementsLeavesNoOption()
        {
            const string REQUIRED_CATEGORY = "Animal";
            const string REQUIRED_TRAIT = "Bear";
            const string GUARDED_CATEGORY = "Colour";
            const string GUARDED_TRAIT = "Blue";

            TraitCategory requiredCategory = new TraitCategory(REQUIRED_CATEGORY, 0);
            requiredCategory.Add(new Trait(REQUIRED_TRAIT));

            TraitCategory guardedCategory = new TraitCategory(GUARDED_CATEGORY);
            Trait guardedTrait = new Trait(GUARDED_TRAIT);
            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait npcHasTrait = new NpcHasTrait(new TraitId(REQUIRED_CATEGORY, REQUIRED_TRAIT), npcHolder);
            Requirement req = new Requirement(npcHasTrait, npcHolder);
            guardedTrait.Set(req);
            guardedCategory.Add(guardedTrait);

            TraitSchema schema = new TraitSchema();
            schema.Add(requiredCategory);
            schema.Add(guardedCategory);

            NpcFactory.Create(schema, 1, new List<Replacement>(), m_random);
        }

        [TestMethod]
        public void NpcPassesSomeFailsSomeTraitRequirements()
        {
            const string REQUIRED_CATEGORY = "Animal";
            const string PASSED_REQUIRED_TRAIT = "Bear";
            const string FAILED_REQUIRED_TRAIT = "Rhino";
            const string GUARDED_CATEGORY = "Colour";
            const string PASSED_GUARDED_TRAIT = "Blue";
            const string FAILED_GUARDED_TRAIT = "Red";

            TraitCategory requiredCategory = new TraitCategory(REQUIRED_CATEGORY);
            requiredCategory.Add(new Trait(PASSED_REQUIRED_TRAIT));
            requiredCategory.Add(new Trait(FAILED_REQUIRED_TRAIT));

            TraitCategory guardedCategory = new TraitCategory(GUARDED_CATEGORY);

            //Deliberately include the locked trait first to show that it is skipped over in favour of the unlocked trait.
            NpcHolder npcHolder0 = new NpcHolder();
            NpcHasTrait npcHasTrait0 = new NpcHasTrait(new TraitId(REQUIRED_CATEGORY, FAILED_REQUIRED_TRAIT), npcHolder0);
            Requirement req0 = new Requirement(npcHasTrait0, npcHolder0);
            Trait failedGuardedTrait = new Trait(FAILED_GUARDED_TRAIT);
            failedGuardedTrait.Set(req0);
            guardedCategory.Add(failedGuardedTrait);

            NpcHolder npcHolder1 = new NpcHolder();
            NpcHasTrait npcHasTrait1 = new NpcHasTrait(new TraitId(REQUIRED_CATEGORY, PASSED_REQUIRED_TRAIT), npcHolder1);
            Requirement req1 = new Requirement(npcHasTrait1, npcHolder1);
            Trait passedGuardedTrait = new Trait(PASSED_GUARDED_TRAIT);
            passedGuardedTrait.Set(req1);
            guardedCategory.Add(passedGuardedTrait);

            TraitSchema schema = new TraitSchema();
            schema.Add(requiredCategory);
            schema.Add(guardedCategory);

            NpcGroup npcGroup = NpcFactory.Create(schema, 1, new List<Replacement>(), m_random);

            Assert.IsNotNull(npcGroup, "Failed to create an npc using a valid schema");
            Assert.AreEqual(1, npcGroup.NpcCount, "Wrong number of npcs created.");

            Npc npc = npcGroup.GetNpcAtIndex(0);

            Npc.Trait[] traitsOfRequiredCategory = npc.GetTraitsOfCategory(REQUIRED_CATEGORY);
            Assert.AreEqual(1, traitsOfRequiredCategory.Length, "Wrong number of traits in category");
            Assert.AreEqual(PASSED_REQUIRED_TRAIT, traitsOfRequiredCategory[0].Name, "Wrong trait in category");

            Npc.Trait[] traitsOfGuardedCategory = npc.GetTraitsOfCategory(GUARDED_CATEGORY);
            Assert.AreEqual(1, traitsOfGuardedCategory.Length, "Wrong number of traits in category");
            Assert.AreEqual(PASSED_GUARDED_TRAIT, traitsOfGuardedCategory[0].Name, "Wrong trait in category");
        }

        [TestMethod]
        public void MutuallyExclusiveTraits()
        {
            const string CATEGORY = "Animal";
            const string MUTUALLY_EXCLUSIVE_TRAIT0 = "Bear";
            const string MUTUALLY_EXCLUSIVE_TRAIT1 = "Rhino";
            const string NON_EXCLUSIVE_TRAIT = "Velociraptor";

            TraitCategory category = new TraitCategory(CATEGORY, 2);

            //Deliberately include the locked trait first to show that it is skipped over in favour of the unlocked trait.
            NpcHolder npcHolder0 = new NpcHolder();
            NpcHasTrait npcHasMutuallyExclusiveTrait1 = new NpcHasTrait(new TraitId(CATEGORY, MUTUALLY_EXCLUSIVE_TRAIT1), npcHolder0);
            LogicalNone none0 = new LogicalNone();
            none0.Add(npcHasMutuallyExclusiveTrait1);
            Requirement req0 = new Requirement(none0, npcHolder0);
            Trait mutuallyExclusiveTrait0 = new Trait(MUTUALLY_EXCLUSIVE_TRAIT0);
            mutuallyExclusiveTrait0.Set(req0);
            category.Add(mutuallyExclusiveTrait0);

            NpcHolder npcHolder1 = new NpcHolder();
            NpcHasTrait npcHasMutuallyExclusiveTrait0 = new NpcHasTrait(new TraitId(CATEGORY, MUTUALLY_EXCLUSIVE_TRAIT0), npcHolder1);
            LogicalNone none1 = new LogicalNone();
            none1.Add(npcHasMutuallyExclusiveTrait0);
            Requirement req1 = new Requirement(none1, npcHolder1);
            Trait mutuallyExclusiveTrait1 = new Trait(MUTUALLY_EXCLUSIVE_TRAIT1);
            mutuallyExclusiveTrait1.Set(req1);
            category.Add(mutuallyExclusiveTrait1);

            category.Add(new Trait(NON_EXCLUSIVE_TRAIT));

            TraitSchema schema = new TraitSchema();
            schema.Add(category);

            NpcGroup npcGroup = NpcFactory.Create(schema, 1, new List<Replacement>(), m_random);

            Assert.IsNotNull(npcGroup, "Failed to create an npc using a valid schema");
            Assert.AreEqual(1, npcGroup.NpcCount, "Wrong number of npcs created.");

            Npc npc = npcGroup.GetNpcAtIndex(0);

            Npc.Trait[] selectedTraits = npc.GetTraitsOfCategory(CATEGORY);
            Assert.AreEqual(2, selectedTraits.Length, "Wrong number of traits in category");
            Npc.Trait selectedMutuallyExclusiveTrait0 = Array.Find(selectedTraits, element => element.Name == MUTUALLY_EXCLUSIVE_TRAIT0);
            Assert.IsNotNull(selectedMutuallyExclusiveTrait0, "Wrong trait in category");
            Npc.Trait selectedNonExclusiveTrait = Array.Find(selectedTraits, element => element.Name == NON_EXCLUSIVE_TRAIT);
            Assert.IsNotNull(selectedNonExclusiveTrait, "Wrong trait in category");
        }

        [TestMethod]
        public void MutuallyExclusiveTraitsWithBonusSelection()
        {
            const string CATEGORY = "Animal";
            const string MUTUALLY_EXCLUSIVE_TRAIT0 = "Bear";
            const string MUTUALLY_EXCLUSIVE_TRAIT1 = "Rhino";
            const string NON_EXCLUSIVE_TRAIT0 = "Velociraptor";
            const string NON_EXCLUSIVE_TRAIT1 = "Tyrannosaurus Rex";
            const string NON_EXCLUSIVE_TRAIT2 = "Baryonyx";
            const string NON_EXCLUSIVE_TRAIT3 = "Tricerotops";

            TraitCategory category = new TraitCategory(CATEGORY, 2);

            //Deliberately include the locked trait first to show that it is skipped over in favour of the unlocked trait.
            NpcHolder npcHolder0 = new NpcHolder();
            NpcHasTrait npcHasMutuallyExclusiveTrait1 = new NpcHasTrait(new TraitId(CATEGORY, MUTUALLY_EXCLUSIVE_TRAIT1), npcHolder0);
            LogicalNone none0 = new LogicalNone();
            none0.Add(npcHasMutuallyExclusiveTrait1);
            Requirement req0 = new Requirement(none0, npcHolder0);
            Trait mutuallyExclusiveTrait0 = new Trait(MUTUALLY_EXCLUSIVE_TRAIT0)
            {
                BonusSelection = new BonusSelection(CATEGORY, 2)
            };
            mutuallyExclusiveTrait0.Set(req0);
            category.Add(mutuallyExclusiveTrait0);

            NpcHolder npcHolder1 = new NpcHolder();
            NpcHasTrait npcHasMutuallyExclusiveTrait0 = new NpcHasTrait(new TraitId(CATEGORY, MUTUALLY_EXCLUSIVE_TRAIT0), npcHolder1);
            LogicalNone none1 = new LogicalNone();
            none1.Add(npcHasMutuallyExclusiveTrait0);
            Requirement req1 = new Requirement(none1, npcHolder1);
            Trait mutuallyExclusiveTrait1 = new Trait(MUTUALLY_EXCLUSIVE_TRAIT1)
            {
                BonusSelection = new BonusSelection(CATEGORY, 2)
            };
            mutuallyExclusiveTrait1.Set(req1);
            category.Add(mutuallyExclusiveTrait1);

            Trait nonExclusiveTrait0 = new Trait(NON_EXCLUSIVE_TRAIT0)
            {
                BonusSelection = new BonusSelection(CATEGORY, 1)
            };
            category.Add(nonExclusiveTrait0);

            category.Add(new Trait(NON_EXCLUSIVE_TRAIT1));

            category.Add(new Trait(NON_EXCLUSIVE_TRAIT2));

            category.Add(new Trait(NON_EXCLUSIVE_TRAIT3));

            TraitSchema schema = new TraitSchema();
            schema.Add(category);

            NpcGroup npcGroup = NpcFactory.Create(schema, 1, new List<Replacement>(), m_random);

            Assert.IsNotNull(npcGroup, "Failed to create an npc using a valid schema");
            Assert.AreEqual(1, npcGroup.NpcCount, "Wrong number of npcs created.");

            Npc npc = npcGroup.GetNpcAtIndex(0);

            Npc.Trait[] selectedTraits = npc.GetTraitsOfCategory(CATEGORY);
            Assert.AreEqual(5, selectedTraits.Length, "Wrong number of traits in category");
            Npc.Trait selectedMutuallyExclusiveTrait0 = Array.Find(selectedTraits, element => element.Name == MUTUALLY_EXCLUSIVE_TRAIT0);
            Assert.IsNotNull(selectedMutuallyExclusiveTrait0, "Wrong trait in category");
            Npc.Trait selectedNonExclusiveTrait0 = Array.Find(selectedTraits, element => element.Name == NON_EXCLUSIVE_TRAIT0);
            Assert.IsNotNull(selectedNonExclusiveTrait0, "Wrong trait in category");
            Npc.Trait selectedNonExclusiveTrait1 = Array.Find(selectedTraits, element => element.Name == NON_EXCLUSIVE_TRAIT1);
            Assert.IsNotNull(selectedNonExclusiveTrait1, "Wrong trait in category");
            Npc.Trait selectedNonExclusiveTrait2 = Array.Find(selectedTraits, element => element.Name == NON_EXCLUSIVE_TRAIT2);
            Assert.IsNotNull(selectedNonExclusiveTrait2, "Wrong trait in category");
            Npc.Trait selectedNonExclusiveTrait3 = Array.Find(selectedTraits, element => element.Name == NON_EXCLUSIVE_TRAIT3);
            Assert.IsNotNull(selectedNonExclusiveTrait3, "Wrong trait in category");
        }

        [TestMethod]
        public void HiddenCategory()
        {
            const string CATEGORY = "Young Fame";
            const string TRAIT = "Social Media";
            TraitCategory youngFameCategory = new TraitCategory(CATEGORY, CATEGORY, 1, isHidden: true);
            youngFameCategory.Add(new Trait(TRAIT));

            TraitSchema schema = new TraitSchema();
            schema.Add(youngFameCategory);

            NpcGroup npcGroup = NpcFactory.Create(schema, 1, new List<Replacement>(), m_random);

            Assert.IsNotNull(npcGroup, "Failed to create an npc using a valid schema");
            Assert.AreEqual(0, npcGroup.VisibleCategoryOrder.Count, "Wrong number of visible categories");

            Assert.AreEqual(1, npcGroup.NpcCount, "Wrong number of npcs created.");
            Npc npc = npcGroup.GetNpcAtIndex(0);
            Assert.IsNotNull(npc, "Failed to create an npc using a valid schema");
            Assert.IsTrue(npc.HasTrait(new TraitId(CATEGORY, TRAIT)), "Npc has the wrong trait");
        }

        [TestMethod]
        public void HiddenCategoryOfSharedOutput()
        {
            const string CATEGORY0_NAME = "Young Fame";
            const string CATEGORY1_NAME = "Old Fame";
            const string SHARED_OUTPUT_CATEGORY_NAME = "Fame";
            const string TRAIT0 = "Social Media";
            const string TRAIT1 = "Radio";
            TraitCategory youngFameCategory = new TraitCategory(CATEGORY0_NAME, SHARED_OUTPUT_CATEGORY_NAME, 1, isHidden: true);
            youngFameCategory.Add(new Trait(TRAIT0));
            TraitCategory oldFameCategory = new TraitCategory(CATEGORY1_NAME, SHARED_OUTPUT_CATEGORY_NAME, 1, isHidden: true);
            oldFameCategory.Add(new Trait(TRAIT1));

            TraitSchema schema = new TraitSchema();
            schema.Add(youngFameCategory);
            schema.Add(oldFameCategory);

            NpcGroup npcGroup = NpcFactory.Create(schema, 1, new List<Replacement>(), m_random);

            Assert.IsNotNull(npcGroup, "Failed to create an npc using a valid schema");
            Assert.AreEqual(0, npcGroup.VisibleCategoryOrder.Count, "Wrong number of visible categories");

            Assert.AreEqual(1, npcGroup.NpcCount, "Wrong number of npcs created.");
            Npc npc = npcGroup.GetNpcAtIndex(0);
            Assert.IsNotNull(npc, "Failed to create an npc using a valid schema");
            Assert.IsTrue(npc.HasTrait(new TraitId(SHARED_OUTPUT_CATEGORY_NAME, TRAIT0)), "Npc has the wrong trait");
            Assert.IsNotNull(npc, "Failed to create an npc using a valid schema");
            Assert.IsTrue(npc.HasTrait(new TraitId(SHARED_OUTPUT_CATEGORY_NAME, TRAIT1)), "Npc has the wrong trait");
        }

        [TestMethod]
        public void CategoryOrderNone()
        {
            const string CATEGORY0_NAME = "Animal";
            const string CATEGORY1_NAME = "Colour";
            TraitCategory category0 = new TraitCategory(CATEGORY0_NAME);
            category0.Add(new Trait("Bear"));
            TraitCategory category1 = new TraitCategory(CATEGORY1_NAME);
            category1.Add(new Trait("Blue"));

            TraitSchema schema = new TraitSchema();
            schema.Add(category0);
            schema.Add(category1);

            NpcGroup npcGroup = NpcFactory.Create(schema, 1, new List<Replacement>(), m_random);

            Assert.IsNotNull(npcGroup, "Failed to create an npc using a valid schema");
            Assert.AreEqual(2, npcGroup.VisibleCategoryOrder.Count, "Wrong number of visible categories");

            Assert.AreEqual(CATEGORY0_NAME, npcGroup.GetTraitCategoryNameAtIndex(0), "Wrong category order");
            Assert.AreEqual(CATEGORY1_NAME, npcGroup.GetTraitCategoryNameAtIndex(1), "Wrong category order");
        }

        [TestMethod]
        public void CategoryOrderPartial()
        {
            const string CATEGORY0_NAME = "Animal";
            const string CATEGORY1_NAME = "Colour";
            TraitCategory category0 = new TraitCategory(CATEGORY0_NAME);
            category0.Add(new Trait("Bear"));
            TraitCategory category1 = new TraitCategory(CATEGORY1_NAME);
            category1.Add(new Trait("Blue"));

            TraitSchema schema = new TraitSchema();
            schema.Add(category0);
            schema.Add(category1);

            schema.SetCategoryOrder(new List<string> { CATEGORY1_NAME });

            NpcGroup npcGroup = NpcFactory.Create(schema, 1, new List<Replacement>(), m_random);

            Assert.IsNotNull(npcGroup, "Failed to create an npc using a valid schema");
            Assert.AreEqual(2, npcGroup.VisibleCategoryOrder.Count, "Wrong number of visible categories");

            Assert.AreEqual(CATEGORY1_NAME, npcGroup.GetTraitCategoryNameAtIndex(0), "Wrong category order");
            Assert.AreEqual(CATEGORY0_NAME, npcGroup.GetTraitCategoryNameAtIndex(1), "Wrong category order");
        }

        [TestMethod]
        public void CategoryOrderComplete()
        {
            const string CATEGORY0_NAME = "Animal";
            const string CATEGORY1_NAME = "Colour";
            const string CATEGORY2_NAME = "Location";
            TraitCategory category0 = new TraitCategory(CATEGORY0_NAME);
            category0.Add(new Trait("Bear"));
            TraitCategory category1 = new TraitCategory(CATEGORY1_NAME);
            category1.Add(new Trait("Blue"));
            TraitCategory category2 = new TraitCategory(CATEGORY2_NAME);
            category2.Add(new Trait("Bridge"));

            TraitSchema schema = new TraitSchema();
            schema.Add(category0);
            schema.Add(category1);
            schema.Add(category2);

            schema.SetCategoryOrder(new List<string> { CATEGORY1_NAME, CATEGORY2_NAME, CATEGORY0_NAME });

            NpcGroup npcGroup = NpcFactory.Create(schema, 1, new List<Replacement>(), m_random);

            Assert.IsNotNull(npcGroup, "Failed to create an npc using a valid schema");
            Assert.AreEqual(3, npcGroup.VisibleCategoryOrder.Count, "Wrong number of visible categories");

            Assert.AreEqual(CATEGORY1_NAME, npcGroup.GetTraitCategoryNameAtIndex(0), "Wrong category order");
            Assert.AreEqual(CATEGORY2_NAME, npcGroup.GetTraitCategoryNameAtIndex(1), "Wrong category order");
            Assert.AreEqual(CATEGORY0_NAME, npcGroup.GetTraitCategoryNameAtIndex(2), "Wrong category order");
        }

        [TestMethod]
        public void CategoryOrderCompleteOutputNames()
        {
            const string CATEGORY0_NAME = "Animal";
            const string CATEGORY1_NAME = "Colour";
            const string CATEGORY1_OUTPUT_NAME = "Shade";
            const string CATEGORY2_NAME = "Location";
            const string CATEGORY2_OUTPUT_NAME = "Position";
            TraitCategory category0 = new TraitCategory(CATEGORY0_NAME);
            category0.Add(new Trait("Bear"));
            TraitCategory category1 = new TraitCategory(CATEGORY1_NAME, CATEGORY1_OUTPUT_NAME, 1);
            category1.Add(new Trait("Blue"));
            TraitCategory category2 = new TraitCategory(CATEGORY2_NAME, CATEGORY2_OUTPUT_NAME, 1);
            category2.Add(new Trait("Bridge"));

            TraitSchema schema = new TraitSchema();
            schema.Add(category0);
            schema.Add(category1);
            schema.Add(category2);

            schema.SetCategoryOrder(new List<string> { CATEGORY1_OUTPUT_NAME, CATEGORY2_OUTPUT_NAME, CATEGORY0_NAME });

            NpcGroup npcGroup = NpcFactory.Create(schema, 1, new List<Replacement>(), m_random);

            Assert.IsNotNull(npcGroup, "Failed to create an npc using a valid schema");
            Assert.AreEqual(3, npcGroup.VisibleCategoryOrder.Count, "Wrong number of visible categories");

            Assert.AreEqual(CATEGORY1_OUTPUT_NAME, npcGroup.GetTraitCategoryNameAtIndex(0), "Wrong category order");
            Assert.AreEqual(CATEGORY2_OUTPUT_NAME, npcGroup.GetTraitCategoryNameAtIndex(1), "Wrong category order");
            Assert.AreEqual(CATEGORY0_NAME, npcGroup.GetTraitCategoryNameAtIndex(2), "Wrong category order");
        }

        [TestMethod]
        public void CategoryOrderPartialSharedOutputNames()
        {
            const string CATEGORY0_NAME = "Animal";
            const string CATEGORY1_NAME = "Colour";
            const string SHARED_OUTPUT_NAME = "Shade";
            const string CATEGORY2_NAME = "Location";
            TraitCategory category0 = new TraitCategory(CATEGORY0_NAME);
            category0.Add(new Trait("Bear"));
            TraitCategory category1 = new TraitCategory(CATEGORY1_NAME, SHARED_OUTPUT_NAME, 1);
            category1.Add(new Trait("Blue"));
            TraitCategory category2 = new TraitCategory(CATEGORY2_NAME, SHARED_OUTPUT_NAME, 1);
            category2.Add(new Trait("Bridge"));

            TraitSchema schema = new TraitSchema();
            schema.Add(category0);
            schema.Add(category1);
            schema.Add(category2);

            schema.SetCategoryOrder(new List<string> { SHARED_OUTPUT_NAME });

            NpcGroup npcGroup = NpcFactory.Create(schema, 1, new List<Replacement>(), m_random);

            Assert.IsNotNull(npcGroup, "Failed to create an npc using a valid schema");
            Assert.AreEqual(2, npcGroup.VisibleCategoryOrder.Count, "Wrong number of visible categories");

            Assert.AreEqual(SHARED_OUTPUT_NAME, npcGroup.GetTraitCategoryNameAtIndex(0), "Wrong category order");
            Assert.AreEqual(CATEGORY0_NAME, npcGroup.GetTraitCategoryNameAtIndex(1), "Wrong category order");
        }

        private readonly MockRandom m_random = new MockRandom();
    }
}
