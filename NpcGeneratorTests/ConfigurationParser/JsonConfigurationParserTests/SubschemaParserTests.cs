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
    public class SubschemaParserTests
    {
        const string SCHEMA_PATH = "ConfigurationSchema.json";

        const string VALID_SUBSCHEMA_RELATIVE_PATH = "ValidSubschema.csv";
        const string INVALID_SUBSCHEMA_RELATIVE_PATH = "InvalidSubschema.csv";

        const string SUBSCHEMA_CATEGORY0 = "Shade";
        const string SUBSCHEMA_C0T0_NAME = "Green";
        const int SUBSCHEMA_C0T0_WEIGHT = 1;
        const string SUBSCHEMA_C0T1_NAME = "Red";
        const int SUBSCHEMA_C0T1_WEIGHT = 3;

        const string SUBSCHEMA_CATEGORY1 = "Animal";
        const string SUBSCHEMA_C1T0_NAME = "Bear";
        const int SUBSCHEMA_C1T0_WEIGHT = 1;
        const string SUBSCHEMA_C1T1_NAME = "Rhino";
        const int SUBSCHEMA_C1T1_WEIGHT = 5;
        const string SUBSCHEMA_C1T2_NAME = "Lion";
        const int SUBSCHEMA_C1T2_WEIGHT = 1;

        [TestMethod]
        public void SubSchemaCategoriesAreValid()
        {
            const string SCHEMA_CATEGORY = "Colour";

            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{SCHEMA_CATEGORY}',
                        'selections': 1,
                        'traits_from_file' : {{
                            'csv_file' : '{VALID_SUBSCHEMA_RELATIVE_PATH}', 
                            'category_name_in_file' : '{SUBSCHEMA_CATEGORY0}' 
                        }}
                    }}
                ]
            }}";

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);
            TraitSchema schema = parser.Parse(text);
            Assert.IsNotNull(schema, "Failed to generate a schema from the valid text");

            IReadOnlyList<TraitCategory> categories = schema.GetTraitCategories();
            Assert.AreEqual(1, categories.Count, "Wrong number of categories");

            TraitCategory category = categories[0];
            Assert.AreEqual(SCHEMA_CATEGORY, category.Name, "Wrong trait name");
            IReadOnlyList<Trait> traits = category.GetTraits();
            Assert.AreEqual(2, traits.Count, "Wrong trait count");

            Trait foundTrait0 = ListUtil.Find(traits, trait => trait.Name == SUBSCHEMA_C0T0_NAME && 
                trait.Weight == SUBSCHEMA_C0T0_WEIGHT && 
                trait.IsHidden == false && 
                trait.BonusSelection == null);
            Assert.IsNotNull(foundTrait0, "Did not find a required trait");

            Trait foundTrait1 = ListUtil.Find(traits, trait => trait.Name == SUBSCHEMA_C0T1_NAME &&
                trait.Weight == SUBSCHEMA_C0T1_WEIGHT &&
                trait.IsHidden == false &&
                trait.BonusSelection == null);
            Assert.IsNotNull(foundTrait1, "Did not find a required trait");
        }

        [TestMethod, ExpectedException(typeof(EmptyCategoryNameException))]
        public void SubSchemaCategoriesInvalidFormat()
        {
            const string SCHEMA_CATEGORY = "Colour";

            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{SCHEMA_CATEGORY}',
                        'selections': 1,
                        'traits_from_file' : {{
                            'csv_file' : '{INVALID_SUBSCHEMA_RELATIVE_PATH}', 
                            'category_name_in_file' : '{SUBSCHEMA_CATEGORY0}' 
                        }}
                    }}
                ]
            }}";

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);
            parser.Parse(text);
        }

        [TestMethod, ExpectedException(typeof(SubSchemaCategoryNotFoundException))]
        public void SubSchemaCategoryNotFound()
        {
            const string SCHEMA_CATEGORY = "Colour";
            const string SUBSCHEMA_CATEOGRY_NOT_FOUND = "Height";

            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{SCHEMA_CATEGORY}',
                        'selections': 1,
                        'traits_from_file' : {{
                            'csv_file' : '{VALID_SUBSCHEMA_RELATIVE_PATH}', 
                            'category_name_in_file' : '{SUBSCHEMA_CATEOGRY_NOT_FOUND}'
                        }}
                    }}
                ]
            }}";

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);
            parser.Parse(text);
        }

        [TestMethod, ExpectedException(typeof(SubSchemaNotFoundException))]
        public void SubSchemaFileNotFound()
        {
            const string SCHEMA_CATEGORY = "Colour";
            const string SUBSCHEMA_NOT_FOUND = "not_found.csv";

            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{SCHEMA_CATEGORY}',
                        'selections': 1,
                        'traits_from_file' : {{
                            'csv_file' : '{SUBSCHEMA_NOT_FOUND}', 
                            'category_name_in_file' : '{SUBSCHEMA_CATEGORY0}'
                        }}
                    }}
                ]
            }}";

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);
            parser.Parse(text);
        }

        [TestMethod]
        public void SubSchemaAndSchemaMergeTraits()
        {
            const string SCHEMA_CATEGORY = "Colour";
            const string LOCAL_C0T0_NAME = "Blue";
            const int LOCAL_C0T0_WEIGHT = 2;

            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{SCHEMA_CATEGORY}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : '{LOCAL_C0T0_NAME}', 
                                'weight' : {LOCAL_C0T0_WEIGHT}
                            }}
                        ],
                        'traits_from_file' : {{
                            'csv_file' : '{VALID_SUBSCHEMA_RELATIVE_PATH}', 
                            'category_name_in_file' : '{SUBSCHEMA_CATEGORY0}' 
                        }}
                    }}
                ]
            }}";

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);
            TraitSchema schema = parser.Parse(text);
            Assert.IsNotNull(schema, "Failed to generate a schema from the valid text");

            IReadOnlyList<TraitCategory> categories = schema.GetTraitCategories();
            Assert.AreEqual(1, categories.Count, "Wrong number of categories");

            TraitCategory category = categories[0];
            Assert.AreEqual(SCHEMA_CATEGORY, category.Name, "Wrong trait name");
            IReadOnlyList<Trait> traits = category.GetTraits();
            Assert.AreEqual(3, traits.Count, "Wrong trait count");

            Trait foundTrait0 = ListUtil.Find(traits, trait => trait.Name == SUBSCHEMA_C0T0_NAME &&
                trait.Weight == SUBSCHEMA_C0T0_WEIGHT &&
                trait.IsHidden == false &&
                trait.BonusSelection == null);
            Assert.IsNotNull(foundTrait0, "Did not find a required trait");

            Trait foundTrait1 = ListUtil.Find(traits, trait => trait.Name == SUBSCHEMA_C0T1_NAME &&
                trait.Weight == SUBSCHEMA_C0T1_WEIGHT &&
                trait.IsHidden == false &&
                trait.BonusSelection == null);
            Assert.IsNotNull(foundTrait1, "Did not find a required trait");

            Trait foundTrait2 = ListUtil.Find(traits, trait => trait.Name == LOCAL_C0T0_NAME &&
                trait.Weight == LOCAL_C0T0_WEIGHT &&
                trait.IsHidden == false &&
                trait.BonusSelection == null);
            Assert.IsNotNull(foundTrait1, "Did not find a required trait");
        }

        [TestMethod, ExpectedException(typeof(DuplicateTraitNameInCategoryException))]
        public void SubSchemaAndSchemaTraitDuplication()
        {
            const string SCHEMA_CATEGORY = "Colour";

            string text = $@"{{
                'trait_categories' : [
                    {{
                        'name' : '{SCHEMA_CATEGORY}',
                        'selections': 1,
                        'traits' : [
                            {{ 
                                'name' : '{SUBSCHEMA_C0T0_NAME}', 
                                'weight' : {SUBSCHEMA_C0T0_WEIGHT}
                            }}
                        ],
                        'traits_from_file' : {{
                            'csv_file' : '{VALID_SUBSCHEMA_RELATIVE_PATH}', 
                            'category_name_in_file' : '{SUBSCHEMA_CATEGORY0}' 
                        }}
                    }}
                ]
            }}";

            JsonConfigurationParser parser = new JsonConfigurationParser(SCHEMA_PATH);
            parser.Parse(text);
        }
    }
}
