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
using System;
using System.IO;
using System.Text;

namespace Tests
{
    [TestClass]
    public class LocalizationTests
    {
        [TestMethod]
        public void ReadSingleLanguage()
        {
            string languageCode = "Martian";
            string languageCodeLowerCase = languageCode.ToLower();
            
            string textId = "window_title";
            string text = "Test Window";

            string sourceText = "ID\tContext\t"+ languageCode + "\n" +
                textId + "\t\t" + text;

            Services.Localization localization = new Services.Localization(sourceText, languageCode);

            Assert.AreEqual(languageCodeLowerCase, localization.CurrentLanguageCode, "Current language is wrong");
            Assert.AreEqual(1, localization.SupportedLanguageCodes.Length, "Incorrect number of parsed languages");
            Assert.IsTrue(Array.IndexOf(localization.SupportedLanguageCodes, languageCodeLowerCase) >= 0, "Parsed wrong language code");
            Assert.IsTrue(localization.IsLanguageCodeSupported(languageCode), "Falsely claims default language is not supported");
            Assert.IsFalse(localization.IsLanguageCodeSupported("MissingLanguage"), "Falsely claims missing language is supported");
            Assert.AreEqual(text, localization.GetText(textId), "Fetched the wrong text");
        }

        [TestMethod]
        public void ReadMultipleLanguages()
        {
            string defaultLanguageCode = "Martian";
            string defaultLanguageCodeLowerCase = defaultLanguageCode.ToLower();
            string languageCode2 = "Dwarvish";
            string languageCode2LowerCase = languageCode2.ToLower();
            
            string textId = "window_title";
            string martianText = "Test Window";
            string dwarvishText = "Mock Window";

            string sourceText = "ID\tContext\t" + defaultLanguageCode + "\t"+ languageCode2 + "\n" +
                textId + "\t\t" + martianText + "\t" + dwarvishText;

            Services.Localization localization = new Services.Localization(sourceText, defaultLanguageCode);

            Assert.AreEqual(defaultLanguageCodeLowerCase, localization.CurrentLanguageCode, "Current language is wrong");
            Assert.AreEqual(2, localization.SupportedLanguageCodes.Length, "Incorrect number of parsed languages");
            Assert.IsTrue(Array.IndexOf(localization.SupportedLanguageCodes, defaultLanguageCodeLowerCase) >= 0, "Parsed wrong language code");
            Assert.IsTrue(Array.IndexOf(localization.SupportedLanguageCodes, languageCode2LowerCase) >= 0, "Parsed wrong language code");
            Assert.IsTrue(localization.IsLanguageCodeSupported(defaultLanguageCode), "Falsely claims default language is not supported");
            Assert.IsTrue(localization.IsLanguageCodeSupported(languageCode2), "Falsely claims default language is not supported");
            Assert.IsFalse(localization.IsLanguageCodeSupported("MissingLanguage"), "Falsely claims missing language is supported");
            Assert.AreEqual(martianText, localization.GetText(textId), "Fetched the wrong text");

            localization.CurrentLanguageCode = languageCode2;

            Assert.AreEqual(languageCode2LowerCase, localization.CurrentLanguageCode, "Current language is wrong");
            Assert.AreEqual(dwarvishText, localization.GetText(textId), "Fetched the wrong text");
        }

        [TestMethod]
        public void LargeLanguageFile()
        {
            string languageCode = "Martian";
            string languageCodeLowerCase = languageCode.ToLower();

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("ID\tContext\t" + languageCode);
            for(int i = 0; i < 500; i++)
            {
                stringBuilder.AppendLine("Id" + i +"\t\tText" + i);
            }

            string textId1 = "window_title";
            string text1 = "Test Window";
            stringBuilder.AppendLine(textId1+"\t\t" + text1);

            for (int i = 500; i < 1000; i++)
            {
                stringBuilder.AppendLine("Id" + i + "\t\tText" + i);
            }

            string textId2 = "exit_greeting";
            string text2 = "Goodbye";
            stringBuilder.Append(textId2 + "\t\t" + text2);

            Services.Localization localization = new Services.Localization(stringBuilder.ToString(), languageCode);

            Assert.AreEqual(languageCodeLowerCase, localization.CurrentLanguageCode, "Current language is wrong");
            Assert.AreEqual(1, localization.SupportedLanguageCodes.Length, "Incorrect number of parsed languages");
            Assert.IsTrue(Array.IndexOf(localization.SupportedLanguageCodes, languageCodeLowerCase) >= 0, "Parsed wrong language code");
            Assert.AreEqual(text1, localization.GetText(textId1), "Fetched the wrong text");
            Assert.AreEqual(text2, localization.GetText(textId2), "Fetched the wrong text");
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void UseUnsupportedLanguage()
        {
            string languageCode = "Martian";
            string textId = "window_title";
            string text = "Test Window";

            string sourceText = "ID\tContext\t" + languageCode + "\n" +
                textId + "\t\t" + text;
            _ = new Services.Localization(sourceText, languageCode)
            {
                CurrentLanguageCode = "NotFoundLanguage"
            };
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void InvalidDefaultLanguage()
        {
            string languageCode = "Martian";
            string textId = "window_title";
            string text = "Test Window";

            string sourceText = "ID\tContext\t" + languageCode + "\n" +
                textId + "\t\t" + text;

            new Services.Localization(sourceText, "NotFoundLanguage");
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void InvalidTextSeparator()
        {
            string languageCode = "Martian";
            string textId = "window_title";
            string text = "Test Window";

            string sourceText = "ID,Context," + languageCode + "\n" +
                textId + ",," + text;

            new Services.Localization(sourceText, languageCode);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void DuplicateTextId()
        {
            string languageCode = "Martian";
            string textId = "window_title";
            string text = "Test Window";

            string sourceText = "ID\tContext\t" + languageCode + "\n" +
                textId + "\t\t" + text +"\n" +
                textId + "\t\t" + text;

            new Services.Localization(sourceText, languageCode);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void MissingTitleRow()
        {
            string languageCode = "Martian";
            string textId = "window_title";
            string text = "Test Window";

            string sourceText = textId + "\t\t" + text;

            new Services.Localization(sourceText, languageCode);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void MissingInputColumns()
        {
            string languageCode = "Martian";
            string textId = "window_title";
            string text = "Test Window";

            string sourceText = "ID\t" + languageCode + "\n" +
                textId + "\t" + text;

            new Services.Localization(sourceText, languageCode);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void ExtraInputColumns()
        {
            string languageCode = "Martian";
            string textId = "window_title";
            string text = "Test Window";

            string sourceText = "ID\tContext\tExtra\t" + languageCode + "\n" +
                textId + "\t\t\t" + text + "\n" +
                textId + "\t\t\t" + text;

            new Services.Localization(sourceText, languageCode);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void OneLanguageNotFullyTranslated()
        {
            string languageCode = "Martian";
            string textId = "window_title";

            string sourceText = "ID\tContext\t" + languageCode + "\n" +
                textId + "\t";

            new Services.Localization(sourceText, languageCode);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void NoLanguages()
        {
            string languageCode = "Martian";
            string textId = "window_title";

            string sourceText = "ID\tContext\t\n" +
                textId + "\t";

            new Services.Localization(sourceText, languageCode);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void IncorrectTextId()
        {
            string languageCode = "Martian";
            string textId = "window_title";
            string text = "Test Window";

            string sourceText = "ID\tContext\t" + languageCode + "\n" +
                textId + "\t\t" + text;

            Services.Localization localization = new Services.Localization(sourceText, languageCode);

            localization.GetText("MissingStringId");
        }

        [TestMethod]
        public void Format1Param()
        {
            string languageCode = "Martian";
            string textId = "ufo_sightings";
            string text = "Spotted {0} ufos";

            string sourceText = "ID\tContext\t" + languageCode + "\n" +
                textId + "\t\t" + text;

            Services.Localization localization = new Services.Localization(sourceText, languageCode);

            int ufosSpotted = 3;
            string correctlyFormattedText = string.Format(text, ufosSpotted);
            string candidateText = localization.GetText(textId, ufosSpotted);
            Assert.AreEqual(correctlyFormattedText, candidateText, "String formatted incorrectly");
        }

        [TestMethod]
        public void Format2Params()
        {
            string languageCode = "Martian";
            string textId = "stars_travelled_and_goal";
            string text = "Visited {0} of {1} in range";

            string sourceText = "ID\tContext\t" + languageCode + "\n" +
                textId + "\t\t" + text;

            Services.Localization localization = new Services.Localization(sourceText, languageCode);

            int visitedStars = 3;
            int reachableStars = 10;
            string correctlyFormattedText = string.Format(text, visitedStars, reachableStars);
            string candidateText = localization.GetText(textId, visitedStars, reachableStars);
            Assert.AreEqual(correctlyFormattedText, candidateText, "String formatted incorrectly");
        }

        [TestMethod, ExpectedException(typeof(FormatException))]
        public void Format0InsteadOf1Params()
        {
            string languageCode = "Martian";
            string textId = "ufo_sightings";
            string text = "Spotted {0} ufos";

            string sourceText = "ID\tContext\t" + languageCode + "\n" +
                textId + "\t\t" + text;

            Services.Localization localization = new Services.Localization(sourceText, languageCode);

            localization.GetText(textId);
        }

        [TestMethod, ExpectedException(typeof(FormatException))]
        public void Format1InsteadOf2Params()
        {
            string languageCode = "Martian";
            string textId = "stars_travelled_and_goal";
            string text = "Visited {0} of {1} in range";

            string sourceText = "ID\tContext\t" + languageCode + "\n" +
                textId + "\t\t" + text;

            Services.Localization localization = new Services.Localization(sourceText, languageCode);

            int visitedStars = 3;
            localization.GetText(textId, visitedStars);
        }

        [TestMethod]
        public void Format1InsteadOf0Params()
        {
            string languageCode = "Martian";
            string textId = "window_title";
            string text = "Test Window";

            string sourceText = "ID\tContext\t" + languageCode + "\n" +
                textId + "\t\t" + text;

            Services.Localization localization = new Services.Localization(sourceText, languageCode);

            Assert.AreEqual(text, localization.GetText(textId, 3), "Fetched the wrong text");
        }

        [TestMethod]
        public void Format2InsteadOf1Params()
        {
            string languageCode = "Martian";
            string textId = "ufo_sightings";
            string text = "Spotted {0} ufos";

            string sourceText = "ID\tContext\t" + languageCode + "\n" +
                textId + "\t\t" + text;

            Services.Localization localization = new Services.Localization(sourceText, languageCode);

            int ufosSpotted = 3;
            string correctlyFormattedText = string.Format(text, ufosSpotted);
            string candidateText = localization.GetText(textId, ufosSpotted, 19);
            Assert.AreEqual(correctlyFormattedText, candidateText, "String formatted incorrectly");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void NullText()
        {
            new Services.Localization(localizationText: null, "martian");
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void EmptyText()
        {
            new Services.Localization(localizationText: String.Empty, "martian");
        }
    }
}
