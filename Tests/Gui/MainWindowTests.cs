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
    public class MainWindowTests
    {
        private class MockTrackingProfile : ITrackingProfile
        {
            public Guid ClientId { get; } = Guid.Empty;
            public string Language { get; } = null;
            public string AppVersion { get; } = null;
            public string OSVersion { get; } = null;
        }

        private class MockGoogleAnalyticsSettings : IGoogleAnalyticsSettings
        {
            public string MeasurementIdDev { get; } = null;
            public string MeasurementIdProd { get; } = null;
            public string AdditionalIdDev { get; } = null;
            public string AdditionalIdProd { get; } = null;
        }

        private class MockAppSettings : IAppSettings
        {
            public IGoogleAnalyticsSettings GoogleAnalytics { get; } = new MockGoogleAnalyticsSettings();
            public int EncryptionKey { get; } = 0;
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
            public string AppDataFolder { get; } = null;
            public string LicensePath { get; } = null;
            public string UserSettingsFilePath { get; } = null;
            public string AppSettingsFilePath { get; } = null;
            public string TrackingProfileFilePath { get; } = null;
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

        public MainWindowTests()
        {
            FilePathProvider filePathProvider = new FilePathProvider();
            string commonAppData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            m_testDirectory = Path.Combine(commonAppData, filePathProvider.AppDataFolder, "UnitTestInput");
            Directory.CreateDirectory(m_testDirectory);
        }

        [TestMethod]
        public void EndToEnd()
        {
            Thread t = new Thread(new ThreadStart(delegate ()
            {
                //********* Setup Variables ********************
                const string configFileName = "TestConfig.csv";
                string configPath = Path.Combine(m_testDirectory, configFileName);
                string text = "Colour,Weight\n" +
                    "Green,1\n" +
                    "Red,1";
                File.WriteAllText(configPath, text);

                MockUserSettings testUserSettings = new MockUserSettings
                {
                    ConfigurationPath = configPath,
                    NpcQuantity = 5
                };

                MockLocalFileIO fileIO = new MockLocalFileIO();

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
                Assert.AreEqual(configPath, configLabel.Content, "Configuration file was changed by Main Window");

                TextBox quantityLabel = (TextBox)mainWindow.FindName("npcQuantityText");
                int quantity = int.Parse(quantityLabel.Text);
                Assert.AreEqual(testUserSettings.NpcQuantity, quantity, "NPC Quantity was changed by Main Window");

                //********* Test Generate ********************
                Button generate = (Button)mainWindow.FindName("generateButton");
                generate.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

                DataGrid dataGrid = (DataGrid)mainWindow.FindName("generatedNpcTable");
                DataTable dataTable = (DataTable)dataGrid.DataContext;
                Assert.AreEqual(testUserSettings.NpcQuantity, dataTable.Rows.Count, "Incorrect number of NPCs generated");

                //********* Test Save ********************
                Button save = (Button)mainWindow.FindName("saveNpcsButton");
                save.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

                Assert.IsTrue(fileIO.SaveCalled, "saveNpcsButton did not invoke ILocalFileIO.SaveToPickedFile");

                //********* Clean Up ********************
                File.Delete(configPath);

            }));

            t.SetApartmentState(ApartmentState.STA);

            t.Start();
            t.Join();

        }

        private readonly string m_testDirectory;
    }
}
