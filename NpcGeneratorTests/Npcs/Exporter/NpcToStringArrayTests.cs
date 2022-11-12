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
            const string CATEGORY = "Colour";
            const string TRAIT = "Blue";

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT, CATEGORY) });

            string[] categories = NpcToStringArray.Export(npc, new string[] { "Colour" });

            Assert.AreEqual(1, categories.Length, "Wrong number of categories");
            Assert.AreEqual(TRAIT, categories[0], "Wrong traits in category");
        }

        [TestMethod]
        public void SingleCategoryWithMultipleTraits()
        {
            const string CATEGORY = "Colour";
            const string TRAIT0 = "Blue";
            const string TRAIT1 = "Green";
            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT0, CATEGORY), new Npc.Trait(TRAIT1, CATEGORY) });

            string[] categories = NpcToStringArray.Export(npc, new string[] { "Colour" });

            Assert.AreEqual(1, categories.Length, "Wrong number of categories");
            Assert.AreEqual(TRAIT0 + NpcToStringArray.MULTI_TRAIT_SEPARATOR + TRAIT1, categories[0], "Wrong traits in category");
        }

        [TestMethod]
        public void MultipleCategories()
        {
            const string COLOUR_CATEGORY = "Colour";
            const string COLOUR_TRAIT = "Blue";
            const string ANIMAL_CATEGORY = "Animal";
            const string ANIMAL_TRAIT = "Bear";

            Npc npc = new Npc();
            npc.Add(COLOUR_CATEGORY, new Npc.Trait[] { new Npc.Trait(COLOUR_TRAIT, COLOUR_CATEGORY) });
            npc.Add(ANIMAL_CATEGORY, new Npc.Trait[] { new Npc.Trait(ANIMAL_TRAIT, ANIMAL_CATEGORY) });

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

            string[] categories = NpcToStringArray.Export(npcGroup.GetNpcAtIndex(0), npcGroup.VisibleCategoryOrder);

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
            npc.Add(COLOUR_CATEGORY, new Npc.Trait[] { new Npc.Trait(COLOUR_TRAIT, COLOUR_CATEGORY) });
            npc.Add(AGE_CATEGORY, new Npc.Trait[] { new Npc.Trait(AGE_TRAIT, AGE_CATEGORY) });

            string[] categories = NpcToStringArray.Export(npc, new List<string>() { COLOUR_CATEGORY, ANIMAL_CATEGORY, AGE_CATEGORY });

            Assert.AreEqual(3, categories.Length, "Wrong number of categories");
            Assert.AreEqual(COLOUR_TRAIT, categories[0], "Wrong traits in category");
            Assert.AreEqual(string.Empty, categories[1], "Wrong traits in category");
            Assert.AreEqual(AGE_TRAIT, categories[2], "Wrong traits in category");
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

            string[] categories = NpcToStringArray.Export(npc, npcGroup.VisibleCategoryOrder);

            Assert.AreEqual(2, categories.Length, "Wrong number of categories");
            Assert.AreEqual(C0T0 + NpcToStringArray.MULTI_TRAIT_SEPARATOR + C0T1, categories[0], "Wrong traits in category");
            Assert.AreEqual(C1T0 + NpcToStringArray.MULTI_TRAIT_SEPARATOR + C1T1, categories[1], "Wrong traits in category");
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

            string[] categories = NpcToStringArray.Export(npc, npcGroup.VisibleCategoryOrder);

            Assert.AreEqual(2, categories.Length, "Wrong number of categories");
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

            string[] categories = NpcToStringArray.Export(npc, npcGroup.VisibleCategoryOrder);

            Assert.AreEqual(1, categories.Length, "Wrong number of categories");
            Assert.AreEqual(0, categories[0].Length, "Hidden incorrectly trait appears");
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

            string[] categories = NpcToStringArray.Export(npc, npcGroup.VisibleCategoryOrder);

            Assert.AreEqual(1, categories.Length, "Wrong number of categories");
            Assert.AreEqual(VISIBLE_TRAIT, categories[0], "Output is not just visible trait with no extra punctuation");
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

            string[] categories = NpcToStringArray.Export(npc, npcGroup.VisibleCategoryOrder);

            Assert.AreEqual(1, categories.Length, "Wrong number of categories");
            Assert.AreEqual(TRAIT, categories[0], "Output is not just visible trait with no extra punctuation");
        }

        [TestMethod]
        public void HiddenCategory()
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
                new NpcGroup.Category(C0_NAME, isHidden: true), new NpcGroup.Category(C1_NAME) });
            npcGroup.Add(npc);

            string[] categories = NpcToStringArray.Export(npc, npcGroup.VisibleCategoryOrder);

            Assert.AreEqual(1, categories.Length, "Wrong number of categories");
            string expectedText = C1T0 + NpcToCsv.MULTI_TRAIT_SEPARATOR + C1T1;
            Assert.AreEqual(expectedText, categories[0], "Output is not just visible trait with no extra punctuation");
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

            string[] categories = NpcToStringArray.Export(npc, npcGroup.VisibleCategoryOrder);

            Assert.AreEqual(0, categories.Length, "Wrong number of categories");
        }

        private readonly MockRandom m_random = new MockRandom();
    }
}
