﻿/*Copyright(C) 2022 Marcus Trenton, marcus.trenton@gmail.com

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
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class HashSetUtilTests
    {
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void FindWithNullList()
        {
            HashSetUtil.Find<int>(set: null, test: x => x > 0);
        }

        [TestMethod]
        public void FindWithEmptyIntList()
        {
            int result = HashSetUtil.Find(new HashSet<int>(), x => x > 0);

            Assert.AreEqual(0, result, "Wrong result when no element is found. Is not the default.");
        }

        [TestMethod]
        public void FindWithEmptyObjectList()
        {
            object result = HashSetUtil.Find(new HashSet<object>(), x => x != null);

            Assert.AreEqual(null, result, "Wrong result when no element is found. Is not the default.");
        }

        [TestMethod]
        public void FindOneElementFound()
        {
            HashSet<int> list = new HashSet<int>() { -1, -2, 9, -3, -4 };
            int result = HashSetUtil.Find(list, x => x > 0);

            Assert.AreEqual(9, result, "Wrong result returned from the search.");
        }

        [TestMethod]
        public void FindNotFound()
        {
            HashSet<int> list = new HashSet<int>() { -1, -2, -6, -3, -4 };
            int result = HashSetUtil.Find(list, x => x > 0);

            Assert.AreEqual(0, result, "Wrong result returned from the search.");
        }

        [TestMethod]
        public void FindOneofTwoElements()
        {
            HashSet<int> list = new HashSet<int>() { -1, 2, -6, 3, -4 };
            int result = HashSetUtil.Find(list, x => x > 0);
            bool findSuccessful = result == 2 || result == 3;

            Assert.IsTrue(findSuccessful, "Wrong result returned from the search.");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void FindWithNullTest()
        {
            HashSet<int> list = new HashSet<int>() { -1, 2, -6, 3, -4 };
            HashSetUtil.Find(list, test: null);
        }
    }
}
