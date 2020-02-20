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
            // Załaduj dane poprzez ustawienie właściwości CollectionViewSource.Source:
            // cableTypeViewSource.Źródło = [ogólne źródło danych]
            cableTypeViewSource.Source = MainWindow.activeProject.CableTypes;
            SourceTextBox.Text = mesurement.Source;
            DestinationTextBox.Text = mesurement.Destination;
            WireTextBox.Text = mesurement.NumberOfWire.ToString();
            DistanceTextBox.Text = mesurement.Distance.ToString();
            PigCountTextBox.Text = mesurement.CountOfPig.ToString();
            WeldCountTextBox.Text = mesurement.CountOfWeld.ToString();
            MesurementTextBox.Text = mesurement.RealAttenuation.ToString();
            CabletypeCombo.SelectedItem = mesurement.Type;
        }
        private void CommitButt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.activeProject.Mesurements.Where(mes => mes.Number == mesurement.Number).FirstOrDefault().NumberOfWire = int.Parse(WireTextBox.Text);
                MainWindow.activeProject.Mesurements.Where(mes => mes.Number == mesurement.Number).FirstOrDefault().RealAttenuation = double.Parse(MesurementTextBox.Text);
                MainWindow.activeProject.Mesurements.Where(mes => mes.Number == mesurement.Number).FirstOrDefault().CountOfPig = int.Parse(PigCountTextBox.Text);
                MainWindow.activeProject.Mesurements.Where(mes => mes.Number == mesurement.Number).FirstOrDefault().CountOfWeld = int.Parse(WeldCountTextBox.Text);
                MainWindow.activeProject.Mesurements.Where(mes => mes.Number == mesurement.Number).FirstOrDefault().Distance = double.Parse(DistanceTextBox.Text);
                MainWindow.activeProject.Mesurements.Where(mes => mes.Number == mesurement.Number).FirstOrDefault().Source = SourceTextBox.Text;
                MainWindow.activeProject.Mesurements.Where(mes => mes.Number == mesurement.Number).FirstOrDefault().Destination = DestinationTextBox.Text;
                MainWindow.activeProject.Mesurements.Where(mes => mes.Number == mesurement.Number).FirstOrDefault().Type = CabletypeCombo.SelectedItem as CableType; 
                MainWindow.activeProject.Mesurements.Where(mes => mes.Number == mesurement.Number).FirstOrDefault().RefreshAttenuation();
            }
            catch (System.FormatException)
            {
                MessageBox.Show("Wprowadź poprawną wartość", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            this.Close();
        }
    }
}
