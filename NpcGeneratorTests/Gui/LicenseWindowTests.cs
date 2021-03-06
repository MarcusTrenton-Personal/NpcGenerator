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
using System;
using System.IO;
using System.Threading;
using System.Windows.Controls;

namespace Tests
{
    [TestClass]
    public class LicenseWindowTests : FileCreatingTests
    {
        //An aborted test is actually a failure. Run with debug to determine what failed.
        [TestMethod]
        public void EndToEnd()
        {
            bool scrollDocumentExists = false;
            Exception uncaughtException = null;

            Thread t = new Thread(new ThreadStart(delegate ()
            {
                try
                {
                    //********* Setup Variables ********************
                    const string licenseFileName = "TestLicense.rtf";
                    string licensePath = Path.Combine(TestDirectory, licenseFileName);
                    //From https://en.wikipedia.org/wiki/Rich_Text_Format
                    string text = @"{\rtf1\ansi{\fonttbl\f0\fswiss Helvetica;}\f0\pard
                        This is some {\b bold} text.\par
                        }";
                    //string text = "";
                    File.WriteAllText(licensePath, text);

                    StubFilePathProvider filePathProvider = new StubFilePathProvider()
                    {
                        LicensePath = licensePath
                    };

                    StubLocalization testLocalization = new StubLocalization
                    {
                        SupportedLanguageCodes = new[] { "en-ca" }
                    };

                    StubLocalizationModel testLocalizationModel = new StubLocalizationModel
                    {
                        Localization = testLocalization
                    };

                    LicenseWindow licenseWindow = new LicenseWindow(
                        messager: new StubMessager(), 
                        filePathProvider: filePathProvider,
                        localizationModel: testLocalizationModel);

                    //********* Test Initial Window ********************
                    FlowDocumentScrollViewer scrollViewer = (FlowDocumentScrollViewer)licenseWindow.FindName("flowViewer");
                    scrollDocumentExists = scrollViewer.Document != null;

                    //********* Clean Up ********************
                    File.Delete(licensePath);
                }
                //Any uncaught exception in this thread will deadlock the parent thread, causing the test to abort instead of fail.
                //Therefore, every exception must be caught and explicitly marked as failure.
                catch (Exception e)
                {
                    uncaughtException = e;
                }
            }));

            t.SetApartmentState(ApartmentState.STA);

            t.Start();
            t.Join();

            Assert.IsNull(uncaughtException, "Test failed from uncaught exception: " + uncaughtException ?? uncaughtException.ToString());
            Assert.IsTrue(scrollDocumentExists, "License scroll viewer is empty");
        }
    }
}
