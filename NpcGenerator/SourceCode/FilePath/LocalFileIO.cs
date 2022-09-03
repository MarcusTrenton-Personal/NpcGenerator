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

using CoupledServices;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace NpcGenerator
{
    public class LocalFileIO : ILocalFileIO
    {
        public LocalFileIO(IFilePathProvider filePathProvider)
        {
            m_filePathProvider = filePathProvider;
        }

        //Cache a copy of a configuration file so it can already be open with a read/write lock at the same time 
        //it is read by this program.
        public string CacheFile(string originalPath)
        {
            string fileName = Path.GetFileName(originalPath);
            string cachePath = Path.Combine(m_filePathProvider.AppDataFolderPath, CacheFolder, fileName);

            string directory = Path.GetDirectoryName(cachePath);
            Directory.CreateDirectory(directory);

            File.Copy(originalPath, cachePath, overwrite: true);
            return cachePath;
        }

        public bool SaveToPickedFile(IReadOnlyList<FileContentProvider> contentProviders, out string pickedFileExtension)
        {
            pickedFileExtension = "";

            string filterString = "";
            for (int i = 0; i < contentProviders.Count; i++)
            {
                filterString += string.Format("{0} file (*.{0})|*.{0}", contentProviders[i].FileExtensionWithoutDot);
                if (i+1 < contentProviders.Count)
                {
                    filterString += "|";
                }
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                
                Filter = filterString,
                RestoreDirectory = true
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    int index = saveFileDialog.FilterIndex - 1; //1-based index for some reason.
                    pickedFileExtension = contentProviders[index].FileExtensionWithoutDot;
                    string content = contentProviders[index].GetContent();
                    using Stream stream = saveFileDialog.OpenFile();
                    byte[] info = new UTF8Encoding(true).GetBytes(content);
                    stream.Write(info, 0, info.Length);
                    stream.Close();

                    return true;
                }
                catch (IOException exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }

            return false;
        }

        private const string CacheFolder = "Cache";

        private readonly IFilePathProvider m_filePathProvider;
    }
}
