/* Title:           Update Work Order
 * Date:            96-20-17
 * Author:          Terry Holmes
 * 
 * Description:     This form is used to Update Work Orders */

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
using CustomersDLL;
using WorkOrderDLL;
using WorkTypeDLL;
using DataValidationDLL;
using DateSearchDLL;
using WorkOrderScheduleDLL;

namespace MDUDropBury
{
    /// <summary>
    /// Interaction logic for UpdateWorkOrder.xaml
    /// </summary>
    public partial class UpdateWorkOrder : Window
    {
        //setting up the class
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        CustomersClass TheCustomersClass = new CustomersClass();
        WorkOrderClass TheWorkOrderClass = new WorkOrderClass();
        WorkTypeClass TheWorkTypeClass = new WorkTypeClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        WorkOrderScheduleClass TheWorkOrderScheduleClass = new WorkOrderScheduleClass();
        MDULettersClass TheMDULettersClass = new MDULettersClass();

        FindAddressByAddressesDataSet TheFindAddressByAddressesDataSet = new FindAddressByAddressesDataSet();
        FindCustomersByAddressIDDataSet TheFindCustomersByAddressIDDataSet = new FindCustomersByAddressIDDataSet();
        FindWorkOrderByAddressIDDataSet TheFindWorkOrderByAddressIDDataSet = new FindWorkOrderByAddressIDDataSet();
        FindActiveCustomerByAccountNumberDataSet TheFindActiveCustomerByAccountNumberDataSet = new FindActiveCustomerByAccountNumberDataSet();
        FindWorkOrderByWorkOrderNumberDataSet TheFindWorkOrderByWorkOrderNumberDataSet = new FindWorkOrderByWorkOrderNumberDataSet();
        FindScheduledWorkOrdersByWorkOrderNumberDataSet TheFindScheduledWorkOrdersByworkOrderNumberDataSet = new FindScheduledWorkOrdersByWorkOrderNumberDataSet();
        FindCustomerByCustomerIDDataSet TheFindCustomerByCustomerIDataSet = new FindCustomerByCustomerIDDataSet();
        FindWorkOrderStatusSortedDataSet TheFindWorkOrderStatusSortedDataSet = new FindWorkOrderStatusSortedDataSet();
        FindWorkZonesDataSet TheFindWorkZonesDataSet = new FindWorkZonesDataSet();
        FindAddressByAddressIDDataSet TheFindAddressByAddressIDDataSet = new FindAddressByAddressIDDataSet();
        FindWorkOrderUpdatesByWorkOrderIDDataSet TheFindWorkOrderUpdatesByWorkOrderIDDataSet = new FindWorkOrderUpdatesByWorkOrderIDDataSet();
       
        WorkOrderForSelectionDataSet TheWorkOrderForSelectionDataSet = new WorkOrderForSelectionDataSet();

        ViewWorkOrderUpdates ViewWorkOrderUpdates = new ViewWorkOrderUpdates();
        ShowWorkScheduled ShowWorkScheduled = new ShowWorkScheduled();

        //setting global variables
        int gintStatusID;
        int gintZoneID;
        string gstrState;

        public UpdateWorkOrder()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            ViewWorkOrderUpdates.Close();
            ShowWorkScheduled.Close();

            MainMenu MainMenu = new MainMenu();
            MainMenu.Show();
            Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void txtEnterInformation_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting local variables
            string strAddress;
            int intAddressLength;
            int intAddressCounter;
            int intAddressNumberOfRecords;
            int intWorkOrderCounter;
            int intWorkOrderNumberOfRecords;
            string strAccountNumber;

            try
            {
                strAddress = txtEnterInformation.Text;
                intAddressLength = strAddress.Length;
                ResetControls();
                TheWorkOrderForSelectionDataSet.workorders.Rows.Clear();
                dgrResults.Visibility = Visibility.Visible;

                if (intAddressLength > 4)
                {
                    TheFindAddressByAddressesDataSet = TheCustomersClass.FindAddressesByAddress(strAddress);

                    intAddressNumberOfRecords = TheFindAddressByAddressesDataSet.FindAddressesByAddress.Rows.Count - 1;

                    if(intAddressNumberOfRecords > -1)
                    {
                        for(intAddressCounter = 0; intAddressCounter <= intAddressNumberOfRecords; intAddressCounter++)
                        {
                            MainWindow.gintAddressID = TheFindAddressByAddressesDataSet.FindAddressesByAddress[intAddressCounter].AddressID;
                            
                            TheFindWorkOrderByAddressIDDataSet = TheWorkOrderClass.FindWorkOrderByAddressID(MainWindow.gintAddressID);

                            intWorkOrderNumberOfRecords = TheFindWorkOrderByAddressIDDataSet.FindWorkOrderByAddressID.Rows.Count - 1;

                            if(intWorkOrderNumberOfRecords > -1)
                            {
                                for(intWorkOrderCounter = 0; intWorkOrderCounter <= intWorkOrderNumberOfRecords; intWorkOrderCounter++)
                                {
                                    strAccountNumber = TheFindWorkOrderByAddressIDDataSet.FindWorkOrderByAddressID[intWorkOrderCounter].AccountNumber;

                                    TheFindActiveCustomerByAccountNumberDataSet = TheCustomersClass.FindActiveCustomerByAccountNumber(strAccountNumber);

                                    WorkOrderForSelectionDataSet.workordersRow NewWorkRow = TheWorkOrderForSelectionDataSet.workorders.NewworkordersRow();

                                    NewWorkRow.AccountNumber = strAccountNumber;
                                    NewWorkRow.AddressID = MainWindow.gintAddressID;
                                    NewWorkRow.City = TheFindWorkOrderByAddressIDDataSet.FindWorkOrderByAddressID[intWorkOrderCounter].City;
                                    NewWorkRow.CustomerID = TheFindActiveCustomerByAccountNumberDataSet.FindActiveCustomerByAccountNumber[0].CustomerID;
                                    NewWorkRow.DateScheduled = TheFindWorkOrderByAddressIDDataSet.FindWorkOrderByAddressID[intWorkOrderCounter].DateScheduled;
                                    NewWorkRow.FirstName = TheFindActiveCustomerByAccountNumberDataSet.FindActiveCustomerByAccountNumber[0].FirstName;
                                    NewWorkRow.LastName = TheFindActiveCustomerByAccountNumberDataSet.FindActiveCustomerByAccountNumber[0].LastName;
                                    NewWorkRow.PhoneNumber = TheFindActiveCustomerByAccountNumberDataSet.FindActiveCustomerByAccountNumber[0].PhoneNumber;
                                    NewWorkRow.WorkOrderNumber = TheFindWorkOrderByAddressIDDataSet.FindWorkOrderByAddressID[intWorkOrderCounter].WorkOrderNumber;
                                    NewWorkRow.WorkOrderStatus = TheFindWorkOrderByAddressIDDataSet.FindWorkOrderByAddressID[intWorkOrderCounter].WorkOrderStatus;
                                    NewWorkRow.WorkType = TheFindWorkOrderByAddressIDDataSet.FindWorkOrderByAddressID[intWorkOrderCounter].WorkType;
                                    NewWorkRow.ZoneLocation = TheFindWorkOrderByAddressIDDataSet.FindWorkOrderByAddressID[intWorkOrderCounter].ZoneLocation;
                                    NewWorkRow.StreetAddress = TheFindWorkOrderByAddressIDDataSet.FindWorkOrderByAddressID[intWorkOrderCounter].StreetAddress;
                                    NewWorkRow.WorkOrderID = TheFindWorkOrderByAddressIDDataSet.FindWorkOrderByAddressID[intWorkOrderCounter].WorkOrderID;

                                    TheWorkOrderForSelectionDataSet.workorders.Rows.Add(NewWorkRow);
                                }
                            }
                        }
                    }

                    dgrResults.ItemsSource = TheWorkOrderForSelectionDataSet.workorders;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Update Work Order // Enter Information Text Box " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            
        }

        private void dgrResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid WorkOrderGrid;
            DataGridRow WorkOrderRow;
            DataGridCell WorkOrderNumber;
            DataGridCell AddressID;
            DataGridCell CustomerID;
            DataGridCell WorkOrderID;
            string strWorkOrderNumber;
            string strWorkOrderID;
            string strAddressID;
            string strCustomerID;
            int intRecordsReturned;
            int intSelectedID;

            try
            {
                intSelectedID = dgrResults.SelectedIndex;

                if(intSelectedID > -1)
                {
                    WorkOrderGrid = dgrResults;
                    WorkOrderRow = (DataGridRow)WorkOrderGrid.ItemContainerGenerator.ContainerFromIndex(WorkOrderGrid.SelectedIndex);
                    WorkOrderNumber = (DataGridCell)WorkOrderGrid.Columns[1].GetCellContent(WorkOrderRow).Parent;
                    AddressID = (DataGridCell)WorkOrderGrid.Columns[12].GetCellContent(WorkOrderRow).Parent;
                    CustomerID = (DataGridCell)WorkOrderGrid.Columns[13].GetCellContent(WorkOrderRow).Parent;
                    WorkOrderID = (DataGridCell)WorkOrderGrid.Columns[14].GetCellContent(WorkOrderRow).Parent;
                    strWorkOrderNumber = ((TextBlock)WorkOrderNumber.Content).Text;
                    strWorkOrderID = ((TextBlock)WorkOrderID.Content).Text;
                    strAddressID = ((TextBlock)AddressID.Content).Text;
                    MainWindow.gintAddressID = Convert.ToInt32(strAddressID);
                    strCustomerID = ((TextBlock)CustomerID.Content).Text;
                    MainWindow.gintCustomerID = Convert.ToInt32(strCustomerID);

                    SetControlVisible();

                    MainWindow.gintWorkOrderID = Convert.ToInt32(strWorkOrderID);

                    TheFindScheduledWorkOrdersByworkOrderNumberDataSet = TheWorkOrderScheduleClass.FindWorkOrderScheduledByWorkOrderNumber(strWorkOrderNumber);

                    intRecordsReturned = TheFindScheduledWorkOrdersByworkOrderNumberDataSet.FindScheduledWorkOrdersByWorkOrderNumber.Rows.Count;

                    if (intRecordsReturned == 0)
                    {
                        TheFindWorkOrderByWorkOrderNumberDataSet = TheWorkOrderClass.FindWorkOrderByWorkOrderNumber(strWorkOrderNumber);

                        intRecordsReturned = TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber.Rows.Count;

                        if (intRecordsReturned == 1)
                        {
                            TheFindCustomerByCustomerIDataSet = TheCustomersClass.FindCustomerByCustomerID(MainWindow.gintCustomerID);

                            SetZoneComboBox(TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].ZoneLocation);
                            SetWorkStatusComboBox(TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].WorkOrderStatus);
                            txtAccountNumber.Text = TheFindCustomerByCustomerIDataSet.FindCustomerByCustomerID[0].AccountNumber;
                            txtAddress.Text = TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].StreetAddress;
                            txtCity.Text = TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].City;
                            txtDateScheduled.Text = Convert.ToString(TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].DateScheduled);
                            txtFirstName.Text = TheFindCustomerByCustomerIDataSet.FindCustomerByCustomerID[0].FirstName;
                            txtLastName.Text = TheFindCustomerByCustomerIDataSet.FindCustomerByCustomerID[0].LastName;
                            txtNotes.Text = "";
                            txtPhoneNumber.Text = TheFindCustomerByCustomerIDataSet.FindCustomerByCustomerID[0].PhoneNumber;
                            txtState.Text = TheFindCustomerByCustomerIDataSet.FindCustomerByCustomerID[0].CustomerState;
                            txtWorkOrderNumber.Text = TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].WorkOrderNumber;
                            txtWorkType.Text = TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].WorkType;
                            txtStartTime.Text = "";
                            txtEndTime.Text = "";
                            gstrState = TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].CustomerState;
                        }
                    }
                    else
                    {
                        //this will load up the controls
                        txtAccountNumber.Text = TheFindScheduledWorkOrdersByworkOrderNumberDataSet.FindScheduledWorkOrdersByWorkOrderNumber[0].AccountNumber;
                        txtAddress.Text = TheFindScheduledWorkOrdersByworkOrderNumberDataSet.FindScheduledWorkOrdersByWorkOrderNumber[0].StreetAddress;
                        txtCity.Text = TheFindScheduledWorkOrdersByworkOrderNumberDataSet.FindScheduledWorkOrdersByWorkOrderNumber[0].City;
                        txtDateScheduled.Text = Convert.ToString(TheFindScheduledWorkOrdersByworkOrderNumberDataSet.FindScheduledWorkOrdersByWorkOrderNumber[0].ScheduleDate);
                        txtEndTime.Text = Convert.ToString(TheFindScheduledWorkOrdersByworkOrderNumberDataSet.FindScheduledWorkOrdersByWorkOrderNumber[0].EndTime);
                        txtFirstName.Text = TheFindScheduledWorkOrdersByworkOrderNumberDataSet.FindScheduledWorkOrdersByWorkOrderNumber[0].FirstName;
                        txtLastName.Text = TheFindScheduledWorkOrdersByworkOrderNumberDataSet.FindScheduledWorkOrdersByWorkOrderNumber[0].LastName;
                        txtNotes.Text = "";
                        txtPhoneNumber.Text = TheFindScheduledWorkOrdersByworkOrderNumberDataSet.FindScheduledWorkOrdersByWorkOrderNumber[0].PhoneNumber;
                        txtStartTime.Text = Convert.ToString(TheFindScheduledWorkOrdersByworkOrderNumberDataSet.FindScheduledWorkOrdersByWorkOrderNumber[0].StartTime);
                        txtState.Text = TheFindScheduledWorkOrdersByworkOrderNumberDataSet.FindScheduledWorkOrdersByWorkOrderNumber[0].CustomerState;
                        txtWorkOrderNumber.Text = TheFindScheduledWorkOrdersByworkOrderNumberDataSet.FindScheduledWorkOrdersByWorkOrderNumber[0].WorkOrderNumber;
                        SetWorkStatusComboBox(TheFindScheduledWorkOrdersByworkOrderNumberDataSet.FindScheduledWorkOrdersByWorkOrderNumber[0].WorkOrderStatus);
                        SetZoneComboBox(TheFindScheduledWorkOrdersByworkOrderNumberDataSet.FindScheduledWorkOrdersByWorkOrderNumber[0].ZoneLocation);
                        txtWorkType.Text = TheFindScheduledWorkOrdersByworkOrderNumberDataSet.FindScheduledWorkOrdersByWorkOrderNumber[0].WorkType;
                        gstrState = TheFindScheduledWorkOrdersByworkOrderNumberDataSet.FindScheduledWorkOrdersByWorkOrderNumber[0].CustomerState;
                    }
                }

               
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury //Update Work Order // Results Selection Change " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void SetZoneComboBox(string strZone)
        {
            int intCounter;
            int intNumberOfRecords;

            intNumberOfRecords = TheFindWorkZonesDataSet.FindWorkZones.Rows.Count - 1;

            for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                if (strZone == TheFindWorkZonesDataSet.FindWorkZones[intCounter].ZoneLocation)
                {
                    cboZone.SelectedIndex = intCounter + 1;
                }
            }
        }
        private void SetWorkStatusComboBox(string strStatus)
        {
            int intCounter;
            int intNumberOfRecords;

            intNumberOfRecords = TheFindWorkOrderStatusSortedDataSet.FindWorkOrderStatusSorted.Rows.Count - 1;

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                if(strStatus == TheFindWorkOrderStatusSortedDataSet.FindWorkOrderStatusSorted[intCounter].WorkOrderStatus)
                {
                    cboWorkOrderStatus.SelectedIndex = intCounter + 1;
                }
            }
        }
        private void SetControlVisible()
        {
            txtAccountNumber.Visibility = Visibility.Visible;
            txtAddress.Visibility = Visibility.Visible;
            txtCity.Visibility = Visibility.Visible;
            txtDateScheduled.Visibility = Visibility.Visible;
            txtFirstName.Visibility = Visibility.Visible;
            txtLastName.Visibility = Visibility.Visible;
            txtNotes.Visibility = Visibility.Visible;
            txtPhoneNumber.Visibility = Visibility.Visible;
            txtState.Visibility = Visibility.Visible;
            txtWorkOrderNumber.Visibility = Visibility.Visible;
            txtWorkType.Visibility = Visibility.Visible;
            txtStartTime.Visibility = Visibility.Visible;
            txtEndTime.Visibility = Visibility.Visible;
            cboWorkOrderStatus.Visibility = Visibility.Visible;
            cboZone.Visibility = Visibility.Visible;
        }
        private void SetControlsInvisible()
        {
            txtAccountNumber.Visibility = Visibility.Hidden;
            txtAddress.Visibility = Visibility.Hidden;
            txtCity.Visibility = Visibility.Hidden;
            txtDateScheduled.Visibility = Visibility.Hidden;
            txtFirstName.Visibility = Visibility.Hidden;
            txtLastName.Visibility = Visibility.Hidden;
            txtNotes.Visibility = Visibility.Hidden;
            txtPhoneNumber.Visibility = Visibility.Hidden;
            txtState.Visibility = Visibility.Hidden;
            txtWorkOrderNumber.Visibility = Visibility.Hidden;
            txtWorkType.Visibility = Visibility.Hidden;
            txtStartTime.Visibility = Visibility.Hidden;
            txtEndTime.Visibility = Visibility.Hidden;
            cboWorkOrderStatus.Visibility = Visibility.Hidden;
            cboZone.Visibility = Visibility.Hidden;
        }
        private void ResetControls()
        {
            txtAccountNumber.Text = "";
            txtAddress.Text = "";
            txtCity.Text = "";
            txtDateScheduled.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtNotes.Text = "";
            txtPhoneNumber.Text = "";
            txtState.Text = "";
            txtWorkOrderNumber.Text = "";
            txtWorkType.Text = "";
            txtStartTime.Text = "";
            txtEndTime.Text = "";
            cboWorkOrderStatus.SelectedIndex = 0;
            cboZone.SelectedIndex = 0;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this will set up the controls
            //setting local variables
            int intCounter;
            int intNumberOfRecords;

            try
            {
                
                ViewWorkOrderUpdates.Show();
                ShowWorkScheduled.Show();

                cboZone.Items.Add("Select Zone");
                cboWorkOrderStatus.Items.Add("Select Order Status");

                //loading work order status
                TheFindWorkOrderStatusSortedDataSet = TheWorkOrderClass.FindWorkOrderStatusSorted();

                intNumberOfRecords = TheFindWorkOrderStatusSortedDataSet.FindWorkOrderStatusSorted.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboWorkOrderStatus.Items.Add(TheFindWorkOrderStatusSortedDataSet.FindWorkOrderStatusSorted[intCounter].WorkOrderStatus);
                }

                TheFindWorkZonesDataSet = TheCustomersClass.FindWorkZones();

                intNumberOfRecords = TheFindWorkZonesDataSet.FindWorkZones.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboZone.Items.Add(TheFindWorkZonesDataSet.FindWorkZones[intCounter].ZoneLocation);
                }

                cboZone.SelectedIndex = 0;
                cboWorkOrderStatus.SelectedIndex = 0;
                SetControlsInvisible();

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Update Work Order // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            //this will load up controls.
            string strWorkOrderNumber;
            int intRecordsReturned;
            string strAccountNumber;
            int intAddressNumberOfRecords;
            int intWorkOrderCounter;
            int intWorkOrderNumberOfRecords;

            try
            {
                //validating
                strWorkOrderNumber = txtEnterInformation.Text;
                if(strWorkOrderNumber == "")
                {
                    TheMessagesClass.ErrorMessage("Inforamtion Was Not Entered");
                    return;
                }

                TheFindWorkOrderByWorkOrderNumberDataSet = TheWorkOrderClass.FindWorkOrderByWorkOrderNumber(strWorkOrderNumber);

                intRecordsReturned = TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber.Rows.Count;

                if(intRecordsReturned == 0)
                {
                    strAccountNumber = strWorkOrderNumber;
                    TheFindActiveCustomerByAccountNumberDataSet = TheCustomersClass.FindActiveCustomerByAccountNumber(strAccountNumber);

                    intRecordsReturned = TheFindActiveCustomerByAccountNumberDataSet.FindActiveCustomerByAccountNumber.Rows.Count;

                    if(intRecordsReturned == 0)
                    {
                        TheMessagesClass.ErrorMessage("No Records Found For the Information Entered");
                        return;
                    }
                    else
                    {
                        MainWindow.gintAddressID = TheFindActiveCustomerByAccountNumberDataSet.FindActiveCustomerByAccountNumber[0].AddressID;

                        TheFindWorkOrderByAddressIDDataSet = TheWorkOrderClass.FindWorkOrderByAddressID(MainWindow.gintAddressID);

                        intRecordsReturned = TheFindWorkOrderByAddressIDDataSet.FindWorkOrderByAddressID.Rows.Count;

                        if(intRecordsReturned == 0)
                        {
                            TheMessagesClass.ErrorMessage("No Records Found For the Information Entered");
                            return;
                        }
                        else
                        {
                            intAddressNumberOfRecords = intRecordsReturned - 1;
                                                      
                            TheFindWorkOrderByAddressIDDataSet = TheWorkOrderClass.FindWorkOrderByAddressID(MainWindow.gintAddressID);

                            intWorkOrderNumberOfRecords = TheFindWorkOrderByAddressIDDataSet.FindWorkOrderByAddressID.Rows.Count - 1;

                            if (intWorkOrderNumberOfRecords > -1)
                            {

                                for (intWorkOrderCounter = 0; intWorkOrderCounter <= intWorkOrderNumberOfRecords; intWorkOrderCounter++)
                                {
                                    strAccountNumber = TheFindWorkOrderByAddressIDDataSet.FindWorkOrderByAddressID[intWorkOrderCounter].AccountNumber;

                                    TheFindActiveCustomerByAccountNumberDataSet = TheCustomersClass.FindActiveCustomerByAccountNumber(strAccountNumber);

                                    WorkOrderForSelectionDataSet.workordersRow NewWorkRow = TheWorkOrderForSelectionDataSet.workorders.NewworkordersRow();

                                    NewWorkRow.AccountNumber = strAccountNumber;
                                    NewWorkRow.AddressID = MainWindow.gintAddressID;
                                    NewWorkRow.City = TheFindWorkOrderByAddressIDDataSet.FindWorkOrderByAddressID[intWorkOrderCounter].City;
                                    NewWorkRow.CustomerID = TheFindActiveCustomerByAccountNumberDataSet.FindActiveCustomerByAccountNumber[0].CustomerID;
                                    NewWorkRow.DateScheduled = TheFindWorkOrderByAddressIDDataSet.FindWorkOrderByAddressID[intWorkOrderCounter].DateScheduled;
                                    NewWorkRow.FirstName = TheFindActiveCustomerByAccountNumberDataSet.FindActiveCustomerByAccountNumber[0].FirstName;
                                    NewWorkRow.LastName = TheFindActiveCustomerByAccountNumberDataSet.FindActiveCustomerByAccountNumber[0].LastName;
                                    NewWorkRow.PhoneNumber = TheFindActiveCustomerByAccountNumberDataSet.FindActiveCustomerByAccountNumber[0].PhoneNumber;
                                    NewWorkRow.WorkOrderNumber = TheFindWorkOrderByAddressIDDataSet.FindWorkOrderByAddressID[intWorkOrderCounter].WorkOrderNumber;
                                    NewWorkRow.WorkOrderStatus = TheFindWorkOrderByAddressIDDataSet.FindWorkOrderByAddressID[intWorkOrderCounter].WorkOrderStatus;
                                    NewWorkRow.WorkType = TheFindWorkOrderByAddressIDDataSet.FindWorkOrderByAddressID[intWorkOrderCounter].WorkType;
                                    NewWorkRow.ZoneLocation = TheFindWorkOrderByAddressIDDataSet.FindWorkOrderByAddressID[intWorkOrderCounter].ZoneLocation;
                                    NewWorkRow.StreetAddress = TheFindWorkOrderByAddressIDDataSet.FindWorkOrderByAddressID[intWorkOrderCounter].StreetAddress;
                                    NewWorkRow.WorkOrderID = TheFindWorkOrderByAddressIDDataSet.FindWorkOrderByAddressID[intWorkOrderCounter].WorkOrderID;

                                    TheWorkOrderForSelectionDataSet.workorders.Rows.Add(NewWorkRow);
                                }
                            }
                        }
                    }
                }
                else
                {
                    dgrResults.Visibility = Visibility.Hidden;
                    SetControlVisible();
                    MainWindow.gintWorkOrderID = TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].WorkOrderID;
                    strAccountNumber = TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].AccountNumber;
                    TheFindActiveCustomerByAccountNumberDataSet = TheCustomersClass.FindActiveCustomerByAccountNumber(strAccountNumber);
                    MainWindow.gintCustomerID = TheFindActiveCustomerByAccountNumberDataSet.FindActiveCustomerByAccountNumber[0].CustomerID;
                    MainWindow.gintAddressID = TheFindActiveCustomerByAccountNumberDataSet.FindActiveCustomerByAccountNumber[0].AddressID;
                    
                    //Checking to see if the work order has been previously scheduled                    
                    TheFindScheduledWorkOrdersByworkOrderNumberDataSet = TheWorkOrderScheduleClass.FindWorkOrderScheduledByWorkOrderNumber(strWorkOrderNumber);

                    intRecordsReturned = TheFindScheduledWorkOrdersByworkOrderNumberDataSet.FindScheduledWorkOrdersByWorkOrderNumber.Rows.Count;

                    txtAccountNumber.Text = strAccountNumber;
                    txtAddress.Text = TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].StreetAddress;
                    txtCity.Text = TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].City;
                    txtDateScheduled.Text = Convert.ToString(TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].DateScheduled);                    
                    txtFirstName.Text = TheFindActiveCustomerByAccountNumberDataSet.FindActiveCustomerByAccountNumber[0].FirstName;
                    txtLastName.Text = TheFindActiveCustomerByAccountNumberDataSet.FindActiveCustomerByAccountNumber[0].LastName;
                    txtNotes.Text = "";
                    txtPhoneNumber.Text = TheFindActiveCustomerByAccountNumberDataSet.FindActiveCustomerByAccountNumber[0].PhoneNumber;                    
                    txtState.Text = TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].CustomerState;
                    txtWorkOrderNumber.Text = TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].WorkOrderNumber;
                    txtWorkType.Text = TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].WorkType;
                    SetWorkStatusComboBox(TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].WorkOrderStatus);
                    SetZoneComboBox(TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].ZoneLocation);

                    if (intRecordsReturned > 0)
                    {
                        txtStartTime.Text = Convert.ToString(TheFindScheduledWorkOrdersByworkOrderNumberDataSet.FindScheduledWorkOrdersByWorkOrderNumber[0].StartTime);
                        txtEndTime.Text = Convert.ToString(TheFindScheduledWorkOrdersByworkOrderNumberDataSet.FindScheduledWorkOrdersByWorkOrderNumber[0].EndTime);
                    }
                   
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Update Work Orders // Search Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            TimeSpan timStartTime = TimeSpan.Zero;
            TimeSpan timEndTime = TimeSpan.Zero;
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";
            DateTime datScheduledDate = DateTime.Now;
            string strAddress;
            string strCity;
            string strState;
            string strNotes;
            string strPhoneNumber;
            string strValueForValidation;
            int intRecordsReturned;
            int intEmployeeID;

            try
            {
                strAddress = txtAddress.Text;
                if(strAddress == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "Address Was Not Entered\n";
                }
                strCity = txtCity.Text;
                if(strCity == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "City Was Not Entered\n";
                }
                strState = txtState.Text;
                if(strState == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "State Was Not Entered\n";
                }
                strPhoneNumber = txtPhoneNumber.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyPhoneNumberFormat(strPhoneNumber);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "Phone Number Was Not Correct\n";
                }
                strValueForValidation = txtDateScheduled.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Date Scheduled is not a Date\n";
                }
                else
                {
                    datScheduledDate = Convert.ToDateTime(strValueForValidation);
                }
                strValueForValidation = txtStartTime.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyTimeSpanInfo(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Start Time is not a Time\n";
                }
                else
                {
                    TimeSpan.TryParse(strValueForValidation, out timStartTime);
                }
                strValueForValidation = txtEndTime.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyTimeSpanInfo(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The End Time is not a time\n";
                }
                else
                {
                    TimeSpan.TryParse(strValueForValidation, out timEndTime);
                }
                if(timStartTime > timEndTime)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Start Time is after the End Time\n";
                }
                if(cboWorkOrderStatus.SelectedIndex == 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "Work Order Status Was Not Selected\n";
                }
                if(cboZone.SelectedIndex == 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Zone was not Selected\n";
                }
                strNotes = txtNotes.Text;
                if(strNotes == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "There Are No Notes Entered\n";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                //checking address
                TheFindAddressByAddressesDataSet = TheCustomersClass.FindAddressesByAddress(strAddress);

                intRecordsReturned = TheFindAddressByAddressesDataSet.FindAddressesByAddress.Rows.Count;

                if(intRecordsReturned == 0)
                {
                    TheFindAddressByAddressIDDataSet = TheCustomersClass.FindAddressByAddressID(MainWindow.gintAddressID);

                    blnFatalError = TheCustomersClass.UpdateCustomerAddress(MainWindow.gintAddressID, strAddress, strCity, strCity, gintZoneID, TheFindAddressByAddressIDDataSet.FindAddressByAddressID[0].ZipCode);

                    if (blnFatalError == true)
                        throw new Exception();
                }

                blnFatalError = TheWorkOrderClass.UpdateWorkOrderScheduleDate(MainWindow.gintWorkOrderID, datScheduledDate);

                if(blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheWorkOrderClass.UpdateWorkOrderStatusID(MainWindow.gintWorkOrderID, gintStatusID);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheWorkOrderClass.InsertWorkOrderUpdate(MainWindow.gintWorkOrderID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, strNotes);

                if (blnFatalError == true)
                    throw new Exception();

                TheFindScheduledWorkOrdersByworkOrderNumberDataSet = TheWorkOrderScheduleClass.FindWorkOrderScheduledByWorkOrderNumber(txtWorkOrderNumber.Text);

                intRecordsReturned = TheFindScheduledWorkOrdersByworkOrderNumberDataSet.FindScheduledWorkOrdersByWorkOrderNumber.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    intEmployeeID = TheFindScheduledWorkOrdersByworkOrderNumberDataSet.FindScheduledWorkOrdersByWorkOrderNumber[0].EmployeeID;

                    blnFatalError = TheWorkOrderScheduleClass.UpdateWorkOrderScheduled(MainWindow.gintWorkOrderID, datScheduledDate, timStartTime, timEndTime, intEmployeeID);
                }
                else
                {
                    blnFatalError = TheWorkOrderScheduleClass.InsertWorkOrderScheduled(MainWindow.gintWorkOrderID, MainWindow.gintCustomerID, datScheduledDate, timStartTime, timEndTime, -1);
                }

                if (blnFatalError == true)
                    throw new Exception();

                //ResetControls();
                //SetControlVisible();

                TheMessagesClass.InformationMessage("The Work Order Has Been Updated");

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Update Work Order // Update Button" + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
           
        }

        private void cboWorkOrderStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboWorkOrderStatus.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                gintStatusID = TheFindWorkOrderStatusSortedDataSet.FindWorkOrderStatusSorted[intSelectedIndex].StatusID;
            }
        }

        private void cboZone_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboZone.SelectedIndex - 1;

            if(intSelectedIndex > -1)
            {
                gintZoneID = TheFindWorkZonesDataSet.FindWorkZones[intSelectedIndex].ZoneID;
            }
            
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            //this will print the report
            int intCounter;
            int intNumberOfRecords;
            string strTransactionDate;
            string strFirstName;
            string strLastName;
            string strNotes;
            int intCurrentRow = 0;


            try
            {
                PrintDialog pdWorkOrder = new PrintDialog();

                if (pdWorkOrder.ShowDialog().Value)
                {
                    FlowDocument fdWorkOrder = new FlowDocument();
                    Paragraph Title = new Paragraph(new Run("Blue Jay Communications Work Order: " + txtWorkOrderNumber.Text));
                    Title.TextAlignment = TextAlignment.Center;
                    fdWorkOrder.Blocks.Add(Title);
                    fdWorkOrder.TextAlignment = TextAlignment.Left;
                    fdWorkOrder.ColumnWidth = 1000;
                    Thickness thickness = new Thickness(50, 50, 50, 50);
                    fdWorkOrder.PagePadding = thickness;
                    Paragraph parHeadingSpace = new Paragraph(new Run());
                    fdWorkOrder.Blocks.Add(parHeadingSpace);

                    Table WorkOrderTable = new Table();
                    fdWorkOrder.Blocks.Add(WorkOrderTable);
                    WorkOrderTable.CellSpacing = 5;

                    WorkOrderTable.Columns.Add(new TableColumn());
                    WorkOrderTable.Columns.Add(new TableColumn());
                    WorkOrderTable.Columns.Add(new TableColumn());
                    WorkOrderTable.Columns.Add(new TableColumn());

                    WorkOrderTable.RowGroups.Add(new TableRowGroup());
                    WorkOrderTable.RowGroups[0].Rows.Add(new TableRow());

                    TableRow newTableRow = WorkOrderTable.RowGroups[0].Rows[intCurrentRow];

                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Account Number:"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(txtAccountNumber.Text))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Date Scheduled"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(txtDateScheduled.Text))));

                    intCurrentRow++;

                    WorkOrderTable.RowGroups.Add(new TableRowGroup());
                    WorkOrderTable.RowGroups[0].Rows.Add(new TableRow());

                    newTableRow = WorkOrderTable.RowGroups[0].Rows[intCurrentRow];

                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Address:"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(txtAddress.Text))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Start Time"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(txtStartTime.Text))));

                    intCurrentRow++;

                    WorkOrderTable.RowGroups.Add(new TableRowGroup());
                    WorkOrderTable.RowGroups[0].Rows.Add(new TableRow());

                    newTableRow = WorkOrderTable.RowGroups[0].Rows[intCurrentRow];

                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("City:"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(txtCity.Text + ", " + gstrState))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("End Time"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(txtEndTime.Text))));

                    intCurrentRow++;

                    WorkOrderTable.RowGroups.Add(new TableRowGroup());
                    WorkOrderTable.RowGroups[0].Rows.Add(new TableRow());

                    newTableRow = WorkOrderTable.RowGroups[0].Rows[intCurrentRow];

                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Name:"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(txtFirstName.Text + " " + txtLastName.Text))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Work Type"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(txtWorkType.Text))));

                    intCurrentRow++;

                    WorkOrderTable.RowGroups.Add(new TableRowGroup());
                    WorkOrderTable.RowGroups[0].Rows.Add(new TableRow());

                    newTableRow = WorkOrderTable.RowGroups[0].Rows[intCurrentRow];

                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Phone Number:"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(txtPhoneNumber.Text))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Zone"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(cboZone.SelectedItem.ToString()))));

                    Paragraph parHeader = new Paragraph(new Run("\t\t\t\t\tWork Order Notes"));
                    fdWorkOrder.Blocks.Add(parHeader);
                    TheFindWorkOrderUpdatesByWorkOrderIDDataSet = TheWorkOrderClass.FindWorkOrderUpdatesByWorkOrderID(MainWindow.gintWorkOrderID);

                    intNumberOfRecords = TheFindWorkOrderUpdatesByWorkOrderIDDataSet.FindWorkOrderUpdateByWorkOrderID.Rows.Count - 1;

                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        strTransactionDate = Convert.ToString(TheFindWorkOrderUpdatesByWorkOrderIDDataSet.FindWorkOrderUpdateByWorkOrderID[intCounter].TransactionDate);
                        strFirstName = TheFindWorkOrderUpdatesByWorkOrderIDDataSet.FindWorkOrderUpdateByWorkOrderID[intCounter].FirstName;
                        strLastName = TheFindWorkOrderUpdatesByWorkOrderIDDataSet.FindWorkOrderUpdateByWorkOrderID[intCounter].LastName;
                        strNotes = TheFindWorkOrderUpdatesByWorkOrderIDDataSet.FindWorkOrderUpdateByWorkOrderID[intCounter].WorkOrderNotes;

                        Paragraph parUpdateLine = new Paragraph(new Run(strTransactionDate + "\t\t" + strNotes));
                        parUpdateLine.LineHeight = 1;
                        fdWorkOrder.Blocks.Add(parUpdateLine);
                    }
                    Paragraph parSpace2 = new Paragraph(new Run());
                    fdWorkOrder.Blocks.Add(parSpace2);
                    Paragraph parHeader1 = new Paragraph(new Run("\t\t\t\t\tTechnician Notes"));
                    fdWorkOrder.Blocks.Add(parHeader1);
                    Paragraph parSpace3 = new Paragraph(new Run());
                    fdWorkOrder.Blocks.Add(parSpace3);
                    Paragraph parSpace4 = new Paragraph(new Run());
                    fdWorkOrder.Blocks.Add(parSpace4);
                    Paragraph parSpace5 = new Paragraph(new Run());
                    fdWorkOrder.Blocks.Add(parSpace5);
                    Paragraph parSpace6 = new Paragraph(new Run());
                    fdWorkOrder.Blocks.Add(parSpace6);
                    Paragraph parSpace7 = new Paragraph(new Run());
                    fdWorkOrder.Blocks.Add(parSpace7);
                    Paragraph parSpace8 = new Paragraph(new Run());
                    fdWorkOrder.Blocks.Add(parSpace8);


                    Paragraph parSignature = new Paragraph(new Run("Date: ________ Customer Signature: __________________________________"));
                    fdWorkOrder.Blocks.Add(parSignature);
                    Paragraph parSpace9 = new Paragraph(new Run());
                    fdWorkOrder.Blocks.Add(parSpace9);
                    Paragraph parSignature1 = new Paragraph(new Run("Date: ________ Technician Signature: ___________________________________"));
                    fdWorkOrder.Blocks.Add(parSignature1);

                    pdWorkOrder.PrintDocument(((IDocumentPaginatorSource)fdWorkOrder).DocumentPaginator, "Blue Jay Communications Work Order");
                }

                TheMDULettersClass.CreateMDUDropAcceptanceLetter(txtAccountNumber.Text, "MDU Drop Bury // ");

                ResetControls();
                SetControlsInvisible();
            }
            catch (Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Update Work Order // Print Button " + Ex.Message);
            }
        }
    }
}
