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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NpcGenerator;
using System.Data;
using System.IO;

namespace Tests
{
    [TestClass]
    public class NpcGeneratorModelTests : FileCreatingTests
    {
        [TestInitialize]
        public void Initialize()
        {
            m_userSettings = new StubUserSettings();
            m_npcGeneratorModel = new NpcGeneratorModel(
                m_userSettings, new StubMessager(), new StubLocalFileIo(), new MockCsvConfigurationParser());
        }

        [TestMethod]
        public void ConfigurationPathReflectsUserSettings()
        {
            const string FILE_PATH1 = "...";
            const string FILE_PATH2 = "FakeFile.csv";

            m_userSettings.ConfigurationPath = FILE_PATH1;
            Assert.AreEqual(FILE_PATH1, m_npcGeneratorModel.ConfigurationPath, 
                "Configuration path is not the one in UserSettings");

            m_userSettings.ConfigurationPath = FILE_PATH2;
            Assert.AreEqual(FILE_PATH2, m_npcGeneratorModel.ConfigurationPath, 
                "Configuration path is not the one in UserSettings");
        }

        [TestMethod]
        public void NpcQuantityReflectsUserSettings()
        {
            const int QUANTITY1 = 1;
            const int QUANTITY2 = 5;

            m_userSettings.NpcQuantity = QUANTITY1;
            Assert.AreEqual(QUANTITY1, m_npcGeneratorModel.NpcQuantity, "NpcQuantity is not the one in UserSettings");

            m_npcGeneratorModel.NpcQuantity = QUANTITY2;
            Assert.AreEqual(QUANTITY2, m_userSettings.NpcQuantity, "NpcQuantity is not the one in UserSettings");
            Assert.AreEqual(QUANTITY2, m_npcGeneratorModel.NpcQuantity, "NpcQuantity is not the one in UserSettings");
        }

        [TestMethod]
        public void InvalidNpcQuantityBlocksNpcGeneration()
        {
            m_npcGeneratorModel.NpcQuantity = 0;
            bool canGenerateNpcs = m_npcGeneratorModel.GenerateNpcs.CanExecute(null);
            Assert.IsFalse(canGenerateNpcs, "Not blocking the generation of 0 npcs");
        }

        [TestMethod]
        public void InvalidConfigurationPathBlocksNpcGeneration()
        {
            m_userSettings.ConfigurationPath = "Bad file";
            bool canGenerateNpcs = m_npcGeneratorModel.GenerateNpcs.CanExecute(null);
            Assert.IsFalse(canGenerateNpcs, "Not blocking the generation of npcs with invalid ConfigurationPath");
        }

        [TestMethod]
        public void GenerateNpcs()
        {
            string path = Path.Combine(TestDirectory, "colour.csv");
            string text = "Colour,Weight\n" +
                "Green,1\n" +
                "Red,1";
            File.WriteAllText(path, text);
            m_userSettings.ConfigurationPath = path;

            const int QUANTITY = 5;
            m_npcGeneratorModel.NpcQuantity = 5;

            m_npcGeneratorModel.GenerateNpcs.Execute(null);
            DataTable result = m_npcGeneratorModel.ResultNpcs;

            Assert.AreEqual(QUANTITY, result.Rows.Count, "Generate the wrong number of npcs");
            bool canSave = m_npcGeneratorModel.SaveNpcs.CanExecute(null);
            Assert.IsTrue(canSave, "Have npcs but cannot save");
        }

        [TestMethod]
        public void CannotSaveWithoutNpcs()
        {
            bool canSave = m_npcGeneratorModel.SaveNpcs.CanExecute(null);
            Assert.IsFalse(canSave, "Can save even though there are no npcs");
        }

        private NpcGeneratorModel m_npcGeneratorModel;
        private StubUserSettings m_userSettings;
    }
}
