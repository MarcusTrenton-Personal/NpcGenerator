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
using Newtonsoft.Json;
using NpcGenerator;
using System;
using System.IO;
using System.Text;

namespace Tests
{
    [TestClass]
    public class AppSettingsTests
    {
        [TestMethod]
        public void ReadSettings()
        {
            const string fileName = "TestSettings.json";
            const int encryptionKey = 1;
            const string additionalIdDev = "Add Dev";
            const string additionalIdProd = "Add Prod";
            const string measurementIdDev = "Measure Dev";
            const string measurementIdProd = "Measure Prod";
            const string hiddenLanguageCode = "Atlantean";

            //Create the original AppSettings
            GoogleAnalyticsSettings googleAnalytics = new GoogleAnalyticsSettings
            {
                AdditionalIdDev = additionalIdDev,
                AdditionalIdProd = additionalIdProd,
                MeasurementIdDev = measurementIdDev,
                MeasurementIdProd = measurementIdProd
            };

            AppSettings original = new AppSettings
            {
                EncryptionKey = encryptionKey,
                GoogleAnalytics = googleAnalytics,
                HiddenLanguageCodes = Array.AsReadOnly(new string[] { hiddenLanguageCode })
            };

            string json = JsonConvert.SerializeObject(original, Formatting.Indented);
            using (FileStream fs = File.Create(fileName))
            {
                byte[] info = new UTF8Encoding(true).GetBytes(json);
                fs.Write(info, 0, info.Length);
            }

            //Read the settings.
            AppSettings readSettings = AppSettings.Load(fileName);

            Assert.AreEqual(encryptionKey, readSettings.EncryptionKey, "Read incorrect EncryptionKey");
            Assert.AreEqual(1, readSettings.HiddenLanguageCodes.Count, "Read incorrect number of HiddenLanguageCodes");
            Assert.AreEqual(hiddenLanguageCode, readSettings.HiddenLanguageCodes[0], "Read incorrect HiddenLanguageCode");
            Assert.AreEqual(additionalIdDev, readSettings.GoogleAnalytics.AdditionalIdDev, "Read incorrect AdditionalIdDev");
            Assert.AreEqual(additionalIdProd, readSettings.GoogleAnalytics.AdditionalIdProd, "Read incorrect AdditionalIdProd");
            Assert.AreEqual(measurementIdDev, readSettings.GoogleAnalytics.MeasurementIdDev, "Read incorrect MeasurementIdDev");
            Assert.AreEqual(measurementIdProd, readSettings.GoogleAnalytics.MeasurementIdProd, "Read incorrect MeasurementIdProd");
        }
    }
}
