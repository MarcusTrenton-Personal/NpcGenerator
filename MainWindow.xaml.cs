using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Win32;

namespace NpcGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string SAVE_FOLDER = "NpcGenerator";
        private const string SETTING_FILE = "Settings.json";
        
        public MainWindow()
        {
            InitializeComponent();

            ReadSettings();
            configurationPathText.Content = m_settings.configurationsPath;
            npcQuantityText.Text = m_settings.npcQuantity.ToString();
        }

        private void ReadSettings()
        {
            string commonAppData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            m_settingsPath = Path.Combine(commonAppData, SAVE_FOLDER, SETTING_FILE);
            m_settings = Settings.Load(m_settingsPath);
            if (m_settings == null)
            {
                m_settings = new Settings();
            }
        }

        private void UpdateGenerateButtonEnabled()
        {
            if(generateButton != null && configurationPathText != null && npcQuantityText != null)
            {
                bool isFilePicked = File.Exists(configurationPathText.Content.ToString());

                int npcQuantity;
                bool isNpcQuantityPositiveInteger = 
                    NumberHelper.TryParsePositiveNumber(npcQuantityText.Text, out npcQuantity);

                generateButton.IsEnabled = isFilePicked && isNpcQuantityPositiveInteger;
            }
        }

        private void ChooseConfiguration(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text CSV (*.csv)|*.csv|All files (*.*)|*.*";
            bool? filePicked = openFileDialog.ShowDialog();
            if(filePicked == true)
            {

                configurationPathText.Content = openFileDialog.FileName;
                UpdateGenerateButtonEnabled();

                m_settings.configurationsPath = openFileDialog.FileName;
                m_settings.Save(m_settingsPath);
            }
        }

        private void NpcQuantityInput(object sender, TextCompositionEventArgs e)
        {
            int unusedResult;
            e.Handled = !NumberHelper.TryParseDigit(e.Text, out unusedResult);
            UpdateGenerateButtonEnabled();

            m_settings.npcQuantity = int.Parse(npcQuantityText.Text);
            m_settings.Save(m_settingsPath);
        }

        private void NpcQuantityInputChanged(object sender, TextChangedEventArgs args)
        {
            UpdateGenerateButtonEnabled();

            if(m_settings != null)
            {
                m_settings.npcQuantity = int.Parse(npcQuantityText.Text);
                m_settings.Save(m_settingsPath);
            }
        }

        private void NpcQuantityPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                int unusedResult;
                if (!NumberHelper.TryParsePositiveNumber(text, out unusedResult))
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
            e.Handled = UriHelper.OpenUri(e.Uri.AbsoluteUri);
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
            try
            {
                string text = File.ReadAllText(configurationPath);
            }
            catch (IOException exception)
            {
                MessageBox.Show(exception.Message);
                return false;
            }
            catch (Exception exception)
            {
                MessageBox.Show("Problem with configuration file: " + exception.Message);
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
            int npcQuantity;
            string configurationPath = "";
            bool isValid = ValidateInput(out npcQuantity, ref configurationPath);
            if(!isValid)
            {
                return;
            }

            try
            {
                List<TraitGroup> traitGroups = Configuration.Parse(configurationPath);
                NpcGroup npcGroup = new NpcGroup(traitGroups, npcQuantity);

                System.Data.DataTable table = new DataTable("Npc Table");
                for(int i = 0; i < npcGroup.TraitGroupCount; ++i)
                {
                    table.Columns.Add(npcGroup.GetTraitGroupNameAtIndex(i));
                } 
                for(int i = 0; i < npcGroup.NpcCount; ++i)
                {
                    table.Rows.Add(npcGroup.GetNpcAtIndex(i).GetTraits());
                }
                generatedNpcTable.DataContext = table;
            }
            catch(Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        Settings m_settings = null;
        string m_settingsPath;
    }
}
