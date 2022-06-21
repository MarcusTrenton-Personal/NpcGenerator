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
using Services;
using Services.Message;

namespace NpcGenerator
{
    //This class is an implementation of the toolbox pattern.
    //Instead of a singleton, all the wanna-be global state is passed in a single, giant container: the toolbox.
    public class ServiceCenter
    {
        public ServiceCenter(
            IAppSettings appSettings,
            IFilePathProvider filePathProvider,
            ILocalFileIO fileIo,
            ILocalization localization,
            IMessager messager, 
            ITrackingProfile profile,
            IUserSettings userSettings,
            Models models
            )
        {
            Profile = profile;
            AppSettings = appSettings;
            Messager = messager;
            UserSettings = userSettings;
            FilePathProvider = filePathProvider;
            FileIO = fileIo;
            Localization = localization;
            Models = models;
        }

        public IAppSettings AppSettings { get; }
        public ILocalFileIO FileIO { get; }
        public IFilePathProvider FilePathProvider { get; }
        public ILocalization Localization { get; }
        public IMessager Messager { get; }
        public ITrackingProfile Profile { get; }
        public IUserSettings UserSettings { get; }
        public IModels Models { get; }
    }
}
