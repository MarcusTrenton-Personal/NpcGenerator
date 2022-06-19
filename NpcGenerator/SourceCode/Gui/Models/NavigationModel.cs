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

using System.Windows;
using System.Windows.Input;

namespace NpcGenerator
{
    //Based on code from https://stackoverflow.com/questions/32064308/pass-command-parameter-to-method-in-viewmodel-in-wpf
    public class NavigationModel : INavigationModel
    {
        //Navigating to any place in the app requires potentially any service, so store them all.
        public NavigationModel(ServiceCenter serviceCenter)
        {
            m_serviceCenter = serviceCenter;
        }

        public ICommand GoToPrivacyPolicy 
        { 
            get
            {
                return m_goToPrivacyPolicyCommand ??= new CommandHandler(
                    (object parameter) => ExecuteGoToPrivacyPolicy(parameter), 
                    (object parameter) => CanExecuteGoToPrivacyPolicy(parameter));
            }
        }

        private static bool CanExecuteGoToPrivacyPolicy(object _)
        {
            return true;
        }

        private void ExecuteGoToPrivacyPolicy(object parameter)
        {
            //Lazily create the data as it's unlikely that this button will be clicked. 
            //It's almost unheard of that anyone would click it twice.
            PrivacyPolicyWindow privacyWindow = new PrivacyPolicyWindow(
                messager: m_serviceCenter.Messager,
                filePathProvider: m_serviceCenter.FilePathProvider,
                localizationModel: m_serviceCenter.Models.Localization)
            {
                Owner = parameter as Window
            };
            privacyWindow.Show();
        }

        public ICommand GoToLicense
        {
            get
            {
                return m_goToLicenseCommand ??= new CommandHandler(
                    (object parameter) => ExecuteGoToLicense(parameter),
                    (object parameter) => CanExecuteGoToLicense(parameter));
            }
        }

        private static bool CanExecuteGoToLicense(object _)
        {
            return true;
        }

        private void ExecuteGoToLicense(object parameter)
        {
            //Lazily create the data as it's unlikely that this button will be clicked. 
            //It's almost unheard of that anyone would click it twice.
            LicenseWindow licenseWindow = new LicenseWindow(
                messager: m_serviceCenter.Messager,
                filePathProvider: m_serviceCenter.FilePathProvider,
                localizationModel: m_serviceCenter.Models.Localization)
            {
                Owner = parameter as Window
            };
            licenseWindow.Show();
        }

        private readonly ServiceCenter m_serviceCenter;
        private ICommand m_goToPrivacyPolicyCommand;
        private ICommand m_goToLicenseCommand;
    }
}
