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

using Services;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Input;

namespace WpfServices
{
    public class AboutModel : BaseModel, IAboutModel
    {
        public AboutModel(Uri website, Uri donation)
        {
            Website = website;
            Donation = donation;
        }

        public string Version 
        { 
            get
            {
                Assembly assembly = Assembly.GetEntryAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                return fvi.FileVersion;
            }
        }

        public Uri Website { get; }

        public Uri Donation { get; }

        public ICommand OpenBrowserToUri 
        { 
            get
            {
                return m_openBrowserCommand ??= new CommandHandler(
                    (object parameter) => ExecuteOpenBrowserToUri(parameter),
                    (object parameter) => CanExecuteOpenBrowserToUri(parameter));
            }
        }

        private static bool CanExecuteOpenBrowserToUri(object parameter)
        {
            Uri uri = parameter as Uri;
            return uri != null;
        }

        private void ExecuteOpenBrowserToUri(object parameter)
        {
            Uri uri = parameter as Uri;
            UriHelper.OpenUri(uri);
        }

        private ICommand m_openBrowserCommand;
    }
}
