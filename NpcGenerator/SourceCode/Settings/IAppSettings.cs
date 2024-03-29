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

using System.Collections.ObjectModel;

namespace NpcGenerator
{
    public interface IGoogleAnalyticsSettings
    {
        public string MeasurementIdDev { get; }
        public string MeasurementIdProd { get; }
        public string AdditionalIdDev { get; }
        public string AdditionalIdProd { get; }
        public void Validate();
    }

    public interface IAppSettings
    {
        public IGoogleAnalyticsSettings GoogleAnalytics { get; }

        public int EncryptionKey { get; }

        public string DefaultLanguageCode { get; }

        public ReadOnlyCollection<string> HiddenLanguageCodes { get; }

        public string HomeWebsite { get; }

        public string DonationWebsite { get; }

        public string SupportEmail { get; }

        public string DefaultConfigurationRelativePath { get; }
    }
}