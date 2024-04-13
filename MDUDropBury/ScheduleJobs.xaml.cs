/* Title:           Schedule Jobs
 * Date:            9-.8-17
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
using DataValidationDLL;
using WorkOrderScheduleDLL;

namespace MDUDropBury
{
    /// <summary>
    /// Interaction logic for ScheduleJobs.xaml
    /// </summary>
    public partial class ScheduleJobs : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        WorkOrderClass TheWorkOrderClass = new WorkOrderClass();
        WorkTypeClass TheWorkTypeClass = new WorkTypeClass();
        CustomersClass TheCustomerClass = new CustomersClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        WorkOrderScheduleClass TheWorkOrderScheduleClass = new WorkOrderScheduleClass();
        MDULettersClass TheMDULettersClass = new MDULettersClass();

        FindWorkOrderByWorkOrderNumberDataSet TheFindWorkOrderByWorkOrderNumberDataSet = new FindWorkOrderByWorkOrderNumberDataSet();
        FindWorkZonesDataSet TheFindWorkZonesDataSet = new FindWorkZonesDataSet();
        FindActiveCustomerByAccountNumberDataSet TheFindActiveCustomerByAccountNumberDataSet = new FindActiveCustomerByAccountNumberDataSet();
        FindAddressByAddressIDDataSet TheFindAddressByAddressIDDataSet = new FindAddressByAddressIDDataSet();
        FindWorkOrderUpdatesByWorkOrderIDDataSet TheFindWorkOrderUpdatesByWorkOrderIDDataSet = new FindWorkOrderUpdatesByWorkOrderIDDataSet();
        FindWorkOrderStatusSortedDataSet TheFindWorkOrderStatusSortedDataSet = new FindWorkOrderStatusSortedDataSet();
        FindScheduledWorkOrdersByWorkOrderNumberDataSet TheFindScheduledWorkOrderByWorkOrderNumberDataSet = new FindScheduledWorkOrdersByWorkOrderNumberDataSet();

        ShowWorkScheduled ShowWorkScheduled = new ShowWorkScheduled();
        
        int gintZoneID;
        string gstrZoneLocation;
        int gintStatusID;

        public ScheduleJobs()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            ShowWorkScheduled.Close();
            this.Close();
        }
        private void SetStatusComboBox(string strStatus)
        {
            int intCounter;
            int intNumberOfRecords;
            int intSelectedIndex = 0;

            intNumberOfRecords = cboStatus.Items.Count - 1;

            for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboStatus.SelectedIndex = intCounter;

                if (strStatus == cboStatus.SelectedItem.ToString())
                {
                    intSelectedIndex = intCounter;
                }
            }

            gintStatusID = TheFindWorkOrderStatusSortedDataSet.FindWorkOrderStatusSorted[intSelectedIndex - 1].StatusID;

            cboStatus.SelectedIndex = intSelectedIndex;

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this will load up the controls
            int intCounter;
            int intNumberOfRecords;
            string strStatus;
            int intRecordsReturned;
            string strStartTime = "";
            string strEndTime = "";

            try
            {
                //loading the zone combo box
                TheFindWorkZonesDataSet = TheCustomerClass.FindWorkZones();
                ShowWorkScheduled.Show();

                intNumberOfRecords = TheFindWorkZonesDataSet.FindWorkZones.Rows.Count - 1;
                cboZone.Items.Add("Select Zone");

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboZone.Items.Add(TheFindWorkZonesDataSet.FindWorkZones[intCounter].ZoneLocation);
                }
                
                TheFindWorkOrderStatusSortedDataSet = TheWorkOrderClass.FindWorkOrderStatusSorted();

                intNumberOfRecords = TheFindWorkOrderStatusSortedDataSet.FindWorkOrderStatusSorted.Rows.Count - 1;
                cboStatus.Items.Add("Select Status");

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboStatus.Items.Add(TheFindWorkOrderStatusSortedDataSet.FindWorkOrderStatusSorted[intCounter].WorkOrderStatus);
                }

                //cboZone.SelectedIndex = 0;

                TheFindWorkOrderByWorkOrderNumberDataSet = TheWorkOrderClass.FindWorkOrderByWorkOrderNumber(MainWindow.gstrWorkOrderNumber);

                //SetZoneCombo(TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].ZoneLocation);
                MainWindow.gintWorkOrderID = TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].WorkOrderID;


                TheFindScheduledWorkOrderByWorkOrderNumberDataSet = TheWorkOrderScheduleClass.FindWorkOrderScheduledByWorkOrderNumber(MainWindow.gstrWorkOrderNumber);

                intRecordsReturned = TheFindScheduledWorkOrderByWorkOrderNumberDataSet.FindScheduledWorkOrdersByWorkOrderNumber.Rows.Count;

                if(intRecordsReturned > 0)
                {
                    strStartTime = Convert.ToString(TheFindScheduledWorkOrderByWorkOrderNumberDataSet.FindScheduledWorkOrdersByWorkOrderNumber[0].StartTime);
                    strEndTime = Convert.ToString(TheFindScheduledWorkOrderByWorkOrderNumberDataSet.FindScheduledWorkOrdersByWorkOrderNumber[0].EndTime);
                }

                //loading the controls
                txtWorkOrderNumber.Text = TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].WorkOrderNumber;
                txtAccountNumber.Text = TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].AccountNumber;
                txtAddress.Text = TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].StreetAddress;
                txtCity.Text = TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].City;
                txtDateScheduled.Text = Convert.ToString(TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].DateScheduled);
                txtPhoneNumber.Text = TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].PhoneNumber;
                strStatus = TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].WorkOrderStatus;
                txtStartTime.Text = strStartTime;
                txtEndTime.Text = strEndTime;

                SetStatusComboBox(strStatus);

                TheFindActiveCustomerByAccountNumberDataSet = TheCustomerClass.FindActiveCustomerByAccountNumber(txtAccountNumber.Text);

                MainWindow.gintCustomerID = TheFindActiveCustomerByAccountNumberDataSet.FindActiveCustomerByAccountNumber[0].CustomerID;

                txtFirstName.Text = TheFindActiveCustomerByAccountNumberDataSet.FindActiveCustomerByAccountNumber[0].FirstName;
                txtLastName.Text = TheFindActiveCustomerByAccountNumberDataSet.FindActiveCustomerByAccountNumber[0].LastName;

                intNumberOfRecords = TheFindWorkZonesDataSet.FindWorkZones.Rows.Count - 1;

                for( intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    if(TheFindWorkZonesDataSet.FindWorkZones[intCounter].ZoneLocation == TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].ZoneLocation)
                    {
                        cboZone.SelectedIndex = intCounter + 1;
                    }
                }


            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Schedule Jobs // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnSchedule_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intAddressID;
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";
            string strValueForValidation;
            DateTime datScheduleDate = DateTime.Now;
            TimeSpan timStartTime = TimeSpan.Zero;
            TimeSpan timEndTime = TimeSpan.Zero;
            int intRecordsReturned;
            int intEmployeeID;

            try
            {
                strValueForValidation = txtDateScheduled.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyDateData(strValueForValidation);
                if (blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage = "The Scheduled Date is not a Date\n";
                }
                else
                {
                    datScheduleDate = Convert.ToDateTime(strValueForValidation);
                }
                strValueForValidation = txtStartTime.Text;
                blnThereIsAProblem = TheDataValidationClass.VerifyTimeSpanInfo(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage = "Start Time is not a Time\n";
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
                    strErrorMessage = "End Time is not a Time\n";
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

                if (gstrZoneLocation != TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].ZoneLocation)
                {
                    TheFindActiveCustomerByAccountNumberDataSet = TheCustomerClass.FindActiveCustomerByAccountNumber(TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].AccountNumber);

                    intAddressID = TheFindActiveCustomerByAccountNumberDataSet.FindActiveCustomerByAccountNumber[0].AddressID;

                    TheFindAddressByAddressIDDataSet = TheCustomerClass.FindAddressByAddressID(intAddressID);

                    blnFatalError = TheCustomerClass.UpdateCustomerAddress(intAddressID, txtAddress.Text, txtCity.Text, TheFindAddressByAddressIDDataSet.FindAddressByAddressID[0].CustomerState, gintZoneID, TheFindAddressByAddressIDDataSet.FindAddressByAddressID[0].ZipCode);

                    

                    if(blnFatalError == true)
                    {
                        throw new Exception();
                    }
                }

                TheFindScheduledWorkOrderByWorkOrderNumberDataSet = TheWorkOrderScheduleClass.FindWorkOrderScheduledByWorkOrderNumber(MainWindow.gstrWorkOrderNumber);

                intRecordsReturned = TheFindScheduledWorkOrderByWorkOrderNumberDataSet.FindScheduledWorkOrdersByWorkOrderNumber.Rows.Count;

                if(intRecordsReturned == 0)
                {
                    blnFatalError = TheWorkOrderScheduleClass.InsertWorkOrderScheduled(MainWindow.gintWorkOrderID, MainWindow.gintCustomerID, datScheduleDate, timStartTime, timEndTime, -1);

                    if (blnFatalError == true)
                        throw new Exception();
                }
                else
                {
                    intEmployeeID = TheFindScheduledWorkOrderByWorkOrderNumberDataSet.FindScheduledWorkOrdersByWorkOrderNumber[0].EmployeeID;

                    blnFatalError = TheWorkOrderScheduleClass.UpdateWorkOrderScheduled(MainWindow.gintWorkOrderID, datScheduleDate, timStartTime, timEndTime, intEmployeeID);

                    if (blnFatalError == true)
                        throw new Exception();
                }

                blnFatalError = TheWorkOrderClass.UpdateWorkOrderStatusDate(MainWindow.gintWorkOrderID, DateTime.Now);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheWorkOrderClass.UpdateWorkOrderScheduleDate(MainWindow.gintWorkOrderID, datScheduleDate);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheWorkOrderClass.UpdateWorkOrderStatusID(MainWindow.gintWorkOrderID, gintStatusID);

                if (blnFatalError == true)
                    throw new Exception();

                blnFatalError = TheWorkOrderClass.InsertWorkOrderUpdate(MainWindow.gintWorkOrderID, MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID, txtNotes.Text + " Work Order Schedued");

                if (blnFatalError == true)
                    throw new Exception();

                TheMessagesClass.InformationMessage("The Work Order Has Been Updated");
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Schedule Jobs // Schedule Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnPrintWorkORder_Click(object sender, RoutedEventArgs e)
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
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(txtCity.Text + ", " + TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].CustomerState))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("End Time"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(txtEndTime.Text))));

                    intCurrentRow++;

                    WorkOrderTable.RowGroups.Add(new TableRowGroup());
                    WorkOrderTable.RowGroups[0].Rows.Add(new TableRow());

                    newTableRow = WorkOrderTable.RowGroups[0].Rows[intCurrentRow];

                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Name:"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(txtFirstName.Text + " " + txtLastName.Text))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Work Type"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(TheFindWorkOrderByWorkOrderNumberDataSet.FindWorkOrderByWorkOrderNumber[0].WorkType))));

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
            }
            catch (Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Select Jobs // Print Button " + Ex.Message);
            }
        }

        private void cboStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;

            intSelectedIndex = cboStatus.SelectedIndex - 1;

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
    }
}
