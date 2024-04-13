/* Title:           Create Work Order
 * Date:            8-9-17
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
using CustomersDLL;
using WorkOrderDLL;
using DataValidationDLL;
using DateSearchDLL;
using WorkTypeDLL;
using WorkOrderScheduleDLL;

namespace MDUDropBury
{
    /// <summary>
    /// Interaction logic for CreateWorkOrder.xaml
    /// </summary>
    public partial class CreateWorkOrder : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        CustomersClass TheCustomerClass = new CustomersClass();
        WorkOrderClass TheWorkOrderClass = new WorkOrderClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        WorkTypeClass TheWorkTypeClass = new WorkTypeClass();
        WorkOrderScheduleClass TheWorkOrderScheduleClass = new WorkOrderScheduleClass();

        //setting up the data
        FindAddressByAddressIDDataSet TheFindAddressByAddressIDDataSet = new FindAddressByAddressIDDataSet();
        FindWorkZoneByZoneIDDataSet TheFindWorkZoneByZoneIDDataSet = new FindWorkZoneByZoneIDDataSet();
        FindCustomerByCustomerIDDataSet TheFindCustomerByCustomerID = new FindCustomerByCustomerIDDataSet();
        FindWorkTypeSortedDataSet TheFindWorkTypeSortedDataSet = new FindWorkTypeSortedDataSet();
        FindWorkTypeByTypeDataSet TheFindWorkTypeByTypeDataSet = new FindWorkTypeByTypeDataSet();
        FindWorkOrderStatusByStatusDataSet TheFindWorkOrderStatusDataSet = new FindWorkOrderStatusByStatusDataSet();
        FindActiveCustomerByAccountNumberDataSet TheFindActiveCustomerByAccountNumberDataSet = new FindActiveCustomerByAccountNumberDataSet();
        FindWorkOrderByWorkOrderNumberDataSet TheFindWorkOrderByWorkOrderNumberDataSet = new FindWorkOrderByWorkOrderNumberDataSet();

        ShowWorkScheduled ShowWorkScheduled = new ShowWorkScheduled();

        int gintZoneID;
        int gintWorkTypeID;
        int gintStatusID;
        
        public CreateWorkOrder()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            ShowWorkScheduled.Close();

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
            DateTime datTransactionDate = DateTime.Now;

            try
            {
                ShowWorkScheduled.Show();

                MainWindow.gintCustomerID = 0;

                TheFindAddressByAddressIDDataSet = TheCustomerClass.FindAddressByAddressID(MainWindow.gintAddressID);

                gintZoneID = TheFindAddressByAddressIDDataSet.FindAddressByAddressID[0].ZoneID;

                TheFindWorkOrderStatusDataSet = TheWorkOrderClass.FindWorkOrderStatusByStatus("SCHEDULED");

                gintStatusID = TheFindWorkOrderStatusDataSet.FindWorkOrderStatusByStatus[0].StatusID;

                TheFindWorkZoneByZoneIDDataSet = TheCustomerClass.FindWorkZoneByZoneID(gintZoneID);

                txtAddress.Text = TheFindAddressByAddressIDDataSet.FindAddressByAddressID[0].StreetAddress;
                txtCity.Text = TheFindAddressByAddressIDDataSet.FindAddressByAddressID[0].City;
                txtZone.Text = TheFindWorkZoneByZoneIDDataSet.FindWorkZoneByZoneID[0].ZoneLocation;

                SelectCustomer SelectCustomer = new SelectCustomer();
                SelectCustomer.ShowDialog();

                if(MainWindow.gintCustomerID != 0)
                {
                    TheFindCustomerByCustomerID = TheCustomerClass.FindCustomerByCustomerID(MainWindow.gintCustomerID);

                    txtAcountNumber.Text = TheFindCustomerByCustomerID.FindCustomerByCustomerID[0].AccountNumber;
                    txtPhoneNumber.Text = TheFindCustomerByCustomerID.FindCustomerByCustomerID[0].PhoneNumber;
                }

                TheFindWorkTypeSortedDataSet = TheWorkTypeClass.FindWorkTypeSorted();

                //getting ready for the loop
                cboWorkType.Items.Add("Select Work Type");
                intNumberOfRecords = TheFindWorkTypeSortedDataSet.FindWorkTypeSorted.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboWorkType.Items.Add(TheFindWorkTypeSortedDataSet.FindWorkTypeSorted[intCounter].WorkType);
                }

                cboWorkType.SelectedIndex = 0;
                txtDateEntered.Text = Convert.ToString(datTransactionDate);
                txtDateReceived.Text = Convert.ToString(datTransactionDate);
                txtDateScheduled.Text = Convert.ToString(datTransactionDate);
                txtStatusDate.Text = Convert.ToString(datTransactionDate);

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Create Work Order // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //creating local variables
            string strValueForValidation;
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";
            string strAccountNumber = "";
            string strPhoneNumber = "";
            string strWorkOrder = "";
            int intSelectedIndex = 0;
            DateTime datEnterDate = DateTime.Now;
            DateTime datScheduledDate = DateTime.Now;
            DateTime datReceivedDate = DateTime.Now;
            DateTime datStatusDate = DateTime.Now;
            int intCustomerID;
            int intRecordsReturned;
            int intWorkOrderID;
            string strFirstName;
            string strLastName;
            TimeSpan timStartTime = TimeSpan.Zero;
            TimeSpan timEndTime = TimeSpan.Zero;

            try
            {
                //beginning data validation
                strAccountNumber = txtAcountNumber.Text;
                if(strAccountNumber == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Account Number is not Entered\n";
                }
                else
                {
                    if(strAccountNumber.Length < 7)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Account Number is not Long Enough\n";
                    }
                }
                strPhoneNumber = txtPhoneNumber.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyPhoneNumberFormat(strPhoneNumber);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Phone Number is not the Correct Format\n";
                }
                strWorkOrder = txtWorkOrderNumber.Text;
                if(strWorkOrder == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The Work Order Number Was Not Entered\n";
                }
                else
                {
                    if(strWorkOrder.Length < 7)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Work Order Number is not Long Enough\n";
                    }
                }
                intSelectedIndex = cboWorkType.SelectedIndex;
                if(intSelectedIndex == 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Work Type was not Selected\n";
                }
                strValueForValidation = txtDateEntered.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Data Entered is not a Date\n";
                }
                else
                {
                    datEnterDate = Convert.ToDateTime(strValueForValidation);
                }
                strValueForValidation = txtDateScheduled.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Date Scheduled is not a Date\n";
                }
                else
                {
                    datScheduledDate = Convert.ToDateTime(strValueForValidation);
                }
                strValueForValidation = txtDateReceived.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Date Received is not a Date\n";
                }
                else
                {
                    datReceivedDate = Convert.ToDateTime(strValueForValidation);
                }

                datStatusDate = Convert.ToDateTime(txtStatusDate.Text);

                strFirstName = txtFirstName.Text;
                if(strFirstName == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "First Name Not Entered\n";
                }
                strLastName = txtLastName.Text;
                if(strLastName == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "Last Name Not Entered\n";
                }
                strValueForValidation = txtStartTime.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyTimeSpanInfo(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Start Time Was Not Entered\n";
                }
                else
                {
                    TimeSpan.TryParse(strValueForValidation, out timStartTime);
                }
                strValueForValidation = txtEndTime.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyTimeSpanInfo(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage += "The End Time Was Not Entered\n";
                }
                else
                {
                    TimeSpan.TryParse(strValueForValidation, out timEndTime);
                }
                if (blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }
                

                TheFindActiveCustomerByAccountNumberDataSet = TheCustomerClass.FindActiveCustomerByAccountNumber(strAccountNumber);

                intRecordsReturned = TheFindActiveCustomerByAccountNumberDataSet.FindActiveCustomerByAccountNumber.Rows.Count;

                if(intRecordsReturned == 0)
                {
                    blnFatalError = TheCustomerClass.InsertCustomer(MainWindow.gintAddressID, strPhoneNumber, strAccountNumber, strFirstName, strLastName);

                    TheFindActiveCustomerByAccountNumberDataSet = TheCustomerClass.FindActiveCustomerByAccountNumber(strAccountNumber);
                }

                intCustomerID = TheFindActiveCustomerByAccountNumberDataSet.FindActiveCustomerByAccountNumber[0].CustomerID;

                blnFatalError = TheWorkOrderClass.InsertWorkOrder(strWorkOrder, gintWorkTypeID, intCustomerID, MainWindow.gintAddressID, datScheduledDate, datReceivedDate, gintStatusID);

                if (blnFatalError == true)
                    throw new Exception();

                TheFindWorkOrderByWorkOrderNumberDataSet = TheWorkOrderClass.FindWorkOrderByWorkOrderNumber(strWorkOrder);

                intWorkOrderID = TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].WorkOrderID;

                blnFatalError = TheWorkOrderScheduleClass.InsertWorkOrderScheduled(intWorkOrderID, intCustomerID, datScheduledDate, timStartTime, timEndTime, -1);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheWorkOrderClass.InsertWorkOrderUpdate(intWorkOrderID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "OPENED WORK ORDER");

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Record Has Been Saved");

                txtAcountNumber.Text = "";
                txtAddress.Text = "";
                txtCity.Text = "";
                txtDateEntered.Text = "";
                txtDateReceived.Text = "";
                txtDateScheduled.Text = "";
                txtEndTime.Text = "";
                txtFirstName.Text = "";
                txtLastName.Text = "";
                txtPhoneNumber.Text = "";
                txtStatusDate.Text = "";
                txtWorkOrderNumber.Text = "";
                txtStartTime.Text = "";
                txtZone.Text = "";
                cboWorkType.SelectedIndex = 0;
                btnSave.IsEnabled = false;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Create Work Order // Save Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void cboWorkType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variables
            int intSelectedIndex;
            string strWorkType;

            intSelectedIndex = cboWorkType.SelectedIndex;

            if(intSelectedIndex > 0)
            {
                strWorkType = cboWorkType.SelectedItem.ToString();

                TheFindWorkTypeByTypeDataSet = TheWorkTypeClass.FindWorkTypeByType(strWorkType);

                gintWorkTypeID = TheFindWorkTypeByTypeDataSet.FindWorkTypeByType[0].TypeID;
            }
        }
    }
}
