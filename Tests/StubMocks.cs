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
using NpcGenerator;
using Services;
using Services.Message;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace Tests
{
    internal class StubTrackingProfile : ITrackingProfile
    {
        public Guid ClientId { get; set; } = Guid.Empty;
        public string SystemLanguage { get; set; } = null;
        public string AppVersion { get; set; } = null;
        public string OSVersion { get; set; } = null;
    }

    internal class StubGoogleAnalyticsSettings : IGoogleAnalyticsSettings
    {
        public string MeasurementIdDev { get; set; } = null;
        public string MeasurementIdProd { get; set; } = null;
        public string AdditionalIdDev { get; set; } = null;
        public string AdditionalIdProd { get; set; } = null;
    }

    internal class StubAppSettings : IAppSettings
    {
        public IGoogleAnalyticsSettings GoogleAnalytics { get; set; } = new StubGoogleAnalyticsSettings();
        public int EncryptionKey { get; set; } = 0;
        public string DefaultLanguageCode { get; set; } = null;
        public ReadOnlyCollection<string> HiddenLanguageCodes { get; set; } = null;
    }

    internal class StubMessager : IMessager
    {
        public void Send<T>(object sender, T message) { }
        public void Subscribe<T>(IChannel<T>.Callback callback) { }
        public void Unsubscribe<T>(IChannel<T>.Callback callback) { }
    }

    internal class StubUserSettings : IUserSettings
    {
        public bool AnalyticsConsent { get; set; } = true;
        public string ConfigurationPath { get; set; } = null;
        public int NpcQuantity { get; set; } = 1;
        public string LanguageCode { get; set; } = null;
    }

    internal class StubFilePathProvider : IFilePathProvider
    {
        public string AppDataFolderPath { get; set; } = null;
        public string AppSettingsFilePath { get; set; } = null;
        public string LicensePath { get; set; } = null;
        public string LocalizationPath { get; } = null;
        public string PrivacyPolicyPath { get; set; } = null;
        public string TrackingProfileFilePath { get; set; } = null;
        public string UserSettingsFilePath { get; set; } = null;
    }

    internal class StubLocalization : ILocalization
    {
        public string[] SupportedLanguageCodes { get; set; } = null;
        public string CurrentLanguageCode { get; set; } = null;

        public bool IsLanguageCodeSupported(string languageCode)
        {
            return false;
        }

        public string GetText(string textId, params object[] formatParameters)
        {
            return textId;
        }
    }

    internal class StubLocalFileIo : ILocalFileIO
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

    internal class StubLocalizationModel : ILocalizationModel
    {
        public ILocalization Localization { get; set; } = null;
        public IEnumerable<string> SelectableLanguages { get; } = null;
        public string CurrentLanguage { get; set; } = null;
    }

    internal class StubAboutModel : IAboutModel
    {
        public string Version { get; set; } = null;
    }

    internal class StubNavigationModel : INavigationModel
    {
        public ICommand GoToPrivacyPolicy { get; } = null;
        public ICommand GoToLicense { get; } = null;
    }

    internal class StubTrackingModel : ITrackingModel
    {
        public bool TrackingConsent { get; set; } = false;
    }

    internal class StubNpcGeneratorModel : INpcGeneratorModel
    {
        public ICommand ChooseConfiguration { get; } = null;
        public string ConfigurationPath { get; set; } = null;
        public int NpcQuantity { get; set; } = 0;
        public ICommand GenerateNpcs { get; } = null;
        public DataTable ResultNpcs { get; set; }
        public ICommand SaveNpcs { get; } = null;
    }
}
