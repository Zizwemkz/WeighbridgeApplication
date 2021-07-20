using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using System;
using System.Linq;
using System.Windows;

namespace WeighBridgeApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public string RcmClientID { get; set; }
        public MainWindow(string RcmClientID)
        {
            InitializeComponent();
            MainFrame.Content = new Product_RecieveListView();
            txtUser.Text = RcmClientID;
        }


        private void Logoff_Click(object sender, RoutedEventArgs e)
        {
            Login Window = new Login();
            Window.Show();
            this.Close();

           
        }

        private void Rec_Click(object sender, RoutedEventArgs e)
        {
            //MainFrame.Content = new Product_Recieves(RcmClientID);
            MainFrame.Content = new Product_RecieveListView();
        }

        private void Disp_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new Product_Dispatch();
        }
    }
}