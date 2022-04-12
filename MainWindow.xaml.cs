using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Win32;

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
        }

        bool TryParsePositiveNumber(string text, out int result)
        {
            bool isInt = int.TryParse(text, out result);
            bool isNaturalNumber = isInt && result > 0;
            return isNaturalNumber;
        }

        void UpdateGenerateButtonEnabled()
        {
            if(generateButton != null && configurationPathText != null && npcQuantityText != null)
            {
                bool isFilePicked = !String.IsNullOrEmpty(configurationPathText.Content.ToString());

                int npcQuantity;
                bool isNpcQuantityPositiveInteger = TryParsePositiveNumber(npcQuantityText.Text, out npcQuantity);

                generateButton.IsEnabled = isFilePicked && isNpcQuantityPositiveInteger;
            }
        }

        private void ChooseConfiguration(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV (*.csv)|*.txt|All files (*.*)|*.*";
            bool? filePicked = openFileDialog.ShowDialog();
            if(filePicked == true)
            {

                configurationPathText.Content = openFileDialog.FileName;
                UpdateGenerateButtonEnabled();
            }
        }

        private void NpcQuantityInput(object sender, TextCompositionEventArgs e)
        {
            int unusedResult;
            e.Handled = !TryParsePositiveNumber(e.Text, out unusedResult);
            UpdateGenerateButtonEnabled();
        }

        private void NpcQuantityInputChanged(object sender, TextChangedEventArgs args)
        {
            UpdateGenerateButtonEnabled();
        }

        private void NpcQuantityPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                int unusedResult;
                if (!TryParsePositiveNumber(text, out unusedResult))
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
            catch (Exception exception)
            {
                MessageBox.Show("Cannot read configuration file. Try a different file.");
                return false;
            }

            bool isNpcQuantityValid = TryParsePositiveNumber(npcQuantityText.Text, out npcQuantity);
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

            //TODO: Parse configuration

            //TODO: Generate NPCs' CSV
            GeneratedText.Text = "";
            for(int i = 0; i < npcQuantity; ++i)
            {
                GeneratedText.Text += "INSERT NPC HERE!";
                if(i+1 < npcQuantity)
                {
                    GeneratedText.Text += "\n";
                }
            }

            //TODO: Display CSV
        }
    }
}
