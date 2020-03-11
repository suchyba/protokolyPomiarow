using Microsoft.Win32;
using ProtokolyPomiarow.Data;
using ProtokolyPomiarow.MesurementsClass;
using ProtokolyPomiarow.Properties;
using ProtokolyPomiarow.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
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
        public static Workspace ProgramData { get; private set; }
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
            CommandBindings.Add(new CommandBinding(OpenCommand, OpenMenuButt_Click));

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
            { }
        }

        private void DuplicateMesurement(object sender, ExecutedRoutedEventArgs e)
        {
            if (MainTabControl.SelectedIndex == 1 && MesurementsDataGrid.SelectedItem != null)
            {
                activeProject.AddMesurement(MesurementsDataGrid.SelectedItem as Mesurement);
                MesurementsDataGrid.Items.Refresh();
            }
        }

        private void AddButt_Click(object sender, RoutedEventArgs e)
        {
            Window addWindow = new AddWindow();
            addWindow.ShowDialog();
            MesurementsDataGrid.Items.Refresh();

            if (MesurementsDataGrid.Items.Count == 0)
                return;

            EditButt.IsEnabled = true;
            DelButt.IsEnabled = true;
            haveUnsavedChanges = true;
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

                if (MesurementsDataGrid.HasItems == false)
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
            string loc = null;
            if (activeProject.Localization == null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Projekt pomiarów (*.prp)|*.prp";
                if (saveFileDialog.ShowDialog() == true)
                {
                    loc = saveFileDialog.FileName;
                    activeProject.Localization = saveFileDialog.FileName;
                }
                else
                    return;
            }
            else
            {
                loc = activeProject.Localization;
            }
            var ds = new DataContractSerializer(typeof(Project), null, 1000, false, true, null);

            XmlWriterSettings settings = new XmlWriterSettings() { Indent = true, ConformanceLevel = ConformanceLevel.Auto };
            using (XmlWriter w = XmlWriter.Create(loc, settings))
            {
                ds.WriteObject(w, activeProject);
            }
            haveUnsavedChanges = false;
        }
        private void SaveAsAction(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Projekt pomiarów (*.prp)|*.prp";
            if (saveFileDialog.ShowDialog() == true)
            {
                activeProject.Localization = saveFileDialog.FileName;
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

        private void OpenMenuButt_Click(object sender, RoutedEventArgs e)
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

            var ds = new DataContractSerializer(typeof(Project), null, 1000, false, true, null);

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Projekt pomiarów (*.prp)|*.prp";

            if (openFileDialog.ShowDialog() == true)
            {
                XmlReaderSettings settings = new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Auto };
                using (XmlReader r = XmlReader.Create(openFileDialog.FileName, settings))
                {
                    activeProject = ds.ReadObject(r) as Project;

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


                    CustomerInfoBlock.Text = activeProject.CustomerInfo;
                    ObjectInfoBlock.Text = activeProject.ObjectInfo;
                    LightSourceLabel.Content = activeProject.LightSourceInfo;
                    GaugeLabel.Content = activeProject.GaugeInfo;
                }
            }
            haveUnsavedChanges = false;
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

            activeProject = new Project();
            mesurementViewSource.Source = activeProject.Mesurements;
            MesurementsDataGrid.Items.Refresh();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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
                            break;
                        }
                    case MessageBoxResult.No:
                        break;
                }
            }

            var ds = new DataContractSerializer(typeof(Workspace), null, 1000, false, false, null);
            XmlWriterSettings settings = new XmlWriterSettings() { Indent = true, ConformanceLevel = ConformanceLevel.Auto };
            using (XmlWriter w = XmlWriter.Create(Settings.Default.workspaceLocation, settings))
            {
                ds.WriteObject(w, activeWorkspace);
            }
        }

        private void CustomerButton_Click(object sender, RoutedEventArgs e)
        {
            Window c = new CustomersWindow();
            c.ShowDialog();
            CustomerInfoBlock.Text = activeProject.CustomerInfo;
        }
    }
}
