using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        bool TryParseDigit(string text, out int result)
        {
            bool isInt = int.TryParse(text, out result);
            bool isDigit = isInt && result >= 0 && result < 10;
            return isDigit;
        }

        void UpdateGenerateButtonEnabled()
        {
            if(generateButton != null && configurationPathText != null && npcQuantityText != null)
            {
                bool isFilePicked = File.Exists(configurationPathText.Content.ToString());

                int npcQuantity;
                bool isNpcQuantityPositiveInteger = TryParsePositiveNumber(npcQuantityText.Text, out npcQuantity);

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
            }
        }

        private void NpcQuantityInput(object sender, TextCompositionEventArgs e)
        {
            int unusedResult;
            e.Handled = !TryParseDigit(e.Text, out unusedResult);
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

            try
            {
                List<TraitGroup> traitGroups = Configuration.Parse(configurationPath);

                StringBuilder text = new StringBuilder();
                GenerateTitles(traitGroups, text);
                text.Append("\n");
                for (int i = 0; i < npcQuantity; ++i)
                {
                    GenerateNpc(traitGroups, text);
                    if (i + 1 < npcQuantity)
                    {
                        text.Append("\n");
                    }
                }

                GeneratedText.Text = text.ToString();
            }
            catch(Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void GenerateTitles(List<TraitGroup> traitGroups, StringBuilder text)
        {
            for (int i = 0; i < traitGroups.Count; ++i)
            {
                text.Append(traitGroups[i].Name);
                if (i + 1 < traitGroups.Count)
                {
                    text.Append(", ");
                }
            }
        }

        private void GenerateNpc(List<TraitGroup> traitGroups, StringBuilder text)
        {
            for(int i = 0; i < traitGroups.Count; ++i)
            {
                text.Append(traitGroups[i].Choose());
                if (i + 1 < traitGroups.Count)
                {
                    text.Append(", ");
                }
            }
        }
    }
}
