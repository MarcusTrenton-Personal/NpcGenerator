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
using Newtonsoft.Json;
using NpcGenerator;
using System.Collections.Generic;
using System.IO;

namespace Tests.JsonConfigurationParserTests
{
    [TestClass]
    public class BasicJsonConfigurationParserTests
    {
        const string SCHEMA_PATH = "ConfigurationSchema.json";

        [TestMethod]
        public void GeneratesTraitSchema()
        {
            string text = @"{
                'trait_categories' : [
                    {
                        'name' : 'Colour',
                        'selections': 1,
                        'traits' : [
                            { 
                                'name' : 'Green', 
                                'weight' : 1
                            },
                            { 
                                'name' : 'Red', 
                                'weight' : 1
                            }
                        ]
                    }
                ]
            }";

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);
            TraitSchema schema = parser.Parse(text);
            Assert.IsNotNull(schema, "Failed to generate a schema from the valid text");
        }

        [TestMethod]
        public void SchemaIsOptional()
        {
            string text = @"{
                'trait_categories' : [
                    {
                        'name' : 'Colour',
                        'selections': 1,
                        'traits' : [
                            { 
                                'name' : 'Green', 
                                'weight' : 1
                            },
                            { 
                                'name' : 'Red', 
                                'weight' : 1
                            }
                        ]
                    }
                ]
            }";

            JsonConfigurationParser parser = new JsonConfigurationParser(null);
            TraitSchema schema = parser.Parse(text);
            Assert.IsNotNull(schema, "Failed to generate a schema from the valid text");
        }

        [TestMethod]
        public void MalformedJson()
        {
            //The initial { and final } are missing.
            string text = @"
                'trait_categories' : [
                    {
                        'name' : 'Colour',
                        'selections': 1,
                        'traits' : [
                            { 
                                'name' : 'Green', 
                                'weight' : 1
                            },
                            { 
                                'name' : 'Red', 
                                'weight' : 1
                            }
                        ]
                    }
                ]
            ";

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(text);
            }
            catch (JsonReaderException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Malformed Json failed to throw exception");
        }

        [TestMethod]
        public void TraitSchemaHasTraitsInCorrectTraitCategories()
        {
            const string CATEGORY1_TITLE = "Colour";
            const int CATEGORY1_SELECTION_COUNT = 1;
            const string CATEGORY1_TRAIT1 = "Green";
            const string CATEGORY1_TRAIT2 = "Red";
            const string CATEGORY2_TITLE = "Animal";
            const int CATEGORY2_SELECTION_COUNT = 2;
            const string CATEGORY2_TRAIT1 = "Gorilla";
            const string CATEGORY2_TRAIT2 = "Rhino";

            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{CATEGORY1_TITLE}',
                        'selections': {CATEGORY1_SELECTION_COUNT},
                        'traits' : [
                            {{ 
                                'name' : '{CATEGORY1_TRAIT1}', 
                                'weight' : 1
                            }},
                            {{ 
                                'name' : '{CATEGORY1_TRAIT2}', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : '{CATEGORY2_TITLE}',
                        'selections': {CATEGORY2_SELECTION_COUNT},
                        'traits' : [
                            {{ 
                                'name' : '{CATEGORY2_TRAIT1}', 
                                'weight' : 1
                            }},
                            {{ 
                                'name' : '{CATEGORY2_TRAIT2}', 
                                'weight' : 1
                            }}
                        ]
                    }}
                ]
            }}";

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);
            TraitSchema schema = parser.Parse(text);
            Assert.IsNotNull(schema, "Failed to generate a schema from the valid text");

            IReadOnlyList<TraitCategory> categories = schema.GetTraitCategories();
            Assert.IsNotNull(categories, "Schema categories is null, which should be impossible");
            Assert.AreEqual(2, categories.Count, "Schema has incorrect number of TraitCategories");
            TraitCategory firstCategory = categories[0];
            Assert.IsNotNull(firstCategory, "Schema has a null first TraitCategory");
            Assert.AreEqual(firstCategory.Name, CATEGORY1_TITLE, "First category doesn't have name " + CATEGORY1_TITLE);
            Assert.AreEqual(CATEGORY1_SELECTION_COUNT, firstCategory.DefaultSelectionCount, "First category has wrong SelectionCount");
            TraitChooser firstChooser = firstCategory.CreateTraitChooser(m_random);
            Npc.Trait[] colours = firstChooser.Choose(firstCategory.DefaultSelectionCount, out IReadOnlyList<BonusSelection> bonusSelections1);
            Assert.AreEqual(0, bonusSelections1.Count, "Bonus selection returned where there should be none.");
            Assert.AreEqual(colours.Length, 1, "Wrong number of selections from " + CATEGORY1_TITLE);
            Assert.IsTrue(colours[0].Name == CATEGORY1_TRAIT1 || colours[0].Name == CATEGORY1_TRAIT2, CATEGORY1_TITLE + 
                " chose an invalid trait " + colours[0].Name);

            TraitCategory secondCategory = categories[1];
            Assert.IsNotNull(secondCategory, "Schema has a null second TraitCategory");
            Assert.AreEqual(secondCategory.Name, CATEGORY2_TITLE, "Second category doesn't have name " + CATEGORY2_TITLE);
            Assert.AreEqual(CATEGORY2_SELECTION_COUNT, secondCategory.DefaultSelectionCount, "Second category has wrong SelectionCount");
            TraitChooser secondChooser = secondCategory.CreateTraitChooser(m_random);
            Npc.Trait[] animals = secondChooser.Choose(secondCategory.DefaultSelectionCount, out IReadOnlyList<BonusSelection> bonusSelections2);
            Assert.AreEqual(0, bonusSelections2.Count, "Bonus selection returned where there should be none.");
            Assert.AreEqual(animals.Length, 2, "Wrong number of selections from " + CATEGORY2_TITLE);
            Assert.IsTrue((animals[0].Name == CATEGORY2_TRAIT1 || animals[0].Name == CATEGORY2_TRAIT2), 
                "Incorrect first animal selected: " + animals[0].Name);
            Assert.IsTrue((animals[1].Name == CATEGORY2_TRAIT1 || animals[1].Name == CATEGORY2_TRAIT2), 
                "Incorrect second animal selected: " + animals[1]);
            Assert.AreNotEqual(animals[0].Name, animals[1], "Did not select both animals");
        }

        [TestMethod]
        public void MissingTraitTitleThrowsException()
        {
            string text = @"{
                'trait_categories' : [
                    { 
                        'name' : 'Green', 
                        'weight' : 1
                    },
                    { 
                        'name' : 'Red', 
                        'weight' : 1
                    }
                ]
            }";

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(text);
            }
            catch (JsonFormatException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Missing title failed to throw exception");
        }

        [TestMethod]
        public void MissingTraitsThrowsException()
        {
            string text = @"{
                'trait_categories' : [
                    {
                        'name' : 'Colour',
                        'selections': 1,
                        'traits' : [
                        ]
                    }
                ]
            }";

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(text);
            }
            catch (JsonFormatException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Missing traits failed to throw exception");
        }
        
        [TestMethod]
        public void MissingTraitName()
        {
            string text = @"{
                'trait_categories' : [
                    {
                        'name' : 'Colour',
                        'selections': 1,
                        'traits' : [
                            { 
                                'weight' : 1
                            },
                            { 
                                'name' : 'Red', 
                                'weight' : 1
                            }
                        ]
                    }
                ]
            }";

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(text);
            }
            catch (JsonFormatException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Missing trait name did not throw exception");
        }

        [TestMethod]
        public void NegativeWeightTrait()
        {
            string text = @"{
                'trait_categories' : [
                    {
                        'name' : 'Colour',
                        'selections': 1,
                        'traits' : [
                            { 
                                'name' : 'Green', 
                                'weight' : -1
                            },
                            { 
                                'name' : 'Red', 
                                'weight' : 1
                            }
                        ]
                    }
                ]
            }";

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(text);
            }
            catch (JsonFormatException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Negative trait weight did not throw exception");
        }

        [TestMethod]
        public void MissingWeightTrait()
        {
            string text = @"{
                'trait_categories' : [
                    {
                        'name' : 'Colour',
                        'selections': 1,
                        'traits' : [
                            { 
                                'name' : 'Green'
                            },
                            { 
                                'name' : 'Red', 
                                'weight' : 1
                            }
                        ]
                    }
                ]
            }";

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(text);
            }
            catch (JsonFormatException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Missing trait weight did not throw exception");
        }

        [TestMethod]
        public void MissingTraitCategoryName()
        {
            string text = @"{
                'trait_categories' : 
                [ 
                    {
                        'selections': 1,
                        'traits' :
                        [
                            { 
                                'name' : 'Green', 
                                'weight' : 1
                            },
                            { 
                                'name' : 'Red', 
                                'weight' : 1
                            }
                        ]
                    }
                ]
            }";

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(text);
            }
            catch (JsonFormatException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Missing trait category name did not throw exception");
        }

        [TestMethod]
        public void MissingSelections()
        {
            string text = @"{
                'trait_categories' : [
                    {
                        'name' : 'Colour',
                        'traits' : [
                            { 
                                'name' : 'Green', 
                                'weight' : 1
                            },
                            { 
                                'name' : 'Red', 
                                'weight' : 1
                            }
                        ]
                    }
                ]
            }";

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(text);
            }
            catch (JsonFormatException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Missing trait selection number did not throw exception");
        }

        [TestMethod]
        public void EmptyJsonObject()
        {
            string text = "{}";

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(text);
            }
            catch (JsonFormatException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Empty json did not throw exception");
        }

        [TestMethod]
        public void DuplicateCategoryNames()
        {
            const string CATEGORY = "Colour";

            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{CATEGORY}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : 'Green', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : '{CATEGORY}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : 'Red', 
                                'weight' : 1
                            }}
                        ]
                    }}
                ]
            }}";

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool throwException = false;
            try
            {
                TraitSchema schema = parser.Parse(text);
            }
            catch(DuplicateCategoryNameException exception)
            {
                Assert.AreEqual(CATEGORY, exception.Category, "Reported category is wrong");

                throwException = true;
            }
            
            Assert.IsTrue(throwException, "Duplicate categories were not rejected.");
        }

        [TestMethod]
        public void DuplicateTraitNamesInACategory()
        {
            const string CATEGORY = "Colour";
            const string TRAIT = "Green";

            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{CATEGORY}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : '{TRAIT}', 
                                'weight' : 1
                            }},
                            {{ 
                                'name' : '{TRAIT}', 
                                'weight' : 1
                            }}
                        ]
                    }}
                ]
            }}";

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool throwException = false;
            try
            {
                TraitSchema schema = parser.Parse(text);
            }
            catch (JsonFormatException)
            {
                throwException = true;
            }

            Assert.IsTrue(throwException, "Duplicate trait names in the same category were not rejected.");
        }

        [TestMethod]
        public void DuplicateTraitNamesButDifferentAttributesInACategory()
        {
            const string CATEGORY = "Colour";
            const string TRAIT = "Green";

            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{CATEGORY}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : '{TRAIT}', 
                                'weight' : 1
                            }},
                            {{ 
                                'name' : '{TRAIT}', 
                                'weight' : 1,
                                'hidden' : true
                            }}
                        ]
                    }}
                ]
            }}";

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool throwException = false;
            try
            {
                TraitSchema schema = parser.Parse(text);
            }
            catch (DuplicateTraitNameInCategoryException exception)
            {
                Assert.AreEqual(new TraitId(CATEGORY, TRAIT), exception.TraitId, "Wrong duplicates identified");

                throwException = true;
            }

            Assert.IsTrue(throwException, "Duplicate trait names in the same category were not rejected.");
        }

        [TestMethod]
        public void DuplicateTraitNamesInDifferentCategories()
        {
            const string TRAIT_NAME = "Brown";

            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : 'Hair',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : '{TRAIT_NAME}', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : 'Skin',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : '{TRAIT_NAME}', 
                                'weight' : 1
                            }}
                        ]
                    }}
                ]
            }}";

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            TraitSchema schema = parser.Parse(text);
            IReadOnlyList<TraitCategory> categories = schema.GetTraitCategories();

            Assert.AreEqual(2, categories.Count);
            Assert.IsNotNull(categories[0].GetTrait(TRAIT_NAME), "Missing expected trait in category");
            Assert.IsNotNull(categories[1].GetTrait(TRAIT_NAME), "Missing expected trait in category");
        }

        [TestMethod]
        public void TooFewTraitsinCategory()
        {
            const string CATEGORY = "Hair";
            const int SELECTIONS = 10;

            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{CATEGORY}',
                        'selections': {SELECTIONS},
                        'traits' : [
                            {{ 
                                'name' : 'Brown', 
                                'weight' : 1
                            }}
                        ]
                    }}
                ]
            }}";

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(text);
            }
            catch (TooFewTraitsInCategoryException exception)
            {
                Assert.AreEqual(CATEGORY, exception.Category, "Wrong category name");
                Assert.AreEqual(SELECTIONS, exception.Requested, "Wrong number of requested traits");
                Assert.AreEqual(1, exception.Available, "Wrong number of available traits");
                threwException = true;
            }

            Assert.IsTrue(threwException, "Failed to throw a TooFewTraitsInCategoryException");
        }

        private readonly MockRandom m_random = new MockRandom();
    }
}
