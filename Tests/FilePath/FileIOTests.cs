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

using CoupledServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NpcGenerator;
using System.IO;

namespace Tests
{
    [TestClass]
    public class FileIOTests
    {
        private class MockFilePathProvider : IFilePathProvider
        {
            public string AppDataFolderPath { get; } = "App";
            public string LicensePath { get; } = "License.txt";
            public string UserSettingsFilePath { get; } = "UserSettings.txt";
            public string AppSettingsFilePath { get; } = "AppSettings.txt";
            public string TrackingProfileFilePath { get; } = "Tracking.txt";
        }

        [TestInitialize]
        public void CreateMockFileProvider()
        {
            MockFilePathProvider mockFilePathProvider = new MockFilePathProvider();
            m_fileIO = new LocalFileIO(mockFilePathProvider);
        }

        [TestMethod]
        public void CacheFileIsCopy()
        {
            const string originalFile = "Test.txt";
            const string originalContent = "Test test test";

            using (StreamWriter writer = File.CreateText(originalFile))
            {
                writer.Write(originalContent);
                writer.Close();
            }

            string cacheFile = m_fileIO.CacheFile(originalFile);
            Assert.IsTrue(File.Exists(cacheFile), "Cache file doesn't exist: " + cacheFile);

            string cacheText = File.ReadAllText(cacheFile);
            Assert.AreEqual(originalContent, cacheText, "Cache file text is different than original");

            File.Delete(cacheFile);
            File.Delete(originalFile);
        }

        [TestMethod]
        public void CacheFileAllowsReadOfOriginalFile()
        {
            const string originalFile = "Test.txt";
            const string originalContent = "Test test test";

            //Create original file with a read/write lock.
            using (StreamWriter writer = File.CreateText(originalFile))
            {
                writer.Write(originalContent);

                //While writing, cache the original file.
                string cacheFile = m_fileIO.CacheFile(originalFile);
                Assert.IsTrue(File.Exists(cacheFile), "Cache file doesn't exist: " + cacheFile);

                File.Delete(cacheFile);

                writer.Close();
            }

            File.Delete(originalFile);
        }

        private LocalFileIO m_fileIO;
    }
}
