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
    public enum WindowMode
    {
        EDIT, ADD
    }
    /// <summary>
    /// Logika interakcji dla klasy ModifyCustomerWindow.xaml
    /// </summary>
    public partial class ModifyObjectWindow : Window
    {
        private WindowMode winMode;
        private PropertyBinding contentType;
        /// <summary>
        /// Adding new customer or editing/deleting existing one.
        /// </summary>
        /// <param name="mode">Window mode.</param>
        public ModifyObjectWindow(WindowMode mode, PropertyBinding property)
        {
            winMode = mode;
            contentType = property;
            InitializeComponent();
            switch (mode)
            {
                case WindowMode.EDIT:
                    {
                        Title = "Edycja";
                        InfoTextBox.Text = ObjectsListWindow.SelectedObj ?? "";
                        break;
                    }
                case WindowMode.ADD:
                    {
                        Title = "Dodawanie";
                        LeftButton.Visibility = Visibility.Collapsed;
                        RightButton.Content = "Dodaj";
                        break;
                    }
                default:
                    break;
            }
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            switch (winMode)
            {
                case WindowMode.EDIT:
                    {
                        ObjectsListWindow.SelectedObj = InfoTextBox.Text;
                        Close();
                        break;
                    }
                case WindowMode.ADD:
                    {
                        switch (contentType)
                        {
                            case PropertyBinding.Customer:
                                {
                                    MainWindow.activeWorkspace.Customers.Add(InfoTextBox.Text);
                                    break;
                                }
                            case PropertyBinding.Building:
                                {
                                    MainWindow.activeWorkspace.Objects.Add(InfoTextBox.Text);
                                    break;
                                }
                            case PropertyBinding.LightSource:
                                {
                                    MainWindow.activeWorkspace.LightSources.Add(InfoTextBox.Text);
                                    break;
                                }
                            case PropertyBinding.Gauge:
                                {
                                    MainWindow.activeWorkspace.Gauges.Add(InfoTextBox.Text);
                                    break;
                                }
                            case PropertyBinding.DoingPerson:
                                {
                                    MainWindow.activeWorkspace.People.Add(InfoTextBox.Text);
                                    break;
                                }
                            case PropertyBinding.VeryfingPerson:
                                {
                                    MainWindow.activeWorkspace.People.Add(InfoTextBox.Text);
                                    break;
                                }
                            case PropertyBinding.Opinion:
                                {
                                    MainWindow.activeWorkspace.Opinions.Add(InfoTextBox.Text);
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }
                        Close();
                        break;
                    }
                default:
                    break;
            }
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            switch (winMode)
            {
                case WindowMode.EDIT:
                    {
                        switch (contentType)
                        {
                            case PropertyBinding.Customer:
                                {
                                    MainWindow.activeWorkspace.Customers.Remove(ObjectsListWindow.SelectedObj);
                                    break;
                                }
                            case PropertyBinding.Building:
                                {
                                    MainWindow.activeWorkspace.Objects.Remove(ObjectsListWindow.SelectedObj);
                                    break;
                                }
                            case PropertyBinding.LightSource:
                                {
                                    MainWindow.activeWorkspace.LightSources.Remove(ObjectsListWindow.SelectedObj);
                                    break;
                                }
                            case PropertyBinding.Gauge:
                                {
                                    MainWindow.activeWorkspace.Gauges.Remove(ObjectsListWindow.SelectedObj);
                                    break;
                                }
                            case PropertyBinding.DoingPerson:
                                {
                                    MainWindow.activeWorkspace.People.Remove(ObjectsListWindow.SelectedObj);
                                    break;
                                }
                            case PropertyBinding.VeryfingPerson:
                                {
                                    MainWindow.activeWorkspace.People.Remove(ObjectsListWindow.SelectedObj);
                                    break;
                                }
                            case PropertyBinding.Opinion:
                                {
                                    MainWindow.activeWorkspace.Opinions.Remove(ObjectsListWindow.SelectedObj);
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }
                        
                        ObjectsListWindow.SelectedObj = null;
                        Close();
                        break;
                    }
                case WindowMode.ADD:
                    {
                        break;
                    }
                default:
                    break;
            }
        }
    }
}
