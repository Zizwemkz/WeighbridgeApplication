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
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        int LoginAttempts = 0;
        string connectionString = @"XpoProvider=MSSqlServer;data source=*;user id=sa;password=*;initial catalog=*;Persist Security Info=true";

        public void login()
        {
            PasswordCryptographer.EnableRfc2898 = true;
            PasswordCryptographer.SupportLegacySha512 = false;
            var dictionary = new ReflectionDictionary();
            dictionary.GetDataStoreSchema(typeof(PermissionPolicyUser));
            //var cs = @"integrated security=SSPI;pooling=false;data source=(localdb)\mssqllocaldb;initial catalog=MainDemo_v20.1";
            var dal = XpoDefault.GetDataLayer(connectionString, AutoCreateOption.None);
            var session = new UnitOfWork(dal);
            //Console.WriteLine("Start Time: " + DateTime.Now);
            try
            {
                var user = session.Query<PermissionPolicyUser>().FirstOrDefault(u => u.UserName == txtUsername.Text);
                //Console.WriteLine("End Time: " + DateTime.Now);
                if (user != null)
                {
                    var saltedPassword = (string)user.GetMemberValue("StoredPassword");
                    var flag = PasswordCryptographer.VerifyHashedPasswordDelegate(saltedPassword, txtPassword.Text);

                    if (flag == true)
                    {

                        MainWindow mainWindow = new MainWindow(user.Oid.ToString());
                        mainWindow.RcmClientID = user.Oid.ToString();
                        mainWindow.Show();
                        this.Close();

                        //Navigate to following screen

                    }
                    else
                    {
                        DisplayDialog("An Input Error Occured,Please re-enter password");
                        txtUsername.Text = string.Empty;
                        txtPassword.Text = string.Empty;
                        LoginAttempts = LoginAttempts + 1;
                        if (LoginAttempts == 3)
                        {
                            Environment.Exit(0);
                        }

                    }
                }
                else
                {
                    DisplayDialog("An Input Error Occured, User not found!");
                    LoginAttempts = LoginAttempts + 1;
                    if (LoginAttempts == 3)
                    {
                        Environment.Exit(0);
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            login();
        }
        private async void DisplayDialog(string msg)
        {
            string message = msg;
            string caption = "An Error Occured";
            MessageBoxButton buttons = MessageBoxButton.OKCancel;
            // Show message box
            MessageBoxResult result = MessageBox.Show(message, caption, buttons);
        }
    }
}
