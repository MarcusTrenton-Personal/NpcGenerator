﻿/*Copyright(C) 2022 Marcus Trenton, marcus.trenton@gmail.com

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

using System;
using System.Collections.Generic;
using System.IO;

namespace Services
{
    public class Localization : ILocalization
    {
        //The localization file must have tab-separated format:
        //ID        Context LanguageCode1   LanguageCode2   ...
        //text_id   context text            text
        //...
        public Localization(string localizationText, string defaultLanguageCode)
        {
            ParseCsv(localizationText, out m_localizedText, out string[] supportedLanguages);
            SupportedLanguageCodes = supportedLanguages;

            bool foundDefaultLanguage = Array.Exists(SupportedLanguageCodes, element => element == defaultLanguageCode.ToLower());
            if (foundDefaultLanguage)
            {
                CurrentLanguageCode = defaultLanguageCode;
            }
            else
            {
                throw new ArgumentException(defaultLanguageCode + " language is not found");
            }
        }

        public string[] SupportedLanguageCodes { get; private set; }

        public string CurrentLanguageCode 
        { 
            get
            {
                return m_currentLanguageCode;
            } 
            
            set
            {
                string valueLowerCase = value.ToLower();
                bool isSupported = IsLanguageCodeSupported(valueLowerCase);
                if(isSupported)
                {
                    m_currentLanguageCode = valueLowerCase;
                    m_currentLanguageCodeHash = m_currentLanguageCode.GetHashCode();
                }
                else 
                {
                    throw new ArgumentException(value + " is not supported.");
                }
            }
        }

        public bool IsLanguageCodeSupported(string languageCode)
        {
            bool isFound = Array.IndexOf(SupportedLanguageCodes, languageCode.ToLower()) >= 0;
            return isFound;
        }

        public string GetText(string textId, params object[] formatParameters)
        {
            if (textId is null)
            {
                throw new ArgumentNullException(nameof(textId));
            }
            if (string.Empty == textId)
            {
                throw new ArgumentException(nameof(textId) + " is empty");
            }

            int textIdHash = textId.GetHashCode();
            int hashKey = Hash(textIdHash: textIdHash, languageCodeHash: m_currentLanguageCodeHash);
            bool found = m_localizedText.TryGetValue(hashKey, out string unformattedText);
            if (!found)
            {
                throw new ArgumentException(textId + " is not found");
            }
            return string.Format(unformattedText, formatParameters);
        }

        private static void ParseCsv(string localizationText, out Dictionary<int, string> localization, out string[] languageCodes)
        {
            if (localizationText is null)
            {
                throw new ArgumentNullException(nameof(localizationText));
            }
            if (localizationText == string.Empty)
            {
                throw new ArgumentException(nameof(localizationText) + " is empty");
            }

            string[] lines = localizationText.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            string[] elements = lines[0].Split('\t');
            int expectedColumns = elements.Length;
            int startingNonLanguageColumns = 2; //Columns are id, comment, language1, language2, etc...
            int languageCount = expectedColumns - startingNonLanguageColumns; 
            if (languageCount < 1)
            {
                throw new ArgumentException("No lanuages found. Needs at least 3 columns");
            }
            languageCodes = new string[languageCount];
            for (int i = 0; i < languageCount; ++i)
            {
                languageCodes[i] = elements[i + startingNonLanguageColumns].ToLower();
            }

            List<string[]> translations = new List<string[]>();
            List<string> textIdentifiers = new List<string>();
            for (int i = 1; i < lines.Length; ++i)
            {
                elements = lines[i].Split('\t');
                if(expectedColumns != elements.Length)
                {
                    throw new IOException("Each row must have the same number of columns: " + expectedColumns);
                }
                string[] lineTranslations = new string[languageCount];
                for(int j = 0; j < languageCount; ++j)
                {
                    lineTranslations[j] = elements[j + startingNonLanguageColumns];
                }
                translations.Add(lineTranslations);

                textIdentifiers.Add(elements[0].ToLower());
            }

            //Convert into the expected format
            localization = new Dictionary<int, string>();
            for(int languageIndex = 0; languageIndex < languageCodes.Length; ++languageIndex)
            {
                int languageCodeHash = languageCodes[languageIndex].GetHashCode();
                for (int textIndex = 0; textIndex < textIdentifiers.Count; ++textIndex)
                {
                    int textIdHash = textIdentifiers[textIndex].GetHashCode();
                    int hashKey = Hash(languageCodeHash: languageCodeHash, textIdHash: textIdHash);
                    bool hashCollision = !localization.TryAdd(hashKey, translations[textIndex][languageIndex]);
                    if(hashCollision)
                    {
                        throw new ArgumentException("Localization hash collision. Text not localized. A better hashing algorithm is needed.");
                    }
                }
            }
        }

        private static int Hash(int languageCodeHash, int textIdHash)
        {
            //The original languageCode and textId should never be mistaken for each other,
            //so having a symmetric hashing function should be fine.
            //This assumption will be verified at runtime, of course.
            return languageCodeHash ^ textIdHash;
        }

        private string m_currentLanguageCode;
        private int m_currentLanguageCodeHash;

        //The key is a combined hash of textId and languageCode
        private readonly Dictionary<int, string> m_localizedText = new Dictionary<int, string>();
    }
}
