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
using System.Text;

namespace Tests
{
    [TestClass]
    public class ExportUtilTests
    {
        [TestMethod]
        public void CombineZeroTraits()
        {
            StringBuilder builder = new StringBuilder();
            ExportUtil.CombineTraits(Array.Empty<Npc.Trait>(), builder);

            Assert.AreEqual(string.Empty, builder.ToString(), "Combining 0 traits should have produced an empty string");
        }

        [TestMethod]
        public void CombineOneTrait()
        {
            const string TRAIT = "Blue";
            Npc.Trait[] traits = new Npc.Trait[] { new Npc.Trait(TRAIT, "Colour") };

            StringBuilder builder = new StringBuilder();
            ExportUtil.CombineTraits(traits, builder);

            Assert.AreEqual(TRAIT, builder.ToString(), "Produced string should have been " + TRAIT);
        }

        [TestMethod]
        public void CombineMultipleTraits()
        {
            const string CATEGORY = "Colour";
            const string TRAIT0 = "Blue";
            const string TRAIT1 = "Green";
            const string TRAIT2 = "Red";
            Npc.Trait[] traits = 
                new Npc.Trait[] { new Npc.Trait(TRAIT0, CATEGORY), new Npc.Trait(TRAIT1, CATEGORY), new Npc.Trait(TRAIT2, CATEGORY) };

            StringBuilder builder = new StringBuilder();
            ExportUtil.CombineTraits(traits, builder);

            string expected = TRAIT0 + ExportUtil.MULTI_TRAIT_SEPARATOR + TRAIT1 + ExportUtil.MULTI_TRAIT_SEPARATOR + TRAIT2;
            Assert.AreEqual(expected, builder.ToString(), "Traits combined incorrectly");
        }

        [TestMethod]
        public void VisibleDistinctTraitsEmpty()
        {
            Npc.Trait[] traits = new Npc.Trait[] { };

            HashSet<string> values = ExportUtil.VisibleDistinctTraits(traits);

            Assert.AreEqual(0, values.Count, "Wrong number of values");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void VisibleDistinctTraitsNull()
        {
            ExportUtil.VisibleDistinctTraits(null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void VisibleDistinctTraitsNullElement()
        {
            Npc.Trait[] traits = new Npc.Trait[] { null };

            ExportUtil.VisibleDistinctTraits(traits);
        }

        [TestMethod]
        public void VisibleDistinctTraitsAlreadyVisibleAndDistinct()
        {
            const string CATEGORY = "Colour";
            const string TRAIT0 = "Blue";
            const string TRAIT1 = "Green";
            const string TRAIT2 = "Red";
            Npc.Trait[] traits =
                new Npc.Trait[] { new Npc.Trait(TRAIT0, CATEGORY), new Npc.Trait(TRAIT1, CATEGORY), new Npc.Trait(TRAIT2, CATEGORY) };

            HashSet<string> values = ExportUtil.VisibleDistinctTraits(traits);

            Assert.AreEqual(3, values.Count, "Wrong number of elements");
            Assert.IsTrue(values.Contains(TRAIT0), "Expected element is not contained");
            Assert.IsTrue(values.Contains(TRAIT1), "Expected element is not contained");
            Assert.IsTrue(values.Contains(TRAIT2), "Expected element is not contained");
        }

        [TestMethod]
        public void VisibleDistinctTraitsFilterDuplicates()
        {
            const string CATEGORY = "Colour";
            const string TRAIT0 = "Blue";
            const string TRAIT1 = "Green";
            Npc.Trait[] traits =
                new Npc.Trait[] { new Npc.Trait(TRAIT0, CATEGORY), new Npc.Trait(TRAIT1, CATEGORY), new Npc.Trait(TRAIT1, CATEGORY) };

            HashSet<string> values = ExportUtil.VisibleDistinctTraits(traits);

            Assert.AreEqual(2, values.Count, "Wrong number of elements");
            Assert.IsTrue(values.Contains(TRAIT0), "Expected element is not contained");
            Assert.IsTrue(values.Contains(TRAIT1), "Expected element is not contained");
        }

        [TestMethod]
        public void VisibleDistinctTraitsFilterHidden()
        {
            const string CATEGORY = "Colour";
            const string TRAIT0 = "Blue";
            const string TRAIT1 = "Green";
            const string TRAIT2 = "Red";
            Npc.Trait[] traits = new Npc.Trait[] { 
                new Npc.Trait(TRAIT0, CATEGORY), 
                new Npc.Trait(TRAIT1, CATEGORY, isHidden: true), 
                new Npc.Trait(TRAIT2, CATEGORY) };

            HashSet<string> values = ExportUtil.VisibleDistinctTraits(traits);

            Assert.AreEqual(2, values.Count, "Wrong number of elements");
            Assert.IsTrue(values.Contains(TRAIT0), "Expected element is not contained");
            Assert.IsTrue(values.Contains(TRAIT2), "Expected element is not contained");
        }

        [TestMethod]
        public void VisibleDistinctTraitsFilterDuplicatesAndHidden()
        {
            const string CATEGORY = "Colour";
            const string TRAIT0 = "Blue";
            const string TRAIT1 = "Green";
            const string TRAIT2 = "Red";

            Npc.Trait[] traits = new Npc.Trait[] {
                new Npc.Trait(TRAIT0, CATEGORY),
                new Npc.Trait(TRAIT1, CATEGORY, isHidden: true),
                new Npc.Trait(TRAIT2, CATEGORY, isHidden: true),
                new Npc.Trait(TRAIT2, CATEGORY)
            };

            HashSet<string> values = ExportUtil.VisibleDistinctTraits(traits);

            Assert.AreEqual(2, values.Count, "Wrong number of elements");
            Assert.IsTrue(values.Contains(TRAIT0), "Expected element is not contained");
            Assert.IsTrue(values.Contains(TRAIT2), "Expected element is not contained");
        }
    }
}
