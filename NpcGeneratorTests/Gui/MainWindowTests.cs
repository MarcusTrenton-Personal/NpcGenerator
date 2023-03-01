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
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Controls;

namespace Tests
{

    [TestClass]
    public class MainWindowTests : FileCreatingTests
    {
        [TestMethod]
        public void EndToEnd()
        {
            Exception uncaughtException = null;
            bool configPathMatches = false;
            bool npcQuantityLabelMachesUserSettings = false;
            bool generatedNpcQuantityMatchesUserSettings = false;

            StubLocalFileIo fileIO = new StubLocalFileIo();

            ThreadCreatingTests.StartInUiThread(delegate ()
            {
                try
                {
                    //********* Setup Variables ********************
                    const string configFileName = "TestConfig.csv";
                    string configPath = Path.Combine(TestDirectory, configFileName);
                    string text = "Colour,Weight\n" +
                        "Green,1\n" +
                        "Red,1";
                    File.WriteAllText(configPath, text, Constants.TEXT_ENCODING);

                    StubUserSettings testUserSettings = new StubUserSettings
                    {
                        ConfigurationPath = configPath,
                        NpcQuantity = 5
                    };

                    StubAppSettings testAppSettings = new StubAppSettings
                    {
                        SupportEmail = "fake@test.com"
                    };

                    StubMessager testMessager = new StubMessager();

                    StubLocalization testLocalization = new StubLocalization
                    {
                        SupportedLanguageCodes = new[] { "en-ca" }
                    };

                    StubLocalizationModel testLocalizationModel = new StubLocalizationModel
                    {
                        Localization = testLocalization
                    };

                    MockCsvConfigurationParser configurationParser = new MockCsvConfigurationParser();
                    Dictionary<string, INpcExport> npcExporters = new Dictionary<string, INpcExport>();

                    MockRandom random = new MockRandom();

                    Models models = new Models(
                        localization: testLocalizationModel, 
                        about: new StubAboutModel(),
                        navigation: new StubNavigationModel(),
                        tracking: new StubTrackingModel(),
                        //Only the npcGenerator is real.
                        npcGenerator: new NpcGeneratorModel(
                            testUserSettings,
                            testAppSettings,
                            testMessager, 
                            fileIO, 
                            configurationParser, 
                            npcExporters, 
                            new StubLocalization(), 
                            random, 
                            showErrorMessages: false,
                            forceFailNpcValidation: false,
                            forceNpcGenerationUncaughtException: false));

                    ServiceCentre serviceCentre = new ServiceCentre(
                        profile: new StubTrackingProfile(),
                        appSettings: new StubAppSettings(),
                        messager: testMessager,
                        userSettings: testUserSettings,
                        filePathProvider: new StubFilePathProvider(),
                        fileIo: fileIO,
                        localization: testLocalization,
                        configurationParser: configurationParser,
                        npcExporters: npcExporters,
                        random: random,
                        models: models
                    );
                    MainWindow mainWindow = new MainWindow(serviceCentre);

                    //********* Test Initial Window ********************
                    TextBlock configText = (TextBlock)mainWindow.FindName("configurationPathText");
                    configPathMatches = configPath == configText.Text.ToString();

                    TextBox quantityLabel = (TextBox)mainWindow.FindName("npcQuantityText");
                    int quantity = int.Parse(quantityLabel.Text);
                    npcQuantityLabelMachesUserSettings = testUserSettings.NpcQuantity == quantity;

                    //********* Test Generate ********************
                    Button generate = (Button)mainWindow.FindName("generateButton");
                    //Click event's don't trigger Commands while the window isn't shown, so invoke directly.
                    generate.Command.Execute(null);

                    DataGrid dataGrid = (DataGrid)mainWindow.FindName("generatedNpcTable");
                    DataView dataView = (DataView)dataGrid.ItemsSource;
                    generatedNpcQuantityMatchesUserSettings = testUserSettings.NpcQuantity == dataView.Count;

                    //********* Test Save ********************
                    Button save = (Button)mainWindow.FindName("saveNpcsButton");
                    save.Command.Execute(null);
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
            });

            Assert.IsNull(uncaughtException, "Test failed from uncaught exception: " + uncaughtException ?? uncaughtException.ToString());
            Assert.IsTrue(configPathMatches, "Configuration file was changed by Main Window");
            Assert.IsTrue(npcQuantityLabelMachesUserSettings, "NPC Quantity was changed by Main Window");
            Assert.IsTrue(generatedNpcQuantityMatchesUserSettings, "Incorrect number of NPCs generated");
            Assert.IsTrue(fileIO.SaveCalled, "saveNpcsButton did not invoke ILocalFileIO.SaveToPickedFile");
        }

        [TestMethod]
        public void NullServiceCentre()
        {
            Exception uncaughtException = null;

            ThreadCreatingTests.StartInUiThread(delegate ()
            {
                try
                {
                    new MainWindow(null);
                }
                //Any uncaught exception in this thread will deadlock the parent thread, causing the test to abort instead of fail.
                //Therefore, every exception must be caught and - unless explicitly expected - marked as failure.
                catch (Exception e)
                {
                    if (!(e is ArgumentNullException))
                    {
                        uncaughtException = e;
                    }
                }
            });

            Assert.IsNull(uncaughtException, "Test failed from uncaught exception: " + uncaughtException ?? uncaughtException.ToString());
        }
    }
}
