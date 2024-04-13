/* Title:           Print Work Order From Schedule
 * Date:            10-10-17
 * Author:          Terry Holmes
 * 
 * Description:     This is will show the information that is to be printed. */

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
using WorkOrderScheduleDLL;

namespace MDUDropBury
{
    /// <summary>
    /// Interaction logic for PrintWorkOrderFromSchedule.xaml
    /// </summary>
    public partial class PrintWorkOrderFromSchedule : Window
    {
        //setting up classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        CustomersClass TheCustomersClass = new CustomersClass();
        WorkOrderClass TheWorkOrderClass = new WorkOrderClass();
        WorkOrderScheduleClass TheWorkOrderScheduleClass = new WorkOrderScheduleClass();
        MDULettersClass TheMDULettersClass = new MDULettersClass();

        FindWorkOrderByWorkOrderNumberDataSet TheFindWorkOrderByWorkOrderNumberNumberDataSet = new FindWorkOrderByWorkOrderNumberDataSet();
        FindScheduledWorkOrdersByWorkOrderNumberDataSet TheFindScheduleWorkOrderByWorkOrderNumberDataSet = new FindScheduledWorkOrdersByWorkOrderNumberDataSet();
        FindCustomerByAccountNumberDataSet TheFindCustomerByAccountNumberDataSet = new FindCustomerByAccountNumberDataSet();
        FindWorkOrderUpdatesByWorkOrderIDDataSet TheFindWorkOrderUpdatesByWorkOrderIDDataSet = new FindWorkOrderUpdatesByWorkOrderIDDataSet();

        string gstrZone;
        string gstrState;
        string gstrAccountNumber;
        string gstrWorkType;
        string gstrStartTime;
        string gstrPhoneNumber;
        string gstrLastName;
        string gstrFirstName;
        string gstrEndTime;
        string gstrDateScheduled;
        string gstrCity;
        string gstrAddress;
        
        public PrintWorkOrderFromSchedule()
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //setting local variables
                        
            try
            {
                TheFindWorkOrderByWorkOrderNumberNumberDataSet = TheWorkOrderClass.FindWorkOrderByWorkOrderNumber(MainWindow.gstrWorkOrderNumber);

                TheFindScheduleWorkOrderByWorkOrderNumberDataSet = TheWorkOrderScheduleClass.FindWorkOrderScheduledByWorkOrderNumber(MainWindow.gstrWorkOrderNumber);

                gstrAccountNumber = TheFindWorkOrderByWorkOrderNumberNumberDataSet.FindWorkOrderByWorkOrderNumber[0].AccountNumber;

                MainWindow.gintWorkOrderID = TheFindWorkOrderByWorkOrderNumberNumberDataSet.FindWorkOrderByWorkOrderNumber[0].WorkOrderID;

                TheFindCustomerByAccountNumberDataSet = TheCustomersClass.FindCustomerByAccountNumber(gstrAccountNumber);

                TheFindWorkOrderUpdatesByWorkOrderIDDataSet = TheWorkOrderClass.FindWorkOrderUpdatesByWorkOrderID(MainWindow.gintWorkOrderID);

                //loading up the variables
                gstrAddress = TheFindCustomerByAccountNumberDataSet.FindCustomerByAccountNumber[0].StreetAddress;
                gstrCity = TheFindCustomerByAccountNumberDataSet.FindCustomerByAccountNumber[0].City;
                gstrDateScheduled = Convert.ToString(TheFindWorkOrderByWorkOrderNumberNumberDataSet.FindWorkOrderByWorkOrderNumber[0].DateScheduled);
                gstrEndTime = Convert.ToString(TheFindScheduleWorkOrderByWorkOrderNumberDataSet.FindScheduledWorkOrdersByWorkOrderNumber[0].EndTime);
                gstrFirstName = TheFindCustomerByAccountNumberDataSet.FindCustomerByAccountNumber[0].FirstName;
                gstrLastName = TheFindCustomerByAccountNumberDataSet.FindCustomerByAccountNumber[0].LastName;
                gstrPhoneNumber = TheFindCustomerByAccountNumberDataSet.FindCustomerByAccountNumber[0].PhoneNumber;
                gstrStartTime = Convert.ToString(TheFindScheduleWorkOrderByWorkOrderNumberDataSet.FindScheduledWorkOrdersByWorkOrderNumber[0].StartTime);
                gstrState = TheFindCustomerByAccountNumberDataSet.FindCustomerByAccountNumber[0].CustomerState;
                gstrWorkType = TheFindWorkOrderByWorkOrderNumberNumberDataSet.FindWorkOrderByWorkOrderNumber[0].WorkType;
                gstrZone = TheFindCustomerByAccountNumberDataSet.FindCustomerByAccountNumber[0].ZoneLocation;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Print Work Order From Schedule // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
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
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(gstrAccountNumber))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Date Scheduled"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(gstrDateScheduled))));

                    intCurrentRow++;

                    WorkOrderTable.RowGroups.Add(new TableRowGroup());
                    WorkOrderTable.RowGroups[0].Rows.Add(new TableRow());

                    newTableRow = WorkOrderTable.RowGroups[0].Rows[intCurrentRow];

                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Address:"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(gstrAddress))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Start Time"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(gstrStartTime))));

                    intCurrentRow++;

                    WorkOrderTable.RowGroups.Add(new TableRowGroup());
                    WorkOrderTable.RowGroups[0].Rows.Add(new TableRow());

                    newTableRow = WorkOrderTable.RowGroups[0].Rows[intCurrentRow];

                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("City:"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(gstrCity + ", " + gstrState))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("End Time"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(gstrEndTime))));

                    intCurrentRow++;

                    WorkOrderTable.RowGroups.Add(new TableRowGroup());
                    WorkOrderTable.RowGroups[0].Rows.Add(new TableRow());

                    newTableRow = WorkOrderTable.RowGroups[0].Rows[intCurrentRow];

                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Name:"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(gstrFirstName + " " + gstrLastName))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Work Type"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(gstrWorkType))));

                    intCurrentRow++;

                    WorkOrderTable.RowGroups.Add(new TableRowGroup());
                    WorkOrderTable.RowGroups[0].Rows.Add(new TableRow());

                    newTableRow = WorkOrderTable.RowGroups[0].Rows[intCurrentRow];

                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Phone Number:"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(gstrPhoneNumber))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run("Zone"))));
                    newTableRow.Cells.Add(new TableCell(new Paragraph(new Run(gstrZone))));

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

                TheMDULettersClass.CreateMDUDropAcceptanceLetter(gstrAccountNumber, "MDU Drop Bury // ");

            }
            catch (Exception Ex)
            {
                TheMessagesClass.ErrorMessage(Ex.ToString());

                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Update Work Order // Print Button " + Ex.Message);
            }
        }
    
    }
}
