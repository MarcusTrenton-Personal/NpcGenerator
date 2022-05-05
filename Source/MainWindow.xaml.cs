using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Win32;

[assembly: CLSCompliant(true)]
namespace NpcGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {   
        public MainWindow()
        {
            InitializeComponent();

            ReadSettings();
            configurationPathText.Content = m_settings.ConfigurationPath;
            npcQuantityText.Text = m_settings.NpcQuantity.ToString(CultureInfo.InvariantCulture);
            UpdateGenerateButtonEnabled();
        }

        private void ReadSettings()
        {
            m_settingsPath = FilePathHelper.SettingsFilePath();
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

                m_settings.ConfigurationPath = openFileDialog.FileName;
                m_settings.Save(m_settingsPath);
            }
        }

        private void NpcQuantityInput(object sender, TextCompositionEventArgs e)
        {
            int unusedResult;
            e.Handled = !NumberHelper.TryParseDigit(e.Text, out unusedResult);
        }

        private void NpcQuantityInputChanged(object sender, TextChangedEventArgs args)
        {
            int newQuantity;
            bool isInt = int.TryParse(npcQuantityText.Text, out newQuantity);
            UpdateGenerateButtonEnabled();

            if(m_settings != null && isInt)
            {
                m_settings.NpcQuantity = int.Parse(npcQuantityText.Text, CultureInfo.InvariantCulture);
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
            int npcQuantity;
            string configurationPath = "";
            bool isValid = ValidateInput(out npcQuantity, ref configurationPath);
            if(!isValid)
            {
                return;
            }

            try
            {
                string cachedConfigurationPath = FilePathHelper.CacheConfigurationFile(configurationPath);
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
            FilePathHelper.SaveToPickedFile(npcCsv, "csv");
        }

        private Settings m_settings;
        private string m_settingsPath;
        private NpcGroup m_npcGroup;
    }
}
