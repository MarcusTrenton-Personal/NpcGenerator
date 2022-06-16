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
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tests
{
    [TestClass]
    public class LocalizationTests : FileCreatingTests
    {
        [TestMethod]
        public void ReadSingleLanguage()
        {
            string languageCode = "Martian";
            string languageCodeLowerCase = languageCode.ToLower();
            
            string textId = "window_title";
            string text = "Test Window";
            
            string path = Path.Combine(TestDirectory, "language.csv");
            string sourceText = "ID\tContext\t"+ languageCode + "\n" +
                textId + "\t\t" + text;
            
            File.WriteAllText(path, sourceText);

            Services.Localization localization = new Services.Localization(path, languageCode);

            Assert.AreEqual(languageCodeLowerCase, localization.CurrentLanguageCode, "Current language is wrong");
            Assert.AreEqual(1, localization.SupportedLanguageCodes.Length, "Incorrect number of parsed languages");
            Assert.IsTrue(Array.IndexOf(localization.SupportedLanguageCodes, languageCodeLowerCase) >= 0, "Parsed wrong language code");
            Assert.IsTrue(localization.IsLanguageCodeSupported(languageCode), "Falsely claims default language is not supported");
            Assert.IsFalse(localization.IsLanguageCodeSupported("MissingLanguage"), "Falsely claims missing language is supported");
            Assert.AreEqual(text, localization.GetText(textId), "Fetched the wrong text");

            File.Delete(path);
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

            string path = Path.Combine(TestDirectory, "language.csv");
            string sourceText = "ID\tContext\t" + defaultLanguageCode + "\t"+ languageCode2 + "\n" +
                textId + "\t\t" + martianText + "\t" + dwarvishText;
            
            File.WriteAllText(path, sourceText);

            Services.Localization localization = new Services.Localization(path, defaultLanguageCode);

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

            File.Delete(path);
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

            string path = Path.Combine(TestDirectory, "language.csv");
            File.WriteAllText(path, stringBuilder.ToString());

            Services.Localization localization = new Services.Localization(path, languageCode);

            Assert.AreEqual(languageCodeLowerCase, localization.CurrentLanguageCode, "Current language is wrong");
            Assert.AreEqual(1, localization.SupportedLanguageCodes.Length, "Incorrect number of parsed languages");
            Assert.IsTrue(Array.IndexOf(localization.SupportedLanguageCodes, languageCodeLowerCase) >= 0, "Parsed wrong language code");
            Assert.AreEqual(text1, localization.GetText(textId1), "Fetched the wrong text");
            Assert.AreEqual(text2, localization.GetText(textId2), "Fetched the wrong text");

            File.Delete(path);
        }

        [TestMethod]
        public void UseUnsupportedLanguage()
        {
            string languageCode = "Martian";
            string textId = "window_title";
            string text = "Test Window";

            string path = Path.Combine(TestDirectory, "language.csv");
            string sourceText = "ID\tContext\t" + languageCode + "\n" +
                textId + "\t\t" + text;

            File.WriteAllText(path, sourceText);

            Services.Localization localization = new Services.Localization(path, languageCode);
            bool causedException = false;
            try
            {
                localization.CurrentLanguageCode = "NotFoundLanguage";
            }
            catch (Exception)
            {
                causedException = true;
            }

            Assert.IsTrue(causedException, "Setting invalid language doesn't cause an exception");

            File.Delete(path);
        }

        [TestMethod]
        public void InvalidDefaultLanguage()
        {
            string languageCode = "Martian";
            string textId = "window_title";
            string text = "Test Window";

            string path = Path.Combine(TestDirectory, "language.csv");
            string sourceText = "ID\tContext\t" + languageCode + "\n" +
                textId + "\t\t" + text;

            File.WriteAllText(path, sourceText);

            bool causedException = false;
            try
            {
                Services.Localization localization = new Services.Localization(path, "NotFoundLanguage");
            }
            catch (Exception)
            {
                causedException = true;
            }

            Assert.IsTrue(causedException, "Setting invalid default language doesn't cause an exception");

            File.Delete(path);
        }

        [TestMethod]
        public void InvalidFilePath()
        {
            string languageCode = "Martian";
            string textId = "window_title";
            string text = "Test Window";

            string path = Path.Combine(TestDirectory, "language.csv");
            string sourceText = "ID\tContext\t" + languageCode + "\n" +
                textId + "\t\t" + text;

            File.WriteAllText(path, sourceText);

            bool causedException = false;
            try
            {
                Services.Localization localization = new Services.Localization("InvalidFile.txt", languageCode);
            }
            catch (Exception)
            {
                causedException = true;
            }

            Assert.IsTrue(causedException, "Passing invalid localization file doesn't cause an exception");

            File.Delete(path);
        }

        [TestMethod]
        public void InvalidTextSeparator()
        {
            string languageCode = "Martian";
            string textId = "window_title";
            string text = "Test Window";

            string path = Path.Combine(TestDirectory, "language.csv");
            string sourceText = "ID,Context," + languageCode + "\n" +
                textId + ",," + text;

            File.WriteAllText(path, sourceText);

            bool causedException = false;
            try
            {
                Services.Localization localization = new Services.Localization(path, languageCode);
            }
            catch (Exception)
            {
                causedException = true;
            }

            Assert.IsTrue(causedException, "Not tab separator in localization file doesn't cause an exception");

            File.Delete(path);
        }

        [TestMethod]
        public void DuplicateTextId()
        {
            string languageCode = "Martian";
            string textId = "window_title";
            string text = "Test Window";

            string path = Path.Combine(TestDirectory, "language.csv");
            string sourceText = "ID\tContext\t" + languageCode + "\n" +
                textId + "\t\t" + text +"\n" +
                textId + "\t\t" + text;

            File.WriteAllText(path, sourceText);

            bool causedException = false;
            try
            {
                Services.Localization localization = new Services.Localization(path, languageCode);
            }
            catch (Exception)
            {
                causedException = true;
            }

            Assert.IsTrue(causedException, "Duplicate text ids doesn't cause an exception");

            File.Delete(path);
        }

        [TestMethod]
        public void MissingTitleRow()
        {
            string languageCode = "Martian";
            string textId = "window_title";
            string text = "Test Window";

            string path = Path.Combine(TestDirectory, "language.csv");
            string sourceText = textId + "\t\t" + text;

            File.WriteAllText(path, sourceText);

            bool causedException = false;
            try
            {
                Services.Localization localization = new Services.Localization(path, languageCode);
            }
            catch (Exception)
            {
                causedException = true;
            }

            Assert.IsTrue(causedException, "Missing title row doesn't cause an exception");

            File.Delete(path);
        }

        [TestMethod]
        public void MissingInputColumns()
        {
            string languageCode = "Martian";
            string textId = "window_title";
            string text = "Test Window";

            string path = Path.Combine(TestDirectory, "language.csv");
            string sourceText = "ID\t" + languageCode + "\n" +
                textId + "\t" + text;

            File.WriteAllText(path, sourceText);

            bool causedException = false;
            try
            {
                Services.Localization localization = new Services.Localization(path, languageCode);
            }
            catch (Exception)
            {
                causedException = true;
            }

            Assert.IsTrue(causedException, "Missing context column doesn't cause an exception");

            File.Delete(path);
        }

        [TestMethod]
        public void ExtraInputColumns()
        {
            string languageCode = "Martian";
            string textId = "window_title";
            string text = "Test Window";

            string path = Path.Combine(TestDirectory, "language.csv");
            string sourceText = "ID\tContext\tExtra\t" + languageCode + "\n" +
                textId + "\t\t\t" + text + "\n" +
                textId + "\t\t\t" + text;

            File.WriteAllText(path, sourceText);

            bool causedException = false;
            try
            {
                Services.Localization localization = new Services.Localization(path, languageCode);
            }
            catch (Exception)
            {
                causedException = true;
            }

            Assert.IsTrue(causedException, "Extra column doesn't cause an exception");

            File.Delete(path);
        }

        [TestMethod]
        public void OneLanguageNotFullyTranslated()
        {
            string languageCode = "Martian";
            string textId = "window_title";

            string path = Path.Combine(TestDirectory, "language.csv");
            string sourceText = "ID\tContext\t" + languageCode + "\n" +
                textId + "\t";

            File.WriteAllText(path, sourceText);

            bool causedException = false;
            try
            {
                Services.Localization localization = new Services.Localization(path, languageCode);
            }
            catch (Exception)
            {
                causedException = true;
            }

            Assert.IsTrue(causedException, "Missing translation doesn't cause an exception");

            File.Delete(path);
        }

        [TestMethod]
        public void NoLanguages()
        {
            string languageCode = "Martian";
            string textId = "window_title";

            string path = Path.Combine(TestDirectory, "language.csv");
            string sourceText = "ID\tContext\t\n" +
                textId + "\t";

            File.WriteAllText(path, sourceText);

            bool causedException = false;
            try
            {
                Services.Localization localization = new Services.Localization(path, languageCode);
            }
            catch (Exception)
            {
                causedException = true;
            }

            Assert.IsTrue(causedException, "No languages doesn't cause an exception");

            File.Delete(path);
        }

        [TestMethod]
        public void IncorrectTextId()
        {
            string languageCode = "Martian";
            string textId = "window_title";
            string text = "Test Window";

            string path = Path.Combine(TestDirectory, "language.csv");
            string sourceText = "ID\tContext\t" + languageCode + "\n" +
                textId + "\t\t" + text;

            File.WriteAllText(path, sourceText);

            Services.Localization localization = new Services.Localization(path, languageCode);

            bool causedException = false;
            try
            {
                string translatedText = localization.GetText("MissingStringId");
            }
            catch(Exception)
            {
                causedException = true;
            }
            Assert.IsTrue(causedException, "Asking for missing text doesn't cause exception");

            File.Delete(path);
        }

        [TestMethod]
        public void Format1Param()
        {
            string languageCode = "Martian";
            string textId = "ufo_sightings";
            string text = "Spotted {0} ufos";

            string path = Path.Combine(TestDirectory, "language.csv");
            string sourceText = "ID\tContext\t" + languageCode + "\n" +
                textId + "\t\t" + text;

            File.WriteAllText(path, sourceText);

            Services.Localization localization = new Services.Localization(path, languageCode);

            int ufosSpotted = 3;
            string correctlyFormattedText = string.Format(text, ufosSpotted);
            string candidateText = localization.GetText(textId, ufosSpotted);
            Assert.AreEqual(correctlyFormattedText, candidateText, "String formatted incorrectly");

            File.Delete(path);
        }

        [TestMethod]
        public void Format2Params()
        {
            string languageCode = "Martian";
            string textId = "stars_travelled_and_goal";
            string text = "Visited {0} of {1} in range";

            string path = Path.Combine(TestDirectory, "language.csv");
            string sourceText = "ID\tContext\t" + languageCode + "\n" +
                textId + "\t\t" + text;

            File.WriteAllText(path, sourceText);

            Services.Localization localization = new Services.Localization(path, languageCode);

            int visitedStars = 3;
            int reachableStars = 10;
            string correctlyFormattedText = string.Format(text, visitedStars, reachableStars);
            string candidateText = localization.GetText(textId, visitedStars, reachableStars);
            Assert.AreEqual(correctlyFormattedText, candidateText, "String formatted incorrectly");

            File.Delete(path);
        }

        [TestMethod]
        public void Format0InsteadOf1Params()
        {
            string languageCode = "Martian";
            string textId = "ufo_sightings";
            string text = "Spotted {0} ufos";

            string path = Path.Combine(TestDirectory, "language.csv");
            string sourceText = "ID\tContext\t" + languageCode + "\n" +
                textId + "\t\t" + text;

            File.WriteAllText(path, sourceText);

            Services.Localization localization = new Services.Localization(path, languageCode);

            bool causedException = false;
            try
            {
                string candidateText = localization.GetText(textId);
            }
            catch(Exception)
            {
                causedException = true;
            }
            
            Assert.IsTrue(causedException, "Formatting with too few parameters did not cause an exception");

            File.Delete(path);
        }

        [TestMethod]
        public void Format1InsteadOf2Params()
        {
            string languageCode = "Martian";
            string textId = "stars_travelled_and_goal";
            string text = "Visited {0} of {1} in range";

            string path = Path.Combine(TestDirectory, "language.csv");
            string sourceText = "ID\tContext\t" + languageCode + "\n" +
                textId + "\t\t" + text;

            File.WriteAllText(path, sourceText);

            Services.Localization localization = new Services.Localization(path, languageCode);

            bool causedException = false;
            try
            {
                int visitedStars = 3;
                string candidateText = localization.GetText(textId, visitedStars);
            }
            catch (Exception)
            {
                causedException = true;
            }

            Assert.IsTrue(causedException, "Formatting with too few parameters did not cause an exception");

            File.Delete(path);
        }

        [TestMethod]
        public void Format1InsteadOf0Params()
        {
            string languageCode = "Martian";
            string textId = "window_title";
            string text = "Test Window";

            string path = Path.Combine(TestDirectory, "language.csv");
            string sourceText = "ID\tContext\t" + languageCode + "\n" +
                textId + "\t\t" + text;

            File.WriteAllText(path, sourceText);

            Services.Localization localization = new Services.Localization(path, languageCode);

            Assert.AreEqual(text, localization.GetText(textId, 3), "Fetched the wrong text");

            File.Delete(path);
        }

        [TestMethod]
        public void Format2InsteadOf1Params()
        {
            string languageCode = "Martian";
            string textId = "ufo_sightings";
            string text = "Spotted {0} ufos";

            string path = Path.Combine(TestDirectory, "language.csv");
            string sourceText = "ID\tContext\t" + languageCode + "\n" +
                textId + "\t\t" + text;

            File.WriteAllText(path, sourceText);

            Services.Localization localization = new Services.Localization(path, languageCode);

            int ufosSpotted = 3;
            string correctlyFormattedText = string.Format(text, ufosSpotted);
            string candidateText = localization.GetText(textId, ufosSpotted, 19);
            Assert.AreEqual(correctlyFormattedText, candidateText, "String formatted incorrectly");

            File.Delete(path);
        }
    }
}
