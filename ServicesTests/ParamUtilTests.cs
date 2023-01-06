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

        [TestMethod]
        public void VerifyInternalDictionaryKeyExistsElementFound()
        {
            Dictionary<int, string> dictionary = new Dictionary<int, string>
            {
                [1] = "One"
            };

            int key = 1;
            ParamUtil.VerifyInternalDictionaryKeyExists(nameof(dictionary), dictionary, nameof(key), key);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void VerifyInternalDictionaryKeyExistsElementMissing()
        {
            Dictionary<int, string> dictionary = new Dictionary<int, string>
            {
                [1] = "One"
            };

            int key = 2;
            ParamUtil.VerifyInternalDictionaryKeyExists(nameof(dictionary), dictionary, nameof(key), key);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void VerifyInternalDictionaryKeyExistsDictionaryIsNull()
        {
            Dictionary<int, string> dictionary = null;

            int key = 1;
            ParamUtil.VerifyInternalDictionaryKeyExists(nameof(dictionary), dictionary, nameof(key), key);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void VerifyStringMatchesPatternNull()
        {
            string text = "abc";
            ParamUtil.VerifyMatchesPattern(nameof(text), text, pattern: null, "String does not match pattern");
        }

        [TestMethod]
        public void VerifyStringMatchesPatternEmpty()
        {
            string text = "abc";
            ParamUtil.VerifyMatchesPattern(nameof(text), text, pattern: "", "String does not match pattern");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void VerifyStringMatchesPatternValueNull()
        {
            ParamUtil.VerifyMatchesPattern("text",value: null, "[+-]?([0-9]*[.])?[0-9]+", "String does not match pattern");
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void VerifyStringMatchesPatternValueEmpty()
        {
            ParamUtil.VerifyMatchesPattern("text", value: String.Empty, "[+-]?([0-9]*[.])?[0-9]+", "String does not match pattern");
        }

        [TestMethod]
        public void VerifyStringMatchesPatternValueEmptyPatternEmpty()
        {
            ParamUtil.VerifyMatchesPattern("text", String.Empty, pattern: String.Empty, "String does not match pattern");
        }

        [TestMethod]
        public void VerifyStringMatchesPatternThatExists()
        {
            string text = "-1.3";
            ParamUtil.VerifyMatchesPattern(nameof(text), text, "[+-]?([0-9]*[.])?[0-9]+", "String does not match pattern");
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void VerifyStringMatchesPatternThatDoesNotExist()
        {
            string text = "abc";
            ParamUtil.VerifyMatchesPattern(nameof(text), text, "[+-]?([0-9]*[.])?[0-9]+", "String does not match pattern");
        }
    }
}
