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

using Newtonsoft.Json;
using Services;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace NpcGenerator
{
    public class TrackingProfile : ITrackingProfile
    {
        private TrackingProfile()
        {
            ClientId = Guid.NewGuid();

            Update();
        }

        //Adapted from https://stackoverflow.com/questions/577634/how-to-get-the-friendly-os-version-name
        private static string GetOsName()
        {
            const string WINDOWS_PATH = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";
            string productName = OSHelper.HKLM_GetString(WINDOWS_PATH, "ProductName");
            string csdVersion = OSHelper.HKLM_GetString(WINDOWS_PATH, "CSDVersion");
            if (!string.IsNullOrEmpty(productName))
            {
                return (productName.StartsWith("Microsoft") ? "" : "Microsoft ") + productName +
                    (!string.IsNullOrEmpty(csdVersion) ? " " + csdVersion : "");
            }
            return string.Empty;
        }

        private static string GetSystemLanguage()
        {
            return CultureInfo.CurrentCulture.Name;
        }

        private static string GetAppVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void Update()
        {
            SystemLanguage = GetSystemLanguage();

            AppVersion = GetAppVersion();

            OsVersion = GetOsName();
        }

        //Excess surveillance and tracking is a problem with modern technology. 
        //Each bit of tracked data must be for a user-friendly purpose.

        //How often does the same user use this software? Using but once indicates a usability problem.
        public Guid ClientId { get; set; }

        //Which languages should be supported?
        public string SystemLanguage { get; set; }

        //Are the deployed versions reaching users?
        public string AppVersion { get; set; }

        //What OS versions need to be supported?
        public string OsVersion { get; set; }

        public void Save()
        {
            string directory = Path.GetDirectoryName(m_savePath);
            Directory.CreateDirectory(directory);

            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            using FileStream fs = File.Create(m_savePath);
            byte[] info = new UTF8Encoding(true).GetBytes(json);
            fs.Write(info, 0, info.Length);
        }

        public static TrackingProfile Load(string path)
        {
            try
            {
                string text = File.ReadAllText(path);
                TrackingProfile profile = JsonConvert.DeserializeObject<TrackingProfile>(text);
                profile.Validate();
                profile.m_savePath = path;
                profile.Update();
                return profile;
            }
            catch (IOException)
            {
                return Default(path);
            }
            catch (JsonSerializationException)
            {
                return Default(path);
            }
            catch (JsonReaderException)
            {
                return Default(path);
            }
        }

        private static TrackingProfile Default(string savePath)
        {
            TrackingProfile profile = new TrackingProfile
            {
                m_savePath = savePath
            };
            profile.Update();
            return profile;
        }

        private void Validate()
        {
            ValidateClientId();
            ValidateSystemLanguage();
            ValidateAppVersion();
            ValidateOsVersion();
        }

        private static void ValidateClientId()
        {
            //Guid automatically validates itself. Nothing to do here.
        }

        private void ValidateSystemLanguage()
        {
            CultureInfo[] allCultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            CultureInfo culture = CollectionUtil.Find(allCultures, culture => culture.ToString() == SystemLanguage);
            if (culture is null)
            {
                SystemLanguage = GetSystemLanguage();
            }
        }

        private void ValidateAppVersion()
        {
            string pattern = @"^(\d+\.)(\d+\.)(\d+\.)(\*|\d+)?$";
            bool isValid = Regex.Match(AppVersion, pattern).Success;
            if (!isValid)
            {
                AppVersion = GetAppVersion();
            }
        }

        private void ValidateOsVersion()
        {
            bool isValid = !string.IsNullOrWhiteSpace(OsVersion);
            if (!isValid)
            {
                OsVersion = GetOsName();
            }
        }

        private string m_savePath;
    }
}
