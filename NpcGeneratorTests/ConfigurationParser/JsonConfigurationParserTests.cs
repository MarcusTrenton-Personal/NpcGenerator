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
using System.IO;

namespace Tests
{
    [TestClass]
    public class JsonConfigurationParserTests : FileCreatingTests
    {
        const string schemaPath = "ConfigurationSchema.json";

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

            JsonConfigurationParser parser = new JsonConfigurationParser(schemaPath);
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

            JsonConfigurationParser parser = new JsonConfigurationParser(schemaPath);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (Exception)
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

            JsonConfigurationParser parser = new JsonConfigurationParser(schemaPath);
            TraitSchema schema = parser.Parse(path);
            Assert.IsNotNull(schema, "Failed to generate a schema from the valid text");

            IReadOnlyList<TraitCategory> categories = schema.GetTraitCategories();
            Assert.IsNotNull(categories, "Schema categories is null, which should be impossible");
            Assert.AreEqual(2, categories.Count, "Schema has incorrect number of TraitCategories");
            TraitCategory firstCategory = categories[0];
            Assert.IsNotNull(firstCategory, "Schema has a null first TraitCategory");
            Assert.AreEqual(firstCategory.Name, CATEGORY1_TITLE, "First category doesn't have name " + CATEGORY1_TITLE);
            Assert.AreEqual(CATEGORY1_SELECTION_COUNT, firstCategory.DefaultSelectionCount, "First category has wrong SelectionCount");
            TraitChooser firstChooser = firstCategory.CreateTraitChooser();
            string[] colours = firstChooser.Choose(firstCategory.DefaultSelectionCount, out List<BonusSelection> bonusSelections1);
            Assert.AreEqual(0, bonusSelections1.Count, "Bonus selection returned where there should be none.");
            Assert.AreEqual(colours.Length, 1, "Wrong number of selections from " + CATEGORY1_TITLE);
            Assert.IsTrue(colours[0] == CATEGORY1_TRAIT1 || colours[0] == CATEGORY1_TRAIT2, CATEGORY1_TITLE + 
                " chose an invalid trait " + colours[0]);

            TraitCategory secondCategory = categories[1];
            Assert.IsNotNull(secondCategory, "Schema has a null second TraitCategory");
            Assert.AreEqual(secondCategory.Name, CATEGORY2_TITLE, "Second category doesn't have name " + CATEGORY2_TITLE);
            Assert.AreEqual(CATEGORY2_SELECTION_COUNT, secondCategory.DefaultSelectionCount, "Second category has wrong SelectionCount");
            TraitChooser secondChooser = secondCategory.CreateTraitChooser();
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

            JsonConfigurationParser parser = new JsonConfigurationParser(schemaPath);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (Exception)
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

            JsonConfigurationParser parser = new JsonConfigurationParser(schemaPath);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (Exception)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Missing traits failed to throw exception");

            File.Delete(path);
        }

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

            JsonConfigurationParser parser = new JsonConfigurationParser(schemaPath);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (Exception)
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

            JsonConfigurationParser parser = new JsonConfigurationParser(schemaPath);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (Exception)
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

            JsonConfigurationParser parser = new JsonConfigurationParser(schemaPath);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (Exception)
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

            JsonConfigurationParser parser = new JsonConfigurationParser(schemaPath);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (Exception)
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

            JsonConfigurationParser parser = new JsonConfigurationParser(schemaPath);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (Exception)
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

            JsonConfigurationParser parser = new JsonConfigurationParser(schemaPath);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (Exception)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Empty json did not throw exception");

            File.Delete(path);
        }

        [TestMethod]
        public void DuplicateCategoryNames()
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
                            }
                        ]
                    },
                    {
                        'name' : 'Colour',
                        'selections': 1,
                        'traits' : [
                            { 
                                'name' : 'Red', 
                                'weight' : 1
                            }
                        ]
                    }
                ]
            }";
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(schemaPath);

            bool throwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch(Exception)
            {
                throwException = true;
            }
            
            Assert.IsTrue(throwException, "Duplicate categories were not rejected.");

            File.Delete(path);
        }

        [TestMethod]
        public void DuplicateTraitNamesInACategory()
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
                                'name' : 'Green', 
                                'weight' : 1
                            }
                        ]
                    }
                ]
            }";
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(schemaPath);

            bool throwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (Exception)
            {
                throwException = true;
            }

            Assert.IsTrue(throwException, "Duplicate trait names in the same category were not rejected.");

            File.Delete(path);
        }

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

            JsonConfigurationParser parser = new JsonConfigurationParser(schemaPath);

            TraitSchema schema = parser.Parse(path);
            IReadOnlyList<TraitCategory> categories = schema.GetTraitCategories();
            
            Assert.AreEqual(2, categories.Count);
            Assert.IsNotNull(categories[0].GetTrait(TRAIT_NAME), "Missing expected trait in category");
            Assert.IsNotNull(categories[1].GetTrait(TRAIT_NAME), "Missing expected trait in category");

            File.Delete(path);
        }
    }
}
