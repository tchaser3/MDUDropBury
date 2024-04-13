/* Title:           Show Work Scheduled
 * Date:            9-25-17
 * Author:          Terry Holmes
 * 
 * Description:     This form will show the work that is scheduled */

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
using WorkOrderScheduleDLL;
using DateSearchDLL;

namespace MDUDropBury
{
    /// <summary>
    /// Interaction logic for ShowWorkScheduled.xaml
    /// </summary>
    public partial class ShowWorkScheduled : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        WorkOrderScheduleClass TheWorkOrderScheduleClass = new WorkOrderScheduleClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();

        FindScheduledWorkOrdersByDateRangeDataSet TheFindScheduledWorkOrdersByDateRangeDataSet = new FindScheduledWorkOrdersByDateRangeDataSet();

        DateTime gdatStartDate;
        DateTime gdatEndDate;

        public ShowWorkScheduled()
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

        private void calSelectDate_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {

            gdatStartDate = Convert.ToDateTime(calSelectDate.SelectedDate);

            gdatEndDate = TheDateSearchClass.AddingDays(gdatStartDate, 1);

            TheFindScheduledWorkOrdersByDateRangeDataSet = TheWorkOrderScheduleClass.FindScheduledWorkOrdersByDateRange(gdatStartDate, gdatStartDate);

            dgrWorkScheduled.ItemsSource = TheFindScheduledWorkOrdersByDateRangeDataSet.FindScheduledWorkOrdersByDateRange;
        }

        private void dgrWorkScheduled_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int intSelectedIndex;
            DataGrid dataGrid;
            DataGridRow Row;
            DataGridCell WorkOrderNumber;

            try
            {
                intSelectedIndex = dgrWorkScheduled.SelectedIndex;

                if(intSelectedIndex > -1)
                {
                    dataGrid = dgrWorkScheduled;
                    Row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                    WorkOrderNumber = (DataGridCell)dataGrid.Columns[1].GetCellContent(Row).Parent;
                    MainWindow.gstrWorkOrderNumber = ((TextBlock)WorkOrderNumber.Content).Text;

                    PrintWorkOrderFromSchedule PrintWorkOrderFromSchedule = new PrintWorkOrderFromSchedule();
                    PrintWorkOrderFromSchedule.ShowDialog();
                    
                    TheFindScheduledWorkOrdersByDateRangeDataSet = TheWorkOrderScheduleClass.FindScheduledWorkOrdersByDateRange(gdatStartDate, gdatStartDate);

                    dgrWorkScheduled.ItemsSource = TheFindScheduledWorkOrdersByDateRangeDataSet.FindScheduledWorkOrdersByDateRange;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "MDU Drop Bury // Show Work Scheduled // Grid Selection Changes " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
