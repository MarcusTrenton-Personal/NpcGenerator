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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NpcGenerator;
using System.IO;
using System.Text;

namespace Tests
{
    [TestClass]
    public class UserSettingsTests
    {
        [TestMethod]
        public void ReadSettings()
        {
            const string fileName = "TestSettings.json";
            const string configurationPath = "Config.csv";
            const int npcQuantity = 1;
            const string languageCode = "Atlantean";

            //Create the original UserSettings
            UserSettings original = new UserSettings
            {
                ConfigurationPath = configurationPath,
                NpcQuantity = npcQuantity,
                LanguageCode = languageCode
            };

            string json = JsonConvert.SerializeObject(original, Formatting.Indented);
            using (FileStream fs = File.Create(fileName))
            {
                byte[] info = new UTF8Encoding(true).GetBytes(json);
                fs.Write(info, 0, info.Length);
            }

            //Read the settings.
            UserSettings readSettings = UserSettings.Load(fileName);

            Assert.AreEqual(configurationPath, readSettings.ConfigurationPath, "Read incorrect ConfigurationPath");
            Assert.AreEqual(npcQuantity, readSettings.NpcQuantity, "Read incorrect NpcQuantity");
            Assert.AreEqual(languageCode, readSettings.LanguageCode, "Read incorrect languageCode");
        }
    }
}
