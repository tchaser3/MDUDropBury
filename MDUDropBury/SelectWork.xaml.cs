/* Title:           Select Work
 * Date:            9-8-17
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
using NewEventLogDLL;
using WorkOrderDLL;
using WorkTypeDLL;
using CustomersDLL;
using DateSearchDLL;

namespace MDUDropBury
{
    /// <summary>
    /// Interaction logic for SelectWork.xaml
    /// </summary>
    public partial class SelectWork : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        WorkOrderClass TheWorkOrderClass = new WorkOrderClass();
        WorkTypeClass TheWorkTypeClass = new WorkTypeClass();
        CustomersClass TheCustomersClass = new CustomersClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();

        //setting up the data
        FindWorkTypeSortedDataSet TheFindWorkTypeSortedDataSet = new FindWorkTypeSortedDataSet();
        FindWorkOrderByWorkOrderTypeAndStatusIDDataSet TheFindWorkByWorkOrderTypeAndStatusIDDataSet = new FindWorkOrderByWorkOrderTypeAndStatusIDDataSet();
        FindWorkOrderStatusByStatusDataSet TheFindWorkOrderStatusByStatusDataSet = new FindWorkOrderStatusByStatusDataSet();
        FindWorkZonesDataSet TheFindWorkZonesDataSet = new FindWorkZonesDataSet();
        FindWorkOrderbyStatusTypeandZoneDataSet TheFindWorkOrderByStatusTypeandZoneDataSet = new FindWorkOrderbyStatusTypeandZoneDataSet();

        WorkSelectDataSet TheWorkSelectedDataSet = new WorkSelectDataSet();

        int gintStatusID;
        string gstrWorkZone;
        string gstrWorkType;

        public SelectWork()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void btnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            MainMenu MainMenu = new MainMenu();
            MainMenu.Show();
            Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;

            try
            {
                //loading work type combo box
                TheFindWorkTypeSortedDataSet = TheWorkTypeClass.FindWorkTypeSorted();

                intNumberOfRecords = TheFindWorkTypeSortedDataSet.FindWorkTypeSorted.Rows.Count - 1;
                cboSelectWorkType.Items.Add("Select Work Type");

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectWorkType.Items.Add(TheFindWorkTypeSortedDataSet.FindWorkTypeSorted[intCounter].WorkType);
                }

                cboSelectWorkType.SelectedIndex = 0;

                TheFindWorkOrderStatusByStatusDataSet = TheWorkOrderClass.FindWorkOrderStatusByStatus("OPEN");

                gintStatusID = TheFindWorkOrderStatusByStatusDataSet.FindWorkOrderStatusByStatus[0].StatusID;

                TheFindWorkZonesDataSet = TheCustomersClass.FindWorkZones();

                intNumberOfRecords = TheFindWorkZonesDataSet.FindWorkZones.Rows.Count - 1;

                cboSelectZone.Items.Add("Select Work Zones");

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectZone.Items.Add(TheFindWorkZonesDataSet.FindWorkZones[intCounter].ZoneLocation);
                }

                cboSelectZone.SelectedIndex = 0;
                cboSelectZone.IsEnabled = false;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Select Work // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
       
        }

        private void cboSelectWorkType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variables
            int intSelectedIndex;
            int intCounter;
            int intNumberOfRecords;
            DateTime datStartDate = DateTime.Now;
            DateTime datEndDate = DateTime.Now;

            try
            {
                intSelectedIndex = cboSelectWorkType.SelectedIndex;

                datStartDate = TheDateSearchClass.RemoveTime(datStartDate);
                datEndDate = TheDateSearchClass.AddingDays(datStartDate, 1);

                if (intSelectedIndex > 0)
                {
                    TheWorkSelectedDataSet.work.Rows.Clear();

                    gstrWorkType = cboSelectWorkType.SelectedItem.ToString();

                    TheFindWorkByWorkOrderTypeAndStatusIDDataSet = TheWorkOrderClass.FindWorkOrderByTypeAndStatusID(gstrWorkType, gintStatusID, datStartDate, datEndDate);

                    intNumberOfRecords = TheFindWorkByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID.Rows.Count - 1;

                    if (intNumberOfRecords > -1)
                    {
                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            
                            WorkSelectDataSet.workRow NewWorkRow = TheWorkSelectedDataSet.work.NewworkRow();

                            NewWorkRow.WorkOrderID = TheFindWorkByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID[intCounter].WorkOrderID;
                            NewWorkRow.WorkOrderNumber = TheFindWorkByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID[intCounter].WorkOrderNumber;
                            NewWorkRow.AccountNumber = TheFindWorkByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID[intCounter].AccountNumber;
                            NewWorkRow.PhoneNumber = TheFindWorkByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID[intCounter].PhoneNumber;
                            NewWorkRow.City = TheFindWorkByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID[intCounter].City;
                            NewWorkRow.DateScheduled = TheFindWorkByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID[intCounter].DateScheduled;
                            NewWorkRow.ZoneLocation = TheFindWorkByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID[intCounter].ZoneLocation;
                            NewWorkRow.StreetAddress = TheFindWorkByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID[intCounter].StreetAddress;

                            TheWorkSelectedDataSet.work.Rows.Add(NewWorkRow);
                               
                        }
                    }

                    dgrWork.ItemsSource = TheWorkSelectedDataSet.work;
                    cboSelectZone.IsEnabled = true;
                    cboSelectZone.SelectedIndex = 0;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Select Work // Select Work CBO " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
            
        }

        private void cboSelectZone_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            int intTypeSelectedIndex;
            int intCounter;
            int intNumberOfRecords;
            DateTime datStartDate = DateTime.Now;
            DateTime datEndDate = DateTime.Now;

            intSelectedIndex = cboSelectZone.SelectedIndex;
            datStartDate = TheDateSearchClass.RemoveTime(datStartDate);
            datEndDate = TheDateSearchClass.AddingDays(datStartDate, 1);

            try
            {
                if (intSelectedIndex > 0)
                {
                    TheWorkSelectedDataSet.work.Rows.Clear();

                    intTypeSelectedIndex = cboSelectWorkType.SelectedIndex;

                    if (intTypeSelectedIndex <= 0)
                    {
                        TheMessagesClass.ErrorMessage("The Work Type Was Not Selected");
                        return;
                    }

                    gstrWorkZone = cboSelectZone.SelectedItem.ToString();

                    TheFindWorkOrderByStatusTypeandZoneDataSet = TheWorkOrderClass.FindWorkOrderbyStatusTypeandZone(gintStatusID, gstrWorkType, gstrWorkZone, datStartDate, datEndDate);

                    intNumberOfRecords = TheFindWorkOrderByStatusTypeandZoneDataSet.FindWorkOrderByStatusTypeandZone.Rows.Count - 1;

                    if (intNumberOfRecords > -1)
                    {
                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            WorkSelectDataSet.workRow NewWorkRow = TheWorkSelectedDataSet.work.NewworkRow();

                            NewWorkRow.WorkOrderID = TheFindWorkOrderByStatusTypeandZoneDataSet.FindWorkOrderByStatusTypeandZone[intCounter].WorkOrderID;
                            NewWorkRow.WorkOrderNumber = TheFindWorkOrderByStatusTypeandZoneDataSet.FindWorkOrderByStatusTypeandZone[intCounter].WorkOrderNumber;
                            NewWorkRow.AccountNumber = TheFindWorkOrderByStatusTypeandZoneDataSet.FindWorkOrderByStatusTypeandZone[intCounter].AccountNumber;
                            NewWorkRow.PhoneNumber = TheFindWorkOrderByStatusTypeandZoneDataSet.FindWorkOrderByStatusTypeandZone[intCounter].PhoneNumber;
                            NewWorkRow.City = TheFindWorkOrderByStatusTypeandZoneDataSet.FindWorkOrderByStatusTypeandZone[intCounter].City;
                            NewWorkRow.DateScheduled = TheFindWorkOrderByStatusTypeandZoneDataSet.FindWorkOrderByStatusTypeandZone[intCounter].DateScheduled;
                            NewWorkRow.ZoneLocation = TheFindWorkOrderByStatusTypeandZoneDataSet.FindWorkOrderByStatusTypeandZone[intCounter].ZoneLocation;
                            NewWorkRow.StreetAddress = TheFindWorkOrderByStatusTypeandZoneDataSet.FindWorkOrderByStatusTypeandZone[intCounter].StreetAddress;

                            TheWorkSelectedDataSet.work.Rows.Add(NewWorkRow);
                        }
                    }

                    dgrWork.ItemsSource = TheWorkSelectedDataSet.work;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Select Work // Select Zone CBO " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            
        }

        private void dgrWork_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variables
            DataGrid WorkGrid;
            DataGridRow WorkRow;
            DataGridCell WorkID;
            DataGridCell WorkOrderNumber;

            string strWorkOrderID;

            try
            {
                WorkGrid = dgrWork;
                WorkRow = (DataGridRow)WorkGrid.ItemContainerGenerator.ContainerFromIndex(WorkGrid.SelectedIndex);
                WorkID = (DataGridCell)WorkGrid.Columns[0].GetCellContent(WorkRow).Parent;
                WorkOrderNumber = (DataGridCell)WorkGrid.Columns[1].GetCellContent(WorkRow).Parent;
                strWorkOrderID = ((TextBlock)WorkID.Content).Text;

                MainWindow.gintWorkOrderID = Convert.ToInt32(strWorkOrderID);
                MainWindow.gstrWorkOrderNumber = ((TextBlock)WorkOrderNumber.Content).Text;

                ScheduleJobs ScheduleJobs = new ScheduleJobs();
                ScheduleJobs.ShowDialog();

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury //Select Work // Work Selection Change " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnImportWorkOrders_Click(object sender, RoutedEventArgs e)
        {
            ImportWorkOrders ImportWorkOrders = new ImportWorkOrders();
            ImportWorkOrders.ShowDialog();

            cboSelectWorkType.SelectedIndex = 0;
            cboSelectZone.SelectedIndex = 0;
        }
    }
}
