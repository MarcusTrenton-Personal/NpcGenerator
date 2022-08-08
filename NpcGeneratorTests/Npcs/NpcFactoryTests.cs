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
    public class NpcFactoryTests
    {
        [TestMethod]
        public void CreateSingleNpc()
        {
            const string CATEGORY_NAME = "Colour";
            const string TRAIT_NAME = "Blue";
            TraitCategory colourCategory = new TraitCategory(CATEGORY_NAME, 1);
            colourCategory.Add(new Trait(TRAIT_NAME, 1, isHidden: false));

            TraitSchema schema = new TraitSchema();
            schema.Add(colourCategory);

            NpcGroup npcGroup = NpcFactory.Create(schema, 1, new List<Replacement>());
            
            Assert.IsNotNull(npcGroup, "Failed to create an npc using a valid schema");
            Assert.AreEqual(1, npcGroup.NpcCount, "Wrong number of npcs created.");

            Npc npc = npcGroup.GetNpcAtIndex(0);
            string[] traits = npc.GetTraitsOfCategory(CATEGORY_NAME);
            Assert.AreEqual(1, traits.Length, "Wrong number of traits in category");
            Assert.AreEqual(TRAIT_NAME, traits[0], "Npc created with incorrect trait in category");
        }

        [TestMethod]
        public void CreateMultipleNpcs()
        {
            const string CATEGORY_NAME = "Colour";
            const string TRAIT_NAME = "Blue";
            TraitCategory colourCategory = new TraitCategory(CATEGORY_NAME, 1);
            colourCategory.Add(new Trait(TRAIT_NAME, 1, isHidden: false));

            TraitSchema schema = new TraitSchema();
            schema.Add(colourCategory);

            NpcGroup npcGroup = NpcFactory.Create(schema, 2, new List<Replacement>());

            Assert.IsNotNull(npcGroup, "Failed to create an npc using a valid schema");
            Assert.AreEqual(2, npcGroup.NpcCount, "Wrong number of npcs created.");

            Npc npc0 = npcGroup.GetNpcAtIndex(0);
            string[] traits0 = npc0.GetTraitsOfCategory(CATEGORY_NAME);
            Assert.AreEqual(1, traits0.Length, "Wrong number of traits in category");
            Assert.AreEqual(TRAIT_NAME, traits0[0], "Npc created with incorrect trait in category");

            Npc npc1 = npcGroup.GetNpcAtIndex(0);
            string[] traits1 = npc1.GetTraitsOfCategory(CATEGORY_NAME);
            Assert.AreEqual(1, traits1.Length, "Wrong number of traits in category");
            Assert.AreEqual(TRAIT_NAME, traits1[0], "Npc created with incorrect trait in category");
        }

        [TestMethod]
        public void CreateZeroNpcs()
        {
            TraitCategory colourCategory = new TraitCategory("Colour", 1);
            colourCategory.Add(new Trait("Blue", 1, isHidden: false));

            TraitSchema schema = new TraitSchema();
            schema.Add(colourCategory);

            NpcGroup npcGroup = NpcFactory.Create(schema, 0, new List<Replacement>());

            Assert.IsNotNull(npcGroup, "Failed to create an npc using a valid schema");
            Assert.AreEqual(0, npcGroup.NpcCount, "Wrong number of npcs created.");
        }
    }
}
