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
            // Załaduj dane poprzez ustawienie właściwości CollectionViewSource.Source:
            // cableTypeViewSource.Źródło = [ogólne źródło danych]
            cableTypeViewSource.Source = MainWindow.activeProject.CableTypes;
            SourceTexBox.Focus();
        }

        private void CommitButt_Click(object sender, RoutedEventArgs e)
        {
            int wire = -1, pigCount = -1, weldCount = -1;
            double distance = -1d, real = -1d;

            WireTexBox.BorderBrush = SystemColors.ActiveBorderBrush;
            PigCounTextBox.BorderBrush = SystemColors.ActiveBorderBrush;
            WeldCountextBox.BorderBrush = SystemColors.ActiveBorderBrush;
            DistanceTextBox.BorderBrush = SystemColors.ActiveBorderBrush;
            MesurementTextBox.BorderBrush = SystemColors.ActiveBorderBrush;

            bool error = false;

            try
            {
                wire = int.Parse(WireTexBox.Text, System.Globalization.NumberStyles.Integer);
            }
            catch (System.FormatException)
            {
                WireTexBox.BorderBrush = Brushes.Red;
                error = true;
            }
            try
            {
                pigCount = int.Parse(PigCounTextBox.Text, System.Globalization.NumberStyles.Integer);
            }
            catch (System.FormatException)
            {
                PigCounTextBox.BorderBrush = Brushes.Red;
                error = true;
            }
            try
            {
                weldCount = int.Parse(WeldCountextBox.Text, System.Globalization.NumberStyles.Integer);
            }
            catch (System.FormatException)
            {
                WeldCountextBox.BorderBrush = Brushes.Red;
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

            if (error)
            {
                MessageBox.Show("Wprowadź poprawną wartość", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MainWindow.activeProject.AddMesurement(SourceTexBox.Text, DestinationTextBox.Text, CabletypeCombo.SelectedItem as CableType, wire, distance, pigCount, weldCount, real);

            this.Close();
        }
    }
}
