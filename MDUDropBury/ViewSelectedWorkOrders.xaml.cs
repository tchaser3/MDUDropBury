/* Title:           View Selected Work Orders
 * Date:            10-16-17
 * Author:          Terry Holmes
 * 
 * Description:     This used to view selected work orders. */

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
using DateSearchDLL;
using WorkOrderDLL;
using WorkOrderScheduleDLL;
using CustomersDLL;
using WorkTypeDLL;


namespace MDUDropBury
{
    /// <summary>
    /// Interaction logic for ViewSelectedWorkOrders.xaml
    /// </summary>
    public partial class ViewSelectedWorkOrders : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        WorkOrderClass TheWorkOrderClass = new WorkOrderClass();
        WorkOrderScheduleClass TheWorkOrderScheduleClass = new WorkOrderScheduleClass();
        CustomersClass TheCustomerClass = new CustomersClass();
        WorkTypeClass TheWorkTypeClass = new WorkTypeClass();

        //setting up the data
        FindWorkTypeSortedDataSet TheFindWorkTypeSortedDataSet = new FindWorkTypeSortedDataSet();
        FindWorkZonesDataSet TheFindWorkZonesDataSet = new FindWorkZonesDataSet();
        FindScheduledWorkByStatusAndDateRangeDataSet TheFindScheduledWorkbyStatusAndDateRangeDataSet = new FindScheduledWorkByStatusAndDateRangeDataSet();
        FindScheduledWorkByTypeStatusDateRangeDataSet TheFindScheduledWorkByTypeStatusAndDateRangeDataSet = new FindScheduledWorkByTypeStatusDateRangeDataSet();
        FindScheduledWorkByZoneTypeStatusDateRangeDataSet TheFindScheduledWorkByZoneTypeStatusDateRangeDataSet = new FindScheduledWorkByZoneTypeStatusDateRangeDataSet();

        DateTime gdatStartDate;
        DateTime gdatEndDate;
        string gstrWorkType;
        int gintWorkTypeID;
        string gstrWorkZone;
        int gintZoneID;

        public ViewSelectedWorkOrders()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //setting local variabvles
            int intCounter;
            int intNumberOfRecords;

            try
            {
                cboSelectType.Items.Add("Select Work Type");

                TheFindWorkTypeSortedDataSet = TheWorkTypeClass.FindWorkTypeSorted();

                intNumberOfRecords = TheFindWorkTypeSortedDataSet.FindWorkTypeSorted.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectType.Items.Add(TheFindWorkTypeSortedDataSet.FindWorkTypeSorted[intCounter].WorkType);
                }

                cboSelectType.SelectedIndex = 0;

                TheFindWorkZonesDataSet = TheCustomerClass.FindWorkZones();

                intNumberOfRecords = TheFindWorkZonesDataSet.FindWorkZones.Rows.Count - 1;

                cboSelectZone.Items.Add("Select Work Zone");

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectZone.Items.Add(TheFindWorkZonesDataSet.FindWorkZones[intCounter].ZoneLocation);
                }

                cboSelectZone.SelectedIndex = 0;

                cboSelectType.IsEnabled = false;
                cboSelectZone.IsEnabled = false;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // View Selected Work Orders // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void calSelectDate_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            gdatStartDate= Convert.ToDateTime(calSelectDate.SelectedDate);
            gdatStartDate = TheDateSearchClass.RemoveTime(gdatStartDate);
            gdatEndDate = gdatStartDate.AddHours(23);
            gdatEndDate = gdatEndDate.AddMinutes(59);
            gdatEndDate = gdatEndDate.AddSeconds(59);

            TheFindScheduledWorkbyStatusAndDateRangeDataSet = TheWorkOrderScheduleClass.FindScheduledWorkByStatusAndDateRange("SCHEDULED", gdatStartDate, gdatEndDate);

            dgrResults.ItemsSource = TheFindScheduledWorkbyStatusAndDateRangeDataSet.FindScheduledWorkByStatusAndDateRange;

            cboSelectType.IsEnabled = true;
            cboSelectType.SelectedIndex = 0;
            cboSelectZone.SelectedIndex = 0;
        }

        private void cboSelectType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectType.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                gintWorkTypeID = TheFindWorkTypeSortedDataSet.FindWorkTypeSorted[intSelectedIndex].TypeID;
                gstrWorkType = TheFindWorkTypeSortedDataSet.FindWorkTypeSorted[intSelectedIndex].WorkType;

                TheFindScheduledWorkByTypeStatusAndDateRangeDataSet = TheWorkOrderScheduleClass.FindScheduledWorkByTypeStatusDateRange(gstrWorkType, "SCHEDULED", gdatStartDate, gdatEndDate);

                dgrResults.ItemsSource = TheFindScheduledWorkByTypeStatusAndDateRangeDataSet.FindScheduleWorkByTypeStatusAndDateRange;

                cboSelectZone.IsEnabled = true;
            }
        }

        private void cboSelectZone_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboSelectZone.SelectedIndex - 1;

            if (intSelectedIndex > -1)
            {
                if(cboSelectType.SelectedIndex > -1)
                {
                    gintZoneID = TheFindWorkZonesDataSet.FindWorkZones[intSelectedIndex].ZoneID;
                    gstrWorkZone = TheFindWorkZonesDataSet.FindWorkZones[intSelectedIndex].ZoneLocation;

                    TheFindScheduledWorkByZoneTypeStatusDateRangeDataSet = TheWorkOrderScheduleClass.FindScheduledWorkByZoneTypeStatusDateRange(gstrWorkZone, gstrWorkType, "SCHEDULED", gdatStartDate, gdatEndDate);

                    dgrResults.ItemsSource = TheFindScheduledWorkByZoneTypeStatusDateRangeDataSet.FindScheduleWorkByZoneTypeStatusAndDateRange;
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

                    TheFindScheduledWorkbyStatusAndDateRangeDataSet = TheWorkOrderScheduleClass.FindScheduledWorkByStatusAndDateRange("SCHEDULED", gdatStartDate, gdatEndDate);

                    dgrResults.ItemsSource = TheFindScheduledWorkbyStatusAndDateRangeDataSet.FindScheduledWorkByStatusAndDateRange;

                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // View Selected Work Orders // Result Selection Change " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
