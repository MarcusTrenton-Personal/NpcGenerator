using Microsoft.VisualStudio.TestTools.UnitTesting;
using NpcGenerator;
using System;
using System.IO;

namespace Tests
{
    [TestClass]
    public class ConfigurationTests
    {
        public ConfigurationTests()
        {
            string commonAppData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            m_testDirectory = Path.Combine(commonAppData, FilePathHelper.APP_DATA_FOLDER, "UnitTestInput");
            Directory.CreateDirectory(m_testDirectory);
        }

        [TestMethod]
        public void GeneratesTraitSchema()
        {
            string path = Path.Combine(m_testDirectory, "colour.csv");
            string text = "Colour,Weight\n" +
                "Green,1\n" +
                "Red,1";
            WriteTestFile(path, text);

            TraitSchema schema = Configuration.Parse(path);
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

            string path = Path.Combine(m_testDirectory, "colourAndAnimal.csv");
            string text = CATEGORY1_TITLE + ",Weight," + CATEGORY2_TITLE + ",Weight\n" +
                CATEGORY1_TRAIT1 + ",1," + CATEGORY2_TRAIT1 + ",1\n" +
                CATEGORY1_TRAIT2 + ",1," + CATEGORY2_TRAIT2 + ",1";
            WriteTestFile(path, text);

            TraitSchema schema = Configuration.Parse(path);
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
            string path = Path.Combine(m_testDirectory, "missingFirstTitle.csv");
            string text = ",Weight\n" +
               "Green,1\n" +
               "Red,1";
            WriteTestFile(path, text);

            bool threwException = false;
            try 
            {
                TraitSchema schema = Configuration.Parse(path);
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
            string path = Path.Combine(m_testDirectory, "missingSecondTitle.csv");
            string text = "Colour,Weight\n" +
                "Green,1,Gorilla,1\n" +
                "Red,1,Rhino,1";
            WriteTestFile(path, text);

            bool threwException = false;
            try
            {
                TraitSchema schema = Configuration.Parse(path);
            }
            catch (Exception)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Missing title failed to throw exception");

            File.Delete(path); 
        }

        private void WriteTestFile(string path, string content)
        {
            using(StreamWriter writer = File.CreateText(path))
            {
                writer.Write(content);
                writer.Close();
            }
        }

        string m_testDirectory;
    }
}
