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
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class NpcToStringArrayTests
    {
        [TestMethod]
        public void SingleCategory()
        {
            const string TRAIT = "Blue";

            Npc npc = new Npc();
            npc.Add("Colour", new Npc.Trait[] { new Npc.Trait(TRAIT) });

            string[] categories = NpcToStringArray.Export(npc, new string[] { "Colour" });

            Assert.AreEqual(1, categories.Length, "Wrong number of categories");
            Assert.AreEqual(TRAIT, categories[0], "Wrong traits in category");
        }

        [TestMethod]
        public void SingleCategoryWithMultipleTraits()
        {
            const string TRAIT0 = "Blue";
            const string TRAIT1 = "Green";
            Npc npc = new Npc();
            npc.Add("Colour", new Npc.Trait[] { new Npc.Trait(TRAIT0), new Npc.Trait(TRAIT1) });

            string[] categories = NpcToStringArray.Export(npc, new string[] { "Colour" });

            Assert.AreEqual(1, categories.Length, "Wrong number of categories");
            Assert.AreEqual(TRAIT0 + ExportUtil.MULTI_TRAIT_SEPARATOR + TRAIT1, categories[0], "Wrong traits in category");
        }

        [TestMethod]
        public void MultipleCategories()
        {
            const string COLOUR_TRAIT = "Blue";
            const string ANIMAL_TRAIT = "Bear";

            Npc npc = new Npc();
            npc.Add("Colour", new Npc.Trait[] { new Npc.Trait(COLOUR_TRAIT) });
            npc.Add("Animal", new Npc.Trait[] { new Npc.Trait(ANIMAL_TRAIT) });

            string[] categories = NpcToStringArray.Export(npc, new string[] { "Colour", "Animal" });

            Assert.AreEqual(2, categories.Length, "Wrong number of categories");
            Assert.AreEqual(COLOUR_TRAIT, categories[0], "Wrong traits in category");
            Assert.AreEqual(ANIMAL_TRAIT, categories[1], "Wrong traits in category");
        }

        [TestMethod]
        public void ZeroCategories()
        {
            TraitSchema schema = new TraitSchema();

            NpcGroup npcGroup = NpcFactory.Create(schema, 0, new List<Replacement>(), m_random);
            Npc npc = new Npc();
            npcGroup.Add(npc);

            string[] categories = NpcToStringArray.Export(npcGroup.GetNpcAtIndex(0), npcGroup.CategoryOrder);

            Assert.AreEqual(0, categories.Length, "Wrong number of categories");
        }

        [TestMethod]
        public void NoTraits()
        {
            Npc npc = new Npc();

            string[] categories = NpcToStringArray.Export(npc, new List<string>() { "Purpose" });

            Assert.AreEqual(1, categories.Length, "Wrong number of trait categories");
            Assert.AreEqual(string.Empty, categories[0], "Wrong traits in category");
        }

        [TestMethod]
        public void NpcHasNoTraitsInCategoryOfMany()
        {
            const string COLOUR_CATEGORY = "Colour";
            const string COLOUR_TRAIT = "Blue";

            const string ANIMAL_CATEGORY = "Animal";

            const string AGE_CATEGORY = "Age";
            const string AGE_TRAIT = "Young";

            Npc npc = new Npc();
            npc.Add(COLOUR_CATEGORY, new Npc.Trait[] { new Npc.Trait(COLOUR_TRAIT) });
            npc.Add(AGE_CATEGORY, new Npc.Trait[] { new Npc.Trait(AGE_TRAIT) });

            string[] categories = NpcToStringArray.Export(npc, new List<string>() { COLOUR_CATEGORY, ANIMAL_CATEGORY, AGE_CATEGORY });

            Assert.AreEqual(3, categories.Length, "Wrong number of categories");
            Assert.AreEqual(COLOUR_TRAIT, categories[0], "Wrong traits in category");
            Assert.AreEqual(string.Empty, categories[1], "Wrong traits in category");
            Assert.AreEqual(AGE_TRAIT, categories[2], "Wrong traits in category");
        }

        [TestMethod]
        public void MultipleTraitsInMultipleCategories()
        {
            const string CATEGORY0_NAME = "Colour";
            const string CATEGORY0_TRAIT0 = "Blue";
            const string CATEGORY0_TRAIT1 = "Green";
            const string CATEGORY1_NAME = "Terrain";
            const string CATEGORY1_TRAIT0 = "Hills";
            const string CATEGORY1_TRAIT1 = "River";

            Npc npc = new Npc();
            npc.Add(category: CATEGORY0_NAME, traits: new Npc.Trait[] { new Npc.Trait(CATEGORY0_TRAIT0), new Npc.Trait(CATEGORY0_TRAIT1) });
            npc.Add(category: CATEGORY1_NAME, traits: new Npc.Trait[] { new Npc.Trait(CATEGORY1_TRAIT0), new Npc.Trait(CATEGORY1_TRAIT1) });

            NpcGroup npcGroup = new NpcGroup(new List<string> { CATEGORY0_NAME, CATEGORY1_NAME });
            npcGroup.Add(npc);

            string[] categories = NpcToStringArray.Export(npc, new List<string>() { CATEGORY0_NAME, CATEGORY1_NAME });

            Assert.AreEqual(2, categories.Length, "Wrong number of categories");
            Assert.AreEqual(CATEGORY0_TRAIT0 + ExportUtil.MULTI_TRAIT_SEPARATOR + CATEGORY0_TRAIT1, categories[0], "Wrong traits in category");
            Assert.AreEqual(CATEGORY1_TRAIT0 + ExportUtil.MULTI_TRAIT_SEPARATOR + CATEGORY1_TRAIT1, categories[1], "Wrong traits in category");
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

            string[] categories = NpcToStringArray.Export(npc, new List<string>() { NOT_FOUND_CATEGORY, CATEGORY });

            Assert.AreEqual(2, categories.Length, "Wrong number of categories");
        }

        MockRandom m_random = new MockRandom();
    }
}
