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
using NpcGenerator;
using System;
using System.IO;

namespace Tests
{
    [TestClass]
    public class TrackingProfileTests : FileCreatingTests
    {
        const string FILE_EXTENSION = ".json";

        [TestMethod]
        public void ReadTrackingProfile()
        {
            string text = $@"{{
                'ClientId': '3fd20634 - 698d - 4f58 - 9a16 - 7c8420ce07f9',
                'SystemLanguage': 'en-CA',
                'AppVersion': '1.13.1029.400',
                'OsVersion': 'Microsoft Windows 10 Home'
            }}";

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + FILE_EXTENSION);
            File.WriteAllText(path, text, Constants.TEXT_ENCODING);
            
            TrackingProfile readProfile = TrackingProfile.Load(path);

            //The TrackingProfile is automatically updated with fresh values, so just check that values are plausible.
            Assert.IsNotNull(readProfile.ClientId, "Loaded incorrect ClientId");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.SystemLanguage), "Loaded incorrect SystemLanguage");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.AppVersion), "Loaded incorrect AppVersion");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.OsVersion), "Loaded incorrect OsVersion");

            File.Delete(path);
        }

        [TestMethod]
        public void Save()
        {
            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + FILE_EXTENSION);

            TrackingProfile profile = TrackingProfile.Load(path);

            Guid CLIENT_ID = new Guid("3fd20634-698d-4f58-9a16-7c8420ce07f9");
            const string SYSTEM_LANGUAGE = "en-CA";
            const string APP_VERSION = "1.1.0.0";
            const string OS_VERSION = "SAL";

            profile.ClientId = CLIENT_ID;
            profile.SystemLanguage = SYSTEM_LANGUAGE;
            profile.AppVersion = APP_VERSION;
            profile.OsVersion = OS_VERSION;

            profile.Save();

            Assert.AreEqual(CLIENT_ID, profile.ClientId, "Save changed value of ClientId");
            Assert.AreEqual(SYSTEM_LANGUAGE, profile.SystemLanguage, "Save changed value of SystemLanguage");
            Assert.AreEqual(APP_VERSION, profile.AppVersion, "Save changed value of AppVersion");
            Assert.AreEqual(OS_VERSION, profile.OsVersion, "Save changed value of OsVersion");

            File.Delete(path);
        }

        [TestMethod]
        public void FileNotFound()
        {
            TrackingProfile readProfile = TrackingProfile.Load("FileNotFOund");

            //The TrackingProfile is automatically updated with fresh values, so just check that values are plausible.
            Assert.IsNotNull(readProfile.ClientId, "Loaded incorrect ClientId");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.SystemLanguage), "Loaded incorrect SystemLanguage");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.AppVersion), "Loaded incorrect AppVersion");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.OsVersion), "Loaded incorrect OsVersion");
        }

        [TestMethod]
        public void EmptyFile()
        {
            string text = "{}";

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + FILE_EXTENSION);
            File.WriteAllText(path, text, Constants.TEXT_ENCODING);

            TrackingProfile readProfile = TrackingProfile.Load(path);

            //The TrackingProfile is automatically updated with fresh values, so just check that values are plausible.
            Assert.IsNotNull(readProfile.ClientId, "Loaded incorrect ClientId");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.SystemLanguage), "Loaded incorrect SystemLanguage");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.AppVersion), "Loaded incorrect AppVersion");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.OsVersion), "Loaded incorrect OsVersion");

            File.Delete(path);
        }

        [TestMethod]
        public void MalformedFile()
        {
            string text = "{'ClientId':}";

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + FILE_EXTENSION);
            File.WriteAllText(path, text, Constants.TEXT_ENCODING);

            TrackingProfile readProfile = TrackingProfile.Load(path);

            //The TrackingProfile is automatically updated with fresh values, so just check that values are plausible.
            Assert.IsNotNull(readProfile.ClientId, "Loaded incorrect ClientId");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.SystemLanguage), "Loaded incorrect SystemLanguage");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.AppVersion), "Loaded incorrect AppVersion");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.OsVersion), "Loaded incorrect OsVersion");

            File.Delete(path);
        }

        [TestMethod]
        public void MissingClientId()
        {
            string text = $@"{{
                'SystemLanguage': 'en-CA',
                'AppVersion': '1.13.1029.400',
                'OsVersion': 'Microsoft Windows 10 Home'
            }}";

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + FILE_EXTENSION);
            File.WriteAllText(path, text, Constants.TEXT_ENCODING);

            TrackingProfile readProfile = TrackingProfile.Load(path);

            //The TrackingProfile is automatically updated with fresh values, so just check that values are plausible.
            Assert.IsNotNull(readProfile.ClientId, "Loaded incorrect ClientId");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.SystemLanguage), "Loaded incorrect SystemLanguage");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.AppVersion), "Loaded incorrect AppVersion");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.OsVersion), "Loaded incorrect OsVersion");

            File.Delete(path);
        }

        [TestMethod]
        public void EmptyClientId()
        {
            string text = $@"{{
                'ClientId': '',
                'SystemLanguage': 'en-CA',
                'AppVersion': '1.13.1029.400',
                'OsVersion': 'Microsoft Windows 10 Home'
            }}";

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + FILE_EXTENSION);
            File.WriteAllText(path, text, Constants.TEXT_ENCODING);

            TrackingProfile readProfile = TrackingProfile.Load(path);

            //The TrackingProfile is automatically updated with fresh values, so just check that values are plausible.
            Assert.IsNotNull(readProfile.ClientId, "Loaded incorrect ClientId");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.SystemLanguage), "Loaded incorrect SystemLanguage");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.AppVersion), "Loaded incorrect AppVersion");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.OsVersion), "Loaded incorrect OsVersion");

            File.Delete(path);
        }

        [TestMethod]
        public void MalformedClientId()
        {
            string text = $@"{{
                'ClientId': '1',
                'SystemLanguage': 'en-CA',
                'AppVersion': '1.13.1029.400',
                'OsVersion': 'Microsoft Windows 10 Home'
            }}";

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + FILE_EXTENSION);
            File.WriteAllText(path, text, Constants.TEXT_ENCODING);

            TrackingProfile readProfile = TrackingProfile.Load(path);

            //The TrackingProfile is automatically updated with fresh values, so just check that values are plausible.
            Assert.IsNotNull(readProfile.ClientId, "Loaded incorrect ClientId");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.SystemLanguage), "Loaded incorrect SystemLanguage");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.AppVersion), "Loaded incorrect AppVersion");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.OsVersion), "Loaded incorrect OsVersion");

            File.Delete(path);
        }

        [TestMethod]
        public void ClientIdIntInsteadOfGuid()
        {
            string text = $@"{{
                'ClientId': 1,
                'SystemLanguage': 'en-CA',
                'AppVersion': '1.13.1029.400',
                'OsVersion': 'Microsoft Windows 10 Home'
            }}";

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + FILE_EXTENSION);
            File.WriteAllText(path, text, Constants.TEXT_ENCODING);

            TrackingProfile readProfile = TrackingProfile.Load(path);

            //The TrackingProfile is automatically updated with fresh values, so just check that values are plausible.
            Assert.IsNotNull(readProfile.ClientId, "Loaded incorrect ClientId");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.SystemLanguage), "Loaded incorrect SystemLanguage");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.AppVersion), "Loaded incorrect AppVersion");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.OsVersion), "Loaded incorrect OsVersion");

            File.Delete(path);
        }

        [TestMethod]
        public void MissingSystemLanguage()
        {
            string text = $@"{{
                'ClientId': '3fd20634 - 698d - 4f58 - 9a16 - 7c8420ce07f9',
                'AppVersion': '1.13.1029.400',
                'OsVersion': 'Microsoft Windows 10 Home'
            }}";

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + FILE_EXTENSION);
            File.WriteAllText(path, text, Constants.TEXT_ENCODING);

            TrackingProfile readProfile = TrackingProfile.Load(path);

            //The TrackingProfile is automatically updated with fresh values, so just check that values are plausible.
            Assert.IsNotNull(readProfile.ClientId, "Loaded incorrect ClientId");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.SystemLanguage), "Loaded incorrect SystemLanguage");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.AppVersion), "Loaded incorrect AppVersion");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.OsVersion), "Loaded incorrect OsVersion");

            File.Delete(path);
        }

        [TestMethod]
        public void EmptySystemLanguage()
        {
            string text = $@"{{
                'ClientId': '3fd20634 - 698d - 4f58 - 9a16 - 7c8420ce07f9',
                'SystemLanguage': '',
                'AppVersion': '1.13.1029.400',
                'OsVersion': 'Microsoft Windows 10 Home'
            }}";

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + FILE_EXTENSION);
            File.WriteAllText(path, text, Constants.TEXT_ENCODING);

            TrackingProfile readProfile = TrackingProfile.Load(path);

            //The TrackingProfile is automatically updated with fresh values, so just check that values are plausible.
            Assert.IsNotNull(readProfile.ClientId, "Loaded incorrect ClientId");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.SystemLanguage), "Loaded incorrect SystemLanguage");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.AppVersion), "Loaded incorrect AppVersion");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.OsVersion), "Loaded incorrect OsVersion");

            File.Delete(path);
        }

        [TestMethod]
        public void MalformedSystemLanguage()
        {
            string text = $@"{{
                'ClientId': '3fd20634 - 698d - 4f58 - 9a16 - 7c8420ce07f9',
                'SystemLanguage': 'Not A Real Language',
                'AppVersion': '1.13.1029.400',
                'OsVersion': 'Microsoft Windows 10 Home'
            }}";

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + FILE_EXTENSION);
            File.WriteAllText(path, text, Constants.TEXT_ENCODING);

            TrackingProfile readProfile = TrackingProfile.Load(path);

            //The TrackingProfile is automatically updated with fresh values, so just check that values are plausible.
            Assert.IsNotNull(readProfile.ClientId, "Loaded incorrect ClientId");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.SystemLanguage), "Loaded incorrect SystemLanguage");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.AppVersion), "Loaded incorrect AppVersion");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.OsVersion), "Loaded incorrect OsVersion");

            File.Delete(path);
        }

        [TestMethod]
        public void SystemLanguageFloatInsteadOfString()
        {
            string text = $@"{{
                'ClientId': '3fd20634 - 698d - 4f58 - 9a16 - 7c8420ce07f9',
                'SystemLanguage': 1.5,
                'AppVersion': '1.13.1029.400',
                'OsVersion': 'Microsoft Windows 10 Home'
            }}";

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + FILE_EXTENSION);
            File.WriteAllText(path, text, Constants.TEXT_ENCODING);

            TrackingProfile readProfile = TrackingProfile.Load(path);

            //The TrackingProfile is automatically updated with fresh values, so just check that values are plausible.
            Assert.IsNotNull(readProfile.ClientId, "Loaded incorrect ClientId");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.SystemLanguage), "Loaded incorrect SystemLanguage");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.AppVersion), "Loaded incorrect AppVersion");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.OsVersion), "Loaded incorrect OsVersion");

            File.Delete(path);
        }

        [TestMethod]
        public void MissingAppVersion()
        {
            string text = $@"{{
                'ClientId': '3fd20634 - 698d - 4f58 - 9a16 - 7c8420ce07f9',
                'SystemLanguage': 'en-CA',
                'OsVersion': 'Microsoft Windows 10 Home'
            }}";

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + FILE_EXTENSION);
            File.WriteAllText(path, text, Constants.TEXT_ENCODING);

            TrackingProfile readProfile = TrackingProfile.Load(path);

            //The TrackingProfile is automatically updated with fresh values, so just check that values are plausible.
            Assert.IsNotNull(readProfile.ClientId, "Loaded incorrect ClientId");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.SystemLanguage), "Loaded incorrect SystemLanguage");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.AppVersion), "Loaded incorrect AppVersion");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.OsVersion), "Loaded incorrect OsVersion");

            File.Delete(path);
        }

        [TestMethod]
        public void EmptyAppVersion()
        {
            string text = $@"{{
                'ClientId': '3fd20634 - 698d - 4f58 - 9a16 - 7c8420ce07f9',
                'SystemLanguage': 'en-CA',
                'AppVersion': '',
                'OsVersion': 'Microsoft Windows 10 Home'
            }}";

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + FILE_EXTENSION);
            File.WriteAllText(path, text, Constants.TEXT_ENCODING);

            TrackingProfile readProfile = TrackingProfile.Load(path);

            //The TrackingProfile is automatically updated with fresh values, so just check that values are plausible.
            Assert.IsNotNull(readProfile.ClientId, "Loaded incorrect ClientId");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.SystemLanguage), "Loaded incorrect SystemLanguage");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.AppVersion), "Loaded incorrect AppVersion");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.OsVersion), "Loaded incorrect OsVersion");

            File.Delete(path);
        }

        [TestMethod]
        public void AppVersionHasLetters()
        {
            string text = $@"{{
                'ClientId': '3fd20634 - 698d - 4f58 - 9a16 - 7c8420ce07f9',
                'SystemLanguage': 'en-CA',
                'AppVersion': 'a.b.c.d',
                'OsVersion': 'Microsoft Windows 10 Home'
            }}";

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + FILE_EXTENSION);
            File.WriteAllText(path, text, Constants.TEXT_ENCODING);

            TrackingProfile readProfile = TrackingProfile.Load(path);

            //The TrackingProfile is automatically updated with fresh values, so just check that values are plausible.
            Assert.IsNotNull(readProfile.ClientId, "Loaded incorrect ClientId");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.SystemLanguage), "Loaded incorrect SystemLanguage");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.AppVersion), "Loaded incorrect AppVersion");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.OsVersion), "Loaded incorrect OsVersion");

            File.Delete(path);
        }

        [TestMethod]
        public void AppVersionTooShort()
        {
            string text = $@"{{
                'ClientId': '3fd20634 - 698d - 4f58 - 9a16 - 7c8420ce07f9',
                'SystemLanguage': 'en-CA',
                'AppVersion': '1.1.1',
                'OsVersion': 'Microsoft Windows 10 Home'
            }}";

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + FILE_EXTENSION);
            File.WriteAllText(path, text, Constants.TEXT_ENCODING);

            TrackingProfile readProfile = TrackingProfile.Load(path);

            //The TrackingProfile is automatically updated with fresh values, so just check that values are plausible.
            Assert.IsNotNull(readProfile.ClientId, "Loaded incorrect ClientId");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.SystemLanguage), "Loaded incorrect SystemLanguage");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.AppVersion), "Loaded incorrect AppVersion");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.OsVersion), "Loaded incorrect OsVersion");

            File.Delete(path);
        }

        [TestMethod]
        public void AppVersionTooLong()
        {
            string text = $@"{{
                'ClientId': '3fd20634 - 698d - 4f58 - 9a16 - 7c8420ce07f9',
                'SystemLanguage': 'en-CA',
                'AppVersion': '1.1.1.1.1',
                'OsVersion': 'Microsoft Windows 10 Home'
            }}";

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + FILE_EXTENSION);
            File.WriteAllText(path, text, Constants.TEXT_ENCODING);

            TrackingProfile readProfile = TrackingProfile.Load(path);

            //The TrackingProfile is automatically updated with fresh values, so just check that values are plausible.
            Assert.IsNotNull(readProfile.ClientId, "Loaded incorrect ClientId");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.SystemLanguage), "Loaded incorrect SystemLanguage");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.AppVersion), "Loaded incorrect AppVersion");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.OsVersion), "Loaded incorrect OsVersion");

            File.Delete(path);
        }

        [TestMethod]
        public void AppVersionIsNegative()
        {
            string text = $@"{{
                'ClientId': '3fd20634 - 698d - 4f58 - 9a16 - 7c8420ce07f9',
                'SystemLanguage': 'en-CA',
                'AppVersion': '-1.1.1.1',
                'OsVersion': 'Microsoft Windows 10 Home'
            }}";

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + FILE_EXTENSION);
            File.WriteAllText(path, text, Constants.TEXT_ENCODING);

            TrackingProfile readProfile = TrackingProfile.Load(path);

            //The TrackingProfile is automatically updated with fresh values, so just check that values are plausible.
            Assert.IsNotNull(readProfile.ClientId, "Loaded incorrect ClientId");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.SystemLanguage), "Loaded incorrect SystemLanguage");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.AppVersion), "Loaded incorrect AppVersion");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.OsVersion), "Loaded incorrect OsVersion");

            File.Delete(path);
        }

        [TestMethod]
        public void AppVersionIsBoolInsteadOfString()
        {
            string text = $@"{{
                'ClientId': '3fd20634 - 698d - 4f58 - 9a16 - 7c8420ce07f9',
                'SystemLanguage': 'en-CA',
                'AppVersion': false,
                'OsVersion': 'Microsoft Windows 10 Home'
            }}";

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + FILE_EXTENSION);
            File.WriteAllText(path, text, Constants.TEXT_ENCODING);

            TrackingProfile readProfile = TrackingProfile.Load(path);

            //The TrackingProfile is automatically updated with fresh values, so just check that values are plausible.
            Assert.IsNotNull(readProfile.ClientId, "Loaded incorrect ClientId");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.SystemLanguage), "Loaded incorrect SystemLanguage");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.AppVersion), "Loaded incorrect AppVersion");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.OsVersion), "Loaded incorrect OsVersion");

            File.Delete(path);
        }

        [TestMethod]
        public void EmptyOsVersion()
        {
            string text = $@"{{
                'ClientId': '3fd20634 - 698d - 4f58 - 9a16 - 7c8420ce07f9',
                'SystemLanguage': 'en-CA',
                'AppVersion': false,
                'OsVersion': ''
            }}";

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + FILE_EXTENSION);
            File.WriteAllText(path, text, Constants.TEXT_ENCODING);

            TrackingProfile readProfile = TrackingProfile.Load(path);

            //The TrackingProfile is automatically updated with fresh values, so just check that values are plausible.
            Assert.IsNotNull(readProfile.ClientId, "Loaded incorrect ClientId");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.SystemLanguage), "Loaded incorrect SystemLanguage");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.AppVersion), "Loaded incorrect AppVersion");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.OsVersion), "Loaded incorrect OsVersion");

            File.Delete(path);
        }

        [TestMethod]
        public void MissingOsVersion()
        {
            string text = $@"{{
                'ClientId': '3fd20634 - 698d - 4f58 - 9a16 - 7c8420ce07f9',
                'SystemLanguage': 'en-CA',
                'AppVersion': false,
            }}";

            string method = System.Reflection.MethodBase.GetCurrentMethod().Name;
            string path = Path.Combine(TestDirectory, method + FILE_EXTENSION);
            File.WriteAllText(path, text, Constants.TEXT_ENCODING);

            TrackingProfile readProfile = TrackingProfile.Load(path);

            //The TrackingProfile is automatically updated with fresh values, so just check that values are plausible.
            Assert.IsNotNull(readProfile.ClientId, "Loaded incorrect ClientId");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.SystemLanguage), "Loaded incorrect SystemLanguage");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.AppVersion), "Loaded incorrect AppVersion");
            Assert.IsFalse(string.IsNullOrWhiteSpace(readProfile.OsVersion), "Loaded incorrect OsVersion");

            File.Delete(path);
        }
    }
}
