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

using System;
using System.IO;

[assembly: CLSCompliant(true)]
namespace CoupledServices
{
    public class FilePathProvider : IFilePathProvider
    {
        public string AppDataFolderPath 
        { 
            get
            {
                string commonAppData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                return Path.Combine(commonAppData, m_appDataFolder);
            }
        }

        public string AppSettingsFilePath
        {
            get
            {
                return Path.Combine(m_appSettingsFolder, m_appSettingsFile);
            }
        }

        public string ConfigurationJsonSchema
        {
            get
            {
                return Path.Combine(m_appSettingsFolder, m_configurationSchemaFile);
            }
        }

        public string NpcExportJsonSchema
        {
            get
            {
                return Path.Combine(m_appSettingsFolder, m_npcExportFile);
            }
        }

        public string LicensePath { get; } = "GNU License.rtf";

        public string LocalizationPath
        {
            get
            {
                return Path.Combine(m_appSettingsFolder, m_localizationFile);
            }
        }

        public string PrivacyPolicyPath { get; } = "Privacy Policy.rtf";

        public string TrackingProfileFilePath
        {
            get
            {
                return Path.Combine(AppDataFolderPath, m_trackingProfileFile);
            }
        }

        public string UserSettingsFilePath 
        {
            get
            {
                return Path.Combine(AppDataFolderPath, m_userSettingsFile);
            }
        }

        private const string m_appDataFolder = "NpcGenerator";
        private const string m_appSettingsFile = "AppSettings.json";
        private const string m_appSettingsFolder = "Settings";
        private const string m_configurationSchemaFile = "ConfigurationSchema.json";
        private const string m_npcExportFile = "NpcGroupSchema.json";
        private const string m_localizationFile = "Localization.csv";
        private const string m_trackingProfileFile = "TrackingProfile.json";
        private const string m_userSettingsFile = "UserSettings.json";
    }
}
