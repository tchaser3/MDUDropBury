/* Title:           View Open Work Orders
 * Date:            9-26-17
 * Author:          Terry Holmes
 * 
 * Description:     This will allow the user to view open work orders */

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
using CustomersDLL;
using WorkTypeDLL;
using DateSearchDLL;

namespace MDUDropBury
{
    /// <summary>
    /// Interaction logic for ViewOpenWorkOrders.xaml
    /// </summary>
    public partial class ViewOpenWorkOrders : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        WorkOrderClass TheWorkOrderClass = new WorkOrderClass();
        CustomersClass TheCustomersClass = new CustomersClass();
        WorkTypeClass TheWorkTypeClass = new WorkTypeClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();

        FindWorkTypeSortedDataSet TheFindWorkTypeSortedDataSet = new FindWorkTypeSortedDataSet();
        FindWorkZonesDataSet TheFindWorkZonesDataSet = new FindWorkZonesDataSet();
        FindWorkOrderByWorkOrderTypeAndStatusIDDataSet TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet = new FindWorkOrderByWorkOrderTypeAndStatusIDDataSet();
        FindWorkOrderStatusByStatusDataSet TheFindWorkOrderStatusByStatusDataSet = new FindWorkOrderStatusByStatusDataSet();
        FindWorkOrderbyStatusTypeandZoneDataSet TheFindWorkOrderbyStatusTypeandZoneDataSet = new FindWorkOrderbyStatusTypeandZoneDataSet();

        //setting up the global variables
        int gintWorkOrderID;
        int gintWorkTypeID;
        int gintZoneID;
        string gstrWorkOrderNumber;
        int gintStatusID;
        string gstrWorkType;
        string gstrZone;

        public ViewOpenWorkOrders()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            MainMenu MainMenu = new MainMenu();
            MainMenu.Show();
            Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;

            try
            {
                TheFindWorkTypeSortedDataSet = TheWorkTypeClass.FindWorkTypeSorted();
                intNumberOfRecords = TheFindWorkTypeSortedDataSet.FindWorkTypeSorted.Rows.Count - 1;
                cboSelectWorkType.Items.Add("Select Work Type");

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectWorkType.Items.Add(TheFindWorkTypeSortedDataSet.FindWorkTypeSorted[intCounter].WorkType);
                }

                cboSelectWorkType.SelectedIndex = 0;

                //filling the second combo box
                TheFindWorkZonesDataSet = TheCustomersClass.FindWorkZones();
                cboSelectZone.Items.Add("Select Work Zone");
                intNumberOfRecords = TheFindWorkZonesDataSet.FindWorkZones.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectZone.Items.Add(TheFindWorkZonesDataSet.FindWorkZones[intCounter].ZoneLocation);
                }


                TheFindWorkOrderStatusByStatusDataSet = TheWorkOrderClass.FindWorkOrderStatusByStatus("OPEN");

                gintStatusID = TheFindWorkOrderStatusByStatusDataSet.FindWorkOrderStatusByStatus[0].StatusID;

                cboSelectZone.SelectedIndex = 0;
            
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // View Open Work Orders // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }        
        }

        private void cboSelectWorkType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local Variables
            int intSelectedIndex;
            DateTime datEndDate = DateTime.Now;
            DateTime datStartDate = DateTime.Now;
           

            intSelectedIndex = cboSelectWorkType.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                gstrWorkType = TheFindWorkTypeSortedDataSet.FindWorkTypeSorted[intSelectedIndex].WorkType;
                gintWorkTypeID = TheFindWorkTypeSortedDataSet.FindWorkTypeSorted[intSelectedIndex].TypeID;

                datEndDate = TheDateSearchClass.RemoveTime(datEndDate);
                datEndDate = TheDateSearchClass.AddingDays(datEndDate, 1);
                datStartDate = TheDateSearchClass.SubtractingDays(datEndDate, 700);

                TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet = TheWorkOrderClass.FindWorkOrderByTypeAndStatusID(gstrWorkType, gintStatusID, datStartDate, datEndDate);

                dgrResults.ItemsSource = TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID;

            }
        }

        private void cboSelectZone_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            DateTime datEndDate = DateTime.Now;
            DateTime datStartDate = DateTime.Now;

            intSelectedIndex = cboSelectZone.SelectedIndex - 1;

            datEndDate = TheDateSearchClass.RemoveTime(datEndDate);
            datEndDate = TheDateSearchClass.AddingDays(datEndDate, 1);
            datStartDate = TheDateSearchClass.SubtractingDays(datEndDate, 700);

            if (intSelectedIndex > -1)
            {
                if(cboSelectWorkType.SelectedIndex > 0)
                {
                    gstrZone = TheFindWorkZonesDataSet.FindWorkZones[intSelectedIndex].ZoneLocation;
                    gintZoneID = TheFindWorkZonesDataSet.FindWorkZones[intSelectedIndex].ZoneID;

                    TheFindWorkOrderbyStatusTypeandZoneDataSet = TheWorkOrderClass.FindWorkOrderbyStatusTypeandZone(gintStatusID, gstrWorkType, gstrZone, datStartDate, datEndDate);

                    dgrResults.ItemsSource = TheFindWorkOrderbyStatusTypeandZoneDataSet.FindWorkOrderByStatusTypeandZone;
                }
                else
                {
                    TheMessagesClass.InformationMessage("Please Select The Work Type");
                }
               
            }
        }

        private void dgrResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variables
            DataGrid WorkGrid;
            DataGridRow WorkRow;
            DataGridCell WorkID;
            DataGridCell WorkOrderNumber;
            int intSelectedIndex;

            string strWorkOrderID;

            try
            {
                intSelectedIndex = dgrResults.SelectedIndex;

                if(intSelectedIndex > -1)
                {

                    WorkGrid = dgrResults;
                    WorkRow = (DataGridRow)WorkGrid.ItemContainerGenerator.ContainerFromIndex(WorkGrid.SelectedIndex);
                    WorkID = (DataGridCell)WorkGrid.Columns[0].GetCellContent(WorkRow).Parent;
                    WorkOrderNumber = (DataGridCell)WorkGrid.Columns[1].GetCellContent(WorkRow).Parent;
                    strWorkOrderID = ((TextBlock)WorkID.Content).Text;

                    MainWindow.gintWorkOrderID = Convert.ToInt32(strWorkOrderID);
                    MainWindow.gstrWorkOrderNumber = ((TextBlock)WorkOrderNumber.Content).Text;

                    ScheduleJobs ScheduleJobs = new ScheduleJobs();
                    ScheduleJobs.ShowDialog();

                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury //View Open Work Orders // Results Selection Change " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
