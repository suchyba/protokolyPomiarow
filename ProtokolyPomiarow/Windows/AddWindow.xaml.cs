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
        }

        private void CommitButt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.activeProject.AddMesurement(SourceTexBox.Text, DestinationTextBox.Text, CabletypeCombo.SelectedItem as CableType, int.Parse(WireTexBox.Text), double.Parse(DistanceTextBox.Text), int.Parse(PigCounTextBox.Text), int.Parse(WeldCountextBox.Text), double.Parse(MesurementTextBox.Text));
            }
            catch(System.FormatException ex)
            {
                MessageBox.Show("Wprowadź poprawną wartość", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            this.Close();
        }
    }
}
