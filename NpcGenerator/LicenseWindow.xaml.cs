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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NpcGenerator
{
    public partial class LicenseWindow : Window
    {
        public LicenseWindow()
        {
            InitializeComponent();

            PopulateText();
        }

        private void PopulateText()
        {
            try
            {
                string path = FilePathHelper.LicensePath();
                using (FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    FlowDocument flowDocument = new FlowDocument();
                    TextRange textRange = new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd);
                    textRange.Load(fileStream, DataFormats.Rtf);
                    FlowViewer.Document = flowDocument;
                }
            }
            catch(IOException exception)
            {
                MessageBox.Show(exception.Message + " Was that file deleted?");
            }
        }
    }
}
