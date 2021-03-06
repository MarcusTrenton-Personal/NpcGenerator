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
using Services;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace NpcGenerator
{
    public class TrackingProfile : ITrackingProfile
    {
        public TrackingProfile()
        {
            ClientId = Guid.NewGuid();

            Update();
        }

        //Adapted from https://stackoverflow.com/questions/577634/how-to-get-the-friendly-os-version-name
        private static string GetOSName()
        {
            string productName = OSHelper.HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
            string csdVersion = OSHelper.HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CSDVersion");
            if (!string.IsNullOrEmpty(productName))
            {
                return (productName.StartsWith("Microsoft") ? "" : "Microsoft ") + productName +
                    (!string.IsNullOrEmpty(csdVersion) ? " " + csdVersion : "");
            }
            return string.Empty;
        }

        private void Update()
        {
            SystemLanguage = CultureInfo.CurrentCulture.Name;

            AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            OSVersion = GetOSName();
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
        public string OSVersion { get; set; }

        public void Save(string path)
        {
            string directory = Path.GetDirectoryName(path);
            Directory.CreateDirectory(directory);

            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            using FileStream fs = File.Create(path);
            byte[] info = new UTF8Encoding(true).GetBytes(json);
            fs.Write(info, 0, info.Length);
        }

        public static TrackingProfile Load(string path)
        {
            bool fileExists = File.Exists(path);
            if (fileExists)
            {
                string text = File.ReadAllText(path);
                TrackingProfile profile = JsonConvert.DeserializeObject<TrackingProfile>(text);
                profile.Update();
                return profile;
            }
            return null;
        }
    }
}
