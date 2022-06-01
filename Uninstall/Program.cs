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

using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using NpcGenerator;

namespace Uninstall
{
    class Program
    {
        static void Main(string[] _)
        {
            //Delete the AppData folder, which includes the UserSettings and TrackingProfile.
            FilePathProvider filePathProvider = new FilePathProvider();
            if(Directory.Exists(filePathProvider.AppDataFolderPath))
            {
                Directory.Delete(filePathProvider.AppDataFolderPath, recursive: true);
            }

            string exeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string exeFolder = Path.GetDirectoryName(exeFilePath);

            //Delete this folder
            //Modified from https://www.codeproject.com/Articles/31454/How-To-Make-Your-Application-Delete-Itself-Immedia
            ProcessStartInfo Info = new ProcessStartInfo
            {
                Arguments = "/C choice /C Y /N /D Y /T 3 & del /s /q *.* " + exeFolder + "& START CMD /C \"ECHO Uninstall complete && PAUSE\"",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = "cmd.exe"
            };
            Process.Start(Info);
        }
    }
}
