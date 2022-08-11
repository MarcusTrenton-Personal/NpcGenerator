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
along with this program. If not, see<https://www.gnu.org/licenses/>.*/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;
using Services.Message;
using System;
using System.Collections.Generic;
using WpfServices;

namespace Tests
{
    [TestClass]
    public class LocalizationModelTests
    {
        public const string MartianLanguageCode = "martian"; //Deliberately test mixed cases for language codes
        public const string AtlanteanLanguageCode = "Atlantean";
        public const string MartianText = "Invasion";
        public const string AtlanteanText = "Ancient";

        private class MockLocalization : ILocalization
        {
            public string[] SupportedLanguageCodes { get; set; } = new string[2] { MartianLanguageCode, AtlanteanLanguageCode };
            public string CurrentLanguageCode { get; set; } = MartianLanguageCode;

            public bool IsLanguageCodeSupported(string languageCode)
            {
                return languageCode == MartianLanguageCode || languageCode == AtlanteanLanguageCode;
            }

            public string GetText(string textId, params object[] formatParameters)
            {
                return CurrentLanguageCode == MartianLanguageCode ? MartianText : AtlanteanText;
            }
        }

        private class MockILanguageCode : ILanguageCode
        {
            public string LanguageCode { get; set; }
        }

        private class MockMessager : IMessager
        {
            public void Send<T>(object sender, T message)
            { 
                m_sentMessages.Add(message);
            }
            public void Subscribe<T>(IChannel<T>.Callback callback) { }
            public void Unsubscribe<T>(IChannel<T>.Callback callback) { }

            public List<object> m_sentMessages = new List<object>();
        }

        public LocalizationModelTests()
        {
            m_localization = new MockLocalization();
            m_languageCode = new MockILanguageCode();
            m_messager = new MockMessager();
            m_localizationModel = new LocalizationModel(
                m_localization,
                hiddenLanguageCodes: Array.AsReadOnly(new string[1] { AtlanteanLanguageCode }),
                m_languageCode,
                m_messager);
        }

        [TestMethod]
        public void GetLocalization()
        {
            ILocalization localization = m_localizationModel.Localization;
            Assert.AreEqual(m_localization, localization, "Returned the wrong localization object");
        }

        [TestMethod]
        public void GetSelectableLanguages()
        {
            IEnumerable<string> selectableLanguages = m_localizationModel.SelectableLanguages;
            IEnumerator<string> enumerator = selectableLanguages.GetEnumerator();

            bool hasLanguage = enumerator.MoveNext();
            Assert.IsTrue(hasLanguage, "No selectable languages");
            string language = enumerator.Current;
            Assert.AreEqual(MartianLanguageCode, language, "Wrong language is selectable");

            bool hasSecondLanguage = enumerator.MoveNext();
            Assert.IsFalse(hasSecondLanguage, "Hidden language is selectable");
        }

        [TestMethod]
        public void GetCurrentLanguageCode()
        {
            Assert.AreEqual(MartianLanguageCode, m_localizationModel.CurrentLanguage, "Wrong initial current language");
        }

        [TestMethod]
        public void SetHiddenCurrentLanguageCode()
        {
            m_localizationModel.CurrentLanguage = AtlanteanLanguageCode;
            Assert.AreEqual(AtlanteanLanguageCode, m_localizationModel.CurrentLanguage, "Cannot set hidden language but should be able");

            Assert.AreEqual(1, m_messager.m_sentMessages.Count, "Wrong number of messages sent by LocalizationModel");
            LanguageSelected languageSelectedMessage = m_messager.m_sentMessages[0] as LanguageSelected;
            Assert.IsNotNull(languageSelectedMessage, "LocalizatoinModel sent the wrong message type");
            Assert.AreEqual(AtlanteanLanguageCode, languageSelectedMessage.Language, "Wrong language code sent");
        }

        [TestMethod]
        public void ChangingLanguageChangesText()
        {
            string initialText = m_localizationModel.Localization.GetText("arbitrary");
            Assert.AreEqual(MartianText, initialText, "Wrong text retrieved");

            m_localizationModel.CurrentLanguage = AtlanteanLanguageCode;
            string subsequentText = m_localizationModel.Localization.GetText("arbitrary");
            Assert.AreEqual(AtlanteanText, subsequentText, "Wrong text retrieved");
        }

        [TestMethod]
        public void SetLanguageCodeChangesUserSettings()
        {
            m_localizationModel.CurrentLanguage = AtlanteanLanguageCode;
            Assert.AreEqual(AtlanteanLanguageCode, m_localizationModel.CurrentLanguage, "Cannot set hidden language but should be able");
            Assert.AreEqual(AtlanteanLanguageCode, m_languageCode.LanguageCode, "Language code not saved to underlying object");
        }

        private readonly ILocalization m_localization;
        private readonly LocalizationModel m_localizationModel;
        private readonly ILanguageCode m_languageCode;
        private readonly MockMessager m_messager;
    }
}
