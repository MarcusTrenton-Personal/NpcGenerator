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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NpcGenerator;
using System;
using System.Collections.Generic;
using System.IO;

namespace Tests
{
    [TestClass]
    public class FileIOTests
    {
        [TestInitialize]
        public void CreateMockFileProvider()
        {
            StubFilePathProvider mockFilePathProvider = new StubFilePathProvider
            {
                AppDataFolderPath = "App"
            };
            m_fileIO = new LocalFileIO(mockFilePathProvider);
        }

        [TestMethod]
        public void CacheFileIsCopy()
        {
            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string originalFile = method + ".txt";
            const string ORIGINAL_CONTENT = "Test test test";

            using (StreamWriter writer = File.CreateText(originalFile))
            {
                writer.Write(ORIGINAL_CONTENT);
                writer.Close();
            }

            string cacheFile = m_fileIO.CacheFile(originalFile);
            Assert.IsTrue(File.Exists(cacheFile), "Cache file doesn't exist: " + cacheFile);

            string cacheText = File.ReadAllText(cacheFile, Constants.TEXT_ENCODING);
            Assert.AreEqual(ORIGINAL_CONTENT, cacheText, "Cache file text is different than original");

            File.Delete(cacheFile);
            File.Delete(originalFile);
        }

        [TestMethod]
        public void CacheFileAllowsReadOfOriginalFile()
        {
            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string originalFile = method + ".txt";
            const string ORIGINAL_CONTENT = "Test test test";

            //Create original file with a read/write lock.
            using (StreamWriter writer = File.CreateText(originalFile))
            {
                writer.Write(ORIGINAL_CONTENT);

                //While writing, cache the original file.
                string cacheFile = m_fileIO.CacheFile(originalFile);
                Assert.IsTrue(File.Exists(cacheFile), "Cache file doesn't exist: " + cacheFile);

                File.Delete(cacheFile);

                writer.Close();
            }

            File.Delete(originalFile);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void ConstructWithNull()
        {
            new LocalFileIO(null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void CacheWithNullPath()
        {
            m_fileIO.CacheFile(null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void CacheWithEmptyPath()
        {
            m_fileIO.CacheFile(String.Empty);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void CacheWithInvalidPath()
        {
            m_fileIO.CacheFile("not a path");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void SaveToPickedFileWithNullCollection()
        {
            m_fileIO.SaveToPickedFile(null, out _);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void SaveToPickedFileWithNullGetContent()
        {
            FileContentProvider contentProvider = new FileContentProvider
            {
                FileExtensionWithoutDot = "abc",
                GetContent = null
            };
            m_fileIO.SaveToPickedFile(new List<FileContentProvider>() { contentProvider }, out _);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void SaveToPickedFileWithInvalidFileExtension()
        {
            static string MockGetContent()
            {
                return "Mock";
            }
            FileContentProvider contentProvider = new FileContentProvider
            {
                FileExtensionWithoutDot = "sfds.abc",
                GetContent = MockGetContent
            };
            m_fileIO.SaveToPickedFile(new List<FileContentProvider>() { contentProvider }, out _);
        }

        private LocalFileIO m_fileIO;
    }
}
