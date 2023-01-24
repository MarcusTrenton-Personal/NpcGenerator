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
using Services;
using System;

namespace Tests
{
    [TestClass]
    public class BitFieldTests
    {
        [TestMethod]
        public void Empty()
        {
            BitField bitField = new BitField();

            Assert.AreEqual("0", bitField.ToString(), "Wrong ToString result");
        }

        [DataTestMethod]
        [DataRow(0, "1")]
        [DataRow(1, "2")]
        [DataRow(2, "4")]
        [DataRow(3, "8")]
        [DataRow(30, "1073741824")]
        [DataRow(31, "-2147483648")] //The left-most bit determines signed 2s complement.
        public void SetSingleBit(int index, string expectedResult)
        {
            BitField bitField = new BitField();
            bitField.Set(index, true);
            Assert.AreEqual(expectedResult, bitField.ToString(), "Incorrect Set result");
        }

        [DataTestMethod, ExpectedException(typeof(ArgumentException))]
        [DataRow(-1)]
        [DataRow(32)]
        public void SetSingleInvalidBit(int index)
        {
            BitField bitField = new BitField();
            bitField.Set(index, true);
        }

        //TODO add constructor that takes an int. Use that for testing sets to 0.
    }
}
