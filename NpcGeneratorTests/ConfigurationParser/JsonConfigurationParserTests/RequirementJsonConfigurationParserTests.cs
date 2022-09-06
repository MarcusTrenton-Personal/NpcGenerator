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
using System.IO;

namespace Tests.JsonConfigurationParserTests
{
    [TestClass]
    public class RequirementJsonConfigurationParserTests : FileCreatingTests
    {
        const string SCHEMA_PATH = "ConfigurationSchema.json";

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
            catch (JsonFormatException)
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
    }
}
