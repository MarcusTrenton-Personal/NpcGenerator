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
            TraitCategory colourCategory = new TraitCategory(CATEGORY, 1);
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
            TraitCategory colourCategory = new TraitCategory(CATEGORY, 1);
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
            TraitCategory colourCategory = new TraitCategory("Colour", 1);
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
            TraitCategory colourCategory = new TraitCategory(CATEGORY, 1);
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
            TraitCategory colourCategory = new TraitCategory(CATEGORY, 1);
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
            TraitCategory colourCategory = new TraitCategory(CATEGORY, 1);
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
            TraitCategory colourCategory = new TraitCategory("Colour", 1);
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
            TraitCategory colourCategory = new TraitCategory(CATEGORY, 1);
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
            TraitCategory colourCategory = new TraitCategory(CATEGORY, 1);
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

        private readonly MockRandom m_random = new MockRandom();
    }
}
