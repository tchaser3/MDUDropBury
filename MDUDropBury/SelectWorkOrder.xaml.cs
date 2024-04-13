/* Title:           Select Work Order
 * Date:            12-21-17
 * Author:          Terry Holmes
 * 
 * Description:     This form will allow the user to select a work order */

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
using NewEventLogDLL;
using WorkOrderDLL;

namespace MDUDropBury
{
    /// <summary>
    /// Interaction logic for SelectWorkOrder.xaml
    /// </summary>
    public partial class SelectWorkOrder : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        WorkOrderClass TheWorkOrderClass = new WorkOrderClass();

        FindWorkOrderByAddressIDDataSet TheFindWorkOrderByAddressIDDataSet = new FindWorkOrderByAddressIDDataSet();

        public SelectWorkOrder()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.gintWorkOrderID = -10;
            Close();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TheFindWorkOrderByAddressIDDataSet = TheWorkOrderClass.FindWorkOrderByAddressID(MainWindow.gintAddressID);

            dgrWorkOrders.ItemsSource = TheFindWorkOrderByAddressIDDataSet.FindWorkOrderByAddressID;

            MainWindow.gblnWorkOrderSet = true;
        }

        private void dgrAddresses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variables
            DataGrid WorkGrid;
            DataGridRow WorkRow;
            DataGridCell WorkID;
            DataGridCell WorkOrderNumber;

            string strWorkOrderID;

            try
            {
                WorkGrid = dgrWorkOrders;
                WorkRow = (DataGridRow)WorkGrid.ItemContainerGenerator.ContainerFromIndex(WorkGrid.SelectedIndex);
                WorkID = (DataGridCell)WorkGrid.Columns[0].GetCellContent(WorkRow).Parent;
                WorkOrderNumber = (DataGridCell)WorkGrid.Columns[2].GetCellContent(WorkRow).Parent;
                strWorkOrderID = ((TextBlock)WorkID.Content).Text;

                MainWindow.gintWorkOrderID = Convert.ToInt32(strWorkOrderID);
                MainWindow.gstrWorkOrderNumber = ((TextBlock)WorkOrderNumber.Content).Text;

                Close();

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Select Work Order // Work Selection Change " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
