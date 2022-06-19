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
        //An aborted test is actually a failure. Run with debug to determine what failed.
        [TestMethod]
        public void EndToEnd()
        {
            Exception uncaughtException = null;
            bool configPathMatches = false;
            bool npcQuantityLabelMachesUserSettings = false;
            bool generatedNpcQuantityMatchesUserSettings = false;

            StubLocalFileIO fileIO = new StubLocalFileIO();

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

                    StubUserSettings testUserSettings = new StubUserSettings
                    {
                        ConfigurationPath = configPath,
                        NpcQuantity = 5
                    };

                    StubLocalization testLocalization = new StubLocalization
                    {
                        SupportedLanguageCodes = new[] { "en-ca" }
                    };

                    StubLocalizationModel testLocalizationModel = new StubLocalizationModel
                    {
                        Localization = testLocalization
                    };

                    Models models = new Models(
                        localization: testLocalizationModel, 
                        about: new StubAboutModel(),
                        navigation: new StubNavigationModel(),
                        tracking: new StubTrackingModel());

                    ServiceCenter serviceCenter = new ServiceCenter(
                        profile: new StubTrackingProfile(),
                        appSettings: new StubAppSettings(),
                        messager: new StubMessager(),
                        userSettings: testUserSettings,
                        filePathProvider: new StubFilePathProvider(),
                        fileIO: fileIO,
                        localization: testLocalization,
                        models: models
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
                catch (Exception e)
                {
                    uncaughtException = e;
                }
            }));

            t.SetApartmentState(ApartmentState.STA);

            t.Start();
            t.Join();

            Assert.IsNull(uncaughtException, "Test failed from uncaught exception: " + uncaughtException ?? uncaughtException.ToString());
            Assert.IsTrue(configPathMatches, "Configuration file was changed by Main Window");
            Assert.IsTrue(npcQuantityLabelMachesUserSettings, "NPC Quantity was changed by Main Window");
            Assert.IsTrue(generatedNpcQuantityMatchesUserSettings, "Incorrect number of NPCs generated");
            Assert.IsTrue(fileIO.SaveCalled, "saveNpcsButton did not invoke ILocalFileIO.SaveToPickedFile");
        }
    }
}
