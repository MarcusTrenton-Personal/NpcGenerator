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
    public class BonusSelectionJsonConfigurationParserTests : FileCreatingTests
    {
        const string SCHEMA_PATH = "ConfigurationSchema.json";

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
    }
}
