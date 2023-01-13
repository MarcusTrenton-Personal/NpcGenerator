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
using NpcGenerator;
using System;
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

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void EmptySchema()
        {
            new NpcToJson(string.Empty);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void EmptySchemaPathIsInvalid()
        {
            new NpcToJson("This is not a file path!");
        }

        [TestMethod, ExpectedException(typeof(FileNotFoundException))]
        public void EmptySchemaPathIsNotFound()
        {
            new NpcToJson("NotFound.json");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void NullNpcGroup()
        {
            NpcToJson npcToJson = new NpcToJson(SCHEMA_PATH);
            npcToJson.Export(null);
        }

        [TestMethod]
        public void SingleNpc()
        {
            const string CATEGORY = "Colour";
            const string TRAIT = "Blue";
            NpcGroup npcGroup = new NpcGroup(new List<NpcGroup.Category> { new NpcGroup.Category(CATEGORY) });

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT, CATEGORY) });
            npcGroup.Add(npc);

            NpcToJson npcToJson = new NpcToJson(SCHEMA_PATH);
            string jsonText = npcToJson.Export(npcGroup);

            DeserializedNpcGroup result = JsonConvert.DeserializeObject<DeserializedNpcGroup>(jsonText);

            Assert.IsNotNull(result, "Serialized using an unknown format");
            Assert.AreEqual(1, result.npc_group.Count, "Wrong number of npcs");
            Assert.AreEqual(1, result.npc_group[0].Count, "Wrong number of categories");
            Assert.AreEqual(1, result.npc_group[0][CATEGORY].Count, "Npc did not serialize category correctly");
            Assert.AreEqual(TRAIT, result.npc_group[0][CATEGORY][0], "Npc did not serialize category correctly");
        }

        [TestMethod]
        public void MultipleNpcs()
        {
            const string CATEGORY = "Colour";
            const string TRAIT = "Blue";
            NpcGroup npcGroup = new NpcGroup(new List<NpcGroup.Category> { new NpcGroup.Category(CATEGORY) });

            Npc npc0 = new Npc();
            npc0.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT, CATEGORY) });
            npcGroup.Add(npc0);

            Npc npc1 = new Npc();
            npc1.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT, CATEGORY) });
            npcGroup.Add(npc1);

            NpcToJson npcToJson = new NpcToJson(SCHEMA_PATH);
            string jsonText = npcToJson.Export(npcGroup);

            DeserializedNpcGroup result = JsonConvert.DeserializeObject<DeserializedNpcGroup>(jsonText);

            Assert.IsNotNull(result, "Serialized using an unknown format");
            Assert.AreEqual(2, result.npc_group.Count, "Wrong number of npcs");
            Assert.AreEqual(1, result.npc_group[0].Count, "Wrong number of categories");
            Assert.AreEqual(1, result.npc_group[0][CATEGORY].Count, "Npc did not serialize category correctly");
            Assert.AreEqual(TRAIT, result.npc_group[0][CATEGORY][0], "Npc did not serialize category correctly");
            Assert.AreEqual(1, result.npc_group[1].Count, "Wrong number of categories");
            Assert.AreEqual(1, result.npc_group[1][CATEGORY].Count, "Npc did not serialize category correctly");
            Assert.AreEqual(TRAIT, result.npc_group[1][CATEGORY][0], "Npc did not serialize category correctly");
        }

        [TestMethod]
        public void ZeroNpcs()
        {
            NpcGroup npcGroup = new NpcGroup(new List<NpcGroup.Category> { new NpcGroup.Category("Colour") });

            NpcToJson npcToJson = new NpcToJson(SCHEMA_PATH);
            string jsonText = npcToJson.Export(npcGroup);

            DeserializedNpcGroup result = JsonConvert.DeserializeObject<DeserializedNpcGroup>(jsonText);

            Assert.IsNotNull(result, "Serialized using an unknown format");
            Assert.AreEqual(0, result.npc_group.Count, "Wrong number of npcs");
        }

        public void EmptyNpc()
        {
            const string CATEGORY = "Colour";
            NpcGroup npcGroup = new NpcGroup(new List<NpcGroup.Category> { new NpcGroup.Category(CATEGORY) });

            Npc npc = new Npc();
            npcGroup.Add(npc);

            NpcToJson npcToJson = new NpcToJson(SCHEMA_PATH);
            string jsonText = npcToJson.Export(npcGroup);

            DeserializedNpcGroup result = JsonConvert.DeserializeObject<DeserializedNpcGroup>(jsonText);

            Assert.IsNotNull(result, "Serialized using an unknown format");
            Assert.AreEqual(1, result.npc_group.Count, "Wrong number of npcs");
            Assert.AreEqual(1, result.npc_group[0].Count, "Wrong number of categories");
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
            npc.Add(category: CATEGORY0_NAME, traits: new Npc.Trait[] { new Npc.Trait(CATEGORY0_TRAIT, CATEGORY0_NAME) });
            npc.Add(category: CATEGORY1_NAME, traits: new Npc.Trait[] { new Npc.Trait(CATEGORY1_TRAIT, CATEGORY0_NAME) });

            NpcGroup npcGroup = new NpcGroup(new List<NpcGroup.Category> {
                new NpcGroup.Category(CATEGORY0_NAME), new NpcGroup.Category(CATEGORY1_NAME) });
            npcGroup.Add(npc);

            NpcToJson npcToJson = new NpcToJson(SCHEMA_PATH);
            string jsonText = npcToJson.Export(npcGroup);

            DeserializedNpcGroup result = JsonConvert.DeserializeObject<DeserializedNpcGroup>(jsonText);

            Assert.IsNotNull(result, "Serialized using an unknown format");
            Assert.AreEqual(1, result.npc_group.Count, "Wrong number of npcs");
            Assert.AreEqual(2, result.npc_group[0].Count, "Wrong number of categories");
            Assert.AreEqual(1, result.npc_group[0][CATEGORY0_NAME].Count, "Npc did not serialize category correctly");
            Assert.AreEqual(CATEGORY0_TRAIT, result.npc_group[0][CATEGORY0_NAME][0], "Npc did not serialize category correctly");
            Assert.AreEqual(1, result.npc_group[0][CATEGORY1_NAME].Count, "Npc did not serialize category correctly");
            Assert.AreEqual(CATEGORY1_TRAIT, result.npc_group[0][CATEGORY1_NAME][0], "Npc did not serialize category correctly");
        }

        [TestMethod]
        public void MultipleTraitsInMultipleCategories()
        {
            const string C0_NAME = "Colour";
            const string C0T0 = "Blue";
            const string C0T1 = "Green";
            const string C1_NAME = "Terrain";
            const string C1T0 = "Hills";
            const string C1T1 = "River";

            Npc npc = new Npc();
            npc.Add(category: C0_NAME, traits: new Npc.Trait[] { new Npc.Trait(C0T0, C0_NAME), new Npc.Trait(C0T1, C0_NAME) });
            npc.Add(category: C1_NAME, traits: new Npc.Trait[] { new Npc.Trait(C1T0, C1_NAME), new Npc.Trait(C1T1, C1_NAME) });

            NpcGroup npcGroup = new NpcGroup(new List<NpcGroup.Category> {
                new NpcGroup.Category(C0_NAME), new NpcGroup.Category(C1_NAME) });
            npcGroup.Add(npc);

            NpcToJson npcToJson = new NpcToJson(SCHEMA_PATH);
            string jsonText = npcToJson.Export(npcGroup);

            DeserializedNpcGroup result = JsonConvert.DeserializeObject<DeserializedNpcGroup>(jsonText);

            Assert.IsNotNull(result, "Serialized using an unknown format");
            Assert.AreEqual(1, result.npc_group.Count, "Wrong number of npcs");
            Assert.AreEqual(2, result.npc_group[0].Count, "Wrong number of categories");
            Assert.AreEqual(2, result.npc_group[0][C0_NAME].Count, "Npc did not serialize category correctly");
            Assert.AreEqual(C0T0, result.npc_group[0][C0_NAME][0], "Npc did not serialize category correctly");
            Assert.AreEqual(C0T1, result.npc_group[0][C0_NAME][1], "Npc did not serialize category correctly");
            Assert.AreEqual(2, result.npc_group[0][C1_NAME].Count, "Npc did not serialize category correctly");
            Assert.AreEqual(C1T0, result.npc_group[0][C1_NAME][0], "Npc did not serialize category correctly");
            Assert.AreEqual(C1T1, result.npc_group[0][C1_NAME][1], "Npc did not serialize category correctly");
        }

        [TestMethod]
        public void OrderHasUnknownCategories()
        {
            const string CATEGORY = "Colour";
            const string NOT_FOUND_CATEGORY = "Animal";
            const string TRAIT = "Blue";
            NpcGroup npcGroup = new NpcGroup(new List<NpcGroup.Category> {
                new NpcGroup.Category(NOT_FOUND_CATEGORY), new NpcGroup.Category(CATEGORY) });

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT, CATEGORY) });
            npcGroup.Add(npc);

            NpcToJson npcToJson = new NpcToJson(SCHEMA_PATH);
            string jsonText = npcToJson.Export(npcGroup);

            DeserializedNpcGroup result = JsonConvert.DeserializeObject<DeserializedNpcGroup>(jsonText);

            Assert.IsNotNull(result, "Serialized using an unknown format");
            Assert.AreEqual(1, result.npc_group.Count, "Wrong number of npcs");
            Assert.AreEqual(1, result.npc_group[0].Count, "Wrong number of categories");
            Assert.AreEqual(1, result.npc_group[0][CATEGORY].Count, "Npc did not serialize category correctly");
            Assert.AreEqual(TRAIT, result.npc_group[0][CATEGORY][0], "Npc did not serialize category correctly");
        }

        [TestMethod]
        public void SchemaIsOptional()
        {
            const string CATEGORY = "Colour";
            const string TRAIT = "Blue";
            NpcGroup npcGroup = new NpcGroup(new List<NpcGroup.Category> { new NpcGroup.Category(CATEGORY) });

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT, CATEGORY) });
            npcGroup.Add(npc);

            NpcToJson npcToJson = new NpcToJson(null);
            string jsonText = npcToJson.Export(npcGroup);

            DeserializedNpcGroup result = JsonConvert.DeserializeObject<DeserializedNpcGroup>(jsonText);

            Assert.IsNotNull(result, "Serialized using an unknown format");
            Assert.AreEqual(1, result.npc_group.Count, "Wrong number of npcs");
            Assert.AreEqual(1, result.npc_group[0].Count, "Wrong number of categories");
            Assert.AreEqual(1, result.npc_group[0][CATEGORY].Count, "Npc did not serialize category correctly");
            Assert.AreEqual(TRAIT, result.npc_group[0][CATEGORY][0], "Npc did not serialize category correctly");
        }

        [TestMethod]
        public void HiddenTrait()
        {
            const string CATEGORY = "Colour";
            const string TRAIT = "Blue";
            NpcGroup npcGroup = new NpcGroup(new List<NpcGroup.Category> { new NpcGroup.Category(CATEGORY) });

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT, CATEGORY, isHidden: true) });
            npcGroup.Add(npc);

            NpcToJson npcToJson = new NpcToJson(null);
            string jsonText = npcToJson.Export(npcGroup);

            DeserializedNpcGroup result = JsonConvert.DeserializeObject<DeserializedNpcGroup>(jsonText);

            Assert.IsNotNull(result, "Serialized using an unknown format");
            Assert.AreEqual(1, result.npc_group.Count, "Wrong number of npcs");
            Assert.AreEqual(1, result.npc_group[0].Count, "Wrong number of categories");
            Assert.AreEqual(0, result.npc_group[0][CATEGORY].Count, "Npc did not serialize category correctly");
        }

        [TestMethod]
        public void VisibleTraitThenHiddenTrait()
        {
            const string CATEGORY = "Colour";
            const string VISIBLE_TRAIT = "Blue";
            const string HIDDEN_TRAIT = "Red";
            NpcGroup npcGroup = new NpcGroup(new List<NpcGroup.Category> { new NpcGroup.Category(CATEGORY) });

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(VISIBLE_TRAIT, CATEGORY), new Npc.Trait(HIDDEN_TRAIT, CATEGORY, isHidden: true) });
            npcGroup.Add(npc);

            NpcToJson npcToJson = new NpcToJson(null);
            string jsonText = npcToJson.Export(npcGroup);

            DeserializedNpcGroup result = JsonConvert.DeserializeObject<DeserializedNpcGroup>(jsonText);

            Assert.IsNotNull(result, "Serialized using an unknown format");
            Assert.AreEqual(1, result.npc_group.Count, "Wrong number of npcs");
            Assert.AreEqual(1, result.npc_group[0].Count, "Wrong number of categories");
            Assert.AreEqual(1, result.npc_group[0][CATEGORY].Count, "Npc did not serialize category correctly");
            Assert.AreEqual(VISIBLE_TRAIT, result.npc_group[0][CATEGORY][0], "Npc did not serialize category correctly");
        }

        [TestMethod]
        public void NoDuplicateTraits()
        {
            const string CATEGORY = "Colour";
            const string TRAIT = "Blue";
            NpcGroup npcGroup = new NpcGroup(new List<NpcGroup.Category> { new NpcGroup.Category(CATEGORY) });

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT, CATEGORY), new Npc.Trait(TRAIT, CATEGORY) });
            npcGroup.Add(npc);

            NpcToJson npcToJson = new NpcToJson(null);
            string jsonText = npcToJson.Export(npcGroup);

            DeserializedNpcGroup result = JsonConvert.DeserializeObject<DeserializedNpcGroup>(jsonText);

            Assert.IsNotNull(result, "Serialized using an unknown format");
            Assert.AreEqual(1, result.npc_group.Count, "Wrong number of npcs");
            Assert.AreEqual(1, result.npc_group[0].Count, "Wrong number of categories");
            Assert.AreEqual(1, result.npc_group[0][CATEGORY].Count, "Npc did not serialize category correctly");
            Assert.AreEqual(TRAIT, result.npc_group[0][CATEGORY][0], "Npc did not serialize category correctly");
        }

        [TestMethod]
        public void HiddenCategory()
        {
            const string HIDDEN_CATEGORY_NAME = "Colour";
            const string C0T0 = "Blue";
            const string C0T1 = "Green";
            const string VISIBLE_CATEGORY_NAME = "Terrain";
            const string C1T0 = "Hills";
            const string C1T1 = "River";

            Npc npc = new Npc();
            npc.Add(category: HIDDEN_CATEGORY_NAME, traits: new Npc.Trait[] { 
                new Npc.Trait(C0T0, HIDDEN_CATEGORY_NAME), new Npc.Trait(C0T1, HIDDEN_CATEGORY_NAME) });
            npc.Add(category: VISIBLE_CATEGORY_NAME, traits: new Npc.Trait[] { 
                new Npc.Trait(C1T0, VISIBLE_CATEGORY_NAME), new Npc.Trait(C1T1, VISIBLE_CATEGORY_NAME) });

            NpcGroup npcGroup = new NpcGroup(new List<NpcGroup.Category> {
                new NpcGroup.Category(HIDDEN_CATEGORY_NAME, isHidden: true), new NpcGroup.Category(VISIBLE_CATEGORY_NAME) });
            npcGroup.Add(npc);

            NpcToJson npcToJson = new NpcToJson(null);
            string jsonText = npcToJson.Export(npcGroup);

            DeserializedNpcGroup result = JsonConvert.DeserializeObject<DeserializedNpcGroup>(jsonText);

            Assert.IsNotNull(result, "Serialized using an unknown format");
            Assert.AreEqual(1, result.npc_group.Count, "Wrong number of npcs");
            Assert.AreEqual(1, result.npc_group[0].Count, "Wrong number of categories");
            Assert.AreEqual(2, result.npc_group[0][VISIBLE_CATEGORY_NAME].Count, "Npc did not serialize category correctly");
            Assert.AreEqual(C1T0, result.npc_group[0][VISIBLE_CATEGORY_NAME][0], "Npc did not serialize category correctly");
            Assert.AreEqual(C1T1, result.npc_group[0][VISIBLE_CATEGORY_NAME][1], "Npc did not serialize category correctly");
        }

        [TestMethod]
        public void AllCategoriesHidden()
        {
            const string C0_NAME = "Colour";
            const string C0T0 = "Blue";
            const string C0T1 = "Green";
            const string C1_NAME = "Terrain";
            const string C1T0 = "Hills";
            const string C1T1 = "River";

            Npc npc = new Npc();
            npc.Add(category: C0_NAME, traits: new Npc.Trait[] { new Npc.Trait(C0T0, C0_NAME), new Npc.Trait(C0T1, C0_NAME) });
            npc.Add(category: C1_NAME, traits: new Npc.Trait[] { new Npc.Trait(C1T0, C1_NAME), new Npc.Trait(C1T1, C1_NAME) });

            NpcGroup npcGroup = new NpcGroup(new List<NpcGroup.Category> {
                new NpcGroup.Category(C0_NAME, isHidden: true), new NpcGroup.Category(C1_NAME, isHidden: true) });
            npcGroup.Add(npc);

            NpcToJson npcToJson = new NpcToJson(null);
            string jsonText = npcToJson.Export(npcGroup);

            DeserializedNpcGroup result = JsonConvert.DeserializeObject<DeserializedNpcGroup>(jsonText);

            string expectedCsv = string.Empty;

            Assert.IsNotNull(result, "Serialized using an unknown format");
            Assert.AreEqual(1, result.npc_group.Count, "Wrong number of npcs");
            Assert.AreEqual(0, result.npc_group[0].Count, "Wrong number of categories");
        }

        [TestMethod]
        public void CategoryIsHiddenInOneNpcAndAbsentFromAnother()
        {
            const string C0_NAME = "Colour";
            const string C0T0 = "Blue";
            const string C0T1 = "Green";
            const string C1_NAME = "Terrain";
            const string C1T0 = "Hills";
            const string C1T1 = "River";

            Npc npc0 = new Npc();
            npc0.Add(category: C0_NAME, traits: new Npc.Trait[] { new Npc.Trait(C0T0, C0_NAME), new Npc.Trait(C0T1, C0_NAME) });
            npc0.Add(category: C1_NAME, traits: new Npc.Trait[] { new Npc.Trait(C1T0, C1_NAME), new Npc.Trait(C1T1, C1_NAME) });

            Npc npc1 = new Npc();
            npc1.Add(category: C1_NAME, traits: new Npc.Trait[] { new Npc.Trait(C1T0, C1_NAME), new Npc.Trait(C1T1, C1_NAME) });

            NpcGroup npcGroup = new NpcGroup(new List<NpcGroup.Category> {
                new NpcGroup.Category(C0_NAME, isHidden : true), new NpcGroup.Category(C1_NAME) });
            npcGroup.Add(npc0);
            npcGroup.Add(npc1);

            NpcToJson npcToJson = new NpcToJson(null);
            string jsonText = npcToJson.Export(npcGroup);

            DeserializedNpcGroup result = JsonConvert.DeserializeObject<DeserializedNpcGroup>(jsonText);

            Assert.IsNotNull(result, "Serialized using an unknown format");
            Assert.AreEqual(2, result.npc_group.Count, "Wrong number of npcs");
            Assert.AreEqual(1, result.npc_group[0].Count, "Wrong number of categories");
            Assert.AreEqual(2, result.npc_group[0][C1_NAME].Count, "Npc did not serialize category correctly");
            Assert.AreEqual(C1T0, result.npc_group[0][C1_NAME][0], "Npc did not serialize category correctly");
            Assert.AreEqual(C1T1, result.npc_group[0][C1_NAME][1], "Npc did not serialize category correctly");
            Assert.AreEqual(1, result.npc_group[1].Count, "Wrong number of categories");
            Assert.AreEqual(2, result.npc_group[1][C1_NAME].Count, "Npc did not serialize category correctly");
            Assert.AreEqual(C1T0, result.npc_group[1][C1_NAME][0], "Npc did not serialize category correctly");
            Assert.AreEqual(C1T1, result.npc_group[1][C1_NAME][1], "Npc did not serialize category correctly");
        }

        [TestMethod]
        public void CategoryIsVisibleInOneNpcAndAbsentFromAnother()
        {
            const string C0_NAME = "Colour";
            const string C0T0 = "Blue";
            const string C0T1 = "Green";
            const string C1_NAME = "Terrain";
            const string C1T0 = "Hills";
            const string C1T1 = "River";

            Npc npc0 = new Npc();
            npc0.Add(category: C0_NAME, traits: new Npc.Trait[] { new Npc.Trait(C0T0, C0_NAME), new Npc.Trait(C0T1, C0_NAME) });
            npc0.Add(category: C1_NAME, traits: new Npc.Trait[] { new Npc.Trait(C1T0, C1_NAME), new Npc.Trait(C1T1, C1_NAME) });

            Npc npc1 = new Npc();
            npc1.Add(category: C1_NAME, traits: new Npc.Trait[] { new Npc.Trait(C1T0, C1_NAME), new Npc.Trait(C1T1, C1_NAME) });

            NpcGroup npcGroup = new NpcGroup(new List<NpcGroup.Category> { new NpcGroup.Category(C0_NAME), new NpcGroup.Category(C1_NAME) });
            npcGroup.Add(npc0);
            npcGroup.Add(npc1);

            NpcToJson npcToJson = new NpcToJson(null);
            string jsonText = npcToJson.Export(npcGroup);

            DeserializedNpcGroup result = JsonConvert.DeserializeObject<DeserializedNpcGroup>(jsonText);

            Assert.IsNotNull(result, "Serialized using an unknown format");
            Assert.AreEqual(2, result.npc_group.Count, "Wrong number of npcs");
            Assert.AreEqual(2, result.npc_group[0].Count, "Wrong number of categories");
            Assert.AreEqual(2, result.npc_group[0][C0_NAME].Count, "Npc did not serialize category correctly");
            Assert.AreEqual(C0T0, result.npc_group[0][C0_NAME][0], "Npc did not serialize category correctly");
            Assert.AreEqual(C0T1, result.npc_group[0][C0_NAME][1], "Npc did not serialize category correctly");
            Assert.AreEqual(2, result.npc_group[0][C1_NAME].Count, "Npc did not serialize category correctly");
            Assert.AreEqual(C1T0, result.npc_group[0][C1_NAME][0], "Npc did not serialize category correctly");
            Assert.AreEqual(C1T1, result.npc_group[0][C1_NAME][1], "Npc did not serialize category correctly");
            Assert.AreEqual(1, result.npc_group[1].Count, "Wrong number of categories");
            Assert.AreEqual(2, result.npc_group[1][C1_NAME].Count, "Npc did not serialize category correctly");
            Assert.AreEqual(C1T0, result.npc_group[1][C1_NAME][0], "Npc did not serialize category correctly");
            Assert.AreEqual(C1T1, result.npc_group[1][C1_NAME][1], "Npc did not serialize category correctly");
        }
    }
}
