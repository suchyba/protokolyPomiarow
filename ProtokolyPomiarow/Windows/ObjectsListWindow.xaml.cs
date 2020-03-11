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
    public enum PropertyBinding { Customer, Building, LightSource, Gauge };
    /// <summary>
    /// Logika interakcji dla klasy CustomersWindow.xaml
    /// </summary>
    public partial class ObjectsListWindow : Window
    {
        public static string SelectedObj { get; set; } = null;
        private PropertyBinding mode;
        public ObjectsListWindow(PropertyBinding property)
        {
            mode = property;
            InitializeComponent();
            switch (property)
            {
                case PropertyBinding.Customer:
                    {
                        ListBox.ItemsSource = MainWindow.activeWorkspace.Customers;
                        ListBox.SelectedItem = MainWindow.activeProject.CustomerInfo;
                        SelectedObj = MainWindow.activeWorkspace.Customers.Select(s => s).Where(s => s == ListBox.SelectedItem as string).FirstOrDefault();
                        break;
                    }
                case PropertyBinding.Building:
                    {
                        ListBox.ItemsSource = MainWindow.activeWorkspace.Objects;
                        ListBox.SelectedItem = MainWindow.activeProject.ObjectInfo;
                        SelectedObj = MainWindow.activeWorkspace.Objects.Select(s => s).Where(s => s == ListBox.SelectedItem as string).FirstOrDefault();
                        break;
                    }
                case PropertyBinding.LightSource:
                    {
                        ListBox.ItemsSource = MainWindow.activeWorkspace.LightSources;
                        ListBox.SelectedItem = MainWindow.activeProject.LightSourceInfo;
                        SelectedObj = MainWindow.activeWorkspace.LightSources.Select(s => s).Where(s => s == ListBox.SelectedItem as string).FirstOrDefault();
                        break;
                    }
                case PropertyBinding.Gauge:
                    {
                        ListBox.ItemsSource = MainWindow.activeWorkspace.Gauges;
                        ListBox.SelectedItem = MainWindow.activeProject.GaugeInfo;
                        SelectedObj = MainWindow.activeWorkspace.Gauges.Select(s => s).Where(s => s == ListBox.SelectedItem as string).FirstOrDefault();
                        break;
                    }
                default:
                    {
                        this.Close();
                        break;
                    }
            }

            ListBox.Focus();
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (ListBox.SelectedItem == null)
            {
                MessageBox.Show("Nie wybrano opcji!", "Wybierz opcję z listy.", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            switch (mode)
            {
                case PropertyBinding.Customer:
                    {
                        MainWindow.activeProject.CustomerInfo = ListBox.SelectedItem as string;
                        break;
                    }
                case PropertyBinding.Building:
                    {
                        MainWindow.activeProject.ObjectInfo = ListBox.SelectedItem as string;
                        break;
                    }
                case PropertyBinding.LightSource:
                    {
                        MainWindow.activeProject.LightSourceInfo = ListBox.SelectedItem as string;
                        break;
                    }
                case PropertyBinding.Gauge:
                    {
                        MainWindow.activeProject.GaugeInfo = ListBox.SelectedItem as string;
                        break;
                    }
                default:
                    {
                        this.Close();
                        break;
                    }
            }
            Close();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Window edit = new ModifyObjectWindow(WindowMode.EDIT, mode);
            edit.ShowDialog();

            if (SelectedObj != null)
            {
                switch (mode)
                {
                    case PropertyBinding.Customer:
                        {
                            MainWindow.activeWorkspace.Customers[ListBox.SelectedIndex] = SelectedObj;
                            break;
                        }
                    case PropertyBinding.Building:
                        {
                            MainWindow.activeWorkspace.Objects[ListBox.SelectedIndex] = SelectedObj;
                            break;
                        }
                    case PropertyBinding.LightSource:
                        {
                            MainWindow.activeWorkspace.LightSources[ListBox.SelectedIndex] = SelectedObj;
                            break;
                        }
                    case PropertyBinding.Gauge:
                        {
                            MainWindow.activeWorkspace.Gauges[ListBox.SelectedIndex] = SelectedObj;
                            break;
                        }
                    default:
                        {
                            this.Close();
                            break;
                        }
                }
                ListBox.SelectedItem = SelectedObj;
            }
            else
            {
                EditButton.IsEnabled = false;
                SelectButton.IsEnabled = false;
            }

            ListBox.Items.Refresh();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Window add = new ModifyObjectWindow(WindowMode.ADD, mode);
            add.ShowDialog();
            ListBox.Items.Refresh();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedObj = ListBox.SelectedItem as string;

            if (ListBox.SelectedItem != null)
            {
                EditButton.IsEnabled = true;
                SelectButton.IsEnabled = true;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            switch (mode)
            {
                case PropertyBinding.Customer:
                    {
                        if (MainWindow.activeProject.CustomerInfo != null)
                            MainWindow.activeProject.CustomerInfo = ListBox.SelectedItem as string;
                        break;
                    }
                case PropertyBinding.Building:
                    {
                        if (MainWindow.activeProject.ObjectInfo != null)
                            MainWindow.activeProject.ObjectInfo = ListBox.SelectedItem as string;
                        break;
                    }
                case PropertyBinding.LightSource:
                    {
                        if (MainWindow.activeProject.LightSourceInfo != null)
                            MainWindow.activeProject.LightSourceInfo = ListBox.SelectedItem as string;
                        break;
                    }
                case PropertyBinding.Gauge:
                    {
                        if (MainWindow.activeProject.GaugeInfo != null)
                            MainWindow.activeProject.GaugeInfo = ListBox.SelectedItem as string;
                        break;
                    }
                default:
                    {
                        this.Close();
                        break;
                    }
            }
        }
    }
}
