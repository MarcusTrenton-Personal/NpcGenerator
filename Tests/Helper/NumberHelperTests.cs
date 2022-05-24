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

namespace Tests
{
    [TestClass]
    public class NumberHelperTests
    {
        [TestMethod]
        public void TryParsePositveNumberWithPositiveNumber()
        {
            int result;
            bool successful = NumberHelper.TryParsePositiveNumber("1234", out result);
            Assert.IsTrue(successful, "Could not parse 1234");
            Assert.AreEqual(1234, result, "Parsed incorrect number");
        }

        [TestMethod]
        public void TryParsePositveNumberWithZero()
        {
            int result;
            bool successful = NumberHelper.TryParsePositiveNumber("0", out result);
            Assert.IsFalse(successful, "Parsed 0 when it shouldn't");
        }

        [TestMethod]
        public void TryParsePositveNumberWithNegativeNumber()
        {
            int result;
            bool successful = NumberHelper.TryParsePositiveNumber("-1234", out result);
            Assert.IsFalse(successful, "Parsed -1234 when it shouldn't");
        }

        [TestMethod]
        public void TryParsePositveNumberWithNonNumeric()
        {
            int result;
            bool successful = NumberHelper.TryParsePositiveNumber("123abc", out result);
            Assert.IsFalse(successful, "Parsed 123abc when it shouldn't");
        }

        [TestMethod]
        public void TryParsePositveNumberWithNumericWords()
        {
            int result;
            bool successful = NumberHelper.TryParsePositiveNumber("one", out result);
            Assert.IsFalse(successful, "Parsed 'one' when it shouldn't");
        }

        [TestMethod]
        public void TryParsePositveNumberDecimal()
        {
            int result;
            bool successful = NumberHelper.TryParsePositiveNumber("1.23", out result);
            Assert.IsFalse(successful, "Parsed 1.23 when it shouldn't");
        }

        [TestMethod]
        public void TryParseDigitZero()
        {
            int result;
            bool successful = NumberHelper.TryParseDigit("0", out result);
            Assert.IsTrue(successful, "Could not parse 0");
            Assert.AreEqual(0, result, "Parsed incorrect number");
        }

        [TestMethod]
        public void TryParseDigitOne()
        {
            int result;
            bool successful = NumberHelper.TryParseDigit("1", out result);
            Assert.IsTrue(successful, "Could not parse 1");
            Assert.AreEqual(1, result, "Parsed incorrect number");
        }

        [TestMethod]
        public void TryParseDigitNegativeOne()
        {
            int result;
            bool successful = NumberHelper.TryParseDigit("-1", out result);
            Assert.IsFalse(successful, "Parsed -1 when it shouldn't");
        }

        [TestMethod]
        public void TryParseDigitTen()
        {
            int result;
            bool successful = NumberHelper.TryParseDigit("10", out result);
            Assert.IsFalse(successful, "Parsed 10 when it shouldn't");
        }

        [TestMethod]
        public void TryParseDigitDecimal()
        {
            int result;
            bool successful = NumberHelper.TryParseDigit("1.23", out result);
            Assert.IsFalse(successful, "Parsed 1.23 when it shouldn't");
        }

        [TestMethod]
        public void TryParseDigitNonNumeric()
        {
            int result;
            bool successful = NumberHelper.TryParseDigit("a", out result);
            Assert.IsFalse(successful, "Parsed 'a' when it shouldn't");
        }

        [TestMethod]
        public void TryParseDigitNumericWord()
        {
            int result;
            bool successful = NumberHelper.TryParseDigit("zero", out result);
            Assert.IsFalse(successful, "Parsed zero when it shouldn't");
        }
    }
}
