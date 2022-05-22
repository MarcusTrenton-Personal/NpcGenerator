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

namespace NpcGenerator
{
    //This class is an implementation of the toolbox pattern.
    //Instead of a singleton, all the wanna-be global state is passed in a single, giant container: the toolbox.
    public class ServiceCenter
    {
        public ServiceCenter()
        {
            string appSettingsPath = FilePathHelper.AppSettingsFilePath();
            ApplicationSettings = AppSettings.Load(appSettingsPath);

            string profilePath = FilePathHelper.TrackingProfileFilePath();
            TrackingProfile trackingProfile = TrackingProfile.Load(profilePath);
            if (trackingProfile == null)
            {
                trackingProfile = new TrackingProfile();
            }
            trackingProfile.Save(profilePath);
            Profile = trackingProfile;

            MessageCenter = new MessageCenter();
        }

        public ITrackingProfile Profile { get; private set; }
        public IAppSettings ApplicationSettings { get; private set; }
        public IMessageCenter MessageCenter { get; private set; }
    }
}
