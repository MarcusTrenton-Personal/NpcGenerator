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
    public static class FilePathHelper
    {
        public const string AppDataFolder = "NpcGenerator";
        public const string AppSettingsFolder = "Settings";

        private const string ConfigurationCacheFolder = "Cache";
        private const string SettingsFile = "Settings.json";
        private const string AppSettingsFile = "AppSettings.json";
        private const string LicenseFile = "GNU License.rtf";

        public static string SettingsFilePath()
        {
            string commonAppData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            return Path.Combine(commonAppData, AppDataFolder, SettingsFile);
        }

        public static string AppSettingsFilePath()
        {
            return Path.Combine(AppSettingsFolder, AppSettingsFile);
        }

        //Cache a copy of a configuration file so it can already be open with a read/write lock at the same time 
        //it is read by this program.
        public static string CacheConfigurationFile(string originalPath)
        {
            string fileName = Path.GetFileName(originalPath);
            string commonAppData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string cachePath = Path.Combine(commonAppData, AppDataFolder, ConfigurationCacheFolder, fileName);

            string directory = Path.GetDirectoryName(cachePath);
            Directory.CreateDirectory(directory);

            File.Copy(originalPath, cachePath, overwrite: true);
            return cachePath;
        }

        public static void SaveToPickedFile(string content, string fileType)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = fileType + " files (*." + fileType + ")|*." + fileType;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == true)
            {
                try
                {
                    using (Stream stream = saveFileDialog1.OpenFile())
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes(content);
                        stream.Write(info, 0, info.Length);
                        stream.Close();
                    }
                }
                catch (IOException exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
        }

        public static string LicensePath()
        {
            return LicenseFile;
        }
    }
}
