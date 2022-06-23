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
using System.Globalization;
using System.Windows.Controls;

namespace Tests
{
    [TestClass]
    public class NaturalNumberValidationRuleTests
    {
        [TestMethod]
        public void NaturalNumberInt()
        {
            ValidationResult result = rule.Validate(3, CultureInfo.InvariantCulture);
            Assert.IsTrue(result.IsValid, "Natural number is incorrectly invalid");
        }

        [TestMethod]
        public void ZeroInt()
        {
            ValidationResult result = rule.Validate(0, CultureInfo.InvariantCulture);
            Assert.IsFalse(result.IsValid, "0 is not a natural number, which must be greater than 0");
        }

        [TestMethod]
        public void NegativeInt()
        {
            ValidationResult result = rule.Validate(-3, CultureInfo.InvariantCulture);
            Assert.IsFalse(result.IsValid, "-3 is not a natural number, which must be greater than 0");
        }

        [TestMethod]
        public void Float()
        {
            ValidationResult result = rule.Validate(3.1, CultureInfo.InvariantCulture);
            Assert.IsFalse(result.IsValid, "Natural number cannot have a decimal");
        }

        [TestMethod]
        public void NaturalNumberString()
        {
            ValidationResult result = rule.Validate("3", CultureInfo.InvariantCulture);
            Assert.IsTrue(result.IsValid, "Natural number is incorrectly invalid");
        }

        [TestMethod]
        public void ZeroString()
        {
            ValidationResult result = rule.Validate("0", CultureInfo.InvariantCulture);
            Assert.IsFalse(result.IsValid, "0 is not a natural number, which must be greater than 0");
        }

        [TestMethod]
        public void NegativeNumberString()
        {
            ValidationResult result = rule.Validate("-3", CultureInfo.InvariantCulture);
            Assert.IsFalse(result.IsValid, "-3 is not a natural number, which must be greater than 0");
        }

        [TestMethod]
        public void FloatString()
        {
            ValidationResult result = rule.Validate("3.1", CultureInfo.InvariantCulture);
            Assert.IsFalse(result.IsValid, "Natural number cannot have a decimal");
        }

        [TestMethod]
        public void NonNumericString()
        {
            ValidationResult result = rule.Validate("Doom", CultureInfo.InvariantCulture);
            Assert.IsFalse(result.IsValid, "Doom is text, not a natural number");
        }

        private readonly NaturalNumberValidationRule rule = new NaturalNumberValidationRule();
    }
}
