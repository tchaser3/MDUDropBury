/* Title:           Closed Work Orders
 * Date:            12-27-17
 * Author:          Terry Holmes
 * 
 * Description:     This is used to show the closed orders */

using DataValidationDLL;
using DropBuryMDUDLL;
using NewEventLogDLL;
using System;
using System.Windows;
using System.Windows.Input;
using WorkOrderDLL;
using WorkTypeDLL;
using DateSearchDLL;
using CustomersDLL;
using System.Windows.Controls;

namespace MDUDropBury
{
    /// <summary>
    /// Interaction logic for ClosedOrders.xaml
    /// </summary>
    public partial class ClosedOrders : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DropBuryMDUClass TheDropBuryMDUClass = new DropBuryMDUClass();
        WorkOrderClass TheWorkOrderClass = new WorkOrderClass();
        WorkTypeClass TheWorkTypeClass = new WorkTypeClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        CustomersClass TheCustomersClass = new CustomersClass();

        FindWorkOrderByWorkOrderTypeAndStatusIDDataSet TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet = new FindWorkOrderByWorkOrderTypeAndStatusIDDataSet();
        FindWorkTypeSortedDataSet TheFindWorkTypeSortedDataSet = new FindWorkTypeSortedDataSet();
        FindWorkZonesDataSet TheFindWorkZonesDataSet = new FindWorkZonesDataSet();
        FindWorkOrderbyStatusTypeandZoneDataSet TheFindWorkOrderByStatusTypeandZoneDataSet = new FindWorkOrderbyStatusTypeandZoneDataSet();

        //created data sets
        OrderStatusDataSet TheOrderStatusDataSet = new OrderStatusDataSet();

        //setting global variables
        int gintStatusID = 1002;
        DateTime gdatStartDate;
        DateTime gdatEndDate;
        int gintWorkTypeID;
        string gstrWorkType;
        int gintZoneID;
        string gstrZoneLocation;
        bool gblnTextSearch;
        bool gblnZoneSearch;

        public ClosedOrders()
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

        private void cboWorkType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variables
            int intSelectedIndex;
            int intCounter;
            int intNumberOfRecords;
            string strCity;
            DateTime datReceivedDate;
            int intWorkOrderID;
            string strWorkOrderNumber;
            string strAccountNumber;
            string strAddress;
            string strPhoneNumber;
            string strStatus;
            string strZone;
            bool blnFatalError;

            try
            {
                intSelectedIndex = cboWorkType.SelectedIndex - 1;
                TheOrderStatusDataSet.orderstatus.Rows.Clear();

                cboReportType.SelectedIndex = 0;
                cboZone.SelectedIndex = 0;
                HideZoneControls();

                if (intSelectedIndex > -1)
                {
                    gintWorkTypeID = TheFindWorkTypeSortedDataSet.FindWorkTypeSorted[intSelectedIndex].TypeID;
                    gstrWorkType = TheFindWorkTypeSortedDataSet.FindWorkTypeSorted[intSelectedIndex].WorkType;

                    TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet = TheWorkOrderClass.FindWorkOrderByTypeAndStatusID(gstrWorkType, gintStatusID, gdatStartDate, gdatEndDate);

                    intNumberOfRecords = TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID.Rows.Count - 1;

                    if (intNumberOfRecords > -1)
                    {
                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            intWorkOrderID = TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID[intCounter].WorkOrderID;
                            datReceivedDate = TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID[intCounter].DateReceived;
                            strAccountNumber = TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID[intCounter].AccountNumber;
                            strAddress = TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID[intCounter].StreetAddress;
                            strCity = TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID[intCounter].City;
                            strPhoneNumber = TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID[intCounter].PhoneNumber;
                            strStatus = TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID[intCounter].WorkOrderStatus;
                            strWorkOrderNumber = TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID[intCounter].WorkOrderNumber;
                            strZone = TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID[intCounter].ZoneLocation;

                            blnFatalError = LoadOrderStatusDataSet(intWorkOrderID, strWorkOrderNumber, strAccountNumber, strAddress, strPhoneNumber, strCity, datReceivedDate, strStatus, strZone);

                            if (blnFatalError == true)
                                throw new Exception();
                        }

                    }

                    dgrResults.ItemsSource = TheOrderStatusDataSet.orderstatus;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Closed Orders // cbo Work Type Changed " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboReportType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string strReportType;
            int intCounter;
            int intNumberOfRecords;
            string strCity;
            DateTime datReceivedDate;
            int intWorkOrderID;
            string strWorkOrderNumber;
            string strAccountNumber;
            string strAddress;
            string strPhoneNumber;
            string strStatus;
            string strZone;
            bool blnFatalError;

            try
            {
                strReportType = cboReportType.SelectedItem.ToString();
                cboZone.SelectedIndex = 0;
                HideZoneControls();
                HideTextControls();
                gblnZoneSearch = false;
                gblnTextSearch = false;
                TheOrderStatusDataSet.orderstatus.Rows.Clear();


                if (cboWorkType.SelectedIndex > 0)
                {
                    if (strReportType == "All Work Type Info")
                    {
                        TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet = TheWorkOrderClass.FindWorkOrderByTypeAndStatusID(gstrWorkType, gintStatusID, gdatStartDate, gdatEndDate);

                        intNumberOfRecords = TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID.Rows.Count - 1;

                        if (intNumberOfRecords > -1)
                        {
                            for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                            {
                                intWorkOrderID = TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID[intCounter].WorkOrderID;
                                datReceivedDate = TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID[intCounter].DateReceived;
                                strAccountNumber = TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID[intCounter].AccountNumber;
                                strAddress = TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID[intCounter].StreetAddress;
                                strCity = TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID[intCounter].City;
                                strPhoneNumber = TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID[intCounter].PhoneNumber;
                                strStatus = TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID[intCounter].WorkOrderStatus;
                                strWorkOrderNumber = TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID[intCounter].WorkOrderNumber;
                                strZone = TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID[intCounter].ZoneLocation;

                                blnFatalError = LoadOrderStatusDataSet(intWorkOrderID, strWorkOrderNumber, strAccountNumber, strAddress, strPhoneNumber, strCity, datReceivedDate, strStatus, strZone);

                                if (blnFatalError == true)
                                    throw new Exception();
                            }

                        }

                        dgrResults.ItemsSource = TheOrderStatusDataSet.orderstatus;
                    }
                    else if (strReportType == "Zone")
                    {
                        ViewZoneControls();
                    }
                    else if (strReportType == "Date Search")
                    {
                        ViewDateControls();
                        gblnZoneSearch = false;
                    }
                    else if (strReportType == "Zone and Date Search")
                    {
                        ViewDateControls();
                        ViewZoneControls();
                        gblnZoneSearch = true;
                    }
                    else if (strReportType == "Individual Record Search")
                    {
                        WorkOrderLookup WorkOrderLookup = new WorkOrderLookup();
                        WorkOrderLookup.ShowDialog();
                    }
                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Cancelled Orders // cbo Report Type Changed " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void HideZoneControls()
        {
            lblSelectZone.Visibility = Visibility.Hidden;
            cboZone.Visibility = Visibility.Hidden;
        }
        private void ViewZoneControls()
        {
            lblSelectZone.Visibility = Visibility.Visible;
            cboZone.Visibility = Visibility.Visible;
        }
        private bool LoadOrderStatusDataSet(int intWorkOrderID, string strWorkOrderNumber, string strAccountNumber, string strAddress, string strPhoneNumber, string strCity, DateTime datReceivedDate, string strStatus, string strZone)
        {
            bool blnFatalError = false;

            try
            {
                OrderStatusDataSet.orderstatusRow NewOrderRow = TheOrderStatusDataSet.orderstatus.NeworderstatusRow();

                NewOrderRow.AccountNumber = strAccountNumber;
                NewOrderRow.Address = strAddress;
                NewOrderRow.PhoneNumber = strPhoneNumber;
                NewOrderRow.Status = strStatus;
                NewOrderRow.WorkOrderID = intWorkOrderID;
                NewOrderRow.WorkOrderNumber = strWorkOrderNumber;
                NewOrderRow.Zone = strZone;
                NewOrderRow.City = strCity;
                NewOrderRow.DateReceived = datReceivedDate;

                TheOrderStatusDataSet.orderstatus.Rows.Add(NewOrderRow);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Closed Orders // Load Order Status Data Set " + Ex.Message);

                blnFatalError = true;
            }

            return blnFatalError;
        }
        private void HideTextControls()
        {
            lblStartDate.Visibility = Visibility.Hidden;
            txtStartDate.Visibility = Visibility.Hidden;
            lblEndDate.Visibility = Visibility.Hidden;
            txtEndDate.Visibility = Visibility.Hidden;
            btnFind.Visibility = Visibility.Hidden;
        }
        private void ViewDateControls()
        {
            lblStartDate.Visibility = Visibility.Visible;
            txtStartDate.Visibility = Visibility.Visible;
            lblEndDate.Visibility = Visibility.Visible;
            txtEndDate.Visibility = Visibility.Visible;
            btnFind.Visibility = Visibility.Visible;
            lblStartDate.Content = "Start Date";
        }

        private void cboZone_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variables
            int intSelectedIndex;
            int intCounter;
            int intNumberOfRecords;
            string strCity;
            DateTime datReceivedDate;
            int intWorkOrderID;
            string strWorkOrderNumber;
            string strAccountNumber;
            string strAddress;
            string strPhoneNumber;
            string strStatus;
            string strZone;
            bool blnFatalError;

            try
            {
                intSelectedIndex = cboZone.SelectedIndex - 1;
                TheOrderStatusDataSet.orderstatus.Rows.Clear();

                if (intSelectedIndex > -1)
                {
                    gintZoneID = TheFindWorkZonesDataSet.FindWorkZones[intSelectedIndex].ZoneID;
                    gstrZoneLocation = TheFindWorkZonesDataSet.FindWorkZones[intSelectedIndex].ZoneLocation;

                    TheFindWorkOrderByStatusTypeandZoneDataSet = TheWorkOrderClass.FindWorkOrderbyStatusTypeandZone(gintStatusID, gstrWorkType, gstrZoneLocation, gdatStartDate, gdatEndDate);
                    intNumberOfRecords = TheFindWorkOrderByStatusTypeandZoneDataSet.FindWorkOrderByStatusTypeandZone.Rows.Count - 1;

                    if (intNumberOfRecords > -1)
                    {
                        for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            intWorkOrderID = TheFindWorkOrderByStatusTypeandZoneDataSet.FindWorkOrderByStatusTypeandZone[intCounter].WorkOrderID;
                            datReceivedDate = TheFindWorkOrderByStatusTypeandZoneDataSet.FindWorkOrderByStatusTypeandZone[intCounter].DateReceived;
                            strAccountNumber = TheFindWorkOrderByStatusTypeandZoneDataSet.FindWorkOrderByStatusTypeandZone[intCounter].AccountNumber;
                            strAddress = TheFindWorkOrderByStatusTypeandZoneDataSet.FindWorkOrderByStatusTypeandZone[intCounter].StreetAddress;
                            strCity = TheFindWorkOrderByStatusTypeandZoneDataSet.FindWorkOrderByStatusTypeandZone[intCounter].City;
                            strPhoneNumber = TheFindWorkOrderByStatusTypeandZoneDataSet.FindWorkOrderByStatusTypeandZone[intCounter].PhoneNumber;
                            strStatus = TheFindWorkOrderByStatusTypeandZoneDataSet.FindWorkOrderByStatusTypeandZone[intCounter].WorkOrderStatus;
                            strWorkOrderNumber = TheFindWorkOrderByStatusTypeandZoneDataSet.FindWorkOrderByStatusTypeandZone[intCounter].WorkOrderNumber;
                            strZone = TheFindWorkOrderByStatusTypeandZoneDataSet.FindWorkOrderByStatusTypeandZone[intCounter].ZoneLocation;

                            blnFatalError = LoadOrderStatusDataSet(intWorkOrderID, strWorkOrderNumber, strAccountNumber, strAddress, strPhoneNumber, strCity, datReceivedDate, strStatus, strZone);

                            if (blnFatalError == true)
                                throw new Exception();
                        }

                    }

                    dgrResults.ItemsSource = TheOrderStatusDataSet.orderstatus;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Closed Orders // cbo Zone Change" + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
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
                cboWorkType.Items.Add("Select Work Type");

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboWorkType.Items.Add(TheFindWorkTypeSortedDataSet.FindWorkTypeSorted[intCounter].WorkType);
                }

                cboWorkType.SelectedIndex = 0;
                gdatEndDate = DateTime.Now;
                gdatEndDate = TheDateSearchClass.RemoveTime(gdatEndDate);
                gdatEndDate = TheDateSearchClass.AddingDays(gdatEndDate, 1);
                gdatStartDate = TheDateSearchClass.SubtractingDays(gdatEndDate, 365);

                cboReportType.Items.Add("Select Report Type");
                cboReportType.Items.Add("All Work Type Info");
                cboReportType.Items.Add("Zone");
                cboReportType.Items.Add("Date Search");
                cboReportType.Items.Add("Zone and Date Search");
                cboReportType.Items.Add("Individual Record Search");
                cboReportType.SelectedIndex = 0;

                TheFindWorkZonesDataSet = TheCustomersClass.FindWorkZones();

                intNumberOfRecords = TheFindWorkZonesDataSet.FindWorkZones.Rows.Count - 1;
                cboZone.Items.Add("Select Zone");

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboZone.Items.Add(TheFindWorkZonesDataSet.FindWorkZones[intCounter].ZoneLocation);
                }

                cboZone.SelectedIndex = 0;
                HideZoneControls();
                HideTextControls();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Closed Orders // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strValueForValidation;
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";
            int intCounter;
            int intNumberOfRecords;
            string strCity;
            DateTime datReceivedDate;
            int intWorkOrderID;
            string strWorkOrderNumber;
            string strAccountNumber;
            string strAddress;
            string strPhoneNumber;
            string strStatus;
            string strZone;

            try
            {
                TheOrderStatusDataSet.orderstatus.Rows.Clear();

                if (gblnTextSearch == true)
                {

                }
                else if (gblnTextSearch == false)
                {
                    strValueForValidation = txtStartDate.Text;
                    blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                    if (blnThereIsAProblem == true)
                    {
                        blnFatalError = true;
                        strErrorMessage += "Start Date is not a Date";
                    }
                    else
                    {
                        gdatStartDate = Convert.ToDateTime(strValueForValidation);
                    }
                    strValueForValidation = txtEndDate.Text;
                    blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                    if (blnThereIsAProblem == true)
                    {
                        blnFatalError = true;
                        strErrorMessage += "End Date is not a Date";
                    }
                    else
                    {
                        gdatEndDate = Convert.ToDateTime(strValueForValidation);
                    }
                    if (blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage(strErrorMessage);
                        return;
                    }
                    else
                    {
                        blnFatalError = TheDataValidationClass.verifyDateRange(gdatStartDate, gdatEndDate);

                        if (blnFatalError == true)
                        {
                            TheMessagesClass.ErrorMessage("The End Date is before the Start Date");
                            return;
                        }
                    }

                    if (gblnZoneSearch == false)
                    {
                        TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet = TheWorkOrderClass.FindWorkOrderByTypeAndStatusID(gstrWorkType, gintStatusID, gdatStartDate, gdatEndDate);

                        intNumberOfRecords = TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID.Rows.Count - 1;

                        if (intNumberOfRecords > -1)
                        {
                            for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                            {
                                intWorkOrderID = TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID[intCounter].WorkOrderID;
                                datReceivedDate = TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID[intCounter].DateReceived;
                                strAccountNumber = TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID[intCounter].AccountNumber;
                                strAddress = TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID[intCounter].StreetAddress;
                                strCity = TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID[intCounter].City;
                                strPhoneNumber = TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID[intCounter].PhoneNumber;
                                strStatus = TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID[intCounter].WorkOrderStatus;
                                strWorkOrderNumber = TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID[intCounter].WorkOrderNumber;
                                strZone = TheFindWorkOrderByWorkOrderTypeAndStatusIDDataSet.FindWorkOrderByWorkOrderTypeAndStatusID[intCounter].ZoneLocation;

                                blnFatalError = LoadOrderStatusDataSet(intWorkOrderID, strWorkOrderNumber, strAccountNumber, strAddress, strPhoneNumber, strCity, datReceivedDate, strStatus, strZone);

                                if (blnFatalError == true)
                                    throw new Exception();
                            }

                        }

                        dgrResults.ItemsSource = TheOrderStatusDataSet.orderstatus;
                    }
                    if (gblnZoneSearch == true)
                    {
                        TheFindWorkOrderByStatusTypeandZoneDataSet = TheWorkOrderClass.FindWorkOrderbyStatusTypeandZone(gintStatusID, gstrWorkType, gstrZoneLocation, gdatStartDate, gdatEndDate);
                        intNumberOfRecords = TheFindWorkOrderByStatusTypeandZoneDataSet.FindWorkOrderByStatusTypeandZone.Rows.Count - 1;

                        if (intNumberOfRecords > -1)
                        {
                            for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                            {
                                intWorkOrderID = TheFindWorkOrderByStatusTypeandZoneDataSet.FindWorkOrderByStatusTypeandZone[intCounter].WorkOrderID;
                                datReceivedDate = TheFindWorkOrderByStatusTypeandZoneDataSet.FindWorkOrderByStatusTypeandZone[intCounter].DateReceived;
                                strAccountNumber = TheFindWorkOrderByStatusTypeandZoneDataSet.FindWorkOrderByStatusTypeandZone[intCounter].AccountNumber;
                                strAddress = TheFindWorkOrderByStatusTypeandZoneDataSet.FindWorkOrderByStatusTypeandZone[intCounter].StreetAddress;
                                strCity = TheFindWorkOrderByStatusTypeandZoneDataSet.FindWorkOrderByStatusTypeandZone[intCounter].City;
                                strPhoneNumber = TheFindWorkOrderByStatusTypeandZoneDataSet.FindWorkOrderByStatusTypeandZone[intCounter].PhoneNumber;
                                strStatus = TheFindWorkOrderByStatusTypeandZoneDataSet.FindWorkOrderByStatusTypeandZone[intCounter].WorkOrderStatus;
                                strWorkOrderNumber = TheFindWorkOrderByStatusTypeandZoneDataSet.FindWorkOrderByStatusTypeandZone[intCounter].WorkOrderNumber;
                                strZone = TheFindWorkOrderByStatusTypeandZoneDataSet.FindWorkOrderByStatusTypeandZone[intCounter].ZoneLocation;

                                blnFatalError = LoadOrderStatusDataSet(intWorkOrderID, strWorkOrderNumber, strAccountNumber, strAddress, strPhoneNumber, strCity, datReceivedDate, strStatus, strZone);

                                if (blnFatalError == true)
                                    throw new Exception();
                            }

                        }

                        dgrResults.ItemsSource = TheOrderStatusDataSet.orderstatus;
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Closed Orders // Find Button");

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void dgrResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //this will grap a work order
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

                if (intSelectedIndex > -1)
                {
                    WorkGrid = dgrResults;
                    WorkRow = (DataGridRow)WorkGrid.ItemContainerGenerator.ContainerFromIndex(WorkGrid.SelectedIndex);
                    WorkID = (DataGridCell)WorkGrid.Columns[0].GetCellContent(WorkRow).Parent;
                    WorkOrderNumber = (DataGridCell)WorkGrid.Columns[1].GetCellContent(WorkRow).Parent;
                    strWorkOrderID = ((TextBlock)WorkID.Content).Text;

                    MainWindow.gintWorkOrderID = Convert.ToInt32(strWorkOrderID);
                    MainWindow.gstrWorkOrderNumber = ((TextBlock)WorkOrderNumber.Content).Text;

                    ViewWorkOrder ViewWorkOrder = new ViewWorkOrder();
                    ViewWorkOrder.ShowDialog();
                }

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Closed Orders // Work Selection Change " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
