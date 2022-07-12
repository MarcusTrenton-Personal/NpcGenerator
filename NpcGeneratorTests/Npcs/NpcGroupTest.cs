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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using NpcGenerator;
using System.Collections.Generic;
using System.IO;

namespace Tests
{
    [TestClass]
    public class NpcGroupTests
    {
        [TestMethod]
        public void NpcGroupGeneratesCsv()
        {
            TraitCategory colourCategory = new TraitCategory("Colour");
            colourCategory.Add(new Trait("Blue", 1));

            TraitCategory animalCategory = new TraitCategory("Animal");
            animalCategory.Add(new Trait("Bear", 1));

            TraitSchema schema = new TraitSchema();
            schema.Add(colourCategory);
            schema.Add(animalCategory);

            NpcGroup npcGroup = new NpcGroup(schema, 1);
            string csv = npcGroup.ToCsv();

            Assert.AreEqual("Colour,Animal\nBlue,Bear", csv, "NpcGroup did not generate expected CSV text");
        }

        [TestMethod]
        public void NpcGroupGeneratesJson()
        {
            TraitCategory colourCategory = new TraitCategory("Colour");
            colourCategory.Add(new Trait("Blue", 1));

            TraitCategory animalCategory = new TraitCategory("Animal");
            animalCategory.Add(new Trait("Bear", 1));

            TraitSchema traitSchema = new TraitSchema();
            traitSchema.Add(colourCategory);
            traitSchema.Add(animalCategory);

            NpcGroup npcGroup = new NpcGroup(traitSchema, 1);
            string jsonText = npcGroup.ToJson();
            JToken json = JToken.Parse(jsonText);

            string schemaPath = "NpcGroupSchema.json";
            string schemaText = File.ReadAllText(schemaPath);
            JSchema schema = JSchema.Parse(schemaText);
            
            IList<string> errorMessages;
            string concatenatedMessages = "";
            bool isValid = json.IsValid(schema, out errorMessages);
            if (!isValid)
            {
                foreach (var error in errorMessages)
                {
                    concatenatedMessages += error + "\n";
                }
            }
            Assert.IsTrue(isValid, "Json validation failed with message: " + concatenatedMessages);
        }
    }
}
