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

namespace Tests
{
    [TestClass]
    public class AppSettingsTests
    {
        const int VALID_ENCRYPTION_KEY = 1;
        const string VALID_ADDITIONAL_ID_DEV = "Add Dev";
        const string VALID_ADDITIONAL_ID_PROD = "Add Prod";
        const string VALID_MEASUREMENT_ID_DEV = "Measure Dev";
        const string VALID_MEASUREMENT_ID_PROD = "Measure Prod";
        const string VALID_DEFAULT_LANGUAGE_CODE = "Martian";
        const string VALID_HIDDEN_LANGUAGE_CODE = "Atlantean";
        const string VALID_HOME_WEBSITE = "http://www.fake.com";
        const string VALID_DONATION_WEBSITE = "https://www.mock.com";
        const string VALID_SUPPORT_EMAIL = "fake@test.com";

        private GoogleAnalyticsSettings ValidGoogleAnalyticsSettings()
        {
            GoogleAnalyticsSettings googleAnalytics = new GoogleAnalyticsSettings
            {
                AdditionalIdDev = VALID_ADDITIONAL_ID_DEV,
                AdditionalIdProd = VALID_ADDITIONAL_ID_PROD,
                MeasurementIdDev = VALID_MEASUREMENT_ID_DEV,
                MeasurementIdProd = VALID_MEASUREMENT_ID_PROD
            };

            return googleAnalytics;
        }
        
        private AppSettings ValidAppSettings()
        {
            GoogleAnalyticsSettings googleAnalytics = ValidGoogleAnalyticsSettings();

            AppSettings original = new AppSettings
            {
                EncryptionKey = VALID_ENCRYPTION_KEY,
                GoogleAnalytics = googleAnalytics,
                DefaultLanguageCode = VALID_DEFAULT_LANGUAGE_CODE,
                HiddenLanguageCodes = Array.AsReadOnly(new string[] { VALID_HIDDEN_LANGUAGE_CODE }),
                HomeWebsite = VALID_HOME_WEBSITE,
                DonationWebsite = VALID_DONATION_WEBSITE,
                SupportEmail = VALID_SUPPORT_EMAIL
            };

            return original;
        }

        private AppSettings AppSettingsIncluding(GoogleAnalyticsSettings googleAnalytics)
        {
            AppSettings original = new AppSettings
            {
                EncryptionKey = VALID_ENCRYPTION_KEY,
                GoogleAnalytics = googleAnalytics,
                DefaultLanguageCode = VALID_DEFAULT_LANGUAGE_CODE,
                HiddenLanguageCodes = Array.AsReadOnly(new string[] { VALID_HIDDEN_LANGUAGE_CODE }),
                HomeWebsite = VALID_HOME_WEBSITE,
                DonationWebsite = VALID_DONATION_WEBSITE,
                SupportEmail = VALID_SUPPORT_EMAIL
            };

            return original;
        }

        [TestMethod]
        public void ReadCorrectSettings()
        {
            AppSettings original = ValidAppSettings();

            string json = JsonConvert.SerializeObject(original, Formatting.Indented);

            AppSettings readSettings = AppSettings.Create(json);

            Assert.AreEqual(VALID_ENCRYPTION_KEY, readSettings.EncryptionKey, "Read incorrect EncryptionKey");
            Assert.AreEqual(VALID_DEFAULT_LANGUAGE_CODE, readSettings.DefaultLanguageCode, "Read incorrect DefaultLanguageCode");
            Assert.AreEqual(1, readSettings.HiddenLanguageCodes.Count, "Read incorrect number of HiddenLanguageCodes");
            Assert.AreEqual(VALID_HIDDEN_LANGUAGE_CODE, readSettings.HiddenLanguageCodes[0], "Read incorrect HiddenLanguageCode");
            Assert.AreEqual(VALID_HOME_WEBSITE, readSettings.HomeWebsite, "Read incorrect HomeWebsite");
            Assert.AreEqual(VALID_DONATION_WEBSITE, readSettings.DonationWebsite, "Read incorrect DonationWebsite");
            Assert.AreEqual(VALID_SUPPORT_EMAIL, readSettings.SupportEmail, "Read incorrect SupportEmail");
            Assert.AreEqual(VALID_ADDITIONAL_ID_DEV, readSettings.GoogleAnalytics.AdditionalIdDev, "Read incorrect AdditionalIdDev");
            Assert.AreEqual(VALID_ADDITIONAL_ID_PROD, readSettings.GoogleAnalytics.AdditionalIdProd, "Read incorrect AdditionalIdProd");
            Assert.AreEqual(VALID_MEASUREMENT_ID_DEV, readSettings.GoogleAnalytics.MeasurementIdDev, "Read incorrect MeasurementIdDev");
            Assert.AreEqual(VALID_MEASUREMENT_ID_PROD, readSettings.GoogleAnalytics.MeasurementIdProd, "Read incorrect MeasurementIdProd");
        }

        [DataTestMethod, ExpectedException(typeof(NullOrEmptyDefaultLanguageCodeException))]
        [DataRow(null, DisplayName = "Null")]
        [DataRow("", DisplayName = "Empty")]
        public void IncorrectDefaultLanguageCode(string defaultLanguageCode)
        {
            AppSettings original = ValidAppSettings();
            original.DefaultLanguageCode = defaultLanguageCode;

            string json = JsonConvert.SerializeObject(original, Formatting.Indented);

            AppSettings.Create(json);
        }

        [DataTestMethod, ExpectedException(typeof(NullOrEmptyHiddenLanguageCodeException))]
        [DataRow(null, DisplayName = "Null")]
        [DataRow("", DisplayName = "Empty")]
        public void IncorrectEntryHiddenLanguageCode(string hiddenLanguage)
        {
            AppSettings original = ValidAppSettings();
            original.HiddenLanguageCodes = Array.AsReadOnly(new string[] { hiddenLanguage });

            string json = JsonConvert.SerializeObject(original, Formatting.Indented);

            AppSettings.Create(json);
        }

        [DataTestMethod]
        [DataRow(null, DisplayName = "Null")]
        [DataRow("", DisplayName = "Empty")]
        [DataRow("incomplete.com", DisplayName = "Malformed")]
        [DataRow(@"D:\FakeFolder\fakeFile.txt", DisplayName = "FilePath")]
        public void IncorrectHomeWebsite(string homeWebsite)
        {
            AppSettings original = ValidAppSettings();
            original.HomeWebsite = homeWebsite;

            string json = JsonConvert.SerializeObject(original, Formatting.Indented);

            bool threwException = true;
            try
            {
                AppSettings.Create(json);
            }
            catch (MalformedWebsiteException exception)
            {
                threwException = true;
                Assert.AreEqual(homeWebsite, exception.Uri, "Returned incorrect malformed website.");
            }

            Assert.IsTrue(threwException, "Failed to throw a MalformedWebsiteException");
        }

        [DataTestMethod]
        [DataRow(null, DisplayName = "Null")]
        [DataRow("", DisplayName = "Empty")]
        [DataRow("incomplete.com", DisplayName = "Malformed")]
        [DataRow(@"D:\FakeFolder\fakeFile.txt", DisplayName = "FilePath")]
        public void IncorrectDonationWebsite(string donationWebsite)
        {
            AppSettings original = ValidAppSettings();
            original.DonationWebsite = donationWebsite;

            string json = JsonConvert.SerializeObject(original, Formatting.Indented);

            bool threwException = true;
            try
            {
                AppSettings.Create(json);
            }
            catch (MalformedWebsiteException exception)
            {
                threwException = true;
                Assert.AreEqual(donationWebsite, exception.Uri, "Returned incorrect malformed website.");
            }

            Assert.IsTrue(threwException, "Failed to throw a MalformedWebsiteException");
        }

        [DataTestMethod]
        [DataRow(null, DisplayName = "Null")]
        [DataRow("", DisplayName = "Empty")]
        [DataRow("incomplete.com", DisplayName = "Malformed")]
        [DataRow("https://fake.com", DisplayName = "Website")]
        public void IncorrectSupportEmail(string supportEmail)
        {
            AppSettings original = ValidAppSettings();
            original.SupportEmail = supportEmail;

            string json = JsonConvert.SerializeObject(original, Formatting.Indented);

            bool threwException = true;
            try
            {
                AppSettings.Create(json);
            }
            catch (MalformedEmailException exception)
            {
                threwException = true;
                Assert.AreEqual(supportEmail, exception.Email, "Returned incorrect malformed email.");
            }

            Assert.IsTrue(threwException, "Failed to throw a MalformedEmailException");
        }

        private void IncorrectProductId(GoogleAnalyticsSettings googleAnalyticsSettings, string variableName, string productKey)
        {
            System.Reflection.PropertyInfo propInfo = googleAnalyticsSettings.GetType().GetProperty(variableName);
            propInfo.SetValue(googleAnalyticsSettings, productKey, index: null);

            AppSettings appSettings = AppSettingsIncluding(googleAnalyticsSettings);
            string json = JsonConvert.SerializeObject(appSettings, Formatting.Indented);

            bool threwException = true;
            try
            {
                AppSettings.Create(json);
            }
            catch (InvalidProductKeyException exception)
            {
                threwException = true;
                Assert.AreEqual(variableName, exception.ProductKeyName, "Returned incorrect ProductKeyName.");
                Assert.AreEqual(productKey, exception.ProductKeyValue, "Returned incorrect ProductKeyValue.");
            }

            Assert.IsTrue(threwException, "Failed to throw a MalformedEmailException");
        }

        [DataTestMethod]
        [DataRow(null, DisplayName = "Null")]
        [DataRow("", DisplayName = "Empty")]
        public void IncorrectMeasurementIdDev(string productKey)
        {
            GoogleAnalyticsSettings original = ValidGoogleAnalyticsSettings();

            IncorrectProductId(original, nameof(original.MeasurementIdDev), productKey);
        }

        [DataTestMethod]
        [DataRow(null, DisplayName = "Null")]
        [DataRow("", DisplayName = "Empty")]
        public void IncorrectMeasurementIdProd(string productKey)
        {
            GoogleAnalyticsSettings original = ValidGoogleAnalyticsSettings();

            IncorrectProductId(original, nameof(original.MeasurementIdProd), productKey);
        }

        [DataTestMethod]
        [DataRow(null, DisplayName = "Null")]
        [DataRow("", DisplayName = "Empty")]
        public void IncorrectAdditionalIdDev(string productKey)
        {
            GoogleAnalyticsSettings original = ValidGoogleAnalyticsSettings();

            IncorrectProductId(original, nameof(original.AdditionalIdDev), productKey);
        }

        [DataTestMethod]
        [DataRow(null, DisplayName = "Null")]
        [DataRow("", DisplayName = "Empty")]
        public void IncorrectAdditionalIdProd(string productKey)
        {
            GoogleAnalyticsSettings original = ValidGoogleAnalyticsSettings();

            IncorrectProductId(original, nameof(original.AdditionalIdProd), productKey);
        }
    }
}
