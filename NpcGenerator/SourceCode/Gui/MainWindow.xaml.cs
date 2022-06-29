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

using Services;
using System.Windows;
using System.Windows.Navigation;
using WpfServices;

namespace NpcGenerator
{
    public partial class MainWindow : Window
    {   
        public MainWindow(ServiceCentre serviceCentre)
        {
            m_serviceCentre = serviceCentre;

            InitializeComponent();

            serviceCentre?.Messager.Send(sender: this, message: new Services.Message.PageView("Main Window"));
        }

        public ILocalizationModel LocalizationModel
        {
            get
            {
                return m_serviceCentre.Models.Localization;
            }
        }

        public IAboutModel AboutModel
        {
            get
            {
                return m_serviceCentre.Models.About;
            }
        }

        public INavigationModel NavigationModel
        {
            get
            {
                return m_serviceCentre.Models.Navigation;
            }
        }

        public ITrackingModel TrackingModel
        {
            get
            {
                return m_serviceCentre.Models.Tracking;
            }
        }

        public INpcGeneratorModel NpcGeneratorModel
        {
            get
            {
                return m_serviceCentre.Models.NpcGenerator;
            }
        }

        private void OpenBrowserToUri(object sender, RequestNavigateEventArgs e)
        {
            e.Handled = UriHelper.OpenUri(e.Uri);
        }

        private readonly ServiceCentre m_serviceCentre;
    }
}
