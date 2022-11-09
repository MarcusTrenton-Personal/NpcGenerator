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
    public class ParamUtilTests
    {
        [TestMethod]
        public void VerifyStringHasContentWhenValid()
        {
            string value = "Red";
            ParamUtil.VerifyHasContent(nameof(value), value);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void VerifyStringHasContentWhenNull()
        {
            string value = null;
            ParamUtil.VerifyHasContent(nameof(value), value);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void VerifyStringHasContentWhenEmpty()
        {
            string value = string.Empty;
            ParamUtil.VerifyHasContent(nameof(value), value);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void VerifyStringHasContentWhenWhitespace()
        {
            string value = " ";
            ParamUtil.VerifyHasContent(nameof(value), value);
        }

        [TestMethod]
        public void VerifyNotNullWhenValid()
        {
            object value = new object();
            ParamUtil.VerifyNotNull(nameof(value), value);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void VerifyNotNullWhenNull()
        {
            object value = null;
            ParamUtil.VerifyNotNull(nameof(value), value);
        }

        [TestMethod]
        public void VerifyElementsAreNotNullWhenValidNotEmptyArray()
        {
            object[] values = new object[1] { new object() };
            ParamUtil.VerifyElementsAreNotNull(nameof(values), values);
        }

        [TestMethod]
        public void VerifyElementsAreNotNullWhenEmptyArray()
        {
            object[] values = Array.Empty<object>();
            ParamUtil.VerifyElementsAreNotNull(nameof(values), values);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void VerifyElementsAreNotNullWhenArrayIsNull()
        {
            object[] values = null;
            ParamUtil.VerifyElementsAreNotNull("values", values);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void VerifyElementsAreNotNullWhenArrayHasNulls()
        {
            object[] values = new object[1] { null };
            ParamUtil.VerifyElementsAreNotNull(nameof(values), values);
        }

        [TestMethod]
        public void VerifyElementsAreNotNullWhenValidNotEmptyList()
        {
            List<object> values = new List<object> { new object() };
            ParamUtil.VerifyElementsAreNotNull(nameof(values), values);
        }

        [TestMethod]
        public void VerifyElementsAreNotNullWhenValidNotEmptyListOfInts()
        {
            List<int> values = new List<int> { 0 };
            ParamUtil.VerifyElementsAreNotNull(nameof(values), values);
        }

        [TestMethod]
        public void VerifyWholeNumberOf1()
        {
            int x = 1;
            ParamUtil.VerifyWholeNumber(nameof(x), x);
        }

        [TestMethod]
        public void VerifyWholeNumberOf0()
        {
            int x = 0;
            ParamUtil.VerifyWholeNumber(nameof(x), x);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void VerifyWholeNumberOfNegative1()
        {
            int x = -1;
            ParamUtil.VerifyWholeNumber(nameof(x), x);
        }
    }
}
