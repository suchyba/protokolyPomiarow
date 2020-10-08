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
    /// Logika interakcji dla klasy AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        public AddWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Data.CollectionViewSource cableTypeViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("cableTypeViewSource")));
            cableTypeViewSource.Source = MainWindow.activeProject.CableTypes;
            List<string> results = new List<string>();
            results.Add("Tak");
            results.Add("Nie");

            ResultComboBox.ItemsSource = results;
            SourceTextBox.Focus();
        }

        private void CommitButt_Click(object sender, RoutedEventArgs e)
        {
            int wire = -1, pigCount = -1, weldCount = -1;
            double distance = -1d, real = -1d;

            WireTextBox.BorderBrush = SystemColors.ActiveBorderBrush;
            PigCountTextBox.BorderBrush = SystemColors.ActiveBorderBrush;
            WeldCountTextBox.BorderBrush = SystemColors.ActiveBorderBrush;
            DistanceTextBox.BorderBrush = SystemColors.ActiveBorderBrush;
            MesurementTextBox.BorderBrush = SystemColors.ActiveBorderBrush;
            ResultComboBox.BorderBrush = SystemColors.ActiveBorderBrush;

            bool error = false;

            try
            {
                wire = int.Parse(WireTextBox.Text, System.Globalization.NumberStyles.Integer);
            }
            catch (System.FormatException)
            {
                WireTextBox.BorderBrush = Brushes.Red;
                error = true;
            }
            try
            {
                pigCount = int.Parse(PigCountTextBox.Text, System.Globalization.NumberStyles.Integer);
            }
            catch (System.FormatException)
            {
                PigCountTextBox.BorderBrush = Brushes.Red;
                error = true;
            }
            try
            {
                weldCount = int.Parse(WeldCountTextBox.Text, System.Globalization.NumberStyles.Integer);
            }
            catch (System.FormatException)
            {
                WeldCountTextBox.BorderBrush = Brushes.Red;
                error = true;
            }
            try
            {
                distance = double.Parse(DistanceTextBox.Text, System.Globalization.NumberStyles.Float);
            }
            catch (System.FormatException)
            {
                DistanceTextBox.BorderBrush = Brushes.Red;
                error = true;
            }
            try
            {
                real = double.Parse(MesurementTextBox.Text, System.Globalization.NumberStyles.Float);
            }
            catch (System.FormatException)
            {
                MesurementTextBox.BorderBrush = Brushes.Red;
                error = true;
            }
            if(ManuallyResultCheckBox.IsChecked == true)
            {
                if(ResultComboBox.SelectedItem == null)
                {
                    ResultComboBox.BorderBrush = Brushes.Red;
                    error = true;
                }
            }

            if (error)
            {
                MessageBox.Show("Wprowadź poprawną wartość", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if(ManuallyResultCheckBox.IsChecked == true)
            {
                if((string)ResultComboBox.SelectedItem == "Tak")
                    MainWindow.activeProject.AddMesurement(SourceTextBox.Text, DestinationTextBox.Text, CabletypeCombo.SelectedItem as CableType, wire, distance, pigCount, weldCount, real, true);
                else
                    MainWindow.activeProject.AddMesurement(SourceTextBox.Text, DestinationTextBox.Text, CabletypeCombo.SelectedItem as CableType, wire, distance, pigCount, weldCount, real, false);
            }
            else
                MainWindow.activeProject.AddMesurement(SourceTextBox.Text, DestinationTextBox.Text, CabletypeCombo.SelectedItem as CableType, wire, distance, pigCount, weldCount, real);

            this.Close();
        }

        private void ManuallyResultCheckBoxCheckChange(object sender, RoutedEventArgs e)
        {
            if (ManuallyResultCheckBox.IsChecked == true)
                ResultComboBox.IsEnabled = true;
            else if(ManuallyResultCheckBox.IsChecked == false)
                ResultComboBox.IsEnabled = false;
        }
    }
}
