using Microsoft.Win32;
using ProtokolyPomiarow.Data;
using ProtokolyPomiarow.MesurementsClass;
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
using System.Windows.Shapes;

namespace ProtokolyPomiarow.Windows
{
    /// <summary>
    /// Logika interakcji dla klasy EditDefaultsWindow.xaml
    /// </summary>
    public partial class EditDefaultsWindow : Window
    {
        public EditDefaultsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            System.Windows.Data.CollectionViewSource cableTypeViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("cableTypeViewSource")));
            
            cableTypeViewSource.Source = MainWindow.activeProject.CableTypes;
            CableTypeAttenuationTextBox.Text = (CableTypeComboBox.SelectedItem as CableType).Attenuation.ToString("n");
            WeldAttenuationTextBox.Text = MainWindow.activeProject.WeldAttenuation.ToString("n");
            PigAttenuationTextBox.Text = MainWindow.activeProject.PigAttenuation.ToString("n");

            CurrProjectNumberTextBox.Text = (MainWindow.activeWorkspace.LastProtocolNumber + 1).ToString("d");

            List<string> numeringOptions = new List<string>();
            numeringOptions.Add("XX");
            numeringOptions.Add("XX/RRRR");

            NumeringOptionComboBox.ItemsSource = numeringOptions;
            NumeringOptionComboBox.SelectedIndex = (int)MainWindow.activeWorkspace.ProtocolsNumeringOption;

            if(MainWindow.activeWorkspace.LogoImg != null)
                LogoImage.Source = new BitmapImage(new Uri(MainWindow.activeWorkspace.LogoImg));
        }

        private void SafeButton_Click(object sender, RoutedEventArgs e)
        {
            CurrProjectNumberTextBox.BorderBrush = SystemColors.ActiveBorderBrush;
            WeldAttenuationTextBox.BorderBrush = SystemColors.ActiveBorderBrush;
            PigAttenuationTextBox.BorderBrush = SystemColors.ActiveBorderBrush;
            CableTypeAttenuationTextBox.BorderBrush = SystemColors.ActiveBorderBrush;

            double weldValue = -1, pigValue = -1, atte = -1;
            int currProt = -1;

            bool error = false;

            try
            {
                weldValue = double.Parse(WeldAttenuationTextBox.Text);
            }
            catch
            {
                error = true;
                WeldAttenuationTextBox.BorderBrush = Brushes.Red;
            }

            try
            {
                pigValue = double.Parse(PigAttenuationTextBox.Text);
            }
            catch
            {
                error = true;
                PigAttenuationTextBox.BorderBrush = Brushes.Red;
            }

            try
            {
                atte = double.Parse(CableTypeAttenuationTextBox.Text);                
            }
            catch
            {
                error = true;
                CableTypeAttenuationTextBox.BorderBrush = Brushes.Red;
            }

            MainWindow.activeWorkspace.ProtocolsNumeringOption = (NumeringOption)NumeringOptionComboBox.SelectedIndex;
            try
            {
                currProt = int.Parse(CurrProjectNumberTextBox.Text);
                --currProt;
            }
            catch
            {
                CurrProjectNumberTextBox.BorderBrush = Brushes.Red;
                error = true;
            }

            if(error)
            {
                MessageBox.Show("Wprowadź poprawną wartość", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else
            {
                MainWindow.activeWorkspace.LastProtocolNumber = currProt;
                MainWindow.activeProject.WeldAttenuation = weldValue;
                MainWindow.activeProject.PigAttenuation = pigValue;
                (CableTypeComboBox.SelectedItem as CableType).Attenuation = atte;
                MainWindow.activeWorkspace.ProtocolsNumeringOption = (NumeringOption)NumeringOptionComboBox.SelectedIndex;

                MainWindow.activeProject.RefreshAllAttenuation();

                MainWindow.activeWorkspace.LogoImg = (LogoImage.Source as BitmapImage)?.UriSource.LocalPath;

                this.Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CableTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CableTypeAttenuationTextBox.Text = (CableTypeComboBox.SelectedItem as CableType).Attenuation.ToString("n");
        }

        private void LogoImageButt_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog selLogoDialog = new OpenFileDialog();
            selLogoDialog.Filter = "Obraz (*.jpg, *.png, *.bmp, *.gif)|*.jpg;*.png;*.bmp;*.gif";

            if (selLogoDialog.ShowDialog() == true)
            {
                //MainWindow.activeWorkspace.LogoImg = selLogoDialog.FileName;
                LogoImage.Source = new BitmapImage(new Uri(selLogoDialog.FileName));
            }
            else
                return;
        }
    }
}
