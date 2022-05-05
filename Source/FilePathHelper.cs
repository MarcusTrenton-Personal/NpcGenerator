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

        private const string ConfigurationCacheFolder = "Cache";
        private const string SettingsFile = "Settings.json";

        public static string SettingsFilePath()
        {
            string commonAppData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            return Path.Combine(commonAppData, AppDataFolder, SettingsFile);
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
    }
}
