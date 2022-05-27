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

using Microsoft.Win32;
using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

[assembly: CLSCompliant(true)]
namespace NpcGenerator
{
    public partial class MainWindow : Window
    {   
        //Normally, only the needed services would be passed, 
        //but since this window can spawn other windows that is too restricting.
        public MainWindow(ServiceCenter serviceCenter)
        {
            m_serviceCenter = serviceCenter;
            m_userSettingsPath = m_serviceCenter.FilePathProvider.UserSettingsFilePath; //Must come before InitializeComponent()

            InitializeComponent();

            SetVersionText(); 

            configurationPathText.Content = m_serviceCenter.UserSettings.ConfigurationPath;
            npcQuantityText.Text = m_serviceCenter.UserSettings.NpcQuantity.ToString(CultureInfo.InvariantCulture);
            UpdateGenerateButtonEnabled();

            serviceCenter?.Messager.Send(sender: this, message: new Message.PageView("Main Window"));
        }

        private void SetVersionText()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            VersionText.Text = fvi.FileVersion;
        }

        private void UpdateGenerateButtonEnabled()
        {
            if(generateButton != null && configurationPathText != null && npcQuantityText != null)
            {
                bool isFilePicked = File.Exists(configurationPathText.Content.ToString());
                bool isNpcQuantityPositiveInteger = NumberHelper.TryParsePositiveNumber(npcQuantityText.Text, out _);

                generateButton.IsEnabled = isFilePicked && isNpcQuantityPositiveInteger;
            }
        }

        private void ChooseConfiguration(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text CSV (*.csv)|*.csv|All files (*.*)|*.*"
            };
            bool? filePicked = openFileDialog.ShowDialog();
            if(filePicked == true)
            {

                configurationPathText.Content = openFileDialog.FileName;
                UpdateGenerateButtonEnabled();

                m_serviceCenter.UserSettings.ConfigurationPath = openFileDialog.FileName;
                m_serviceCenter.UserSettings.Save(m_userSettingsPath);

                m_serviceCenter.Messager.Send(sender: this, message: new Message.SelectConfiguration());
            }
        }

        private void NpcQuantityInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !NumberHelper.TryParseDigit(e.Text, out _);
        }

        private void NpcQuantityInputChanged(object sender, TextChangedEventArgs args)
        {
            bool isInt = int.TryParse(npcQuantityText.Text, out _);
            UpdateGenerateButtonEnabled();

            if(m_serviceCenter.UserSettings != null && isInt)
            {
                m_serviceCenter.UserSettings.NpcQuantity = int.Parse(npcQuantityText.Text, CultureInfo.InvariantCulture);
                m_serviceCenter.UserSettings.Save(m_userSettingsPath);
            }
        }

        private void NpcQuantityPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!NumberHelper.TryParsePositiveNumber(text, out _))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }

            UpdateGenerateButtonEnabled();
        }

        private void OpenBrowserToUri(object sender, RequestNavigateEventArgs e)
        {
            e.Handled = UriHelper.OpenUri(e.Uri);
        }

        private bool ValidateInput(out int npcQuantity, ref string configurationPath)
        {
            npcQuantity = 0;

            configurationPath = configurationPathText.Content.ToString();
            bool configurationFileExists = File.Exists(configurationPath);
            if (!configurationFileExists)
            {
                MessageBox.Show("Configuration path is not valid");
                return false;
            }

            bool isNpcQuantityValid = NumberHelper.TryParsePositiveNumber(npcQuantityText.Text, out npcQuantity);
            if (!isNpcQuantityValid)
            {
                MessageBox.Show("NPC Quantity must be at least 1");
                return false;
            }

            return true;
        }

        private void GenerateNpcs(object sender, RoutedEventArgs e)
        {
            string configurationPath = "";
            bool isValid = ValidateInput(out int npcQuantity, ref configurationPath);
            if(!isValid)
            {
                return;
            }

            try
            {
                string cachedConfigurationPath = m_serviceCenter.FileIO.CacheFile(configurationPath);
                TraitSchema traitSchema = ConfigurationFile.Parse(cachedConfigurationPath);
                m_npcGroup = new NpcGroup(traitSchema, npcQuantity);

                System.Data.DataTable table = new DataTable("Npc Table");
                for(int i = 0; i < m_npcGroup.TraitGroupCount; ++i)
                {
                    table.Columns.Add(m_npcGroup.GetTraitGroupNameAtIndex(i));
                } 
                for(int i = 0; i < m_npcGroup.NpcCount; ++i)
                {
                    table.Rows.Add(m_npcGroup.GetNpcAtIndex(i).GetTraits());
                }
                generatedNpcTable.DataContext = table;
                saveNpcsButton.IsEnabled = true;

                m_serviceCenter.Messager.Send(sender: this, message: new Message.GenerateNpcs(npcQuantity));
            }
            catch(IOException exception)
            {
                MessageBox.Show(exception.Message);
            }
            catch(FormatException exception)
            {
                MessageBox.Show(exception.Message);
            }
            catch (ArithmeticException exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void SaveNpcs(object sender, RoutedEventArgs e)
        {
            string npcCsv = m_npcGroup.ToCsv();
            m_serviceCenter.FileIO.SaveToPickedFile(npcCsv, "csv");

            m_serviceCenter.Messager.Send(sender: this, message: new Message.SaveNpcs());
        }

        private void ShowLicensePopup(object sender, RoutedEventArgs e)
        {
            //Lazily create the data as it's unlikely that this button will be clicked. 
            //It's almost unheard of that anyone would click it twice.
            LicenseWindow licenseWindow = new LicenseWindow(m_serviceCenter.Messager, m_serviceCenter.FilePathProvider)
            {
                Owner = this
            };
            licenseWindow.Show();
        }

        private readonly ServiceCenter m_serviceCenter;
        private readonly string m_userSettingsPath;
        private NpcGroup m_npcGroup;
    }
}