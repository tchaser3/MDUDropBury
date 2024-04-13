/* Title:           Select Customer
 * Date:            8-16-17
 * Author:          Terry Holmes */

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
using CustomersDLL;
using NewEventLogDLL;

namespace MDUDropBury
{
    /// <summary>
    /// Interaction logic for SelectCustomer.xaml
    /// </summary>
    public partial class SelectCustomer : Window
    {
        //setting up the class
        CustomersClass theCustomersClass = new CustomersClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();

        public SelectCustomer()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intRecordsReturned;

            try
            {


                MainWindow.TheFindCustomersByAddressIDDataSet = theCustomersClass.FindCustomersByAddressID(MainWindow.gintAddressID);

                intRecordsReturned = MainWindow.TheFindCustomersByAddressIDDataSet.FindCustomersByAddressID.Rows.Count;

                if(intRecordsReturned == 0)
                {
                    Close();
                }
                else
                {
                    dgrResults.ItemsSource = MainWindow.TheFindCustomersByAddressIDDataSet.FindCustomersByAddressID;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Select Customer // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.gintCustomerID = 0;

            Close();
        }

        private void dgrResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = dgrResults.SelectedIndex;

            MainWindow.gintCustomerID = MainWindow.TheFindCustomersByAddressIDDataSet.FindCustomersByAddressID[intSelectedIndex].CustomerID;

            Close();
        }
    }
}
