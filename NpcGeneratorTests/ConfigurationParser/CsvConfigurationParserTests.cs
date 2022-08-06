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
using System.Collections.Generic;
using System.IO;

namespace Tests
{
    [TestClass]
    public class CsvConfigurationParserTests : FileCreatingTests
    {
        [TestMethod]
        public void GeneratesTraitSchema()
        {
            string path = Path.Combine(TestDirectory, "colour.csv");
            string text = "Colour,Weight\n" +
                "Green,1\n" +
                "Red,1";
            File.WriteAllText(path, text);

            CsvConfigurationParser parser = new CsvConfigurationParser();
            TraitSchema schema = parser.Parse(path);
            Assert.IsNotNull(schema, "Failed to generate a schema from the valid text");

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

            string path = Path.Combine(TestDirectory, "colourAndAnimal.csv");
            string text = CATEGORY1_TITLE + ",Weight," + CATEGORY2_TITLE + ",Weight\n" +
                CATEGORY1_TRAIT1 + ",1," + CATEGORY2_TRAIT1 + ",1\n" +
                CATEGORY1_TRAIT2 + ",1," + CATEGORY2_TRAIT2 + ",1";
            File.WriteAllText(path, text);

            CsvConfigurationParser parser = new CsvConfigurationParser();
            TraitSchema schema = parser.Parse(path);
            Assert.IsNotNull(schema, "Failed to generate a schema from the valid text");

            IReadOnlyList<TraitCategory> categories = schema.GetTraitCategories();
            Assert.IsNotNull(categories, "Schema categories is null which should never happen.");
            Assert.AreEqual(2, categories.Count, "Schema has incorrect number of TraitCategories");
            TraitCategory firstCategory = categories[0];
            Assert.IsNotNull(firstCategory, "Schema has a null first TraitCategory");
            Assert.AreEqual(firstCategory.Name, CATEGORY1_TITLE, "First category doesn't have name " + CATEGORY1_TITLE);
            TraitChooser firstChooser = firstCategory.CreateTraitChooser();
            string[] colours = firstChooser.Choose(firstCategory.DefaultSelectionCount, out List<BonusSelection> bonusSelections1);
            Assert.AreEqual(0, bonusSelections1.Count, "Bonus selection returned where there should be none.");
            Assert.AreEqual(1, colours.Length, "Wrong number of traits selected from " + CATEGORY1_TITLE);
            Assert.IsTrue(colours[0] == CATEGORY1_TRAIT1 || colours[0] == CATEGORY1_TRAIT2, 
                CATEGORY1_TITLE + " chose an invalid trait " + colours[0]);

            TraitCategory secondCategory = categories[1];
            Assert.IsNotNull(secondCategory, "Schema has a null second TraitCategory");
            Assert.AreEqual(secondCategory.Name, CATEGORY2_TITLE, "Second category doesn't have name " + CATEGORY2_TITLE);
            TraitChooser secondChooser = secondCategory.CreateTraitChooser();
            string[] animals = secondChooser.Choose(secondCategory.DefaultSelectionCount, out List<BonusSelection> bonusSelections2);
            Assert.AreEqual(0, bonusSelections2.Count, "Bonus selection returned where there should be none.");
            Assert.AreEqual(1, animals.Length, "Wrong number of traits selected from " + CATEGORY2_TITLE);
            Assert.IsTrue(animals[0] == CATEGORY2_TRAIT1 || animals[0] == CATEGORY2_TRAIT2, 
                CATEGORY2_TITLE + " chose an invalid trait " + animals[0]);

            File.Delete(path);
        }

        [TestMethod]
        public void MissingFirstTitleThrowsException()
        {
            string path = Path.Combine(TestDirectory, "missingFirstTitle.csv");
            string text = ",Weight\n" +
               "Green,1\n" +
               "Red,1";
            File.WriteAllText(path, text);

            bool threwException = false;
            try 
            {
                CsvConfigurationParser parser = new CsvConfigurationParser();
                TraitSchema schema = parser.Parse(path);
            }
            catch(Exception)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Missing title failed to throw exception");

            File.Delete(path); 
        }

        [TestMethod]
        public void MissingSecondTitleThrowsException()
        {
            string path = Path.Combine(TestDirectory, "missingSecondTitle.csv");
            string text = "Colour,Weight\n" +
                "Green,1,Gorilla,1\n" +
                "Red,1,Rhino,1";
            File.WriteAllText(path, text);

            bool threwException = false;
            try
            {
                CsvConfigurationParser parser = new CsvConfigurationParser();
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
        public void DuplicateCategoryNames()
        {
            string path = Path.Combine(TestDirectory, "colour.csv");
            string text = "Colour,Weight,Colour,Weight\n" +
                "Green,1,Red,1";
            File.WriteAllText(path, text);

            CsvConfigurationParser parser = new CsvConfigurationParser();

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch(Exception)
            {
                threwException = true;
            }
            
            Assert.IsTrue(threwException, "Failed to throw exception for categories with duplicate names");

            File.Delete(path);
        }

        [TestMethod]
        public void DuplicateTraitNamesInACategory()
        {
            string path = Path.Combine(TestDirectory, "colour.csv");
            string text = "Colour,Weight\n" +
                "Green,1\n" +
                "Green,1";
            File.WriteAllText(path, text);

            CsvConfigurationParser parser = new CsvConfigurationParser();

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (Exception)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Failed to throw exception for categories with duplicate names");

            File.Delete(path);
        }
    }
}
