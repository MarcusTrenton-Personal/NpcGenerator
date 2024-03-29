﻿/*Copyright(C) 2022 Marcus Trenton, marcus.trenton@gmail.com

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
using Services;
using Services.Message;
using System.Collections.Generic;

namespace NpcGenerator
{
    //This class is an implementation of the toolbox pattern.
    //Instead of a singleton, all the wanna-be global state is passed in a single, giant container: the toolbox.
    public class ServiceCentre
    {
        public ServiceCentre(
            in IAppSettings appSettings,
            in IFilePathProvider filePathProvider,
            in ILocalFileIO fileIo,
            in ILocalization localization,
            in IMessager messager, 
            in ITrackingProfile profile,
            in IUserSettings userSettings,
            in IConfigurationParser configurationParser,
            in Dictionary<string, INpcExport> npcExporters,
            in IRandom random,
            in Models models
            )
        {
            Profile = profile;
            AppSettings = appSettings;
            Messager = messager;
            UserSettings = userSettings;
            FilePathProvider = filePathProvider;
            FileIO = fileIo;
            Localization = localization;
            ConfigurationParser = configurationParser;
            NpcExporters = npcExporters;
            Random = random;
            Models = models;
        }

        public IAppSettings AppSettings { get; }
        public ILocalFileIO FileIO { get; }
        public IFilePathProvider FilePathProvider { get; }
        public ILocalization Localization { get; }
        public IMessager Messager { get; }
        public ITrackingProfile Profile { get; }
        public IUserSettings UserSettings { get; }
        public IConfigurationParser ConfigurationParser { get; }
        public Dictionary<string, INpcExport> NpcExporters { get; }
        public IRandom Random { get; }
        public IModels Models { get; }
    }
}
