/* Title:           Enter Address
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
using CustomersDLL;
using NewEventLogDLL;
using DataValidationDLL;

namespace MDUDropBury
{
    /// <summary>
    /// Interaction logic for EnterAddress.xaml
    /// </summary>
    public partial class EnterAddress : Window
    {
        //setting up the class
        WPFMessagesClass TheMessageClass = new WPFMessagesClass();
        CustomersClass TheCustomersClass = new CustomersClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        //setting up the data
        FindAddressByAddressesDataSet TheFindAddressByAddressesDataSet = new FindAddressByAddressesDataSet();
        FindWorkZonesDataSet TheFindWorkZonesDataSet = new FindWorkZonesDataSet();
        FindWorkZoneCityByZipCodeDataSet TheFindWorkZoneCityByZipCodeDataSet = new FindWorkZoneCityByZipCodeDataSet();
        FindWorkZoneByZoneIDDataSet TheFindWorkZoneByZoneIDDataSet = new FindWorkZoneByZoneIDDataSet();
        FindWorkZoneByZoneNameDataSet TheFindWorkZoneByZoneNameDataSet = new FindWorkZoneByZoneNameDataSet();
        FindCustomerAddressDateMatchDataSet TheFindCustomerAddressDateMatchDataSet = new FindCustomerAddressDateMatchDataSet();
        FindWorkZoneCityByCityDataSet TheFindWorkZoneCityByCityDataSet = new FindWorkZoneCityByCityDataSet();
        FindAddressByAddressIDDataSet TheFindAddressByAddressIDDataSet = new FindAddressByAddressIDDataSet();

        //setting global variables
        int gintAddressID;
        bool gblnAddressWasFound;
        bool gblnAddressSelected;
        string gstrAddress;
        string gstrZipCode;
        string gstrState;
        int gintZoneID;
        string gstrCity;
        
        public EnterAddress()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessageClass.CloseTheProgram();
        }

        private void btnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            MainMenu MainMenu = new MainMenu();
            MainMenu.Show();
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this will run during load up
            //setting local variables
            int intCounter;
            int intNumberOfRecords;

            try
            {
                TheFindWorkZonesDataSet = TheCustomersClass.FindWorkZones();

                intNumberOfRecords = TheFindWorkZonesDataSet.FindWorkZones.Rows.Count - 1;

                cboSelectZone.Items.Add("Select Work Zone");

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectZone.Items.Add(TheFindWorkZonesDataSet.FindWorkZones[intCounter].ZoneLocation);
                }

                cboSelectZone.SelectedIndex = 0;

                dgrResults.ItemsSource = TheFindAddressByAddressesDataSet.FindAddressesByAddress;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Enter Address // Window Loaded " + Ex.Message);

                TheMessageClass.ErrorMessage(Ex.ToString());
            }
        }

        private void txtAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            //this will load the grid
            int intLength;
            int intRecordsReturned;
            
            gstrAddress = txtAddress.Text;
            intLength = gstrAddress.Length;
            gintAddressID = 0;

            if(gblnAddressSelected == false)
            {
                ClearControls();
            }

            if(intLength > 3)
            {
                TheFindAddressByAddressesDataSet = TheCustomersClass.FindAddressesByAddress(gstrAddress);

                intRecordsReturned = TheFindAddressByAddressesDataSet.FindAddressesByAddress.Rows.Count;

                if(intRecordsReturned == 0)
                {
                    gblnAddressSelected = false;
                }

                dgrResults.ItemsSource = TheFindAddressByAddressesDataSet.FindAddressesByAddress;
            }

            gblnAddressSelected = false;
            
        }

        private void dgrResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            int intZoneID;
            int intCounter;
            int intNumberOfRecords;
            int intComboSelectedIndex = 0;

            try
            {
                intSelectedIndex = dgrResults.SelectedIndex;

                if(intSelectedIndex > -1)
                {
                    gblnAddressWasFound = true;
                    gstrCity = TheFindAddressByAddressesDataSet.FindAddressesByAddress[intSelectedIndex].City;
                    gstrZipCode = TheFindAddressByAddressesDataSet.FindAddressesByAddress[intSelectedIndex].ZipCode;
                    intZoneID = TheFindAddressByAddressesDataSet.FindAddressesByAddress[intSelectedIndex].ZoneID;
                    gstrState = TheFindAddressByAddressesDataSet.FindAddressesByAddress[intSelectedIndex].CustomerState;
                    gstrAddress = TheFindAddressByAddressesDataSet.FindAddressesByAddress[intSelectedIndex].StreetAddress;
                    gintAddressID = TheFindAddressByAddressesDataSet.FindAddressesByAddress[intSelectedIndex].AddressID;

                    txtCity.Text = gstrCity;
                    
                    txtState.Text = gstrState;

                    TheFindWorkZoneByZoneIDDataSet = TheCustomersClass.FindWorkZoneByZoneID(intZoneID);

                    intNumberOfRecords = TheFindWorkZonesDataSet.FindWorkZones.Rows.Count - 1;

                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        if(intZoneID == TheFindWorkZonesDataSet.FindWorkZones[intCounter].ZoneID)
                        {
                            intComboSelectedIndex = intCounter + 1;
                        }
                    }

                    gblnAddressSelected = true;
                    cboSelectZone.SelectedIndex = intComboSelectedIndex;
                    txtZipCode.Text = gstrZipCode;
                    txtAddress.Text = gstrAddress;
                    gblnAddressSelected = true;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Enter Address // Data Grid Selectgion " + Ex.Message);

                TheMessageClass.ErrorMessage(Ex.ToString());
            }
        }

        private void txtZipCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            //setting local variables
            int intLength;
            string strZone;
            int intRecordsReturned;
            int intCounter;
            int intNumberOfRecords;
            int intSelectedIndex = 0;

            try
            {
                gstrZipCode = txtZipCode.Text;

                intLength = gstrZipCode.Length;

                if(intLength == 5)
                {
                    TheFindWorkZoneCityByZipCodeDataSet = TheCustomersClass.FindWorkZoneCityByZipCode(gstrZipCode);

                    intRecordsReturned = TheFindWorkZoneCityByZipCodeDataSet.FindWorkZoneCityByZipCode.Rows.Count;

                    if(intRecordsReturned == 0)
                    {
                        TheMessageClass.InformationMessage("Zip Code Was Not Found");
                        return;
                    }
                    else
                    {
                        strZone = TheFindWorkZoneCityByZipCodeDataSet.FindWorkZoneCityByZipCode[0].ZoneLocation;

                        intNumberOfRecords = TheFindWorkZonesDataSet.FindWorkZones.Rows.Count - 1;

                        for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                        {
                            if(strZone == TheFindWorkZonesDataSet.FindWorkZones[intCounter].ZoneLocation)
                            {
                                intSelectedIndex = intCounter + 1;
                            }
                        }

                        cboSelectZone.SelectedIndex = intSelectedIndex;
                        txtCity.Text = TheFindWorkZoneCityByZipCodeDataSet.FindWorkZoneCityByZipCode[0].City;
                        txtState.Text = "OH";
                        gstrState = "OH";
                        gstrCity = txtCity.Text;
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Enter Address // Zip Code Text Change " + Ex.Message);

                TheMessageClass.ErrorMessage(Ex.ToString());
            }
        }
        private void ClearControls()
        {
            txtCity.Text = "";
            txtState.Text = "";
            txtZipCode.Text = "";
            cboSelectZone.SelectedIndex = 0;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            DateTime datTransactionDate = DateTime.Now;
            bool blnCreateNewRecord = false;
            bool blnFatalError = false;
            string strErrorMessage = "";
            int intRecordsReturned;
            bool blnNeedsUpdate = false;
            int intCounter;
            bool blnZipMatches;
            
            try
            {
                //data validation
                if(txtAddress.Text == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "Address Was Not Entered\n";
                }
                if(txtZipCode.Text == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "Zip Code Was Not Entered\n";
                }
                if(txtState.Text == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The State Was Not Entered\n";
                }
                if(txtCity.Text == "")
                {
                    blnFatalError = true;
                    strErrorMessage += "The City Was Not Entered\n";
                }
                if(cboSelectZone.SelectedIndex == 0)
                {
                    blnFatalError = true;
                    strErrorMessage += "The Zone Was Not Selected\n";
                }
                if(blnFatalError == true)
                {
                    TheMessageClass.ErrorMessage(strErrorMessage);
                    return;
                }
                else
                {
                    TheFindWorkZoneCityByCityDataSet = TheCustomersClass.FindWorkZoneCityByCity(gstrCity);

                    intRecordsReturned = TheFindWorkZoneCityByCityDataSet.FindWorkZoneCityByCity.Rows.Count;

                    if(intRecordsReturned == 0)
                    {
                        TheMessageClass.ErrorMessage("The City Entered Is Not Located with in Table");
                        return;
                    }
                    else
                    {
                        blnZipMatches = false;

                        for (intCounter = 0; intCounter < intRecordsReturned; intCounter++)
                        {
                            if (gstrZipCode == TheFindWorkZoneCityByCityDataSet.FindWorkZoneCityByCity[intCounter].ZipCode)
                            {
                                blnZipMatches = true;
                            }
                        }

                        if(blnZipMatches == false)
                        {
                            TheMessageClass.ErrorMessage("The City and the Zip Code do not Match");
                        }
                    }
                }
                if(gintAddressID == 0)
                {
                    TheFindAddressByAddressesDataSet = TheCustomersClass.FindAddressesByAddress(gstrAddress);

                    CheckAddress();
                }

                if(gintAddressID == 0)
                {
                    blnCreateNewRecord = true;
                }
                else
                {
                    if(gblnAddressSelected == true)
                    {
                        TheFindAddressByAddressIDDataSet = TheCustomersClass.FindAddressByAddressID(gintAddressID);

                        if (gstrCity != TheFindAddressByAddressIDDataSet.FindAddressByAddressID[0].City)
                            blnNeedsUpdate = true;
                        if (gstrZipCode != TheFindAddressByAddressIDDataSet.FindAddressByAddressID[0].ZipCode)
                            blnNeedsUpdate = true;
                        if (gintZoneID != TheFindAddressByAddressIDDataSet.FindAddressByAddressID[0].ZoneID)
                            blnNeedsUpdate = true;

                        if(blnNeedsUpdate == true)
                        {
                            blnFatalError = TheCustomersClass.UpdateCustomerAddress(gintAddressID, gstrAddress, gstrCity, gstrState, gintZoneID, gstrZipCode);

                            if (blnFatalError == true)
                            {
                                throw new Exception();
                            }
                        }

                        blnCreateNewRecord = false;
                        MainWindow.gintAddressID = gintAddressID;
                    }
                    else
                    {
                        blnCreateNewRecord = true;
                    }
                }
                
                if(blnCreateNewRecord == true)
                {
                    blnFatalError = TheCustomersClass.InsertCustomerAddress(gstrAddress, gstrCity, gstrState, gintZoneID, gstrZipCode, datTransactionDate);

                    if(blnFatalError == true)
                    {
                        throw new Exception();
                    }

                    TheFindCustomerAddressDateMatchDataSet = TheCustomersClass.FindCustomerAddressDateMatch(datTransactionDate);

                    MainWindow.gintAddressID = TheFindCustomerAddressDateMatchDataSet.FindCustomerAddressesDateMatch[0].AddressID;
                }

                CreateWorkOrder CreateWorkOrder = new CreateWorkOrder();
                CreateWorkOrder.Show();
                Close();
                
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Enter Address // Save Button " + Ex.Message);

                TheMessageClass.ErrorMessage(Ex.ToString());
            }
        }
        private void CheckAddress()
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;

            intNumberOfRecords = TheFindAddressByAddressesDataSet.FindAddressesByAddress.Rows.Count - 1;
            
            if(intNumberOfRecords > -1)
            {
                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    gintAddressID = TheFindAddressByAddressesDataSet.FindAddressesByAddress[intCounter].AddressID;
                }
            }
        }
        private void cboSelectZone_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variables
            int intSelectedIndex;

            try
            {
                intSelectedIndex = cboSelectZone.SelectedIndex - 1;

                if(intSelectedIndex > -1)
                {
                    gintZoneID = TheFindWorkZonesDataSet.FindWorkZones[intSelectedIndex].ZoneID;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Select Zone Combo Box Changed Event " + Ex.Message);

                TheMessageClass.ErrorMessage(Ex.ToString());
            }
        }

        private void txtCity_TextChanged(object sender, TextChangedEventArgs e)
        {
            gstrCity = txtCity.Text;
        }

        private void txtState_TextChanged(object sender, TextChangedEventArgs e)
        {
            gstrState = txtState.Text;
        }
    }
}
