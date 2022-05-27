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

using System.Windows;

namespace NpcGenerator
{
    /// <summary>
    /// Interaction logic for App.xaml. 
    /// Singleton holder since there is no other way to pass data to the other window classes.
    /// </summary>
    public partial class App : Application
    {
        App()
        {
            FilePathProvider filePathProvider = new FilePathProvider();
            FileIO fileIO = new FileIO(filePathProvider);

            string appSettingsPath = filePathProvider.AppSettingsFilePath;
            AppSettings appSettings = AppSettings.Load(appSettingsPath);

            TrackingProfile trackingProfile = ReadTrackingProfile(filePathProvider);

            Message.Messager messager = new Message.Messager();

            UserSettings userSettings = ReadUserSettings(filePathProvider);

            m_serviceCenter = new ServiceCenter(
                profile: trackingProfile, 
                appSettings: appSettings, 
                messager: messager,
                userSettings: userSettings,
                filePathProvider: filePathProvider,
                fileIO: fileIO);

            //Only pass the part of the ServiceCenter that are needed, rather than the entire class.
            //Coupling should always be deliberate and minimized.
            m_googleAnalytics = new GoogleAnalytics(
                 appSettings: m_serviceCenter.AppSettings,
                 trackingProfile: m_serviceCenter.Profile,
                 messager: m_serviceCenter.Messager);

            m_serviceCenter.Messager.Send(sender: this, message: new Message.Login());

            Current.MainWindow = new MainWindow(m_serviceCenter);
            Current.MainWindow.Show();
        }

        private TrackingProfile ReadTrackingProfile(FilePathProvider filePathProvider)
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

        private UserSettings ReadUserSettings(FilePathProvider filePathProvider)
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
