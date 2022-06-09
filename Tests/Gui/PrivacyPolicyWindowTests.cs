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
using Services;
using Services.Message;
using System;
using System.IO;
using System.Threading;
using System.Windows.Controls;

namespace Tests
{
    [TestClass]
    public class PrivacyPolicyWindowTests : FileCreatingTests
    {
        private class MockMessager : IMessager
        {
            public void Send<T>(object sender, T message) { }

            public void Subscribe<T>(IChannel<T>.Callback callback) { }

            public void Unsubscribe<T>(IChannel<T>.Callback callback) { }
        }

        private class MockFilePathProvider : IFilePathProvider
        {
            public string AppDataFolderPath { get; set; } = null;
            public string AppSettingsFilePath { get; set; } = null;
            public string LicensePath { get; set; } = null;
            public string LocalizationPath { get; } = null;
            public string PrivacyPolicyPath { get; set; } = null;
            public string TrackingProfileFilePath { get; set; } = null;
            public string UserSettingsFilePath { get; set; } = null;
        }

        private class MockLocalization : ILocalization
        {
            public string[] SupportedLanguageCodes { get; set; } = null;
            public string CurrentLanguageCode { get; set; } = null;

            public bool IsLanguageCodeSupported(string languageCode)
            {
                return false;
            }

            public string GetText(string textId, params object[] formatParameters)
            {
                return textId;
            }
        }

        //An aborted test is actually a failure. Run with debug to determine what failed.
        [TestMethod]
        public void EndToEnd()
        {
            bool scrollDocumentExists = false;
            bool uncaughtException = false;

            Thread t = new Thread(new ThreadStart(delegate ()
            {
                try
                {
                    //********* Setup Variables ********************
                    const string privacyPolicyFileName = "TestPolicy.rtf";
                    string privacyPolicyPath = Path.Combine(TestDirectory, privacyPolicyFileName);
                    //From https://en.wikipedia.org/wiki/Rich_Text_Format
                    string text = @"{\rtf1\ansi{\fonttbl\f0\fswiss Helvetica;}\f0\pard
                        This is some {\b bold} text.\par
                        }";
                    //string text = "";
                    File.WriteAllText(privacyPolicyPath, text);

                    MockFilePathProvider filePathProvider = new MockFilePathProvider()
                    {
                        PrivacyPolicyPath = privacyPolicyPath
                    };

                    PrivacyPolicyWindow privacyPolicyWindow = new PrivacyPolicyWindow(
                        messager: new MockMessager(), 
                        filePathProvider: filePathProvider,
                        localization: new MockLocalization());

                    //********* Test Initial Window ********************
                    FlowDocumentScrollViewer scrollViewer = (FlowDocumentScrollViewer)privacyPolicyWindow.FindName("flowViewer");
                    scrollDocumentExists = scrollViewer.Document != null;

                    //********* Clean Up ********************
                    File.Delete(privacyPolicyPath);
                }
                //Any uncaught exception in this thread will deadlock the parent thread, causing the test to abort instead of fail.
                //Therefore, every exception must be caught and explicitly marked as failure.
                catch (Exception)
                {
                    uncaughtException = true;
                }
            }));

            t.SetApartmentState(ApartmentState.STA);

            t.Start();
            t.Join();

            Assert.IsTrue(scrollDocumentExists, "Privacy Policy scroll viewer is empty");
            Assert.IsFalse(uncaughtException, "Test failed from uncaught exception");
        }
    }
}
