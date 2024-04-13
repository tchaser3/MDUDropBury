/* Title:           Select Address
 * Date:            12-21-17
 * Author:          Terry Holmes
 * 
 * Description:     This form is used to select the address */

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
    /// Interaction logic for SelectAddress.xaml
    /// </summary>
    public partial class SelectAddress : Window
    {
        //setting up classes
        EventLogClass TheEventLogClass = new EventLogClass();
        CustomersClass TheCustomerClass = new CustomersClass();
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();

        FindAddressByAddressesDataSet TheFindAddressByAddressesDataSet = new FindAddressByAddressesDataSet();

        public SelectAddress()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                TheFindAddressByAddressesDataSet = TheCustomerClass.FindAddressesByAddress(MainWindow.gstrAddress);

                dgrAddresses.ItemsSource = TheFindAddressByAddressesDataSet.FindAddressesByAddress;

                MainWindow.gblnFormSet = true;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Select Address // Grid Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void dgrAddresses_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            //setting local variables
            DataGrid AddressGrid;
            DataGridRow AddressRow;
            DataGridCell AddressID;
            DataGridCell Address;
            int intSelectedIndex;

            string strAddressID;

            try
            {
                intSelectedIndex = dgrAddresses.SelectedIndex;

                if (intSelectedIndex > -1)
                {

                    AddressGrid = dgrAddresses;
                    AddressRow = (DataGridRow)AddressGrid.ItemContainerGenerator.ContainerFromIndex(AddressGrid.SelectedIndex);
                    AddressID = (DataGridCell)AddressGrid.Columns[0].GetCellContent(AddressRow).Parent;
                    Address = (DataGridCell)AddressGrid.Columns[1].GetCellContent(AddressRow).Parent;
                    strAddressID = ((TextBlock)AddressID.Content).Text;

                    MainWindow.gintAddressID = Convert.ToInt32(strAddressID);
                    MainWindow.gstrAddress = ((TextBlock)Address.Content).Text;

                    Close();

                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Select Address // Address Selection Change " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
