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

using Services;
using Services.Message;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WpfServices
{
    public class LocalizationModel : BaseModel, ILocalizationModel
    {
        public LocalizationModel(
            in ILocalization localization, 
            in ReadOnlyCollection<string> hiddenLanguageCodes, 
            in ILanguageCodeProvider currentLanguageProvider, 
            in IMessager messager)
        {
            ParamUtil.VerifyNotNull(nameof(hiddenLanguageCodes), hiddenLanguageCodes); //Later, there are specific tests needed for elements.
            m_messager = messager ?? throw new ArgumentNullException(nameof(messager));
            m_currentLanguageProvider = currentLanguageProvider ?? throw new ArgumentNullException(nameof(currentLanguageProvider));

            m_localization = localization ?? throw new ArgumentNullException(nameof(localization));
            if (!string.IsNullOrEmpty(currentLanguageProvider.LanguageCode))
            {
                m_localization.CurrentLanguageCode = currentLanguageProvider.LanguageCode;
            }
            m_currentLanguageProvider.LanguageCode = m_localization.CurrentLanguageCode;

            m_hiddenLanguageCodes = ToLowerCase(hiddenLanguageCodes);

            ValidateHiddenLanguages();
        }

        private List<string> ToLowerCase(IReadOnlyCollection<string> strings)
        {
            List<string> results = new List<string>(strings.Count);
            foreach (string s in strings)
            {
                if (s is null)
                {
                    throw new HiddenLanguageNotFound(null);
                }
                results.Add(s.ToLower());
            }
            return results;
        }

        private void ValidateHiddenLanguages()
        {
            foreach (string languageCode in m_hiddenLanguageCodes)
            {
                bool isLanguageFound = m_localization.IsLanguageCodeSupported(languageCode);
                if (!isLanguageFound)
                {
                    throw new HiddenLanguageNotFound(languageCode);
                }
            }
        }

        public ILocalization Localization
        {
            get
            {
                return m_localization;
            }
        }

        public IEnumerable<string> SelectableLanguages
        {
            get
            {
                List<string> allowedLanguageCodes = new List<string>();
                if (m_localization.SupportedLanguageCodes != null)
                {
                    foreach (string languageCode in m_localization.SupportedLanguageCodes)
                    {
                        if (m_hiddenLanguageCodes != null)
                        {
                            bool isHidden = m_hiddenLanguageCodes.Contains(languageCode.ToLower());
                            if (!isHidden)
                            {
                                allowedLanguageCodes.Add(languageCode);
                            }
                        }
                    }
                }

                return allowedLanguageCodes.ToArray();
            }
        }

        public string CurrentLanguage
        {
            get 
            { 
                return m_localization.CurrentLanguageCode;
            }

            set
            {
                m_localization.CurrentLanguageCode = value;
                m_currentLanguageProvider.LanguageCode = value;

                m_messager.Send(sender: this, new Services.Message.LanguageSelected(value));
                NotifyPropertyChanged("Localization");
            }
        }

        private readonly ILocalization m_localization;
        private readonly List<string> m_hiddenLanguageCodes;
        private readonly ILanguageCodeProvider m_currentLanguageProvider;
        private readonly IMessager m_messager;
    }

    public class HiddenLanguageNotFound : Exception
    {
        public HiddenLanguageNotFound(string language)
        {
            Language = language;
        }

        public string Language { get; private set; }
    }
}
