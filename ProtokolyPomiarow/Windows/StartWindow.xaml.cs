using Microsoft.Win32;
using ProtokolyPomiarow.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace ProtokolyPomiarow.Windows
{
    /// <summary>
    /// Logika interakcji dla klasy StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        public event Action<Project> SelectedProject;
        public StartWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Data.CollectionViewSource projectViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("projectViewSource")));
            projectViewSource.Source = MainWindow.activeWorkspace.Projects;

            List<Project> toDelete = new List<Project>();

            foreach (var project in MainWindow.activeWorkspace.Projects)
            {
                if (!File.Exists(project.Localization))
                    toDelete.Add(project);
            }
            foreach (var project in toDelete)
            {
                MainWindow.activeWorkspace.Projects.Remove(project);
            }

            ProjectsDataGrid.Items.Refresh();

            ProjectsDataGrid.SelectedIndex = -1;
            ProjectsDataGrid.Focus();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectsDataGrid.SelectedItem != null)
            {
                if (SelectedProject != null)
                    SelectedProject(ProjectsDataGrid.SelectedItem as Project);
                this.Close();
            }
        }

        private void DelButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Czy jesteś pewien, że chcesz usunąć ten projekt?", "Usuwanie projektu", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                MainWindow.activeWorkspace.Projects.Remove(ProjectsDataGrid.SelectedItem as Project);
                ProjectsDataGrid.Items.Refresh();

                OpenButton.IsEnabled = false;
                DelButton.IsEnabled = false;

                File.Delete((ProjectsDataGrid.SelectedItem as Project).Localization);

                ProjectsDataGrid.SelectedItem = null;
                ProjectsDataGrid.SelectedIndex = -1;
            }            
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            Project newP = new Project();

            if (SelectedProject != null)
                SelectedProject(newP);
            this.Close();
        }

        private void ProjectsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProjectsDataGrid.SelectedItem != null)
            {
                OpenButton.IsEnabled = true;
                DelButton.IsEnabled = true;
            }
            else
            {
                OpenButton.IsEnabled = false;
                DelButton.IsEnabled = false;
            }
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Projekt pomiarów (*.prp)|*.prp";

            if (openFileDialog.ShowDialog() == true)
            {
                Project newProject = null;

                var ds = new DataContractSerializer(typeof(Project), null, 1000, false, true, null);

                XmlReaderSettings settings = new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Auto };
                try
                {
                    using (XmlReader r = XmlReader.Create(openFileDialog.FileName, settings))
                    {
                        newProject = ds.ReadObject(r) as Project;
                    }
                }
                catch (System.IO.FileNotFoundException)
                {
                    MessageBox.Show("Wybrany projekt nie istnieje!", "Brak projektu", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if(MainWindow.activeWorkspace.Projects.Find(p => p.Localization == newProject.Localization) == null)
                {
                    MainWindow.activeWorkspace.Projects.Add(newProject);
                    SelectedProject(newProject);
                    ProjectsDataGrid.Items.Refresh();
                }
                else
                {
                    MessageBox.Show("Wybrany projekt już znajduje się na liście!", "Wybrany projekt", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else
                return;
        }
    }
}
