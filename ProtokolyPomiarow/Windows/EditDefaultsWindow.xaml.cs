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
        }

        private void SafeButton_Click(object sender, RoutedEventArgs e)
        {
            double weldStartValue = MainWindow.activeProject.WeldAttenuation;
            double pigStartValue = MainWindow.activeProject.PigAttenuation;
            double cableTypeStartValue = (CableTypeComboBox.SelectedItem as CableType).Attenuation;
            try
            {
                MainWindow.activeProject.WeldAttenuation = double.Parse(WeldAttenuationTextBox.Text);
                MainWindow.activeProject.PigAttenuation = double.Parse(PigAttenuationTextBox.Text);

                (CableTypeComboBox.SelectedItem as CableType).Attenuation = double.Parse(CableTypeAttenuationTextBox.Text);

                MainWindow.activeProject.RefreshAllAttenuation();
            }
            catch (System.FormatException)
            {
                MainWindow.activeProject.WeldAttenuation = weldStartValue;
                MainWindow.activeProject.PigAttenuation = pigStartValue;
                (CableTypeComboBox.SelectedItem as CableType).Attenuation = cableTypeStartValue;
                MessageBox.Show("Wprowadź poprawną wartość", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
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
    }
}
