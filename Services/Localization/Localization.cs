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

using System;
using System.Collections.Generic;
using System.IO;

namespace Services
{
    public class Localization : ILocalization
    {
        public Localization(string filePath, string defaultLanguageCode)
        {
            ParseCsv(filePath, out m_localizedText, out string[] supportedLanguages);
            SupportedLanguageCodes = supportedLanguages;

            bool foundDefaultLanguage = Array.Exists<string>(SupportedLanguageCodes, element => element == defaultLanguageCode.ToLower());
            if (foundDefaultLanguage)
            {
                CurrentLanguageCode = defaultLanguageCode;
            }
            else
            {
                throw new ArgumentException(defaultLanguageCode + " language is not found in " + filePath);
            }
        }

        public string[] SupportedLanguageCodes { get; private set; }

        public string CurrentLanguageCode { get; set; }

        public string GetText(string textId, params object[] formatParameters)
        {
            string unformattedText = m_localizedText[new LocalizationKey(textId: textId, languageCode: CurrentLanguageCode)];
            return String.Format(unformattedText, formatParameters);
        }

        private static void ParseCsv(string filePath, out Dictionary<LocalizationKey, string> localization, out string[] languageCodes)
        {
            IEnumerable<string> lines = File.ReadAllLines(filePath);
            IEnumerator<string> enumerator = lines.GetEnumerator();
            bool hasTitleRow = enumerator.MoveNext();
            if (!hasTitleRow)
            {
                throw new IOException("The file is empty: " + filePath);
            }

            string[] elements = enumerator.Current.Split('\t');
            int expectedColumns = elements.Length;
            int startingNonLanguageColumns = 2; //Columns are id, comment, language1, language2, etc...
            int languageCount = expectedColumns - startingNonLanguageColumns; 
            if (languageCount < 1)
            {
                throw new IOException("No lanuages found. Needs at least 3 columns: " + filePath);
            }
            languageCodes = new string[languageCount];
            for (int i = 0; i < languageCount; ++i)
            {
                languageCodes[i] = elements[i + startingNonLanguageColumns].ToLower();
            }

            List<string[]> translations = new List<string[]>();
            List<string> textIdentifiers = new List<string>();
            while (enumerator.MoveNext())
            {
                elements = enumerator.Current.Split('\t');
                if(expectedColumns != elements.Length)
                {
                    throw new IOException("Each row must have the same number of columns: " + expectedColumns);
                }
                string[] lineTranslations = new string[languageCount];
                for(int i = 0; i < languageCount; ++i)
                {
                    lineTranslations[i] = elements[i + startingNonLanguageColumns];
                }
                translations.Add(lineTranslations);

                textIdentifiers.Add(elements[0].ToLower());
            }

            //Convert into the expected format
            localization = new Dictionary<LocalizationKey, string>();
            for(int languageIndex = 0; languageIndex < languageCodes.Length; ++languageIndex)
            {
                for(int textIndex = 0; textIndex < textIdentifiers.Count; ++textIndex)
                {
                    string textId = textIdentifiers[textIndex];
                    localization[new LocalizationKey(textId: textId, languageCode: languageCodes[languageIndex])] = translations[textIndex][languageIndex];
                }
            }
        }

        private struct LocalizationKey : IEquatable<LocalizationKey>
        {
            public LocalizationKey(string textId, string languageCode)
            {
                TextId = textId;
                LanguageCode = languageCode;
            }

            public override bool Equals(object other)
            {
                return other is LocalizationKey otherKey && Equals(otherKey);
            }

            public bool Equals(LocalizationKey other)
            {
                // Optimization for a common success case.
                if (Object.ReferenceEquals(this, other))
                {
                    return true;
                }

                // If run-time types are not exactly the same, return false.
                if (this.GetType() != other.GetType())
                {
                    return false;
                }

                // Return true if the fields match.
                // Note that the base class is not invoked because it is
                // System.Object, which defines Equals as reference equality.
                return (TextId == other.TextId) && (LanguageCode == other.LanguageCode);
            }

            public override int GetHashCode() => (TextId, LanguageCode).GetHashCode();

            public static bool operator ==(LocalizationKey lhs, LocalizationKey rhs)
            {
                // Equals handles case of null on right side.
                return lhs.Equals(rhs);
            }

            public static bool operator !=(LocalizationKey lhs, LocalizationKey rhs) => !(lhs == rhs);

            public string TextId { get; private set; }
            public string LanguageCode { get; private set; }
        }

        private readonly Dictionary<LocalizationKey, string> m_localizedText = new Dictionary<LocalizationKey, string>();
    }
}
