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
    public class NpcToCsvTests
    {
        [TestMethod]
        public void SingleNpc()
        {
            Npc npc = new Npc();
            npc.Add("Colour", new string[] { "Blue" });

            NpcGroup npcGroup = new NpcGroup(new List<string>() { "Colour" });
            npcGroup.Add(npc);

            NpcToCsv npcToCsv = new NpcToCsv();
            string csv = npcToCsv.Export(npcGroup);

            Assert.AreEqual("Colour\nBlue", csv, "NpcGroup did not generate expected CSV text");
        }

        [TestMethod]
        public void MultipleNpcs()
        {
            Npc npc0 = new Npc();
            npc0.Add("Colour", new string[] { "Blue" });

            Npc npc1 = new Npc();
            npc1.Add("Colour", new string[] { "Blue" });

            NpcGroup npcGroup = new NpcGroup(new List<string>() { "Colour" });
            npcGroup.Add(npc0);
            npcGroup.Add(npc1);

            NpcToCsv npcToCsv = new NpcToCsv();
            string csv = npcToCsv.Export(npcGroup);

            Assert.AreEqual("Colour\nBlue\nBlue", csv, "NpcGroup did not generate expected CSV text");
        }

        [TestMethod]
        public void ZeroNpcs()
        {
            NpcGroup npcGroup = new NpcGroup(new List<string>() { "Colour" });
            NpcToCsv npcToCsv = new NpcToCsv();
            string csv = npcToCsv.Export(npcGroup);

            Assert.AreEqual("Colour\n", csv, "NpcGroup did not generate expected CSV text");
        }

        [TestMethod]
        public void MultipleCategories()
        {
            Npc npc = new Npc();
            npc.Add(category: "Colour", traits: new string[] { "Blue" });
            npc.Add(category: "Animal", traits: new string[] { "Bear" });

            NpcGroup npcGroup = new NpcGroup(new List<string>() { "Colour", "Animal" });
            npcGroup.Add(npc);

            NpcToCsv npcToCsv = new NpcToCsv();
            string csv = npcToCsv.Export(npcGroup);

            Assert.AreEqual("Colour,Animal\nBlue,Bear", csv, "Npc did not generate expected CSV row");
        }

        [TestMethod]
        public void ExportMultiTraitCsv()
        {
            Npc npc = new Npc();
            npc.Add(category: "Colour", traits: new string[] { "Blue", "Green" });
            npc.Add(category: "Terrain", traits: new string[] { "Hills", "River" });

            NpcGroup npcGroup = new NpcGroup(new List<string>() { "Colour", "Terrain" });
            npcGroup.Add(npc);

            NpcToCsv npcToCsv = new NpcToCsv();
            string csv = npcToCsv.Export(npcGroup);

            Assert.AreEqual("Colour,Terrain\nBlue & Green,Hills & River", csv, "Npc did not generate expected CSV row");
        }

        [TestMethod]
        public void ExportNoTraitCsv()
        {
            Npc npc = new Npc();

            NpcGroup npcGroup = new NpcGroup(new List<string>() { "Colour" });
            npcGroup.Add(npc);

            NpcToCsv npcToCsv = new NpcToCsv();
            string csv = npcToCsv.Export(npcGroup);

            Assert.AreEqual("Colour\n", csv, "Npc did not generate expected CSV row");
        }
    }
}
