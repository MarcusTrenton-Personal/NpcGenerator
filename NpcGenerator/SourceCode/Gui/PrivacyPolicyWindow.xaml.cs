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

using CoupledServices;
using Services;
using Services.Message;
using System;
using System.IO;
using System.Windows;

namespace NpcGenerator
{
    /// <summary>
    /// Interaction logic for PrivacyPolicyWindow.xaml
    /// </summary>
    public partial class PrivacyPolicyWindow : Window, ILocalizationProvider
    {
        public PrivacyPolicyWindow(IMessager messager, IFilePathProvider filePathProvider, ILocalization localization)
        {
            if (filePathProvider == null)
            {
                throw new ArgumentNullException(nameof(filePathProvider));
            }
            if (localization == null)
            {
                throw new ArgumentNullException(nameof(localization));
            }

            m_localization = localization;

            InitializeComponent();

            try
            {
                flowViewer.Document = GuiHelper.ReadRtfText(filePathProvider.PrivacyPolicyPath);
            }
            catch (IOException exception)
            {
                string message = localization.GetText("exception_maybe_file_deleted", exception.Message);
                MessageBox.Show(message);
            }

            messager?.Send(sender: this, message: new Message.PageView("Privacy Policy"));
        }

        public ILocalization Localization
        {
            get
            {
                return m_localization;
            }
        }

        private readonly ILocalization m_localization;
    }
}