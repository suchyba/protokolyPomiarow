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
    /// Logika interakcji dla klasy EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        private Mesurement mesurement;
        public EditWindow(Mesurement m)
        {
            mesurement = m;
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

            SourceTextBox.Text = mesurement.Source;
            DestinationTextBox.Text = mesurement.Destination;
            WireTextBox.Text = mesurement.NumberOfWire.ToString();
            DistanceTextBox.Text = mesurement.Distance.ToString();
            PigCountTextBox.Text = mesurement.CountOfPig.ToString();
            WeldCountTextBox.Text = mesurement.CountOfWeld.ToString();
            MesurementTextBox.Text = mesurement.RealAttenuation.ToString();
            CabletypeCombo.SelectedItem = mesurement.Type;

            ManuallyResultCheckBox.IsChecked = mesurement.IsPropperValueManuallySet;

            if (mesurement.IsPropperValueManuallySet)
            {
                ResultComboBox.SelectedIndex = mesurement.PropperValue == true ? 0 : 1;
            }

            SourceTextBox.Focus();
        }
        private void CommitButt_Click(object sender, RoutedEventArgs e)
        {
            int wire = -1, pigCount = -1, weldCount = -1;
            double distance = -1d, real = -1d;

            Mesurement tmp = MainWindow.activeProject.Mesurements.Where(mes => mes.Number == mesurement.Number).FirstOrDefault();

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
            if (ManuallyResultCheckBox.IsChecked == true)
            {
                if (ResultComboBox.SelectedItem == null)
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

            tmp.NumberOfWire = wire;
            tmp.RealAttenuation = real;
            tmp.CountOfPig = pigCount;
            tmp.CountOfWeld = weldCount;
            tmp.Distance = distance;
            tmp.Source = SourceTextBox.Text;
            tmp.Destination = DestinationTextBox.Text;
            tmp.Type = CabletypeCombo.SelectedItem as CableType;

            if (ManuallyResultCheckBox.IsChecked == true)
            {
                if ((string)ResultComboBox.SelectedItem == "Tak")
                {
                    tmp.IsPropperValueManuallySet = true;
                    tmp.PropperValue = true;
                }
                else
                {
                    tmp.IsPropperValueManuallySet = true;
                    tmp.PropperValue = false;
                }
            }
            else
            {
                tmp.IsPropperValueManuallySet = false;
            }

            tmp.RefreshAttenuation();

            this.Close();
        }
        private void ManuallyResultCheckBoxCheckChange(object sender, RoutedEventArgs e)
        {
            if (ManuallyResultCheckBox.IsChecked == true)
                ResultComboBox.IsEnabled = true;
            else if (ManuallyResultCheckBox.IsChecked == false)
                ResultComboBox.IsEnabled = false;
        }
    }
}
