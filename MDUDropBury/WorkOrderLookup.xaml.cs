/* Title:           Work Order Lookup
 * Date:            12-19-17
 * Author:          Terry Holmes
 * 
 * Description:     This will allow the user to look up an order */

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
using WorkOrderDLL;
using DataValidationDLL;
using NewEventLogDLL;
using CustomersDLL;
using WorkOrderScheduleDLL;

namespace MDUDropBury
{
    /// <summary>
    /// Interaction logic for WorkOrderLookup.xaml
    /// </summary>
    public partial class WorkOrderLookup : Window
    {
        //setting up classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        WorkOrderClass TheWorkOrderClass = new WorkOrderClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        CustomersClass TheCustomersClass = new CustomersClass();
        MDULettersClass TheMDULettersClass = new MDULettersClass();
        WorkOrderScheduleClass TheWorkOrderScheduleClass = new WorkOrderScheduleClass();

        //setting up the data
        FindAddressByAddressesDataSet TheFindAddressbyAddressesDataSet = new FindAddressByAddressesDataSet();
        FindWorkOrderByAddressIDDataSet TheFindWorkOrderByAddressIDDataSet = new FindWorkOrderByAddressIDDataSet();
        FindWorkOrderByWorkOrderNumberDataSet TheFindWorkOrderbyWorkOrderNumberDataSet = new FindWorkOrderByWorkOrderNumberDataSet();
        FindWorkOrderUpdatesByWorkOrderIDDataSet TheFindWorkOrderUpdatesByIDDataSet = new FindWorkOrderUpdatesByWorkOrderIDDataSet();
        FindCustomerByAccountNumberDataSet TheFindCustomerByAccountNumberDataSet = new FindCustomerByAccountNumberDataSet();
        FindScheduledWorkOrdersByWorkOrderNumberDataSet TheFindScheduledWorkOrdersByWorkOrderNumberDataSet = new FindScheduledWorkOrdersByWorkOrderNumberDataSet();
        FindCustomerByAccountNumberDataSet FindCustomerByAccountNumberDataSet = new FindCustomerByAccountNumberDataSet();

        //setting up the variables
        string gstrDateScheduled;
        string gstrStartTime;
        string gstrEndTime;
        string gstrFirstName;
        string gstrLastName;


        public WorkOrderLookup()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strValueEntered;
            int intWorkOrderID;
            string strWorkOrderNumber;
            string strAccountNumber;
            int intRecordsReturned;
            bool blnItemFound = false;        

            try
            {
                strValueEntered = txtEnterData.Text;
                if(strValueEntered == "")
                {
                    TheMessagesClass.ErrorMessage("No Text Was Added");
                    return;
                }

                //checking see if this is a work order number
                strWorkOrderNumber = strValueEntered;
                TheFindWorkOrderbyWorkOrderNumberDataSet = TheWorkOrderClass.FindWorkOrderByWorkOrderNumber(strWorkOrderNumber);
                intRecordsReturned = TheFindWorkOrderbyWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber.Rows.Count;
                if(intRecordsReturned > 0)
                {
                    blnItemFound = true;
                    MainWindow.gstrWorkOrderNumber = strWorkOrderNumber;
                }
                else
                {
                    //checking for account number
                    strAccountNumber = strValueEntered;
                    TheFindCustomerByAccountNumberDataSet = TheCustomersClass.FindCustomerByAccountNumber(strAccountNumber);
                    intRecordsReturned = TheFindCustomerByAccountNumberDataSet.FindCustomerByAccountNumber.Rows.Count;

                    if(intRecordsReturned > 0)
                    {
                        MainWindow.gstrAddress = TheFindCustomerByAccountNumberDataSet.FindCustomerByAccountNumber[0].StreetAddress;
                        TheFindAddressbyAddressesDataSet = TheCustomersClass.FindAddressesByAddress(MainWindow.gstrAddress);
                        MainWindow.gintAddressID = TheFindAddressbyAddressesDataSet.FindAddressesByAddress[0].AddressID;

                        SelectWorkOrder SelectWorkOrder = new SelectWorkOrder();
                        SelectWorkOrder.ShowDialog();
                        blnItemFound = true;

                        if(MainWindow.gintWorkOrderID > -10)
                        {
                            TheFindWorkOrderbyWorkOrderNumberDataSet = TheWorkOrderClass.FindWorkOrderByWorkOrderNumber(MainWindow.gstrWorkOrderNumber);
                        }
                        else
                        {
                            blnItemFound = false;
                        }
                    }
                }

                if (blnItemFound == false)
                {
                    TheMessagesClass.ErrorMessage("No Work Orders Found");
                    return;
                }

                MainWindow.gintWorkOrderID = TheFindWorkOrderbyWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].WorkOrderID;

                MainWindow.gstrAccountNumber = TheFindWorkOrderbyWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].AccountNumber;

                TheFindCustomerByAccountNumberDataSet = TheCustomersClass.FindCustomerByAccountNumber(MainWindow.gstrAccountNumber);

                TheFindWorkOrderUpdatesByIDDataSet = TheWorkOrderClass.FindWorkOrderUpdatesByWorkOrderID(MainWindow.gintWorkOrderID);

                TheFindScheduledWorkOrdersByWorkOrderNumberDataSet = TheWorkOrderScheduleClass.FindWorkOrderScheduledByWorkOrderNumber(MainWindow.gstrWorkOrderNumber);

                gstrDateScheduled = Convert.ToString(TheFindWorkOrderbyWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].DateScheduled);
                gstrEndTime = Convert.ToString(TheFindScheduledWorkOrdersByWorkOrderNumberDataSet.FindScheduledWorkOrdersByWorkOrderNumber[0].EndTime);
                gstrStartTime = Convert.ToString(TheFindScheduledWorkOrdersByWorkOrderNumberDataSet.FindScheduledWorkOrdersByWorkOrderNumber[0].StartTime);
                gstrFirstName = TheFindCustomerByAccountNumberDataSet.FindCustomerByAccountNumber[0].FirstName;
                gstrLastName = TheFindCustomerByAccountNumberDataSet.FindCustomerByAccountNumber[0].LastName;

                //loading the controls
                txtAccountNumber.Text = TheFindWorkOrderbyWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].AccountNumber;
                txtAddress.Text = TheFindWorkOrderbyWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].StreetAddress;
                txtCity.Text = TheFindWorkOrderbyWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].City;
                txtPhoneNUmber.Text = TheFindWorkOrderbyWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].PhoneNumber;
                txtStatus.Text = TheFindWorkOrderbyWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].WorkOrderStatus;
                txtStatusDate.Text = Convert.ToString(TheFindWorkOrderbyWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].StatusDate);
                txtWorkOrderID.Text = Convert.ToString(MainWindow.gintWorkOrderID);
                txtWorkOrderNumber.Text = MainWindow.gstrWorkOrderNumber;
                txtZone.Text = TheFindWorkOrderbyWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].ZoneLocation;

                dgrUpdates.ItemsSource = TheFindWorkOrderUpdatesByIDDataSet.FindWorkOrderUpdateByWorkOrderID;

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Work Order Lookup // Find Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void txtEnterData_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting local variables
            string strInformationEntered;
            int intLength;
            int intRecordsReturned;

            try
            {
                strInformationEntered = txtEnterData.Text;

                intLength = strInformationEntered.Length;

                if(intLength > 4)
                {
                    btnFind.IsEnabled = true;
                   
                    TheFindAddressbyAddressesDataSet = TheCustomersClass.FindAddressesByAddress(strInformationEntered);

                    intRecordsReturned = TheFindAddressbyAddressesDataSet.FindAddressesByAddress.Rows.Count;

                    if(intRecordsReturned == 0)
                    {
                        return;
                    }
                    else if(intRecordsReturned == 1)
                    {
                        MainWindow.gintAddressID = TheFindAddressbyAddressesDataSet.FindAddressesByAddress[0].AddressID;
                        MainWindow.gstrAddress = TheFindAddressbyAddressesDataSet.FindAddressesByAddress[0].StreetAddress;                        
                    }
                    else if(intRecordsReturned != 1)
                    {
                        if(MainWindow.gblnFormSet == false)
                        {
                            MainWindow.gstrAddress = strInformationEntered;
                            SelectAddress SelectAddress = new SelectAddress();
                            SelectAddress.ShowDialog();
                        }
                        
                    }

                    TheFindWorkOrderByAddressIDDataSet = TheWorkOrderClass.FindWorkOrderByAddressID(MainWindow.gintAddressID);

                    intRecordsReturned = TheFindWorkOrderByAddressIDDataSet.FindWorkOrderByAddressID.Rows.Count;

                    if(intRecordsReturned == 0)
                    {
                        return;
                    }
                    else if(intRecordsReturned == 1)
                    {
                        MainWindow.gintWorkOrderID = TheFindWorkOrderByAddressIDDataSet.FindWorkOrderByAddressID[0].WorkOrderID;
                        MainWindow.gstrWorkOrderNumber = TheFindWorkOrderByAddressIDDataSet.FindWorkOrderByAddressID[0].WorkOrderNumber;
                        MainWindow.gstrAccountNumber = TheFindWorkOrderByAddressIDDataSet.FindWorkOrderByAddressID[0].AccountNumber;
                    }
                    else if(intRecordsReturned > 1)
                    {
                        if(MainWindow.gblnWorkOrderSet == false)
                        {
                            SelectWorkOrder SelectWorkOrder = new SelectWorkOrder();
                            SelectWorkOrder.ShowDialog();
                        }
                    }

                    if (MainWindow.gintWorkOrderID == -10)
                        return;

                    TheFindWorkOrderUpdatesByIDDataSet = TheWorkOrderClass.FindWorkOrderUpdatesByWorkOrderID(MainWindow.gintWorkOrderID);

                    TheFindWorkOrderbyWorkOrderNumberDataSet = TheWorkOrderClass.FindWorkOrderByWorkOrderNumber(MainWindow.gstrWorkOrderNumber);

                    TheFindScheduledWorkOrdersByWorkOrderNumberDataSet = TheWorkOrderScheduleClass.FindWorkOrderScheduledByWorkOrderNumber(MainWindow.gstrWorkOrderNumber);

                    MainWindow.gstrAccountNumber = TheFindWorkOrderbyWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].AccountNumber;

                    TheFindCustomerByAccountNumberDataSet = TheCustomersClass.FindCustomerByAccountNumber(MainWindow.gstrAccountNumber);

                    gstrDateScheduled = Convert.ToString(TheFindWorkOrderbyWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].DateScheduled);
                    gstrEndTime = Convert.ToString(TheFindScheduledWorkOrdersByWorkOrderNumberDataSet.FindScheduledWorkOrdersByWorkOrderNumber[0].EndTime);
                    gstrStartTime = Convert.ToString(TheFindScheduledWorkOrdersByWorkOrderNumberDataSet.FindScheduledWorkOrdersByWorkOrderNumber[0].StartTime);
                    gstrFirstName = TheFindCustomerByAccountNumberDataSet.FindCustomerByAccountNumber[0].FirstName;
                    gstrLastName = TheFindCustomerByAccountNumberDataSet.FindCustomerByAccountNumber[0].LastName;

                    //loading the controls
                    txtAccountNumber.Text = TheFindWorkOrderbyWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].AccountNumber;
                    txtAddress.Text = TheFindWorkOrderbyWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].StreetAddress;
                    txtCity.Text = TheFindWorkOrderbyWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].City;
                    txtPhoneNUmber.Text = TheFindWorkOrderbyWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].PhoneNumber;
                    txtStatus.Text = TheFindWorkOrderbyWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].WorkOrderStatus;
                    txtStatusDate.Text = Convert.ToString(TheFindWorkOrderbyWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].StatusDate);
                    txtWorkOrderID.Text = Convert.ToString(MainWindow.gintWorkOrderID);
                    txtWorkOrderNumber.Text = MainWindow.gstrWorkOrderNumber;
                    txtZone.Text = TheFindWorkOrderbyWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].ZoneLocation;

                    dgrUpdates.ItemsSource = TheFindWorkOrderUpdatesByIDDataSet.FindWorkOrderUpdateByWorkOrderID;
                }
                else
                {
                    btnFind.IsEnabled = false;
                    MainWindow.gblnFormSet = false;
                    MainWindow.gblnWorkOrderSet = false;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Work Order Look Up // txt Enter Data Text Changed " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            btnFind.IsEnabled = false;
            MainWindow.gblnFormSet = false;
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
                    Paragraph Title = new Paragraph(new Run("Blue Jay Communications Work Order: " + MainWindow.gstrWorkOrderNumber));
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
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(gstrDateScheduled))));

                    intCurrentRow++;

                    WorkOrderTable.RowGroups.Add(new TableRowGroup());
                    WorkOrderTable.RowGroups[0].Rows.Add(new TableRow());

                    newTableRow = WorkOrderTable.RowGroups[0].Rows[intCurrentRow];

                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Address:"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(txtAddress.Text))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Start Time"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(gstrStartTime))));

                    intCurrentRow++;

                    WorkOrderTable.RowGroups.Add(new TableRowGroup());
                    WorkOrderTable.RowGroups[0].Rows.Add(new TableRow());

                    newTableRow = WorkOrderTable.RowGroups[0].Rows[intCurrentRow];

                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("City:"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(txtCity.Text + ", " + TheFindCustomerByAccountNumberDataSet.FindCustomerByAccountNumber[0].CustomerState))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("End Time"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(gstrEndTime))));

                    intCurrentRow++;

                    WorkOrderTable.RowGroups.Add(new TableRowGroup());
                    WorkOrderTable.RowGroups[0].Rows.Add(new TableRow());

                    newTableRow = WorkOrderTable.RowGroups[0].Rows[intCurrentRow];

                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Name:"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(gstrFirstName + " " + gstrLastName))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Work Type"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(TheFindWorkOrderbyWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].WorkType))));

                    intCurrentRow++;

                    WorkOrderTable.RowGroups.Add(new TableRowGroup());
                    WorkOrderTable.RowGroups[0].Rows.Add(new TableRow());

                    newTableRow = WorkOrderTable.RowGroups[0].Rows[intCurrentRow];

                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Phone Number:"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(txtPhoneNUmber.Text))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Zone"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(TheFindWorkOrderbyWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].ZoneLocation))));

                    Paragraph parHeader = new Paragraph(new Run("\t\t\t\t\tWork Order Notes"));
                    fdWorkOrder.Blocks.Add(parHeader);

                    TheFindWorkOrderUpdatesByIDDataSet = TheWorkOrderClass.FindWorkOrderUpdatesByWorkOrderID(MainWindow.gintWorkOrderID);

                    intNumberOfRecords = TheFindWorkOrderUpdatesByIDDataSet.FindWorkOrderUpdateByWorkOrderID.Rows.Count - 1;

                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        strTransactionDate = Convert.ToString(TheFindWorkOrderUpdatesByIDDataSet.FindWorkOrderUpdateByWorkOrderID[intCounter].TransactionDate);
                        strFirstName = TheFindWorkOrderUpdatesByIDDataSet.FindWorkOrderUpdateByWorkOrderID[intCounter].FirstName;
                        strLastName = TheFindWorkOrderUpdatesByIDDataSet.FindWorkOrderUpdateByWorkOrderID[intCounter].LastName;
                        strNotes = TheFindWorkOrderUpdatesByIDDataSet.FindWorkOrderUpdateByWorkOrderID[intCounter].WorkOrderNotes;

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

                TheMDULettersClass.CreateMDUDropAcceptanceLetter(MainWindow.gstrAccountNumber, "MDU Drop Bury // ");

            }
            catch (Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Update Work Order // Print Button " + Ex.Message);
            }
        }
    }
}
