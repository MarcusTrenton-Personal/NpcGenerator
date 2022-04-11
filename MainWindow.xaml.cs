using System;
using System.Collections.Generic;
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

        bool IsPositiveNumber(string text)
        {
            int result;
            bool isInt = int.TryParse(text, out result);
            bool isNaturalNumber = isInt && result > 0;
            return isNaturalNumber;
        }

        private void ChooseConfiguration(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV (*.csv)|*.txt|All files (*.*)|*.*";
            bool? filePicked = openFileDialog.ShowDialog();
            if(filePicked == true)
            {
                configurationPath.Content = openFileDialog.FileName;
            }
        }

        private void NpcQuantityInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsPositiveNumber(e.Text);
        }

        private void NpcQuantityPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsPositiveNumber(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void GenerateNpcs(object sender, RoutedEventArgs e)
        {
            //TODO:
            //Validate input. Seperate handlers?
            //Parse configuration
            //Generate NPCs' CSV
            //Display CSV
        }
    }
}
