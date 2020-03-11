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
    public partial class ModifyCustomerWindow : Window
    {
        private WindowMode winMode;
        /// <summary>
        /// Adding new customer or editing/deleting existing one.
        /// </summary>
        /// <param name="mode">Window mode.</param>
        public ModifyCustomerWindow(WindowMode mode)
        {
            winMode = mode;
            InitializeComponent();
            switch (mode)
            {
                case WindowMode.EDIT:
                    {
                        Title = "Edutuj zleceniodawcę";
                        CustomerInfoTextBox.Text = CustomersWindow.SelectedCustomer ?? "";
                        break;
                    }
                case WindowMode.ADD:
                    {
                        Title = "Nowy zleceniodawca";
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
                        CustomersWindow.SelectedCustomer = CustomerInfoTextBox.Text;
                        Close();
                        break;
                    }
                case WindowMode.ADD:
                    {
                        MainWindow.activeWorkspace.Customers.Add(CustomerInfoTextBox.Text);
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
                        MainWindow.activeWorkspace.Customers.Remove(CustomersWindow.SelectedCustomer);
                        CustomersWindow.SelectedCustomer = null;
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
