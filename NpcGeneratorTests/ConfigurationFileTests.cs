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
    public class ConfigurationFileTests : FileCreatingTests
    {
        [TestMethod]
        public void GeneratesTraitSchema()
        {
            string path = Path.Combine(TestDirectory, "colour.csv");
            string text = "Colour,Weight\n" +
                "Green,1\n" +
                "Red,1";
            File.WriteAllText(path, text);

            TraitSchema schema = ConfigurationFile.Parse(path);
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

            TraitSchema schema = ConfigurationFile.Parse(path);
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
                TraitSchema schema = ConfigurationFile.Parse(path);
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
                TraitSchema schema = ConfigurationFile.Parse(path);
            }
            catch (Exception)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Missing title failed to throw exception");

            File.Delete(path); 
        }
    }
}
