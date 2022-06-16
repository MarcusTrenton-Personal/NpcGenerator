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
using Services.Message;
using System;
using System.Globalization;
using System.Threading;
using System.Windows;

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
        }

        public App()
        {
            m_serviceCenter = CreateServices();
            AppParameters parameters = ReadAppParameters();

            m_googleAnalytics = new GoogleAnalytics(
                 appSettings: m_serviceCenter.AppSettings,
                 trackingProfile: m_serviceCenter.Profile,
                 messager: m_serviceCenter.Messager,
                 userSettings: m_serviceCenter.UserSettings,
                 dryRunValidation: parameters.analyticsDryRun);

            m_serviceCenter.Messager.Send(sender: this, message: new Message.Login());

            PickLanguage(m_serviceCenter, parameters);

            Current.MainWindow = new MainWindow(m_serviceCenter);
            Current.MainWindow.Show();
        }

        private static ServiceCenter CreateServices()
        {
            FilePathProvider filePathProvider = new FilePathProvider();
            LocalFileIO fileIO = new LocalFileIO(filePathProvider);
            AppSettings appSettings = AppSettings.Load(filePathProvider.AppSettingsFilePath);
            Services.Localization localization = new Services.Localization(filePathProvider.LocalizationPath, appSettings.DefaultLanguageCode);
            Messager messager = new Messager();
            TrackingProfile trackingProfile = ReadTrackingProfile(filePathProvider);
            UserSettings userSettings = ReadUserSettings(filePathProvider);

            return new ServiceCenter(
                profile: trackingProfile,
                appSettings: appSettings,
                messager: messager,
                userSettings: userSettings,
                filePathProvider: filePathProvider,
                fileIO: fileIO,
                localization: localization);
        }

        private static AppParameters ReadAppParameters()
        {
            AppParameters parameters = new AppParameters
            {
                analyticsDryRun = false,
                forcedLanguageCode = null
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

                    default:
                    {
                        throw new ArgumentException("Unknown command line argument: " + commandLineArgs[i]);
                    }
                }
            }
            return parameters;
        }

        private static void PickLanguage(ServiceCenter serviceCenter, AppParameters parameters)
        {
            if (!string.IsNullOrEmpty(parameters.forcedLanguageCode))
            {
                serviceCenter.Localization.CurrentLanguageCode = parameters.forcedLanguageCode;
            }
            else if (!string.IsNullOrEmpty(serviceCenter.UserSettings.LanguageCode))
            {
                serviceCenter.Localization.CurrentLanguageCode = serviceCenter.UserSettings.LanguageCode;
            }
            else
            {
                UseUsersLanguageOrReport(serviceCenter.Localization, serviceCenter.Messager);
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
                messager.Send(Current, new Message.UserLanguageNotSupported(userLanguageCode));
            }
        }

        private static TrackingProfile ReadTrackingProfile(FilePathProvider filePathProvider)
        {
            string profilePath = filePathProvider.TrackingProfileFilePath;
            TrackingProfile trackingProfile = TrackingProfile.Load(profilePath);
            if (trackingProfile == null)
            {
                trackingProfile = new TrackingProfile();
            }
            trackingProfile.Save(profilePath);
            return trackingProfile;
        }

        private static UserSettings ReadUserSettings(FilePathProvider filePathProvider)
        {
            string userSettingsPath = filePathProvider.UserSettingsFilePath;
            UserSettings userSettings = UserSettings.Load(userSettingsPath);
            if (userSettings == null)
            {
                userSettings = new UserSettings();
            }
            return userSettings;
        }

        
        private readonly ServiceCenter m_serviceCenter;
#pragma warning disable IDE0052 // Remove unread private members. 
        //This warning is stupid. The subscriber object only need to be created to be useful. It does not need to be read.
        private readonly GoogleAnalytics m_googleAnalytics;
#pragma warning restore IDE0052 // Remove unread private members
    }
}
