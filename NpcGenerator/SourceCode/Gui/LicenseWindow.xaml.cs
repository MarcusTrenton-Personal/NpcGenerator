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

using CoupledServices;
using Services;
using Services.Message;
using System;
using System.IO;
using System.Windows;
using WpfServices;

namespace NpcGenerator
{
    public partial class LicenseWindow : Window
    {
        public LicenseWindow(
            IMessager messager, 
            IFilePathProvider filePathProvider, 
            ILocalizationModel localizationModel)
        {
            if (filePathProvider == null)
            {
                throw new ArgumentNullException(nameof(filePathProvider));
            }

            m_localizationModel = localizationModel ?? throw new ArgumentNullException(nameof(localizationModel));

            InitializeComponent();

            try
            {
                flowViewer.Document = GuiHelper.ReadRtfText(filePathProvider.LicensePath);
            }
            catch (IOException exception)
            {
                string message = m_localizationModel.Localization.GetText("exception_maybe_file_deleted", exception.Message);
                MessageBox.Show(message);
            }

            messager?.Send(sender: this, message: new Services.Message.PageView("License"));
        }

        public ILocalizationModel LocalizationModel
        {
            get
            {
                return m_localizationModel;
            }
        }

        private readonly ILocalizationModel m_localizationModel;
    }
}
