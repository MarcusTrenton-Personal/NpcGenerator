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
    public class AboutModelTests
    {
        [TestMethod]
        public void VersionText()
        {
            string version = model.Version;

            string[] parts = version.Split('.');
            Assert.AreEqual(4, parts.Length, "Version should be 4 parts separated by .");
            for(int i = 0; i < parts.Length; i++)
            {
                Assert.IsTrue(int.TryParse(parts[i], out int number), "Part " + i + " must be an integer");
                Assert.IsTrue(number >= 0, "Part " + i + " must be non-negative");
            }
        }

        private readonly AboutModel model = new AboutModel();
    }


}
