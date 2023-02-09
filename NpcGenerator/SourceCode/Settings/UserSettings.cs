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

using Newtonsoft.Json;
using Services;
using System;
using System.IO;
using System.Text;

namespace NpcGenerator
{
    //User settings are everything that the user explicitly chooses that are persistent session-to-session.
    public class UserSettings : IUserSettings
    {
        public string ConfigurationPath 
        {
            get => m_configurationPath; 
            set
            {
                m_configurationPath = value;
                Save();
            }
        }

        public int NpcQuantity 
        { 
            get => m_npcQuantity; 
            set
            {
                m_npcQuantity = value;
                Save();
            } 
        }

        public bool AnalyticsConsent
        {
            get => m_analyticsConsent; 
            set
            {
                m_analyticsConsent = value;
                Save();
            }
        }

        public string LanguageCode 
        { 
            get => m_languageCode; 
            set
            {
                m_languageCode = value;
                Save();
            }
        }

        private void Save()
        {
            if (!string.IsNullOrEmpty(m_savePath)) //If not set yet, ignore Save call.
            {
                string directory = Path.GetDirectoryName(m_savePath);
                Directory.CreateDirectory(directory);

                string json = JsonConvert.SerializeObject(this, Formatting.Indented);
                using FileStream fs = File.Create(m_savePath);
                byte[] info = new UTF8Encoding(true).GetBytes(json);
                fs.Write(info, 0, info.Length);
            }
        }

        public static UserSettings Load(string path)
        {
            ParamUtil.VerifyHasContent(nameof(path), path);

            try
            {
                string text = File.ReadAllText(path, Constants.TEXT_ENCODING);
                UserSettings settings = JsonConvert.DeserializeObject<UserSettings>(text);
                settings.Validate();
                settings.m_savePath = path;
                return settings;
            }
            catch (IOException)
            {
                return Default(path);
            }
            catch (JsonSerializationException)
            {
                return Default(path);
            }
            catch (JsonReaderException)
            {
                return Default(path);
            }
        }

        private static UserSettings Default(string savePath)
        {
            UserSettings settings = new UserSettings();
            settings.ConfigurationPath = DEFAULT_CONFIGURATION_PATH;
            settings.NpcQuantity = DEFAULT_NPC_QUANTITY;
            settings.AnalyticsConsent = DEFAULT_ANALYTICS_CONSENT;
            settings.LanguageCode = DEFAULT_LANGUAGE_CODE;
            settings.m_savePath = savePath;
            return settings;
        }

        private void Validate()
        {
            ValidateConfigurationPath();
            ValidateNpcQuantity();
            ValidateAnalyticsConsent();
            ValidateLanguageCode();
        }

        private static void ValidateConfigurationPath()
        {
            //Invalid paths will be dealt with at the Model layer. 
            //Since paths can be invalidated by simple OS actions.
            //There is no reason an invalid path should cause all UserSettings to be deleted.
        }

        private void ValidateNpcQuantity()
        {
            if (NpcQuantity < 1)
            {
                NpcQuantity = DEFAULT_NPC_QUANTITY;
            }
        }

        private static void ValidateAnalyticsConsent()
        {
            //Both true and false are value
        }

        private void ValidateLanguageCode()
        {
            if (string.IsNullOrWhiteSpace(LanguageCode))
            {
                LanguageCode = DEFAULT_LANGUAGE_CODE;
            }
        }

        public const string DEFAULT_CONFIGURATION_PATH = "...";
        private string m_configurationPath = DEFAULT_CONFIGURATION_PATH;

        private const int DEFAULT_NPC_QUANTITY = 5;
        private int m_npcQuantity = DEFAULT_NPC_QUANTITY;

        private const bool DEFAULT_ANALYTICS_CONSENT = true;
        private bool m_analyticsConsent = DEFAULT_ANALYTICS_CONSENT;

        private const string DEFAULT_LANGUAGE_CODE = null;
        private string m_languageCode = DEFAULT_LANGUAGE_CODE;

        private string m_savePath = null;
    }

}
