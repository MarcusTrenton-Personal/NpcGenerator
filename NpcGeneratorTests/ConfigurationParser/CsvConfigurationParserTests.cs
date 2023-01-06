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
using System;
using System.Collections.Generic;
using System.IO;

namespace Tests
{
    [TestClass]
    public class CsvConfigurationParserTests
    {
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void NullText()
        {
            CsvConfigurationParser parser = new CsvConfigurationParser();
            parser.Parse(null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void EmptyText()
        {
            CsvConfigurationParser parser = new CsvConfigurationParser();
            parser.Parse(String.Empty);
        }

        [TestMethod]
        public void GeneratesTraitSchema()
        {
            string text = "Colour,Weight\n" +
                "Green,1\n" +
                "Red,1";

            CsvConfigurationParser parser = new CsvConfigurationParser();
            TraitSchema schema = parser.Parse(text);
            Assert.IsNotNull(schema, "Failed to generate a schema from the valid text");
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

            string text = CATEGORY1_TITLE + ",Weight," + CATEGORY2_TITLE + ",Weight\n" +
                CATEGORY1_TRAIT1 + ",1," + CATEGORY2_TRAIT1 + ",1\n" +
                CATEGORY1_TRAIT2 + ",1," + CATEGORY2_TRAIT2 + ",1";

            CsvConfigurationParser parser = new CsvConfigurationParser();
            TraitSchema schema = parser.Parse(text);
            Assert.IsNotNull(schema, "Failed to generate a schema from the valid text");

            IReadOnlyList<TraitCategory> categories = schema.GetTraitCategories();
            Assert.IsNotNull(categories, "Schema categories is null which should never happen.");
            Assert.AreEqual(2, categories.Count, "Schema has incorrect number of TraitCategories");

            TraitCategory firstCategory = categories[0];
            Assert.IsNotNull(firstCategory, "Schema has a null first TraitCategory");
            Assert.AreEqual(firstCategory.Name, CATEGORY1_TITLE, "First category doesn't have name " + CATEGORY1_TITLE);
            TraitChooser firstChooser = firstCategory.CreateTraitChooser(m_random, new Npc());
            Npc.Trait[] colours = firstChooser.Choose(firstCategory.DefaultSelectionCount, out IReadOnlyList<BonusSelection> bonusSelections1);
            Assert.AreEqual(0, bonusSelections1.Count, "Bonus selection returned where there should be none.");
            Assert.AreEqual(1, colours.Length, "Wrong number of traits selected from " + CATEGORY1_TITLE);
            Assert.IsTrue(colours[0].Name == CATEGORY1_TRAIT1 || colours[0].Name == CATEGORY1_TRAIT2, 
                CATEGORY1_TITLE + " chose an invalid trait " + colours[0].Name);

            TraitCategory secondCategory = categories[1];
            Assert.IsNotNull(secondCategory, "Schema has a null second TraitCategory");
            Assert.AreEqual(secondCategory.Name, CATEGORY2_TITLE, "Second category doesn't have name " + CATEGORY2_TITLE);
            TraitChooser secondChooser = secondCategory.CreateTraitChooser(m_random, new Npc());
            Npc.Trait[] animals = secondChooser.Choose(secondCategory.DefaultSelectionCount, out IReadOnlyList<BonusSelection> bonusSelections2);
            Assert.AreEqual(0, bonusSelections2.Count, "Bonus selection returned where there should be none.");
            Assert.AreEqual(1, animals.Length, "Wrong number of traits selected from " + CATEGORY2_TITLE);
            Assert.IsTrue(animals[0].Name == CATEGORY2_TRAIT1 || animals[0].Name == CATEGORY2_TRAIT2, 
                CATEGORY2_TITLE + " chose an invalid trait " + animals[0].Name);
        }

        [TestMethod]
        public void MissingFirstTitleThrowsException()
        {
            string text = ",Weight\n" +
               "Green,1\n" +
               "Red,1";

            bool threwException = false;
            try 
            {
                CsvConfigurationParser parser = new CsvConfigurationParser();
                TraitSchema schema = parser.Parse(text);
            }
            catch(EmptyCategoryNameException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Missing title failed to throw exception");
        }

        [TestMethod]
        public void MissingSecondTitleThrowsException()
        {
            const string FIRST_TRAIT_MISSING_CATEGORY = "Gorilla";

            string text = "Colour,Weight\n" +
                "Green,1," + FIRST_TRAIT_MISSING_CATEGORY + ",1\n" +
                "Red,1,Rhino,1";

            bool threwException = false;
            try
            {
                CsvConfigurationParser parser = new CsvConfigurationParser();
                TraitSchema schema = parser.Parse(text);
            }
            catch (TraitMissingCategoryException exception)
            {
                Assert.AreEqual(FIRST_TRAIT_MISSING_CATEGORY, exception.TraitName, "Wrong trait name returned");

                threwException = true;
            }

            Assert.IsTrue(threwException, "Missing title failed to throw exception");
        }

        [TestMethod, ExpectedException(typeof(CategoryWeightMismatchException))]
        public void MissingWeightColumn()
        {
            string text = "Colour\n" +
                "Green,1\n" +
                "Red,1";

            CsvConfigurationParser parser = new CsvConfigurationParser();
            parser.Parse(text);
        }

        [TestMethod, ExpectedException(typeof(CategoryWeightMismatchException))]
        public void ExtraColumn()
        {
            string text = "Colour,Weight,Bonus Selection\n" +
                "Green,1\n" +
                "Red,1";

            CsvConfigurationParser parser = new CsvConfigurationParser();
            parser.Parse(text);
        }

        [TestMethod, ExpectedException(typeof(CategoryWeightMismatchException))]
        public void SkipFirstColumn()
        {
            string text = ",Colour,Weight\n" +
                ",Green,1\n" +
                ",Red,1";

            CsvConfigurationParser parser = new CsvConfigurationParser();
            parser.Parse(text);
        }

        [TestMethod, ExpectedException(typeof(CategoryWeightMismatchException))]
        public void SkipMiddleColumn()
        {
            string text = "Colour,,Weight\n" +
                "Green,,1\n" +
                "Red,,1";

            CsvConfigurationParser parser = new CsvConfigurationParser();
            parser.Parse(text);
        }

        [TestMethod, ExpectedException(typeof(CategoryWeightMismatchException))]
        public void SkipColumnBetweenCategories()
        {
            string text = "Colour,Weight,,Animal,Weight\n" +
                "Green,1,,Bear,1\n" +
                "Red,1,,Rhino,1";

            CsvConfigurationParser parser = new CsvConfigurationParser();
            parser.Parse(text);
        }

        [TestMethod, ExpectedException(typeof(MissingWeightException))]
        public void MissingWeightCell()
        {
            const string CATEGORY = "Colour";
            const string TRAIT = "Green";

            string text = CATEGORY + ",Weight\n" +
                TRAIT + "\n" +
                "Red,1";

            CsvConfigurationParser parser = new CsvConfigurationParser();
            parser.Parse(text);
        }

        [TestMethod, ExpectedException(typeof(MissingWeightException))]
        public void EmptyWeightCell()
        {
            const string CATEGORY = "Colour";
            const string TRAIT = "Green";

            string text = CATEGORY + ",Weight\n" +
                TRAIT + ",\n" +
                "Red,1";

            CsvConfigurationParser parser = new CsvConfigurationParser();
            parser.Parse(text);
        }

        [TestMethod]
        public void WeightIsNegative()
        {
            const string CATEGORY = "Colour";
            const string TRAIT = "Green";
            const string WEIGHT = "-2";

            string text = CATEGORY + ",Weight\n" +
                TRAIT + "," + WEIGHT + "\n" +
                "Red,1";

            bool threwException = false;
            try
            {
                CsvConfigurationParser parser = new CsvConfigurationParser();
                TraitSchema schema = parser.Parse(text);
            }
            catch (WeightIsNotWholeNumberException exception)
            {
                Assert.AreEqual(new TraitId(CATEGORY, TRAIT), exception.TraitId, "Wrong TraitId returned");
                Assert.AreEqual(WEIGHT, exception.InvalidWeight, "Wrong weight returned");

                threwException = true;
            }

            Assert.IsTrue(threwException, "Malformed weight cell failed to throw exception");
        }

        [TestMethod]
        public void WeightIsFraction()
        {
            const string CATEGORY = "Colour";
            const string TRAIT = "Green";
            const string WEIGHT = "2.0";

            string text = CATEGORY + ",Weight\n" +
                TRAIT + "," + WEIGHT + "\n" +
                "Red,1";

            bool threwException = false;
            try
            {
                CsvConfigurationParser parser = new CsvConfigurationParser();
                TraitSchema schema = parser.Parse(text);
            }
            catch (WeightIsNotWholeNumberException exception)
            {
                Assert.AreEqual(new TraitId(CATEGORY, TRAIT), exception.TraitId, "Wrong TraitId returned");
                Assert.AreEqual(WEIGHT, exception.InvalidWeight, "Wrong weight returned");

                threwException = true;
            }

            Assert.IsTrue(threwException, "Malformed weight cell failed to throw exception");
        }

        [TestMethod]
        public void WeightIsWord()
        {
            const string CATEGORY = "Colour";
            const string TRAIT = "Green";
            const string WEIGHT = "One";

            string text = CATEGORY + ",Weight\n" +
                TRAIT + "," + WEIGHT + "\n" +
                "Red,1";

            bool threwException = false;
            try
            {
                CsvConfigurationParser parser = new CsvConfigurationParser();
                TraitSchema schema = parser.Parse(text);
            }
            catch (WeightIsNotWholeNumberException exception)
            {
                Assert.AreEqual(new TraitId(CATEGORY, TRAIT), exception.TraitId, "Wrong TraitId returned");
                Assert.AreEqual(WEIGHT, exception.InvalidWeight, "Wrong weight returned");

                threwException = true;
            }

            Assert.IsTrue(threwException, "Malformed weight cell failed to throw exception");
        }

        [TestMethod]
        public void WeightIsBool()
        {
            const string CATEGORY = "Colour";
            const string TRAIT = "Green";
            const string WEIGHT = "false";

            string text = CATEGORY + ",Weight\n" +
                TRAIT + "," + WEIGHT + "\n" +
                "Red,1";

            bool threwException = false;
            try
            {
                CsvConfigurationParser parser = new CsvConfigurationParser();
                TraitSchema schema = parser.Parse(text);
            }
            catch (WeightIsNotWholeNumberException exception)
            {
                Assert.AreEqual(new TraitId(CATEGORY, TRAIT), exception.TraitId, "Wrong TraitId returned");
                Assert.AreEqual(WEIGHT, exception.InvalidWeight, "Wrong weight returned");

                threwException = true;
            }

            Assert.IsTrue(threwException, "Malformed weight cell failed to throw exception");
        }

        [TestMethod]
        public void DuplicateCategoryNames()
        {
            const string CATEGORY = "Colour";

            string text = CATEGORY + ",Weight," + CATEGORY + ",Weight\n" +
                "Green,1,Red,1";

            CsvConfigurationParser parser = new CsvConfigurationParser();

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(text);
            }
            catch(DuplicateCategoryNameException exception)
            {
                Assert.AreEqual(CATEGORY, exception.Category, "Wrong duplicate detected");

                threwException = true;
            }
            
            Assert.IsTrue(threwException, "Failed to throw exception for categories with duplicate names");
        }

        [TestMethod]
        public void DuplicateTraitNamesInACategory()
        {
            const string CATEGORY = "Colour";
            const string TRAIT = "Green";

            string text = CATEGORY + ",Weight\n" +
                TRAIT + ",1\n" +
                TRAIT + ",1";

            CsvConfigurationParser parser = new CsvConfigurationParser();

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(text);
            }
            catch (DuplicateTraitNameInCategoryException exception)
            {
                Assert.AreEqual(new TraitId(CATEGORY, TRAIT), exception.TraitId, "Wrong duplicate TraitId detected");

                threwException = true;
            }

            Assert.IsTrue(threwException, "Failed to throw exception for categories with duplicate names");
        }

        [TestMethod]
        public void DuplicateTraitNamesInDifferentCategories()
        {
            const string TRAIT_NAME = "Brown";

            string text = "Hair,Weight,Skin,Weight\n" +
                TRAIT_NAME + ",1," + TRAIT_NAME + ",1";

            CsvConfigurationParser parser = new CsvConfigurationParser();

            TraitSchema schema = parser.Parse(text);
            IReadOnlyList<TraitCategory> categories = schema.GetTraitCategories();

            Assert.AreEqual(2, categories.Count, "Wrong number of categories");
            Assert.IsNotNull(categories[0].GetTrait(TRAIT_NAME), "Missing expected trait in category");
            Assert.IsNotNull(categories[1].GetTrait(TRAIT_NAME), "Missing expected trait in category");
        }

        readonly MockRandom m_random = new MockRandom();
    }
}
