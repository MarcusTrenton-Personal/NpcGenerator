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
using NpcGenerator.Message;
using System;

namespace Tests
{
    [TestClass]
    public class GenerateNpcsTests
    {
        [DataTestMethod]
        [DataRow(5)]
        [DataRow(0)]
        public void ValidWholeNUmber(int quantity)
        {
            GenerateNpcs message = new GenerateNpcs(quantity, TraitSchema.Features.None);

            Assert.AreEqual(quantity, message.Quantity, "Wrong quantity was stored");
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void NegativeNumber()
        {
            new GenerateNpcs(-5, TraitSchema.Features.None);
        }
    }
}
