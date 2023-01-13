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
using System;
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class NpcToCsvTests
    {
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void NullGroup()
        {
            NpcToCsv npcToCsv = new NpcToCsv();
            npcToCsv.Export(null);
        }

        [TestMethod]
        public void SingleNpc()
        {
            const string CATEGORY = "Colour";
            const string TRAIT = "Blue";

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT, CATEGORY) });

            NpcGroup npcGroup = new NpcGroup(new List<NpcGroup.Category> { new NpcGroup.Category(CATEGORY) });
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
            npc0.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT, CATEGORY) });

            Npc npc1 = new Npc();
            npc1.Add(CATEGORY, new Npc.Trait[] { new Npc.Trait(TRAIT, CATEGORY) });

            NpcGroup npcGroup = new NpcGroup(new List<NpcGroup.Category> { new NpcGroup.Category(CATEGORY) });
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

            NpcGroup npcGroup = new NpcGroup(new List<NpcGroup.Category> { new NpcGroup.Category(CATEGORY) });
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
            npc.Add(CATEGORY0, traits: new Npc.Trait[] { new Npc.Trait(TRAIT0, CATEGORY0) });
            npc.Add(CATEGORY1, traits: new Npc.Trait[] { new Npc.Trait(TRAIT1, CATEGORY1) });

            NpcGroup npcGroup = new NpcGroup(new List<NpcGroup.Category> { 
                new NpcGroup.Category(CATEGORY0), new NpcGroup.Category(CATEGORY1) });
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
            npc.Add(CATEGORY0, traits: new Npc.Trait[] { new Npc.Trait(CATEGORY0_TRAIT0, CATEGORY0), new Npc.Trait(CATEGORY0_TRAIT1, CATEGORY0) });
            npc.Add(CATEGORY1, traits: new Npc.Trait[] { new Npc.Trait(CATEGORY1_TRAIT0, CATEGORY1), new Npc.Trait(CATEGORY1_TRAIT1, CATEGORY1) });

            NpcGroup npcGroup = new NpcGroup(new List<NpcGroup.Category> {
                new NpcGroup.Category(CATEGORY0), new NpcGroup.Category(CATEGORY1) });
            npcGroup.Add(npc);

            NpcToCsv npcToCsv = new NpcToCsv();
            string csv = npcToCsv.Export(npcGroup);

            string expectedCsv = CATEGORY0 + NpcToCsv.SEPARATOR + CATEGORY1 + "\n" +
                CATEGORY0_TRAIT0 + NpcToCsv.MULTI_TRAIT_SEPARATOR + CATEGORY0_TRAIT1 + NpcToCsv.SEPARATOR +
                CATEGORY1_TRAIT0 + NpcToCsv.MULTI_TRAIT_SEPARATOR + CATEGORY1_TRAIT1;

            Assert.AreEqual(expectedCsv, csv, "Npc did not generate expected CSV row");
        }

        [TestMethod]
        public void ExportNoTraitCsv()
        {
            const string CATEGORY = "Colour";

            Npc npc = new Npc();

            NpcGroup npcGroup = new NpcGroup(new List<NpcGroup.Category> { new NpcGroup.Category(CATEGORY) });
            npcGroup.Add(npc);

            NpcToCsv npcToCsv = new NpcToCsv();
            string csv = npcToCsv.Export(npcGroup);

            Assert.AreEqual(CATEGORY + "\n", csv, "Npc did not generate expected CSV row");
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

            NpcToCsv npcToCsv = new NpcToCsv();
            string csv = npcToCsv.Export(npcGroup);

            string expectedCsv = C0_NAME + NpcToCsv.SEPARATOR + C1_NAME + "\n" +
                C0T0 + NpcToCsv.MULTI_TRAIT_SEPARATOR + C0T1 + NpcToCsv.SEPARATOR + 
                C1T0 + NpcToCsv.MULTI_TRAIT_SEPARATOR + C1T1;

            Assert.AreEqual(expectedCsv, csv, "Csv was not exported correctly");
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

            NpcToCsv npcToCsv = new NpcToCsv();
            string csv = npcToCsv.Export(npcGroup);

            string expectedCsv = NOT_FOUND_CATEGORY + NpcToCsv.SEPARATOR + CATEGORY + "\n" + NpcToCsv.SEPARATOR + TRAIT;

            Assert.AreEqual(expectedCsv, csv, "Csv was not exported correctly");
        }

        [TestMethod]
        public void InterAndIntraTraitSeparatorsAreDifferent()
        {
            Assert.AreNotEqual(NpcToCsv.MULTI_TRAIT_SEPARATOR, NpcToCsv.SEPARATOR, 
                "Trait lists in a category are not distinguishable from a trait in another category because separators are the same.");
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

            NpcToCsv npcToCsv = new NpcToCsv();
            string csv = npcToCsv.Export(npcGroup);

            string expectedCsv = CATEGORY + "\n";

            Assert.AreEqual(expectedCsv, csv, "Csv was not exported correctly");
        }

        [TestMethod]
        public void VisibleTraitThenHiddenTrait()
        {
            const string CATEGORY = "Colour";
            const string VISIBLE_TRAIT = "Blue";
            const string HIDDEN_TRAIT = "Red";
            NpcGroup npcGroup = new NpcGroup(new List<NpcGroup.Category> { new NpcGroup.Category(CATEGORY) });

            Npc npc = new Npc();
            npc.Add(CATEGORY, new Npc.Trait[] { 
                new Npc.Trait(VISIBLE_TRAIT, CATEGORY), new Npc.Trait(HIDDEN_TRAIT, CATEGORY, isHidden: true) });
            npcGroup.Add(npc);

            NpcToCsv npcToCsv = new NpcToCsv();
            string csv = npcToCsv.Export(npcGroup);

            string expectedCsv = CATEGORY + "\n" + VISIBLE_TRAIT;

            Assert.AreEqual(expectedCsv, csv, "Csv was not exported correctly");
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

            NpcToCsv npcToCsv = new NpcToCsv();
            string csv = npcToCsv.Export(npcGroup);

            string expectedCsv = CATEGORY + "\n" + TRAIT;

            Assert.AreEqual(expectedCsv, csv, "Csv was not exported correctly");
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

            NpcToCsv npcToCsv = new NpcToCsv();
            string csv = npcToCsv.Export(npcGroup);

            string expectedCsv = C1_NAME + "\n" +
                C1T0 + NpcToCsv.MULTI_TRAIT_SEPARATOR + C1T1;

            Assert.AreEqual(expectedCsv, csv, "Csv was not exported correctly");
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

            NpcToCsv npcToCsv = new NpcToCsv();
            string csv = npcToCsv.Export(npcGroup);

            string expectedCsv = string.Empty;

            Assert.AreEqual(expectedCsv, csv, "Csv was not exported correctly");
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

            NpcToCsv npcToCsv = new NpcToCsv();
            string csv = npcToCsv.Export(npcGroup);

            string expectedCsv = C1_NAME + "\n" +
                C1T0 + NpcToCsv.MULTI_TRAIT_SEPARATOR + C1T1 + "\n" +
                C1T0 + NpcToCsv.MULTI_TRAIT_SEPARATOR + C1T1;

            Assert.AreEqual(expectedCsv, csv, "Csv was not exported correctly");
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

            NpcToCsv npcToCsv = new NpcToCsv();
            string csv = npcToCsv.Export(npcGroup);

            string expectedCsv = C0_NAME + NpcToCsv.SEPARATOR + C1_NAME + "\n" +
                C0T0 + NpcToCsv.MULTI_TRAIT_SEPARATOR + C0T1 + NpcToCsv.SEPARATOR + C1T0 + NpcToCsv.MULTI_TRAIT_SEPARATOR + C1T1 + "\n" +
                NpcToCsv.SEPARATOR + C1T0 + NpcToCsv.MULTI_TRAIT_SEPARATOR + C1T1;

            Assert.AreEqual(expectedCsv, csv, "Csv was not exported correctly");
        }
    }
}
