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
            Npc.Trait[] traits = npc.GetTraitsOfCategory(CATEGORY);
            Assert.AreEqual(1, traits.Length, "Wrong number of traits in category");
            Assert.AreEqual(TRAIT, traits[0].Name, "Wrong trait in category");
            Assert.IsTrue(traits[0].IsHidden, "Trait should be hidden is not");
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

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void AreNpcsValidNullNpcGroup()
        {
            TraitSchema schema = new TraitSchema();

            NpcFactory.AreNpcsValid(npcGroup: null, schema, new List<Replacement>(), out Dictionary<Npc,List<NpcSchemaViolation>> _);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void AreNpcsValidNullNpc()
        {
            NpcGroup npcGroup = new NpcGroup(new List<string>() { "Animal" });
            npcGroup.Add(null);
            TraitSchema schema = new TraitSchema();

            NpcFactory.AreNpcsValid(npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> _);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void AreNpcsValidNullSchema()
        {
            NpcGroup npcGroup = new NpcGroup(new List<string>() { "Animal" });
            npcGroup.Add(new Npc());

            NpcFactory.AreNpcsValid(npcGroup, schema: null, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> _);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void AreNpcsValidNullReplacements()
        {
            TraitSchema schema = new TraitSchema();
            NpcGroup npcGroup = new NpcGroup(new List<string>() { "Animal" });
            npcGroup.Add(new Npc());

            NpcFactory.AreNpcsValid(npcGroup, schema, replacements: null, out Dictionary<Npc, List<NpcSchemaViolation>> _);
        }

        [TestMethod]
        public void AreNpcsValidEmpty()
        {
            TraitSchema schema = new TraitSchema();
            NpcGroup npcGroup = new NpcGroup(new List<string>() { "Animal" });
            Npc npc = new Npc();
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsTrue(areValid, "Npc is incorrectly invalid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(0, violations.Count, "Wrong number of violations");
        }

        [TestMethod]
        public void AreNpcsValidEmptyViolationTooFewTraits()
        {
            const string CATEGORY = "Animal";
            const string TRAIT = "Bear";
            const int TRAIT_COUNT = 2;

            Trait trait = new Trait(TRAIT);
            Trait trait2 = new Trait("Velociraptor");
            TraitCategory category = new TraitCategory(CATEGORY, TRAIT_COUNT);
            category.Add(trait);
            category.Add(trait2);
            TraitSchema schema = new TraitSchema();
            schema.Add(category);

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);
            
            Assert.IsFalse(areValid, "Npc is incorrectly valid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(1, violations.Count, "Wrong number of violations");
            Assert.AreEqual(CATEGORY, violations[0].Category, "Wrong violation category");
            Assert.IsNull(violations[0].Trait, "Wrong violation trait");
            Assert.AreEqual(NpcSchemaViolation.Reason.TooFewTraitsInCategory, violations[0].Violation, "Wrong violation reason");
        }

        [TestMethod]
        public void AreNpcsValidWithSingleCategoryAndSingleTrait()
        {
            const string CATEGORY = "Animal";
            const string TRAIT = "Bear";

            Trait trait = new Trait(TRAIT);
            Trait trait2 = new Trait("Velociraptor");
            TraitCategory category = new TraitCategory(CATEGORY);
            category.Add(trait);
            category.Add(trait2);
            TraitSchema schema = new TraitSchema();
            schema.Add(category);

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsTrue(areValid, "Npc is incorrectly invalid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(0, violations.Count, "Wrong number of violations");
        }

        [TestMethod]
        public void AreNpcsValidWithSingleCategoryAndMultipleTraits()
        {
            const string CATEGORY = "Animal";
            const string TRAIT0 = "Bear";
            const string TRAIT1 = "Velociraptor";

            Trait trait0 = new Trait(TRAIT0);
            Trait trait1 = new Trait(TRAIT1);
            Trait trait2 = new Trait("Baby Shark");
            TraitCategory category = new TraitCategory(CATEGORY, 2);
            category.Add(trait0);
            category.Add(trait1);
            category.Add(trait2);
            TraitSchema schema = new TraitSchema();
            schema.Add(category);

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT0), new Npc.Trait(TRAIT1) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsTrue(areValid, "Npc is incorrectly invalid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(0, violations.Count, "Wrong number of violations");
        }

        [TestMethod]
        public void AreNpcsValidWithMulipleCategoriesAndMultipleTraits()
        {
            const string CATEGORY0 = "Animal";
            const string C0T0 = "Bear";
            const string C0T1 = "Velociraptor";

            const string CATEGORY1 = "Colour";
            const string C1T0 = "Blue";

            Trait c0t0 = new Trait(C0T0);
            Trait c0t1 = new Trait(C0T1);
            Trait c0t2 = new Trait("Baby Shark");
            TraitCategory category0 = new TraitCategory(CATEGORY0, 2);
            category0.Add(c0t0);
            category0.Add(c0t1);
            category0.Add(c0t2);

            Trait c1t0 = new Trait(C1T0);
            Trait c1t1 = new Trait("Red");
            TraitCategory category1 = new TraitCategory(CATEGORY1);
            category1.Add(c1t0);
            category1.Add(c1t1);

            TraitSchema schema = new TraitSchema();
            schema.Add(category0);
            schema.Add(category1);

            Npc npc = new Npc();
            npc.Add(CATEGORY0, new Npc.Trait[] { new Npc.Trait(C0T0), new Npc.Trait(C0T1) });
            npc.Add(CATEGORY1, new Npc.Trait[] { new Npc.Trait(C1T0) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY0, CATEGORY1 });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsTrue(areValid, "Npc is incorrectly invalid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(0, violations.Count, "Wrong number of violations");
        }

        [TestMethod]
        public void AreNpcsValidWith0SelectionCategory()
        {
            const string CATEGORY0 = "Animal";
            const string C0T0 = "Bear";
            const string C0T1 = "Velociraptor";

            Trait c0t0 = new Trait(C0T0);
            Trait c0t1 = new Trait(C0T1);
            Trait c0t2 = new Trait("Baby Shark");
            TraitCategory category0 = new TraitCategory(CATEGORY0, 2);
            category0.Add(c0t0);
            category0.Add(c0t1);
            category0.Add(c0t2);

            Trait c1t0 = new Trait("Blue");
            Trait c1t1 = new Trait("Red");
            const string CATEGORY1 = "Colour";
            TraitCategory category1 = new TraitCategory(CATEGORY1, 0);
            category1.Add(c1t0);
            category1.Add(c1t1);

            TraitSchema schema = new TraitSchema();
            schema.Add(category0);
            schema.Add(category1);

            Npc npc = new Npc();
            npc.Add(CATEGORY0, new Npc.Trait[] { new Npc.Trait(C0T0), new Npc.Trait(C0T1) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY0 });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsTrue(areValid, "Npc is incorrectly invalid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(0, violations.Count, "Wrong number of violations");
        }

        [TestMethod]
        public void AreNpcsValidViolationTooManyAndTooFewTraits()
        {
            const string CATEGORY0 = "Animal";
            const string C0T0 = "Bear";
            const string C0T1 = "Velociraptor";

            Trait c0t0 = new Trait(C0T0);
            Trait c0t1 = new Trait(C0T1);
            Trait c0t2 = new Trait("Baby Shark");
            TraitCategory category0 = new TraitCategory(CATEGORY0, 3);
            category0.Add(c0t0);
            category0.Add(c0t1);
            category0.Add(c0t2);

            const string CATEGORY1 = "Colour";
            const string C1T0 = "Blue";

            Trait c1t0 = new Trait(C1T0);
            Trait c1t1 = new Trait("Red");
            TraitCategory category1 = new TraitCategory(CATEGORY1, 0);
            category1.Add(c1t0);
            category1.Add(c1t1);

            TraitSchema schema = new TraitSchema();
            schema.Add(category0);
            schema.Add(category1);

            Npc npc = new Npc();
            npc.Add(CATEGORY0, new Npc.Trait[] { new Npc.Trait(C0T0), new Npc.Trait(C0T1) });
            npc.Add(CATEGORY1, new Npc.Trait[] { new Npc.Trait(C1T0) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY0, CATEGORY1 });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsFalse(areValid, "Npc is incorrectly invalid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(2, violations.Count, "Wrong number of violations");

            NpcSchemaViolation tooFewTraitsViolation = violations.Find(
                violation => violation.Category == CATEGORY0 && 
                violation.Trait is null && 
                violation.Violation == NpcSchemaViolation.Reason.TooFewTraitsInCategory);
            Assert.IsNotNull(tooFewTraitsViolation, "TooFewTraitsInCategory violation not detected");

            NpcSchemaViolation tooManyTraitsViolation = violations.Find(
                violation => violation.Category == CATEGORY1 &&
                violation.Trait is null &&
                violation.Violation == NpcSchemaViolation.Reason.TooManyTraitsInCategory);
            Assert.IsNotNull(tooManyTraitsViolation, "TooManyTraitsInCategory violation not detected");
        }

        [TestMethod]
        public void AreNpcsValidMultipleViolationsMultipleNpcs()
        {
            const string CATEGORY0 = "Animal";
            const string C0T0 = "Bear";
            const string C0T1 = "Velociraptor";
            const string C0T2 = "Baby Shark";

            Trait c0t0 = new Trait(C0T0);
            Trait c0t1 = new Trait(C0T1);
            Trait c0t2 = new Trait(C0T2);
            TraitCategory category0 = new TraitCategory(CATEGORY0, 3);
            category0.Add(c0t0);
            category0.Add(c0t1);
            category0.Add(c0t2);

            const string CATEGORY1 = "Colour";
            const string C1T0 = "Blue";

            Trait c1t0 = new Trait(C1T0);
            Trait c1t1 = new Trait("Red");
            TraitCategory category1 = new TraitCategory(CATEGORY1, 0);
            category1.Add(c1t0);
            category1.Add(c1t1);

            TraitSchema schema = new TraitSchema();
            schema.Add(category0);
            schema.Add(category1);

            Npc npc0 = new Npc();
            npc0.Add(CATEGORY0, new Npc.Trait[] { new Npc.Trait(C0T0), new Npc.Trait(C0T1) });
            npc0.Add(CATEGORY1, new Npc.Trait[] { new Npc.Trait(C1T0) });

            const string TRAIT_NOT_FOUND = "Kraken";
            Npc npc1 = new Npc();
            npc1.Add(CATEGORY0, new Npc.Trait[] { new Npc.Trait(C0T0), new Npc.Trait(C0T1), new Npc.Trait(TRAIT_NOT_FOUND) });

            Npc npc2 = new Npc();
            npc2.Add(CATEGORY0, new Npc.Trait[] { new Npc.Trait(C0T0), new Npc.Trait(C0T1), new Npc.Trait(C0T2) });

            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY0, CATEGORY1 });
            npcGroup.Add(npc0);
            npcGroup.Add(npc1);
            npcGroup.Add(npc2);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsFalse(areValid, "Npc is incorrectly invalid");
            
            List<NpcSchemaViolation> violations0 = violationsPerNpc[npc0];
            Assert.AreEqual(2, violations0.Count, "Wrong number of violations");

            NpcSchemaViolation tooFewTraitsViolation = violations0.Find(
                violation => violation.Category == CATEGORY0 &&
                violation.Trait is null &&
                violation.Violation == NpcSchemaViolation.Reason.TooFewTraitsInCategory);
            Assert.IsNotNull(tooFewTraitsViolation, "TooFewTraitsInCategory violation not detected");

            NpcSchemaViolation tooManyTraitsViolation = violations0.Find(
                violation => violation.Category == CATEGORY1 &&
                violation.Trait is null &&
                violation.Violation == NpcSchemaViolation.Reason.TooManyTraitsInCategory);
            Assert.IsNotNull(tooManyTraitsViolation, "TooManyTraitsInCategory violation not detected");

            List<NpcSchemaViolation> violations1 = violationsPerNpc[npc1];
            Assert.AreEqual(1, violations1.Count, "Wrong number of violations");

            NpcSchemaViolation traiNotFoundViolation = violations1.Find(
                violation => violation.Category == CATEGORY0 &&
                violation.Trait == TRAIT_NOT_FOUND &&
                violation.Violation == NpcSchemaViolation.Reason.TraitNotFoundInSchema);
            Assert.IsNotNull(traiNotFoundViolation, "TraitNotFoundInSchema violation not detected");

            List<NpcSchemaViolation> violations2 = violationsPerNpc[npc2];
            Assert.AreEqual(0, violations2.Count, "Wrong number of violations");
        }

        [TestMethod]
        public void AreNpcsValidViolationCategoryNotFound()
        {
            const string CATEGORY0 = "Animal";
            const string C0T0 = "Bear";
            const string C0T1 = "Velociraptor";

            const string CATEGORY_NOT_FOUND = "Hair Dye";
            const string TRAIT_NOT_FOUND = "Blonde";

            Trait c0t0 = new Trait(C0T0);
            Trait c0t1 = new Trait(C0T1);
            Trait c0t2 = new Trait("Baby Shark");
            TraitCategory category0 = new TraitCategory(CATEGORY0, 3);
            category0.Add(c0t0);
            category0.Add(c0t1);
            category0.Add(c0t2);

            TraitSchema schema = new TraitSchema();
            schema.Add(category0);

            Npc npc = new Npc();
            npc.Add(CATEGORY_NOT_FOUND, new Npc.Trait[] { new Npc.Trait(TRAIT_NOT_FOUND) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY0, CATEGORY_NOT_FOUND });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsFalse(areValid, "Npc is incorrectly valid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(1, violations.Count, "Wrong number of violations");

            NpcSchemaViolation categoryNotFoundViolation = violations.Find(
                violation => violation.Category == CATEGORY_NOT_FOUND &&
                violation.Trait is null &&
                violation.Violation == NpcSchemaViolation.Reason.CategoryNotFoundInSchema);
            Assert.IsNotNull(categoryNotFoundViolation, "CategoryNotFoundInSchema violation not detected");
        }

        [TestMethod]
        public void AreNpcsValidViolationTraitNotFound()
        {
            const string CATEGORY = "Hair Dye";
            const string TRAIT_NOT_FOUND = "Blonde";

            Trait c0t0 = new Trait("Purple");
            TraitCategory category = new TraitCategory(CATEGORY, 1);
            category.Add(c0t0);

            TraitSchema schema = new TraitSchema();
            schema.Add(category);

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT_NOT_FOUND) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsFalse(areValid, "Npc is incorrectly valid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(1, violations.Count, "Wrong number of violations");

            NpcSchemaViolation categoryNotFoundViolation = violations.Find(
                violation => violation.Category == CATEGORY &&
                violation.Trait == TRAIT_NOT_FOUND &&
                violation.Violation == NpcSchemaViolation.Reason.TraitNotFoundInSchema);
            Assert.IsNotNull(categoryNotFoundViolation, "TraitNotFoundInSchema violation not detected");
        }

        [TestMethod]
        public void AreNpcsValidWithHiddenTraits()
        {
            const string CATEGORY = "Hair Dye";
            const string TRAIT = "Blonde";

            Trait c0t0 = new Trait(TRAIT, 1, isHidden: true);
            TraitCategory category = new TraitCategory(CATEGORY);
            category.Add(c0t0);

            TraitSchema schema = new TraitSchema();
            schema.Add(category);

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT, isHidden: true) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsTrue(areValid, "Npc is incorrectly invalid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(0, violations.Count, "Wrong number of violations");
        }

        [TestMethod]
        public void AreNpcsValidViolationIsHiddenOnlyInNpc()
        {
            const string CATEGORY = "Hair Dye";
            const string TRAIT = "Blonde";

            Trait c0t0 = new Trait(TRAIT, 1, isHidden: false);
            TraitCategory category = new TraitCategory(CATEGORY);
            category.Add(c0t0);

            TraitSchema schema = new TraitSchema();
            schema.Add(category);

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT, isHidden: true) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsFalse(areValid, "Npc is incorrectly valid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(1, violations.Count, "Wrong number of violations");

            NpcSchemaViolation categoryNotFoundViolation = violations.Find(
                violation => violation.Category == CATEGORY &&
                violation.Trait == TRAIT &&
                violation.Violation == NpcSchemaViolation.Reason.TraitIsIncorrectlyHidden);
            Assert.IsNotNull(categoryNotFoundViolation, "TraitIsHiddenMismatch violation not detected");
        }

        [TestMethod]
        public void AreNpcsValidViolationIsHiddenOnlyInSchema()
        {
            const string CATEGORY = "Hair Dye";
            const string TRAIT = "Blonde";

            Trait c0t0 = new Trait(TRAIT, 1, isHidden: true);
            TraitCategory category = new TraitCategory(CATEGORY);
            category.Add(c0t0);

            TraitSchema schema = new TraitSchema();
            schema.Add(category);

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT, isHidden: false) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsFalse(areValid, "Npc is incorrectly valid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(1, violations.Count, "Wrong number of violations");

            NpcSchemaViolation categoryNotFoundViolation = violations.Find(
                violation => violation.Category == CATEGORY &&
                violation.Trait == TRAIT &&
                violation.Violation == NpcSchemaViolation.Reason.TraitIsIncorrectlyNotHidden);
            Assert.IsNotNull(categoryNotFoundViolation, "TraitIsHiddenMismatch violation not detected");
        }

        [TestMethod]
        public void AreNpcsValidWithIntraCategoryBonusSelection()
        {
            const string CATEGORY = "Race";
            const string TRAIT0 = "Caucasian";
            const string TRAIT1 = "African";
            const string TRAIT2_WITH_BONUS_SELECTION = "Biracial";

            Trait trait0 = new Trait(TRAIT0);
            Trait trait1 = new Trait(TRAIT1);
            Trait trait2 = new Trait(TRAIT2_WITH_BONUS_SELECTION)
            {
                BonusSelection = new BonusSelection(CATEGORY, 2)
            };
            Trait trait3 = new Trait("Hispanic");
            Trait trait4 = new Trait("Asian");
            TraitCategory category = new TraitCategory(CATEGORY, 1);
            category.Add(trait0);
            category.Add(trait1);
            category.Add(trait2);
            category.Add(trait3);
            category.Add(trait4);
            TraitSchema schema = new TraitSchema();
            schema.Add(category);

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT0), new Npc.Trait(TRAIT1), new Npc.Trait(TRAIT2_WITH_BONUS_SELECTION) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsTrue(areValid, "Npc is incorrectly invalid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(0, violations.Count, "Wrong number of violations");
        }

        [TestMethod]
        public void AreNpcsValidWithInterCategoryBonusSelection()
        {
            const string CATEGORY0 = "Animal";
            const string C0T0 = "Bear";
            const string C0T1 = "Velociraptor";
            const string C0T2 = "Baby Shark";

            const string CATEGORY1 = "Colour";
            const string C1T0 = "Blue";
            const string C1T1 = "Red";

            Trait c0t0 = new Trait(C0T0)
            {
                BonusSelection = new BonusSelection(CATEGORY1, 1)
            };
            Trait c0t1 = new Trait(C0T1);
            Trait c0t2 = new Trait(C0T2);
            TraitCategory category0 = new TraitCategory(CATEGORY0, 2);
            category0.Add(c0t0);
            category0.Add(c0t1);
            category0.Add(c0t2);

            Trait c1t0 = new Trait(C1T0)
            {
                BonusSelection = new BonusSelection(CATEGORY0, 1)
            };
            Trait c1t1 = new Trait(C1T1);
            TraitCategory category1 = new TraitCategory(CATEGORY1);
            category1.Add(c1t0);
            category1.Add(c1t1);

            TraitSchema schema = new TraitSchema();
            schema.Add(category0);
            schema.Add(category1);

            Npc npc = new Npc();
            npc.Add(CATEGORY0, new Npc.Trait[] { new Npc.Trait(C0T0), new Npc.Trait(C0T1), new Npc.Trait(C0T2) });
            npc.Add(CATEGORY1, new Npc.Trait[] { new Npc.Trait(C1T0), new Npc.Trait(C1T1) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY0, CATEGORY1 });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsTrue(areValid, "Npc is incorrectly invalid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(0, violations.Count, "Wrong number of violations");
        }

        [TestMethod]
        public void AreNpcsValidViolationTooFewTraitsWithBonusSelection()
        {
            const string CATEGORY = "Race";
            const string TRAIT0 = "Caucasian";
            const string TRAIT1 = "African";
            const string TRAIT2_WITH_BONUS_SELECTION = "Biracial";

            Trait trait0 = new Trait(TRAIT0);
            Trait trait1 = new Trait(TRAIT1);
            Trait trait2 = new Trait(TRAIT2_WITH_BONUS_SELECTION)
            {
                BonusSelection = new BonusSelection(CATEGORY, 2)
            };
            Trait trait3 = new Trait("Hispanic");
            Trait trait4 = new Trait("Asian");
            TraitCategory category = new TraitCategory(CATEGORY, 1);
            category.Add(trait0);
            category.Add(trait1);
            category.Add(trait2);
            category.Add(trait3);
            category.Add(trait4);
            TraitSchema schema = new TraitSchema();
            schema.Add(category);

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT0), new Npc.Trait(TRAIT2_WITH_BONUS_SELECTION) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsFalse(areValid, "Npc is incorrectly valid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(1, violations.Count, "Wrong number of violations");

            NpcSchemaViolation categoryNotFoundViolation = violations.Find(
                violation => violation.Category == CATEGORY &&
                violation.Trait is null &&
                violation.Violation == NpcSchemaViolation.Reason.TooFewTraitsInCategory);
            Assert.IsNotNull(categoryNotFoundViolation, "TooFewTraitsInCategory violation not detected");
        }

        [TestMethod]
        public void AreNpcsValidWithUnusedReplacement()
        {
            const string CATEGORY = "Animal";
            const string TRAIT = "Bear";
            const string TRAIT_WITH_REPLACEMENT = "Velociraptor";

            Trait trait = new Trait(TRAIT);
            Trait traitWithReplacement = new Trait(TRAIT_WITH_REPLACEMENT);
            TraitCategory category = new TraitCategory(CATEGORY);
            category.Add(trait);
            category.Add(traitWithReplacement);
            TraitSchema schema = new TraitSchema();
            schema.Add(category);

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY });
            npcGroup.Add(npc);

            Replacement replacement = new Replacement(traitWithReplacement, "Tyrannosaurus Rex", category);
            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>() { replacement }, out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsTrue(areValid, "Npc is incorrectly invalid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(0, violations.Count, "Wrong number of violations");
        }

        [TestMethod]
        public void AreNpcsValidWithReplacement()
        {
            const string CATEGORY = "Animal";
            const string TRAIT = "Bear";
            const string TRAIT_ORIGINAL_NAME = "Velociraptor";
            const string REPLACEMENT_NAME = "Tyrannosaurus Rex";

            Trait trait = new Trait(TRAIT);
            Trait traitWithReplacement = new Trait(TRAIT_ORIGINAL_NAME);
            TraitCategory category = new TraitCategory(CATEGORY);
            category.Add(trait);
            category.Add(traitWithReplacement);
            TraitSchema schema = new TraitSchema();
            schema.Add(category);

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(REPLACEMENT_NAME, isHidden: false, originalName: TRAIT_ORIGINAL_NAME) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY });
            npcGroup.Add(npc);

            Replacement replacement = new Replacement(traitWithReplacement, REPLACEMENT_NAME, category);
            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>() { replacement }, out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsTrue(areValid, "Npc is incorrectly invalid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(0, violations.Count, "Wrong number of violations");
        }

        [TestMethod]
        public void AreNpcsValidViolationUnusedReplacement()
        {
            const string CATEGORY = "Animal";
            const string TRAIT = "Bear";
            const string TRAIT_ORIGINAL_NAME = "Velociraptor";
            const string REPLACEMENT_NAME = "Tyrannosaurus Rex";

            Trait trait = new Trait(TRAIT);
            Trait traitWithReplacement = new Trait(TRAIT_ORIGINAL_NAME);
            TraitCategory category = new TraitCategory(CATEGORY);
            category.Add(trait);
            category.Add(traitWithReplacement);
            TraitSchema schema = new TraitSchema();
            schema.Add(category);

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT_ORIGINAL_NAME) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY });
            npcGroup.Add(npc);

            Replacement replacement = new Replacement(traitWithReplacement, REPLACEMENT_NAME, category);
            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>() { replacement }, out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsFalse(areValid, "Npc is incorrectly valid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(1, violations.Count, "Wrong number of violations");

            NpcSchemaViolation categoryNotFoundViolation = violations.Find(violation => 
                violation.Category == CATEGORY &&
                violation.Trait == TRAIT_ORIGINAL_NAME &&
                violation.Violation == NpcSchemaViolation.Reason.UnusedReplacement);
            Assert.IsNotNull(categoryNotFoundViolation, "UnusedReplacement violation not detected");
        }

        [TestMethod]
        public void AreNpcsValidWithLockedCategory()
        {
            const string REQUIRED_CATEGORY = "Animal";
            const string REQUIRED_TRAIT = "Bear";
            const string ALTERNATIVE_TO_REQUIRED_TRAIT = "Rhino";

            Trait c0t0 = new Trait(REQUIRED_TRAIT);
            Trait c0t1 = new Trait(ALTERNATIVE_TO_REQUIRED_TRAIT);
            TraitCategory category = new TraitCategory(REQUIRED_CATEGORY);
            category.Add(c0t0);
            category.Add(c0t1);

            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait hasTrait = new NpcHasTrait(new TraitId(categoryName: REQUIRED_CATEGORY, traitName: REQUIRED_TRAIT), npcHolder);
            Requirement requirement = new Requirement(hasTrait, npcHolder);

            Trait c1t0 = new Trait("Blue");
            const string LOCKED_CATEGORY = "Colour";
            TraitCategory lockedCategory = new TraitCategory(LOCKED_CATEGORY);
            lockedCategory.Add(c1t0);
            lockedCategory.Set(requirement);

            TraitSchema schema = new TraitSchema();
            schema.Add(category);
            schema.Add(lockedCategory);

            Npc npc = new Npc();
            npc.Add(REQUIRED_CATEGORY, new Npc.Trait[] { new Npc.Trait(ALTERNATIVE_TO_REQUIRED_TRAIT) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { REQUIRED_CATEGORY, LOCKED_CATEGORY });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsTrue(areValid, "Npc is incorrectly invalid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(0, violations.Count, "Wrong number of violations");
        }

        [TestMethod]
        public void AreNpcsValidViolationTraitFromLockedCategory()
        {
            const string REQUIRED_CATEGORY = "Animal";
            const string REQUIRED_TRAIT = "Bear";
            const string ALTERNATIVE_TO_REQUIRED_TRAIT = "Rhino";

            const string LOCKED_CATEGORY = "Colour";
            const string LOCKED_TRAIT = "Blue";

            Trait c0t0 = new Trait(REQUIRED_TRAIT);
            Trait c0t1 = new Trait(ALTERNATIVE_TO_REQUIRED_TRAIT);
            TraitCategory category = new TraitCategory(REQUIRED_CATEGORY);
            category.Add(c0t0);
            category.Add(c0t1);

            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait hasTrait = new NpcHasTrait(new TraitId(categoryName: REQUIRED_CATEGORY, traitName: REQUIRED_TRAIT), npcHolder);
            Requirement requirement = new Requirement(hasTrait, npcHolder);

            Trait c1t0 = new Trait(LOCKED_TRAIT);
            TraitCategory lockedCategory = new TraitCategory(LOCKED_CATEGORY);
            lockedCategory.Add(c1t0);
            lockedCategory.Set(requirement);

            TraitSchema schema = new TraitSchema();
            schema.Add(category);
            schema.Add(lockedCategory);

            Npc npc = new Npc();
            npc.Add(REQUIRED_CATEGORY, new Npc.Trait[] { new Npc.Trait(ALTERNATIVE_TO_REQUIRED_TRAIT) });
            npc.Add(LOCKED_CATEGORY, new Npc.Trait[] { new Npc.Trait(LOCKED_TRAIT) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { REQUIRED_CATEGORY, LOCKED_CATEGORY });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsFalse(areValid, "Npc is incorrectly valid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(1, violations.Count, "Wrong number of violations");

            NpcSchemaViolation categoryNotFoundViolation = violations.Find(
                violation => violation.Category == LOCKED_CATEGORY &&
                violation.Trait == LOCKED_TRAIT &&
                violation.Violation == NpcSchemaViolation.Reason.HasTraitInLockedCategory);
            Assert.IsNotNull(categoryNotFoundViolation, "HasTraitInLockedCategory violation not detected");
        }

        [TestMethod]
        public void AreNpcsValidWithUnlockedCategory()
        {
            const string REQUIRED_CATEGORY = "Animal";
            const string REQUIRED_TRAIT = "Bear";
            const string ALTERNATIVE_TO_REQUIRED_TRAIT = "Rhino";

            const string LOCKED_CATEGORY = "Colour";
            const string LOCKED_TRAIT = "Blue";

            Trait c0t0 = new Trait(REQUIRED_TRAIT);
            Trait c0t1 = new Trait(ALTERNATIVE_TO_REQUIRED_TRAIT);
            TraitCategory category = new TraitCategory(REQUIRED_CATEGORY);
            category.Add(c0t0);
            category.Add(c0t1);

            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait hasTrait = new NpcHasTrait(new TraitId(categoryName: REQUIRED_CATEGORY, traitName: REQUIRED_TRAIT), npcHolder);
            Requirement requirement = new Requirement(hasTrait, npcHolder);

            Trait c1t0 = new Trait(LOCKED_TRAIT);
            TraitCategory lockedCategory = new TraitCategory(LOCKED_CATEGORY);
            lockedCategory.Add(c1t0);
            lockedCategory.Set(requirement);

            TraitSchema schema = new TraitSchema();
            schema.Add(category);
            schema.Add(lockedCategory);

            Npc npc = new Npc();
            npc.Add(REQUIRED_CATEGORY, new Npc.Trait[] { new Npc.Trait(REQUIRED_TRAIT) });
            npc.Add(LOCKED_CATEGORY, new Npc.Trait[] { new Npc.Trait(LOCKED_TRAIT) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { REQUIRED_CATEGORY, LOCKED_CATEGORY });
            npcGroup.Add(npc);

            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>(), out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsTrue(areValid, "Npc is incorrectly invalid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(0, violations.Count, "Wrong number of violations");
        }

        [TestMethod]
        public void AreNpcsValidWithReplacementUnlockedCategory()
        {
            const string REQUIRED_CATEGORY = "Animal";
            const string TRAIT_ORIGINAL_NAME = "Velociraptor";
            const string REPLACEMENT_NAME = "Tyrannosaurus Rex";

            const string LOCKED_CATEGORY = "Colour";
            const string LOCKED_TRAIT = "Blue";

            Trait traitWithReplacement = new Trait(TRAIT_ORIGINAL_NAME);
            Trait c0t1 = new Trait("Rhino");
            Trait c0t2 = new Trait(REPLACEMENT_NAME);
            TraitCategory category = new TraitCategory(REQUIRED_CATEGORY);
            category.Add(traitWithReplacement);
            category.Add(c0t1);
            category.Add(c0t2);

            NpcHolder npcHolder = new NpcHolder();
            NpcHasTrait hasTrait = new NpcHasTrait(new TraitId(categoryName: REQUIRED_CATEGORY, traitName: REPLACEMENT_NAME), npcHolder);
            Requirement requirement = new Requirement(hasTrait, npcHolder);

            Trait c1t0 = new Trait(LOCKED_TRAIT);
            TraitCategory lockedCategory = new TraitCategory(LOCKED_CATEGORY);
            lockedCategory.Add(c1t0);
            lockedCategory.Set(requirement);

            TraitSchema schema = new TraitSchema();
            schema.Add(category);
            schema.Add(lockedCategory);

            Npc npc = new Npc();
            npc.Add(REQUIRED_CATEGORY, new Npc.Trait[] 
                { new Npc.Trait(name: REPLACEMENT_NAME, isHidden: false, originalName: TRAIT_ORIGINAL_NAME) });
            npc.Add(LOCKED_CATEGORY, new Npc.Trait[] { new Npc.Trait(LOCKED_TRAIT) });
            NpcGroup npcGroup = new NpcGroup(new List<string>() { REQUIRED_CATEGORY, LOCKED_CATEGORY });
            npcGroup.Add(npc);

            Replacement replacement = new Replacement(traitWithReplacement, REPLACEMENT_NAME, category);
            bool areValid = NpcFactory.AreNpcsValid(
                npcGroup, schema, new List<Replacement>() { replacement }, out Dictionary<Npc, List<NpcSchemaViolation>> violationsPerNpc);

            Assert.IsTrue(areValid, "Npc is incorrectly invalid");
            List<NpcSchemaViolation> violations = violationsPerNpc[npc];
            Assert.AreEqual(0, violations.Count, "Wrong number of violations");
        }

        private readonly MockRandom m_random = new MockRandom();
    }
}
