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
using Services;
using Services.Message;
using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace NpcGenerator
{
    public class NpcGeneratorModel : BaseModel, INpcGeneratorModel
    {
        public NpcGeneratorModel(IUserSettings userSettings, IMessager messager, ILocalFileIO fileIo)
        {
            m_userSettings = userSettings;
            m_messager = messager;
            m_fileIo = fileIo;
        }

        public ICommand ChooseConfiguration 
        { 
            get
            {
                return m_chooseConfigurationCommand ??= new CommandHandler(
                    (object parameter) => ExecuteChooseConfiguration(parameter),
                    (object parameter) => AlwaysTrue(parameter));
            }
        }

        public string ConfigurationPath 
        { 
            get
            {
                return m_userSettings.ConfigurationPath;
            }
        }

        public int NpcQuantity
        {
            get
            {
                return m_userSettings.NpcQuantity;
            }

            set
            {
                m_userSettings.NpcQuantity = value;
            }
        }

        public ICommand GenerateNpcs 
        { 
            get
            {
                return m_generateNpcsCommand ??= new CommandHandler(
                    (object parameter) => ExecuteGenerateNpcs(parameter),
                    (object parameter) => CanExecuteGenerateNpcs(parameter));
            }
        }

        public DataTable ResultNpcs 
        { 
            get
            {
                return m_table;
            }
        }

        public ICommand SaveNpcs 
        { 
            get
            {
                return m_saveNpcsCommand ??= new CommandHandler(
                    (object parameter) => ExecuteSaveNpcs(parameter),
                    (object parameter) => CanExecuteSaveNpcs(parameter));
            }
        }

        private bool AlwaysTrue(object _)
        {
            return true;
        }

        private void ExecuteChooseConfiguration(object _)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text CSV (*.csv)|*.csv|All files (*.*)|*.*"
            };
            bool? filePicked = openFileDialog.ShowDialog();
            if (filePicked == true)
            {
                m_userSettings.ConfigurationPath = openFileDialog.FileName;
                NotifyPropertyChanged("ConfigurationPath");

                m_messager.Send(sender: this, message: new Message.SelectConfiguration());
            }
        }

        private bool CanExecuteGenerateNpcs(object _)
        {
            bool configurationFileExists = File.Exists(m_userSettings.ConfigurationPath);
            bool isNpcQuantityValid = m_userSettings.NpcQuantity > 0;
            bool canExecute = configurationFileExists && isNpcQuantityValid;
            return canExecute;
        }

        private void ExecuteGenerateNpcs(object _)
        {
            try
            {
                string cachedConfigurationPath = m_fileIo.CacheFile(m_userSettings.ConfigurationPath);
                TraitSchema traitSchema = ConfigurationFile.Parse(cachedConfigurationPath);
                m_npcGroup = new NpcGroup(traitSchema, m_userSettings.NpcQuantity);

                DataTable table = new DataTable("Npc Table");
                for (int i = 0; i < m_npcGroup.TraitGroupCount; ++i)
                {
                    table.Columns.Add(m_npcGroup.GetTraitGroupNameAtIndex(i));
                }
                for (int i = 0; i < m_npcGroup.NpcCount; ++i)
                {
                    table.Rows.Add(m_npcGroup.GetNpcAtIndex(i).GetTraits());
                }
                m_table = table;
                NotifyPropertyChanged("ResultNpcs");

                m_messager.Send(sender: this, message: new Message.GenerateNpcs(m_userSettings.NpcQuantity));
            }
            catch (IOException exception)
            {
                MessageBox.Show(exception.Message);
            }
            catch (FormatException exception)
            {
                MessageBox.Show(exception.Message);
            }
            catch (ArithmeticException exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private bool CanExecuteSaveNpcs(object _)
        {
            return m_npcGroup != null;
        }

        private void ExecuteSaveNpcs(object _)
        {
            string npcCsv = m_npcGroup.ToCsv();
            m_fileIo.SaveToPickedFile(npcCsv, "csv");

            m_messager.Send(sender: this, message: new Message.SaveNpcs());
        }

        private readonly IUserSettings m_userSettings;
        private readonly ILocalFileIO m_fileIo;
        private readonly IMessager m_messager;

        private ICommand m_chooseConfigurationCommand;
        private ICommand m_generateNpcsCommand;
        private ICommand m_saveNpcsCommand;

        private NpcGroup m_npcGroup;
        private DataTable m_table;
    }
}