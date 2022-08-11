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

using Newtonsoft.Json;
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

        public string SavePath { get; set; } = null;

        private void Save()
        {
            if (!string.IsNullOrEmpty(SavePath))
            {
                string directory = Path.GetDirectoryName(SavePath);
                Directory.CreateDirectory(directory);

                string json = JsonConvert.SerializeObject(this, Formatting.Indented);
                using FileStream fs = File.Create(SavePath);
                byte[] info = new UTF8Encoding(true).GetBytes(json);
                fs.Write(info, 0, info.Length);
            }
        }

        public static UserSettings Load(string path)
        {
            bool fileExists = File.Exists(path);
            if(fileExists)
            {
                string text = File.ReadAllText(path);
                UserSettings settings = JsonConvert.DeserializeObject<UserSettings>(text);
                return settings;
            }
            return null;
        }

        private string m_configurationPath = "...";
        private int m_npcQuantity = 1;
        private bool m_analyticsConsent = true;
        private string m_languageCode = null;
    }
}
