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

using Microsoft.Win32;
using System;
using System.IO;
using System.Security;

namespace Services
{
    public static class OSHelper
    {
        //Taken from https://stackoverflow.com/questions/577634/how-to-get-the-friendly-os-version-name
        public static string HKLM_GetString(string path, string key)
        {
            try
            {
                RegistryKey rk = Registry.LocalMachine.OpenSubKey(path);
                if (rk == null)
                {
                    return string.Empty;
                }
                return (string)rk.GetValue(key);
            }
            catch(ArgumentException)
            { 
                return string.Empty; 
            }
            catch (SecurityException)
            {
                return string.Empty;
            }
            catch (ObjectDisposedException)
            {
                return string.Empty;
            }
            catch (IOException)
            {
                return string.Empty;
            }
            catch (UnauthorizedAccessException)
            {
                return string.Empty;
            }
        }
    }
}