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
using System;
using System.IO;

namespace Tests
{
    [TestClass]
    public class UserSettingsTests : FileCreatingTests
    {
        const string FILE_EXTENSION = ".json";

        [TestMethod]
        public void ReadSettings()
        {
            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + FILE_EXTENSION);

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
            File.WriteAllText(path, json);

            //Read the settings.
            UserSettings readSettings = UserSettings.Load(path);

            Assert.AreEqual(configurationPath, readSettings.ConfigurationPath, "Read incorrect ConfigurationPath");
            Assert.AreEqual(npcQuantity, readSettings.NpcQuantity, "Read incorrect NpcQuantity");
            Assert.AreEqual(languageCode, readSettings.LanguageCode, "Read incorrect languageCode");

            File.Delete(path);
        }

        [TestMethod]
        public void MalformedJson()
        {
            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + FILE_EXTENSION);

            string json = "{";
            File.WriteAllText(path, json);

            UserSettings settings = UserSettings.Load(path);

            Assert.IsNotNull(settings, "Failed to generate default UserSettings");

            File.Delete(path);
        }

        [TestMethod]
        public void EmptyJson()
        {
            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + FILE_EXTENSION);

            string json = "{}";
            File.WriteAllText(path, json);

            UserSettings settings = UserSettings.Load(path);

            Assert.IsNotNull(settings, "Failed to generate default UserSettings");

            File.Delete(path);
        }

        [TestMethod]
        public void FileDoeNotExist()
        {
            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + FILE_EXTENSION);

            UserSettings settings = UserSettings.Load(path);

            Assert.IsNotNull(settings, "Failed to generate default UserSettings");
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void PathIsNull()
        {
            UserSettings settings = UserSettings.Load(null);

            Assert.IsNotNull(settings, "Failed to generate default UserSettings");
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void PathIsEmpty()
        {
            UserSettings settings = UserSettings.Load(string.Empty);

            Assert.IsNotNull(settings, "Failed to generate default UserSettings");
        }

        [TestMethod]
        public void WrongNpcQuantityJson()
        {
            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + FILE_EXTENSION);

            string json = $@"{{
                {{
                  'ConfigurationPath': 'C:\\ProgramData\\NpcGenerator\\NotARealFile.json',
                  'NpcQuantity': -1,
                  'AnalyticsConsent': true,
                  'LanguageCode': 'en-ca'
                }}
            }}";
            File.WriteAllText(path, json);

            UserSettings settings = UserSettings.Load(path);

            Assert.IsNotNull(settings, "Failed to generate default UserSettings");

            File.Delete(path);
        }

        [TestMethod]
        public void MissingNpcQuantityJson()
        {
            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + FILE_EXTENSION);

            string json = $@"{{
                {{
                  'ConfigurationPath': 'C:\\ProgramData\\NpcGenerator\\NotARealFile.json',
                  'AnalyticsConsent': true,
                  'LanguageCode': 'en-ca'
                }}
            }}";
            File.WriteAllText(path, json);

            UserSettings settings = UserSettings.Load(path);

            Assert.IsNotNull(settings, "Failed to generate default UserSettings");

            File.Delete(path);
        }

        [TestMethod]
        public void MalformedConfigurationPath()
        {
            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + FILE_EXTENSION);

            string json = $@"{{
                {{
                  'ConfigurationPath': false,
                  'NpcQuantity': 1,
                  'AnalyticsConsent': true,
                  'LanguageCode': 'en-ca'
                }}
            }}";
            File.WriteAllText(path, json);

            UserSettings settings = UserSettings.Load(path);

            Assert.IsNotNull(settings, "Failed to generate default UserSettings");

            File.Delete(path);
        }

        [TestMethod]
        public void MissingConfigurationPath()
        {
            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + FILE_EXTENSION);

            string json = $@"{{
                {{
                  'NpcQuantity': 1,
                  'AnalyticsConsent': true,
                  'LanguageCode': 'en-ca'
                }}
            }}";
            File.WriteAllText(path, json);

            UserSettings settings = UserSettings.Load(path);

            Assert.IsNotNull(settings, "Failed to generate default UserSettings");

            File.Delete(path);
        }

        [TestMethod]
        public void WrongAnayticsConsent()
        {
            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + FILE_EXTENSION);

            string json = $@"{{
                {{
                  'ConfigurationPath': 'C:\\ProgramData\\NpcGenerator\\NotARealFile.json',
                  'NpcQuantity': 1,
                  'AnalyticsConsent': 1,
                  'LanguageCode': 'en-ca'
                }}
            }}";
            File.WriteAllText(path, json);

            UserSettings settings = UserSettings.Load(path);

            Assert.IsNotNull(settings, "Failed to generate default UserSettings");

            File.Delete(path);
        }

        [TestMethod]
        public void MissingAnalyticsConsent()
        {
            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + FILE_EXTENSION);

            string json = $@"{{
                {{
                  'ConfigurationPath': 'C:\\ProgramData\\NpcGenerator\\NotARealFile.json',
                  'NpcQuantity': 1,
                  'LanguageCode': 'en-ca'
                }}
            }}";
            File.WriteAllText(path, json);

            UserSettings settings = UserSettings.Load(path);

            Assert.IsNotNull(settings, "Failed to generate default UserSettings");

            File.Delete(path);
        }

        [TestMethod]
        public void MalformedLanguageCode()
        {
            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + FILE_EXTENSION);

            string json = $@"{{
                {{
                  'ConfigurationPath': 'C:\\ProgramData\\NpcGenerator\\NotARealFile.json',
                  'NpcQuantity': 1,
                  'AnalyticsConsent': true,
                  'LanguageCode': 1.5
                }}
            }}";
            File.WriteAllText(path, json);

            UserSettings settings = UserSettings.Load(path);

            Assert.IsNotNull(settings, "Failed to generate default UserSettings");

            File.Delete(path);
        }

        [TestMethod]
        public void MissingLanguageCode()
        {
            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + FILE_EXTENSION);

            string json = $@"{{
                {{
                  'ConfigurationPath': 'C:\\ProgramData\\NpcGenerator\\NotARealFile.json',
                  'NpcQuantity': 1,
                  'AnalyticsConsent': true,
                }}
            }}";
            File.WriteAllText(path, json);

            UserSettings settings = UserSettings.Load(path);

            Assert.IsNotNull(settings, "Failed to generate default UserSettings");

            File.Delete(path);
        }
    }
}
