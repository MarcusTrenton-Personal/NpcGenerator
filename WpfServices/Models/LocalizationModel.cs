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
            ILocalization localization, 
            ReadOnlyCollection<string> hiddenLanguageCodes, 
            ILanguageCodeProvider currentLanguageProvider, 
            IMessager messager)
        {
            if (localization is null)
            {
                throw new ArgumentNullException(nameof(localization));
            }
            if (hiddenLanguageCodes is null)
            {
                throw new ArgumentNullException(nameof(hiddenLanguageCodes));
            }
            if (currentLanguageProvider is null)
            {
                throw new ArgumentNullException(nameof(currentLanguageProvider));
            }
            if (messager is null)
            {
                throw new ArgumentNullException(nameof(messager));
            }

            m_localization = localization;
            if (currentLanguageProvider != null && !string.IsNullOrEmpty(currentLanguageProvider.LanguageCode))
            {
                m_localization.CurrentLanguageCode = currentLanguageProvider.LanguageCode;
            }
            m_currentLanguageProvider = currentLanguageProvider;
            m_currentLanguageProvider.LanguageCode = m_localization.CurrentLanguageCode;
            m_messager = messager;

            List<string> lowerCaseHiddenLanguageCodes = new List<string>(hiddenLanguageCodes);
            for(int i = 0; i < lowerCaseHiddenLanguageCodes.Count; i++)
            {
                if (lowerCaseHiddenLanguageCodes[i] is null)
                {
                    throw new HiddenLanguageNotFound(null);
                }
                lowerCaseHiddenLanguageCodes[i] = lowerCaseHiddenLanguageCodes[i].ToLower();
            }
            m_hiddenLanguageCodes = new ReadOnlyCollection<string>(lowerCaseHiddenLanguageCodes);

            ValidateHiddenLanguages();
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
        private readonly ReadOnlyCollection<string> m_hiddenLanguageCodes;
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
