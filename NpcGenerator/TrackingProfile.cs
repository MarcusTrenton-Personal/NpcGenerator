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
along with this program.If not, see<https://www.gnu.org/licenses/>.*/

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NpcGenerator
{
    public class TrackingProfile
    {
        public TrackingProfile()
        {
            ClientId = Guid.NewGuid();
        }

        public Guid ClientId { get; set; }

        public void Save(string path)
        {
            string directory = Path.GetDirectoryName(path);
            Directory.CreateDirectory(directory);

            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            using (FileStream fs = File.Create(path))
            {
                byte[] info = new UTF8Encoding(true).GetBytes(json);
                fs.Write(info, 0, info.Length);
            }
        }

        public static TrackingProfile Load(string path)
        {
            bool fileExists = File.Exists(path);
            if (fileExists)
            {
                string text = File.ReadAllText(path);
                TrackingProfile profile = JsonConvert.DeserializeObject<TrackingProfile>(text);
                return profile;
            }
            return null;
        }
    }
}
