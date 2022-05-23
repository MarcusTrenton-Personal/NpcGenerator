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

using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Windows;

namespace NpcGenerator
{
    public class FilePathProvider : IFilePathProvider
    {
        public string AppDataFolder { get; } = "NpcGenerator";
        public string AppSettingsFolder { get; } = "Settings";
        public string LicensePath { get; } = "GNU License.rtf";

        public string UserSettingsFilePath 
        {
            get
            {
                string commonAppData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                return Path.Combine(commonAppData, AppDataFolder, UserSettingsFile);
            }
        }

        public string AppSettingsFilePath
        {
            get
            {
                return Path.Combine(AppSettingsFolder, AppSettingsFile);
            }
        }

        public string TrackingProfileFilePath
        {
            get
            {
                string commonAppData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                return Path.Combine(commonAppData, AppDataFolder, TrackingProfileFile);
            }
        }

        private const string UserSettingsFile = "UserSettings.json";
        private const string AppSettingsFile = "AppSettings.json";
        private const string TrackingProfileFile = "TrackingProfile.json";
    }
}
