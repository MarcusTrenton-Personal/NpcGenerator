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

namespace Tests.JsonConfigurationParserTests
{
    [TestClass]
    public class OutputCategoryParserTests
    {
        const string SCHEMA_PATH = "ConfigurationSchema.json";

        [TestMethod]
        public void IsolatedOutputName()
        {
            const string CATEGORY_NAME = "Colour";
            const string CATEGORY_OUTPUT_NAME = "Shade";

            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{CATEGORY_NAME}',
                        'output_name': '{CATEGORY_OUTPUT_NAME}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : 'Green', 
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
            TraitCategory firstCategory = categories[0];
            Assert.IsNotNull(firstCategory, "Schema has a null first TraitCategory");
            Assert.AreEqual(firstCategory.Name, CATEGORY_NAME, "First category doesn't have name " + CATEGORY_NAME);
            Assert.AreEqual(firstCategory.OutputName, CATEGORY_OUTPUT_NAME, "First category doesn't have output name " + CATEGORY_OUTPUT_NAME);
        }

        [TestMethod]
        public void NameIsDefaultOutputName()
        {
            const string CATEGORY_NAME = "Colour";;

            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{CATEGORY_NAME}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : 'Green', 
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
            TraitCategory firstCategory = categories[0];
            Assert.IsNotNull(firstCategory, "Schema has a null first TraitCategory");
            Assert.AreEqual(firstCategory.Name, CATEGORY_NAME, "First category doesn't have name " + CATEGORY_NAME);
            Assert.AreEqual(firstCategory.OutputName, CATEGORY_NAME, "First category doesn't have output name " + CATEGORY_NAME);
        }

        [TestMethod]
        public void SharedOutputName()
        {
            const string CATEGORY0_NAME = "Young Fame";
            const string CATEGORY1_NAME = "Old Fame";
            const string SHARED_OUTPUT_NAME = "Fame";

            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{CATEGORY0_NAME}',
                        'output_name': '{SHARED_OUTPUT_NAME}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : 'Social Media', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : '{CATEGORY1_NAME}',
                        'output_name': '{SHARED_OUTPUT_NAME}',
                        'selections': 1,
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

            IReadOnlyList<TraitCategory> categories = schema.GetTraitCategories();
            Assert.IsNotNull(categories, "Schema categories is null, which should be impossible");
            Assert.AreEqual(2, categories.Count, "Schema has incorrect number of TraitCategories");

            TraitCategory firstCategory = categories[0];
            Assert.IsNotNull(firstCategory, "Schema has a null first TraitCategory");
            Assert.AreEqual(firstCategory.Name, CATEGORY0_NAME, "First category doesn't have name " + CATEGORY0_NAME);
            Assert.AreEqual(firstCategory.OutputName, SHARED_OUTPUT_NAME, "First category doesn't have output name " + SHARED_OUTPUT_NAME);

            TraitCategory secondCategory = categories[1];
            Assert.IsNotNull(secondCategory, "Schema has a null second TraitCategory");
            Assert.AreEqual(secondCategory.Name, CATEGORY1_NAME, "First category doesn't have name " + CATEGORY0_NAME);
            Assert.AreEqual(secondCategory.OutputName, SHARED_OUTPUT_NAME, "First category doesn't have output name " + SHARED_OUTPUT_NAME);
        }

        [TestMethod]
        public void OutputNameIsAnotherCategory()
        {
            const string CATEGORY0_NAME = "Young Fame";
            const string CATEGORY0_OUTPUT_NAME_AND_CATEGORY1_NAME = "Fame";

            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{CATEGORY0_NAME}',
                        'output_name': '{CATEGORY0_OUTPUT_NAME_AND_CATEGORY1_NAME}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : 'Social Media', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : '{CATEGORY0_OUTPUT_NAME_AND_CATEGORY1_NAME}',
                        'selections': 1,
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

            IReadOnlyList<TraitCategory> categories = schema.GetTraitCategories();
            Assert.IsNotNull(categories, "Schema categories is null, which should be impossible");
            Assert.AreEqual(2, categories.Count, "Schema has incorrect number of TraitCategories");

            TraitCategory firstCategory = categories[0];
            Assert.IsNotNull(firstCategory, "Schema has a null first TraitCategory");
            Assert.AreEqual(firstCategory.Name, CATEGORY0_NAME, "First category doesn't have name " + CATEGORY0_NAME);
            Assert.AreEqual(firstCategory.OutputName, CATEGORY0_OUTPUT_NAME_AND_CATEGORY1_NAME, 
                "First category doesn't have output name " + CATEGORY0_OUTPUT_NAME_AND_CATEGORY1_NAME);

            TraitCategory secondCategory = categories[1];
            Assert.IsNotNull(secondCategory, "Schema has a null second TraitCategory");
            Assert.AreEqual(secondCategory.Name, CATEGORY0_OUTPUT_NAME_AND_CATEGORY1_NAME, 
                "First category doesn't have name " + CATEGORY0_OUTPUT_NAME_AND_CATEGORY1_NAME);
            Assert.AreEqual(secondCategory.OutputName, CATEGORY0_OUTPUT_NAME_AND_CATEGORY1_NAME, 
                "First category doesn't have output name " + CATEGORY0_OUTPUT_NAME_AND_CATEGORY1_NAME);
        }

        [TestMethod, ExpectedException(typeof(JsonFormatException))]
        public void OutputNameIsIntInsteadOfString()
        {
            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : 'Colour',
                        'output_name': 1,
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : 'Green', 
                                'weight' : 1
                            }}
                        ]
                    }}
                ]
            }}";

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);
            parser.Parse(text);
        }
    }
}
