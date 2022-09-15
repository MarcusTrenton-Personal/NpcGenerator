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
        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void FindWithNullList()
        {
            ListUtil.Find<int>(list: null, test: x => x > 0);
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

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void FindWithNullTest()
        {
            List<int> list = new List<int>() { -1, 2, -6, 3, -4 };
            ListUtil.Find(list, test: null);
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
        public void FindFailElementWithParameterizedConstructor()
        {
            TestClass defaultObject = default;

            List<TestClass> list = new List<TestClass>();
            TestClass result = ListUtil.Find(list, i => i.x > 0);

            Assert.AreEqual(defaultObject, result, "");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ConvertAllWithNullList()
        {
            IReadOnlyList<string> result = ListUtil.ConvertAll<string, int>(list: null, converter: number => number.ToString());
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ConvertAllWithNullConverter()
        {
            IReadOnlyList<int> list = new List<int>() { 1, 2, 3 };
            ListUtil.ConvertAll<string, int>(list: list, converter: null);
        }

        [TestMethod]
        public void ConvertAllWithEmptyList()
        {
            IReadOnlyList<int> list = new List<int>();
            IReadOnlyList<string> result = ListUtil.ConvertAll(list: list, converter: number => number.ToString());

            Assert.AreEqual(list.Count, result.Count, "Resulting list should be empty");
        }

        [TestMethod]
        public void ConvertAllIntToString()
        {
            IReadOnlyList<int> list = new List<int>() { 1, 2, 3 };
            IReadOnlyList<string> result = ListUtil.ConvertAll(list: list, converter: number => number.ToString());

            Assert.AreEqual(list.Count, result.Count, "Resulting list has wrong number of elements");
            for (int i = 0; i < list.Count; ++i)
            {
                Assert.AreEqual(list[i].ToString(), result[i], "Converter produces incorrect results");
            }
        }

        [TestMethod]
        public void ConvertAllStringToString()
        {
            IReadOnlyList<string> list = new List<string>() { "Chaos" };
            IReadOnlyList<string> result = ListUtil.ConvertAll(list: list, converter: element => element.ToUpper());

            Assert.AreEqual(list.Count, result.Count, "Resulting list has wrong number of elements");
            for (int i = 0; i < list.Count; ++i)
            {
                Assert.AreEqual(list[i].ToUpper(), result[i], "Converter produces incorrect results");
            }
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void IndexOfNullList()
        {
            ListUtil.IndexOf<int>(list: null, x => x > 0);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void IndexOfNullTest()
        {
            IReadOnlyList<float> list = new List<float>() { 1.0f, 1.1f, 1.2f }.AsReadOnly();
            ListUtil.IndexOf(list: list, test: null);
        }

        [TestMethod]
        public void IndexOfEmptyList()
        {
            IReadOnlyList<object> list = new List<object>().AsReadOnly();
            int result = ListUtil.IndexOf(list: list, test: x => x.Equals(null));

            Assert.AreEqual(ListUtil.NOT_FOUND, result, "Should not find an element in an empty list");
        }

        [TestMethod]
        public void IndexOfNotFound()
        {
            IReadOnlyList<int> list = new List<int>() { 1, 1, 2 }.AsReadOnly();
            int result = ListUtil.IndexOf(list: list, test: x => x < 0);

            Assert.AreEqual(ListUtil.NOT_FOUND, result, "Incorrectly found an element despite the test");
        }

        [TestMethod]
        public void IndexOfFoundFirst()
        {
            IReadOnlyList<int> list = new List<int>() { 1, -3, -2 }.AsReadOnly();
            int result = ListUtil.IndexOf(list: list, test: x => x > 0);

            Assert.AreEqual(0, result, "Found element at wrong index");
        }

        [TestMethod]
        public void IndexOfFoundLast()
        {
            IReadOnlyList<int> list = new List<int>() { -1, -3, 2 }.AsReadOnly();
            int result = ListUtil.IndexOf(list: list, test: x => x > 0);

            Assert.AreEqual(2, result, "Found element at wrong index");
        }

        [TestMethod]
        public void IndexOfFoundMultiple()
        {
            IReadOnlyList<string> list = new List<string>() { "Ant", "Bear", "Antelope" }.AsReadOnly();
            int result = ListUtil.IndexOf(list: list, test: x => x.StartsWith('A'));

            Assert.AreEqual(0, result, "Found element at wrong index");
        }
    }
}
