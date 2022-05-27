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

using Newtonsoft.Json;
using System.IO;

namespace NpcGenerator
{
    public class GoogleAnalyticsSettings : IGoogleAnalyticsSettings
    {
        new public string MeasurementIdDev { get; set; }
        new public string MeasurementIdProd { get; set; }
        new public string AdditionalIdDev { get; set; }
        new public string AdditionalIdProd { get; set; }
    }

    public class AppSettings : IAppSettings
    {
        public IGoogleAnalyticsSettings GoogleAnalytics { get; set; } = new GoogleAnalyticsSettings();

        public int EncryptionKey { get; set; }

        public static AppSettings Load(string path)
        {
            string text = File.ReadAllText(path);
            AppSettings settings = JsonConvert.DeserializeObject<AppSettings>(text);
            return settings;
        }
    }
}