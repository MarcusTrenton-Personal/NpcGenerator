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
        [DataRow(0, "0")]
        [DataRow(1, "1")]
        [DataRow(3, "3")]
        [DataRow(7, "7")]
        [DataRow(-1, "-1")]
        public void ParameterizedConstructorToString(int initialValue, string expectedResult)
        {
            BitField bitField = new BitField(initialValue);
            Assert.AreEqual(expectedResult, bitField.ToString(), "Incorrect initial value");
        }

        [DataTestMethod]
        [DataRow(0, 0, "1")]
        [DataRow(0, 1, "2")]
        [DataRow(0, 2, "4")]
        [DataRow(0, 3, "8")]
        [DataRow(0, 30, "1073741824")]
        [DataRow(0, 31, "-2147483648")] //The left-most bit determines signed 2s complement.
        [DataRow(1, 0, "1")]
        [DataRow(4, 0, "5")]
        [DataRow(7, 1, "7")]
        [DataRow(10, 2, "14")]
        public void SetSingleBitTrue(int intialValue, int index, string expectedResult)
        {
            BitField bitField = new BitField(intialValue);
            bitField.Set(index, true);
            Assert.AreEqual(expectedResult, bitField.ToString(), "Incorrect Set result");
        }

        [DataTestMethod]
        [DataRow(1, 0, "0")]
        [DataRow(3, 1, "1")]
        [DataRow(15, 2, "11")]
        [DataRow(-1, 0, "-2")] //Two's compliment
        [DataRow(1073741824, 30, "0")]
        [DataRow(-1, 31, "2147483647")] //Two's compliment
        [DataRow(0, 0, "0")]
        [DataRow(4, 1, "4")]
        public void SetSingleBitFalse(int initialValue, int index, string expectedResult)
        {
            BitField bitField = new BitField(initialValue);
            bitField.Set(index, false);
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

        [DataTestMethod]
        [DataRow(0, 0, false)]
        [DataRow(2, 0, false)]
        [DataRow(1, 0, true)]
        [DataRow(7, 2, true)]
        [DataRow(7, 9, false)]
        [DataRow(-1, 31, true)] //2's compliment
        public void GetSingleBit(int initialValue, int index, bool expectedValue)
        {
            BitField bitField = new BitField(initialValue);
            bool value = bitField.Get(index);
            Assert.AreEqual(expectedValue, value, "Incorrect Get result");
        }

        [DataTestMethod, ExpectedException(typeof(ArgumentException))]
        [DataRow(-1)]
        [DataRow(32)]
        public void GetSingleInvalidBit(int index)
        {
            BitField bitField = new BitField();
            bitField.Get(index);
        }

        [TestMethod]
        public void SetThenGetBit()
        {
            const int INDEX = 5;
            
            BitField bitField = new BitField();
            bool value = bitField.Get(INDEX);

            Assert.IsFalse(value, "Retrieved value is wrong");

            bitField.Set(INDEX, true);

            bool value2 = bitField.Get(INDEX);

            Assert.IsTrue(value2, "Retrieved value is wrong");
        }
    }
}
