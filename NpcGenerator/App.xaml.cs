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
            string appSettingsPath = FilePathHelper.AppSettingsFilePath();
            AppSettings appSettings = AppSettings.Load(appSettingsPath);

            string profilePath = FilePathHelper.TrackingProfileFilePath();
            TrackingProfile trackingProfile = TrackingProfile.Load(profilePath);
            if (trackingProfile == null)
            {
                trackingProfile = new TrackingProfile();
            }
            trackingProfile.Save(profilePath);

            Message.Messager messager = new Message.Messager();

            ServiceCenter = new ServiceCenter(profile: trackingProfile, appSettings: appSettings, messager: messager);

            //Only pass the part of the ServiceCenter that are needed, rather than the entire class.
            //Coupling should always be deliberate and minimized.
            m_googleAnalytics = new GoogleAnalytics(
                 appSettings: ServiceCenter.AppSettings,
                 trackingProfile: ServiceCenter.Profile,
                 messager: ServiceCenter.Messager);

            ServiceCenter.Messager.Send(sender: this, message: new Message.Login());
        }

        public static ServiceCenter ServiceCenter { get; private set; }
        private readonly GoogleAnalytics m_googleAnalytics;
    }
}
