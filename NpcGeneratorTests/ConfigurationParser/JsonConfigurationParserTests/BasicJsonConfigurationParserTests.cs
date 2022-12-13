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
using Services;
using System.Collections.Generic;

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

        [TestMethod, ExpectedException(typeof(JsonFormatException))]
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

            parser.Parse(text);
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
            TraitChooser firstChooser = firstCategory.CreateTraitChooser(m_random, new Npc());
            Npc.Trait[] colours = firstChooser.Choose(firstCategory.DefaultSelectionCount, out IReadOnlyList<BonusSelection> bonusSelections1);
            Assert.AreEqual(0, bonusSelections1.Count, "Bonus selection returned where there should be none.");
            Assert.AreEqual(colours.Length, 1, "Wrong number of selections from " + CATEGORY1_TITLE);
            Assert.IsTrue(colours[0].Name == CATEGORY1_TRAIT1 || colours[0].Name == CATEGORY1_TRAIT2, CATEGORY1_TITLE + 
                " chose an invalid trait " + colours[0].Name);

            TraitCategory secondCategory = categories[1];
            Assert.IsNotNull(secondCategory, "Schema has a null second TraitCategory");
            Assert.AreEqual(secondCategory.Name, CATEGORY2_TITLE, "Second category doesn't have name " + CATEGORY2_TITLE);
            Assert.AreEqual(CATEGORY2_SELECTION_COUNT, secondCategory.DefaultSelectionCount, "Second category has wrong SelectionCount");
            TraitChooser secondChooser = secondCategory.CreateTraitChooser(m_random, new Npc());
            Npc.Trait[] animals = secondChooser.Choose(secondCategory.DefaultSelectionCount, out IReadOnlyList<BonusSelection> bonusSelections2);
            Assert.AreEqual(0, bonusSelections2.Count, "Bonus selection returned where there should be none.");
            Assert.AreEqual(animals.Length, 2, "Wrong number of selections from " + CATEGORY2_TITLE);
            Assert.IsTrue((animals[0].Name == CATEGORY2_TRAIT1 || animals[0].Name == CATEGORY2_TRAIT2), 
                "Incorrect first animal selected: " + animals[0].Name);
            Assert.IsTrue((animals[1].Name == CATEGORY2_TRAIT1 || animals[1].Name == CATEGORY2_TRAIT2), 
                "Incorrect second animal selected: " + animals[1]);
            Assert.AreNotEqual(animals[0].Name, animals[1], "Did not select both animals");
        }

        [TestMethod, ExpectedException(typeof(JsonFormatException))]
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

            parser.Parse(text);
        }

        [TestMethod, ExpectedException(typeof(JsonFormatException))]
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

            parser.Parse(text);
        }
        
        [TestMethod, ExpectedException(typeof(JsonFormatException))]
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

            parser.Parse(text);
        }

        [TestMethod, ExpectedException(typeof(JsonFormatException))]
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

            parser.Parse(text);
        }

        [TestMethod]
        public void MissingWeightTrait()
        {
            const string CATEGORY = "Colour";
            const string WEIGHTLESS_TRAIT = "Green";

            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{CATEGORY}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : '{WEIGHTLESS_TRAIT}'
                            }},
                            {{ 
                                'name' : 'Red', 
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
            Assert.AreEqual(1, categories.Count, "Schema has incorrect number of TraitCategories");
            
            TraitCategory category = categories[0];
            Trait weightlessTrait = category.GetTrait(WEIGHTLESS_TRAIT);
            Assert.IsNotNull(weightlessTrait, "Trait is not found");
            Assert.AreEqual(JsonConfigurationParser.DEFAULT_WEIGHT, weightlessTrait.Weight, "Wrong default weight");
        }

        [TestMethod, ExpectedException(typeof(JsonFormatException))]
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

            parser.Parse(text);
        }

        [TestMethod, ExpectedException(typeof(JsonFormatException))]
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

            parser.Parse(text);
        }

        [TestMethod, ExpectedException(typeof(JsonFormatException))]
        public void EmptyJsonObject()
        {
            string text = "{}";

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            parser.Parse(text);
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

        [TestMethod, ExpectedException(typeof(JsonFormatException))]
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

            parser.Parse(text);
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

            Assert.AreEqual(2, categories.Count, "Wrong number of categories");
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
                parser.Parse(text);
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

        [DataTestMethod]
        [DataRow(true, DisplayName = "True")]
        [DataRow(false, DisplayName = "False")]
        public void HiddenCategory(bool isHidden)
        {
            const string CATEGORY = "Hair";

            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{CATEGORY}',
                        'selections' : 1,
                        'hidden' : {isHidden.ToString().ToLower()},
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

            TraitSchema schema = parser.Parse(text);
            IReadOnlyList<TraitCategory> categories = schema.GetTraitCategories();

            Assert.AreEqual(1, categories.Count, "Wrong number of categories");
            TraitCategory category = categories[0];
            Assert.AreEqual(CATEGORY, category.Name, "Wrong category name");
            Assert.AreEqual(isHidden, category.IsHidden, "Wrong IsHidden value");
        }

        [TestMethod]
        public void HiddenAndVisibleCategories()
        {
            const string CATEGORY0 = "Animal";
            const bool CATEGORY0_IS_HIDDEN = false;
            const string CATEGORY1 = "Colour";
            const bool CATEGORY1_IS_HIDDEN = true;

            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{CATEGORY0}',
                        'selections' : 1,
                        'hidden' : {CATEGORY0_IS_HIDDEN.ToString().ToLower()},
                        'traits' : [
                            {{ 
                                'name' : 'Bear', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : '{CATEGORY1}',
                        'selections' : 1,
                        'hidden' : {CATEGORY1_IS_HIDDEN.ToString().ToLower()},
                        'traits' : [
                            {{ 
                                'name' : 'Blue', 
                                'weight' : 1
                            }}
                        ]
                    }}
                ]
            }}";

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            TraitSchema schema = parser.Parse(text);
            IReadOnlyList<TraitCategory> categories = schema.GetTraitCategories();

            Assert.AreEqual(2, categories.Count, "Wrong number of categories");
            TraitCategory category0 = ListUtil.Find(categories, category => category.Name == CATEGORY0);
            Assert.IsNotNull(category0, "Category is missing");
            Assert.AreEqual(CATEGORY0_IS_HIDDEN, category0.IsHidden, "Wrong IsHidden value");
            TraitCategory category1 = ListUtil.Find(categories, category => category.Name == CATEGORY1);
            Assert.IsNotNull(category1, "Category is missing");
            Assert.AreEqual(CATEGORY1_IS_HIDDEN, category1.IsHidden, "Wrong IsHidden value");
        }

        [TestMethod, ExpectedException(typeof(JsonFormatException))]
        public void CategoryHiddenAsInt()
        {
            const string CATEGORY = "Hair";

            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{CATEGORY}',
                        'selections' : 1,
                        'hidden' : 1,
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

            parser.Parse(text);
        }

        [TestMethod]
        public void ConflictingOutputCategoryVisibilityWith2Conflicts()
        {
            const string OUTPUT_CATEGORY_NAME = "Fame";
            const string CATEGORY0 = "Young Fame";
            const bool CATEGORY0_IS_HIDDEN = true;
            const string CATEGORY1 = "Old Fame";
            const bool CATEGORY1_IS_HIDDEN = false;

            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{CATEGORY0}',
                        'selections' : 1,
                        'output_name' : '{OUTPUT_CATEGORY_NAME}',
                        'hidden' : {CATEGORY0_IS_HIDDEN.ToString().ToLower()},
                        'traits' : [
                            {{ 
                                'name' : 'Social Media', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : '{CATEGORY1}',
                        'selections' : 1,
                        'output_name' : '{OUTPUT_CATEGORY_NAME}',
                        'hidden' : {CATEGORY1_IS_HIDDEN.ToString().ToLower()},
                        'traits' : [
                            {{ 
                                'name' : 'Radio', 
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
                parser.Parse(text);
            }
            catch (ConflictingCategoryVisibilityException exception)
            {
                threwException = true;

                Assert.AreEqual(2, exception.ConflictingCategories.Count, "Wrong number of conflicting categories");

                TraitCategory category0 = ListUtil.Find(exception.ConflictingCategories, category => category.Name == CATEGORY0);
                Assert.IsNotNull(category0, "Wrong category in exception");

                TraitCategory category1 = ListUtil.Find(exception.ConflictingCategories, category => category.Name == CATEGORY1);
                Assert.IsNotNull(category1, "Wrong category in exception");
            }

            Assert.IsTrue(threwException, "Failed to throw ConflictingCategoryVisibilityException");
        }

        [TestMethod]
        public void ConflictingOutputCategoryVisibilityWithManyConflicts()
        {
            const string OUTPUT_CATEGORY_NAME = "Fame";
            const string CATEGORY0 = "Young Fame";
            const bool CATEGORY0_IS_HIDDEN = true;
            const string CATEGORY1 = "Old Fame";
            const bool CATEGORY1_IS_HIDDEN = false;
            const string CATEGORY2 = "Ancient Fame";
            const bool CATEGORY2_IS_HIDDEN = true;

            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{CATEGORY0}',
                        'selections' : 1,
                        'output_name' : '{OUTPUT_CATEGORY_NAME}',
                        'hidden' : {CATEGORY0_IS_HIDDEN.ToString().ToLower()},
                        'traits' : [
                            {{ 
                                'name' : 'Social Media', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : '{CATEGORY1}',
                        'selections' : 1,
                        'output_name' : '{OUTPUT_CATEGORY_NAME}',
                        'hidden' : {CATEGORY1_IS_HIDDEN.ToString().ToLower()},
                        'traits' : [
                            {{ 
                                'name' : 'Radio', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : 'Animal',
                        'selections' : 1,
                        'output_name' : 'Species',
                        'traits' : [
                            {{ 
                                'name' : 'Bear', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : '{CATEGORY2}',
                        'selections' : 1,
                        'output_name' : '{OUTPUT_CATEGORY_NAME}',
                        'hidden' : {CATEGORY2_IS_HIDDEN.ToString().ToLower()},
                        'traits' : [
                            {{ 
                                'name' : 'Cave Painting', 
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
                parser.Parse(text);
            }
            catch (ConflictingCategoryVisibilityException exception)
            {
                threwException = true;

                Assert.AreEqual(3, exception.ConflictingCategories.Count, "Wrong number of conflicting categories");

                TraitCategory category0 = ListUtil.Find(exception.ConflictingCategories, category => category.Name == CATEGORY0);
                Assert.IsNotNull(category0, "Wrong category in exception");

                TraitCategory category1 = ListUtil.Find(exception.ConflictingCategories, category => category.Name == CATEGORY1);
                Assert.IsNotNull(category1, "Wrong category in exception");

                TraitCategory category2 = ListUtil.Find(exception.ConflictingCategories, category => category.Name == CATEGORY2);
                Assert.IsNotNull(category2, "Wrong category in exception");
            }

            Assert.IsTrue(threwException, "Failed to throw ConflictingCategoryVisibilityException");
        }

        [TestMethod]
        public void CategoryHiddenSharingOutputCategories()
        {
            const string OUTPUT_CATEGORY_NAME = "Fame";
            const string CATEGORY0 = "Young Fame";
            const bool IS_HIDDEN = true;
            const string CATEGORY1 = "Old Fame";

            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{CATEGORY0}',
                        'selections' : 1,
                        'output_name' : '{OUTPUT_CATEGORY_NAME}',
                        'hidden' : {IS_HIDDEN.ToString().ToLower()},
                        'traits' : [
                            {{ 
                                'name' : 'Social Media', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : '{CATEGORY1}',
                        'selections' : 1,
                        'output_name' : '{OUTPUT_CATEGORY_NAME}',
                        'hidden' : {IS_HIDDEN.ToString().ToLower()},
                        'traits' : [
                            {{ 
                                'name' : 'Radio', 
                                'weight' : 1
                            }}
                        ]
                    }}
                ]
            }}";

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);
            
            TraitSchema schema = parser.Parse(text);

            Assert.IsNotNull(schema, "Failed to generate a schema from the valid text");
        }

        [TestMethod]
        public void CategoryOrderingPartial()
        {
            const string CATEGORY0 = "Animal";
            const string CATEGORY1 = "Colour";

            string text = $@"{{
                'category_order': [
                    '{CATEGORY1}',
                ],
                'trait_categories' : [
                    {{
                        'name' : '{CATEGORY0}',
                        'selections' : 1,
                        'traits' : [
                            {{ 
                                'name' : 'Bear', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : '{CATEGORY1}',
                        'selections' : 1,
                        'traits' : [
                            {{ 
                                'name' : 'Blue', 
                                'weight' : 1
                            }}
                        ]
                    }}
                ]
            }}";

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            TraitSchema schema = parser.Parse(text);

            Assert.IsNotNull(schema, "Failed to generate a schema from the valid text");

            IReadOnlyList<string> order = schema.GetCategoryOrder();
            Assert.AreEqual(1, order.Count, "Wrong number of elements in category ordering");
            Assert.AreEqual(CATEGORY1, order[0], "Wrong element in category order");
        }

        [TestMethod]
        public void CategoryOrderingFull()
        {
            const string CATEGORY0 = "Animal";
            const string CATEGORY1 = "Colour";
            const string CATEGORY2 = "Location";

            string text = $@"{{
                'category_order': [
                    '{CATEGORY1}',
                    '{CATEGORY2}',
                    '{CATEGORY0}',
                ],
                'trait_categories' : [
                    {{
                        'name' : '{CATEGORY0}',
                        'selections' : 1,
                        'traits' : [
                            {{ 
                                'name' : 'Bear', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : '{CATEGORY1}',
                        'selections' : 1,
                        'traits' : [
                            {{ 
                                'name' : 'Blue', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : '{CATEGORY2}',
                        'selections' : 1,
                        'traits' : [
                            {{ 
                                'name' : 'Bridge', 
                                'weight' : 1
                            }}
                        ]
                    }},
                ]
            }}";

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            TraitSchema schema = parser.Parse(text);

            Assert.IsNotNull(schema, "Failed to generate a schema from the valid text");

            IReadOnlyList<string> order = schema.GetCategoryOrder();
            Assert.AreEqual(3, order.Count, "Wrong number of elements in category ordering");
            Assert.AreEqual(CATEGORY1, order[0], "Wrong element in category order");
            Assert.AreEqual(CATEGORY2, order[1], "Wrong element in category order");
            Assert.AreEqual(CATEGORY0, order[2], "Wrong element in category order");
        }

        [TestMethod]
        public void CategoryOrderingPrefersOutputNameToName()
        {
            const string CATEGORY0 = "Animal";
            const string CATEGORY1 = "Colour";
            const string OUTPUT_NAME1 = "Shade";

            string text = $@"{{
                'category_order': [
                    '{OUTPUT_NAME1}',
                    '{CATEGORY0}',
                ],
                'trait_categories' : [
                    {{
                        'name' : '{CATEGORY0}',
                        'selections' : 1,
                        'traits' : [
                            {{ 
                                'name' : 'Bear', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : '{CATEGORY1}',
                        'output_name' : '{OUTPUT_NAME1}',
                        'selections' : 1,
                        'traits' : [
                            {{ 
                                'name' : 'Blue', 
                                'weight' : 1
                            }}
                        ]
                    }},
                ]
            }}";

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            TraitSchema schema = parser.Parse(text);

            Assert.IsNotNull(schema, "Failed to generate a schema from the valid text");

            IReadOnlyList<string> order = schema.GetCategoryOrder();
            Assert.AreEqual(2, order.Count, "Wrong number of elements in category ordering");
            Assert.AreEqual(OUTPUT_NAME1, order[0], "Wrong element in category order");
            Assert.AreEqual(CATEGORY0, order[1], "Wrong element in category order");
        }

        [TestMethod]
        public void CategoryOrderingWithSharedOutputNames()
        {
            const string CATEGORY0 = "Animal";
            const string CATEGORY1 = "Old Fame";
            const string CATEGORY2 = "Young Fame";
            const string OUTPUT_NAME = "Fame";

            string text = $@"{{
                'category_order': [
                    '{OUTPUT_NAME}',
                    '{CATEGORY0}',
                ],
                'trait_categories' : [
                    {{
                        'name' : '{CATEGORY0}',
                        'selections' : 1,
                        'traits' : [
                            {{ 
                                'name' : 'Bear', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : '{CATEGORY1}',
                        'output_name' : '{OUTPUT_NAME}',
                        'selections' : 1,
                        'traits' : [
                            {{ 
                                'name' : 'Radio', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : '{CATEGORY2}',
                        'output_name' : '{OUTPUT_NAME}',
                        'selections' : 1,
                        'traits' : [
                            {{ 
                                'name' : 'Pro Gamer', 
                                'weight' : 1
                            }}
                        ]
                    }},
                ]
            }}";

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            TraitSchema schema = parser.Parse(text);

            Assert.IsNotNull(schema, "Failed to generate a schema from the valid text");

            IReadOnlyList<string> order = schema.GetCategoryOrder();
            Assert.AreEqual(2, order.Count, "Wrong number of elements in category ordering");
            Assert.AreEqual(OUTPUT_NAME, order[0], "Wrong element in category order");
            Assert.AreEqual(CATEGORY0, order[1], "Wrong element in category order");
        }

        [TestMethod, ExpectedException(typeof(JsonFormatException))]
        public void CategoryOrderingDuplicate()
        {
            const string CATEGORY0 = "Animal";
            const string CATEGORY1 = "Colour";

            string text = $@"{{
                'category_order': [
                    '{CATEGORY1}',
                    '{CATEGORY1}',
                ],
                'trait_categories' : [
                    {{
                        'name' : '{CATEGORY0}',
                        'selections' : 1,
                        'traits' : [
                            {{ 
                                'name' : 'Bear', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : '{CATEGORY1}',
                        'selections' : 1,
                        'traits' : [
                            {{ 
                                'name' : 'Blue', 
                                'weight' : 1
                            }}
                        ]
                    }}
                ]
            }}";

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            parser.Parse(text);
        }

        [TestMethod]
        public void CategoryOrderingUnknown()
        {
            const string CATEGORY0 = "Animal";
            const string CATEGORY1 = "Colour";
            const string UNKNOWN_CATEGORY = "Location";

            string text = $@"{{
                'category_order': [
                    '{CATEGORY1}',
                    '{UNKNOWN_CATEGORY}',
                ],
                'trait_categories' : [
                    {{
                        'name' : '{CATEGORY0}',
                        'selections' : 1,
                        'traits' : [
                            {{ 
                                'name' : 'Bear', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : '{CATEGORY1}',
                        'selections' : 1,
                        'traits' : [
                            {{ 
                                'name' : 'Blue', 
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
                parser.Parse(text);
            }
            catch (OrderCategoryNotFoundException exception)
            {
                threwException = true;

                Assert.AreEqual(UNKNOWN_CATEGORY, exception.Category, "Wrong unknown category");
            }

            Assert.IsTrue(threwException, "Failed to throw OrderCategoryNotFoundException");
        }

        private readonly MockRandom m_random = new MockRandom();
    }
}
