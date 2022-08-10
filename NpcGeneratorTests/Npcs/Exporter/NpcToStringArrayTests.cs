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
            npc.Add("Colour", new string[] { TRAIT });

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
            npc.Add("Colour", new string[] { TRAIT0, TRAIT1 });

            string[] categories = NpcToStringArray.Export(npc, new string[] { "Colour" });

            Assert.AreEqual(1, categories.Length, "Wrong number of categories");
            Assert.AreEqual(TRAIT0 + ExportUtil.MULTI_TRAIT_SEPARATOR + TRAIT1, categories[0], "Wrong traits in category");
        }

        [TestMethod]
        public void MultipleCategories()
        {
            const string COLOUR_TRAIT = "Blue";
            TraitCategory colourCategory = new TraitCategory("Colour", 1);
            colourCategory.Add(new Trait(COLOUR_TRAIT, 1, isHidden: false));

            const string ANIMAL_TRAIT = "Bear";
            TraitCategory animalCategory = new TraitCategory("Animal", 1);
            animalCategory.Add(new Trait(ANIMAL_TRAIT, 1, isHidden: false));

            Npc npc = new Npc();
            npc.Add("Colour", new string[] { COLOUR_TRAIT });
            npc.Add("Animal", new string[] { ANIMAL_TRAIT });

            string[] categories = NpcToStringArray.Export(npc, new string[] { "Colour", "Animal" });

            Assert.AreEqual(2, categories.Length, "Wrong number of categories");
            Assert.AreEqual(COLOUR_TRAIT, categories[0], "Wrong traits in category");
            Assert.AreEqual(ANIMAL_TRAIT, categories[1], "Wrong traits in category");
        }

        [TestMethod]
        public void ZeroCategories()
        {
            TraitSchema schema = new TraitSchema();

            NpcGroup npcGroup = NpcFactory.Create(schema, 0, new List<Replacement>());
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

            Assert.AreEqual(0, categories.Length, "Wrong number of trait categories");
        }
    }
}
