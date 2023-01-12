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
using NpcGenerator.Message;
using Services;
using System;


namespace Tests
{
    [TestClass]
    public class SaveNpcsTests
    {
        [TestMethod]
        public void FormatFileExtensionWithoutDot()
        {
            const string FILE_TYPE = "csv";
            SaveNpcs message = new SaveNpcs(FILE_TYPE);

            Assert.AreEqual(FILE_TYPE, message.Format, "Wrong Format was stored");
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void FormatFileExtensionWithDot()
        {
            new SaveNpcs(".csv");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void NullFormat()
        {
            new SaveNpcs(null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void EmptyFormat()
        {
            new SaveNpcs(String.Empty);
        }
    }
}
