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
using System;
using System.Collections.Generic;
using System.IO;
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
            ExportUtil.CombineTraits(Array.Empty<string>(), builder);

            Assert.AreEqual(String.Empty, builder.ToString(), "Combining 0 traits should have produced an empty string");
        }

        [TestMethod]
        public void CombineOneTrait()
        {
            const string TRAIT = "Blue";
            string[] traits = new string[] { TRAIT };

            StringBuilder builder = new StringBuilder();
            ExportUtil.CombineTraits(traits, builder);

            Assert.AreEqual(TRAIT, builder.ToString(), "Produced string should have been " + TRAIT);
        }

        [TestMethod]
        public void CombineMultipleTraits()
        {
            const string TRAIT0 = "Blue";
            const string TRAIT1 = "Greed";
            const string TRAIT2 = "Red";
            string[] traits = new string[] { TRAIT0, TRAIT1, TRAIT2 };

            StringBuilder builder = new StringBuilder();
            ExportUtil.CombineTraits(traits, builder);

            string expected = TRAIT0 + ExportUtil.MULTI_TRAIT_SEPARATOR + TRAIT1 + ExportUtil.MULTI_TRAIT_SEPARATOR + TRAIT2;
            Assert.AreEqual(expected, builder.ToString(), "Traits combined incorrectly");
        }
    }
}
