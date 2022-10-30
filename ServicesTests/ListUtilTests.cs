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
using System.Drawing;

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
            List<int> list = new List<int>() { -1, -2, 9, -3, -4 };
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

        private struct TestStruct
        {
            public TestStruct(int x, object y)
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
            TestStruct defaultObject = default;

            List<TestStruct> list = new List<TestStruct>();
            TestStruct result = ListUtil.Find(list, i => i.x > 0);

            Assert.AreEqual(defaultObject, result, "Failed find did not return default value");
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

            Assert.AreEqual(ListUtil.INDEX_NOT_FOUND, result, "Should not find an element in an empty list");
        }

        [TestMethod]
        public void IndexOfNotFound()
        {
            IReadOnlyList<int> list = new List<int>() { 1, 1, 2 }.AsReadOnly();
            int result = ListUtil.IndexOf(list: list, test: x => x < 0);

            Assert.AreEqual(ListUtil.INDEX_NOT_FOUND, result, "Incorrectly found an element despite the test");
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

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void DistinctPreserveOrderNullList()
        {
            ListUtil.DistinctPreserveOrder<int>(null);
        }

        [TestMethod]
        public void DistinctPreserveOrderEmptyList()
        {
            ListUtil.DistinctPreserveOrder(new List<int>());
        }

        [TestMethod]
        public void DistinctPreserveOrderOneElement()
        {
            const int E0 = 3;

            IReadOnlyList<int> result = ListUtil.DistinctPreserveOrder(new List<int>() { E0 });

            Assert.AreEqual(1, result.Count, "Wrong number of elements in list");
            Assert.AreEqual(E0, result[0], "Wrong element in list");
        }

        [TestMethod]
        public void DistinctPreserveOrderTwoElementsNoDupes()
        {
            const int E0 = 3;
            const int E1 = 1;

            IReadOnlyList<int> result = ListUtil.DistinctPreserveOrder(new List<int>() { E0, E1 });

            Assert.AreEqual(2, result.Count, "Wrong number of elements in list");
            Assert.AreEqual(E0, result[0], "Wrong element in list");
            Assert.AreEqual(E1, result[1], "Wrong element in list");
        }

        [TestMethod]
        public void DistinctPreserveOrderTwoDupeElementsInt()
        {
            const int E0 = 3;
            const int E1 = E0;

            IReadOnlyList<int> result = ListUtil.DistinctPreserveOrder(new List<int>() { E0, E1 });

            Assert.AreEqual(1, result.Count, "Wrong number of elements in list");
            Assert.AreEqual(E0, result[0], "Wrong element in list");
        }

        [TestMethod]
        public void DistinctPreserveOrderTwoDupeElementsString()
        {
            const string E0 = "Blue";
            const string E1 = E0;

            IReadOnlyList<string> result = ListUtil.DistinctPreserveOrder(new List<string>() { E0, E1 });

            Assert.AreEqual(1, result.Count, "Wrong number of elements in list");
            Assert.AreEqual(E0, result[0], "Wrong element in list");
        }

        [TestMethod]
        public void DistinctPreserveOrderTwoDupeElementsObjectRef()
        {
            TestStruct e0 = new TestStruct(3, new object());
            TestStruct e1 = e0;

            IReadOnlyList<TestStruct> result = ListUtil.DistinctPreserveOrder(new List<TestStruct>() { e0, e1 });

            Assert.AreEqual(1, result.Count, "Wrong number of elements in list");
            Assert.AreEqual(e0, result[0], "Wrong element in list");
        }

        [TestMethod]
        public void DistinctPreserveOrderTwoDupeElementsObjectValue()
        {
            Point e0 = new Point(3, 2);
            Point e1 = new Point(3, 2);

            IReadOnlyList<Point> result = ListUtil.DistinctPreserveOrder(new List<Point>() { e0, e1 });

            Assert.AreEqual(1, result.Count, "Wrong number of elements in list");
            Assert.AreEqual(e0, result[0], "Wrong element in list");
        }

        [TestMethod]
        public void DistinctPreserveOrderLostListOfElementsWithDupes()
        {
            const string E0 = "Blue";
            const string E1 = "Red";
            const string E2 = "Red";
            const string E3 = "Blue";
            const string E4 = "Green";
            const string E5 = "Blue";
            const string E6 = "Black";
            const string E7 = "White";
            const string E8 = "Red";

            IReadOnlyList<string> result = ListUtil.DistinctPreserveOrder(new List<string>() { E0, E1, E2, E3, E4, E5, E6, E7, E8 });

            Assert.AreEqual(5, result.Count, "Wrong number of elements in list");
            Assert.AreEqual(E0, result[0], "Wrong element in list");
            Assert.AreEqual(E1, result[1], "Wrong element in list");
            Assert.AreEqual(E4, result[2], "Wrong element in list");
            Assert.AreEqual(E6, result[3], "Wrong element in list");
            Assert.AreEqual(E7, result[4], "Wrong element in list");
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void DistinctPreserveOrderNullElement()
        {
            object e0 = new object();
            object e1 = null;

            ListUtil.DistinctPreserveOrder(new List<object>() { e0, e1 });
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void FindAllNullList()
        {
            ListUtil.FindAll<object>(null, test: x => x is null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void FindAllNullTest()
        {
            ListUtil.FindAll(new List<int>() { 3, 4 }, test: null);
        }

        [TestMethod]
        public void FindAllEmptyList()
        {
            IReadOnlyList<string> result = ListUtil.FindAll(new List<string>(), test: x => x.Contains('x'));

            Assert.AreEqual(0, result.Count, "Resulting list is the wrong size");
        }

        [TestMethod]
        public void FindAllNoElementsFound()
        {
            IReadOnlyList<int> result = ListUtil.FindAll(new List<int>() { -1, -2, -3 }, test: x => x > 0);

            Assert.AreEqual(0, result.Count, "Resulting list is the wrong size");
        }

        [TestMethod]
        public void FindAll1ElementFound()
        {
            const int FOUND_ELEMENT = 2;

            IReadOnlyList<int> result = ListUtil.FindAll(new List<int>() { -1, FOUND_ELEMENT, -3 }, test: x => x > 0);

            Assert.AreEqual(1, result.Count, "Resulting list is the wrong size");
            Assert.AreEqual(FOUND_ELEMENT, result[0], "Resulting list has the wrong element");
        }

        [TestMethod]
        public void FindAllManyElementsFound()
        {
            const int FOUND_ELEMENT0 = 2;
            const int FOUND_ELEMENT1 = 4;
            const int FOUND_ELEMENT2 = 5;
            List<int> list = new List<int>() { -1, FOUND_ELEMENT0, -3, FOUND_ELEMENT1, FOUND_ELEMENT2 };

            List<int> result = ListUtil.FindAll(list, test: x => x > 0);

            Assert.AreEqual(3, result.Count, "Resulting list is the wrong size");
            Assert.IsTrue(result.Contains(FOUND_ELEMENT0), "Resulting list has the wrong elements");
            Assert.IsTrue(result.Contains(FOUND_ELEMENT1), "Resulting list has the wrong elements");
            Assert.IsTrue(result.Contains(FOUND_ELEMENT2), "Resulting list has the wrong elements");
        }
        //FindAllManyElementsFound
    }
}
