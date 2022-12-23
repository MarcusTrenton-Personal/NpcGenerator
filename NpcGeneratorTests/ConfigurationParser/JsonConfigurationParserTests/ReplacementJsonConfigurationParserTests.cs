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
using System.Collections.Generic;

namespace Tests.JsonConfigurationParserTests
{
    [TestClass]
    public class ReplacementJsonConfigurationParserTests
    {
        const string SCHEMA_PATH = "ConfigurationSchema.json";

        [TestMethod]
        public void ReplacementsForSchemaWithReplacements()
        {
            const string REPLACEMENT_CATEGORY = "Colour";
            const string REPLACEMENT_TRAIT = "Green";

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

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);
            TraitSchema schema = parser.Parse(text);
            Assert.IsNotNull(schema, "Failed to generate a schema from the valid text");

            IReadOnlyList<ReplacementSearch> replacements = schema.GetReplacementSearches();
            Assert.AreEqual(1, replacements.Count, "Wrong number of replacements found.");
            Assert.AreEqual(REPLACEMENT_CATEGORY, replacements[0].Category.Name, "Wrong replacement category");
            Assert.AreEqual(REPLACEMENT_TRAIT, replacements[0].Trait.Name, "Wrong replacement trait");
        }

        [TestMethod]
        public void ReplacementsForSchemaWithMultipleReplacements()
        {
            const string REPLACEMENT_CATEGORY0 = "Colour";
            const string REPLACEMENT_TRAIT0 = "Green";
            const string REPLACEMENT_CATEGORY1 = "Animal";
            const string REPLACEMENT_TRAIT1 = "Bear";

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

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);
            TraitSchema schema = parser.Parse(text);
            Assert.IsNotNull(schema, "Failed to generate a schema from the valid text");

            IReadOnlyList<ReplacementSearch> replacements = schema.GetReplacementSearches();
            Assert.AreEqual(2, replacements.Count, "Wrong number of replacements found.");

            ReplacementSearch replacement0 = ListUtil.Find(replacements, replacement => replacement.Category.Name == REPLACEMENT_CATEGORY0);
            Assert.AreEqual(REPLACEMENT_CATEGORY0, replacement0.Category.Name, "Wrong replacement category");
            Assert.AreEqual(REPLACEMENT_TRAIT0, replacement0.Trait.Name, "Wrong replacement trait");

            ReplacementSearch replacement1 = ListUtil.Find(replacements, replacement => replacement.Category.Name == REPLACEMENT_CATEGORY1);
            Assert.AreEqual(REPLACEMENT_CATEGORY1, replacement1.Category.Name, "Wrong replacement category");
            Assert.AreEqual(REPLACEMENT_TRAIT1, replacement1.Trait.Name, "Wrong replacement trait");
        }

        [TestMethod]
        public void ReplacementHasMissingTrait()
        {
            const string REPLACEMENT_CATEGORY = "Colour";
            const string MISSING_REPLACEMENT_TRAIT = "Blue";

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

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(text);
            }
            catch (MissingReplacementTraitException exception)
            {
                Assert.AreEqual(new TraitId(REPLACEMENT_CATEGORY, MISSING_REPLACEMENT_TRAIT), exception.TraitId, "Wrong replacement trait");

                threwException = true;
            }

            Assert.IsTrue(threwException, "Faile to throw a MissingReplacementTraitException exception");
        }

        [TestMethod]
        public void ReplacementHasMissingCategory()
        {
            const string MISSING_REPLACEMENT_CATEGORY = "Hair";
            const string REPLACEMENT_TRAIT = "Green";

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

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(text);
            }
            catch (MissingReplacementCategoryException exception)
            {
                Assert.AreEqual(new TraitId(MISSING_REPLACEMENT_CATEGORY, REPLACEMENT_TRAIT), exception.TraitId, "Wrong replacement trait");

                threwException = true;
            }

            Assert.IsTrue(threwException, "Failed to throw MissingReplacementCategoryException");
        }

        [TestMethod, ExpectedException(typeof(JsonFormatException))]
        public void ReplacementHasNoCategory()
        {
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

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            parser.Parse(text);
        }

        [TestMethod, ExpectedException(typeof(JsonFormatException))]
        public void ReplacementHasNoTrait()
        {
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

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            parser.Parse(text);
        }

        [TestMethod, ExpectedException(typeof(JsonFormatException))]
        public void ReplacementIsEmpty()
        {
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

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            parser.Parse(text);
        }

        [TestMethod, ExpectedException(typeof(JsonFormatException))]
        public void ReplacementHasMultipleIdenticalObjects()
        {
            const string REPLACEMENT_CATEGORY = "Hair";
            const string REPLACEMENT_TRAIT = "Green";

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

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            parser.Parse(text);
        }

        [TestMethod]
        public void ReplacementHasValidSortBy()
        {
            const string REPLACEMENT_CATEGORY = "Colour";
            const string REPLACEMENT_TRAIT = "Green";
            const Sort SORT_BY = Sort.Alphabetical;

            string text = $@"{{
                'replacements' : [
                    {{
                        'category_name' : '{REPLACEMENT_CATEGORY}',
                        'trait_name' : '{REPLACEMENT_TRAIT}',
                        'sort_by' : '{SORT_BY}',
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

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);
            TraitSchema schema = parser.Parse(text);
            Assert.IsNotNull(schema, "Failed to generate a schema from the valid text");

            IReadOnlyList<ReplacementSearch> replacements = schema.GetReplacementSearches();
            Assert.AreEqual(1, replacements.Count, "Wrong number of replacements found.");
            Assert.AreEqual(REPLACEMENT_CATEGORY, replacements[0].Category.Name, "Wrong replacement category");
            Assert.AreEqual(REPLACEMENT_TRAIT, replacements[0].Trait.Name, "Wrong replacement trait");
            Assert.AreEqual(SORT_BY, replacements[0].SortCriteria, "Wrong sort criteria");
        }

        [TestMethod, ExpectedException(typeof(JsonFormatException))]
        public void ReplacementHasInvalidSortBy()
        {
            const string REPLACEMENT_CATEGORY = "Colour";
            const string REPLACEMENT_TRAIT = "Green";
            const string INVALID_SORT_BY = "Astrology";

            string text = $@"{{
                'replacements' : [
                    {{
                        'category_name' : '{REPLACEMENT_CATEGORY}',
                        'trait_name' : '{REPLACEMENT_TRAIT}',
                        'sort_by' : '{INVALID_SORT_BY}',
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

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);
            parser.Parse(text);
        }
    }
}
