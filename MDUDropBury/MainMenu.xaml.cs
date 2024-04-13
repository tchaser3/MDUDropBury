/* Title:           Main Menu
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
using System.Windows.Shapes;

namespace MDUDropBury
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();

        public MainMenu()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void btnCreateWorkOrder_Click(object sender, RoutedEventArgs e)
        {
            EnterAddress EnterAddress = new EnterAddress();
            EnterAddress.Show();
            Close();
        }

        private void btnScheduleJob_Click(object sender, RoutedEventArgs e)
        {
            SelectWork SelectWork = new SelectWork();
            SelectWork.Show();
            Close();
        }

        private void btnUpdateWorkOrder_Click(object sender, RoutedEventArgs e)
        {
            UpdateWorkOrder UpdateWorkOrder = new UpdateWorkOrder();
            UpdateWorkOrder.Show();
            Close();
        }

        private void btnViewOpenWorkOrders_Click(object sender, RoutedEventArgs e)
        {
            ViewOpenWorkOrders ViewOpenWorkOrders = new ViewOpenWorkOrders();
            ViewOpenWorkOrders.Show();
            Close();
        }

        private void btnViewScheduledWork_Click(object sender, RoutedEventArgs e)
        {
            ViewSelectedWorkOrders ViewSelectedWorkOrders = new ViewSelectedWorkOrders();
            ViewSelectedWorkOrders.Show();
            Close();
        }

        private void btnCancelledOrders_Click(object sender, RoutedEventArgs e)
        {
            CancelledOrders CancelledOrders = new CancelledOrders();
            CancelledOrders.Show();
            Close();
        }

        private void btnClosedOrders_Click(object sender, RoutedEventArgs e)
        {
            ClosedOrders ClosedOrders = new ClosedOrders();
            ClosedOrders.Show();
            Close();
        }
    }
}
