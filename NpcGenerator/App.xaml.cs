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

using CoupledServices;
using Newtonsoft.Json;
using Services;
using Services.Message;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using WpfServices;

[assembly: CLSCompliant(true)]
namespace NpcGenerator
{
    /// <summary>
    /// Interaction logic for App.xaml. 
    /// Singleton holder since there is no other way to pass data to the other window classes.
    /// </summary>
    public partial class App : Application
    {
        private struct AppParameters
        {
            public bool analyticsDryRun;
            public string forcedLanguageCode;
            public bool forceFailNpcGeneration;
        }

        const string REPAIR_ACTION = "Redownload the application to repair.";

        public App()
        {
            FilePathProvider filePathProvider = new FilePathProvider();
            AppSettings appSettings = LoadAppSettings(filePathProvider);
            if (appSettings is null) //App broken. Abort. Abort.
            {
                return;
            }

            LocalFileIO fileIo = new LocalFileIO(filePathProvider);
            string LocalizationText = File.ReadAllText(filePathProvider.LocalizationPath);
            Services.Localization localization;
            try
            {
                localization = new Services.Localization(LocalizationText, appSettings.DefaultLanguageCode);
            }
            catch (LanguageNotFoundException exception) //App broken. Abort. Abort.
            {
                string message = PathHelper.FullPathOf(filePathProvider.AppSettingsFilePath) + " DefaultLanguageCode " + exception.Language +
                    " is not found in localization file " + PathHelper.FullPathOf(filePathProvider.LocalizationPath) + ". " + REPAIR_ACTION;
                ExitAppAfterPopupClosed(message);
                return;
            }
            catch (LanguageCodeMalformedException exception) //App broken. Abort. Abort.
            {
                string message = PathHelper.FullPathOf(filePathProvider.LocalizationPath) + " localization file has LanguageCode " + 
                    exception.LanguageCode + " that does not fit pattern " + exception.Pattern + ". " + REPAIR_ACTION;
                ExitAppAfterPopupClosed(message);
                return;
            }

            AppParameters parameters = ReadAppParameters();
            m_serviceCentre = CreateServices(parameters, appSettings, filePathProvider, fileIo, localization);
            if (m_serviceCentre is null) //App broken. Abort. Abort.
            {
                return;
            }

            m_googleAnalytics = new GoogleAnalytics(
                 appSettings: m_serviceCentre.AppSettings,
                 trackingProfile: m_serviceCentre.Profile,
                 messager: m_serviceCentre.Messager,
                 userSettings: m_serviceCentre.UserSettings,
                 dryRunValidation: parameters.analyticsDryRun);

            m_serviceCentre.Messager.Send(sender: this, message: new Services.Message.Login());

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnUnhandledException);

            PickLanguage(m_serviceCentre, parameters);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (m_serviceCentre == null) //App broken. Abort. Abort.
            {
                return;
            }

            Current.MainWindow = new MainWindow(m_serviceCentre);       
            Current.MainWindow.Show();
        }

        private static ServiceCentre CreateServices(
            AppParameters parameters, 
            AppSettings appSettings, 
            FilePathProvider filePathProvider, 
            LocalFileIO fileIo,
            Services.Localization localization)
        {
            Messager messager = new Messager();
            TrackingProfile trackingProfile = ReadTrackingProfile(filePathProvider);
            UserSettings userSettings = UserSettings.Load(filePathProvider.UserSettingsFilePath);

            LocalizationModel localizationModel;
            try
            {
                localizationModel = new LocalizationModel(
                    localization: localization,
                    hiddenLanguageCodes: appSettings.HiddenLanguageCodes,
                    currentLanguageProvider: userSettings,
                    messager: messager);
            }
            catch (HiddenLanguageNotFound exception)
            {
                string message = PathHelper.FullPathOf(filePathProvider.AppSettingsFilePath) + " HiddenLanguageCodes has language " + 
                    exception.Language + " that is not found in localization file " + 
                    PathHelper.FullPathOf(filePathProvider.LocalizationPath) + ". " + REPAIR_ACTION;
                ExitAppAfterPopupClosed(message);
                return null;
            }

            TrackingModel trackingModel = new TrackingModel(userSettings);

            CsvConfigurationParser csvConfigurationParser = new CsvConfigurationParser();
            JsonConfigurationParser jsonConfigurationParser = new JsonConfigurationParser(
                filePathProvider.ConfigurationJsonSchema);
            List<FormatParser> parsers = new List<FormatParser>()
            { 
                new FormatParser(".csv", csvConfigurationParser), 
                new FormatParser(".json", jsonConfigurationParser)
            };
            ConfigurationParser configurationParser = new ConfigurationParser(parsers);

            Dictionary<string, INpcExport> npcExporters = new Dictionary<string, INpcExport>();
            NpcToCsv csv = new NpcToCsv();
            npcExporters[csv.FileExtensionWithoutDot] = csv;
            NpcToJson json = new NpcToJson(filePathProvider.NpcExportJsonSchema);
            npcExporters[json.FileExtensionWithoutDot] = json;

            CryptoRandom random = new CryptoRandom();

            NpcGeneratorModel npcGeneratorModel = new NpcGeneratorModel(
                userSettings, 
                appSettings, 
                messager, 
                fileIo, 
                configurationParser, 
                npcExporters, 
                localization, 
                random, 
                showErrorMessages: true,
                forceFailNpcGeneration: parameters.forceFailNpcGeneration);

            AboutModel aboutModel = new AboutModel(
                website: new Uri(appSettings.HomeWebsite), 
                donation: new Uri(appSettings.DonationWebsite),
                supportEmail: appSettings.SupportEmail);

            //Temporarily set navigation to null, as it requires a constructed ServiceCentre as a parameter.
            Models models = new Models(localizationModel, aboutModel, navigation: null, trackingModel, npcGeneratorModel);

            ServiceCentre serviceCentre = new ServiceCentre(
                profile: trackingProfile,
                appSettings: appSettings,
                messager: messager,
                userSettings: userSettings,
                filePathProvider: filePathProvider,
                fileIo: fileIo,
                localization: localization,
                configurationParser: configurationParser,
                npcExporters: npcExporters,
                random: random,
                models: models);

            //Set all the services that require access to the whole ServiceCentre.
            models.Navigation = new NavigationModel(serviceCentre);

            return serviceCentre;
        }

        private static AppParameters ReadAppParameters()
        {
            AppParameters parameters = new AppParameters
            {
                analyticsDryRun = false,
                forcedLanguageCode = null,
                forceFailNpcGeneration = false,
            };

            string[] commandLineArgs = Environment.GetCommandLineArgs();
            //Skip the first commandline argument, as it's always the path of the exe/dll.
            for(int i = 1; i < commandLineArgs.Length; i++)
            {
                switch(commandLineArgs[i])
                {
                    case "-analyticsDryRun":
                    {
                        parameters.analyticsDryRun = true;
                    }
                    break;

                    case "-language":
                    {
                        if(i + 1 >= commandLineArgs.Length)
                        {
                            throw new ArgumentOutOfRangeException("-language command line argument must be followed by a language code");
                        }
                        parameters.forcedLanguageCode = commandLineArgs[i + 1];
                        ++i;
                    }
                    break;

                    case "-forceFailNpc":
                    {
                        parameters.forceFailNpcGeneration = true;
                    }
                    break;

                    default:
                    {
                        throw new ArgumentException("Unknown command line argument: " + commandLineArgs[i]);
                    }
                }
            }
            return parameters;
        }

        private static void PickLanguage(ServiceCentre serviceCentre, AppParameters parameters)
        {
            if (!string.IsNullOrEmpty(parameters.forcedLanguageCode))
            {
                serviceCentre.Localization.CurrentLanguageCode = parameters.forcedLanguageCode;
            }
            else if (!string.IsNullOrEmpty(serviceCentre.UserSettings.LanguageCode))
            {
                serviceCentre.Localization.CurrentLanguageCode = serviceCentre.UserSettings.LanguageCode;
            }
            else
            {
                UseUsersLanguageOrReport(serviceCentre.Localization, serviceCentre.Messager);
            }
        }

        private static void UseUsersLanguageOrReport(Services.ILocalization localization, IMessager messager)
        {
            string userLanguageCode = Thread.CurrentThread.CurrentCulture.Name;
            bool userLanguageIsSupported = localization.IsLanguageCodeSupported(userLanguageCode);
            if(userLanguageIsSupported)
            {
                localization.CurrentLanguageCode = userLanguageCode;
            }
            else
            {
                messager.Send(Current, new Services.Message.UserLanguageNotSupported(userLanguageCode));
            }
        }

        private static TrackingProfile ReadTrackingProfile(FilePathProvider filePathProvider)
        {
            TrackingProfile trackingProfile = TrackingProfile.Load(filePathProvider.TrackingProfileFilePath);
            trackingProfile.Save();
            return trackingProfile;
        }

        public void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is LocalizedTextNotFoundException)
            {
                LocalizedTextNotFoundException localizationException = e.ExceptionObject as LocalizedTextNotFoundException;
                ShowMissingLocalizaedTextError(localizationException);
            }
        }

        private void ShowMissingLocalizaedTextError(LocalizedTextNotFoundException localizationException)
        {
            string language = m_serviceCentre.Localization.CurrentLanguageCode;
            string localizationFile = PathHelper.FullPathOf(m_serviceCentre.FilePathProvider.LocalizationPath);

            try
            {
                string errorMessage = m_serviceCentre.Localization.GetText(
                    "localized_text_not_found", localizationException.TextId, language, localizationFile);
                MessageBox.Show(errorMessage);
            }
            catch (LocalizedTextNotFoundException)
            {
                MessageBox.Show("Multiple text entries missing for language " + language + " in localization file " + localizationFile + ".");
            }
        }

        private static AppSettings LoadAppSettings(IFilePathProvider filePathProvider)
        {
            //Don't try to localize any exceptions. Localization requires AppSettings for default language, so circular reference.
            try
            {
                string text = File.ReadAllText(filePathProvider.AppSettingsFilePath);
                AppSettings appSettings = AppSettings.Create(text);
                return appSettings;
            }
            catch (FileNotFoundException exception)
            {
                ExitAppAfterPopupClosed("Required file " + PathHelper.FullPathOf(exception.FileName) + " not found. " + REPAIR_ACTION);
            }
            catch (DirectoryNotFoundException exception)
            {
                ExitAppAfterPopupClosed("Folder " + exception.Message + " with required files is not found. " + REPAIR_ACTION);
            }
            catch (PathTooLongException exception)
            {
                ExitAppAfterPopupClosed(exception.Message + " Reinstall the application in a shorter directory.");
            }
            catch (IOException exception)
            {
                ExitAppAfterPopupClosed(exception.Message);
            }
            catch (InvalidDefaultLanguageCodeException)
            {
                ExitAppAfterPopupClosed(PathHelper.FullPathOf(filePathProvider.AppSettingsFilePath) 
                    + " has missing or empty DefaultLanguageCode field. " + REPAIR_ACTION);
            }
            catch (InvalidHiddenLanguageCodeException)
            {
                ExitAppAfterPopupClosed(PathHelper.FullPathOf(filePathProvider.AppSettingsFilePath) + 
                    " has empty language code in HiddenLanguageCode field. " + REPAIR_ACTION);
            }
            catch (MalformedWebsiteException exception)
            {
                ExitAppAfterPopupClosed(PathHelper.FullPathOf(filePathProvider.AppSettingsFilePath) + 
                    " has malformed website " + exception.Uri + ". " + REPAIR_ACTION);
            }
            catch (MalformedEmailException exception)
            {
                ExitAppAfterPopupClosed(PathHelper.FullPathOf(filePathProvider.AppSettingsFilePath) + " has malformed email " + 
                    exception.Email + ". " + REPAIR_ACTION);
            }
            catch (InvalidProductKeyException exception)
            {
                string message = PathHelper.FullPathOf(filePathProvider.AppSettingsFilePath) + " has product key " + exception.ProductKeyName + 
                    " with invalid value " + exception.ProductKeyValue + ". " + REPAIR_ACTION;
                ExitAppAfterPopupClosed(message);
            }
            catch (InvalidDefaultConfigurationRelativePath exception)
            {
                string message = PathHelper.FullPathOf(filePathProvider.AppSettingsFilePath) + " has an invalid configuration path " +
                    exception.Path + ". " + REPAIR_ACTION;
                ExitAppAfterPopupClosed(message);
            }
            catch (JsonReaderException exception)
            {
                string message = PathHelper.FullPathOf(filePathProvider.AppSettingsFilePath) + " has error: " + exception.Message + " " + REPAIR_ACTION;
                ExitAppAfterPopupClosed(message);
            }

            return null;
        }

        private static void ExitAppAfterPopupClosed(string message)
        {
            MessageBox.Show(message);
            Current.Shutdown();
        }

        private readonly ServiceCentre m_serviceCentre;
#pragma warning disable IDE0052 // Remove unread private members. 
        //This warning is stupid. The subscriber object only need to be created to be useful. It does not need to be read.
        private readonly GoogleAnalytics m_googleAnalytics;
#pragma warning restore IDE0052 // Remove unread private members
    }
}
