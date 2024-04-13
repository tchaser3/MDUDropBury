/* Title:           MDU Drop Bury Main Window
 * Date:            7-27-17
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using NewEventLogDLL;
using NewEmployeeDLL;
using DataValidationDLL;
using CustomersDLL;

namespace MDUDropBury
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //setting up th classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        CustomersClass TheCustomersClass = new CustomersClass();
        
        public static VerifyLogonDataSet TheVerifyLogonDataSet = new VerifyLogonDataSet();
        public static FindCustomersByAddressIDDataSet TheFindCustomersByAddressIDDataSet = new FindCustomersByAddressIDDataSet();

        int gintNoOfMisses;
        public static int gintAddressID;
        public static int gintCustomerID;
        public static int gintWorkOrderID;
        public static string gstrWorkOrderNumber;
        public static string gstrAddress;
        public static string gstrAccountNumber;
        public static bool gblnFormSet;
        public static bool gblnWorkOrderSet;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strValueForValidation;
            int intEmployeeID = 0;
            string strLastName;
            bool blnFatalError = false;
            int intRecordsReturned;
            string strErrorMessage = "";

            //beginning data validation
            strValueForValidation = pbxPassword.Password;
            strLastName = txtLastName.Text;
            blnFatalError = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
            if (blnFatalError == true)
            {
                strErrorMessage = "The Employee ID is not an Integer\n";
            }
            else
            {
                intEmployeeID = Convert.ToInt32(strValueForValidation);
            }
            if (strLastName == "")
            {
                blnFatalError = true;
                strErrorMessage += "The Last Name Was Not Entered\n";
            }
            if (blnFatalError == true)
            {
                TheMessagesClass.ErrorMessage(strErrorMessage);
                return;
            }

            //filling the data set
            TheVerifyLogonDataSet = TheEmployeeClass.VerifyLogon(intEmployeeID, strLastName);

            intRecordsReturned = TheVerifyLogonDataSet.VerifyLogon.Rows.Count;

            if (intRecordsReturned == 0)
            {
                LogonFailed();
            }
            else
            {
                if(TheVerifyLogonDataSet.VerifyLogon[0].EmployeeGroup != "USERS")
                {
                    MainMenu MainMenu = new MainMenu();
                    MainMenu.Show();
                    Hide();
                }
            }
            
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            gintNoOfMisses = 0;

            pbxPassword.Focus();
        }
        private void LogonFailed()
        {
            gintNoOfMisses++;

            if (gintNoOfMisses == 3)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "There Have Been Three Attemps to Sign Into MDU Drop Buries");

                TheMessagesClass.ErrorMessage("You Have Tried To Sign In Three Times\nThe Program Will Now Close");

                Application.Current.Shutdown();
            }
            else
            {
                TheMessagesClass.InformationMessage("You Have Failed The Sign In Process");
                return;
            }
        }
    }
}
