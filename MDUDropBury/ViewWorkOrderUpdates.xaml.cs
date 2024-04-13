/* Title:           View Work Order Updates
 * Date:            9-22-17
 * Author:          Terry Holmes
 * 
 * Description:     This form is for displaying the updates */

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
using System.Windows.Threading;

namespace MDUDropBury
{
    /// <summary>
    /// Interaction logic for ViewWorkOrderUpdates.xaml
    /// </summary>
    public partial class ViewWorkOrderUpdates : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        WorkOrderClass TheWorkOrderClass = new WorkOrderClass();

        FindWorkOrderUpdatesByWorkOrderIDDataSet TheFindWorkOrderUpdatesByWorkOrderID = new FindWorkOrderUpdatesByWorkOrderIDDataSet();

        DispatcherTimer MyTimer = new DispatcherTimer();

        public ViewWorkOrderUpdates()
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
        private void BeginTheProcess(object sender, EventArgs e)
        {
            UpdateGrid();
        }
        private void UpdateGrid()
        {
            TheFindWorkOrderUpdatesByWorkOrderID = TheWorkOrderClass.FindWorkOrderUpdatesByWorkOrderID(MainWindow.gintWorkOrderID);

            dgrUpdates.ItemsSource = TheFindWorkOrderUpdatesByWorkOrderID.FindWorkOrderUpdateByWorkOrderID;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this will load the control
            MyTimer.Tick += new EventHandler(BeginTheProcess);
            MyTimer.Interval = new TimeSpan(0, 0, 2);
            MyTimer.Start();

        }
    }
}
