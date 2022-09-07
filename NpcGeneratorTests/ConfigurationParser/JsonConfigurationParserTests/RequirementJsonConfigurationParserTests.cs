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
    public class RequirementJsonConfigurationParserTests : FileCreatingTests
    {
        const string SCHEMA_PATH = "ConfigurationSchema.json";

        [TestMethod]
        public void RequirementTrait()
        {
            const string GUARDED_CATEGORY = "Animal";
            const string REQUIREMENT_CATEGORY = "Colour";
            const string REQUIREMENT_TRAIT = "Blue";

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + ".json");
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
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);
            TraitSchema schema = parser.Parse(path);

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
            npcWithTrait.Add(REQUIREMENT_CATEGORY, new string[] { REQUIREMENT_TRAIT });
            Assert.IsTrue(guardedCategory.IsUnlockedFor(npcWithTrait), "Category is incorrectly locked for npc with required trait");
            
            Npc npcWithoutTrait = new Npc();
            Assert.IsFalse(guardedCategory.IsUnlockedFor(npcWithoutTrait), "Category is incorrectly unlocked for npc without required trait");

            File.Delete(path);
        }

        [TestMethod]
        public void RequirementAny()
        {
            const string GUARDED_CATEGORY = "Animal";
            const string REQUIREMENT_CATEGORY0 = "Colour";
            const string REQUIREMENT_TRAIT0 = "Blue";
            const string REQUIREMENT_CATEGORY1 = "Hair";
            const string REQUIREMENT_TRAIT1 = "Shaggy";

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + ".json");
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
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);
            TraitSchema schema = parser.Parse(path);

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
            npcWithTrait0.Add(REQUIREMENT_CATEGORY0, new string[] { REQUIREMENT_TRAIT0 });
            Assert.IsTrue(guardedCategory.IsUnlockedFor(npcWithTrait0), "Category is incorrectly locked for npc with required trait");

            Npc npcWithTrait1 = new Npc();
            npcWithTrait1.Add(REQUIREMENT_CATEGORY1, new string[] { REQUIREMENT_TRAIT1 });
            Assert.IsTrue(guardedCategory.IsUnlockedFor(npcWithTrait1), "Category is incorrectly locked for npc with required trait");

            Npc npcWithBothTraits = new Npc();
            npcWithBothTraits.Add(REQUIREMENT_CATEGORY0, new string[] { REQUIREMENT_TRAIT0 });
            npcWithBothTraits.Add(REQUIREMENT_CATEGORY1, new string[] { REQUIREMENT_TRAIT1 });
            Assert.IsTrue(guardedCategory.IsUnlockedFor(npcWithBothTraits), "Category is incorrectly locked for npc with required trait");

            Npc npcWithoutAnyRequiredTraits = new Npc();
            Assert.IsFalse(guardedCategory.IsUnlockedFor(npcWithoutAnyRequiredTraits), 
                "Category is incorrectly unlocked for npc without required trait");

            File.Delete(path);
        }

        [TestMethod]
        public void RequirementAll()
        {
            const string GUARDED_CATEGORY = "Animal";
            const string REQUIREMENT_CATEGORY0 = "Colour";
            const string REQUIREMENT_TRAIT0 = "Blue";
            const string REQUIREMENT_CATEGORY1 = "Hair";
            const string REQUIREMENT_TRAIT1 = "Shaggy";

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + ".json");
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
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);
            TraitSchema schema = parser.Parse(path);

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
            npcWithTrait0.Add(REQUIREMENT_CATEGORY0, new string[] { REQUIREMENT_TRAIT0 });
            Assert.IsFalse(guardedCategory.IsUnlockedFor(npcWithTrait0), "Category is incorrectly unlocked for npc without required trait");

            Npc npcWithTrait1 = new Npc();
            npcWithTrait1.Add(REQUIREMENT_CATEGORY1, new string[] { REQUIREMENT_TRAIT1 });
            Assert.IsFalse(guardedCategory.IsUnlockedFor(npcWithTrait1), "Category is incorrectly unlocked for npc without required trait");

            Npc npcWithBothTraits = new Npc();
            npcWithBothTraits.Add(REQUIREMENT_CATEGORY0, new string[] { REQUIREMENT_TRAIT0 });
            npcWithBothTraits.Add(REQUIREMENT_CATEGORY1, new string[] { REQUIREMENT_TRAIT1 });
            Assert.IsTrue(guardedCategory.IsUnlockedFor(npcWithBothTraits), "Category is incorrectly locked for npc with required trait");

            Npc npcWithoutAnyRequiredTraits = new Npc();
            Assert.IsFalse(guardedCategory.IsUnlockedFor(npcWithoutAnyRequiredTraits),
                "Category is incorrectly unlocked for npc without required trait");

            File.Delete(path);
        }

        [TestMethod]
        public void RequirementNone()
        {
            const string GUARDED_CATEGORY = "Animal";
            const string REQUIREMENT_CATEGORY0 = "Colour";
            const string REQUIREMENT_TRAIT0 = "Blue";
            const string REQUIREMENT_CATEGORY1 = "Hair";
            const string REQUIREMENT_TRAIT1 = "Shaggy";

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + ".json");
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
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);
            TraitSchema schema = parser.Parse(path);

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
            npcWithTrait0.Add(REQUIREMENT_CATEGORY0, new string[] { REQUIREMENT_TRAIT0 });
            Assert.IsFalse(guardedCategory.IsUnlockedFor(npcWithTrait0), "Category is incorrectly unlocked for npc with disqualifying trait");

            Npc npcWithTrait1 = new Npc();
            npcWithTrait1.Add(REQUIREMENT_CATEGORY1, new string[] { REQUIREMENT_TRAIT1 });
            Assert.IsFalse(guardedCategory.IsUnlockedFor(npcWithTrait1), "Category is incorrectly unlocked for npc with disqualifying trait");

            Npc npcWithBothTraits = new Npc();
            npcWithBothTraits.Add(REQUIREMENT_CATEGORY0, new string[] { REQUIREMENT_TRAIT0 });
            npcWithBothTraits.Add(REQUIREMENT_CATEGORY1, new string[] { REQUIREMENT_TRAIT1 });
            Assert.IsFalse(guardedCategory.IsUnlockedFor(npcWithBothTraits), "Category is incorrectly unlocked for npc with disqualifying traits");

            Npc npcWithoutAnyRequiredTraits = new Npc();
            Assert.IsTrue(guardedCategory.IsUnlockedFor(npcWithoutAnyRequiredTraits),
                "Category is incorrectly locked for npc without disqualifying traits");

            File.Delete(path);
        }

        [TestMethod]
        public void RequirementNested()
        {
            const string GUARDED_CATEGORY = "Animal";
            const string REQUIREMENT_CATEGORY = "Colour";
            const string REQUIREMENT_TRAIT = "Blue";
            const string DISQUALIFYING_CATEGORY = "Hair";
            const string DISQUALIFYING_TRAIT = "Shaggy";

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + ".json");
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
            File.WriteAllText(path, text);

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);
            TraitSchema schema = parser.Parse(path);

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
            npcWithRequiredTrait.Add(REQUIREMENT_CATEGORY, new string[] { REQUIREMENT_TRAIT });
            Assert.IsTrue(guardedCategory.IsUnlockedFor(npcWithRequiredTrait), "Category is incorrectly locked for npc with required trait");

            Npc npcWithDisqualifyingTrait = new Npc();
            npcWithDisqualifyingTrait.Add(DISQUALIFYING_CATEGORY, new string[] { DISQUALIFYING_TRAIT });
            Assert.IsFalse(guardedCategory.IsUnlockedFor(npcWithDisqualifyingTrait), 
                "Category is incorrectly unlocked for npc with disqualifying trait");

            Npc npcWithBothRequiredAndDisqualifyingTraits = new Npc();
            npcWithBothRequiredAndDisqualifyingTraits.Add(REQUIREMENT_CATEGORY, new string[] { REQUIREMENT_TRAIT });
            npcWithBothRequiredAndDisqualifyingTraits.Add(DISQUALIFYING_CATEGORY, new string[] { DISQUALIFYING_TRAIT });
            Assert.IsFalse(guardedCategory.IsUnlockedFor(npcWithBothRequiredAndDisqualifyingTraits),
                "Category is incorrectly unlocked for npc with disqualifying trait");

            Npc npcWithoutAnyRequiredTraits = new Npc();
            Assert.IsFalse(guardedCategory.IsUnlockedFor(npcWithoutAnyRequiredTraits),
                "Category is incorrectly unlocked for npc without required trait");

            File.Delete(path);
        }

        [TestMethod]
        public void RequirementNoOperands()
        {
            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + ".json");
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
            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + ".json");
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
            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + ".json");
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
            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + ".json");
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

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + ".json");
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

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + ".json");
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

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + ".json");
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

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + ".json");
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
