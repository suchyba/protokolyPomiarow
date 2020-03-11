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
    /// Logika interakcji dla klasy CustomersWindow.xaml
    /// </summary>
    public partial class CustomersWindow : Window
    {
        public static string SelectedCustomer { get; set; } = null;
        public CustomersWindow()
        {
            InitializeComponent();
            CustomersListBox.ItemsSource = MainWindow.activeWorkspace.Customers;
            CustomersListBox.SelectedItem = MainWindow.activeProject.CustomerInfo;
            CustomersListBox.Focus();
            SelectedCustomer = MainWindow.activeWorkspace.Customers.Select(s => s).Where(s => s == CustomersListBox.SelectedItem as string).FirstOrDefault();
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (CustomersListBox.SelectedItem == null)
            {
                MessageBox.Show("Nie wybrano opcji!", "Wybierz opcję z listy.", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            MainWindow.activeProject.CustomerInfo = CustomersListBox.SelectedItem as string;
            Close();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Window edit = new ModifyCustomerWindow(WindowMode.EDIT);
            edit.ShowDialog();

            if (SelectedCustomer != null)
            {
                MainWindow.activeWorkspace.Customers[CustomersListBox.SelectedIndex] = SelectedCustomer;
                CustomersListBox.SelectedItem = SelectedCustomer;
            }
            else
            {
                EditButton.IsEnabled = false;
                SelectButton.IsEnabled = false;
            }

            CustomersListBox.Items.Refresh();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Window add = new ModifyCustomerWindow(WindowMode.ADD);
            add.ShowDialog();
            CustomersListBox.Items.Refresh();
        }

        private void CustomersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedCustomer = CustomersListBox.SelectedItem as string;

            if (CustomersListBox.SelectedItem != null)
            {
                EditButton.IsEnabled = true;
                SelectButton.IsEnabled = true;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MainWindow.activeProject.CustomerInfo != null)
                MainWindow.activeProject.CustomerInfo = CustomersListBox.SelectedItem as string;
        }
    }
}
