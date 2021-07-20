using DevExpress.Xpo;
using RapidCMv1.Module;
using RapidCMv1.Module.BusinessObjects.RapidCMV1_Master;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;


namespace WeighBridgeApplication
{
    /// <summary>
    /// Interaction logic for Product_Recieves.xaml
    /// </summary>
    public partial class Product_Recieves : Page, INotifyPropertyChanged
    {
        public string RcmClientID { get; set; }
        public Product_Recieves(string RcmClientID)
        {

            InitializeComponent();
            SetFakeData();
            Console.WriteLine("========================================= Start Time: " + DateTime.Now);

            populateLookupEdits();
            Console.WriteLine("========================================== End Time: " + DateTime.Now);

            txtComments.Text = RcmClientID;
            DataContext = new WeightViewModel();

        }




        string connectionString = @"XpoProvider=MSSqlServer;data source=*;user id=sa;password=*;initial catalog=*;Persist Security Info=true";
        //string connectionString = @"XpoProvider=MSSqlServer;data source=DESKTOP-32QVBUV\SQL2017;user id=201619;password=pPqtKc19;initial catalog=RapidCM_Master_Dev2;Persist Security Info=true";

        // private INavigationService NavigationService { get { return this.GetService<INavigationService>(); } }
        bool updateRec = false;
        int RecId = 0;

        double tar, gross;
        string Transporter, OrderID, TruckType;
        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            TestXtraReport report = new TestXtraReport();
            report.Parameters[0].Visible = false;
            //report.getId(99659);
            //PrintHelper.ShowRibbonPrintPreviewDialog(this, report);
        }
        private void SetFakeData()
        {
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.5);
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private int _Weight;
        public int Weight
        {
            get
            {
                return _Weight;
            }
            set
            {
                _Weight = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

        }
        private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void Timer_Tick(object sender, object e)
        {
            Random random = new Random();
            Weight = random.Next(0, 40);
        }

        private void Click_Edit(object sender, RoutedEventArgs e)
        {

            //this.Frame.Navigate(typeof(Product_RecievesPage), ((RapidCM_WeighBridge.RapidCM_PGS_Dev.Product_ProductReceive)((Windows.UI.Xaml.FrameworkElement)e.OriginalSource).DataContext).ID);
            TestXtraReport report = new TestXtraReport();
            report.Parameters[0].Visible = false;
            //report.getId(99659);
            //PrintHelper.ShowRibbonPrintPreviewDialog(this, report);
        }
        public void populateLookupEdits()
        {
            var inMemoryDAL = XpoDefault.GetDataLayer(connectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
            try
            {
                using (var uow = new UnitOfWork(inMemoryDAL))
                {
                    TransComboBox.ItemsSource = uow.Query<Library_Transporter>().Select(i => new { ID = (i.ID).ToString(), Name = i.Name }).ToList();
                    TransComboBox.DisplayMember = "Name";

                    SupplierComboBox.ItemsSource = uow.Query<Library_Transporter>().Select(i => new { ID = (i.ID).ToString(), Name = i.Name }).ToList();
                    SupplierComboBox.DisplayMember = "Name";

                    OrderLookup.ItemsSource = uow.Query<Order_OrderIn>().Select(i => new { ID = (i.ID).ToString(), OrderNumber = i.OrderNo }).ToList();
                    OrderLookup.DisplayMember = "OrderNumber";

                    ImgCombobox.ItemsSource = uow.Query<Library_TruckType>().Select(i => new { Image = ImageFromBytes(i.Image), Name = i.Name }).ToList();
                    ImgCombobox.DisplayMember = "Name";
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
        }

        private void TransComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(Transporter))
            {
                Transporter = ((System.Windows.Controls.ContentControl)((object[])e.AddedItems)[0]).Content.ToString();
            }
        }
        private void SupplierComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        //insert records
        private void commitButton_Click(object sender, RoutedEventArgs e)
        {
            //validateInput();

            if (updateRec == true)
            {
                updateRecord(RecId);

            }
            else
            {
                saveRecord();
                //NavigationService.Navigate("GridWindow", null, this);

            }
            //this.Frame.Navigate(typeof(ProductRecieveListViewPage));
            //this.NavigationService.Navigate(newpage);
        }
        public void updateRecord(int id)
        {
            var inMemoryDAL = XpoDefault.GetDataLayer(connectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
            id = RecId;
            try
            {

                using (var uow = new UnitOfWork(inMemoryDAL))
                {
                    Order_OrderIn searchItem = new XPQuery<Order_OrderIn>(uow).FirstOrDefault(i => i.ID.Equals(""));
                    //ExpectedTrucks searchTruck = new XPQuery<ExpectedTrucks>(uow).FirstOrDefault(i => i.ID == Convert.ToInt32(lblExpTruck.Text));
                    //Library_Supplier L_SearchItem = new XPQuery<Library_Supplier>(uow).FirstOrDefault(i => i.Name == (SupplierComboBox.SelectedValue.ToString()));
                    Library_Transporter LT_SearchItem = new XPQuery<Library_Transporter>(uow).FirstOrDefault(i => i.Name == (TransComboBox.SelectedItemValue.ToString()));
                    Product_ProductReceive pr = new XPQuery<Product_ProductReceive>(uow).FirstOrDefault(i => i.ID.Equals(id));
                    tar = Convert.ToDouble(txtTar.Text);
                    gross = Convert.ToDouble(txtGross.Text);
                    txtNett.Text = calcNettWeight(tar, gross).ToString();
                    pr.TarWeight = tar;
                    pr.GrossWeight = gross;
                    pr.NettWeight = calcNettWeight(tar, gross);
                    //pr.SupplierID = L_SearchItem;
                    pr.RecOrderID = searchItem;
                    //pr.ExpectedID = searchTruck;
                    pr.TransporterID = LT_SearchItem;
                    //pr.DateModified = DateTime.Now;
                    pr.DriverName = newDriver.Text;
                    pr.NettWeight = calcNettWeight(Convert.ToDouble(txtGross.Text), Convert.ToDouble(txtNett.Text));
                    uow.CommitChanges();

                }
            }
            catch (Exception ex)
            {

            }
        }



        public void saveRecord()
        {
            var inMemoryDAL = XpoDefault.GetDataLayer(connectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);

            validateInput();
            try
            {
                using (var uow = new UnitOfWork(inMemoryDAL))
                {

                    OrderID = OrderLookup.SelectedItem.ToString();
                    TruckType = ImgCombobox.SelectedText.ToString();
                    Transporter = TransComboBox.SelectedItem.ToString();
                    //Order_OrderIn OrderSearchItem = uow.Query<Order_OrderIn>().FirstOrDefault(i => i.ID==Guid.Parse("6FCD8E0E-E69B-4896-92F5-2EEAF73EC54E"));
                    Order_OrderIn OrderSearchItem = uow.Query<Order_OrderIn>().FirstOrDefault(i => i.ID == cleanString(OrderID));


                    Library_TruckType TruckSearchItem = new XPQuery<Library_TruckType>(uow).FirstOrDefault(i => i.Name == TruckType);


                    Library_Transporter TransSearchItem = uow.Query<Library_Transporter>().FirstOrDefault(i => i.ID == cleanString(Transporter));

                    Product_ProductReceive pr = new Product_ProductReceive(uow);

                    try
                    {
                        tar = Convert.ToDouble(txtTar.Text);
                        gross = Convert.ToDouble(txtGross.Text);
                        txtNett.Text = calcNettWeight(tar, gross).ToString();
                        pr.NettWeight = Convert.ToDouble(txtNett.Text);
                        pr.RecOrderID = OrderSearchItem;
                        pr.TransporterID = TransSearchItem;
                        pr.TruckRegistration = txtReg.Text;
                        pr.OriginDate = DateTime.Parse(txtOriginDate.Text);
                        pr.ArrivalDate = DateTime.Parse(ArrivalDate.Text);
                        pr.DriverName = newDriver.Text;
                        pr.OriginWeighbridgeTicketNo = txtOriginWB.Text;
                        pr.OriginDeliveryNote = txtOriginNote.Text;
                        pr.OriginNettWeight = Convert.ToDouble(txtOriginNett.Text);
                        pr.SplitWBTicket = Convert.ToInt16(txtSplitWBTicket.Text);
                        if ((bool)SplitCheck.IsChecked)
                        {
                            pr.SplitTicket = true;
                        }


                        //pr.RCMClientID = uow.Query<Library_RCMClient>().FirstOrDefault(i => i.ID == Guid.Parse("A26393D4-6D7F-4A7B-8903-2FFF7A638D36"));
                        pr.NettWeight = calcNettWeight(Convert.ToDouble(txtGross.Text), Convert.ToDouble(txtNett.Text));
                        pr.WBTicket = Convert.ToInt32(txtWbTicket.Text);
                        pr.Save();
                    }
                    catch (Exception ex)
                    {
                        DisplayDialog("An Input Error Occured" + ex.Message.ToString());
                        return;
                    }
                    uow.CommitChanges();
                    clearInput();
                    DisplayDialog("Transaction Saved");
                }

            }

            catch (Exception ex)
            {
                var Name = ex.Message;

                DisplayDialog("An input error occured" + Name.ToString());
            }
        }
        public void clearInput()
        {

            txtWbTicket.Text = null;
            newDriver.Text = null;
            txtNote.Text = null;
            txtNett.Text = null;
            txtGross.Text = null;
            txtRef.Text = null;
            txtSealNo.Text = null;
            txtTar.Text = null;
            ImgCombobox.SelectedItem = null;
            SupplierComboBox.SelectedItem = null; 
            TransComboBox.SelectedItem = null;
            OrderLookup.SelectedItem = null;
            txtComments.Text = null;
            txtReg.Text = null;
            ArrivalDate.Text = null;
            txtSequence.Text = null;
            SplitCheck.IsChecked = null;
            txtOriginWB.Text = null;
            txtOriginNote.Text = null;
            txtOriginNett.Text = null;
            txtSplitWBTicket.Text = null;
            txtOriginDate.Text = null;



            //validateInput();
        }
        public double calcNettWeight(double tar, double gross)
        {
            double nett = 0;
            if (tar < gross)
            {
                nett = (gross - tar);
            }
            else
            {
                nett = 0;
                //DisplayDialog("The TarWeight cannot be greater than the GrossWeight");
            }
            return nett;
        }

        public static BitmapImage ImageFromBytes(Byte[] bytes)
        {
            BitmapImage image = new BitmapImage();

            try
            {
                using (var mem = new MemoryStream(bytes))
                {
                    mem.Position = 0;
                    image.BeginInit();
                    //image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    //image.UriSource = null;
                    image.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                    image.StreamSource = mem;
                    image.EndInit();
                }
                image.Freeze();

            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            return image;
        }

        private void GetEmpty_Click(object sender, RoutedEventArgs e)
        {
            txtTar.Text = Weight.ToString();

        }

        private void GetFull_Click(object sender, RoutedEventArgs e)
        {

            txtGross.Text = Weight.ToString();
            txtNett.Text = (calcNettWeight(Convert.ToDouble(txtGross.Text), Convert.ToDouble(txtTar.Text)).ToString());
        }
        private async void DisplayDialog(string msg)
        {
            string message = msg;
            string caption = "An Error Occured";
            MessageBoxButton buttons = MessageBoxButton.OKCancel;
            // Show message box
            MessageBoxResult result = MessageBox.Show(message, caption, buttons);
        }
        private void dataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        protected void OnNavigatedTo(NavigationEventArgs e)
        {
            //base.OnNavigatedTo(e);
            updateRec = true;
            //ExpectedTrucksComboBox expCombo = new ExpectedTrucksComboBox();
            //var parameters = (Product_ProductReceive)e.Parameter;
            try
            {
                var param = e.Content;
                RecId = Convert.ToInt32(param);
                var inMemoryDAL = XpoDefault.GetDataLayer(connectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
                using (var uow = new UnitOfWork(inMemoryDAL))
                {
                    //XPQuery<Product_ProductReceive> items = new XPQuery<Product_ProductReceive>(uow);
                    Product_ProductReceive searchItem = new XPQuery<Product_ProductReceive>(uow).FirstOrDefault(i => i.ID.Equals(param));

                    if (searchItem.WBTicket != 0)
                    {
                        txtWbTicket.Text = (searchItem.WBTicket).ToString();
                    }
                    if (!String.IsNullOrEmpty(searchItem.DriverName))
                    {
                        newDriver.Text = (searchItem.DriverName).ToString();
                    }
                    if (!String.IsNullOrEmpty(searchItem.Seal1Number))
                    {
                        txtSealNo.Text = (searchItem.Seal1Number).ToString();
                    }
                    //if (searchItem.ExpectedID != null)
                    //{
                    //    //expCombo.ExpectedId = searchItem.ExpectedID.ID;
                    //}
                    if (!String.IsNullOrEmpty(searchItem.Seal1Number))
                    {
                        txtSealNo.Text = (searchItem.Seal1Number).ToString();
                    }
                    if (!String.IsNullOrEmpty(searchItem.Seal1Number))
                    {
                        txtSealNo.Text = (searchItem.Seal1Number).ToString();
                    }
                    if (!String.IsNullOrEmpty(searchItem.Seal1Number))
                    {
                        txtSealNo.Text = (searchItem.Seal1Number).ToString();
                    }
                    //if (searchItem.TransporterID != null)
                    //{
                    //    TransComboBox.SelectedIndex = searchItem.TransporterID.ID;
                    //}
                    //if (searchItem.SiteID != null)
                    //{
                    //    TransComboBox.SelectedIndex = searchItem.SiteID.ID;
                    //}

                    //newWbTicket.Text = (searchItem.WBTicket).ToString();
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            //return true;
        }

        private void OrderLookup_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            //var value = ((System.Windows.FrameworkElement)e.Source)..ToString();

        }

        public void validateInput()
        {

            if (String.IsNullOrEmpty(txtSealNo.Text))
            {
                txtSealNo.Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                txtSealNo.Background = new SolidColorBrush(Colors.Transparent);
            }

            if (String.IsNullOrEmpty(txtTar.Text))
            {
                txtTar.Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                txtTar.Background = new SolidColorBrush(Colors.Transparent);
            }

            if (String.IsNullOrEmpty(txtWbTicket.Text))
            {
                txtWbTicket.Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                txtWbTicket.Background = new SolidColorBrush(Colors.Transparent);
            }
            if (String.IsNullOrEmpty(txtNote.Text))
            {
                txtNote.Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                txtNote.Background = new SolidColorBrush(Colors.Transparent);
            }
            if (String.IsNullOrEmpty(txtRef.Text))
            {
                txtRef.Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                txtRef.Background = new SolidColorBrush(Colors.Transparent);
            }
            if (String.IsNullOrEmpty(newDriver.Text))
            {
                newDriver.Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                newDriver.Background = new SolidColorBrush(Colors.Transparent);
            }
            if (String.IsNullOrEmpty(txtGross.Text))
            {
                txtGross.Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                txtGross.Background = new SolidColorBrush(Colors.Transparent);
            }
            if (String.IsNullOrEmpty(txtNett.Text))
            {
                txtNett.Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                txtNett.Background = new SolidColorBrush(Colors.Transparent);
            }
            if (SupplierComboBox.SelectedItemValue == null)
            {
                SupplierComboBox.Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                SupplierComboBox.Background = new SolidColorBrush(Colors.Transparent);
            }
            if (ImgCombobox.SelectedItem == null)
            {
                ImgCombobox.Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                ImgCombobox.Background = new SolidColorBrush(Colors.Transparent);
            }
            //TransComboBox
            if (TransComboBox.SelectedItemValue == null)
            {
                TransComboBox.Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                TransComboBox.Background = new SolidColorBrush(Colors.Transparent);
            }

        }
        public Guid cleanString(string val)
        {
            string[] list = val.Split('=');
            string NewVal = list[1].Split(',')[0];
            Guid order = Guid.Parse(NewVal.Trim());
            return order;
        }

    }
    public class WeightViewModel : DependencyObject
    {


        public static readonly DependencyProperty HourProperty =
            DependencyProperty.Register("Hour", typeof(double), typeof(WeightViewModel));
        public static readonly DependencyProperty MinuteProperty =
            DependencyProperty.Register("Minute", typeof(double), typeof(WeightViewModel));
        public static readonly DependencyProperty ScaleWeightProperty =
            DependencyProperty.Register("ScaleWeight", typeof(double), typeof(WeightViewModel));
        public static readonly DependencyProperty TareWeightProperty =
            DependencyProperty.Register("TareWeight", typeof(double), typeof(WeightViewModel));
        public static readonly DependencyProperty GrossWeightProperty =
            DependencyProperty.Register("GrossWeight", typeof(double), typeof(WeightViewModel));

        public static readonly DependencyProperty Exel1WeightProperty =
            DependencyProperty.Register("Exel1Weight", typeof(double), typeof(WeightViewModel));

        public static readonly DependencyProperty Exel2WeightProperty =
           DependencyProperty.Register("Exel2Weight", typeof(double), typeof(WeightViewModel));

        public static readonly DependencyProperty Exel3WeightProperty =
           DependencyProperty.Register("Exel3Weight", typeof(double), typeof(WeightViewModel));

        public static readonly DependencyProperty Exel4WeightProperty =
           DependencyProperty.Register("Exel4Weight", typeof(double), typeof(WeightViewModel));

        public static readonly DependencyProperty Exel5WeightProperty =
           DependencyProperty.Register("Exel5Weight", typeof(double), typeof(WeightViewModel));

        public static readonly DependencyProperty Exel6WeightProperty =
           DependencyProperty.Register("Exel6Weight", typeof(double), typeof(WeightViewModel));

        public static readonly DependencyProperty Exel7WeightProperty =
           DependencyProperty.Register("Exel7Weight", typeof(double), typeof(WeightViewModel));

        public static readonly DependencyProperty Exel8WeightProperty =
           DependencyProperty.Register("Exel8Weight", typeof(double), typeof(WeightViewModel));

        public double Hour
        {
            get { return (double)GetValue(HourProperty); }
            set { SetValue(HourProperty, value); }
        }

        public double Minute
        {
            get { return (double)GetValue(MinuteProperty); }
            set { SetValue(MinuteProperty, value); }
        }

        public double ScaleWeight
        {
            get { return (double)GetValue(ScaleWeightProperty); }
            set { SetValue(ScaleWeightProperty, value); }
        }

        public double TareWeight
        {
            get { return (double)GetValue(TareWeightProperty); }
            set { SetValue(TareWeightProperty, value); }
        }

        public double GrossWeight
        {
            get { return (int)GetValue(GrossWeightProperty); }
            set { SetValue(GrossWeightProperty, value); }
        }

        public double Exel1Weight
        {
            get { return (double)GetValue(Exel1WeightProperty); }
            set { SetValue(Exel1WeightProperty, value); }
        }

        public double Exel2Weight
        {
            get { return (double)GetValue(Exel2WeightProperty); }
            set { SetValue(Exel2WeightProperty, value); }
        }
        public double Exel3Weight
        {
            get { return (double)GetValue(Exel3WeightProperty); }
            set { SetValue(Exel3WeightProperty, value); }
        }
        public double Exel4Weight
        {
            get { return (double)GetValue(Exel4WeightProperty); }
            set { SetValue(Exel4WeightProperty, value); }
        }
        readonly DispatcherTimer timer = new DispatcherTimer();

        public WeightViewModel()
        {
            timer.Tick += new EventHandler(OnTimedEvent);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
            Update();
        }

        void Update()
        {
            DateTime currentDate = DateTime.Now;
            Random random = new Random();

            ScaleWeight = random.Next(0, 50);
            Exel1Weight = random.Next(10, 30);
            Exel2Weight = random.Next(15, 45);
            Exel3Weight = random.Next(25, 55);
            Exel4Weight = random.Next(10, 35);

            Minute = ((currentDate.Minute + currentDate.Second / 60.0) / 60.0) * 12;
            Hour = (currentDate.Hour % 12) + currentDate.Minute / 60.0;

            //GrossWeight = currentDate.Day;
            //TareWeight = (int)currentDate.TareWeight;
            //Exel1Weight = currentDate.Month;
        }

        void OnTimedEvent(object source, EventArgs e)
        {
            Update();
        }
    }

}
