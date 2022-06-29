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
using Services;
using System;
using System.Globalization;
using WpfServices;

namespace Tests
{
    [TestClass]
    public class LocalizationConverterTests
    {
        const string noParameterTextId = "noParameters";
        const string noParameterText = "basicText";

        const string oneParameterTextId = "oneParameter";
        const string oneParameterText = "found {0}";

        const string twoParametersTextId = "twoParameters";
        const string twoParametersText = "{0} of the {1}";
        readonly ILocalization m_localization = new MockLocalization();
        readonly LocalizationConverter m_converter = new LocalizationConverter();

        private class MockLocalization : ILocalization
        {
            public string[] SupportedLanguageCodes { get; } = null;
            public string CurrentLanguageCode { get; set; } = null;

            public bool IsLanguageCodeSupported(string languageCode)
            {
                return false;
            }

            public string GetText(string textId, params object[] formatParameters)
            {
                return textId switch
                {
                    noParameterTextId => noParameterText,
                    oneParameterTextId => string.Format(oneParameterText, formatParameters),
                    twoParametersTextId => string.Format(twoParametersText, formatParameters),
                    _ => throw new ArgumentException(),
                };
            }
        }

        [TestMethod]
        public void GetTextWithoutFormating()
        {
            object[] values = { m_localization };
            string text = (string)m_converter.Convert(
                values: values,
                targetType: values.GetType(),
                parameter: noParameterTextId,
                culture: CultureInfo.InvariantCulture);

            Assert.AreEqual(noParameterText, text, "Incorrect text returned");
        }

        [TestMethod]
        public void GetTextWith1Parameter()
        {
            string parameter = "Bigfoot";
            string expectedText = string.Format(oneParameterText, parameter);

            object[] values = { m_localization, parameter };
            string text = (string)m_converter.Convert(
                values: values,
                targetType: values.GetType(),
                parameter: oneParameterTextId,
                culture: CultureInfo.InvariantCulture);

            Assert.AreEqual(expectedText, text, "Incorrect text returned");
        }

        [TestMethod]
        public void GetTextWith2Parameters()
        {
            string parameter1 = "Hero";
            string parameter2 = "Day";
            string expectedText = string.Format(twoParametersText, parameter1, parameter2);

            object[] values = { m_localization, parameter1, parameter2 };
            string text = (string)m_converter.Convert(
                values: values,
                targetType: values.GetType(),
                parameter: twoParametersTextId,
                culture: CultureInfo.InvariantCulture);

            Assert.AreEqual(expectedText, text, "Incorrect text returned");
        }

        [TestMethod]
        public void NullTextId()
        {
            bool causedException = false;
            try 
            {
                object[] values = { m_localization };
                string text = (string)m_converter.Convert(
                    values: values,
                    targetType: values.GetType(),
                    parameter: null,
                    culture: CultureInfo.InvariantCulture);
            }
            catch(Exception)
            {
                causedException = true;
            }

            Assert.IsTrue(causedException, "Missing text id didn't cause an exception");
        }

        [TestMethod]
        public void TextIdNotFound()
        {
            bool causedException = false;
            try
            {
                object[] values = { m_localization };
                string text = (string)m_converter.Convert(
                    values: values,
                    targetType: values.GetType(),
                    parameter: "missing_text_id",
                    culture: CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                causedException = true;
            }

            Assert.IsTrue(causedException, "Not found text id didn't cause an exception");
        }

        [TestMethod]
        public void MissingParameter()
        {
            bool causedException = false;
            try
            {
                object[] values = { m_localization };
                string text = (string)m_converter.Convert(
                    values: values,
                    targetType: values.GetType(),
                    parameter: oneParameterTextId,
                    culture: CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                causedException = true;
            }

            Assert.IsTrue(causedException, "Converting text without enough parameters didn't cause an exception");
        }

        [TestMethod]
        public void TooManyParameters()
        {
            string parameter1 = "Bigfoot";
            string parameter2 = "Venus";
            string expectedText = string.Format(oneParameterText, parameter1, parameter2);

            object[] values = { m_localization, parameter1, parameter2 };
            string text = (string)m_converter.Convert(
                values: values,
                targetType: values.GetType(),
                parameter: oneParameterTextId,
                culture: CultureInfo.InvariantCulture);

            Assert.AreEqual(expectedText, text, "Incorrect text returned");
        }

        [TestMethod]
        public void DesignTimeLocalization()
        {
            object[] values = { null };
            string text = (string)m_converter.Convert(
                values: values,
                targetType: values.GetType(),
                parameter: noParameterTextId,
                culture: CultureInfo.InvariantCulture);

            Assert.AreEqual(noParameterTextId, text, "Null localization didn't return the parameter id as the result text");
        }

        [TestMethod]
        public void EmptyValues()
        {
            bool causedException = false;
            try
            {
                object[] values = {};
                string text = (string)m_converter.Convert(
                    values: values,
                    targetType: values.GetType(),
                    parameter: noParameterTextId,
                    culture: CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                causedException = true;
            }

            Assert.IsTrue(causedException, "Empty values didn't cause an exception");
        }

        [TestMethod]
        public void ConvertBackNotImplemented()
        {
            bool causedException = false;
            try
            {
                Type[] targetTypes = { causedException.GetType() };
                object[] originals = m_converter.ConvertBack(
                    value: noParameterText,
                    targetTypes: targetTypes,
                    parameter: noParameterTextId,
                    culture: CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                causedException = true;
            }

            Assert.IsTrue(causedException, "ConvertBack stub didn't cause an exception");
        }
    }
}
