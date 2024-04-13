/* Title:           Import Work Orders
 * Date:            9-19-17
 * Author:          Terry Holmes
 * 
 * Description:     This form is used to import the spreadsheets into the program */

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
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using CustomersDLL;
using DataValidationDLL;
using DateSearchDLL;
using DropBuryMDUDLL;
using KeyWordDLL;
using WorkOrderDLL;
using WorkTypeDLL;

namespace MDUDropBury
{
    /// <summary>
    /// Interaction logic for ImportWorkOrders.xaml
    /// </summary>
    public partial class ImportWorkOrders : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        CustomersClass TheCustomersClass = new CustomersClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();
        DropBuryMDUClass TheDropBuryMDUClass = new DropBuryMDUClass();
        KeyWordClass TheKeyWordClass = new KeyWordClass();
        WorkOrderClass TheWorkOrderClass = new WorkOrderClass();
        WorkTypeClass TheWorkTypeClass = new WorkTypeClass();

        ImportedDataSet TheImportedDataSet = new ImportedDataSet();
        CustomersDataSet TheCustomersDataSet = new CustomersDataSet();
        FindWorkTypeSortedDataSet TheFindWorkTypeSortedDataSet = new FindWorkTypeSortedDataSet();
        FindCustomerAddressDateMatchDataSet TheFindCustomerAddressDateMatchDataSet = new FindCustomerAddressDateMatchDataSet();
        FindCustomerByPhoneNumberDataSet TheFindCustomerByPhoneNumberDataSet = new FindCustomerByPhoneNumberDataSet();
        FindCustomersByAddressIDDataSet TheFindCustomersByAddressIDDataSet = new FindCustomersByAddressIDDataSet();
        FindActiveCustomerByAccountNumberDataSet TheFindActiveCustomerByAccountNumberDataSet = new FindActiveCustomerByAccountNumberDataSet();
        FindCustomerByAccountNumberDataSet TheFindCustomerByAccountNumberDataSet = new FindCustomerByAccountNumberDataSet();
        FindWorkOrderByWorkOrderNumberDataSet TheFindWorkOrderByWorkOrderNumberDataSet = new FindWorkOrderByWorkOrderNumberDataSet();
        FindAddressByAddressesDataSet TheFindAddressByAddressesDataSet = new FindAddressByAddressesDataSet();
        FindWorkZoneByZoneNameDataSet TheFindWorkZoneByZoneNameDataSet = new FindWorkZoneByZoneNameDataSet();
        

        int gintColumnCounter;
        int gintPhoneCounter;
        int gintWorkTypeID;
        string gstrWorkType;

        public ImportWorkOrders()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSelectSpreadSheet_Click(object sender, RoutedEventArgs e)
        {
            bool blnFatalError = false;
            Excel.Application xlDropOrder;
            Excel.Workbook xlDropBook;
            Excel.Worksheet xlDropSheet;
            Excel.Range range;

            string strInformation = "";
            int intColumnRange = 0;
            int intCounter;
            int intNumberOfRecords;
            char[] chaInformation;
            int intCharCounter;
            int intCharLength;
            string strCompleteWord = "";
            int intLength;

            try
            {
                TheImportedDataSet.importedjobs.Rows.Clear();

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.FileName = "Document"; // Default file name
                dlg.DefaultExt = ".csv"; // Default file extension
                dlg.Filter = "Excel (.csv)|*.csv"; // Filter files by extension

                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process open file dialog box results
                if (result == true)
                {
                    // Open document
                    string filename = dlg.FileName;
                }

                PleaseWait PleaseWait = new PleaseWait();
                PleaseWait.Show();

                xlDropOrder = new Excel.Application();
                xlDropBook = xlDropOrder.Workbooks.Open(dlg.FileName, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                xlDropSheet = (Excel.Worksheet)xlDropOrder.Worksheets.get_Item(1);

                range = xlDropSheet.UsedRange;
                intNumberOfRecords = range.Rows.Count - 1;
                intColumnRange = range.Columns.Count;

                for(intCounter = 2; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strInformation = Convert.ToString((range.Cells[intCounter, 1] as Excel.Range).Value2);

                    gintColumnCounter = 0;

                    chaInformation = strInformation.ToCharArray();

                    intCharLength = strInformation.Length - 1;

                    ImportedDataSet.importedjobsRow NewJobRow = TheImportedDataSet.importedjobs.NewimportedjobsRow();

                    NewJobRow.ScheduledDate = DateTime.Now;

                    for(intCharCounter = 0; intCharCounter <= intCharLength; intCharCounter++)
                    {
                        if(chaInformation[intCharCounter] != ',')
                        {
                            strCompleteWord += Convert.ToString(chaInformation[intCharCounter]);
                            strCompleteWord = strCompleteWord.ToUpper();
                        }
                        else
                        {
                            if(gintColumnCounter == 1)
                            {
                                NewJobRow.WorkorderID = strCompleteWord;
                            }
                            else if(gintColumnCounter == 11)
                            {
                                NewJobRow.JobStatus = strCompleteWord;
                            }
                            else if (gintColumnCounter == 29)
                            {
                                strCompleteWord = strCompleteWord.Substring(2);

                                if(strCompleteWord == "MAPLE")
                                {
                                    strCompleteWord = "MAPLE HTS";
                                }

                                NewJobRow.Pool = strCompleteWord;
                            }
                            else if (gintColumnCounter == 57)
                            {
                                NewJobRow.AccountID = strCompleteWord;
                            }
                            else if (gintColumnCounter == 58)
                            {
                                NewJobRow.FirstName = strCompleteWord;
                            }
                            else if (gintColumnCounter == 59)
                            {
                                NewJobRow.LastName = strCompleteWord;
                            }
                            else if (gintColumnCounter == 60)
                            {
                                if(strCompleteWord == "(999) 999-9999")
                                {
                                    strCompleteWord = Convert.ToString(gintPhoneCounter);
                                    gintPhoneCounter++;
                                }
                                else if (strCompleteWord == "")
                                {
                                    strCompleteWord = Convert.ToString(gintPhoneCounter);
                                    gintPhoneCounter++;
                                }
                                else if(strCompleteWord == null)
                                {
                                    strCompleteWord = Convert.ToString(gintPhoneCounter);
                                    gintPhoneCounter++;
                                }

                                NewJobRow.PhoneNumber = strCompleteWord;
                            }
                            else if (gintColumnCounter == 75)
                            {
                                NewJobRow.StreetAddress = strCompleteWord;
                            }
                            else if (gintColumnCounter == 77)
                            {
                                NewJobRow.City = strCompleteWord;
                            }
                            else if (gintColumnCounter == 78)
                            {
                                NewJobRow.State = strCompleteWord;
                            }
                            else if (gintColumnCounter == 79)
                            {
                                intLength = strCompleteWord.Length;

                                if(intColumnRange > 5)
                                {
                                    strCompleteWord = strCompleteWord.Substring(0, 5);
                                }
                                                           

                                NewJobRow.Zip = strCompleteWord;
                            }

                            strCompleteWord = "";
                            gintColumnCounter++;
                        }
                    }

                    TheImportedDataSet.importedjobs.Rows.Add(NewJobRow);
                }

                PleaseWait.Close();
                dgrResults.ItemsSource = TheImportedDataSet.importedjobs;
                btnImport.IsEnabled = true;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Import Work Orders // Select Spreadsheet Button " + Ex.Message);

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
                TheCustomersDataSet = TheCustomersClass.GetCustomersInfo();

                gintPhoneCounter = TheCustomersDataSet.customers.Rows.Count;

                TheFindWorkTypeSortedDataSet = TheWorkTypeClass.FindWorkTypeSorted();

                intNumberOfRecords = TheFindWorkTypeSortedDataSet.FindWorkTypeSorted.Rows.Count - 1;
                cboSelectWorkType.Items.Add("Select Work Type");

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectWorkType.Items.Add(TheFindWorkTypeSortedDataSet.FindWorkTypeSorted[intCounter].WorkType);
                }

                cboSelectWorkType.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Import Work Orders // Windows Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            
        }

        private void cboSelectWorkType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //this will select the work type
            int intSelectedIndex;

            intSelectedIndex = cboSelectWorkType.SelectedIndex - 1;

            if (intSelectedIndex == -1)
            {
                btnImport.IsEnabled = false;
                btnSelectSpreadSheet.IsEnabled = false;
                TheImportedDataSet.importedjobs.Rows.Clear();
                dgrResults.ItemsSource = TheImportedDataSet.importedjobs;
            }
            else
            {
                gintWorkTypeID = TheFindWorkTypeSortedDataSet.FindWorkTypeSorted[intSelectedIndex].TypeID;
                gstrWorkType = TheFindWorkTypeSortedDataSet.FindWorkTypeSorted[intSelectedIndex].WorkType;
                btnSelectSpreadSheet.IsEnabled = true;
            }
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            string strWorkOrderNumber;
            string strAccountNumber;
            string strJobStatus;
            DateTime datScheduledDate;
            string strZone;
            string strPhoneNumber;
            string strFirstName;
            string strLastName;
            string strAddress;
            string strCity;
            string strState;
            string strZip;
            int intAddressID;
            int intCustomerID;
            DateTime datTransactionDate;
            int intRecordsReturned;
            bool blnFatalError = false;
            int intWorkOrderID;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                intNumberOfRecords = TheImportedDataSet.importedjobs.Rows.Count - 1;

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    strWorkOrderNumber = TheImportedDataSet.importedjobs[intCounter].WorkorderID;
                    strAccountNumber = TheImportedDataSet.importedjobs[intCounter].AccountID;
                    strJobStatus = TheImportedDataSet.importedjobs[intCounter].JobStatus;
                    strZone = TheImportedDataSet.importedjobs[intCounter].Pool;
                    strPhoneNumber = TheImportedDataSet.importedjobs[intCounter].PhoneNumber;
                    strFirstName = TheImportedDataSet.importedjobs[intCounter].FirstName;
                    strLastName = TheImportedDataSet.importedjobs[intCounter].LastName;
                    strAddress = TheImportedDataSet.importedjobs[intCounter].StreetAddress;
                    strCity = TheImportedDataSet.importedjobs[intCounter].City;
                    strState = TheImportedDataSet.importedjobs[intCounter].State;
                    strZip = TheImportedDataSet.importedjobs[intCounter].Zip;
                    datTransactionDate = DateTime.Now;
                    datScheduledDate = TheImportedDataSet.importedjobs[intCounter].ScheduledDate;

                    TheFindWorkOrderByWorkOrderNumberDataSet = TheWorkOrderClass.FindWorkOrderByWorkOrderNumber(strWorkOrderNumber);

                    intRecordsReturned = TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber.Rows.Count;

                    if (intRecordsReturned == 0)
                    {
                        //searching for the customer
                        TheFindActiveCustomerByAccountNumberDataSet = TheCustomersClass.FindActiveCustomerByAccountNumber(strAccountNumber);

                        intRecordsReturned = TheFindActiveCustomerByAccountNumberDataSet.FindActiveCustomerByAccountNumber.Rows.Count;

                        if (intRecordsReturned > 0)
                        {
                            intCustomerID = TheFindActiveCustomerByAccountNumberDataSet.FindActiveCustomerByAccountNumber[0].CustomerID;
                            intAddressID = TheFindActiveCustomerByAccountNumberDataSet.FindActiveCustomerByAccountNumber[0].AddressID;
                        }
                        else if (intRecordsReturned == 0)
                        {
                            intAddressID = FindAddressID(strAddress, strCity, strState, strZip, strZone);

                            if (intAddressID == -1)
                                throw new Exception();

                            blnFatalError = TheCustomersClass.InsertCustomer(intAddressID, strPhoneNumber, strAccountNumber, strFirstName, strLastName);

                            if (blnFatalError == true)
                                throw new Exception();

                            TheFindActiveCustomerByAccountNumberDataSet = TheCustomersClass.FindActiveCustomerByAccountNumber(strAccountNumber);

                            intCustomerID = TheFindActiveCustomerByAccountNumberDataSet.FindActiveCustomerByAccountNumber[0].CustomerID;

                            blnFatalError = TheWorkOrderClass.InsertWorkOrder(strWorkOrderNumber, gintWorkTypeID, intCustomerID, intAddressID, datScheduledDate, DateTime.Now, 1001);

                            if (blnFatalError == true)
                                throw new Exception();

                            TheFindWorkOrderByWorkOrderNumberDataSet = TheWorkOrderClass.FindWorkOrderByWorkOrderNumber(strWorkOrderNumber);

                            intWorkOrderID = TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].WorkOrderID;

                            blnFatalError = TheWorkOrderClass.InsertWorkOrderUpdate(intWorkOrderID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, "INFORMATION IMPORTED FROM ARRIS");

                            if (blnFatalError == true)
                                throw new Exception();
                        }
                    }
                }

                TheMessagesClass.InformationMessage("Import Complete");
                cboSelectWorkType.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Import Work Orders // Import Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            PleaseWait.Close();
        }
        private int FindAddressID(string strAddress, string strCity, string strState, string strZip, string strZone)
        {
            int intAddressID = -1;
            int intRecordsReturned;
            DateTime datTransactionDate = DateTime.Now;
            int intCounter;
            bool blnFatalError;
            int intZoneID;

            try
            {
                TheFindAddressByAddressesDataSet = TheCustomersClass.FindAddressesByAddress(strAddress);

                intRecordsReturned = TheFindAddressByAddressesDataSet.FindAddressesByAddress.Rows.Count - 1;

                if(intRecordsReturned > -1)
                {
                    for(intCounter = 0; intCounter <= intRecordsReturned; intCounter++)
                    {
                        if(strCity == TheFindAddressByAddressesDataSet.FindAddressesByAddress[intCounter].City)
                        {
                            intAddressID = TheFindAddressByAddressesDataSet.FindAddressesByAddress[intCounter].AddressID;
                        }
                    }
                }

                if(intAddressID == -1)
                {
                    TheFindWorkZoneByZoneNameDataSet = TheCustomersClass.FindWorkZoneByZoneName(strZone);

                    intRecordsReturned = TheFindWorkZoneByZoneNameDataSet.FindWorkZoneByZoneName.Rows.Count;

                    if(intRecordsReturned == 0)
                    {
                        intZoneID = 1010;
                    }
                    else
                    {
                        intZoneID = TheFindWorkZoneByZoneNameDataSet.FindWorkZoneByZoneName[0].ZoneID;
                    }

                    blnFatalError = TheCustomersClass.InsertCustomerAddress(strAddress, strCity, strState, intZoneID, strZip, datTransactionDate);

                    if (blnFatalError == true)
                        throw new Exception();

                    TheFindCustomerAddressDateMatchDataSet = TheCustomersClass.FindCustomerAddressDateMatch(datTransactionDate);

                    intAddressID = TheFindCustomerAddressDateMatchDataSet.FindCustomerAddressesDateMatch[0].AddressID;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Import Work Orders // Find Address ID " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());

                intAddressID = -1;
            }

            return intAddressID;
        }
    }
}
