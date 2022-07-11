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
using NpcGenerator;
using System;
using System.IO;

namespace Tests
{
    [TestClass]
    public class JsonConfigurationParserTests : FileCreatingTests
    {
        const string schemaPath = "../../../../../NpcGenerator/SourceAssets/ConfigurationSchema.json";

        [TestMethod]
        public void GeneratesTraitSchema()
        {
            string path = Path.Combine(TestDirectory, "colour.json");
            string text = @"{
                'trait_categories' : [
                    {
                        'name' : 'Colour','traits' : [
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
                        'name' : 'Colour','traits' : [
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
                        'name' : 'Colour','traits' : [
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
            const string CATEGORY1_TRAIT1 = "Green";
            const string CATEGORY1_TRAIT2 = "Red";
            const string CATEGORY2_TITLE = "Animal";
            const string CATEGORY2_TRAIT1 = "Gorilla";
            const string CATEGORY2_TRAIT2 = "Rhino";

            string path = Path.Combine(TestDirectory, "colourAndAnimal.json");
            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{CATEGORY1_TITLE}','traits' : [
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
                        'name' : '{CATEGORY2_TITLE}','traits' : [
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

            Assert.AreEqual(schema.TraitCategoryCount, 2, "Schema has incorrect number of TraitCategories");
            TraitCategory firstCategory = schema.GetAtIndex(0);
            Assert.IsNotNull(firstCategory, "Schema has a null first TraitCategory");
            Assert.AreEqual(firstCategory.Name, CATEGORY1_TITLE, "First category doesn't have name " + CATEGORY1_TITLE);
            string colour = firstCategory.Choose();
            Assert.IsTrue(colour == CATEGORY1_TRAIT1 || colour == CATEGORY1_TRAIT2, CATEGORY1_TITLE + " chose an invalid trait " + colour);

            TraitCategory secondCategory = schema.GetAtIndex(1);
            Assert.IsNotNull(secondCategory, "Schema has a null second TraitCategory");
            Assert.AreEqual(secondCategory.Name, CATEGORY2_TITLE, "Second category doesn't have name " + CATEGORY2_TITLE);
            string animal = secondCategory.Choose();
            Assert.IsTrue(animal == CATEGORY2_TRAIT1 || animal == CATEGORY2_TRAIT2, CATEGORY2_TITLE + " chose an invalid trait " + animal);

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
                        'name' : 'Colour','traits' : [
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
                        'name' : 'Colour','traits' : [
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
                        'name' : 'Colour','traits' : [
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
                        'name' : 'Colour','traits' : [
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
    }
}
