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
along with this program.If not, see<https://www.gnu.org/licenses/>.*/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using NpcGenerator;
using System;
using System.Collections.Generic;
using System.IO;

namespace Tests
{
    [TestClass]
    public class NpcGroupTests
    {
        [TestMethod]
        public void NpcGroupGeneratesCsvSingleNpc()
        {
            TraitCategory colourCategory = new TraitCategory("Colour", 1);
            colourCategory.Add(new Trait("Blue", 1, isHidden: false));

            TraitSchema schema = new TraitSchema();
            schema.Add(colourCategory);

            NpcGroup npcGroup = NpcFactory.Create(schema, 1, new List<Replacement>());
            string csv = npcGroup.ToCsv();

            Assert.AreEqual("Colour\nBlue", csv, "NpcGroup did not generate expected CSV text");
        }

        [TestMethod]
        public void NpcGroupGeneratesCsvMultipleNpcs()
        {
            TraitCategory colourCategory = new TraitCategory("Colour", 1);
            colourCategory.Add(new Trait("Blue", 1, isHidden: false));

            TraitSchema schema = new TraitSchema();
            schema.Add(colourCategory);

            NpcGroup npcGroup = NpcFactory.Create(schema, 2, new List<Replacement>());
            string csv = npcGroup.ToCsv();

            Assert.AreEqual("Colour\nBlue\nBlue", csv, "NpcGroup did not generate expected CSV text");
        }

        [TestMethod]
        public void NpcGroupGeneratesCsvZeroNpcs()
        {
            TraitCategory colourCategory = new TraitCategory("Colour", 1);
            colourCategory.Add(new Trait("Blue", 1, isHidden: false));

            TraitSchema schema = new TraitSchema();
            schema.Add(colourCategory);

            NpcGroup npcGroup = NpcFactory.Create(schema, 0, new List<Replacement>());
            string csv = npcGroup.ToCsv();

            Assert.AreEqual("Colour\n", csv, "NpcGroup did not generate expected CSV text");
        }

        public void NpcCategoryWithNoSelectionsCsv()
        {
            TraitCategory colourCategory = new TraitCategory("Colour", 1);
            colourCategory.Add(new Trait("Blue", 0, isHidden: false));

            TraitSchema schema = new TraitSchema();
            schema.Add(colourCategory);

            NpcGroup npcGroup = NpcFactory.Create(schema, 1, new List<Replacement>());
            string csv = npcGroup.ToCsv();

            Assert.AreEqual("Colour\n", csv, "NpcGroup did not generate expected CSV text");
        }

        [TestMethod]
        public void NpcGroupGeneratesJsonSingleNpc()
        {
            TraitCategory colourCategory = new TraitCategory("Colour", 1);
            colourCategory.Add(new Trait("Blue", 1, isHidden: false));

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(colourCategory);

            NpcGroup npcGroup = NpcFactory.Create(traitSchema, 1, new List<Replacement>());
            string jsonText = npcGroup.ToJson();
            JToken json = JToken.Parse(jsonText);

            string schemaPath = "NpcGroupSchema.json";
            string schemaText = File.ReadAllText(schemaPath);
            JSchema schema = JSchema.Parse(schemaText);

            string concatenatedMessages = "";
            bool isValid = json.IsValid(schema, out IList<string> errorMessages);
            if (!isValid)
            {
                foreach (var error in errorMessages)
                {
                    concatenatedMessages += error + "\n";
                }
            }
            Assert.IsTrue(isValid, "Json validation failed with message: " + concatenatedMessages);
        }

        [TestMethod]
        public void NpcGroupGeneratesJsonMultipleNpcs()
        {
            TraitCategory colourCategory = new TraitCategory("Colour", 1);
            colourCategory.Add(new Trait("Blue", 1, isHidden: false));

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(colourCategory);

            NpcGroup npcGroup = NpcFactory.Create(traitSchema, 2, new List<Replacement>());
            string jsonText = npcGroup.ToJson();
            JToken json = JToken.Parse(jsonText);

            string schemaPath = "NpcGroupSchema.json";
            string schemaText = File.ReadAllText(schemaPath);
            JSchema schema = JSchema.Parse(schemaText);

            string concatenatedMessages = "";
            bool isValid = json.IsValid(schema, out IList<string> errorMessages);
            if (!isValid)
            {
                foreach (var error in errorMessages)
                {
                    concatenatedMessages += error + "\n";
                }
            }
            Assert.IsTrue(isValid, "Json validation failed with message: " + concatenatedMessages);
        }

        [TestMethod]
        public void NpcGroupGeneratesJsonZeroNpcs()
        {
            TraitCategory colourCategory = new TraitCategory("Colour", 1);
            colourCategory.Add(new Trait("Blue", 1, isHidden: false));

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(colourCategory);

            NpcGroup npcGroup = NpcFactory.Create(traitSchema, 0, new List<Replacement>());
            string jsonText = npcGroup.ToJson();
            JToken json = JToken.Parse(jsonText);

            string schemaPath = "NpcGroupSchema.json";
            string schemaText = File.ReadAllText(schemaPath);
            JSchema schema = JSchema.Parse(schemaText);

            string concatenatedMessages = "";
            bool isValid = json.IsValid(schema, out IList<string> errorMessages);
            if (!isValid)
            {
                foreach (var error in errorMessages)
                {
                    concatenatedMessages += error + "\n";
                }
            }
            Assert.IsTrue(isValid, "Json validation failed with message: " + concatenatedMessages);
        }

        public void NpcCategoryWithNoSelectionsJson()
        {
            TraitCategory colourCategory = new TraitCategory("Colour", 1);
            colourCategory.Add(new Trait("Blue", 0, isHidden: false));

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(colourCategory);

            NpcGroup npcGroup = NpcFactory.Create(traitSchema, 1, new List<Replacement>());
            string jsonText = npcGroup.ToJson();
            JToken json = JToken.Parse(jsonText);

            string schemaPath = "NpcGroupSchema.json";
            string schemaText = File.ReadAllText(schemaPath);
            JSchema schema = JSchema.Parse(schemaText);

            string concatenatedMessages = "";
            bool isValid = json.IsValid(schema, out IList<string> errorMessages);
            if (!isValid)
            {
                foreach (var error in errorMessages)
                {
                    concatenatedMessages += error + "\n";
                }
            }
            Assert.IsTrue(isValid, "Json validation failed with message: " + concatenatedMessages);
        }

        [TestMethod]
        public void BonusSelectionForMissingCategory()
        {
            TraitCategory colourCategory = new TraitCategory("Colour", 1);
            Trait trait = new Trait("Blue", 1, isHidden: false)
            {
                BonusSelection = new BonusSelection(new TraitCategory("NotInSchema", 1), selectionCount: 1)
            };
            colourCategory.Add(trait);

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(colourCategory);

            bool threwException = false;
            try 
            {
                NpcGroup npcGroup = NpcFactory.Create(traitSchema, 1, new List<Replacement>());
            }
            catch(Exception)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Bonus selection of category not in schema did not cause exception.");
        }

        [TestMethod]
        public void BonusSelectionExceedsTraits()
        {
            TraitCategory colourCategory = new TraitCategory("Colour", 1);
            Trait trait = new Trait("Blue", 1, isHidden: false)
            {
                BonusSelection = new BonusSelection(colourCategory, 1)
            };
            colourCategory.Add(trait);

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(colourCategory);

            bool threwException = false;
            try
            {
                NpcGroup npcGroup = NpcFactory.Create(traitSchema, 1, new List<Replacement>());
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
            Trait blue = new Trait(BLUE, int.MaxValue-1, isHidden: false)
            {
                BonusSelection = new BonusSelection(colourCategory, 1)
            };
            colourCategory.Add(blue);
            Trait green = new Trait(GREEN, 1, isHidden: false);
            colourCategory.Add(green);

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(colourCategory);

            //Attempt 3 times to randomly select blue. Given the weighting, the odds are literally astronimical
            //that blue won't be selected.
            string[] traits = null;
            for (int i = 0; i < 3; ++i)
            {
                NpcGroup npcGroup = NpcFactory.Create(traitSchema, 1, new List<Replacement>());
                traits = npcGroup.GetNpcAtIndex(0).GetTraitsOfCategory(colourCategory.Name);
                int index = Array.FindIndex(traits, trait => trait == BLUE);
                bool isTraitWithBonusSelected = index > 0;
                if (isTraitWithBonusSelected)
                {
                    break;
                }
            }

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
            Trait blue = new Trait(BLUE, int.MaxValue - 1, isHidden: false)
            {
                BonusSelection = new BonusSelection(colourCategory, 1)
            };
            colourCategory.Add(blue);
            Trait green = new Trait(GREEN, 1, isHidden: false);
            colourCategory.Add(green);

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(colourCategory);

            //Attempt 3 times to randomly select blue. Given the weighting, the odds are literally astronimical
            //that blue won't be selected.
            const int NPC_COUNT = 3;
            NpcGroup npcGroup = NpcFactory.Create(traitSchema, NPC_COUNT, new List<Replacement>());

            Assert.AreEqual(NPC_COUNT, npcGroup.NpcCount, "Wrong number of NPCs generated");
        }

        [TestMethod]
        public void IntrerCategoryBonusSelection()
        {
            const string BLUE = "Blue";
            const string BEAR = "Bear";

            TraitCategory animalCategory = new TraitCategory("Animal", 0);
            animalCategory.Add(new Trait(BEAR, 1, isHidden: false));

            TraitCategory colourCategory = new TraitCategory("Colour", 1);
            Trait blue = new Trait(BLUE, 1, isHidden: false)
            {
                BonusSelection = new BonusSelection(animalCategory, 1)
            };
            colourCategory.Add(blue);

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(colourCategory);
            traitSchema.Add(animalCategory);

            NpcGroup npcGroup = NpcFactory.Create(traitSchema, 1, new List<Replacement>());
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
            const string CATEGORY_NAME = "Colour";
            TraitCategory colourCategory = new TraitCategory(CATEGORY_NAME, 1);
            Trait trait = new Trait("Blue", 1, isHidden: false);
            colourCategory.Add(trait);

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(colourCategory);

            const string REPLACEMENT_COLOUR = "Red";
            Replacement replacement = new Replacement(trait, REPLACEMENT_COLOUR, colourCategory);
            NpcGroup npcGroup = NpcFactory.Create(traitSchema, 1, new List<Replacement>() { replacement });
            string jsonText = npcGroup.ToJson();
            JToken json = JToken.Parse(jsonText);

            string schemaPath = "NpcGroupSchema.json";
            string schemaText = File.ReadAllText(schemaPath);
            JSchema schema = JSchema.Parse(schemaText);

            string concatenatedMessages = "";
            bool isValid = json.IsValid(schema, out IList<string> errorMessages);
            if (!isValid)
            {
                foreach (var error in errorMessages)
                {
                    concatenatedMessages += error + "\n";
                }
            }
            Assert.IsTrue(isValid, "Json validation failed with message: " + concatenatedMessages);

            string colourTrait = npcGroup.GetNpcAtIndex(0).GetTraitsOfCategory(CATEGORY_NAME)[0];
            Assert.AreEqual(REPLACEMENT_COLOUR, colourTrait, "Replacement was not honoured");
        }

        [TestMethod]
        public void MultipleNpcsWithReplacements()
        {
            TraitCategory colourCategory = new TraitCategory("Colour", 1);
            colourCategory.Add(new Trait("Blue", 1, isHidden: false));

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(colourCategory);

            NpcGroup npcGroup = NpcFactory.Create(traitSchema, 1, new List<Replacement>());
            string jsonText = npcGroup.ToJson();
            JToken json = JToken.Parse(jsonText);

            string schemaPath = "NpcGroupSchema.json";
            string schemaText = File.ReadAllText(schemaPath);
            JSchema schema = JSchema.Parse(schemaText);

            string concatenatedMessages = "";
            bool isValid = json.IsValid(schema, out IList<string> errorMessages);
            if (!isValid)
            {
                foreach (var error in errorMessages)
                {
                    concatenatedMessages += error + "\n";
                }
            }
            Assert.IsTrue(isValid, "Json validation failed with message: " + concatenatedMessages);
        }
    }
}
