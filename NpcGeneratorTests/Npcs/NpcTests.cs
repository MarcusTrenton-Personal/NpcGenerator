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
using Newtonsoft.Json;
using NpcGenerator;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tests
{
    [TestClass]
    public class NpcTests
    {
        [TestMethod]
        public void ExportSingleTraitCsv()
        {
            Npc npc = new Npc();
            npc.AddTrait(category:"Colour", traits: new string[] { "Blue" });
            npc.AddTrait(category:"Animal", traits: new string[] { "Bear" });

            StringBuilder textBuilder = new StringBuilder();
            npc.ToCsvRow(textBuilder, new List<string>() { "Colour", "Animal"});
            string csvRow = textBuilder.ToString();

            Assert.AreEqual("Blue,Bear", csvRow, "Npc did not generate expected CSV row");
        }

        [TestMethod]
        public void ExportMultiTraitCsv()
        {
            Npc npc = new Npc();
            npc.AddTrait(category: "Colour", traits: new string[] { "Blue", "Green" });
            npc.AddTrait(category: "Terrain", traits: new string[] { "Hills", "River" });

            StringBuilder textBuilder = new StringBuilder();
            npc.ToCsvRow(textBuilder, new List<string>() { "Colour", "Terrain" });
            string csvRow = textBuilder.ToString();

            Assert.AreEqual("Blue & Green,Hills & River", csvRow, "Npc did not generate expected CSV row");
        }

        [TestMethod]
        public void ExportNoTraitCsv()
        {
            Npc npc = new Npc();

            StringBuilder textBuilder = new StringBuilder();
            npc.ToCsvRow(textBuilder, new List<string>() { "Purpose"});
            string csvRow = textBuilder.ToString();

            Assert.AreEqual("", csvRow, "Npc did not generate expected CSV row");
        }

        [TestMethod]
        public void ExportSingleTraitJson()
        {
            const string CATEGORY1_NAME = "Colour";
            const string CATEGORY1_TRAIT = "Blue";
            const string CATEGORY2_NAME = "Animal";
            const string CATEGORY2_TRAIT = "Bear";

            Npc npc = new Npc();
            npc.AddTrait(category: CATEGORY1_NAME, traits: new string[] { CATEGORY1_TRAIT });
            npc.AddTrait(category: CATEGORY2_NAME, traits: new string[] { CATEGORY2_TRAIT });

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw) { Formatting = Formatting.Indented })
            {
                npc.ToJsonObject(writer, new List<string>() { CATEGORY1_NAME, CATEGORY2_NAME });

            }
            string json = sw.ToString();

            Dictionary<string, List<string>> traits = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);

            Assert.IsNotNull(traits, "Serialized using an unknown format");
            Assert.AreEqual(1, traits[CATEGORY1_NAME].Count, "Npc did not serialize category correctly");
            Assert.AreEqual(CATEGORY1_TRAIT, traits[CATEGORY1_NAME][0], "Npc did not serialize category correctly");
            Assert.AreEqual(1, traits[CATEGORY2_NAME].Count, "Npc did not serialize category correctly");
            Assert.AreEqual(CATEGORY2_TRAIT, traits[CATEGORY2_NAME][0], "Npc did not serialize category correctly");
        }

        [TestMethod]
        public void ExportMulitTraitJson()
        {
            const string CATEGORY1_NAME = "Colour";
            const string CATEGORY1_TRAIT1 = "Blue";
            const string CATEGORY1_TRAIT2 = "Green";
            const string CATEGORY2_NAME = "Terrain";
            const string CATEGORY2_TRAIT1 = "Hills";
            const string CATEGORY2_TRAIT2 = "River";

            Npc npc = new Npc();
            npc.AddTrait(category: CATEGORY1_NAME, traits: new string[] { CATEGORY1_TRAIT1, CATEGORY1_TRAIT2 });
            npc.AddTrait(category: CATEGORY2_NAME, traits: new string[] { CATEGORY2_TRAIT1, CATEGORY2_TRAIT2 });

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw) { Formatting = Formatting.Indented })
            {
                npc.ToJsonObject(writer, new List<string>() { CATEGORY1_NAME, CATEGORY2_NAME });

            }
            string json = sw.ToString();

            Dictionary<string, List<string>> traits = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);

            Assert.IsNotNull(traits, "Serialized using an unknown format");
            Assert.AreEqual(2, traits[CATEGORY1_NAME].Count, "Npc did not serialize category correctly");
            Assert.AreEqual(CATEGORY1_TRAIT1, traits[CATEGORY1_NAME][0], "Npc did not serialize category correctly");
            Assert.AreEqual(CATEGORY1_TRAIT2, traits[CATEGORY1_NAME][1], "Npc did not serialize category correctly");
            Assert.AreEqual(2, traits[CATEGORY2_NAME].Count, "Npc did not serialize category correctly");
            Assert.AreEqual(CATEGORY2_TRAIT1, traits[CATEGORY2_NAME][0], "Npc did not serialize category correctly");
            Assert.AreEqual(CATEGORY2_TRAIT2, traits[CATEGORY2_NAME][1], "Npc did not serialize category correctly");
        }

        [TestMethod]
        public void ExportNoTraitJson()
        {
            Npc npc = new Npc();

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw) { Formatting = Formatting.Indented })
            {
                npc.ToJsonObject(writer, new List<string>() { "Purpose" });

            }
            string json = sw.ToString();

            Dictionary<string, List<string>> traits = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);

            Assert.AreEqual(0, traits.Count, "Npc exported traits that it was not given");
        }

        [TestMethod]
        public void ExportSingleTraitStringArray()
        {
            Npc npc = new Npc();
            npc.AddTrait(category: "Colour", traits: new string[] { "Blue" });
            npc.AddTrait(category: "Animal", traits: new string[] { "Bear" });

            string[] traitsByCategories = npc.ToStringArrayByCategory(new List<string>() { "Colour", "Animal" });

            Assert.AreEqual(2, traitsByCategories.Length, "Wrong number of trait categories");
            Assert.AreEqual("Blue", traitsByCategories[0], "Npc did not generate expected CSV row");
            Assert.AreEqual("Bear", traitsByCategories[1], "Npc did not generate expected CSV row");
        }

        [TestMethod]
        public void ExportMultiTraitStringArray()
        {
            Npc npc = new Npc();
            npc.AddTrait(category: "Colour", traits: new string[] { "Blue", "Green" });
            npc.AddTrait(category: "Terrain", traits: new string[] { "Hills", "River" });

            string[] traitsByCategories = npc.ToStringArrayByCategory(new List<string>() { "Colour", "Terrain" });

            Assert.AreEqual(2, traitsByCategories.Length, "Wrong number of trait categories");
            Assert.AreEqual("Blue & Green", traitsByCategories[0], "Npc did not generate expected CSV row");
            Assert.AreEqual("Hills & River", traitsByCategories[1], "Npc did not generate expected CSV row");
        }

        [TestMethod]
        public void ExportNoTraitStringArray()
        {
            Npc npc = new Npc();

            string[] traitsByCategories = npc.ToStringArrayByCategory(new List<string>() { "Purpose" });

            Assert.AreEqual(0, traitsByCategories.Length, "Wrong number of trait categories");
        }
    }
}
