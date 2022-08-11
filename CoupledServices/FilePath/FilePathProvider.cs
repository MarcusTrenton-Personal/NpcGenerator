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
                return Path.Combine(commonAppData, appDataFolder);
            }
        }

        public string AppSettingsFilePath
        {
            get
            {
                return Path.Combine(appSettingsFolder, appSettingsFile);
            }
        }

        public string ConfigurationJsonSchema
        {
            get
            {
                return Path.Combine(appSettingsFolder, configurationFile);
            }
        }

        public string LicensePath { get; } = "GNU License.rtf";

        public string LocalizationPath
        {
            get
            {
                return Path.Combine(appSettingsFolder, localizationFile);
            }
        }

        public string PrivacyPolicyPath { get; } = "Privacy Policy.rtf";

        public string TrackingProfileFilePath
        {
            get
            {
                return Path.Combine(AppDataFolderPath, trackingProfileFile);
            }
        }

        public string UserSettingsFilePath 
        {
            get
            {
                return Path.Combine(AppDataFolderPath, userSettingsFile);
            }
        }

        private const string appDataFolder = "NpcGenerator";
        private const string appSettingsFile = "AppSettings.json";
        private const string appSettingsFolder = "Settings";
        private const string configurationFile = "ConfigurationSchema.json";
        private const string localizationFile = "Localization.csv";
        private const string trackingProfileFile = "TrackingProfile.json";
        private const string userSettingsFile = "UserSettings.json";
    }
}
