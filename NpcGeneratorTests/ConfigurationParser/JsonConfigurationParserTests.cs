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
using System;
using System.Collections.Generic;
using System.IO;

namespace Tests
{
    [TestClass]
    public class JsonConfigurationParserTests : FileCreatingTests
    {
        const string SCHEMA_PATH = "ConfigurationSchema.json";

        [TestMethod]
        public void GeneratesTraitSchema()
        {
            string path = Path.Combine(TestDirectory, "colour.json");
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
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);
            TraitSchema schema = parser.Parse(path);
            Assert.IsNotNull(schema, "Failed to generate a schema from the valid text");

            File.Delete(path);
        }

        [TestMethod]
        public void SchemaIsOptional()
        {
            string path = Path.Combine(TestDirectory, "colour.json");
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
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(null);
            TraitSchema schema = parser.Parse(path);
            Assert.IsNotNull(schema, "Failed to generate a schema from the valid text");

            File.Delete(path);
        }

        [TestMethod]
        public void MalformedJson()
        {
            string path = Path.Combine(TestDirectory, "malformed.json");
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
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (JsonReaderException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Malformed Json failed to throw exception");

            File.Delete(path);
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

            string path = Path.Combine(TestDirectory, "colourAndAnimal.json");
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
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);
            TraitSchema schema = parser.Parse(path);
            Assert.IsNotNull(schema, "Failed to generate a schema from the valid text");

            IReadOnlyList<TraitCategory> categories = schema.GetTraitCategories();
            Assert.IsNotNull(categories, "Schema categories is null, which should be impossible");
            Assert.AreEqual(2, categories.Count, "Schema has incorrect number of TraitCategories");
            TraitCategory firstCategory = categories[0];
            Assert.IsNotNull(firstCategory, "Schema has a null first TraitCategory");
            Assert.AreEqual(firstCategory.Name, CATEGORY1_TITLE, "First category doesn't have name " + CATEGORY1_TITLE);
            Assert.AreEqual(CATEGORY1_SELECTION_COUNT, firstCategory.DefaultSelectionCount, "First category has wrong SelectionCount");
            TraitChooser firstChooser = firstCategory.CreateTraitChooser(m_random);
            string[] colours = firstChooser.Choose(firstCategory.DefaultSelectionCount, out List<BonusSelection> bonusSelections1);
            Assert.AreEqual(0, bonusSelections1.Count, "Bonus selection returned where there should be none.");
            Assert.AreEqual(colours.Length, 1, "Wrong number of selections from " + CATEGORY1_TITLE);
            Assert.IsTrue(colours[0] == CATEGORY1_TRAIT1 || colours[0] == CATEGORY1_TRAIT2, CATEGORY1_TITLE + 
                " chose an invalid trait " + colours[0]);

            TraitCategory secondCategory = categories[1];
            Assert.IsNotNull(secondCategory, "Schema has a null second TraitCategory");
            Assert.AreEqual(secondCategory.Name, CATEGORY2_TITLE, "Second category doesn't have name " + CATEGORY2_TITLE);
            Assert.AreEqual(CATEGORY2_SELECTION_COUNT, secondCategory.DefaultSelectionCount, "Second category has wrong SelectionCount");
            TraitChooser secondChooser = secondCategory.CreateTraitChooser(m_random);
            string[] animals = secondChooser.Choose(secondCategory.DefaultSelectionCount, out List<BonusSelection> bonusSelections2);
            Assert.AreEqual(0, bonusSelections2.Count, "Bonus selection returned where there should be none.");
            Assert.AreEqual(animals.Length, 2, "Wrong number of selections from " + CATEGORY2_TITLE);
            Assert.IsTrue((animals[0] == CATEGORY2_TRAIT1 || animals[0] == CATEGORY2_TRAIT2), 
                "Incorrect first animal selected: " + animals[0]);
            Assert.IsTrue((animals[1] == CATEGORY2_TRAIT1 || animals[1] == CATEGORY2_TRAIT2), 
                "Incorrect second animal selected: " + animals[1]);
            Assert.AreNotEqual(animals[0], animals[1], "Did not select both animals");

            File.Delete(path);
        }

        [TestMethod]
        public void MissingTraitTitleThrowsException()
        {
            string path = Path.Combine(TestDirectory, "missingTitle.json");
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
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (JsonFormatException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Missing title failed to throw exception");

            File.Delete(path);
        }

        [TestMethod]
        public void MissingTraitsThrowsException()
        {
            string path = Path.Combine(TestDirectory, "missingTraits.json");
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
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (JsonFormatException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Missing traits failed to throw exception");

            File.Delete(path);
        }
        
        [TestMethod]
        public void MissingTraitName()
        {
            string path = Path.Combine(TestDirectory, "missingTraitName.json");
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
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (JsonFormatException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Missing trait name did not throw exception");

            File.Delete(path);
        }

        [TestMethod]
        public void NegativeWeightTrait()
        {
            string path = Path.Combine(TestDirectory, "negativeWeightTrait.json");
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
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (JsonFormatException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Negative trait weight did not throw exception");

            File.Delete(path);
        }

        [TestMethod]
        public void MissingWeightTrait()
        {
            string path = Path.Combine(TestDirectory, "missingWeightTrait.json");
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
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (JsonFormatException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Missing trait weight did not throw exception");

            File.Delete(path);
        }

        [TestMethod]
        public void MissingTraitCategoryName()
        {
            string path = Path.Combine(TestDirectory, "missingTraitCategoryName.json");
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
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (JsonFormatException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Missing trait category name did not throw exception");

            File.Delete(path);
        }

        [TestMethod]
        public void MissingSelections()
        {
            string path = Path.Combine(TestDirectory, "colour.json");
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
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (JsonFormatException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Missing trait selection number did not throw exception");

            File.Delete(path);
        }

        [TestMethod]
        public void EmptyJsonObject()
        {
            string path = Path.Combine(TestDirectory, "empty.json");
            string text = "{}";
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (JsonFormatException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Empty json did not throw exception");

            File.Delete(path);
        }

        [TestMethod]
        public void DuplicateCategoryNames()
        {
            const string CATEGORY = "Colour";
            string path = Path.Combine(TestDirectory, "colour.json");
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
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool throwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch(DuplicateCategoryNameException exception)
            {
                Assert.AreEqual(CATEGORY, exception.Category, "Reported category is wrong");

                throwException = true;
            }
            
            Assert.IsTrue(throwException, "Duplicate categories were not rejected.");

            File.Delete(path);
        }

        [TestMethod]
        public void DuplicateTraitNamesInACategory()
        {
            const string CATEGORY = "Colour";
            const string TRAIT = "Green";
            string path = Path.Combine(TestDirectory, "colour.json");
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
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool throwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (JsonFormatException)
            {
                throwException = true;
            }

            Assert.IsTrue(throwException, "Duplicate trait names in the same category were not rejected.");

            File.Delete(path);
        }

        [TestMethod]
        public void DuplicateTraitNamesButDifferentAttributesInACategory()
        {
            const string CATEGORY = "Colour";
            const string TRAIT = "Green";
            string path = Path.Combine(TestDirectory, "colour.json");
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
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool throwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (DuplicateTraitNameInCategoryException exception)
            {
                Assert.AreEqual(new TraitId(CATEGORY, TRAIT), exception.TraitId, "Wrong duplicates identified");

                throwException = true;
            }

            Assert.IsTrue(throwException, "Duplicate trait names in the same category were not rejected.");

            File.Delete(path);
        }

        [TestMethod]
        public void DuplicateTraitNamesInDifferentCategories()
        {
            const string TRAIT_NAME = "Brown";
            string path = Path.Combine(TestDirectory, "person.json");
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
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            TraitSchema schema = parser.Parse(path);
            IReadOnlyList<TraitCategory> categories = schema.GetTraitCategories();

            Assert.AreEqual(2, categories.Count);
            Assert.IsNotNull(categories[0].GetTrait(TRAIT_NAME), "Missing expected trait in category");
            Assert.IsNotNull(categories[1].GetTrait(TRAIT_NAME), "Missing expected trait in category");

            File.Delete(path);
        }

        [TestMethod]
        public void TooFewTraitsinCategory()
        {
            const string CATEGORY = "Hair";
            const int SELECTIONS = 10;
            string path = Path.Combine(TestDirectory, "hair.json");
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
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (TooFewTraitsInCategoryException exception)
            {
                Assert.AreEqual(CATEGORY, exception.Category, "Wrong category name");
                Assert.AreEqual(SELECTIONS, exception.Requested, "Wrong number of requested traits");
                Assert.AreEqual(1, exception.Available, "Wrong number of available traits");
                threwException = true;
            }

            Assert.IsTrue(threwException, "Failed to throw a TooFewTraitsInCategoryException");

            File.Delete(path);
        }

        [TestMethod]
        public void ReplacementsForSchemaWithReplacements()
        {
            const string REPLACEMENT_CATEGORY = "Colour";
            const string REPLACEMENT_TRAIT = "Green";
            string path = Path.Combine(TestDirectory, "replacements.json");
            string text = $@"{{
                'replacements' : [
                    {{
                        'category_name' : '{REPLACEMENT_CATEGORY}',
                        'trait_name' : '{REPLACEMENT_TRAIT}'
                    }}
                ],
                'trait_categories' : [
                    {{
                        'name' : '{REPLACEMENT_CATEGORY}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : '{REPLACEMENT_TRAIT}', 
                                'weight' : 1
                            }},
                            {{ 
                                'name' : 'Red', 
                                'weight' : 1
                            }}
                        ]
                    }}
                ]
            }}";
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);
            TraitSchema schema = parser.Parse(path);
            Assert.IsNotNull(schema, "Failed to generate a schema from the valid text");

            IReadOnlyList<ReplacementSearch> replacements = schema.GetReplacementSearches();
            Assert.AreEqual(1, replacements.Count, "Wrong number of replacements found.");
            Assert.AreEqual(REPLACEMENT_CATEGORY, replacements[0].Category.Name, "Wrong replacement category");
            Assert.AreEqual(REPLACEMENT_TRAIT, replacements[0].Trait.Name, "Wrong replacement trait");

            File.Delete(path);
        }

        [TestMethod]
        public void ReplacementsForSchemaWithMultipleReplacements()
        {
            const string REPLACEMENT_CATEGORY0 = "Colour";
            const string REPLACEMENT_TRAIT0 = "Green";
            const string REPLACEMENT_CATEGORY1 = "Animal";
            const string REPLACEMENT_TRAIT1 = "Bear";
            string path = Path.Combine(TestDirectory, "replacements.json");
            string text = $@"{{
                'replacements' : [
                    {{
                        'category_name' : '{REPLACEMENT_CATEGORY0}',
                        'trait_name' : '{REPLACEMENT_TRAIT0}'
                    }},
                    {{
                        'category_name' : '{REPLACEMENT_CATEGORY1}',
                        'trait_name' : '{REPLACEMENT_TRAIT1}'
                    }}
                ],
                'trait_categories' : [
                    {{
                        'name' : '{REPLACEMENT_CATEGORY0}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : '{REPLACEMENT_TRAIT0}', 
                                'weight' : 1
                            }},
                            {{ 
                                'name' : 'Red', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : '{REPLACEMENT_CATEGORY1}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : '{REPLACEMENT_TRAIT1}', 
                                'weight' : 1
                            }},
                            {{ 
                                'name' : 'Rhino', 
                                'weight' : 1
                            }}
                        ]
                    }}
                ]
            }}";
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);
            TraitSchema schema = parser.Parse(path);
            Assert.IsNotNull(schema, "Failed to generate a schema from the valid text");

            IReadOnlyList<ReplacementSearch> replacements = schema.GetReplacementSearches();
            Assert.AreEqual(2, replacements.Count, "Wrong number of replacements found.");

            ReplacementSearch replacement0 = ListUtil.Find(replacements, replacement => replacement.Category.Name == REPLACEMENT_CATEGORY0);
            Assert.AreEqual(REPLACEMENT_CATEGORY0, replacement0.Category.Name, "Wrong replacement category");
            Assert.AreEqual(REPLACEMENT_TRAIT0, replacement0.Trait.Name, "Wrong replacement trait");

            ReplacementSearch replacement1 = ListUtil.Find(replacements, replacement => replacement.Category.Name == REPLACEMENT_CATEGORY1);
            Assert.AreEqual(REPLACEMENT_CATEGORY1, replacement1.Category.Name, "Wrong replacement category");
            Assert.AreEqual(REPLACEMENT_TRAIT1, replacement1.Trait.Name, "Wrong replacement trait");

            File.Delete(path);
        }

        [TestMethod]
        public void ReplacementHasMissingTrait()
        {
            const string REPLACEMENT_CATEGORY = "Colour";
            const string MISSING_REPLACEMENT_TRAIT = "Blue";
            string path = Path.Combine(TestDirectory, "replacements.json");
            string text = $@"{{
                'replacements' : [
                    {{
                        'category_name' : '{REPLACEMENT_CATEGORY}',
                        'trait_name' : '{MISSING_REPLACEMENT_TRAIT}'
                    }}
                ],
                'trait_categories' : [
                    {{
                        'name' : '{REPLACEMENT_CATEGORY}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : 'Green', 
                                'weight' : 1
                            }},
                            {{ 
                                'name' : 'Red', 
                                'weight' : 1
                            }}
                        ]
                    }}
                ]
            }}";
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (MissingReplacementTraitException exception)
            {
                Assert.AreEqual(new TraitId(REPLACEMENT_CATEGORY, MISSING_REPLACEMENT_TRAIT), exception.TraitId, "Wrong replacement trait");

                threwException = true;
            }

            Assert.IsTrue(threwException, "Faile to throw a MissingReplacementTraitException exception");

            File.Delete(path);
        }

        [TestMethod]
        public void ReplacementHasMissingCategory()
        {
            const string MISSING_REPLACEMENT_CATEGORY = "Hair";
            const string REPLACEMENT_TRAIT = "Green";
            string path = Path.Combine(TestDirectory, "replacements.json");
            string text = $@"{{
                'replacements' : [
                    {{
                        'category_name' : '{MISSING_REPLACEMENT_CATEGORY}',
                        'trait_name' : '{REPLACEMENT_TRAIT}'
                    }}
                ],
                'trait_categories' : [
                    {{
                        'name' : 'Colour',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : '{REPLACEMENT_TRAIT}', 
                                'weight' : 1
                            }},
                            {{ 
                                'name' : 'Red', 
                                'weight' : 1
                            }}
                        ]
                    }}
                ]
            }}";
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (MissingReplacementCategoryException exception)
            {
                Assert.AreEqual(new TraitId(MISSING_REPLACEMENT_CATEGORY, REPLACEMENT_TRAIT), exception.TraitId, "Wrong replacement trait");

                threwException = true;
            }

            Assert.IsTrue(threwException, "Failed to throw MissingReplacementCategoryException");

            File.Delete(path);
        }

        [TestMethod]
        public void ReplacementHasNoCategory()
        {
            string path = Path.Combine(TestDirectory, "replacements.json");
            string text = $@"{{
                'replacements' : [
                    {{
                        'category_name' : 'Colour'
                    }}
                ],
                'trait_categories' : [
                    {{
                        'name' : 'Colour',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : 'Green', 
                                'weight' : 1
                            }},
                            {{ 
                                'name' : 'Red', 
                                'weight' : 1
                            }}
                        ]
                    }}
                ]
            }}";
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (JsonFormatException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Failed to throw JsonFormatException");

            File.Delete(path);
        }

        [TestMethod]
        public void ReplacementHasNoTrait()
        {
            string path = Path.Combine(TestDirectory, "replacements.json");
            string text = $@"{{
                'replacements' : [
                    {{
                        'trait_name' : 'Green'
                    }}
                ],
                'trait_categories' : [
                    {{
                        'name' : 'Colour',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : 'Green', 
                                'weight' : 1
                            }},
                            {{ 
                                'name' : 'Red', 
                                'weight' : 1
                            }}
                        ]
                    }}
                ]
            }}";
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (JsonFormatException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Failed to throw JsonFormatException");

            File.Delete(path);
        }

        [TestMethod]
        public void ReplacementIsEmpty()
        {
            string path = Path.Combine(TestDirectory, "replacements.json");
            string text = $@"{{
                'replacements' : [
                    {{
                    }}
                ],
                'trait_categories' : [
                    {{
                        'name' : 'Colour',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : 'Green', 
                                'weight' : 1
                            }},
                            {{ 
                                'name' : 'Red', 
                                'weight' : 1
                            }}
                        ]
                    }}
                ]
            }}";
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (JsonFormatException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Failed to throw JsonFormatException");

            File.Delete(path);
        }

        [TestMethod]
        public void ReplacementHasMultipleIdenticalObjects()
        {
            const string REPLACEMENT_CATEGORY = "Hair";
            const string REPLACEMENT_TRAIT = "Green";
            string path = Path.Combine(TestDirectory, "replacements.json");
            string text = $@"{{
                'replacements' : [
                    {{
                        'category_name' : '{REPLACEMENT_CATEGORY}',
                        'trait_name' : '{REPLACEMENT_TRAIT}'
                    }},
                    {{
                        'category_name' : '{REPLACEMENT_CATEGORY}',
                        'trait_name' : '{REPLACEMENT_TRAIT}'
                    }}
                ],
                'trait_categories' : [
                    {{
                        'name' : '{REPLACEMENT_CATEGORY}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : '{REPLACEMENT_TRAIT}', 
                                'weight' : 1
                            }},
                            {{ 
                                'name' : 'Red', 
                                'weight' : 1
                            }}
                        ]
                    }}
                ]
            }}";
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (JsonFormatException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Failed to throw JsonFormatException");

            File.Delete(path);
        }

        [TestMethod]
        public void BonusSelectionForMissingCategory()
        {
            const string PRESENT_CATEGORY = "Animal";
            const string PRESENT_TRAIT = "Bear";
            const string MISSING_CATEGORY = "Hair";
            string path = Path.Combine(TestDirectory, "missingHair.json");
            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{PRESENT_CATEGORY}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : '{PRESENT_TRAIT}', 
                                'weight' : 1,
                                'bonus_selection' : {{ 'trait_category_name' : '{MISSING_CATEGORY}', 'selections' : 1 }}
                            }}
                        ]
                    }}
                ]
            }}";
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (MismatchedBonusSelectionException exception)
            {
                Assert.AreEqual(MISSING_CATEGORY, exception.NotFoundCategoryName, "Wrong missing category name");
                Assert.AreEqual(new TraitId(PRESENT_CATEGORY, PRESENT_TRAIT), exception.SourceTraitId, "Wrong SourceTraitId");
                threwException = true;
            }

            Assert.IsTrue(threwException, "Failed to throw a MismatchedBonusSelectionException");

            File.Delete(path);
        }

        [TestMethod]
        public void BonusSelectionWithMissingTraitName()
        {
            string path = Path.Combine(TestDirectory, "bonusSelection.json");
            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : 'Animal',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : 'Bear', 
                                'weight' : 1,
                                'bonus_selection' : {{ 'selections' : 1 }}
                            }}
                        ]
                    }}
                ]
            }}";
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (JsonFormatException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Failed to throw a JsonFormatException");

            File.Delete(path);
        }

        [TestMethod]
        public void BonusSelectionWithMissingSelection()
        {
            string path = Path.Combine(TestDirectory, "bonusSelection.json");
            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : 'Animal',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : 'Bear', 
                                'weight' : 1,
                                'bonus_selection' : {{ 'trait_category_name' : 'Animal' }}
                            }}
                        ]
                    }}
                ]
            }}";
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (JsonFormatException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Failed to throw a JsonFormatException");

            File.Delete(path);
        }

        [TestMethod]
        public void BonusSelectionWithNegativeSelection()
        {
            string path = Path.Combine(TestDirectory, "bonusSelection.json");
            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : 'Animal',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : 'Bear', 
                                'weight' : 1,
                                'bonus_selection' : {{ 'trait_category_name' : 'Animal', 'selections' : -1 }}
                            }}
                        ]
                    }}
                ]
            }}";
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (JsonFormatException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Failed to throw a JsonFormatException");

            File.Delete(path);
        }

        [TestMethod]
        public void BonusSelectionWithFractionSelection()
        {
            string path = Path.Combine(TestDirectory, "bonusSelection.json");
            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : 'Animal',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : 'Bear', 
                                'weight' : 1,
                                'bonus_selection' : {{ 'trait_category_name' : 'Animal', 'selections' : 1.5 }}
                            }}
                        ]
                    }}
                ]
            }}";
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (JsonFormatException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Failed to throw a JsonFormatException");

            File.Delete(path);
        }

        [TestMethod]
        public void BonusSelectionWithTextSelection()
        {
            string path = Path.Combine(TestDirectory, "bonusSelection.json");
            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : 'Animal',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : 'Bear', 
                                'weight' : 1,
                                'bonus_selection' : {{ 'trait_category_name' : 'Animal', 'selections' : '1' }}
                            }}
                        ]
                    }}
                ]
            }}";
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (JsonFormatException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Failed to throw a JsonFormatException");

            File.Delete(path);
        }

        [TestMethod]
        public void BonusSelectionWithOneSelfSelection()
        {
            const string TRAIT = "Bear";
            const string CATEGORY = "Animal";
            const int SELECTION_COUNT = 1;

            string path = Path.Combine(TestDirectory, "bonusSelection.json");
            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{CATEGORY}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : '{TRAIT}', 
                                'weight' : 1,
                                'bonus_selection' : {{ 'trait_category_name' : '{CATEGORY}', 'selections' : {SELECTION_COUNT} }}
                            }}
                        ]
                    }}
                ]
            }}";
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            TraitSchema schema = parser.Parse(path);

            IReadOnlyList<TraitCategory> categories = schema.GetTraitCategories();
            TraitCategory category = categories[0];
            Trait trait = category.GetTrait(TRAIT);
            BonusSelection bonusSelection = trait.BonusSelection;

            Assert.IsNotNull(bonusSelection, "BonusSelection is null");
            Assert.AreEqual(CATEGORY, bonusSelection.TraitCategory.Name, "Wrong category name in BonusSelection");
            Assert.AreEqual(SELECTION_COUNT, bonusSelection.SelectionCount, "Wrong selection count in BonusSelection");

            File.Delete(path);
        }

        [TestMethod]
        public void BonusSelectionWithZeroSelections()
        {
            const string TRAIT = "Bear";
            const string CATEGORY = "Animal";
            const int SELECTION_COUNT = 0;

            string path = Path.Combine(TestDirectory, "bonusSelection.json");
            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{CATEGORY}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : '{TRAIT}', 
                                'weight' : 1,
                                'bonus_selection' : {{ 'trait_category_name' : '{CATEGORY}', 'selections' : {SELECTION_COUNT} }}
                            }}
                        ]
                    }}
                ]
            }}";
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            TraitSchema schema = parser.Parse(path);

            IReadOnlyList<TraitCategory> categories = schema.GetTraitCategories();
            TraitCategory category = categories[0];
            Trait trait = category.GetTrait(TRAIT);
            BonusSelection bonusSelection = trait.BonusSelection;

            Assert.IsNotNull(bonusSelection, "BonusSelection is null");
            Assert.AreEqual(CATEGORY, bonusSelection.TraitCategory.Name, "BonusSelection has the wrong category name");
            Assert.AreEqual(SELECTION_COUNT, bonusSelection.SelectionCount, "BonusSelection has the wrong selection count");

            File.Delete(path);
        }

        [TestMethod]
        public void BonusSelectionForADifferentCategory()
        {
            const string TRAIT = "Bear";
            const string SOURCE_CATEGORY = "Animal";
            const string TARGET_CATEGORY = "Hair";
            const int SELECTION_COUNT = 1;

            string path = Path.Combine(TestDirectory, "bonusSelection.json");
            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{SOURCE_CATEGORY}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : '{TRAIT}', 
                                'weight' : 1,
                                'bonus_selection' : {{ 'trait_category_name' : '{TARGET_CATEGORY}', 'selections' : {SELECTION_COUNT} }}
                            }}
                        ]
                    }},
                    {{
                        'name' : '{TARGET_CATEGORY}',
                        'selections': 0,
                        'traits' : [
                            {{ 
                                'name' : 'Fuzzy', 
                                'weight' : 1
                            }}
                        ]
                    }}
                ]
            }}";
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            TraitSchema schema = parser.Parse(path);

            IReadOnlyList<TraitCategory> categories = schema.GetTraitCategories();
            TraitCategory category = ListUtil.Find(categories, category => category.Name == SOURCE_CATEGORY);
            Trait trait = category.GetTrait(TRAIT);
            BonusSelection bonusSelection = trait.BonusSelection;

            Assert.IsNotNull(bonusSelection, "BonusSelection is null");
            Assert.AreEqual(TARGET_CATEGORY, bonusSelection.TraitCategory.Name, "BonusSelection category is wrong");
            Assert.AreEqual(SELECTION_COUNT, bonusSelection.SelectionCount, "BonusSelection selection count is wrong");

            File.Delete(path);
        }

        [TestMethod]
        public void RequirementTrait()
        {
            const string REQUIREMENT_CATEGORY = "Colour";
            const string REQUIREMENT_TRAIT = "Blue";

            string path = Path.Combine(TestDirectory, "replacement.json");
            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : 'Animal',
                        'selections': 1,
                        'requirements' : {{
                            'category_name' : '{REQUIREMENT_CATEGORY}',
				            'trait_name' : '{REQUIREMENT_TRAIT}'
			            }},
                        'traits' : [
                            {{ 
                                'name' : 'Bear', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : '{REQUIREMENT_CATEGORY}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : '{REQUIREMENT_TRAIT}', 
                                'weight' : 1
                            }}
                        ]
                    }}
                ]
            }}";
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);
            TraitSchema schema = parser.Parse(path);

            Assert.IsNotNull(schema, "Schema is null");
            TraitCategory category = ListUtil.Find(schema.GetTraitCategories(), category => category.Name == REQUIREMENT_CATEGORY);
            //TODO: Interrogate category requirement object.

            File.Delete(path);
        }

        //RequirementTrait
        //RequirementOr
        //RequirementAnd
        //RequirementNone
        //RequirementNested

        [TestMethod]
        public void RequirementNoOperands()
        {
            string path = Path.Combine(TestDirectory, "replacement.json");
            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : 'Animal',
                        'selections': 1,
                        'requirements' : {{
                            'operator' : 'Any'
			            }},
                        'traits' : [
                            {{ 
                                'name' : 'Bear', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : 'Colour',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : 'Blue', 
                                'weight' : 1
                            }}
                        ]
                    }}
                ]
            }}";
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch(JsonFormatException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Failed to throw JsonFormatException");

            File.Delete(path);
        }

        [TestMethod]
        public void RequirementEmptyOperand()
        {
            string path = Path.Combine(TestDirectory, "replacement.json");
            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : 'Animal',
                        'selections': 1,
                        'requirements' : {{
                            'operator' : 'Any',
				            'operands' : []
			            }},
                        'traits' : [
                            {{ 
                                'name' : 'Bear', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : 'Colour',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : 'Blue', 
                                'weight' : 1
                            }}
                        ]
                    }}
                ]
            }}";
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (JsonFormatException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Failed to throw JsonFormatException");

            File.Delete(path);
        }

        [TestMethod]
        public void RequirementNoOperator()
        {
            string path = Path.Combine(TestDirectory, "replacement.json");
            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : 'Animal',
                        'selections': 1,
                        'requirements' : {{
				            'operands' : [
                                {{'category_name': 'Quirk', 'trait_name' : 'Fashionable'}}
                            ]
			            }},
                        'traits' : [
                            {{ 
                                'name' : 'Bear', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : 'Colour',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : 'Blue', 
                                'weight' : 1
                            }}
                        ]
                    }}
                ]
            }}";
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (JsonFormatException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Failed to throw JsonFormatException");

            File.Delete(path);
        }

        [TestMethod]
        public void RequirementPresentButEmpty()
        {
            string path = Path.Combine(TestDirectory, "replacement.json");
            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : 'Animal',
                        'selections': 1,
                        'requirements' : {{}},
                        'traits' : [
                            {{ 
                                'name' : 'Bear', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : 'Colour',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : 'Blue', 
                                'weight' : 1
                            }}
                        ]
                    }}
                ]
            }}";
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (JsonFormatException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Failed to throw JsonFormatException");

            File.Delete(path);
        }

        [TestMethod]
        public void RequirementTraitIdNotFoundException()
        {
            const string REQUIREMENT_CATEGORY = "Animal";
            const string NOT_FOUND_CATEGORY = "Colour";
            const string NOT_FOUND_TRAIT = "Blue";

            string path = Path.Combine(TestDirectory, "replacement.json");
            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{REQUIREMENT_CATEGORY}',
                        'selections': 1,
                        'requirements' : {{
                            'category_name' : '{NOT_FOUND_CATEGORY}',
				            'trait_name' : '{NOT_FOUND_TRAIT}'
			            }},
                        'traits' : [
                            {{ 
                                'name' : 'Bear', 
                                'weight' : 1
                            }}
                        ]
                    }}
                ]
            }}";
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (RequirementTraitIdNotFoundException exception)
            {
                Assert.AreEqual(REQUIREMENT_CATEGORY, exception.RequirementCategory, "Wrong requirement category");
                Assert.AreEqual(NOT_FOUND_CATEGORY, exception.TraitIdNotFound.CategoryName, "Wrong category was not found");
                Assert.AreEqual(NOT_FOUND_TRAIT, exception.TraitIdNotFound.TraitName, "Wrong trait was not found");

                threwException = true;
            }

            Assert.IsTrue(threwException, "Failed to throw RequirementTraitIdNotFoundException");

            File.Delete(path);
        }

        [TestMethod]
        public void RequirementUnknownLogicalOperatorException()
        {
            const string OPERATOR = "BitShift";

            string path = Path.Combine(TestDirectory, "replacement.json");
            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : 'Animal',
                        'selections': 1,
                        'requirements' : {{
                            'operator' : '{OPERATOR}',
				            'operands' : [
                                {{'category_name': 'Colour', 'trait_name' : 'Blue'}}
                            ]
			            }},
                        'traits' : [
                            {{ 
                                'name' : 'Bear', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : 'Colour',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : 'Blue', 
                                'weight' : 1
                            }}
                        ]
                    }}
                ]
            }}";
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (JsonFormatException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Failed to throw JsonFormatException");

            File.Delete(path);
        }

        [TestMethod]
        public void RequirementUnknownLogicalOperatorExceptionWithoutSchema()
        {
            const string OPERATOR = "BitShift";

            string path = Path.Combine(TestDirectory, "replacement.json");
            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : 'Animal',
                        'selections': 1,
                        'requirements' : {{
                            'operator' : '{OPERATOR}',
				            'operands' : [
                                {{'category_name': 'Colour', 'trait_name' : 'Blue'}}
                            ]
			            }},
                        'traits' : [
                            {{ 
                                'name' : 'Bear', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : 'Colour',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : 'Blue', 
                                'weight' : 1
                            }}
                        ]
                    }}
                ]
            }}";
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(schemaPath: null);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (UnknownLogicalOperatorException exception)
            {
                Assert.AreEqual(OPERATOR, exception.OperatorName, "Wrong unknown operator");

                threwException = true;
            }

            Assert.IsTrue(threwException, "Failed to throw UnknownLogicalOperatorException");

            File.Delete(path);
        }

        [TestMethod]
        public void RequirementSelfRequiringCategoryException()
        {
            const string CATEGORY = "Animal";
            const string TRAIT = "Bear";

            string path = Path.Combine(TestDirectory, "replacement.json");
            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{CATEGORY}',
                        'selections': 1,
                        'requirements' : {{
                            'category_name' : '{CATEGORY}',
				            'trait_name' : '{TRAIT}'
			            }},
                        'traits' : [
                            {{ 
                                'name' : '{TRAIT}', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : 'Colour',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : 'Blue', 
                                'weight' : 1
                            }}
                        ]
                    }}
                ]
            }}";
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (SelfRequiringCategoryException exception)
            {
                Assert.AreEqual(CATEGORY, exception.Category, "Wrong category in SelfRequiringCategoryException");

                threwException = true;
            }

            Assert.IsTrue(threwException, "Failed to throw SelfRequiringCategoryException");

            File.Delete(path);
        }

        //Requirement chain tests.

        private readonly MockRandom m_random = new MockRandom();
    }
}
