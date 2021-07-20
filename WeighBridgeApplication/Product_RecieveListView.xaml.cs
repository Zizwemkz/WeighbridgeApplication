using DevExpress.Xpf.Grid.Printing;
using DevExpress.Xpo;
using RapidCMv1.Module.BusinessObjects.RapidCMV1_Master;
using RapidCMv1.Module;
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
using DevExpress.Xpf.Grid;

namespace WeighBridgeApplication
{
    /// <summary>
    /// Interaction logic for Product_RecieveListView.xaml
    /// </summary>
    public partial class Product_RecieveListView : Page
    {
        public Product_RecieveListView()
        {
            InitializeComponent();
            PopulateGridControl();
        }

        string connectionString = @"XpoProvider=MSSqlServer;data source=*;user id=sa;password=*;initial catalog=*;Persist Security Info=true";

        public void PopulateGridControl()
        {
            var inMemoryDAL = XpoDefault.GetDataLayer(connectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
            try
            {
                using (var uow = new UnitOfWork(inMemoryDAL))
                {
                    dataGrid1.ItemsSource = new XPCollection<Product_ProductReceive>(uow).Select(i => new { OrderNo = i.RecOrderID.OrderNo, ArrivalDate = i.ArrivalDate, TruckReg = i.TruckRegistration, WbTicket = i.WBTicket, ID = i.ID.ToString() }).ToList();
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
        }
        private void grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
            var row = ItemsControl.ContainerFromElement((DataGrid)sender,
                                                e.OriginalSource as DependencyObject) as DataGridRow;

            if (row == null) return;

            
                Product_Recieves Window = new Product_Recieves(row.Item.ToString());
        }
        private void Row_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
   
        }
        private void Click_Edit(object sender, RoutedEventArgs e)
        {

            //this.Frame.Navigate(typeof(Product_RecievesPage), ((RapidCM_WeighBridge.RapidCM_PGS_Dev.Product_ProductReceive)((Windows.UI.Xaml.FrameworkElement)e.OriginalSource).DataContext).ID);
            GRN_Email report = new GRN_Email();
            //report.Parameters[0].Visible = false;
            report.getId(((System.Windows.FrameworkElement)e.Source).DataContext.ToString());
            //PrintHelper.ShowRibbonPrintPreviewDialog(this, report);
        }
    }
}
