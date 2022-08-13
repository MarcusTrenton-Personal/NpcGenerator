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
along with this program. If not, see<https://www.gnu.org/licenses/>.*/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NpcGenerator;
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class NpcToCsvTests
    {
        [TestMethod]
        public void SingleNpc()
        {
            const string CATEGORY = "Colour";
            const string TRAIT = "Blue";

            Npc npc = new Npc();
            npc.Add(CATEGORY, new string[] { TRAIT });

            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY });
            npcGroup.Add(npc);

            NpcToCsv npcToCsv = new NpcToCsv();
            string csv = npcToCsv.Export(npcGroup);

            Assert.AreEqual(CATEGORY + "\n" + TRAIT, csv, "NpcGroup did not generate expected CSV text");
        }

        [TestMethod]
        public void MultipleNpcs()
        {
            const string CATEGORY = "Colour";
            const string TRAIT = "Blue";

            Npc npc0 = new Npc();
            npc0.Add(CATEGORY, new string[] { TRAIT });

            Npc npc1 = new Npc();
            npc1.Add(CATEGORY, new string[] { TRAIT });

            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY });
            npcGroup.Add(npc0);
            npcGroup.Add(npc1);

            NpcToCsv npcToCsv = new NpcToCsv();
            string csv = npcToCsv.Export(npcGroup);

            Assert.AreEqual(CATEGORY + "\n" + TRAIT + "\n" + TRAIT, csv, "NpcGroup did not generate expected CSV text");
        }

        [TestMethod]
        public void ZeroNpcs()
        {
            const string CATEGORY = "Colour";

            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY });
            NpcToCsv npcToCsv = new NpcToCsv();
            string csv = npcToCsv.Export(npcGroup);

            Assert.AreEqual(CATEGORY + "\n", csv, "NpcGroup did not generate expected CSV text");
        }

        [TestMethod]
        public void MultipleCategories()
        {
            const string CATEGORY0 = "Colour";
            const string TRAIT0 = "Blue";

            const string CATEGORY1 = "Animal";
            const string TRAIT1 = "Bear";

            Npc npc = new Npc();
            npc.Add(CATEGORY0, traits: new string[] { TRAIT0 });
            npc.Add(CATEGORY1, traits: new string[] { TRAIT1 });

            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY0, CATEGORY1 });
            npcGroup.Add(npc);

            NpcToCsv npcToCsv = new NpcToCsv();
            string csv = npcToCsv.Export(npcGroup);

            string expectedCsv = CATEGORY0 + NpcToCsv.SEPARATOR + CATEGORY1 + "\n" +
                TRAIT0 + NpcToCsv.SEPARATOR + TRAIT1;

            Assert.AreEqual(expectedCsv, csv, "Npc did not generate expected CSV row");
        }

        [TestMethod]
        public void ExportMultiTraitCsv()
        {
            const string CATEGORY0 = "Colour";
            const string CATEGORY0_TRAIT0 = "Blue";
            const string CATEGORY0_TRAIT1 = "Green";

            const string CATEGORY1 = "Terrain";
            const string CATEGORY1_TRAIT0 = "Hills";
            const string CATEGORY1_TRAIT1 = "River";

            Npc npc = new Npc();
            npc.Add(CATEGORY0, traits: new string[] { CATEGORY0_TRAIT0, CATEGORY0_TRAIT1 });
            npc.Add(CATEGORY1, traits: new string[] { CATEGORY1_TRAIT0, CATEGORY1_TRAIT1 });

            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY0, CATEGORY1 });
            npcGroup.Add(npc);

            NpcToCsv npcToCsv = new NpcToCsv();
            string csv = npcToCsv.Export(npcGroup);

            string expectedCsv = CATEGORY0 + NpcToCsv.SEPARATOR + CATEGORY1 + "\n" +
                CATEGORY0_TRAIT0 + ExportUtil.MULTI_TRAIT_SEPARATOR + CATEGORY0_TRAIT1 + NpcToCsv.SEPARATOR +
                CATEGORY1_TRAIT0 + ExportUtil.MULTI_TRAIT_SEPARATOR + CATEGORY1_TRAIT1;

            Assert.AreEqual(expectedCsv, csv, "Npc did not generate expected CSV row");
        }

        [TestMethod]
        public void ExportNoTraitCsv()
        {
            const string CATEGORY = "Colour";

            Npc npc = new Npc();

            NpcGroup npcGroup = new NpcGroup(new List<string>() { CATEGORY });
            npcGroup.Add(npc);

            NpcToCsv npcToCsv = new NpcToCsv();
            string csv = npcToCsv.Export(npcGroup);

            Assert.AreEqual(CATEGORY + "\n", csv, "Npc did not generate expected CSV row");
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
            npc.Add(category: CATEGORY0_NAME, traits: new string[] { CATEGORY0_TRAIT0, CATEGORY0_TRAIT1 });
            npc.Add(category: CATEGORY1_NAME, traits: new string[] { CATEGORY1_TRAIT0, CATEGORY1_TRAIT1 });

            NpcGroup npcGroup = new NpcGroup(new List<string> { CATEGORY0_NAME, CATEGORY1_NAME });
            npcGroup.Add(npc);

            NpcToCsv npcToCsv = new NpcToCsv();
            string csv = npcToCsv.Export(npcGroup);

            string expectedCsv = CATEGORY0_NAME + NpcToCsv.SEPARATOR + CATEGORY1_NAME + "\n" +
                CATEGORY0_TRAIT0 + ExportUtil.MULTI_TRAIT_SEPARATOR + CATEGORY0_TRAIT1 + NpcToCsv.SEPARATOR + 
                CATEGORY1_TRAIT0 + ExportUtil.MULTI_TRAIT_SEPARATOR + CATEGORY1_TRAIT1;

            Assert.AreEqual(expectedCsv, csv, "Csv was not exported correctly");
        }

        [TestMethod]
        public void OrderHasUnknownCategories()
        {
            const string CATEGORY = "Colour";
            const string NOT_FOUND_CATEGORY = "Animal";
            const string TRAIT = "Blue";
            NpcGroup npcGroup = new NpcGroup(new List<string> { NOT_FOUND_CATEGORY, CATEGORY });

            Npc npc = new Npc();
            npc.Add(CATEGORY, new string[] { TRAIT });
            npcGroup.Add(npc);

            NpcToCsv npcToCsv = new NpcToCsv();
            string csv = npcToCsv.Export(npcGroup);

            string expectedCsv = NOT_FOUND_CATEGORY + NpcToCsv.SEPARATOR + CATEGORY + "\n" + NpcToCsv.SEPARATOR + TRAIT;

            Assert.AreEqual(expectedCsv, csv, "Csv was not exported correctly");
        }
    }
}
