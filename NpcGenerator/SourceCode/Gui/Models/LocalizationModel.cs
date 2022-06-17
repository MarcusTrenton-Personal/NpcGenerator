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

using Services;
using System;
using System.Collections.Generic;

namespace NpcGenerator
{
    public class LocalizationModel : BaseModel, ILocalizationModel
    {
        public LocalizationModel(ILocalization localization, IUserSettings userSettings, string[] hiddenLanguageCodes, string userSettingsPath)
        {
            m_localization = localization;
            m_userSettings = userSettings;
            m_hiddenLanguageCodes = hiddenLanguageCodes;
            m_userSettingsPath = userSettingsPath;
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
                            bool isHidden = Array.Exists(m_hiddenLanguageCodes,
                                hiddenLanguageCode => hiddenLanguageCode.ToLower() == languageCode);
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
                m_userSettings.LanguageCode = value;
                m_userSettings.Save(m_userSettingsPath);

                m_localization.CurrentLanguageCode = value;
                NotifyPropertyChanged("Localization");
            }
        }

        private readonly ILocalization m_localization;
        private readonly string[] m_hiddenLanguageCodes;
        private readonly IUserSettings m_userSettings;
        private readonly string m_userSettingsPath;
    }
}
