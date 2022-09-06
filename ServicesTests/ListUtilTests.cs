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
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class ListUtilTests
    {
        [TestMethod]
        public void FindWithNullList()
        {
            bool threwException = false;
            try
            {
                int result = ListUtil.Find<int>(null, x => x > 0);
            }
            catch(ArgumentNullException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Cannot find element of null list. Should throw exception.");
        }

        [TestMethod]
        public void FindWithEmptyIntList()
        {
            int result = ListUtil.Find(new List<int>(), x => x > 0);

            Assert.AreEqual(0, result, "Wrong result when no element is found. Is not the default.");
        }

        [TestMethod]
        public void FindWithEmptyObjectList()
        {
            object result = ListUtil.Find(new List<object>(), x => x != null);

            Assert.AreEqual(null, result, "Wrong result when no element is found. Is not the default.");
        }

        [TestMethod]
        public void FindOneElementFound()
        {
            List<int> list = new List<int>() { -1, -2, 9, -3, -4};
            int result = ListUtil.Find(list, x => x > 0);

            Assert.AreEqual(9, result, "Wrong result returned from the search.");
        }

        [TestMethod]
        public void FindNotFound()
        {
            List<int> list = new List<int>() { -1, -2, -6, -3, -4 };
            int result = ListUtil.Find(list, x => x > 0);

            Assert.AreEqual(0, result, "Wrong result returned from the search.");
        }

        [TestMethod]
        public void FindOneofTwoElements()
        {
            List<int> list = new List<int>() { -1, 2, -6, 3, -4 };
            int result = ListUtil.Find(list, x => x > 0);
            bool findSuccessful = result == 2 || result == 3;

            Assert.IsTrue(findSuccessful, "Wrong result returned from the search.");
        }

        [TestMethod]
        public void FindWithNullPredicate()
        {
            bool threwException = false;
            try
            {
                List<int> list = new List<int>() { -1, 2, -6, 3, -4 };
                int result = ListUtil.Find(list, null);
            }
            catch (ArgumentNullException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Cannot find element of null list. Should throw exception.");
        }

        [TestMethod]
        public void IsNullOrEmptyWithNull()
        {
            List<int> nullList = null;
            bool isNullOrEmpty = ListUtil.IsNullOrEmpty(nullList);

            Assert.IsTrue(isNullOrEmpty, "IsNullOrEmpty should be true");
        }

        [TestMethod]
        public void IsNullOrEmptyWithEmpty()
        {
            List<int> emptyList = new List<int>();
            bool isNullOrEmpty = ListUtil.IsNullOrEmpty(emptyList);

            Assert.IsTrue(isNullOrEmpty, "IsNullOrEmpty should be true");
        }

        [TestMethod]
        public void IsNullOrEmptyWithSingleElement()
        {
            List<int> singleElementList = new List<int>() { 3 };
            bool isNullOrEmpty = ListUtil.IsNullOrEmpty(singleElementList);

            Assert.IsFalse(isNullOrEmpty, "IsNullOrEmpty should be false");
        }

        [TestMethod]
        public void IsNullOrEmptyWithMultipleElements()
        {
            List<int> multipleElementList = new List<int>() { 3, 4, 5 };
            bool isNullOrEmpty = ListUtil.IsNullOrEmpty(multipleElementList);

            Assert.IsFalse(isNullOrEmpty, "IsNullOrEmpty should be false");
        }

        private struct TestClass
        {
            public TestClass(int x, object y)
            {
                this.x = x;
                this.y = y;
            }

            public int x;
            public object y;
        }

        [TestMethod]
        public void FindFailElementWithParaterizedConstructor()
        {
            TestClass defaultObject = default;

            List<TestClass> list = new List<TestClass>();
            TestClass result = ListUtil.Find(list, i => i.x > 0);

            Assert.AreEqual(defaultObject, result, "");
        }
    }
}
