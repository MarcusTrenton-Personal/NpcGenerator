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
            string path = Path.Combine(TestDirectory, "colour.csv");
            string text = "Colour,Weight\n" +
                "Green,1\n" +
                "Red,1";
            File.WriteAllText(path, text);

            StubFormatConfigurationParser stubParser = new StubFormatConfigurationParser()
            {
                SupportedFileExtension = ".csv"
            };
            ConfigurationParser parser = new ConfigurationParser(new List<IFormatConfigurationParser>() { stubParser });
            TraitSchema schema = parser.Parse(path);
            Assert.IsNotNull(schema, "Failed to generate a schema from the valid text");

            File.Delete(path);
        }

        [TestMethod]
        public void NoSchemaFromUnsupportedFileType()
        {
            string path = Path.Combine(TestDirectory, "colour.xml");
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
            File.WriteAllText(path, text);

            bool threwException = false;
            try
            {
                StubFormatConfigurationParser stubParser = new StubFormatConfigurationParser()
                {
                    SupportedFileExtension = ".csv"
                };
                ConfigurationParser parser = new ConfigurationParser(new List<IFormatConfigurationParser>() { stubParser });
                TraitSchema schema = parser.Parse(path);
            }
            catch (Exception e)
            {
                threwException = true;
                Assert.IsTrue(e.Message.Contains("csv"), "Exception doesn't contain a list of supported file types");
            }

            Assert.IsTrue(threwException, "Unsupported xml did not throw an exception");

            File.Delete(path);
        }
    }
}
