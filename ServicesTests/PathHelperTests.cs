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
    public class PathHelperTests
    {
        [TestMethod]
        public void CombineWithoutSlash()
        {
            const string RELATIVE_PATH = @"folder\file.txt";

            string absolutePath = PathHelper.FullPathOf(RELATIVE_PATH);

            string expectedPath = AppContext.BaseDirectory + RELATIVE_PATH;
            Assert.AreEqual(expectedPath, absolutePath, "Incorrect path combination");
        }

        [TestMethod]
        public void CombineRooted()
        {
            const string ROOTED_PATH = @"\folder\file.txt";

            string absolutePath = PathHelper.FullPathOf(ROOTED_PATH);

            Assert.AreEqual(ROOTED_PATH, absolutePath, "Incorrect path combination");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void NullRelativePath()
        {
            PathHelper.FullPathOf(path: null);
        }

        [TestMethod]
        public void EmptyRelativePath()
        {
            string absolutePath = PathHelper.FullPathOf(path: string.Empty);

            Assert.AreEqual(AppContext.BaseDirectory, absolutePath, "Incorrect path combination");
        }

        [TestMethod]
        public void RelativePathIsAbsolute()
        {
            string originalAbsolutePath = @"C:\windows\I Should not be here";
            string resultAbsolutePath = PathHelper.FullPathOf(path: originalAbsolutePath);

            Assert.AreEqual(originalAbsolutePath, resultAbsolutePath, "Incorrect path combination");
        }
    }
}
