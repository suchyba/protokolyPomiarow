using Microsoft.Win32;
using ProtokolyPomiarow.Data;
using ProtokolyPomiarow.MesurementsClass;
using ProtokolyPomiarow.Windows;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Xml;

namespace ProtokolyPomiarow
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Project activeProject { get; private set; } = new Project();
        private System.Windows.Data.CollectionViewSource mesurementViewSource;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mesurementViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("mesurementViewSource")));
            // Załaduj dane poprzez ustawienie właściwości CollectionViewSource.Source:
            // mesurementViewSource.Źródło = [ogólne źródło danych]

            mesurementViewSource.Source = activeProject.Mesurements;
            activeProject.AddCableType("Jednomod", 0.25);
            activeProject.AddCableType("Wielomod", 0.3);
        }

        private void AddButt_Click(object sender, RoutedEventArgs e)
        {
            Window addWindow = new AddWindow();
            addWindow.ShowDialog();
            MesurementsDataGrid.Items.Refresh();
            EditButt.IsEnabled = true;
            DelButt.IsEnabled = true;
        }

        private void EditButt_Click(object sender, RoutedEventArgs e)
        {
            if (MesurementsDataGrid.SelectedItem == null)
                return;

            Window editWindow = new EditWindow(MesurementsDataGrid.SelectedItem as Mesurement);
            editWindow.ShowDialog();
            MesurementsDataGrid.Items.Refresh();
        }

        private void DelButt_Click(object sender, RoutedEventArgs e)
        {
            if (MesurementsDataGrid.SelectedItem == null)
                return;

            if (MessageBox.Show("Czy jesteś pewien, że chcesz usunąć ten pomiar?", "Usuwanie pomiaru", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {

                activeProject.Mesurements.Remove(MesurementsDataGrid.SelectedItem as Mesurement);
                activeProject.RefreshId();
                MesurementsDataGrid.Items.Refresh();

                if (MesurementsDataGrid.HasItems == false)
                {
                    DelButt.IsEnabled = false;
                    EditButt.IsEnabled = false;
                }
            }
        }

        private void DefValMenuButt_Click(object sender, RoutedEventArgs e)
        {
            Window editDefWindow = new EditDefaultsWindow();
            editDefWindow.ShowDialog();
            MesurementsDataGrid.Items.Refresh();
        }

        private void SaveMenuButt_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Projekt pomiarów (*.prp)|*.prp";
            if (saveFileDialog.ShowDialog() == true)
            {
                var ds = new DataContractSerializer(typeof(Project), null, 1000, false, true, null);

                XmlWriterSettings settings = new XmlWriterSettings() { Indent = true, ConformanceLevel = ConformanceLevel.Auto};
                using (XmlWriter w = XmlWriter.Create(saveFileDialog.FileName, settings))
                {
                    ds.WriteObject(w, activeProject);
                }
            }
        }

        private void OpenMenuButt_Click(object sender, RoutedEventArgs e)
        {
            var ds = new DataContractSerializer(typeof(Project), null, 1000, false, true, null);

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Projekt pomiarów (*.prp)|*.prp";

            if (openFileDialog.ShowDialog() == true)
            {
                XmlReaderSettings settings = new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Auto };
                using (XmlReader r = XmlReader.Create(openFileDialog.FileName, settings))
                {
                    activeProject= ds.ReadObject(r) as Project;

                    activeProject.RefreshAllAttenuation();

                    mesurementViewSource.Source = activeProject.Mesurements;
                    MesurementsDataGrid.Items.Refresh();

                    if (MesurementsDataGrid.HasItems == true)
                    {
                        DelButt.IsEnabled = true;
                        EditButt.IsEnabled = true;
                    }
                    else
                    {
                        DelButt.IsEnabled = false;
                        EditButt.IsEnabled = false;
                    }
                }
            }
        }
    }
}
