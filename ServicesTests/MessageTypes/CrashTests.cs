/*Copyright(C) 2023 Marcus Trenton, marcus.trenton@gmail.com

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
    public class CrashTests
    {
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ExceptionIsNull()
        {
            new Crash(null);
        }

        [TestMethod]
        public void ExceptionIsNotNull()
        {
            Exception e = new Exception("Test");
            Crash crash = new Crash(e);

            Assert.AreEqual(e, crash.Exception, "Parsed incorrect number");
        }
    }
}
