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

namespace ServicesTests
{
    [TestClass]
    public class ParamUtilTests
    {
        [TestMethod]
        public void VerifyStringHasContentWhenValid()
        {
            string value = "Red";
            ParamUtil.VerifyStringHasContent(nameof(value), value);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void VerifyStringHasContentWhenNull()
        {
            string value = null;
            ParamUtil.VerifyStringHasContent(nameof(value), value);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void VerifyStringHasContentWhenEmpty()
        {
            string value = string.Empty;
            ParamUtil.VerifyStringHasContent(nameof(value), value);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void VerifyStringHasContentWhenWhitespace()
        {
            string value = " ";
            ParamUtil.VerifyStringHasContent(nameof(value), value);
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
        public void VerifyArrayElementsNotNullWhenValidNotEmptyArray()
        {
            object[] values = new object[1] { new object() };
            ParamUtil.VerifyArrayElementsNotNull(nameof(values), values);
        }

        [TestMethod]
        public void VerifyArrayElementsNotNullWhenEmptyArray()
        {
            object[] values = Array.Empty<object>();
            ParamUtil.VerifyArrayElementsNotNull(nameof(values), values);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void VerifyArrayElementsNotNullWhenArrayIsNull()
        {
            object[] values = null;
            ParamUtil.VerifyArrayElementsNotNull("values", values);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void VerifyArrayElementsNotNullWhenArrayHasNulls()
        {
            object[] values = new object[1] { null };
            ParamUtil.VerifyArrayElementsNotNull(nameof(values), values);
        }
    }
}
