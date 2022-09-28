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
using System.IO;

namespace Tests.JsonConfigurationParserTests
{
    [TestClass]
    public class RequirementJsonConfigurationParserTests
    {
        const string SCHEMA_PATH = "ConfigurationSchema.json";

        [TestMethod]
        public void RequirementTrait()
        {
            const string GUARDED_CATEGORY = "Animal";
            const string REQUIREMENT_CATEGORY = "Colour";
            const string REQUIREMENT_TRAIT = "Blue";

            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{GUARDED_CATEGORY}',
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

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);
            TraitSchema schema = parser.Parse(text);

            Assert.IsNotNull(schema, "Schema is null");
            
            TraitCategory guardedCategory = ListUtil.Find(schema.GetTraitCategories(), category => category.Name == GUARDED_CATEGORY);
            HashSet<string> dependentCategoryNames = guardedCategory.DependentCategoryNames();
            Assert.AreEqual(1, dependentCategoryNames.Count, "Wrong number of dependencies");
            foreach (string dependency in dependentCategoryNames)
            {
                Assert.AreEqual(REQUIREMENT_CATEGORY, dependency, "Wrong dependency");
            }

            TraitId requiredTraitId = new TraitId(REQUIREMENT_CATEGORY, REQUIREMENT_TRAIT);
            Npc npcWithTrait = new Npc();
            npcWithTrait.Add(REQUIREMENT_CATEGORY, new Npc.Trait[] { new Npc.Trait(REQUIREMENT_TRAIT, isHidden: false) });
            Assert.IsTrue(guardedCategory.IsUnlockedFor(npcWithTrait), "Category is incorrectly locked for npc with required trait");
            
            Npc npcWithoutTrait = new Npc();
            Assert.IsFalse(guardedCategory.IsUnlockedFor(npcWithoutTrait), "Category is incorrectly unlocked for npc without required trait");
        }

        [TestMethod]
        public void RequirementAny()
        {
            const string GUARDED_CATEGORY = "Animal";
            const string REQUIREMENT_CATEGORY0 = "Colour";
            const string REQUIREMENT_TRAIT0 = "Blue";
            const string REQUIREMENT_CATEGORY1 = "Hair";
            const string REQUIREMENT_TRAIT1 = "Shaggy";

            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{GUARDED_CATEGORY}',
                        'selections': 1,
                        'requirements' : {{
                            'operator' : 'Any',
                            'operands' : [
                                {{
                                    'category_name' : '{REQUIREMENT_CATEGORY0}',
				                    'trait_name' : '{REQUIREMENT_TRAIT0}'
                                }},
                                {{
                                    'category_name' : '{REQUIREMENT_CATEGORY1}',
				                    'trait_name' : '{REQUIREMENT_TRAIT1}'
                                }}
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
                        'name' : '{REQUIREMENT_CATEGORY0}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : '{REQUIREMENT_TRAIT0}', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : '{REQUIREMENT_CATEGORY1}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : '{REQUIREMENT_TRAIT1}', 
                                'weight' : 1
                            }}
                        ]
                    }}
                ]
            }}";

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);
            TraitSchema schema = parser.Parse(text);

            Assert.IsNotNull(schema, "Schema is null");

            TraitCategory guardedCategory = ListUtil.Find(schema.GetTraitCategories(), category => category.Name == GUARDED_CATEGORY);
            HashSet<string> dependentCategoryNames = guardedCategory.DependentCategoryNames();
            Assert.AreEqual(2, dependentCategoryNames.Count, "Wrong number of dependencies");
            SortedList<string, string> alphabeticalCategoryDependencies = new SortedList<string, string>();
            foreach (string dep in dependentCategoryNames)
            {
                alphabeticalCategoryDependencies.Add(dep, dep);
            }
            Assert.AreEqual(REQUIREMENT_CATEGORY0, alphabeticalCategoryDependencies.Values[0], "Wrong dependency");
            Assert.AreEqual(REQUIREMENT_CATEGORY1, alphabeticalCategoryDependencies.Values[1], "Wrong dependency");

            TraitId requiredTraitId0 = new TraitId(REQUIREMENT_CATEGORY0, REQUIREMENT_TRAIT0);
            TraitId requiredTraitId1 = new TraitId(REQUIREMENT_CATEGORY1, REQUIREMENT_TRAIT1);

            Npc npcWithTrait0 = new Npc();
            npcWithTrait0.Add(REQUIREMENT_CATEGORY0, new Npc.Trait[] { new Npc.Trait(REQUIREMENT_TRAIT0, isHidden: false) });
            Assert.IsTrue(guardedCategory.IsUnlockedFor(npcWithTrait0), "Category is incorrectly locked for npc with required trait");

            Npc npcWithTrait1 = new Npc();
            npcWithTrait1.Add(REQUIREMENT_CATEGORY1, new Npc.Trait[] { new Npc.Trait(REQUIREMENT_TRAIT1, isHidden: false) });
            Assert.IsTrue(guardedCategory.IsUnlockedFor(npcWithTrait1), "Category is incorrectly locked for npc with required trait");

            Npc npcWithBothTraits = new Npc();
            npcWithBothTraits.Add(REQUIREMENT_CATEGORY0, new Npc.Trait[] { new Npc.Trait(REQUIREMENT_TRAIT0, isHidden: false) });
            npcWithBothTraits.Add(REQUIREMENT_CATEGORY1, new Npc.Trait[] { new Npc.Trait(REQUIREMENT_TRAIT1, isHidden: false) });
            Assert.IsTrue(guardedCategory.IsUnlockedFor(npcWithBothTraits), "Category is incorrectly locked for npc with required trait");

            Npc npcWithoutAnyRequiredTraits = new Npc();
            Assert.IsFalse(guardedCategory.IsUnlockedFor(npcWithoutAnyRequiredTraits), 
                "Category is incorrectly unlocked for npc without required trait");
        }

        [TestMethod]
        public void RequirementAll()
        {
            const string GUARDED_CATEGORY = "Animal";
            const string REQUIREMENT_CATEGORY0 = "Colour";
            const string REQUIREMENT_TRAIT0 = "Blue";
            const string REQUIREMENT_CATEGORY1 = "Hair";
            const string REQUIREMENT_TRAIT1 = "Shaggy";

            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{GUARDED_CATEGORY}',
                        'selections': 1,
                        'requirements' : {{
                            'operator' : 'All',
                            'operands' : [
                                {{
                                    'category_name' : '{REQUIREMENT_CATEGORY0}',
				                    'trait_name' : '{REQUIREMENT_TRAIT0}'
                                }},
                                {{
                                    'category_name' : '{REQUIREMENT_CATEGORY1}',
				                    'trait_name' : '{REQUIREMENT_TRAIT1}'
                                }}
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
                        'name' : '{REQUIREMENT_CATEGORY0}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : '{REQUIREMENT_TRAIT0}', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : '{REQUIREMENT_CATEGORY1}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : '{REQUIREMENT_TRAIT1}', 
                                'weight' : 1
                            }}
                        ]
                    }}
                ]
            }}";

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);
            TraitSchema schema = parser.Parse(text);

            Assert.IsNotNull(schema, "Schema is null");

            TraitCategory guardedCategory = ListUtil.Find(schema.GetTraitCategories(), category => category.Name == GUARDED_CATEGORY);
            HashSet<string> dependentCategoryNames = guardedCategory.DependentCategoryNames();
            Assert.AreEqual(2, dependentCategoryNames.Count, "Wrong number of dependencies");
            SortedList<string, string> alphabeticalCategoryDependencies = new SortedList<string, string>();
            foreach (string dep in dependentCategoryNames)
            {
                alphabeticalCategoryDependencies.Add(dep, dep);
            }
            Assert.AreEqual(REQUIREMENT_CATEGORY0, alphabeticalCategoryDependencies.Values[0], "Wrong dependency");
            Assert.AreEqual(REQUIREMENT_CATEGORY1, alphabeticalCategoryDependencies.Values[1], "Wrong dependency");

            TraitId requiredTraitId0 = new TraitId(REQUIREMENT_CATEGORY0, REQUIREMENT_TRAIT0);
            TraitId requiredTraitId1 = new TraitId(REQUIREMENT_CATEGORY1, REQUIREMENT_TRAIT1);

            Npc npcWithTrait0 = new Npc();
            npcWithTrait0.Add(REQUIREMENT_CATEGORY0, new Npc.Trait[] { new Npc.Trait(REQUIREMENT_TRAIT0, isHidden: false) });
            Assert.IsFalse(guardedCategory.IsUnlockedFor(npcWithTrait0), "Category is incorrectly unlocked for npc without required trait");

            Npc npcWithTrait1 = new Npc();
            npcWithTrait1.Add(REQUIREMENT_CATEGORY1, new Npc.Trait[] { new Npc.Trait(REQUIREMENT_TRAIT1, isHidden: false) });
            Assert.IsFalse(guardedCategory.IsUnlockedFor(npcWithTrait1), "Category is incorrectly unlocked for npc without required trait");

            Npc npcWithBothTraits = new Npc();
            npcWithBothTraits.Add(REQUIREMENT_CATEGORY0, new Npc.Trait[] { new Npc.Trait(REQUIREMENT_TRAIT0, isHidden: false) });
            npcWithBothTraits.Add(REQUIREMENT_CATEGORY1, new Npc.Trait[] { new Npc.Trait(REQUIREMENT_TRAIT1, isHidden: false) });
            Assert.IsTrue(guardedCategory.IsUnlockedFor(npcWithBothTraits), "Category is incorrectly locked for npc with required trait");

            Npc npcWithoutAnyRequiredTraits = new Npc();
            Assert.IsFalse(guardedCategory.IsUnlockedFor(npcWithoutAnyRequiredTraits),
                "Category is incorrectly unlocked for npc without required trait");
        }

        [TestMethod]
        public void RequirementNone()
        {
            const string GUARDED_CATEGORY = "Animal";
            const string REQUIREMENT_CATEGORY0 = "Colour";
            const string REQUIREMENT_TRAIT0 = "Blue";
            const string REQUIREMENT_CATEGORY1 = "Hair";
            const string REQUIREMENT_TRAIT1 = "Shaggy";

            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{GUARDED_CATEGORY}',
                        'selections': 1,
                        'requirements' : {{
                            'operator' : 'None',
                            'operands' : [
                                {{
                                    'category_name' : '{REQUIREMENT_CATEGORY0}',
				                    'trait_name' : '{REQUIREMENT_TRAIT0}'
                                }},
                                {{
                                    'category_name' : '{REQUIREMENT_CATEGORY1}',
				                    'trait_name' : '{REQUIREMENT_TRAIT1}'
                                }}
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
                        'name' : '{REQUIREMENT_CATEGORY0}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : '{REQUIREMENT_TRAIT0}', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : '{REQUIREMENT_CATEGORY1}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : '{REQUIREMENT_TRAIT1}', 
                                'weight' : 1
                            }}
                        ]
                    }}
                ]
            }}";

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);
            TraitSchema schema = parser.Parse(text);

            Assert.IsNotNull(schema, "Schema is null");

            TraitCategory guardedCategory = ListUtil.Find(schema.GetTraitCategories(), category => category.Name == GUARDED_CATEGORY);
            HashSet<string> dependentCategoryNames = guardedCategory.DependentCategoryNames();
            Assert.AreEqual(2, dependentCategoryNames.Count, "Wrong number of dependencies");
            SortedList<string, string> alphabeticalCategoryDependencies = new SortedList<string, string>();
            foreach (string dep in dependentCategoryNames)
            {
                alphabeticalCategoryDependencies.Add(dep, dep);
            }
            Assert.AreEqual(REQUIREMENT_CATEGORY0, alphabeticalCategoryDependencies.Values[0], "Wrong dependency");
            Assert.AreEqual(REQUIREMENT_CATEGORY1, alphabeticalCategoryDependencies.Values[1], "Wrong dependency");

            TraitId requiredTraitId0 = new TraitId(REQUIREMENT_CATEGORY0, REQUIREMENT_TRAIT0);
            TraitId requiredTraitId1 = new TraitId(REQUIREMENT_CATEGORY1, REQUIREMENT_TRAIT1);

            Npc npcWithTrait0 = new Npc();
            npcWithTrait0.Add(REQUIREMENT_CATEGORY0, new Npc.Trait[] { new Npc.Trait(REQUIREMENT_TRAIT0, isHidden: false) });
            Assert.IsFalse(guardedCategory.IsUnlockedFor(npcWithTrait0), "Category is incorrectly unlocked for npc with disqualifying trait");

            Npc npcWithTrait1 = new Npc();
            npcWithTrait1.Add(REQUIREMENT_CATEGORY1, new Npc.Trait[] { new Npc.Trait(REQUIREMENT_TRAIT1, isHidden: false) });
            Assert.IsFalse(guardedCategory.IsUnlockedFor(npcWithTrait1), "Category is incorrectly unlocked for npc with disqualifying trait");

            Npc npcWithBothTraits = new Npc();
            npcWithBothTraits.Add(REQUIREMENT_CATEGORY0, new Npc.Trait[] { new Npc.Trait(REQUIREMENT_TRAIT0, isHidden: false) });
            npcWithBothTraits.Add(REQUIREMENT_CATEGORY1, new Npc.Trait[] { new Npc.Trait(REQUIREMENT_TRAIT1, isHidden: false) });
            Assert.IsFalse(guardedCategory.IsUnlockedFor(npcWithBothTraits), "Category is incorrectly unlocked for npc with disqualifying traits");

            Npc npcWithoutAnyRequiredTraits = new Npc();
            Assert.IsTrue(guardedCategory.IsUnlockedFor(npcWithoutAnyRequiredTraits),
                "Category is incorrectly locked for npc without disqualifying traits");
        }

        [TestMethod]
        public void RequirementNested()
        {
            const string GUARDED_CATEGORY = "Animal";
            const string REQUIREMENT_CATEGORY = "Colour";
            const string REQUIREMENT_TRAIT = "Blue";
            const string DISQUALIFYING_CATEGORY = "Hair";
            const string DISQUALIFYING_TRAIT = "Shaggy";

            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{GUARDED_CATEGORY}',
                        'selections': 1,
                        'requirements' : {{
                            'operator' : 'All',
                            'operands' : [
                                {{
                                    'category_name' : '{REQUIREMENT_CATEGORY}',
				                    'trait_name' : '{REQUIREMENT_TRAIT}'
                                }},
                                {{
                                    'operator' : 'None',
                                    'operands' : [
                                        {{
                                            'category_name' : '{DISQUALIFYING_CATEGORY}',
				                            'trait_name' : '{DISQUALIFYING_TRAIT}'
                                        }}
                                    ]
                                }}
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
                        'name' : '{REQUIREMENT_CATEGORY}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : '{REQUIREMENT_TRAIT}', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : '{DISQUALIFYING_CATEGORY}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : '{DISQUALIFYING_TRAIT}', 
                                'weight' : 1
                            }}
                        ]
                    }}
                ]
            }}";

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);
            TraitSchema schema = parser.Parse(text);

            Assert.IsNotNull(schema, "Schema is null");

            TraitCategory guardedCategory = ListUtil.Find(schema.GetTraitCategories(), category => category.Name == GUARDED_CATEGORY);
            HashSet<string> dependentCategoryNames = guardedCategory.DependentCategoryNames();
            Assert.AreEqual(2, dependentCategoryNames.Count, "Wrong number of dependencies");
            SortedList<string, string> alphabeticalCategoryDependencies = new SortedList<string, string>();
            foreach (string dep in dependentCategoryNames)
            {
                alphabeticalCategoryDependencies.Add(dep, dep);
            }
            Assert.AreEqual(REQUIREMENT_CATEGORY, alphabeticalCategoryDependencies.Values[0], "Wrong dependency");
            Assert.AreEqual(DISQUALIFYING_CATEGORY, alphabeticalCategoryDependencies.Values[1], "Wrong dependency");

            TraitId requiredTraitId0 = new TraitId(REQUIREMENT_CATEGORY, REQUIREMENT_TRAIT);
            TraitId requiredTraitId1 = new TraitId(DISQUALIFYING_CATEGORY, DISQUALIFYING_TRAIT);

            Npc npcWithRequiredTrait = new Npc();
            npcWithRequiredTrait.Add(REQUIREMENT_CATEGORY, new Npc.Trait[] { new Npc.Trait(REQUIREMENT_TRAIT, isHidden: false) });
            Assert.IsTrue(guardedCategory.IsUnlockedFor(npcWithRequiredTrait), "Category is incorrectly locked for npc with required trait");

            Npc npcWithDisqualifyingTrait = new Npc();
            npcWithDisqualifyingTrait.Add(DISQUALIFYING_CATEGORY, new Npc.Trait[] { new Npc.Trait(DISQUALIFYING_TRAIT, isHidden: false) });
            Assert.IsFalse(guardedCategory.IsUnlockedFor(npcWithDisqualifyingTrait), 
                "Category is incorrectly unlocked for npc with disqualifying trait");

            Npc npcWithBothRequiredAndDisqualifyingTraits = new Npc();
            npcWithBothRequiredAndDisqualifyingTraits.Add(
                REQUIREMENT_CATEGORY, new Npc.Trait[] { new Npc.Trait(REQUIREMENT_TRAIT, isHidden: false) });
            npcWithBothRequiredAndDisqualifyingTraits.Add(
                DISQUALIFYING_CATEGORY, new Npc.Trait[] { new Npc.Trait(DISQUALIFYING_TRAIT, isHidden: false) });
            Assert.IsFalse(guardedCategory.IsUnlockedFor(npcWithBothRequiredAndDisqualifyingTraits),
                "Category is incorrectly unlocked for npc with disqualifying trait");

            Npc npcWithoutAnyRequiredTraits = new Npc();
            Assert.IsFalse(guardedCategory.IsUnlockedFor(npcWithoutAnyRequiredTraits),
                "Category is incorrectly unlocked for npc without required trait");
        }

        [TestMethod]
        public void RequirementNoOperands()
        {
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

            Assert.IsTrue(threwException, "Failed to throw JsonFormatException");
        }

        [TestMethod]
        public void RequirementEmptyOperand()
        {
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

            Assert.IsTrue(threwException, "Failed to throw JsonFormatException");
        }

        [TestMethod]
        public void RequirementNoOperator()
        {
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

            Assert.IsTrue(threwException, "Failed to throw JsonFormatException");
        }

        [TestMethod]
        public void RequirementPresentButEmpty()
        {
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

            Assert.IsTrue(threwException, "Failed to throw JsonFormatException");
        }

        [TestMethod]
        public void RequirementTraitIdNotFoundException()
        {
            const string REQUIREMENT_CATEGORY = "Animal";
            const string NOT_FOUND_CATEGORY = "Colour";
            const string NOT_FOUND_TRAIT = "Blue";

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

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(text);
            }
            catch (RequirementTraitIdNotFoundException exception)
            {
                Assert.AreEqual(REQUIREMENT_CATEGORY, exception.RequirementCategory, "Wrong requirement category");
                Assert.AreEqual(NOT_FOUND_CATEGORY, exception.TraitIdNotFound.CategoryName, "Wrong category was not found");
                Assert.AreEqual(NOT_FOUND_TRAIT, exception.TraitIdNotFound.TraitName, "Wrong trait was not found");

                threwException = true;
            }

            Assert.IsTrue(threwException, "Failed to throw RequirementTraitIdNotFoundException");
        }

        [TestMethod]
        public void RequirementUnknownLogicalOperatorException()
        {
            const string OPERATOR = "BitShift";

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

            Assert.IsTrue(threwException, "Failed to throw JsonFormatException");
        }

        [TestMethod]
        public void RequirementUnknownLogicalOperatorExceptionWithoutSchema()
        {
            const string OPERATOR = "BitShift";

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

            JsonConfigurationParser parser = new JsonConfigurationParser(schemaPath: null);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(text);
            }
            catch (UnknownLogicalOperatorException exception)
            {
                Assert.AreEqual(OPERATOR, exception.OperatorName, "Wrong unknown operator");

                threwException = true;
            }

            Assert.IsTrue(threwException, "Failed to throw UnknownLogicalOperatorException");
        }

        [TestMethod]
        public void RequirementSelfRequiringCategoryException()
        {
            const string CATEGORY = "Animal";
            const string TRAIT = "Bear";

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

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(text);
            }
            catch (SelfRequiringCategoryException exception)
            {
                Assert.AreEqual(CATEGORY, exception.Category, "Wrong category in SelfRequiringCategoryException");

                threwException = true;
            }

            Assert.IsTrue(threwException, "Failed to throw SelfRequiringCategoryException");
        }

        [TestMethod]
        public void RequirementCircularRequirementsException()
        {
            const string CATEGORY0 = "Animal";
            const string TRAIT0 = "Bear";
            const string CATEGORY1 = "Colour";
            const string TRAIT1 = "Blue";

            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{CATEGORY0}',
                        'selections': 1,
                        'requirements' : {{
                            'category_name' : '{CATEGORY1}',
				            'trait_name' : '{TRAIT1}'
			            }},
                        'traits' : [
                            {{ 
                                'name' : '{TRAIT0}', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : '{CATEGORY1}',
                        'selections': 1,
                        'requirements' : {{
                            'category_name' : '{CATEGORY0}',
				            'trait_name' : '{TRAIT0}'
			            }},
                        'traits' : [
                            {{ 
                                'name' : '{TRAIT1}', 
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
            catch (CircularRequirementsException exception)
            {
                Assert.AreEqual(2, exception.Cycle.Count, "Wrong number of links in the CircularRequirementsException");
                foreach (TraitSchema.Dependency cycle in exception.Cycle)
                {
                    if (cycle.DependentCategory == CATEGORY0)
                    {
                        Assert.AreEqual(TraitSchema.Dependency.Type.Requirement, cycle.DependencyType, "Wrong dependency type");
                        Assert.AreEqual(CATEGORY1, cycle.OriginalCategory, "Wrong DependentCategory");
                    }
                    else
                    {
                        Assert.AreEqual(CATEGORY1, cycle.DependentCategory, "Wrong OriginalCategory");
                        Assert.AreEqual(TraitSchema.Dependency.Type.Requirement, cycle.DependencyType, "Wrong dependency type");
                        Assert.AreEqual(CATEGORY0, cycle.OriginalCategory, "Wrong DependentCategory");
                    }
                }

                threwException = true;
            }

            Assert.IsTrue(threwException, "Failed to throw CircularRequirementsException");
        }

        [TestMethod]
        public void RequirementBonusSelectionCircularRequirementsException()
        {
            const string CATEGORY0 = "Animal";
            const string TRAIT0 = "Bear";
            const string CATEGORY1 = "Building";
            const string TRAIT1 = "School";
            const string CATEGORY2 = "Colour";
            const string TRAIT2 = "Blue";

            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{CATEGORY0}',
                        'selections': 1,
                        'requirements' : {{
                            'category_name' : '{CATEGORY2}',
				            'trait_name' : '{TRAIT2}'
			            }},
                        'traits' : [
                            {{ 
                                'name' : '{TRAIT0}', 
                                'weight' : 1,
                                'bonus_selection' : {{ 'trait_category_name' : '{CATEGORY1}', 'selections' : 1 }}
                            }}
                        ]
                    }},
                    {{
                        'name' : '{CATEGORY1}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : '{TRAIT1}', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : '{CATEGORY2}',
                        'selections': 1,
                        'requirements' : {{
                            'category_name' : '{CATEGORY1}',
				            'trait_name' : '{TRAIT1}'
			            }},
                        'traits' : [
                            {{ 
                                'name' : '{TRAIT2}', 
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
            catch (CircularRequirementsException exception)
            {
                Assert.AreEqual(3, exception.Cycle.Count, "Wrong number of links in the CircularRequirementsException");
                foreach (TraitSchema.Dependency cycle in exception.Cycle)
                {
                    if (cycle.DependentCategory == CATEGORY0)
                    {
                        Assert.AreEqual(TraitSchema.Dependency.Type.Requirement, cycle.DependencyType, "Wrong dependency type");
                        Assert.AreEqual(CATEGORY2, cycle.OriginalCategory, "Wrong DependentCategory");
                    }
                    else if (cycle.DependentCategory == CATEGORY1)
                    {
                        Assert.AreEqual(TraitSchema.Dependency.Type.BonusSelection, cycle.DependencyType, "Wrong dependency type");
                        Assert.AreEqual(CATEGORY0, cycle.OriginalCategory, "Wrong DependentCategory");
                    }
                    else
                    {
                        Assert.AreEqual(CATEGORY2, cycle.DependentCategory, "Wrong OriginalCategory");
                        Assert.AreEqual(TraitSchema.Dependency.Type.Requirement, cycle.DependencyType, "Wrong dependency type");
                        Assert.AreEqual(CATEGORY1, cycle.OriginalCategory, "Wrong DependentCategory");
                    }
                }

                threwException = true;
            }

            Assert.IsTrue(threwException, "Failed to throw CircularRequirementsException");
        }

        [TestMethod]
        public void RequirementChainedBonusSelectionCircularRequirementsException()
        {
            const string CATEGORY0 = "Animal";
            const string TRAIT0 = "Bear";
            const string CATEGORY1 = "Building";
            const string TRAIT1 = "School";
            const string CATEGORY2 = "Colour";
            const string TRAIT2 = "Blue";
            const string CATEGORY3 = "Demeanor";
            const string TRAIT3 = "Arrogant";

            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{CATEGORY0}',
                        'selections': 1,
                        'requirements' : {{
                            'category_name' : '{CATEGORY3}',
				            'trait_name' : '{TRAIT3}'
			            }},
                        'traits' : [
                            {{ 
                                'name' : '{TRAIT0}', 
                                'weight' : 1,
                                'bonus_selection' : {{ 'trait_category_name' : '{CATEGORY1}', 'selections' : 1 }}
                            }}
                        ]
                    }},
                    {{
                        'name' : '{CATEGORY1}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : '{TRAIT1}', 
                                'weight' : 1,
                                'bonus_selection' : {{ 'trait_category_name' : '{CATEGORY2}', 'selections' : 1 }}
                            }}
                        ]
                    }},
                    {{
                        'name' : '{CATEGORY2}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : '{TRAIT2}', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : '{CATEGORY3}',
                        'selections': 1,
                        'requirements' : {{
                            'category_name' : '{CATEGORY2}',
				            'trait_name' : '{TRAIT2}'
			            }},
                        'traits' : [
                            {{ 
                                'name' : '{TRAIT3}', 
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
            catch (CircularRequirementsException exception)
            {
                Assert.AreEqual(4, exception.Cycle.Count, "Wrong number of links in the CircularRequirementsException");
                foreach (TraitSchema.Dependency cycle in exception.Cycle)
                {
                    if (cycle.DependentCategory == CATEGORY0)
                    {
                        Assert.AreEqual(TraitSchema.Dependency.Type.Requirement, cycle.DependencyType, "Wrong dependency type");
                        Assert.AreEqual(CATEGORY3, cycle.OriginalCategory, "Wrong DependentCategory");
                    }
                    else if (cycle.DependentCategory == CATEGORY1)
                    {
                        Assert.AreEqual(TraitSchema.Dependency.Type.BonusSelection, cycle.DependencyType, "Wrong dependency type");
                        Assert.AreEqual(CATEGORY0, cycle.OriginalCategory, "Wrong DependentCategory");
                    }
                    else if (cycle.DependentCategory == CATEGORY2)
                    {
                        Assert.AreEqual(TraitSchema.Dependency.Type.BonusSelection, cycle.DependencyType, "Wrong dependency type");
                        Assert.AreEqual(CATEGORY1, cycle.OriginalCategory, "Wrong DependentCategory");
                    }
                    else
                    {
                        Assert.AreEqual(CATEGORY3, cycle.DependentCategory, "Wrong OriginalCategory");
                        Assert.AreEqual(TraitSchema.Dependency.Type.Requirement, cycle.DependencyType, "Wrong dependency type");
                        Assert.AreEqual(CATEGORY2, cycle.OriginalCategory, "Wrong DependentCategory");
                    }
                }

                threwException = true;
            }

            Assert.IsTrue(threwException, "Failed to throw CircularRequirementsException");
        }

        [TestMethod]
        public void RequirementChainedBonusSelectionAndRequirementCircularRequirementsException()
        {
            const string CATEGORY_A = "Animal";
            const string TRAIT_A = "Bear";
            const string CATEGORY_B = "Building";
            const string TRAIT_B = "School";
            const string CATEGORY_C = "Colour";
            const string TRAIT_C = "Blue";
            const string CATEGORY_D = "Demeanor";
            const string TRAIT_D = "Arrogant";
            const string CATEGORY_E = "Energy";
            const string TRAIT_E = "Nuclear";
            const string CATEGORY_F = "Fear";
            const string TRAIT_F = "Spiders";

            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{CATEGORY_A}',
                        'selections': 1,
                        'requirements' : {{
                            'category_name' : '{CATEGORY_F}',
				            'trait_name' : '{TRAIT_F}'
			            }},
                        'traits' : [
                            {{ 
                                'name' : '{TRAIT_A}', 
                                'weight' : 1,
                                'bonus_selection' : {{ 'trait_category_name' : '{CATEGORY_B}', 'selections' : 1 }}
                            }}
                        ]
                    }},
                    {{
                        'name' : '{CATEGORY_B}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : '{TRAIT_B}', 
                                'weight' : 1,
                                'bonus_selection' : {{ 'trait_category_name' : '{CATEGORY_C}', 'selections' : 1 }}
                            }}
                        ]
                    }},
                    {{
                        'name' : '{CATEGORY_C}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : '{TRAIT_C}', 
                                'weight' : 1
                            }}
                        ]
                    }},
                    {{
                        'name' : '{CATEGORY_D}',
                        'selections': 1,
                        'requirements' : {{
                            'category_name' : '{CATEGORY_C}',
				            'trait_name' : '{TRAIT_C}'
			            }},
                        'traits' : [
                            {{ 
                                'name' : '{TRAIT_D}', 
                                'weight' : 1,
                                'bonus_selection' : {{ 'trait_category_name' : '{CATEGORY_E}', 'selections' : 1 }}
                            }}
                        ]
                    }},
                    {{
                        'name' : '{CATEGORY_E}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : '{TRAIT_E}', 
                                'weight' : 1,
                                'bonus_selection' : {{ 'trait_category_name' : '{CATEGORY_F}', 'selections' : 1 }}
                            }}
                        ]
                    }},
                    {{
                        'name' : '{CATEGORY_F}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : '{TRAIT_F}', 
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
            catch (CircularRequirementsException exception)
            {
                Assert.AreEqual(6, exception.Cycle.Count, "Wrong number of links in the CircularRequirementsException");
                foreach (TraitSchema.Dependency cycle in exception.Cycle)
                {
                    if (cycle.DependentCategory == CATEGORY_A)
                    {
                        Assert.AreEqual(TraitSchema.Dependency.Type.Requirement, cycle.DependencyType, "Wrong dependency type");
                        Assert.AreEqual(CATEGORY_F, cycle.OriginalCategory, "Wrong DependentCategory");
                    }
                    else if (cycle.DependentCategory == CATEGORY_B)
                    {
                        Assert.AreEqual(TraitSchema.Dependency.Type.BonusSelection, cycle.DependencyType, "Wrong dependency type");
                        Assert.AreEqual(CATEGORY_A, cycle.OriginalCategory, "Wrong DependentCategory");
                    }
                    else if (cycle.DependentCategory == CATEGORY_C)
                    {
                        Assert.AreEqual(TraitSchema.Dependency.Type.BonusSelection, cycle.DependencyType, "Wrong dependency type");
                        Assert.AreEqual(CATEGORY_B, cycle.OriginalCategory, "Wrong DependentCategory");
                    }
                    else if (cycle.DependentCategory == CATEGORY_D)
                    {
                        Assert.AreEqual(TraitSchema.Dependency.Type.Requirement, cycle.DependencyType, "Wrong dependency type");
                        Assert.AreEqual(CATEGORY_C, cycle.OriginalCategory, "Wrong DependentCategory");
                    }
                    else if (cycle.DependentCategory == CATEGORY_E)
                    {
                        Assert.AreEqual(TraitSchema.Dependency.Type.BonusSelection, cycle.DependencyType, "Wrong dependency type");
                        Assert.AreEqual(CATEGORY_D, cycle.OriginalCategory, "Wrong DependentCategory");
                    }
                    else
                    {
                        Assert.AreEqual(CATEGORY_F, cycle.DependentCategory, "Wrong OriginalCategory");
                        Assert.AreEqual(TraitSchema.Dependency.Type.BonusSelection, cycle.DependencyType, "Wrong dependency type");
                        Assert.AreEqual(CATEGORY_E, cycle.OriginalCategory, "Wrong DependentCategory");
                    }
                }

                threwException = true;
            }

            Assert.IsTrue(threwException, "Failed to throw CircularRequirementsException");
        }
    }
}
