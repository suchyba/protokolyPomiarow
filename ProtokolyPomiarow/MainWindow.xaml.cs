using Microsoft.Win32;
using ProtokolyPomiarow.Data;
using ProtokolyPomiarow.MesurementsClass;
using ProtokolyPomiarow.PDF;
using ProtokolyPomiarow.Properties;
using ProtokolyPomiarow.Windows;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml;

namespace ProtokolyPomiarow
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Project activeProject { get; private set; } = new Project();
        public static Workspace activeWorkspace { get; private set; }
        private System.Windows.Data.CollectionViewSource mesurementViewSource;

        private static RoutedCommand SaveCommand = new RoutedCommand();
        private static RoutedCommand OpenCommand = new RoutedCommand();
        private static RoutedCommand SaveAsCommand = new RoutedCommand();
        private static RoutedCommand NewCommand = new RoutedCommand();
        private static RoutedCommand DuplicateCommand = new RoutedCommand();

        private bool haveUnsavedChanges = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mesurementViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("mesurementViewSource")));

            mesurementViewSource.Source = activeProject.Mesurements;

            SaveCommand.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(SaveCommand, SaveAction));

            OpenCommand.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(OpenCommand, OpenProjectEvent));

            SaveAsCommand.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift));
            CommandBindings.Add(new CommandBinding(SaveAsCommand, SaveAsAction));

            NewCommand.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(NewCommand, NewProject));

            DuplicateCommand.InputGestures.Add(new KeyGesture(Key.D, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(DuplicateCommand, DuplicateMesurement));

            var ds = new DataContractSerializer(typeof(Workspace), null, 1000, false, false, null);
            XmlReaderSettings settings = new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Auto };
            try
            {
                using (XmlReader r = XmlReader.Create(Settings.Default.workspaceLocation, settings))
                {
                    activeWorkspace = ds.ReadObject(r) as Workspace ?? new Workspace();
                }
            }
            catch
            {
                activeWorkspace = new Workspace();
            }

            OpenProjectEvent(this, new RoutedEventArgs());
        }

        private void DuplicateMesurement(object sender, ExecutedRoutedEventArgs e)
        {
            if (MainTabControl.SelectedIndex == 1 && MesurementsDataGrid.SelectedItem != null && (MesurementsDataGrid.SelectedItem as Mesurement).Number != null)
            {
                activeProject.AddMesurement(MesurementsDataGrid.SelectedItem as Mesurement);
                MesurementsDataGrid.Items.Refresh();
                haveUnsavedChanges = true;
            }
            else
            {
                activeProject.AddLabel((MesurementsDataGrid.SelectedItem as Mesurement).Source);
                MesurementsDataGrid.Items.Refresh();
                haveUnsavedChanges = true;
            }
        }

        private void AddButt_Click(object sender, RoutedEventArgs e)
        {
            Window addWindow = new AddWindow();
            addWindow.ShowDialog();
            MesurementsDataGrid.Items.Refresh();

            if (MesurementsDataGrid.Items.Count == 0)
                return;
        }
        private void AddLabelButt_Click(object sender, RoutedEventArgs e)
        {
            Window addWindow = new AddLabelWindow();
            addWindow.ShowDialog();

            MesurementsDataGrid.Items.Refresh();
        }
        private void EditButt_Click(object sender, RoutedEventArgs e)
        {
            if (MesurementsDataGrid.SelectedItem == null)
                return;

            Window editWindow = new EditWindow(MesurementsDataGrid.SelectedItem as Mesurement);
            editWindow.ShowDialog();
            MesurementsDataGrid.Items.Refresh();
            haveUnsavedChanges = true;
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

                if (MesurementsDataGrid.SelectedItem == null)
                {
                    DelButt.IsEnabled = false;
                    EditButt.IsEnabled = false;
                }
                haveUnsavedChanges = true;
            }
        }

        private void DefValMenuButt_Click(object sender, RoutedEventArgs e)
        {
            Window editDefWindow = new EditDefaultsWindow();
            editDefWindow.ShowDialog();
            MesurementsDataGrid.Items.Refresh();
            haveUnsavedChanges = true;
        }

        private void SaveAction(object sender, RoutedEventArgs e)
        {
            activeProject.ProtocolNumber = ProtocolNumberTextBox.Text;
            activeProject.MesurementDate = MesurementDatePicker.SelectedDate;
            activeProject.DocumentDate = (DateTime)DocumentDatePicker.SelectedDate;
            activeProject.Conclusions = ConclusionsTextBox.Text;

            string loc = null;
            if (activeProject.Localization == null)
            {
                loc = System.AppDomain.CurrentDomain.BaseDirectory + @"projects\" + activeProject.ProtocolNumber.Replace('/', '_') + ".prp";
                activeProject.Localization = loc;
                ++activeWorkspace.LastProtocolNumber;
                activeWorkspace.Projects.Add(activeProject);
            }
            else
            {
                loc = activeProject.Localization;
                activeWorkspace.Projects.Remove(activeWorkspace.Projects.Find(p => p.Localization == activeProject.Localization));
                activeWorkspace.Projects.Add(activeProject);
            }
            var ds = new DataContractSerializer(typeof(Project), null, 1000, false, true, null);

            XmlWriterSettings settings = new XmlWriterSettings() { Indent = true, ConformanceLevel = ConformanceLevel.Auto };
            using (XmlWriter w = XmlWriter.Create(loc, settings))
            {
                ds.WriteObject(w, activeProject);
            }
            haveUnsavedChanges = false;
            MessageBox.Show("Zapisano zmiany", "Zapis", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void SaveAsAction(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Projekt pomiarów (*.prp)|*.prp";
            bool localization = false;
            if (activeProject.Localization == null)
            {
                ++activeWorkspace.LastProtocolNumber;
                localization = true;
            }

            if (saveFileDialog.ShowDialog() == true)
            {
                activeProject.Localization = saveFileDialog.FileName;
                if (localization)
                    activeWorkspace.Projects.Add(activeProject);
            }
            else
                return;

            var ds = new DataContractSerializer(typeof(Project), null, 1000, false, true, null);

            XmlWriterSettings settings = new XmlWriterSettings() { Indent = true, ConformanceLevel = ConformanceLevel.Auto };
            using (XmlWriter w = XmlWriter.Create(saveFileDialog.FileName, settings))
            {
                ds.WriteObject(w, activeProject);
            }
            haveUnsavedChanges = false;
        }

        private void OpenProjectEvent(object sender, RoutedEventArgs e)
        {
            if (haveUnsavedChanges)
            {
                MessageBoxResult result = MessageBox.Show("Czy zapisać zmiany w projekcie?", "Zmiany", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                switch (result)
                {
                    case MessageBoxResult.Cancel:
                        return;
                    case MessageBoxResult.Yes:
                        {
                            SaveAction(sender, null);
                            if (haveUnsavedChanges)
                                return;
                            break;
                        }
                    case MessageBoxResult.No:
                        break;
                }
            }
            do
            {
                Project selProject = null;
                StartWindow start = new StartWindow();
                start.SelectedProject += p => selProject = p;
                Visibility = Visibility.Collapsed;
                start.ShowDialog();

                if (selProject == null)
                {
                    activeProject = null;
                    this.Close();
                    return;
                }
                else
                {
                    LoadProject(selProject);
                }
            }
            while (activeProject == null);
            try
            { this.Visibility = Visibility.Visible; }
            catch { }
            haveUnsavedChanges = true;
        }
        private void NewProject(object sender, RoutedEventArgs e)
        {
            if (haveUnsavedChanges)
            {
                MessageBoxResult result = MessageBox.Show("Czy zapisać zmiany w projekcie?", "Zmiany", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                switch (result)
                {
                    case MessageBoxResult.Cancel:
                        return;
                    case MessageBoxResult.Yes:
                        {
                            SaveAction(sender, null);
                            if (haveUnsavedChanges)
                                return;
                            break;
                        }
                    case MessageBoxResult.No:
                        break;
                }
            }

            if (activeProject == null)
                activeProject = new Project();

            mesurementViewSource.Source = activeProject.Mesurements;
            MesurementsDataGrid.Items.Refresh();
            CustomerInfoBlock.Text = "";
            ObjectInfoBlock.Text = "";
            GaugeLabel.Content = "";
            LightSourceLabel.Content = "";
            switch (activeWorkspace.ProtocolsNumeringOption)
            {
                case NumeringOption.XX:
                    {
                        activeProject.ProtocolNumber = $"{activeWorkspace.LastProtocolNumber + 1}";
                        break;
                    }
                case NumeringOption.XX_YYYY:
                    {
                        activeProject.ProtocolNumber = $"{activeWorkspace.LastProtocolNumber + 1}/{DateTime.Now.Year}";
                        break;
                    }
                default:
                    break;
            }
            ProtocolNumberTextBox.Text = activeProject.ProtocolNumber;
            activeProject.DocumentDate = DateTime.Now;
            DocumentDatePicker.SelectedDate = activeProject.DocumentDate;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (activeProject != null)
            {
                if (haveUnsavedChanges)
                {
                    MessageBoxResult result = MessageBox.Show("Czy zapisać zmiany w projekcie?", "Zmiany", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                    switch (result)
                    {
                        case MessageBoxResult.Cancel:
                            {
                                e.Cancel = true;
                                break;
                            }
                        case MessageBoxResult.Yes:
                            {
                                SaveAction(sender, null);
                                if (haveUnsavedChanges)
                                    e.Cancel = true;
                                OpenProjectEvent(this, null);
                                break;
                            }
                        case MessageBoxResult.No:
                            {
                                haveUnsavedChanges = false;
                                OpenProjectEvent(this, null);
                                e.Cancel = true;
                                break;
                            }
                    }
                }
                else
                {
                    e.Cancel = true;
                    OpenProjectEvent(this, null);
                }
            }
        }

        private void CustomerButton_Click(object sender, RoutedEventArgs e)
        {
            Window c = new ObjectsListWindow(PropertyBinding.Customer);
            c.ShowDialog();
            CustomerInfoBlock.Text = activeProject.CustomerInfo;
        }

        private void ObjectButton_Click(object sender, RoutedEventArgs e)
        {
            Window c = new ObjectsListWindow(PropertyBinding.Building);
            c.ShowDialog();
            ObjectInfoBlock.Text = activeProject.ObjectInfo;
        }

        private void SourceButton_Click(object sender, RoutedEventArgs e)
        {
            Window c = new ObjectsListWindow(PropertyBinding.LightSource);
            c.ShowDialog();
            LightSourceLabel.Content = activeProject.LightSourceInfo;
        }

        private void GaugeButton_Click(object sender, RoutedEventArgs e)
        {
            Window c = new ObjectsListWindow(PropertyBinding.Gauge);
            c.ShowDialog();
            GaugeLabel.Content = activeProject.GaugeInfo;
        }

        private void LoadProject(string localization)
        {
            if (localization == null)
            {
                NewProject(this, new RoutedEventArgs());
            }

            else
            {
                var ds = new DataContractSerializer(typeof(Project), null, 1000, false, true, null);

                XmlReaderSettings settings = new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Auto };
                try
                {
                    using (XmlReader r = XmlReader.Create(localization, settings))
                    {
                        activeProject = ds.ReadObject(r) as Project;

                        activeProject.RefreshAllAttenuation();

                        mesurementViewSource.Source = activeProject.Mesurements;
                        MesurementsDataGrid.Items.Refresh();
                        MesurementsDataGrid.SelectedIndex = -1;

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
                        if (activeProject.CustomerInfo != null && activeWorkspace.Customers.Find(s => s == activeProject.CustomerInfo) == null)
                        {
                            activeWorkspace.Customers.Add(activeProject.CustomerInfo);
                        }
                        if (activeProject.ObjectInfo != null && activeWorkspace.Objects.Find(s => s == activeProject.ObjectInfo) == null)
                        {
                            activeWorkspace.Objects.Add(activeProject.ObjectInfo);
                        }
                        if (activeProject.LightSourceInfo != null && activeWorkspace.LightSources.Find(s => s == activeProject.LightSourceInfo) == null)
                        {
                            activeWorkspace.LightSources.Add(activeProject.LightSourceInfo);
                        }
                        if (activeProject.GaugeInfo != null && activeWorkspace.Gauges.Find(s => s == activeProject.GaugeInfo) == null)
                        {
                            activeWorkspace.Gauges.Add(activeProject.GaugeInfo);
                        }
                        if (activeProject.DoingPerson != null && activeWorkspace.People.Find(s => s == activeProject.DoingPerson) == null)
                        {
                            activeWorkspace.People.Add(activeProject.DoingPerson);
                        }
                        if (activeProject.VeryfingPerson != null && activeWorkspace.People.Find(s => s == activeProject.VeryfingPerson) == null)
                        {
                            activeWorkspace.People.Add(activeProject.VeryfingPerson);
                        }
                        if (activeProject.Opinion != null && activeWorkspace.Opinions.Find(s => s == activeProject.Opinion) == null)
                        {
                            activeWorkspace.Opinions.Add(activeProject.Opinion);
                        }

                        CustomerInfoBlock.Text = activeProject.CustomerInfo;
                        ObjectInfoBlock.Text = activeProject.ObjectInfo;
                        LightSourceLabel.Content = activeProject.LightSourceInfo;
                        GaugeLabel.Content = activeProject.GaugeInfo;
                        ProtocolNumberTextBox.Text = activeProject.ProtocolNumber;
                        DocumentDatePicker.SelectedDate = activeProject.DocumentDate;
                        MesurementDatePicker.SelectedDate = activeProject.MesurementDate;
                        DoingPersonTextBlock.Text = activeProject.DoingPerson;
                        VeryfingPersonTextBlock.Text = activeProject.VeryfingPerson;
                        OpinionTextBlock.Text = activeProject.Opinion;
                        ConclusionsTextBox.Text = activeProject.Conclusions;
                    }
                }
                catch (System.IO.FileNotFoundException)
                {
                    MessageBox.Show("Wybrany projekt nie istnieje!", "Brak projektu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    activeWorkspace.Projects.Remove(activeWorkspace.Projects.Find(p => p.Localization == localization));
                    activeProject = null;
                    return;
                }

                haveUnsavedChanges = true;
            }
        }
        private void LoadProject(Project project)
        {
            activeProject = project;
            LoadProject(project.Localization);
            haveUnsavedChanges = true;
        }

        private void CloseProject(object sender, RoutedEventArgs e)
        {
            OpenProjectEvent(sender, e);
        }

        private void SthChangedInProject(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            haveUnsavedChanges = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            var ds = new DataContractSerializer(typeof(Workspace), null, 1000, false, false, null);
            XmlWriterSettings settings = new XmlWriterSettings() { Indent = true, ConformanceLevel = ConformanceLevel.Auto };
            using (XmlWriter w = XmlWriter.Create(Settings.Default.workspaceLocation, settings))
            {
                ds.WriteObject(w, activeWorkspace);
            }
        }

        private void DoingPersonButt_Click(object sender, RoutedEventArgs e)
        {
            Window c = new ObjectsListWindow(PropertyBinding.DoingPerson);
            c.ShowDialog();
            DoingPersonTextBlock.Text = activeProject.DoingPerson;
        }

        private void VeryfingPersonButt_Click(object sender, RoutedEventArgs e)
        {
            Window c = new ObjectsListWindow(PropertyBinding.VeryfingPerson);
            c.ShowDialog();
            VeryfingPersonTextBlock.Text = activeProject.VeryfingPerson;
        }

        private void OpinionButton_Click(object sender, RoutedEventArgs e)
        {
            Window c = new ObjectsListWindow(PropertyBinding.Opinion);
            c.ShowDialog();
            OpinionTextBlock.Text = activeProject.Opinion;
        }

        private void MesurementsDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(MesurementsDataGrid.SelectedItem != null)
            {
                EditButt.IsEnabled = true;
                DelButt.IsEnabled = true;
            }
            else
            {
                EditButt.IsEnabled = false;
                DelButt.IsEnabled = false;
            }
        }

        private void ExportMenuButt_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog savePfdDialog = new SaveFileDialog();
            savePfdDialog.Filter = "Dokument PDF (*.pdf)|*.pdf";

            if (savePfdDialog.ShowDialog() == true)
            {
                PdfDesigner.MakePDF(activeProject, savePfdDialog.FileName);
            }
            else
                return;
        }
    }

    [ValueConversion(typeof(Nullable<bool>), typeof(String))]
    public class BoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return " ";
            
            return (bool)value == true ? "Pozytywna" : "Negatywna";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value == "Tak" ? true : false;
        }
    }
}
