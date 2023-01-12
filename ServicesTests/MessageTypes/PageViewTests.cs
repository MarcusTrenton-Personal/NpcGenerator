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
using Services.Message;
using System;

namespace Tests
{
    [TestClass]
    public class PageViewTests
    {
        [TestMethod]
        public void ValidPage()
        {
            const string TITLE = "Main menu";
            PageView message = new PageView(TITLE);

            Assert.AreEqual(TITLE, message.Title, "Wrong title was stored");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void NullPage()
        {
            new PageView(null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void EmptyPage()
        {
            new PageView(string.Empty);
        }
    }
}
