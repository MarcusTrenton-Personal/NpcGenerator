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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using NpcGenerator;
using System.Collections.Generic;
using System.IO;

namespace Tests
{
    public class DeserializedNpcGroup
    {
        public List<Dictionary<string, List<string>>> npc_group;
    }

    [TestClass]
    public class NpcToJsonTests
    {
        const string SCHEMA_PATH = "NpcGroupSchema.json";

        [TestMethod]
        public void SingleNpc()
        {
            const string CATEGORY = "Colour";
            const string TRAIT = "Blue";
            NpcGroup npcGroup = new NpcGroup(new List<string> { CATEGORY });

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT) });
            npcGroup.Add(npc);

            NpcToJson npcToJson = new NpcToJson(SCHEMA_PATH);
            string jsonText = npcToJson.Export(npcGroup);

            DeserializedNpcGroup result = JsonConvert.DeserializeObject<DeserializedNpcGroup>(jsonText);

            Assert.IsNotNull(result, "Serialized using an unknown format");
            Assert.AreEqual(1, result.npc_group.Count, "Wrong number of npcs");
            Assert.AreEqual(1, result.npc_group[0][CATEGORY].Count, "Npc did not serialize category correctly");
            Assert.AreEqual(TRAIT, result.npc_group[0][CATEGORY][0], "Npc did not serialize category correctly");
        }

        [TestMethod]
        public void MultipleNpcs()
        {
            const string CATEGORY = "Colour";
            const string TRAIT = "Blue";
            NpcGroup npcGroup = new NpcGroup(new List<string> { CATEGORY });

            Npc npc0 = new Npc();
            npc0.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT) });
            npcGroup.Add(npc0);

            Npc npc1 = new Npc();
            npc1.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT) });
            npcGroup.Add(npc1);

            NpcToJson npcToJson = new NpcToJson(SCHEMA_PATH);
            string jsonText = npcToJson.Export(npcGroup);

            DeserializedNpcGroup result = JsonConvert.DeserializeObject<DeserializedNpcGroup>(jsonText);

            Assert.IsNotNull(result, "Serialized using an unknown format");
            Assert.AreEqual(2, result.npc_group.Count, "Wrong number of npcs");
            Assert.AreEqual(1, result.npc_group[0][CATEGORY].Count, "Npc did not serialize category correctly");
            Assert.AreEqual(TRAIT, result.npc_group[0][CATEGORY][0], "Npc did not serialize category correctly");
            Assert.AreEqual(1, result.npc_group[1][CATEGORY].Count, "Npc did not serialize category correctly");
            Assert.AreEqual(TRAIT, result.npc_group[1][CATEGORY][0], "Npc did not serialize category correctly");
        }

        [TestMethod]
        public void ZeroNpcs()
        {
            NpcGroup npcGroup = new NpcGroup(new List<string> { "Colour" });

            NpcToJson npcToJson = new NpcToJson(SCHEMA_PATH);
            string jsonText = npcToJson.Export(npcGroup);

            DeserializedNpcGroup result = JsonConvert.DeserializeObject<DeserializedNpcGroup>(jsonText);

            Assert.IsNotNull(result, "Serialized using an unknown format");
            Assert.AreEqual(0, result.npc_group.Count, "Wrong number of npcs");
        }

        public void EmptyNpc()
        {
            const string CATEGORY = "Colour";
            NpcGroup npcGroup = new NpcGroup(new List<string> { CATEGORY });

            Npc npc = new Npc();
            npcGroup.Add(npc);

            NpcToJson npcToJson = new NpcToJson(SCHEMA_PATH);
            string jsonText = npcToJson.Export(npcGroup);

            DeserializedNpcGroup result = JsonConvert.DeserializeObject<DeserializedNpcGroup>(jsonText);

            Assert.IsNotNull(result, "Serialized using an unknown format");
            Assert.AreEqual(1, result.npc_group.Count, "Wrong number of npcs");
            Assert.AreEqual(0, result.npc_group[0][CATEGORY].Count, "Npc did not serialize category correctly");
        }

        [TestMethod]
        public void MultipleCategories()
        {
            const string CATEGORY0_NAME = "Colour";
            const string CATEGORY0_TRAIT = "Blue";
            const string CATEGORY1_NAME = "Animal";
            const string CATEGORY1_TRAIT = "Bear";

            Npc npc = new Npc();
            npc.Add(category: CATEGORY0_NAME, traits: new Npc.Trait[] { new Npc.Trait(CATEGORY0_TRAIT) });
            npc.Add(category: CATEGORY1_NAME, traits: new Npc.Trait[] { new Npc.Trait(CATEGORY1_TRAIT) });

            NpcGroup npcGroup = new NpcGroup(new List<string> { CATEGORY0_NAME, CATEGORY1_NAME });
            npcGroup.Add(npc);

            NpcToJson npcToJson = new NpcToJson(SCHEMA_PATH);
            string jsonText = npcToJson.Export(npcGroup);

            DeserializedNpcGroup result = JsonConvert.DeserializeObject<DeserializedNpcGroup>(jsonText);

            Assert.IsNotNull(result, "Serialized using an unknown format");
            Assert.AreEqual(1, result.npc_group.Count, "Wrong number of npcs");
            Assert.AreEqual(1, result.npc_group[0][CATEGORY0_NAME].Count, "Npc did not serialize category correctly");
            Assert.AreEqual(CATEGORY0_TRAIT, result.npc_group[0][CATEGORY0_NAME][0], "Npc did not serialize category correctly");
            Assert.AreEqual(1, result.npc_group[0][CATEGORY1_NAME].Count, "Npc did not serialize category correctly");
            Assert.AreEqual(CATEGORY1_TRAIT, result.npc_group[0][CATEGORY1_NAME][0], "Npc did not serialize category correctly");
        }

        [TestMethod]
        public void MultipleTraitsInMultipleCategories()
        {
            const string CATEGORY0_NAME = "Colour";
            const string CATEGORY0_TRAIT0 = "Blue";
            const string CATEGORY0_TRAIT1 = "Green";
            const string CATEGORY1_NAME = "Terrain";
            const string CATEGORY1_TRAIT0 = "Hills";
            const string CATEGORY2_TRAIT1 = "River";

            Npc npc = new Npc();
            npc.Add(category: CATEGORY0_NAME, traits: new Npc.Trait[] { new Npc.Trait(CATEGORY0_TRAIT0), new Npc.Trait(CATEGORY0_TRAIT1) });
            npc.Add(category: CATEGORY1_NAME, traits: new Npc.Trait[] { new Npc.Trait(CATEGORY1_TRAIT0), new Npc.Trait(CATEGORY2_TRAIT1) });

            NpcGroup npcGroup = new NpcGroup(new List<string> { CATEGORY0_NAME, CATEGORY1_NAME });
            npcGroup.Add(npc);

            NpcToJson npcToJson = new NpcToJson(SCHEMA_PATH);
            string jsonText = npcToJson.Export(npcGroup);

            DeserializedNpcGroup result = JsonConvert.DeserializeObject<DeserializedNpcGroup>(jsonText);

            Assert.IsNotNull(result, "Serialized using an unknown format");
            Assert.AreEqual(1, result.npc_group.Count, "Wrong number of npcs");
            Assert.AreEqual(2, result.npc_group[0][CATEGORY0_NAME].Count, "Npc did not serialize category correctly");
            Assert.AreEqual(CATEGORY0_TRAIT0, result.npc_group[0][CATEGORY0_NAME][0], "Npc did not serialize category correctly");
            Assert.AreEqual(CATEGORY0_TRAIT1, result.npc_group[0][CATEGORY0_NAME][1], "Npc did not serialize category correctly");
            Assert.AreEqual(2, result.npc_group[0][CATEGORY1_NAME].Count, "Npc did not serialize category correctly");
            Assert.AreEqual(CATEGORY1_TRAIT0, result.npc_group[0][CATEGORY1_NAME][0], "Npc did not serialize category correctly");
            Assert.AreEqual(CATEGORY2_TRAIT1, result.npc_group[0][CATEGORY1_NAME][1], "Npc did not serialize category correctly");
        }

        [TestMethod]
        public void OrderHasUnknownCategories()
        {
            const string CATEGORY = "Colour";
            const string NOT_FOUND_CATEGORY = "Animal";
            const string TRAIT = "Blue";
            NpcGroup npcGroup = new NpcGroup(new List<string> { NOT_FOUND_CATEGORY, CATEGORY });

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT) });
            npcGroup.Add(npc);

            NpcToJson npcToJson = new NpcToJson(SCHEMA_PATH);
            string jsonText = npcToJson.Export(npcGroup);

            DeserializedNpcGroup result = JsonConvert.DeserializeObject<DeserializedNpcGroup>(jsonText);

            Assert.IsNotNull(result, "Serialized using an unknown format");
            Assert.AreEqual(1, result.npc_group.Count, "Wrong number of npcs");
            Assert.AreEqual(1, result.npc_group[0][CATEGORY].Count, "Npc did not serialize category correctly");
            Assert.AreEqual(TRAIT, result.npc_group[0][CATEGORY][0], "Npc did not serialize category correctly");
        }

        [TestMethod]
        public void SchemaIsOptional()
        {
            const string CATEGORY = "Colour";
            const string TRAIT = "Blue";
            NpcGroup npcGroup = new NpcGroup(new List<string> { CATEGORY });

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT) });
            npcGroup.Add(npc);

            NpcToJson npcToJson = new NpcToJson(null);
            string jsonText = npcToJson.Export(npcGroup);

            DeserializedNpcGroup result = JsonConvert.DeserializeObject<DeserializedNpcGroup>(jsonText);

            Assert.IsNotNull(result, "Serialized using an unknown format");
            Assert.AreEqual(1, result.npc_group.Count, "Wrong number of npcs");
            Assert.AreEqual(1, result.npc_group[0][CATEGORY].Count, "Npc did not serialize category correctly");
            Assert.AreEqual(TRAIT, result.npc_group[0][CATEGORY][0], "Npc did not serialize category correctly");
        }
    }
}
