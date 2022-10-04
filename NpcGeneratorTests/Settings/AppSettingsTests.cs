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
using System.Text;

namespace Tests
{
    [TestClass]
    public class AppSettingsTests
    {
        [TestMethod]
        public void ReadSettings()
        {
            const string FILE_NAME = "TestSettings.json";
            const int ENCRYPTION_KEY = 1;
            const string ADDITIONAL_ID_DEV = "Add Dev";
            const string ADDITIONAL_ID_PROD = "Add Prod";
            const string MEASUREMENT_ID_DEV = "Measure Dev";
            const string MEASUREMENT_ID_PROD = "Measure Prod";
            const string HIDDEN_LANGUAGE_CODE = "Atlantean";
            const string HOME_WEBSITE = "fake.com";
            const string DONATION_WEBSITE = "mock.com";
            const string SUPPORT_EMAIL = "fake@test.com";

            //Create the original AppSettings
            GoogleAnalyticsSettings googleAnalytics = new GoogleAnalyticsSettings
            {
                AdditionalIdDev = ADDITIONAL_ID_DEV,
                AdditionalIdProd = ADDITIONAL_ID_PROD,
                MeasurementIdDev = MEASUREMENT_ID_DEV,
                MeasurementIdProd = MEASUREMENT_ID_PROD
            };

            AppSettings original = new AppSettings
            {
                EncryptionKey = ENCRYPTION_KEY,
                GoogleAnalytics = googleAnalytics,
                HiddenLanguageCodes = Array.AsReadOnly(new string[] { HIDDEN_LANGUAGE_CODE }),
                HomeWebsite = HOME_WEBSITE,
                DonationWebsite = DONATION_WEBSITE,
                SupportEmail = SUPPORT_EMAIL
            };

            string json = JsonConvert.SerializeObject(original, Formatting.Indented);
            using (FileStream fs = File.Create(FILE_NAME))
            {
                byte[] info = new UTF8Encoding(true).GetBytes(json);
                fs.Write(info, 0, info.Length);
            }

            //Read the settings.
            AppSettings readSettings = AppSettings.Load(FILE_NAME);

            Assert.AreEqual(ENCRYPTION_KEY, readSettings.EncryptionKey, "Read incorrect EncryptionKey");
            Assert.AreEqual(1, readSettings.HiddenLanguageCodes.Count, "Read incorrect number of HiddenLanguageCodes");
            Assert.AreEqual(HIDDEN_LANGUAGE_CODE, readSettings.HiddenLanguageCodes[0], "Read incorrect HiddenLanguageCode");
            Assert.AreEqual(HOME_WEBSITE, readSettings.HomeWebsite, "Read incorrect HomeWebsite");
            Assert.AreEqual(DONATION_WEBSITE, readSettings.DonationWebsite, "Read incorrect DonationWebsite");
            Assert.AreEqual(SUPPORT_EMAIL, readSettings.SupportEmail, "Read incorrect SupportEmail");
            Assert.AreEqual(ADDITIONAL_ID_DEV, readSettings.GoogleAnalytics.AdditionalIdDev, "Read incorrect AdditionalIdDev");
            Assert.AreEqual(ADDITIONAL_ID_PROD, readSettings.GoogleAnalytics.AdditionalIdProd, "Read incorrect AdditionalIdProd");
            Assert.AreEqual(MEASUREMENT_ID_DEV, readSettings.GoogleAnalytics.MeasurementIdDev, "Read incorrect MeasurementIdDev");
            Assert.AreEqual(MEASUREMENT_ID_PROD, readSettings.GoogleAnalytics.MeasurementIdProd, "Read incorrect MeasurementIdProd");
        }
    }
}
