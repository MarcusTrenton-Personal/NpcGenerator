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
using NpcGenerator.Message;
using System;
using System.Data;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Tests
{
    [TestClass]
    public class MainWindowTests : FileCreatingTests
    {
        private class MockTrackingProfile : ITrackingProfile
        {
            public Guid ClientId { get; set; } = Guid.Empty;
            public string Language { get; set; } = null;
            public string AppVersion { get; set; } = null;
            public string OSVersion { get; set; } = null;
        }

        private class MockGoogleAnalyticsSettings : IGoogleAnalyticsSettings
        {
            public string MeasurementIdDev { get; set; } = null;
            public string MeasurementIdProd { get; set; } = null;
            public string AdditionalIdDev { get; set; } = null;
            public string AdditionalIdProd { get; set; } = null;
        }

        private class MockAppSettings : IAppSettings
        {
            public IGoogleAnalyticsSettings GoogleAnalytics { get; set; } = new MockGoogleAnalyticsSettings();
            public int EncryptionKey { get; set; } = 0;
        }

        private class MockMessager : IMessager
        {
            public void Send<T>(object sender, T message) { }

            public void Subscribe<T>(IChannel<T>.Callback callback) { }

            public void Unsubscribe<T>(IChannel<T>.Callback callback) { }
        }

        private class MockUserSettings : IUserSettings
        {
            public string ConfigurationPath { get; set; } = null;

            public int NpcQuantity { get; set; } = 1;

            public void Save(string path) { }
        }

        private class MockFilePathProvider : IFilePathProvider
        {
            public string AppDataFolderPath { get; set; } = null;
            public string LicensePath { get; set; } = null;
            public string UserSettingsFilePath { get; set; } = null;
            public string AppSettingsFilePath { get; set; } = null;
            public string TrackingProfileFilePath { get; set; } = null;
        }

        private class MockLocalFileIO : ILocalFileIO
        {
            public string CacheFile(string originalPath)
            {
                return originalPath;
            }

            public void SaveToPickedFile(string content, string fileType) 
            {
                SaveCalled = true;
            }

            public bool SaveCalled { get; set; } = false;
        }

        //An aborted test is actually a failure. Run with debug to determine what failed.
        [TestMethod]
        public void EndToEnd()
        {
            bool uncaughtException = false;
            bool configPathMatches = false;
            bool npcQuantityLabelMachesUserSettings = false;
            bool generatedNpcQuantityMatchesUserSettings = false;

            MockLocalFileIO fileIO = new MockLocalFileIO();

            Thread t = new Thread(new ThreadStart(delegate ()
            {
                try
                {
                    //********* Setup Variables ********************
                    const string configFileName = "TestConfig.csv";
                    string configPath = Path.Combine(TestDirectory, configFileName);
                    string text = "Colour,Weight\n" +
                        "Green,1\n" +
                        "Red,1";
                    File.WriteAllText(configPath, text);

                    MockUserSettings testUserSettings = new MockUserSettings
                    {
                        ConfigurationPath = configPath,
                        NpcQuantity = 5
                    };

                    ServiceCenter serviceCenter = new ServiceCenter(
                        profile: new MockTrackingProfile(),
                        appSettings: new MockAppSettings(),
                        messager: new MockMessager(),
                        userSettings: testUserSettings,
                        filePathProvider: new MockFilePathProvider(),
                        fileIO: fileIO
                    );
                    MainWindow mainWindow = new MainWindow(serviceCenter);

                    //********* Test Initial Window ********************
                    Label configLabel = (Label)mainWindow.FindName("configurationPathText");
                    configPathMatches = configPath == configLabel.Content.ToString();

                    TextBox quantityLabel = (TextBox)mainWindow.FindName("npcQuantityText");
                    int quantity = int.Parse(quantityLabel.Text);
                    npcQuantityLabelMachesUserSettings = testUserSettings.NpcQuantity == quantity;

                    //********* Test Generate ********************
                    Button generate = (Button)mainWindow.FindName("generateButton");
                    generate.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

                    DataGrid dataGrid = (DataGrid)mainWindow.FindName("generatedNpcTable");
                    DataTable dataTable = (DataTable)dataGrid.DataContext;
                    generatedNpcQuantityMatchesUserSettings = testUserSettings.NpcQuantity == dataTable.Rows.Count;

                    //********* Test Save ********************
                    Button save = (Button)mainWindow.FindName("saveNpcsButton");
                    save.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    //Track the result of event through fileIO.SavedCalled.

                    //********* Clean Up ********************
                    File.Delete(configPath);
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

            Assert.IsTrue(configPathMatches, "Configuration file was changed by Main Window");
            Assert.IsTrue(npcQuantityLabelMachesUserSettings, "NPC Quantity was changed by Main Window");
            Assert.IsTrue(generatedNpcQuantityMatchesUserSettings, "Incorrect number of NPCs generated");
            Assert.IsTrue(fileIO.SaveCalled, "saveNpcsButton did not invoke ILocalFileIO.SaveToPickedFile");
            Assert.IsFalse(uncaughtException, "Test failed from uncaught exception");
        }
    }
}
