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
        [DataRow(0, 31, "-2147483648")] //The furthest bit determines signed 2s complement.
        [DataRow(1, 0, "1")]
        [DataRow(4, 0, "5")]
        [DataRow(7, 1, "7")]
        [DataRow(10, 2, "14")]
        public void SetSingleBitTrue(int initialValue, int index, string expectedResult)
        {
            BitField bitField = new BitField(initialValue);
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
        public void GetSingleBitInvalid(int index)
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

        [DataTestMethod]
        [DataRow(0, 0, 1, 1, "1")]
        [DataRow(0, 1, 1, 1, "2")]
        [DataRow(0, 2, 2, 3, "12")]
        [DataRow(0, 3, 2, 2, "16")]
        [DataRow(0, 0, 31, 1073741824, "1073741824")]
        [DataRow(0, 0, 32, -2147483648, "-2147483648")] //The furthest bit determines signed 2s complement.
        [DataRow(2, 0, 1, 0, "2")]
        [DataRow(4, 0, 1, 1, "5")]
        [DataRow(7, 1, 2, 0, "1")]
        [DataRow(9, 1, 2, 3, "15")]
        public void SetMultipleBits(int initialValue, int index, int length, int value, string expectedResult)
        {
            BitField bitField = new BitField(initialValue);
            bitField.Set(index, length, value);
            Assert.AreEqual(expectedResult, bitField.ToString(), "Incorrect Set result");
        }

        [DataTestMethod, ExpectedException(typeof(ArgumentException))]
        [DataRow(-1, 1, 0)]
        [DataRow(32, 1, 0)]
        [DataRow(1, 0, 0)]
        [DataRow(0, 33, 0)]
        [DataRow(20, 13, 0)]
        [DataRow(0, 1, 2)]
        [DataRow(0, 4, 16)]
        public void SetMultipleBitsInvalid(int index, int length, int value)
        {
            BitField bitField = new BitField();
            bitField.Set(index, length, value);
        }

        [DataTestMethod]
        [DataRow(0, 0, 2, 0)]
        [DataRow(2, 0, 2, 2)]
        [DataRow(1, 0, 1, 1)]
        [DataRow(7, 1, 2, 3)]
        [DataRow(7, 9, 2, 0)]
        [DataRow(-1, 30, 2, 3)] //2's compliment
        [DataRow(-1, 0, 32, -1)] //2's compliment
        [DataRow(11, 1, 3, 5)]
        public void GetMultipleBits(int initialValue, int index, int length, int expectedValue)
        {
            BitField bitField = new BitField(initialValue);
            int value = bitField.Get(index, length);
            Assert.AreEqual(expectedValue, value, "Incorrect Get result");
        }

        [DataTestMethod, ExpectedException(typeof(ArgumentException))]
        [DataRow(-1, 2)]
        [DataRow(32, 1)]
        [DataRow(5, 0)]
        [DataRow(5, -1)]
        [DataRow(0, 33)]
        [DataRow(20, 13)]
        public void GetMultipleBitsInvalid(int index, int length)
        {
            BitField bitField = new BitField();
            bitField.Get(index, length);
        }

        [TestMethod]
        public void GetThenSetMultipleBits()
        {
            const int INDEX = 4;
            const int LENGTH = 2;
            const int VALUE = 3;
            BitField bitField = new BitField();
            bitField.Set(INDEX, LENGTH, VALUE);

            Assert.AreEqual("48", bitField.ToString(), "Wrong bits stored");

            int value = bitField.Get(INDEX, LENGTH);

            Assert.AreEqual(VALUE, value, "Wrong bits retrieved");
        }
    }
}
