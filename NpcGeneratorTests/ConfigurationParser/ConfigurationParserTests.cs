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
    public class ConfigurationParserTests : FileCreatingTests
    {
        [TestMethod]
        public void GeneratesTraitSchemaFromSupportedFileType()
        {
            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + ".csv");
            string text = "Colour,Weight\n" +
                "Green,1\n" +
                "Red,1";
            File.WriteAllText(path, text, Constants.TEXT_ENCODING);

            StubFormatConfigurationParser stubParser = new StubFormatConfigurationParser();
            ConfigurationParser parser = new ConfigurationParser(new List<FormatParser>() { new FormatParser(".csv", stubParser) });
            TraitSchema schema = parser.Parse(path);
            Assert.IsNotNull(schema, "Failed to generate a schema from the valid text");

            File.Delete(path);
        }

        [TestMethod]
        public void EmptyText()
        {
            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string fileName = method + ".csv";
            string path = Path.Combine(TestDirectory, fileName);
            string text = string.Empty;
            File.WriteAllText(path, text, Constants.TEXT_ENCODING);

            StubFormatConfigurationParser stubParser = new StubFormatConfigurationParser();
            ConfigurationParser parser = new ConfigurationParser(new List<FormatParser>() { new FormatParser(".csv", stubParser) });

            bool threwException = false;
            try
            {
                TraitSchema schema = parser.Parse(path);
            }
            catch (EmptyFileException exception)
            {
                Assert.AreEqual(fileName, exception.FileName, "Wrong file name returned");

                threwException = true;
            }

            Assert.IsTrue(threwException, "Empty file failed to throw exception");

            File.Delete(path);
        }

        [TestMethod]
        public void NoSchemaFromUnsupportedFileType()
        {
            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + ".xml");
            string text = "<trait_categories>\n"+
                "\t<array>\n" +
                "\t\t<item>\n" +
                "\t\t\t<traitCategory name=\"Colour\">\n" +
                "\t\t\t\t<name>Colour</name>\n" +
                "\t\t\t\t<array>\n" +
                "\t\t\t\t\t<item>\n" +
                "\t\t\t\t\t\t<trait>\n" +
                "\t\t\t\t\t\t\t<name>Green</name>\n" +
                "\t\t\t\t\t\t\t<weight>1</weight>\n" +
                "\t\t\t\t\t\t</trait>\n" +
                "\t\t\t\t\t</item>\n" +
                "\t\t\t\t\t<item>\n" +
                "\t\t\t\t\t\t<trait>\n" +
                "\t\t\t\t\t\t\t<name>Red</name>\n" +
                "\t\t\t\t\t\t\t<weight>1</weight>\n" +
                "\t\t\t\t\t\t</trait>\n" +
                "\t\t\t\t\t</item>\n" +
                "\t\t\t\t</array>\n" +
                "\t\t\t</traitCategory>" +
                "\t\t</item>\n" +
                "\t</array>\n" +
                "</trait_categories>";
            File.WriteAllText(path, text, Constants.TEXT_ENCODING);

            bool threwException = false;
            try
            {
                StubFormatConfigurationParser stubParser = new StubFormatConfigurationParser();
                ConfigurationParser parser = new ConfigurationParser(new List<FormatParser>() { new FormatParser(".csv", stubParser) }); ;
                TraitSchema schema = parser.Parse(path);
            }
            catch (ArgumentException e)
            {
                threwException = true;
                Assert.IsTrue(e.Message.Contains("csv"), "Exception doesn't contain a list of supported file types");
            }

            Assert.IsTrue(threwException, "Unsupported xml did not throw an exception");

            File.Delete(path);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void NullParserCollection()
        {
            new ConfigurationParser(null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void NullParserInCollection()
        {
            new ConfigurationParser(new List<FormatParser>() { null });
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ParseStringIsNull()
        {
            StubFormatConfigurationParser stubParser = new StubFormatConfigurationParser();
            ConfigurationParser parser = new ConfigurationParser(new List<FormatParser>() { new FormatParser(".csv", stubParser) });

            parser.Parse(null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void ParseStringIsEmpty()
        {
            StubFormatConfigurationParser stubParser = new StubFormatConfigurationParser();
            ConfigurationParser parser = new ConfigurationParser(new List<FormatParser>() { new FormatParser(".csv", stubParser) });

            parser.Parse(String.Empty);
        }

        [TestMethod, ExpectedException(typeof(FileNotFoundException))]
        public void FileNotFound()
        {
            StubFormatConfigurationParser stubParser = new StubFormatConfigurationParser();
            ConfigurationParser parser = new ConfigurationParser(new List<FormatParser>() { new FormatParser(".csv", stubParser) });

            parser.Parse("FileNotFound.csv");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void FormatParserHasNullParser()
        {
            new FormatParser(".csv", null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void FormatParserHasNullExtension()
        {
            new FormatParser(null, new StubFormatConfigurationParser());
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void FormatParserHasEmptyExtension()
        {
            new FormatParser(String.Empty, new StubFormatConfigurationParser());
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void FormatParserHasMalformedExtension()
        {
            new FormatParser("abc", new StubFormatConfigurationParser());
        }

        [TestMethod]
        public void FormatParserConstructedSuccessfully()
        {
            new FormatParser(".abc", new StubFormatConfigurationParser());
        }
    }
}
