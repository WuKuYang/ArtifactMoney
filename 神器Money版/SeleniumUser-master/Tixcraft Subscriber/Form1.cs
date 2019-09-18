using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Drawing.Imaging; 
using TSubscriber;
using SQLTest;
using OCRInspection;
using HTool;
using System.Net;
using TixcraftAutomationSystem;
using VIPGeneral;
using System.Reflection;
using HalconDotNet;
using System.Text.RegularExpressions;

namespace Tixcraft_Subscriber
{
    public partial class Form1 : Form
    {

        public string g_AnswerSwitchText = "";

        public string strFormatEmailInfo = "";
        public string strPinelPinPassword = "";
        public List<TixcraftSubscriber.Activity> lstShow = new List<TixcraftSubscriber.Activity>();
        public int g_ShowSelected = 6;
        public int g_iThreadsVerCode = 1;//自動打碼的倍率(一個問題請幾個人打)

        public bool g_bIsListen_OCR_History = false; //是否紀錄驗證碼歷程
        public bool g_bFlagStartBuyStatue = false;  //搶票流程開關

        public int g_Yw0_DelayTime = 80; // 滑鼠點擊Yw0元素後，等候多久再進行屏幕截圖 ( 單位 ms )

        //自動打驗證碼
        const bool g_bIsAutoKeyIn = true; 
        bool g_bIsRandSeats = false;
        const bool g_bIsAutoByPassQuestion = true;   //自動回答防黃牛提問 
        bool g_bIsUsing_OtherAnswerByWindow = false;    //回答防黃牛問題時，是否使用自定義答案
        int g_PayMode = -1;//-1=不執行  0 => ATM付款   1 = ibon付款  2 = 信用卡
        /// <summary>
        /// -1=不執行  0 => ATM付款   1 = ibon付款  2 = 信用卡
        /// </summary>
        public int PayMode 
        {
            get { return g_PayMode; }
            set { g_PayMode = value; }
        }
        string g_OtherAnswerByWindow = "";
        public string OCRRecipe_IP_Address = "127.0.0.1";
        public string g_strProxyInfo = "proxy.hinet.net:80";
        TixcraftSQLServer g_ShareAnswerServer = new TixcraftSQLServer();
        TixcraftCookieServer g_CookieServer = new TixcraftCookieServer();
        //20190729 add 
        TixcraftSubscriber g_tTSBuyer = new TixcraftSubscriber();
        TixcraftSubscriber Tixcraft = new TixcraftSubscriber();
        FastHttpWebDriver FastHttpDriver = new FastHttpWebDriver(); 
        mFacebook SelfFaceBook = new mFacebook(); 
        public Subscriber SubscrEr = new Subscriber();  
        /// <summary>
        /// true = using GoogleChrome , false = using PhantomJSDriver
        /// </summary>
        public bool bIsOpenWithGoogleChrome = true;
        public bool bIsMountProxy  = false;
        /// <summary>
        /// 瀏覽器是否忙碌中( 搶票中、載入中...等)
        /// </summary>
        public bool bIsBrowserBusying = false;
        public bool bIsSwitchPageStepByStep = false; //搶票時，是否進行頁面跳轉的動作(1->3) 或 ( 1 > 2 > 3 )  
        public bool bIsASK_CheckPage = false;       //在新版流程內，使用提早訪問Check功能，看看是否會提早取得票務資訊
        public bool bIsUseOLDsch = false;           //使用舊版流程 ( 舊版流程 : 不使用封包 )
        string g_SeatInformation = "";
        bool g_IsAutoFlag = false; // 是否上線掛機
        OCRServer myOCRServer = new OCRServer();

        public bool g_bIsDelayTimerEnable = false;  //20190915 開賣智能延遲器 , 功能開關
        public Stopwatch g_DelayTimer = new Stopwatch();    //20190915 開賣後網址延遲器，讀取網址成功後，延遲N秒後再進行提交比單的計時器
        public bool g_bIsDelayTimerRunning = false; //20190915 開賣智能延遲器，紀錄計時器狀態使用
        public long g_DelayThreshold = 800;    //20190915 開賣後0.8秒再進行送出

        public List<OCR_History> g_lstOCR_History = new List<OCR_History>();

        public Bitmap bSnapShot = new Bitmap(10, 10);

        private delegate void udDrawImage(PictureBox ctr, Bitmap msg);
        private udDrawImage degDrawImageVirfyCode;
        private delegate void udRefreshText(Control ctr, string msg);
        private udRefreshText degRefreshText;
        private delegate void udRefreshBox(ListBox lstBox, List<TixcraftSubscriber.Activity> act);
        private udRefreshBox degRefreshBox;
        private delegate void udControlEnable(Control ctr, bool bIsEnable);
        private udControlEnable degControlEnable;
        public LoginType LoginMode = LoginType.PixelPin;

        public enum Human_DelayMode 
        {
            DoNotDelay ,
            DelayMilliSecond,
            DelayRand
            
        }

        public class HDelayManager
        {
            public Human_DelayMode mode = Human_DelayMode.DoNotDelay;
            public int DelayMin_ms = 0;
            public int DelayMax_ms = 0;
            public int Delay_ms = 0;
        }

        public class OCR_History
        {
            public Bitmap ImageA = null;
            public Bitmap ImageB = null;
            public Bitmap ImageC = null;
            public Bitmap ImageD = null;
            public Bitmap SourceImage = null;
            public string Answer = "";
        }


        #region Sam => 委派更新畫面
        private delegate void myIGCallBack(Image myIG, PictureBox ctl);
        private delegate void myTxtCallBack(string text, Label ctl);
        private delegate void myTxtBoxCallBack(string text, TextBox ctl);
        private delegate void myComboBoxCallBack(List<string> text, int index, ComboBox ctl);
        private delegate void myControlCallBack(string text, Control ctl);
        private delegate void myControlVisibleCallBack(bool value, Control ctl);
        private delegate void myFormCallBack(string text, Form ctl);
        private delegate void myWebCallBack(string text, WebBrowser ctl);
        private delegate void myCokorbCallBack(Color color, Control ctl);



        private delegate void myCircleUpdateBar(CircularProgressBar.CircularProgressBar bar, int value);
        private delegate void myCircleText(CircularProgressBar.CircularProgressBar bar, string value);
        private delegate void myCircleSpeed(CircularProgressBar.CircularProgressBar bar, int value);
        private delegate void myCircleVisiable(CircularProgressBar.CircularProgressBar bar, bool value);

        private void UpdateCircleBar(CircularProgressBar.CircularProgressBar bar, int value)
        {

            if (this.InvokeRequired)
            {
                myCircleUpdateBar myUpdate = new myCircleUpdateBar(UpdateCircleBar);
                this.Invoke(myUpdate, bar, value);
            }
            else
            {
                circularProgressBar1.Value = value;
                circularProgressBar1.Update();
            }
        }
        private void UpdateCircleText(CircularProgressBar.CircularProgressBar bar, string value)
        {

            if (this.InvokeRequired)
            {
                myCircleText myUpdate = new myCircleText(UpdateCircleText);
                this.Invoke(myUpdate, bar, value);
            }
            else
            {
                circularProgressBar1.Text = value;
                circularProgressBar1.Update();
            }
        }
        private void UpdateCircleSpeed(CircularProgressBar.CircularProgressBar bar, int value)
        {

            if (this.InvokeRequired)
            {
                myCircleSpeed myUpdate = new myCircleSpeed(UpdateCircleSpeed);
                this.Invoke(myUpdate, bar, value);
            }
            else
            {
                circularProgressBar1.MarqueeAnimationSpeed = value;
                circularProgressBar1.Update();
            }
        }
        private void UpdateCircleVisiable(CircularProgressBar.CircularProgressBar bar, bool value)
        {

            if (this.InvokeRequired)
            {
                myCircleVisiable myUpdate = new myCircleVisiable(UpdateCircleVisiable);
                this.Invoke(myUpdate, bar, value);
            }
            else
            {
                circularProgressBar1.Visible = value;
            }
        }


        /// <summary>
        /// 更新圖片
        /// </summary>
        /// <param name="myIG"></param>
        /// <param name="ctl"></param>
        private void UpdateImage(Image myIG, PictureBox ctl)
        {

            if (this.InvokeRequired)
            {
                myIGCallBack myUpdate = new myIGCallBack(UpdateImage);
                this.Invoke(myUpdate, myIG, ctl);
            }
            else
            {
                ctl.BackgroundImage = myIG;
                ctl.Refresh();
            }
        }
        private void UpdateControl(string text, Control ctl)
        {

            if (this.InvokeRequired)
            {
                myControlCallBack myUpdate = new myControlCallBack(UpdateControl);
                this.Invoke(myUpdate, text, ctl);
            }
            else
            {
                ctl.Text = text;
                ctl.Refresh();
            }
        }
        private void UpdateControl(bool text, Control ctl)
        {

            if (this.InvokeRequired)
            {
                myControlVisibleCallBack myUpdate = new myControlVisibleCallBack(UpdateControl);
                this.Invoke(myUpdate, text, ctl);
            }
            else
            {
                ctl.Visible = text;

            }
        }
        private void UpdateForm(string text, Form ctl)
        {

            if (this.InvokeRequired)
            {
                myFormCallBack myUpdate = new myFormCallBack(UpdateControl);
                this.Invoke(myUpdate, text, ctl);
            }
            else
            {
                ctl.Text = text;
                ctl.Refresh();
            }
        }
        private void UpdateBackColor(Color color, Control ctl)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    myCokorbCallBack myUpdate = new myCokorbCallBack(UpdateBackColor);
                    this.Invoke(myUpdate, color, ctl);
                }
                else
                {
                    ctl.BackColor = color;
                }
            }
            catch (Exception ex)
            {

            }

        }
        private void UpdateLable(string text, Label ctl)
        {
            try
            { 
                if (this.InvokeRequired)
                {
                    myTxtCallBack myUpdate = new myTxtCallBack(UpdateLable);
                    this.Invoke(myUpdate, text, ctl);
                }
                else
                {
                    ctl.Text = text;
                    ctl.Refresh();
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void UpdateText(string text, TextBox ctl)
        {
            try
            { 
                if (this.InvokeRequired)
                {
                    myTxtBoxCallBack myUpdate = new myTxtBoxCallBack(UpdateText);
                    this.Invoke(myUpdate, text, ctl);
                }
                else
                {
                    ctl.Text = text;
                    ctl.Refresh();
                }
            }
            catch (Exception ex)
            {
                 
            }
        }
        private void UpdateWeb(string text, WebBrowser ctl)
        {
            try
            { 
                if (this.InvokeRequired)
                {
                    myWebCallBack myUpdate = new myWebCallBack(UpdateWeb);
                    this.Invoke(myUpdate, text, ctl);
                }
                else
                {
                    ctl.DocumentText = text;
                }
            }
            catch (Exception ex)
            {
                 
            }

        }
        private void UpdateComboBox(List<string> text, int index, ComboBox ctl)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    myComboBoxCallBack myUpdate = new myComboBoxCallBack(UpdateComboBox);
                    this.Invoke(myUpdate, text, index, ctl);
                }
                else
                {
                    ctl.Items.Clear();
                    if (text != null)
                    {
                        ctl.Items.AddRange(text.ToArray());
                        ctl.SelectedIndex = index;
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        #endregion 

        public void DrawImage(PictureBox ctr, Bitmap msg)
        {
            try
            { 
                ctr.Image = msg;
            }
            catch (Exception ex)
            {
                 
            }
        }
        public void RefreshText(Control ctr, string msg)
        {
            ctr.Text = msg;
        }
        public void ControlEnable(Control ctr, bool bIsEnable)
        {
            ctr.Enabled = bIsEnable;
        }

        public void RefreshListBox(ListBox lstBox, List<TixcraftSubscriber.Activity> act)
        {
            lstBox.Items.Clear();
            for (int i = 0; i < act.Count; i++)
            {
                lstBox.Items.Add(act[i].Name);
            }
        }

        public Form1()
        {
            InitializeComponent();
        }


        public void TimerRun()
        {
            if (g_bIsDelayTimerEnable == false) return;

            if (g_bIsDelayTimerRunning == false)
            { 
                g_DelayTimer.Restart();
                g_bIsDelayTimerRunning = true; 
                VPState.Report("開賣智能延遲 - 計時器開始計時", MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
            }
        }

        public void TimerStop()
        {

            if (g_bIsDelayTimerEnable == false) return;
            if (g_bIsDelayTimerRunning == true)
            {
                g_DelayTimer.Stop();
                g_bIsDelayTimerRunning = false;
                VPState.Report("開賣智能延遲 - 關閉", MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
            }
        }

        private bool CheckIfAlart()
        {
            try
            {
                string strIII = SubscrEr.Driver.Url;
                return false;
            }
            catch (OpenQA.Selenium.UnhandledAlertException e)
            {
                if (e.ToString().Contains("連續座位"))
                {
                    this.Invoke(degRefreshText, this, "沒有連續座位!!");
                }

                //連續座位
                if (e.ToString().Contains("驗證碼"))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }
        private string CheckAlert_Once()
        {
            try
            {
                string strResult = "";
                IAlert alart = SubscrEr.Driver.SwitchTo().Alert();
                if (alart.Text.Contains("驗證碼不正確"))
                {
                    strResult = "驗證碼輸入有誤";
                }
                else if (alart.Text.Contains("連續座位"))
                {
                    strResult = "沒有連續座位";
                }
                else if (alart.Text.Contains("同意會員服務"))
                {
                    strResult = "沒有勾選同意條款";
                }
                else if (alart.Text.Contains("選擇一種票種"))
                {
                    strResult = "沒有選擇票( 張數0 )";
                }
                alart.Accept();
                return strResult;
            }
            catch (Exception ex)
            {

            }
            return "返回";
        }


        private string CheckAlart()
        {
            IAlert alart = null;
            int iTargetTimeOut = 1500;
            int iCurrentTime = 0;
            int iDelayms = 100;
            while (alart == null)
            {
                try
                {
                    string strResult = "";
                    alart = SubscrEr.Driver.SwitchTo().Alert();
                    if (alart.Text.Contains("驗證碼不正確"))
                    {
                        strResult = "驗證碼輸入有誤";
                    }
                    else if (alart.Text.Contains("連續座位"))
                    {
                        strResult = "沒有連續座位";
                    }
                    else if (alart.Text.Contains("同意會員服務"))
                    {
                        strResult = "沒有勾選同意條款";
                    }
                    else if (alart.Text.Contains("選擇一種票種"))
                    {
                        strResult = "沒有選擇票( 張數0 )";
                    }
                    alart.Accept();
                    //
                    return strResult;
                }
                catch (Exception ex)
                {
                    //VPState.Report(ex, MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                    #region ==Check if url at "order"==   
                    try
                    { 
                        if (SubscrEr.Driver.Url.Contains("order"))
                        {
                            return "找不到Alert視窗";
                        }
                    }
                    catch (Exception)
                    {

                    }
                    #endregion
                }
                Thread.Sleep(iDelayms);
                iCurrentTime += iDelayms;
                if (iTargetTimeOut < iCurrentTime) return "找不到Alert視窗";
            }
            return "返回";
        }



        /// <summary>
        /// 轉換cookie 從 FastHttpWebDriver 到 Subscriber
        /// </summary>
        /// <param name="src"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool TransformsCookie(FastHttpWebDriver src, ref Subscriber target)
        {
            try
            {
                #region 轉換cookie
                List<System.Net.Cookie> SendCookie = src.getCookiesList();
                if (target.Driver != null)
                {
                    //刪除所有cookie
                    //target.Driver.Manage().Cookies.DeleteAllCookies();
                    //只能轉換FastHttpWebDriver類別cookie
                    //加入新cookie
                    for (int i = 0; i < SendCookie.Count; i++)
                    {
                        OpenQA.Selenium.Cookie Ck = new OpenQA.Selenium.Cookie(SendCookie[i].Name, SendCookie[i].Value);
                        target.Driver.Manage().Cookies.AddCookie(Ck);
                    }

                }
                #endregion
                return true;
            }
            catch (Exception ex)
            {
                VPState.Report(ex, MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                MessageBox.Show(ex.ToString());
                return false;
            }
        }
        /// <summary>
        /// Copy Cookie from GoogleBrowser to miniBrowser
        /// </summary>
        /// <param name="src_GoogleBrowser"></param>
        /// <param name="target_miniBrowser"></param>
        /// <returns></returns>
        public bool TransformsCookieToMiniBrowser(ref Subscriber src_GoogleBrowser, ref FastHttpWebDriver target_miniBrowser)
        {
             
            try
            {
                // miniBrowser
                FastHttpWebDriver bTixBrowser = target_miniBrowser;

                // googleBrowser
                IWebDriver bGoogleBrowser = src_GoogleBrowser.Driver;  
                
                //Get Cookie and Set new Session to miniBrowser
                CookieCollection cc = new CookieCollection();
                foreach (OpenQA.Selenium.Cookie cook in bGoogleBrowser.Manage().Cookies.AllCookies)
                {
                    System.Net.Cookie cookie = new System.Net.Cookie();
                    cookie.Name = cook.Name;
                    cookie.Value = cook.Value;
                    cookie.Domain = cook.Domain;
                    cc.Add(cookie);
                }
                bTixBrowser.Session.Add(cc);
                //done 
                return true;
            }
            catch (Exception ex)
            {
                VPState.Report(ex, MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                MessageBox.Show(ex.ToString());
                return false;
            } 
        }

        public void SetAutoKeyInCheckCode(bool bIsAuto , int iVeryfyThreads)
        {            
            g_iThreadsVerCode = iVeryfyThreads;
        }
        public void SetRandSeats(bool bIsRand)
        {
           chkRandSeats.Checked = bIsRand;
        }
        public void SetSwitchStepByStep(bool bIsStepByStep)
        {
            ckbSwitchPageStepByStep.Checked = bIsStepByStep;
        }
        public void CloseBrowser()
        {
            try
            { 
                this.SubscrEr.Driver.Quit();
            }
            catch (Exception ex)
            {
                VPState.Report(ex, MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //pbChar0.Visible = false;
            //pbChar1.Visible = false;
            //pbChar2.Visible = false;
            //pbChar3.Visible = false;

            //繪圖委派
            degDrawImageVirfyCode = DrawImage;
            degRefreshText = RefreshText;
            degRefreshBox = RefreshListBox;
            degControlEnable = ControlEnable;
            //開啟主要瀏覽器

            SubscrEr.ProxyIP = this.g_strProxyInfo; //Proxy IP Setting
            SubscrEr.OpenBrowser(bIsOpenWithGoogleChrome, bIsMountProxy);
            if (bIsMountProxy)
            {
                WebProxy proxyObject = new WebProxy(g_strProxyInfo, false);
                Tixcraft.TixcraftWebDriver.Proxy = proxyObject;
            }

            

            //add gmail information 到資訊上
            
            string[] strGoogleExcelTable = strFormatEmailInfo.Split('\t');
            if (strGoogleExcelTable.Length >= 3)
            { 
             txt_window_gmail.Text =   strGoogleExcelTable[0];    // gmail
             txt_window_pwd.Text =   strGoogleExcelTable[1];    // pwd
             txt_window_backupEmail.Text =   strGoogleExcelTable[2];   // backup email 
            } 
            lblLogin.Text = "自動辨識連線中...";
            lblDebug.Text = "選購節目 :" + lstShow[g_ShowSelected].Name;
            bool bIsYDMOK = false;
            try
            { 
                //連線OCR伺服器
                myOCRServer.strServerIP = OCRRecipe_IP_Address; //外部接收IP，用已連線
                myOCRServer.Connect();
                
                //連線SQL
                g_ShareAnswerServer.Connect();
                g_CookieServer.Connect();

                if (g_ShareAnswerServer.IsConnect == false)
                { 
                    VIPGeneral.Window.VPMessageBox.ShowError("共享伺服器連線失敗！");
                }
                lblUserName.Text = "Connect : " + myOCRServer.strServerIP;
                SubscrEr.GoTo("https://tixcraft.com/activity");
                AllButtonEnableStatue(true); 
                bIsYDMOK = true;
            }
            catch (Exception ex)
            {
                bIsYDMOK = false;
                VIPGeneral.Window.VPMessageBox.ShowError("無法連線Spyder , 請開啟Spyder後並重新啟動本程式");

                VPState.Report(ex, MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
            }
            if (false == bIsYDMOK)
            {
                lblLogin.Text = "無法使用自動打碼 , IP:" + myOCRServer.strServerIP;
            }
            else 
            { 
                lblLogin.Text = "自動辨識連線...OK";
            }

            //if (strFBAddress != "")
            //    Task.Factory.StartNew(() =>
            //    {
            //        AllButtonEnableStatue(false);
            //        //使用FastWebDriver登入FB
            //        SubscrEr.GoTo("https://tixcraft.com/activity");
            //        bool bIsLogin = Login();
            //        Thread.Sleep(1000);
            //        //將FastWebDriver的Cookie複製給主要瀏覽器
            //        bool result = TransformsCookie(Tixcraft.TixcraftWebDriver, ref SubscrEr);
            //        if (result)
            //        {
            //            //MessageBox.Show("傳送cookie成功!");
            //            SubscrEr.Driver.Navigate().Refresh();
            //        }
            //        else
            //        {
            //            MessageBox.Show("傳送cookie失敗!");
            //        }
            //        if (bIsLogin)
            //        {
            //            AllButtonEnableStatue(true);
            //        }
            //    }); 

        }
        public void WarningUp()
        {
            //暖機 
            for (int i = 0; i < 2; i++)
            {
                SubscrEr.GoTo("https://tixcraft.com/activity");
                SubscrEr.GoTo("https://tixcraft.com/");
                Debug.Print("暖機...{0}", i);
            }
        }

        public void Test2019()
        {
            int iDayIndex = 0; int.TryParse(txtShowTime.Text, out iDayIndex);
            int iTickets = 0; int.TryParse(txtTicketCount.Text, out iTickets);
            HDelayManager HumanDelayer = new HDelayManager();
            if (rd_Delay_none.Checked)
            {
                HumanDelayer.mode = Human_DelayMode.DoNotDelay;
            }
            else if (rd_Delay_Rand.Checked)
            {
                HumanDelayer.mode = Human_DelayMode.DelayRand;
                int.TryParse(txt_Delay_ms_min.Text, out HumanDelayer.DelayMin_ms);
                int.TryParse(textBox3.Text, out HumanDelayer.DelayMax_ms); 
            }
            else if (rd_Delay_MilliSecond.Checked)
            {
                HumanDelayer.mode = Human_DelayMode.DelayMilliSecond;
                int.TryParse(txt_Delay_ms.Text, out HumanDelayer.Delay_ms); 
            }


            bool bIsCanBuy = false; //false = 在大廳一直刷，刷到"立即訂購" 跑出來  
            int icount = 0;
            int iAutoKeyInFailCount = 0;    //紀錄驗證碼打錯次數
            int iAutoKeyIn_Confused = 0;    //紀錄驗證碼在後台換圖的次數 
            string strSelectSeat_Text = ""; //紀錄選擇座位(show this on UI log)
            string strDaysURL = "";//20180930 -> 記錄座位頁面 以便流程1-2-3這樣運作
            List<ActivityDate> thisShowDatas = null;
            Stopwatch swLoading = new Stopwatch();
            Stopwatch swLoadingTime_SelectSeat = new Stopwatch();
            Stopwatch swAutoCheckCodeDownLoad = new Stopwatch();
            bool bIsHaveCheckCode = false; //是否有事前驗證.
            bool bIsNull = true;
            //Step1 : 更新節目資訊

            //注入Google Browser Session給MiniBrowser , 確保快速登入
            // 此部分需先注入，後在訪問拓元，否則會登入無效
            SubscrEr.CopyCookieTo(ref Tixcraft.TixcraftWebDriver);
            Tixcraft.RefreshActivity();
            //登入後取得登入者姓名
            string strUserTaiwanName = Tixcraft.GetTixUserName();
            this.Invoke(degRefreshText, this, "登入資訊:" + strUserTaiwanName);

            //準備開始搶票
            SubscrEr.GoTo(Tixcraft.GetActivity(g_ShowSelected).url);  
            Tixcraft.TixcraftWebDriver.GetWebSourceCode(Tixcraft.GetActivity(g_ShowSelected).url);

            UpdateCircleVisiable(circularProgressBar1, true);
            while (bIsNull == true)
            {
                //開關 (可暫停)
                if (g_bFlagStartBuyStatue == false) return; 
                //取得節目資訊列表
                TixcraftSubscriber.Activity ShowBuy = Tixcraft.GetActivity(g_ShowSelected);
                //取得節目內容
                TixcraftSubscriber.Activity.ShowDate Days = null;
                if (ShowBuy != null)
                {
                    ShowBuy.RefreshDate();
                    Days = ShowBuy.GetShowDate(iDayIndex);
                }
                //判斷是否有座位，有的話代表已經開放
                int iSeats = -1;
                if (Days != null)
                {

                    Days.RefreshAllSeat();
                    strDaysURL = Days.url;
                    iSeats = Days.SeatCount;
                    string sTemp = "訊息:" + Days.info;
                    this.Invoke(degRefreshText, lblInfo, sTemp);
                    try
                    {
                        #region 如果有發現事前驗證碼，則跳入事前驗證碼頁面
                        // iSeat = Request取到0個位置
                        // Days.info = 立即訂購 = 已經開了 --> 已經開了卻沒有位置 => 問答
                        //if ((iSeats == 0) && (Days.info.Contains("立即訂購")) && Days.TixcraftWebDriver.strPageSourceCode.Contains("checkCode"))


                        //if ((iSeats == 0) && (Days.info.Contains("選購一空")) && Days.TixcraftWebDriver.strPageSourceCode.Contains("checkCode"))
                        //if ((iSeats == 0) && (Days.info.Contains("立即訂購")) )
                        if ((iSeats == 0) && (Days.info.Contains("立即訂購")) && Days.TixcraftWebDriver.strPageSourceCode.Contains("checkCode"))
                        {
                            TimerRun(); //20190915 發現驗證瑪代表已經開賣，就開始計時
                            if (SubscrEr.Driver.Url != Days.url)
                            {
                                SubscrEr.GoTo_normal(Days.url);
                                VPState.Report("發現事前驗證碼，則跳入事前驗證碼頁面" , MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                            }

                            ReadOnlyCollection<IWebElement> TD_CheckCode = SubscrEr.Driver.FindElements(By.Id("checkCode"));
                            //ReadOnlyCollection<IWebElement> TD_SubmitButton = SubscrEr.Driver.FindElements(By.Id("submitButton"));
                            if (TD_CheckCode != null)
                            {
                                    VPState.Report("發現 TD_CheckCode ", MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                                    int iTryCount = 0;

                                    while (true)
                                    {
                                        //TD_SubmitButton = SubscrEr.Driver.FindElements(By.Id("submitButton"));
                                        TD_CheckCode = SubscrEr.Driver.FindElements(By.Id("checkCode"));
                                        //if (TD_SubmitButton.Count > 0 && TD_CheckCode.Count > 0)
                                        if ( TD_CheckCode.Count > 0)
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            Thread.Sleep(25);
                                            iTryCount++;
                                            //VPState.Report("等候問答頁面載入完畢", MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                                        }
                                        if (iTryCount > 10)
                                        {
                                            VPState.Report("等候時間過長(等候10次)..跳出等待迴圈", MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                                            break;
                                        }
                                    }

                                    if (TD_CheckCode.Count > 0)
                                    {
                                        VPState.Report("發現 TD_SubmitButton ", MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                                        //如果真的有事前驗證頁面，那就協助載入事前頁面
                                        bIsHaveCheckCode = true;
                                        //SubscrEr.GoTo(Days.url);
                                        if (g_bIsAutoByPassQuestion)
                                        {
                                            string tempstrMyAnswer = "";
                                            #region 自動回答防黃牛問題 (回答三次，使用Cookie判斷是否輸入正確)
                                            for (int iReAnswerIdx = 0; iReAnswerIdx < 999999;)
                                            {
                                                swAutoCheckCodeDownLoad.Restart();
                                                string strMyAnswer = "";
                                                if (g_bIsUsing_OtherAnswerByWindow == true)
                                                {
                                                    strMyAnswer = g_OtherAnswerByWindow; //使用自定義答案
                                                    Thread.Sleep(500);
                                                }
                                                else
                                                {
                                                    //==正常下載答案
                                                    strMyAnswer = DownLoadAnswer(g_AnswerSwitchText);   //
                                                }

                                                //string strMyAnswer = DownLoadAnswer(); 

                                                if ((strMyAnswer != tempstrMyAnswer) && (strMyAnswer != ""))//如果抓下來的答案跟上一個不一樣(再重新提交答案)
                                                {
                                                    iReAnswerIdx++;
                                                    SendCheckCode(strMyAnswer);                 //透過流覽器提交答案

                                                    VPState.Report("提交答案 : " + strMyAnswer, MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                                                    //CheckAlart();
                                                    tempstrMyAnswer = strMyAnswer;//紀錄上一個答案
                                                }
                                                try
                                                {

                                                    if (false == SubscrEr.Driver.Url.Contains("verify"))
                                                    {
                                                        //表示防黃牛回答正確！(因為上一次在驗證碼頁面，這一次沒有)
                                                        bIsNull = false;
                                                        break;
                                                    }
                                                }
                                                catch (Exception)
                                                {

                                                }
                                                UpdateCircleSpeed(circularProgressBar1, (int)swAutoCheckCodeDownLoad.ElapsedMilliseconds);
                                                string strTemp = "下載答案中..刷新..." + "耗時:" + swAutoCheckCodeDownLoad.ElapsedMilliseconds;
                                                this.Invoke(degRefreshText, lblDebug, strTemp);
                                            }
                                            #endregion
                                        }
                                    }
                            }
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {

                        VPState.Report(ex, MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                    }
                    // 有座位 = 安全進入
                    if (iSeats > 0)
                    {
                        TimerRun(); //20190915 開始計時 因為有位置
                        bIsNull = false; 
                    }
                    //如果進來之後直接在第三頁，那改用油猴處理選位置-->走考試流程 = 安全進入
                    if (SubscrEr.Driver.Url.Contains("ticket/ticket") || SubscrEr.Driver.Url.Contains("ticket/area"))
                    {
                        TimerRun(); //20190915 開始計時 因為有跳到第三頁了
                        bIsNull = false;
                        bIsHaveCheckCode = true; 
                    }
                    //bIsNull 用來表示是否開始進行填單動作(驗證碼 & PreSubmit)
                }
                icount++;

                UpdateCircleSpeed(circularProgressBar1, (int)(swLoading.ElapsedMilliseconds / 1.2));
                UpdateCircleText(circularProgressBar1, (swLoading.ElapsedMilliseconds).ToString() + " ms");

                string strMsg = "沒有找到...繼續刷新..." + "耗時:" + swLoading.ElapsedMilliseconds;
                Debug.Print(strMsg);
                this.Invoke(degRefreshText, lblDebug, strMsg);
                swLoading.Restart();
                Debug.Print("\n==================================="); 
            }
            swLoading.Stop();
            swLoadingTime_SelectSeat.Restart();
            UpdateCircleVisiable(circularProgressBar1, false);
            //重新刷新頁面    
            this.Invoke(degRefreshText, lblDebug, "選座位中....");

            int Index = 0; 
            #region ===== 選座位、或不用選座位====
            //step 4:取得購票網址 
            if (bIsHaveCheckCode == true)
            {
                Tixcraft.GetActivity(g_ShowSelected).GetShowDate(iDayIndex).RefreshAllSeat();
            }
            TixcraftSubscriber.Activity.ShowDate AllSeat = Tixcraft.GetActivity(g_ShowSelected).GetShowDate(iDayIndex);
            this.Invoke(degRefreshText, lblDebug, "step 4:取得購票網址....OK");
            int iMaxSeats = AllSeat.SeatCount;
            if (g_bIsRandSeats)
            {
                #region 隨機座位
                Random rnd = new Random();
                int iRand = rnd.Next(0, iMaxSeats);
                Index = iRand;
                #endregion
            }
            else
            {
                #region 指定範圍隨機座位

                bool bIsSelectRangSeat = true;
                if (bIsSelectRangSeat)
                {
                    List<int> lstIndexRang_Match = new List<int>();
                    bool bIsForceSeat = true;
                    for (int iS = 0; iS < AllSeat.SeatCount; iS++)
                    {
                        if (AllSeat.GetSeatTicket(iS).Text.Contains(g_SeatInformation))
                        {
                            lstIndexRang_Match.Add(iS);
                        }
                    }
                    if (lstIndexRang_Match.Count > 0)
                    {
                        #region 隨機指定區域座位
                        Random rnd = new Random();
                        int iRand = rnd.Next(0, lstIndexRang_Match.Count);
                        Index = lstIndexRang_Match[iRand];
                        #endregion
                    }
                    else
                    {
                        #region 隨機座位
                        Random rnd = new Random();
                        int iRand = rnd.Next(0, iMaxSeats);
                        Index = iRand;
                        #endregion
                    }
                }

                #endregion
            }
            #endregion

            //=====座位已選完，此時在第三頁==== 

            //--紀錄驗證碼資訊-- 


            //Total Run ms
            Stopwatch swDriverLoading = new Stopwatch();
            swDriverLoading.Restart();
             

            string strBuyTicketURL = "none";
             
            if (true) //20190730 fix
            {
                #region == 如果沒有防黃牛，那就取miniBrowser的資料進入 ==
                VPState.Report("從第一頁強制載入至第三頁", MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                Tixcraft.RefreshActivity();//test

                TixcraftSubscriber.Activity bActivity = Tixcraft.GetActivity(g_ShowSelected);
                bActivity.RefreshDate();
                TixcraftSubscriber.Activity.ShowDate bShowDate = bActivity.GetShowDate(iDayIndex);
                bShowDate.RefreshAllSeat();
                //TixcraftSubscriber.Activity.ShowDate.SeatTicket  bSeatTicket = bShowDate.GetSeatTicket(Index);
                //bSeatTicket.GetTicket();
                TixcraftSubscriber.Activity.ShowDate.SeatTicket BuyTicket = Tixcraft.GetActivity(g_ShowSelected).GetShowDate(iDayIndex).GetSeatTicket(Index);
                
                
                if (BuyTicket == null) return;
                strBuyTicketURL = BuyTicket.url;
                SubscrEr.GoTo(BuyTicket.url);
                string strTicketURL = BuyTicket.url;
                strSelectSeat_Text = BuyTicket.Text;
                string strSeatMsg = string.Format("位置 : {0}", strSelectSeat_Text);
                this.Invoke(degRefreshText, lblSelectSeat, strSeatMsg);
                #endregion
            }

            if (bIsHaveCheckCode == true)
            { 
                this.Invoke(degRefreshText, lblSelectSeat, "有考試，已自動進入頁面");
            }
            #region  === 強制跳轉到第三頁之後，人性化延遲 === 
            if (HumanDelayer.mode == Human_DelayMode.DoNotDelay)
            {
                // do nothing 
            }
            else if (HumanDelayer.mode == Human_DelayMode.DelayMilliSecond)
            {
                string strMsg = string.Format("[指定] 執行人性化延遲:{0}", HumanDelayer.Delay_ms);
                VPState.Report(strMsg, MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                Thread.Sleep(HumanDelayer.Delay_ms);
            }
            else if (HumanDelayer.mode == Human_DelayMode.DelayRand)
            {
                Random rnd = new Random();
                int iRand_ms = rnd.Next(HumanDelayer.DelayMin_ms, HumanDelayer.DelayMax_ms);
                string strMsg = string.Format("[隨機] 執行人性化延遲:{0} ~ {1} 毫秒 - 實際 : {2} ms", HumanDelayer.DelayMin_ms, HumanDelayer.DelayMax_ms, iRand_ms);
                VPState.Report(strMsg, MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                Thread.Sleep(iRand_ms);
            } 
            #endregion


            //自動判斷開賣後 N 秒 再進行打驗證碼
            if (g_bIsDelayTimerEnable == true)
            {
                while (true)
                {
                    if (g_DelayTimer.ElapsedMilliseconds > g_DelayThreshold)
                    {
                        break;
                    }
                    Thread.Sleep(10);
                }
                string strMessageTimer = string.Format("開賣智能延遲 - {0} ms 等待完成 , 允許開始打碼 , 人工條件為 : {1} ms", g_DelayTimer.ElapsedMilliseconds, g_DelayThreshold);

                VPState.Report(strMessageTimer, MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
            }
            swDriverLoading.Stop();
            try
            {
                bool bIsKeyInBad = true;
                while (bIsKeyInBad == true)
                {
                    int iDpwnLoadCount = 0;
                    List<Bitmap> lstbitmap = new List<Bitmap>();
                    int iChangeImageCount = 0;


                    HImagTool myTool = new HImagTool();;
                    Bitmap VerifyCodeImage;

                    bool bNeedRefreshVeryfiImage = false;

                    while (lstbitmap.Count != 4)
                    {

                        if (g_bFlagStartBuyStatue == false) return;

                        try
                        {
                            if (iDpwnLoadCount > 0)
                            {
                                this.Invoke(degRefreshText, lblVerifyCodeInfo, "1.更新驗證碼...");
                                try
                                {
                                    IAlert ala = SubscrEr.Driver.SwitchTo().Alert();
                                    ala.Dismiss();
                                }
                                catch (Exception ex)
                                {

                                    //VPState.Report(ex, MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                                }

                                iChangeImageCount = iChangeImageCount+1;

                                Stopwatch swCostScreenShot = new Stopwatch();
                                swCostScreenShot.Restart();

                            }
                            Stopwatch swTest = new Stopwatch();
                            swTest.Restart();
                            this.Invoke(degRefreshText, lblVerifyCodeInfo, "2.拍照..." + swTest.ElapsedMilliseconds.ToString());



                            //Tixcraft.TixcraftWebDriver.GetWebSourceCode(strBuyTicketURL);
                            TixcraftSubscriber.Activity.ShowDate.SeatTicket BuyTicket = Tixcraft.GetActivity(g_ShowSelected).GetShowDate(iDayIndex).GetSeatTicket(Index);

                            if (bNeedRefreshVeryfiImage == false)
                            {
                                BuyTicket.GetTicket();
                                VPState.Report("下載圖檔耗時 : " + swTest.ElapsedMilliseconds.ToString() + "ms", MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                            }
                            else 
                            {
                                BuyTicket.RefreshVeryfiImage();
                                //BuyTicket.GetTicket();
                                iAutoKeyIn_Confused++;
                                VPState.Report("切割失敗，換一張驗證碼 : " + swTest.ElapsedMilliseconds.ToString() + "ms", MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                            } 

                            //SubscrEr.ScreenShot();
                            //Bitmap myBrowserScreen = new Bitmap(SubscrEr.bSnapShot);
                            //Bitmap myBrowserScreen = SubscrEr.HwndController.GetScreenShotBy_WindowHwnd();
                            this.Invoke(degRefreshText, lblVerifyCodeInfo, "3.辨識..." + swTest.ElapsedMilliseconds.ToString());
                            VerifyCodeImage = new Bitmap(BuyTicket.VerificationCodeImage);  //改用miniBrowser抓圖
                            UpdateImage(BuyTicket.VerificationCodeImage, pb_VeryfiImage);

                            if (g_bIsListen_OCR_History)
                            { 
                                //Debug用，畫出當時拍照的畫面
                                this.Invoke(degDrawImageVirfyCode, pb_SnapTest, VerifyCodeImage); 
                            }

                            if (VerifyCodeImage != null)
                            {
                                //有驗證碼就進行切割
                                lstbitmap = HalconProcess.Vision.SplitVerifyCode(VerifyCodeImage); //驗證碼取出四個字
                                if (lstbitmap.Count != 4)
                                {
                                    //沒有切割成功，換一張 
                                    bNeedRefreshVeryfiImage = true;
                                }
                                else
                                {
                                    //有成功，不用換
                                    bNeedRefreshVeryfiImage = false;
                                    this.Invoke(degRefreshText, lblVerifyCodeInfo, string.Format("3.辨識...OK "));
                                }
                            }
                            else
                            {
                                // 沒有驗證馬，就重新下載一張
                                BuyTicket.GetTicket(); 
                                lstbitmap.Clear();
                                VPState.Report(" 沒有驗證馬，就重新下載一張 : " , MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);

                            }
                            iDpwnLoadCount++;
                            if (iDpwnLoadCount > 10000000) break;
                        }
                        catch (Exception ex)
                        {
                            //throw;
                        }
                    }
                     
                    //驗證碼準備OK ==> 打勾 + 選張數
                    string strResult = "";
                    if (lstbitmap != null)
                    {
                        if (lstbitmap.Count == 4)
                        {
                            int iTCPIP_SendCount = 0;
                            while (strResult.Length < 4)
                            {
                                Stopwatch swOCR = new Stopwatch();
                                swOCR.Restart();
                                strResult = "";
                                strResult += this.myOCRServer.GetChar(lstbitmap[0]);
                                strResult += this.myOCRServer.GetChar(lstbitmap[1]);
                                strResult += this.myOCRServer.GetChar(lstbitmap[2]);
                                strResult += this.myOCRServer.GetChar(lstbitmap[3]);
                                swOCR.Stop();
                                VPState.Report("Python 辨識耗時" + swOCR.ElapsedMilliseconds.ToString() + " ms", MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                                iTCPIP_SendCount++;
                            }

                            #region ==紀錄驗證碼歷程==
                            
                            if (g_bIsListen_OCR_History)
                            {
                                OCR_History OCR_CheckNode = new OCR_History();
                                OCR_CheckNode.ImageA = lstbitmap[0];
                                OCR_CheckNode.ImageB = lstbitmap[1];
                                OCR_CheckNode.ImageC = lstbitmap[2];
                                OCR_CheckNode.ImageD = lstbitmap[3];

                                OCR_CheckNode.Answer = strResult;
                                g_lstOCR_History.Add(OCR_CheckNode);
                            }

                            #endregion

                            if (iTCPIP_SendCount > 1)
                            {
                                VPState.Report("辨識通訊錯誤 , 重送" + iTCPIP_SendCount.ToString() + "次", MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                            }
                        }
                        for (int i = 0; i < lstbitmap.Count; i++)
                        {

                            PictureBox pTarget = null;
                            if (i == 0) pTarget = pbChar0;
                            if (i == 1) pTarget = pbChar1;
                            if (i == 2) pTarget = pbChar2;
                            if (i == 3) pTarget = pbChar3; ;
                            this.Invoke(degDrawImageVirfyCode, pTarget, lstbitmap[i]);
                        }
                    }
                    //strResult = ScreenShot_and_GetVeryfiCodeResult(); 
                    //SubscrEr.PreSubmit(iTickets);
                    string strMessage = string.Format("{0} --> {1}", iDpwnLoadCount, strResult);
                    this.Invoke(degRefreshText, lblVerifyCodeInfo, strMessage);

                    #region -等待載入提交頁面-  20190730
                    //-等待載入提交頁面- 
                    Stopwatch swCostWait = new Stopwatch();
                    swCostWait.Restart();
                    IWebElement iAgreeElement = null;
                    while (iAgreeElement == null)
                    {
                        try
                        {
                            iAgreeElement = SubscrEr.Driver.FindElement(By.Id("TicketForm_agree"));

                            this.Invoke(degRefreshText, lblDebug, "等待載入票券資訊成功！"); 
                        }
                        catch (Exception)
                        {
                            iAgreeElement = null;
                            this.Invoke(degRefreshText, lblDebug, "等待載入票券中...！"); 
                        }
                        Thread.Sleep(10);
                    }
                    swCostWait.Stop();
                    VPState.Report("等待載入提交頁面耗時" + swCostWait.ElapsedMilliseconds.ToString() + " ms", MethodBase.GetCurrentMethod(), VPState.eVPType.Windows); 
                    #endregion
                   
                    #region 提交
                    Stopwatch swSubmit_Cost = new Stopwatch();
                    swSubmit_Cost.Restart();
                    if (strResult.Length < 4)
                    {
                        //SubscrEr.Submit("GGGG"); 
                        SubscrEr.SubmitAndPreSubtmi("GGGG" , 1); 
                    }
                    else
                    {

                        SubscrEr.SubmitAndPreSubtmi(strResult, iTickets);  
                    }
                    swSubmit_Cost.Stop();
                    VPState.Report("提交耗時" + swSubmit_Cost.ElapsedMilliseconds.ToString() + " ms", MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                    
                    Stopwatch swWaitForAlart = new Stopwatch();
                    swWaitForAlart.Restart(); 
                    string strAlartText = CheckAlart();
                    swWaitForAlart.Stop(); 
                    string strWaitForAlart = string.Format("Wait for Alart is cost {0} ms", swWaitForAlart.ElapsedMilliseconds.ToString()); 
                    VPState.Report(strWaitForAlart, MethodBase.GetCurrentMethod(), VPState.eVPType.Windows); 
            


                    if (strAlartText == "驗證碼輸入有誤")
                    {
                        iAutoKeyInFailCount++;
                        this.Invoke(degRefreshText, lblVerifyCodeInfo, "驗證碼輸入有誤");
                        bIsKeyInBad = true;
                    }
                    #endregion
                    #region 重新填滿一次選擇張數與同意條款
                    if (strAlartText != "找不到Alert視窗")
                    {
                        SubscrEr.PreSubmit(iTickets);
                    }
                    else
                    {
                        this.Invoke(degRefreshText, lblVerifyCodeInfo, "驗證碼正確，已提交送出！請確認訂單");

                        long iVisit_ms = 6000;
                        //---after submit , visit check to get ticket---
                        if (bIsASK_CheckPage == true)
                        {
                            Task.Factory.StartNew(() =>
                            {
                                int iCount_Times = 0;
                                Stopwatch swCheck = new Stopwatch();
                                swCheck.Restart();
                                // 訪問Check , 直到封包返回訂票成功訊息
                                while (swCheck.ElapsedMilliseconds < iVisit_ms)
                                {
                                    iCount_Times++;
                                    string strResponse = Tixcraft.TixcraftWebDriver.GetWebSourceCode("https://tixcraft.com/ticket/check");
                                    strResponse = Uri.UnescapeDataString(strResponse.Replace("\\\\", "\\"));
                                    strResponse = UnicodeToString(strResponse);

                                    string strResponse_Message = iCount_Times.ToString() + "," + strResponse;

                                    this.Invoke(degRefreshText, lblVerifyCodeInfo, strResponse_Message);
                                    if (strResponse.Contains("即將"))
                                    {
                                        //如果返回此訊息，代表已完成訂購，主瀏覽器直接跳到結帳頁面
                                        SubscrEr.GoTo("https://tixcraft.com/ticket/payment");
                                        string strReportVIP_Msg = string.Format("{0} , {1}", Tixcraft.USER_NAME, strResponse);
                                        VPState.Report(strReportVIP_Msg, MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                                        break;
                                    }
                                    else if (strResponse != "")
                                    {
                                        string strReportVIP_Msg = string.Format("{0} , {1}" , Tixcraft.USER_NAME , strResponse );
                                        VPState.Report(strReportVIP_Msg, MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                                        break;
                                    }
                                }
                                swCheck.Stop();
                            });
                        }


                        bIsKeyInBad = false;
                    }
                    #endregion 
                }
            }
            catch (Exception ex)
            { 
                VPState.Report(ex, MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
            }
            this.Invoke(degRefreshText, lblDebug, "自動填單中...OK"); 
            double iCostTime = swLoadingTime_SelectSeat.ElapsedMilliseconds / 1000.0;
            this.Invoke(degRefreshText, lblDebug,"換" + iAutoKeyIn_Confused.ToString() + "次，錯" + iAutoKeyInFailCount.ToString() + "次，總耗時 : " + iCostTime.ToString("F2") + "秒");
            //this.Invoke(degRefreshText, this, "打錯" + iAutoKeyInFailCount.ToString() + "次，開放→自動打碼完畢耗時 : " + iCostTime.ToString("F2") + "秒");
            double iLoadCosttime =  swDriverLoading.ElapsedMilliseconds   / 1000.0;  
            this.Invoke(degRefreshText, lblInfo, "網頁載入時間 : " + iLoadCosttime.ToString("F2") + "秒");
            string strMessaggLog = string.Format("{0}=網頁載:{1}秒 = 總耗時:{2}秒 = 錯:{3}次", strSelectSeat_Text, iLoadCosttime.ToString("F2"), iCostTime.ToString("F2"), iAutoKeyInFailCount.ToString()); 
            VPState.Report(strMessaggLog, MethodBase.GetCurrentMethod(), VPState.eVPType.Windows); 
            Task.Factory.StartNew(() => 
            {
                for (int i = 0; i < 10; i++)
                {
                    //-1=不執行  0 => ATM付款   1 = ibon付款  2 = 信用卡
                   SubscrEr.PayModeClick(g_PayMode);
                   Thread.Sleep(3000);
                } 
            });
        }

        public void Test()
        {
            int iDayIndex = 0; int.TryParse(txtShowTime.Text, out iDayIndex);
            int iTickets = 0; int.TryParse(txtTicketCount.Text, out iTickets);
            HDelayManager HumanDelayer = new HDelayManager();
            if (rd_Delay_none.Checked)
            {
                HumanDelayer.mode = Human_DelayMode.DoNotDelay;
            }
            else if (rd_Delay_Rand.Checked)
            {
                HumanDelayer.mode = Human_DelayMode.DelayRand;
                int.TryParse(txt_Delay_ms_min.Text, out HumanDelayer.DelayMin_ms);
                int.TryParse(textBox3.Text, out HumanDelayer.DelayMax_ms); 
            }
            else if (rd_Delay_MilliSecond.Checked)
            {
                HumanDelayer.mode = Human_DelayMode.DelayMilliSecond;
                int.TryParse(txt_Delay_ms.Text, out HumanDelayer.Delay_ms); 
            }


            bool bIsCanBuy = false; //false = 在大廳一直刷，刷到"立即訂購" 跑出來  
            int icount = 0;
            int iAutoKeyInFailCount = 0;    //紀錄驗證碼打錯次數
            string strSelectSeat_Text = ""; //紀錄選擇座位(show this on UI log)
            string strDaysURL = "";//20180930 -> 記錄座位頁面 以便流程1-2-3這樣運作
            List<ActivityDate> thisShowDatas = null;
            Stopwatch swLoading = new Stopwatch();
            Stopwatch swLoadingTime_SelectSeat = new Stopwatch();
            Stopwatch swAutoCheckCodeDownLoad = new Stopwatch();
            bool bIsHaveCheckCode = false; //是否有事前驗證.
            bool bIsNull = true;
            //Step1 : 更新節目資訊
            Tixcraft.RefreshActivity();
            SubscrEr.GoTo(Tixcraft.GetActivity(g_ShowSelected).url); 
            UpdateCircleVisiable(circularProgressBar1, true);
            while (bIsNull == true)
            {
                //開關 (可暫停)
                if (g_bFlagStartBuyStatue == false) return; 
                //取得節目資訊列表
                TixcraftSubscriber.Activity ShowBuy = Tixcraft.GetActivity(g_ShowSelected);
                //取得節目內容
                TixcraftSubscriber.Activity.ShowDate Days = null;
                if (ShowBuy != null)
                {
                    ShowBuy.RefreshDate();
                    Days = ShowBuy.GetShowDate(iDayIndex);
                }
                //判斷是否有座位，有的話代表已經開放
                int iSeats = -1;
                if (Days != null)
                {

                    Days.RefreshAllSeat();
                    strDaysURL = Days.url;
                    iSeats = Days.SeatCount;
                    string sTemp = "訊息:" + Days.info;
                    this.Invoke(degRefreshText, lblInfo, sTemp);
                    try
                    {
                        #region 如果有發現事前驗證碼，則跳入事前驗證碼頁面
                        // iSeat = Request取到0個位置
                        // Days.info = 立即訂購 = 已經開了 --> 已經開了卻沒有位置 => 問答
                        //if ((iSeats == 0) && (Days.info.Contains("立即訂購")) && Days.TixcraftWebDriver.strPageSourceCode.Contains("checkCode"))


                        //if ((iSeats == 0) && (Days.info.Contains("選購一空")) && Days.TixcraftWebDriver.strPageSourceCode.Contains("checkCode"))
                        //if ((iSeats == 0) && (Days.info.Contains("立即訂購")) )
                        if ((iSeats == 0) && (Days.info.Contains("立即訂購")) && Days.TixcraftWebDriver.strPageSourceCode.Contains("checkCode"))
                        {
                            if (SubscrEr.Driver.Url != Days.url)
                            {
                                SubscrEr.GoTo_normal(Days.url);
                                VPState.Report("發現事前驗證碼，則跳入事前驗證碼頁面" , MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                            }

                            ReadOnlyCollection<IWebElement> TD_CheckCode = SubscrEr.Driver.FindElements(By.Id("checkCode"));
                            //ReadOnlyCollection<IWebElement> TD_SubmitButton = SubscrEr.Driver.FindElements(By.Id("submitButton"));
                            if (TD_CheckCode != null)
                            {
                                    VPState.Report("發現 TD_CheckCode ", MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                                    int iTryCount = 0;

                                    while (true)
                                    {
                                        //TD_SubmitButton = SubscrEr.Driver.FindElements(By.Id("submitButton"));
                                        TD_CheckCode = SubscrEr.Driver.FindElements(By.Id("checkCode"));
                                        //if (TD_SubmitButton.Count > 0 && TD_CheckCode.Count > 0)
                                        if ( TD_CheckCode.Count > 0)
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            Thread.Sleep(25);
                                            iTryCount++;
                                            //VPState.Report("等候問答頁面載入完畢", MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                                        }
                                        if (iTryCount > 10)
                                        {
                                            VPState.Report("等候時間過長(等候10次)..跳出等待迴圈", MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                                            break;
                                        }
                                    }

                                    if (TD_CheckCode.Count > 0)
                                    {
                                        VPState.Report("發現 TD_SubmitButton ", MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                                        //如果真的有事前驗證頁面，那就協助載入事前頁面
                                        bIsHaveCheckCode = true;
                                        //SubscrEr.GoTo(Days.url);
                                        if (g_bIsAutoByPassQuestion)
                                        {
                                            string tempstrMyAnswer = "";
                                            #region 自動回答防黃牛問題 (回答三次，使用Cookie判斷是否輸入正確)
                                            for (int iReAnswerIdx = 0; iReAnswerIdx < 999999;)
                                            {
                                                swAutoCheckCodeDownLoad.Restart();
                                                string strMyAnswer = "";
                                                if (g_bIsUsing_OtherAnswerByWindow == true)
                                                {
                                                    strMyAnswer = g_OtherAnswerByWindow; //使用自定義答案
                                                    Thread.Sleep(500);
                                                }
                                                else
                                                {
                                                    //==正常下載答案
                                                    strMyAnswer = DownLoadAnswer(g_AnswerSwitchText);   //
                                                }

                                                //string strMyAnswer = DownLoadAnswer(); 

                                                if ((strMyAnswer != tempstrMyAnswer) && (strMyAnswer != ""))//如果抓下來的答案跟上一個不一樣(再重新提交答案)
                                                {
                                                    iReAnswerIdx++;
                                                    SendCheckCode(strMyAnswer);                 //透過流覽器提交答案

                                                    VPState.Report("提交答案 : " + strMyAnswer, MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                                                    //CheckAlart();
                                                    tempstrMyAnswer = strMyAnswer;//紀錄上一個答案
                                                }
                                                try
                                                {
                                                    //使用猴子腳本 --> 協助選座位
                                                    if (false == SubscrEr.Driver.Url.Contains("verify"))
                                                    {
                                                        bool bIsLoadingFinish_Seat = false;
                                                        Stopwatch swWaitLoad = new Stopwatch();
                                                        swWaitLoad.Restart();

                                                        #region == 等待頁面載入(座位頁面)，載入完畢後注入JS協助選座位 ==

                                                        while (bIsLoadingFinish_Seat == false)
                                                        {
                                                            //等候載入第二頁
                                                            IWebElement game_areaList = null;
                                                            try
                                                            {
                                                                //第二頁的特徵
                                                                game_areaList = SubscrEr.Driver.FindElement(By.Id("game_id"));

                                                            }
                                                            catch (Exception)
                                                            {
                                                                game_areaList = null;
                                                                //如果沒有第二頁，就找第三頁看看 
                                                                IWebElement TicketForm_verifyCode = null;
                                                                try
                                                                {
                                                                    TicketForm_verifyCode = SubscrEr.Driver.FindElement(By.Id("TicketForm_verifyCode"));
                                                                }
                                                                catch (Exception)
                                                                {
                                                                    TicketForm_verifyCode = null;
                                                                }
                                                                if (TicketForm_verifyCode != null)
                                                                {
                                                                    // 如果找到第三頁，那就直接跳出填單吧！
                                                                    bIsLoadingFinish_Seat = true;
                                                                    VPState.Report("考完試，選座位(第三頁)" + swWaitLoad.ElapsedMilliseconds.ToString() + " ms", MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);

                                                                }
                                                            }

                                                            if (game_areaList != null)
                                                            {
                                                                VPState.Report("考完試，選座位(第二頁)" + swWaitLoad.ElapsedMilliseconds.ToString() + " ms", MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);

                                                                #region == 注入JS直到頁面切換 ==

                                                                bool bIsInjectOK = false;
                                                                int iInjectCount = 0;
                                                                while (bIsInjectOK == false)
                                                                {
                                                                    if (SubscrEr.Driver.Url.Contains("ticket/area"))
                                                                    {
                                                                        #region == 使用猴子腳本 --> 協助選座位 ==
                                                                        string strScrpitSelectSeat = SubscrEr.JS_ReadFile("Tix_Automation.txt");
                                                                        strScrpitSelectSeat = SubscrEr.JS_Setting_Seats(strScrpitSelectSeat, iDayIndex.ToString(), g_SeatInformation, iTickets.ToString(), "-1");
                                                                        SubscrEr.InjectJavaScript(strScrpitSelectSeat);
                                                                        #endregion
                                                                        CheckAlert_Once();
                                                                        VPState.Report("注入JS中...注入完畢(選座位)..." + iInjectCount.ToString() + "次", MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                                                                    }
                                                                    else
                                                                    {
                                                                        bIsInjectOK = true;
                                                                    }
                                                                    Thread.Sleep(50);
                                                                }

                                                                #endregion

                                                                bIsLoadingFinish_Seat = true;
                                                            }
                                                            Thread.Sleep(50);
                                                        }
                                                        #endregion

                                                        swWaitLoad.Stop();
                                                        VPState.Report("等待頁面載入完畢...耗時 : " + swWaitLoad.ElapsedMilliseconds.ToString() + " ms", MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);

                                                        //表示防黃牛回答正確！(因為上一次在驗證碼頁面，這一次沒有)
                                                        bIsNull = false;
                                                        break;
                                                    }
                                                }
                                                catch (Exception)
                                                {

                                                }
                                                UpdateCircleSpeed(circularProgressBar1, (int)swAutoCheckCodeDownLoad.ElapsedMilliseconds);
                                                string strTemp = "下載答案中..刷新..." + "耗時:" + swAutoCheckCodeDownLoad.ElapsedMilliseconds;
                                                this.Invoke(degRefreshText, lblDebug, strTemp);
                                            }
                                            #endregion
                                        }
                                    }
                            }
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {

                        VPState.Report(ex, MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                    }
                    // 有座位 = 安全進入
                    if (iSeats > 0) bIsNull = false;
                    //如果進來之後直接在第三頁，那改用油猴處理選位置-->走考試流程 = 安全進入
                    if (SubscrEr.Driver.Url.Contains("ticket/ticket") || SubscrEr.Driver.Url.Contains("ticket/area"))
                    { 
                        bIsNull = false;
                        bIsHaveCheckCode = true; 
                    }
                    //bIsNull 用來表示是否開始進行填單動作(驗證碼 & PreSubmit)
                }
                icount++;

                UpdateCircleSpeed(circularProgressBar1, (int)(swLoading.ElapsedMilliseconds / 1.2));
                UpdateCircleText(circularProgressBar1, (swLoading.ElapsedMilliseconds).ToString() + " ms");

                string strMsg = "沒有找到...繼續刷新..." + "耗時:" + swLoading.ElapsedMilliseconds;
                Debug.Print(strMsg);
                this.Invoke(degRefreshText, lblDebug, strMsg);
                swLoading.Restart();
                Debug.Print("\n==================================="); 
            }
            swLoading.Stop();
            swLoadingTime_SelectSeat.Restart();
            UpdateCircleVisiable(circularProgressBar1, false);
            //重新刷新頁面    
            this.Invoke(degRefreshText, lblDebug, "選座位中....");

            //Tixcraft.GetActivity(g_ShowSelected).GetShowDate(iDayIndex).RefreshAllSeat();
            //while (Tixcraft.GetActivity(g_ShowSelected).GetShowDate(iDayIndex).SeatCount == 0) ;

            
            bool bIsEntryBy3Pages = bIsSwitchPageStepByStep; //false = 1頁 --> 3頁 , true = 1 --> 2 --> 3頁

            int Index = 0;
            //=====選座位、或不用選座位====  
            if (bIsHaveCheckCode == true)
            {
                //如果有考試，那麼使用猴子來填單( 填入張數 & 打勾 )
                #region == 如果有防黃牛回答的話，那麼miniBrowser將不適用，需要使用JS協助進入 == 
                string strScrpitSelectSeat = SubscrEr.JS_ReadFile("Tix_Automation.txt");
                strScrpitSelectSeat = SubscrEr.JS_Setting_Seats(strScrpitSelectSeat, iDayIndex.ToString(), g_SeatInformation, iTickets.ToString(), "-1");
                SubscrEr.InjectJavaScript(strScrpitSelectSeat);
                #endregion
            }
            else
            {
                if (bIsEntryBy3Pages == true)
                {
                    #region 沒有防黃牛回答，而且強制使用三頁面進行轉換，則使用JS協助換下一頁 (進入第2頁、3頁)

                    //載入第二頁
                    VPState.Report("載入下一頁", MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                    SubscrEr.GoTo_normal(strDaysURL);
                    bool bIsWaitingPageLoadDone = false;
                    int iCurrentPageIndex = 0; // 2 = 第二頁 , 3 = 第3頁
                    while (bIsWaitingPageLoadDone == false)
                    {
                        string strCurrentURL = SubscrEr.Driver.Url;
                        if (strCurrentURL.Contains("ticket/ticket") )
                        {
                            iCurrentPageIndex = 3; 
                            VPState.Report("目前位於-3 - 驗證碼頁面 . 結束等待", MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                        }
                        if ( SubscrEr.Driver.Url.Contains("ticket/area"))
                        {
                            iCurrentPageIndex = 2;
                            VPState.Report("目前位於-2 - 選座位頁面 . 結束等待", MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                        }
                        if (iCurrentPageIndex != 0)
                        {
                            bIsWaitingPageLoadDone = true;
                        }
                        //載入第二頁 等待完成
                        Thread.Sleep(50);
                    }
                    if (iCurrentPageIndex == 2)
                    { 
                        while (SubscrEr.Driver.Url.Contains("ticket/area"))
                        {
                            string strScrpitSelectSeat = SubscrEr.JS_ReadFile("Tix_Automation.txt");
                            strScrpitSelectSeat = SubscrEr.JS_Setting_Seats(strScrpitSelectSeat, iDayIndex.ToString(), g_SeatInformation, iTickets.ToString(), "-1");
                            SubscrEr.InjectJavaScript(strScrpitSelectSeat);
                            VPState.Report("點擊並切換到第3頁...", MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                            Thread.Sleep(400);
                        }  
                    } 
                    VPState.Report("======切換頁面流程結束 - 準備打驗證碼=====", MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                    #endregion
                }
                else
                { 
                    #region == 如果沒有防黃牛的話，那麼miniBrowser可快速進入取得座位URL ==
                    //step 4:取得購票網址 
                    TixcraftSubscriber.Activity.ShowDate AllSeat = Tixcraft.GetActivity(g_ShowSelected).GetShowDate(iDayIndex);
                    this.Invoke(degRefreshText, lblDebug, "step 4:取得購票網址....OK");
                    int iMaxSeats = AllSeat.SeatCount;
                    if (g_bIsRandSeats)
                    {
                        #region 隨機座位
                        Random rnd = new Random();
                        int iRand = rnd.Next(0, iMaxSeats);
                        Index = iRand;
                        #endregion
                    }
                    else
                    {
                        //指定範圍隨機座位
                        bool bIsSelectRangSeat = true;
                        if (bIsSelectRangSeat)
                        {
                            List<int> lstIndexRang_Match = new List<int>();
                            bool bIsForceSeat = true;
                            for (int iS = 0; iS < AllSeat.SeatCount; iS++)
                            {
                                if (AllSeat.GetSeatTicket(iS).Text.Contains(g_SeatInformation))
                                {
                                    lstIndexRang_Match.Add(iS);
                                }
                            }
                            if (lstIndexRang_Match.Count > 0)
                            {
                                #region 隨機指定區域座位
                                Random rnd = new Random();
                                int iRand = rnd.Next(0, lstIndexRang_Match.Count);
                                Index = lstIndexRang_Match[iRand];
                                #endregion
                            }
                            else
                            {
                                #region 隨機座位
                                Random rnd = new Random();
                                int iRand = rnd.Next(0, iMaxSeats);
                                Index = iRand;
                                #endregion
                            }
                        }
                    }
                    #endregion
                }
            }


            //=====座位已選完，此時在第三頁==== 

            //--紀錄驗證碼資訊-- 

            Stopwatch swDriverLoading = new Stopwatch();
            swDriverLoading.Restart();
            if (bIsHaveCheckCode == false)
            {
                if (bIsEntryBy3Pages == false)
                {
                    #region == 如果沒有防黃牛，那就取miniBrowser的資料進入 ==
                    VPState.Report("從第一頁強制載入至第三頁", MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                    TixcraftSubscriber.Activity.ShowDate.SeatTicket BuyTicket = Tixcraft.GetActivity(g_ShowSelected).GetShowDate(iDayIndex).GetSeatTicket(Index);
                    if (BuyTicket == null) return;
                    SubscrEr.GoTo(BuyTicket.url);
                    string strTicketURL = BuyTicket.url;
                    strSelectSeat_Text = BuyTicket.Text;
                    string strSeatMsg = string.Format("位置 : {0}", strSelectSeat_Text);
                    this.Invoke(degRefreshText, lblSelectSeat, strSeatMsg);
                    #endregion
                }
            }
            else
            { 
                this.Invoke(degRefreshText, lblSelectSeat, "有考試，已自動進入頁面");
            }
            #region  === 強制跳轉到第三頁之後，人性化延遲 === 
            if (HumanDelayer.mode == Human_DelayMode.DoNotDelay)
            {
                // do nothing 
            }
            else if (HumanDelayer.mode == Human_DelayMode.DelayMilliSecond)
            {
                string strMsg = string.Format("[指定] 執行人性化延遲:{0}", HumanDelayer.Delay_ms);
                VPState.Report(strMsg, MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                Thread.Sleep(HumanDelayer.Delay_ms);
            }
            else if (HumanDelayer.mode == Human_DelayMode.DelayRand)
            {
                Random rnd = new Random();
                int iRand_ms = rnd.Next(HumanDelayer.DelayMin_ms, HumanDelayer.DelayMax_ms);
                string strMsg = string.Format("[隨機] 執行人性化延遲:{0} ~ {1} 毫秒 - 實際 : {2} ms", HumanDelayer.DelayMin_ms, HumanDelayer.DelayMax_ms, iRand_ms);
                VPState.Report(strMsg, MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                Thread.Sleep(iRand_ms);
            }
            #endregion
            swDriverLoading.Stop();
            try
            {
                bool bIsKeyInBad = true;
                Bitmap bSrc = null;
                Bitmap UPbSrc = null;
                while (bIsKeyInBad == true)
                {
                    int iDpwnLoadCount = 0;
                    List<Bitmap> lstbitmap = new List<Bitmap>();
                    int iChangeImageCount = 0;


                    int iRoi_x1 = 0;
                    int iRoi_y1 = 0;
                    int iRoi_x2 = 0;
                    int iRoi_y2 = 0;
                    HImagTool myTool = new HImagTool();;
                    Bitmap VerifyCodeImage;

                    while (lstbitmap.Count != 4)
                    {

                        if (g_bFlagStartBuyStatue == false) return;

                        try
                        {
                            if (iDpwnLoadCount > 0)
                            {
                                this.Invoke(degRefreshText, lblVerifyCodeInfo, "1.更新驗證碼...");
                                try
                                {
                                    IAlert ala = SubscrEr.Driver.SwitchTo().Alert();
                                    ala.Dismiss();
                                }
                                catch (Exception ex)
                                {

                                    //VPState.Report(ex, MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                                }

                                iChangeImageCount = iChangeImageCount+1;

                                Stopwatch swCostScreenShot = new Stopwatch();
                                swCostScreenShot.Restart();

                            }
                            Stopwatch swTest = new Stopwatch();
                            swTest.Restart();
                            this.Invoke(degRefreshText, lblVerifyCodeInfo, "2.拍照..." + swTest.ElapsedMilliseconds.ToString());
                            SubscrEr.ScreenShot();
                            //Bitmap myBrowserScreen = new Bitmap(SubscrEr.bSnapShot);
                            //Bitmap myBrowserScreen = SubscrEr.HwndController.GetScreenShotBy_WindowHwnd();
                            this.Invoke(degRefreshText, lblVerifyCodeInfo, "3.辨識..." + swTest.ElapsedMilliseconds.ToString());
                            VerifyCodeImage = new Bitmap(SubscrEr.bSnapShot);

                            if (g_bIsListen_OCR_History)
                            { 
                                //Debug用，畫出當時拍照的畫面
                                this.Invoke(degDrawImageVirfyCode, pb_SnapTest, VerifyCodeImage); 
                            }
                            
                            
                            lstbitmap.Clear();
                            lstbitmap = myTool.SplitVerifyFromScreenShot(VerifyCodeImage, ref iRoi_x1, ref iRoi_y1, ref iRoi_x2, ref iRoi_y2);
                            //this.Invoke(degRefreshText, lblVerifyCodeInfo, string.Format("{0} , {1} , {2} , {3} , ", iRoi_x1, iRoi_y1, iRoi_x2, iRoi_y2) + "__" + swTest.ElapsedMilliseconds.ToString() + "__" + lstbitmap.Count.ToString()); 
                            bSrc = new Bitmap(lstbitmap[0]);
                            if (lstbitmap.Count > 0)
                            {
                                lstbitmap = HalconProcess.Vision.SplitVerifyCode(lstbitmap[0]); //驗證碼取出四個字
                                if (lstbitmap.Count != 4)
                                {
                                    IWebElement iImage = SubscrEr.Driver.FindElement(By.Id("yw0"));
                                    //== Win Api 滑鼠控制 ==
                                    //int iClick_x = iRoi_x1 + 10;
                                    //int iClick_y = iRoi_y1 + 10;
                                    int iClick_x = iImage.Location.X + 10;
                                    int iClick_y = iImage.Location.Y + 10; 
                                    SubscrEr.HwndController.Mouse_LeftButton_Down_ByOffset(iClick_x, iClick_y);
                                    Thread.Sleep(10);
                                    SubscrEr.HwndController.Mouse_LeftButton_Up_ByOffset(iClick_x, iClick_y);
                                    Thread.Sleep(g_Yw0_DelayTime);


                                    SubscrEr.ScreenShot();
                                    //myTool = new HImagTool();
                                    VerifyCodeImage = new Bitmap(SubscrEr.bSnapShot);
                                    lstbitmap.Clear();
                                    lstbitmap = myTool.SplitVerifyFromScreenShot(VerifyCodeImage, ref iRoi_x1, ref iRoi_y1, ref iRoi_x2, ref iRoi_y2);
                                    UPbSrc = new Bitmap(lstbitmap[0]);
                                    lstbitmap.Clear();
                                    Stopwatch swCostTime = new Stopwatch();
                                    swCostTime.Restart();
                                    while (HalconProcess.Vision.GetPSNR(bSrc, UPbSrc) == false)
                                    {
                                        SubscrEr.ScreenShot();
                                        //myTool = new HImagTool();
                                        VerifyCodeImage = new Bitmap(SubscrEr.bSnapShot);
                                        lstbitmap.Clear();
                                        lstbitmap = myTool.SplitVerifyFromScreenShot(VerifyCodeImage, ref iRoi_x1, ref iRoi_y1, ref iRoi_x2, ref iRoi_y2);
                                        UPbSrc = new Bitmap(lstbitmap[0]);
                                        //Application.DoEvents();
                                    }
                                    lstbitmap.Clear();
                                    swCostTime.Stop();
                                }
                                else
                                {
                                    this.Invoke(degRefreshText, lblVerifyCodeInfo, string.Format("3.辨識...OK , xy({0} , {1}) , roi : {2}", iRoi_x1, iRoi_y1, lstbitmap.Count));
                                }
                            }
                            else
                            {
                                IWebElement iImage = SubscrEr.Driver.FindElement(By.Id("yw0"));
                                int iClick_x = iImage.Location.X + 10;
                                int iClick_y = iImage.Location.Y + 10; 
                                SubscrEr.HwndController.Mouse_LeftButton_Down_ByOffset(iClick_x, iClick_y);
                                Thread.Sleep(10);
                                SubscrEr.HwndController.Mouse_LeftButton_Up_ByOffset(iClick_x, iClick_y);
                                Thread.Sleep(g_Yw0_DelayTime);
                                lstbitmap.Clear();
                            }
                            iDpwnLoadCount++;
                            if (iDpwnLoadCount > 10000000) break;
                        }
                        catch (Exception ex)
                        {
                            //throw;
                        }
                    }
                     
                    //驗證碼準備OK ==> 打勾 + 選張數
                    string strResult = "";
                    if (lstbitmap != null)
                    {
                        if (lstbitmap.Count == 4)
                        {
                            int iTCPIP_SendCount = 0;
                            while (strResult.Length < 4)
                            {
                                Stopwatch swOCR = new Stopwatch();
                                swOCR.Restart();
                                strResult = "";
                                strResult += this.myOCRServer.GetChar(lstbitmap[0]);
                                strResult += this.myOCRServer.GetChar(lstbitmap[1]);
                                strResult += this.myOCRServer.GetChar(lstbitmap[2]);
                                strResult += this.myOCRServer.GetChar(lstbitmap[3]);
                                swOCR.Stop();
                                VPState.Report("辨識耗時" + swOCR.ElapsedMilliseconds.ToString() + " ms", MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                                iTCPIP_SendCount++;
                            }

                            #region ==紀錄驗證碼歷程==
                            
                            if (g_bIsListen_OCR_History)
                            {
                                OCR_History OCR_CheckNode = new OCR_History();
                                OCR_CheckNode.ImageA = lstbitmap[0];
                                OCR_CheckNode.ImageB = lstbitmap[1];
                                OCR_CheckNode.ImageC = lstbitmap[2];
                                OCR_CheckNode.ImageD = lstbitmap[3];
                                OCR_CheckNode.SourceImage = bSrc;
                                OCR_CheckNode.Answer = strResult;
                                g_lstOCR_History.Add(OCR_CheckNode);
                            }

                            #endregion

                            if (iTCPIP_SendCount > 1)
                            {
                                VPState.Report("辨識通訊錯誤 , 重送" + iTCPIP_SendCount.ToString() + "次", MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                            }
                        }
                        for (int i = 0; i < lstbitmap.Count; i++)
                        {

                            PictureBox pTarget = null;
                            if (i == 0) pTarget = pbChar0;
                            if (i == 1) pTarget = pbChar1;
                            if (i == 2) pTarget = pbChar2;
                            if (i == 3) pTarget = pbChar3; ;
                            this.Invoke(degDrawImageVirfyCode, pTarget, lstbitmap[i]);
                        }
                    }
                    //strResult = ScreenShot_and_GetVeryfiCodeResult(); 
                    //SubscrEr.PreSubmit(iTickets);
                    string strMessage = string.Format("{0} --> {1}", iDpwnLoadCount, strResult);
                    this.Invoke(degRefreshText, lblVerifyCodeInfo, strMessage);

                    #region 提交
                    Stopwatch swSubmit_Cost = new Stopwatch();
                    swSubmit_Cost.Restart();
                    if (strResult.Length < 4)
                    {
                        //SubscrEr.Submit("GGGG"); 
                        SubscrEr.SubmitAndPreSubtmi("GGGG" , 1); 
                    }
                    else
                    {

                        //IWebElement iAgreeElement = null;
                        //try
                        //{ 
                        //    iAgreeElement = SubscrEr.Driver.FindElement(By.Id("TicketForm_agree"));
                        //    //if ( == false)
                        //    {
                        //        int iClick_x = iAgreeElement.Location.X +0;
                        //        int iClick_y = iAgreeElement.Location.Y +0;
                        //        SubscrEr.HwndController.Mouse_LeftButton_Down_ByOffset(iClick_x, iClick_y);
                        //        Thread.Sleep(10);
                        //        SubscrEr.HwndController.Mouse_LeftButton_Up_ByOffset(iClick_x, iClick_y);
                        //    }
                        //}
                        //catch (Exception ex)
                        //{
                        //    VPState.Report("找不到\"我同意\"這個選項", MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                        //} 
                        
                        SubscrEr.SubmitAndPreSubtmi(strResult, iTickets); 

                    }
                    swSubmit_Cost.Stop();
                    VPState.Report("提交耗時" + swSubmit_Cost.ElapsedMilliseconds.ToString() + " ms", MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                                 
                    string strAlartText = CheckAlart();
                    if (strAlartText == "驗證碼輸入有誤")
                    {
                        iAutoKeyInFailCount++;
                        this.Invoke(degRefreshText, lblVerifyCodeInfo, "驗證碼輸入有誤");
                        bIsKeyInBad = true;
                    }
                    #endregion
                    #region 重新填滿一次選擇張數與同意條款
                    if (strAlartText != "找不到Alert視窗")
                    {
                        SubscrEr.PreSubmit(iTickets);
                    }
                    else
                    {
                        this.Invoke(degRefreshText, lblVerifyCodeInfo, "驗證碼正確，已提交送出！請確認訂單");
                        bIsKeyInBad = false;
                    }
                    #endregion 
                }
            }
            catch (Exception ex)
            { 
                VPState.Report(ex, MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
            }
            this.Invoke(degRefreshText, lblDebug, "自動填單中...OK"); 
            double iCostTime = swLoadingTime_SelectSeat.ElapsedMilliseconds / 1000.0;
            this.Invoke(degRefreshText, lblDebug, "打錯" + iAutoKeyInFailCount.ToString() + "次，開放→自動打碼完畢耗時 : " + iCostTime.ToString("F2") + "秒");
            this.Invoke(degRefreshText, this, "打錯" + iAutoKeyInFailCount.ToString() + "次，開放→自動打碼完畢耗時 : " + iCostTime.ToString("F2") + "秒");
            double iLoadCosttime = swDriverLoading.ElapsedMilliseconds / 1000.0;
            this.Invoke(degRefreshText, lblInfo, "網頁載入時間 : " + iLoadCosttime.ToString("F2") + "秒");
            string strMessaggLog = string.Format("{0}=網頁載:{1}秒 = 總耗時:{2}秒 = 錯:{3}次", strSelectSeat_Text, iLoadCosttime.ToString("F2"), iCostTime.ToString("F2"), iAutoKeyInFailCount.ToString()); 
            VPState.Report(strMessaggLog, MethodBase.GetCurrentMethod(), VPState.eVPType.Windows); 
            Task.Factory.StartNew(() => 
            {
                for (int i = 0; i < 10; i++)
                {
                    //-1=不執行  0 => ATM付款   1 = ibon付款  2 = 信用卡
                   SubscrEr.PayModeClick(g_PayMode);
                   Thread.Sleep(3000);
                } 
            });
        }

        private void timer1_Tick(object sender, EventArgs e)
        { 
        }
        private void btnGetCode_Click(object sender, EventArgs e)
        {
            AllButtonEnableStatue(false);
            SubscrEr.Driver.Navigate().Refresh();
            AllButtonEnableStatue(true);


            int iDayIndex = 0; int.TryParse(txtShowTime.Text, out iDayIndex);
            int iTickets = 0; int.TryParse(txtTicketCount.Text, out iTickets);
            int Index = 0;

            //step 4:取得購票網址
            for (int i = 0; i < Tixcraft.GetActivity(g_ShowSelected).GetShowDate(iDayIndex).SeatCount; i++)
            {
                if (Tixcraft.GetActivity(g_ShowSelected).GetShowDate(iDayIndex).GetSeatTicket(i).Text.Contains(g_SeatInformation))
                {
                    Index = i;
                    break;
                }
            }
            TixcraftSubscriber.Activity.ShowDate.SeatTicket BuyTicket = Tixcraft.GetActivity(g_ShowSelected).GetShowDate(iDayIndex).GetSeatTicket(Index);
            if (BuyTicket == null) return;
            BuyTicket.GetTicket();
            Bitmap VerifyCodeImage = new Bitmap(BuyTicket.VerificationCodeImage);
            SubscrEr.PreSubmit(iTickets);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            AllButtonEnableStatue(false);
            this.Text = SubscrEr.Login(LoginType.FB, strFormatEmailInfo, strPinelPinPassword) + " 帳號 : " + strFormatEmailInfo; 
            AllButtonEnableStatue(true);
        }


        private string UnicodeToString(string str)
        { 
            string outStr = "";
            if (!string.IsNullOrEmpty(str))
            {
                string[] strlist = str.Replace("\\", "").Split('u');
                try
                {
                    for (int i = 1; i < strlist.Length; i++)
                    {
                        //將unicode轉為10進制整數，然後轉為char中文
                        outStr += (char)int.Parse(strlist[i], System.Globalization.NumberStyles.HexNumber);
                    }
                }
                catch (FormatException ex)
                {
                    //outStr = ex.Message;
                }
            }
            return outStr;
        } 
        

        public void UpdateFaceBookStatus()
        {
            FastHttpDriver = SelfFaceBook.miniFacebookBrowser;
            UpdateLable(SelfFaceBook.ConectionStatus, lblLogin);
            UpdateLable(SelfFaceBook.UserName, lblUserName); 
        }

        /// <summary>
        /// Facebook登入 ---> 拓元
        /// </summary>
        public bool Login()
        {
            bool bIsLogin = false;
            SelfFaceBook = new mFacebook(strFormatEmailInfo ,  strPinelPinPassword);
            mFacebook.UpdateStatus = UpdateFaceBookStatus;
            if (SelfFaceBook.Login())
            {
                UpdateLable("帳號驗證中", lblLogin);
                //if (CattleManager.CheckCattleVerification(SelfFaceBook.email))
                if (TixcraftSQL.FBAccountDatabase.IsAllow(SelfFaceBook.email))
                {
                    UpdateLable("等待傳送cookie...", lblLogin);
                    FastHttpDriver = SelfFaceBook.miniFacebookBrowser;
                    //傳送FB Cookie資料到 Tixcraft
                    if (Tixcraft.FacebookLogin(SelfFaceBook.miniFacebookBrowser.Session))
                    {
                        FastHttpDriver = Tixcraft.TixcraftWebDriver;
                        UpdateLable("登入拓元成功!...初始化中.", lblLogin);
                        bIsLogin = true;
                    }
                    else
                    {
                        FastHttpDriver = Tixcraft.TixcraftWebDriver;
                        UpdateLable("登入拓元失敗!", lblLogin);
                        bIsLogin = false;
                    }
                }
                else
                { 
                    UpdateLable("不接受身分登入(無開通)!", lblLogin);
                    if (TixcraftSQL.FBAccountDatabase.IsExist(SelfFaceBook.email))
                    {
                        TixcraftSQL.FBAccountDatabase.UpDate(SelfFaceBook.email, SQLBoolean.off);
                    }
                    else
                    {
                        TixcraftSQL.FBAccountDatabase.Add(SelfFaceBook.email, SQLBoolean.off, DateTime.Now.ToString());
                    }
                    bIsLogin = false;
                }
            }
            else
            {
                bIsLogin = false;
            }
            return bIsLogin;
        }


        public void AllButtonEnableStatue(bool bIsEnable)
        {
            //開搶
            this.Invoke(degControlEnable, button3, bIsEnable); 

            UpdateLable("帳號 : " + this.strFormatEmailInfo, lblAddress);
            if (bIsEnable)
            {
                UpdateLable(" - OK - ", lblLogin); 
            }
            bIsBrowserBusying = !bIsEnable;
        }
         
        private void button1_Click(object sender, EventArgs e)
        {
            //Task.Factory.StartNew(() =>
            //{

            //    this.Invoke(degRefreshText, lblDebug, "載入節目資訊中......");
            //    TimeSpan ts1 = new TimeSpan(DateTime.Now.Ticks);//紀錄目前時間
            //    string strHomePage = "https://tixcraft.com/activity";
            //    SubscrEr.GoTo(strHomePage);
            //    this.Invoke(degRefreshText, lblDebug, "轉換資料中......");
            //    SubscrEr.allactivity = SubscrEr.FindActivity();
            //    TimeSpan ts2 = new TimeSpan(DateTime.Now.Ticks);//紀錄目前時間
            //    Debug.Print("取得節目資訊Spend time:" + ts2.Subtract(ts1).TotalMilliseconds);
            //    this.Invoke(degRefreshBox, listBox1, SubscrEr.allactivity);
            //    this.Invoke(degRefreshText, lblDebug, "取得節目資訊完成!! ( 不需要再重複點選 )");
            //});
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {

        }

        public void button3_Click(object sender, EventArgs e)
        {
            try
            { 
                UpdateCircleBar(circularProgressBar1, 3); 
                if (g_IsAutoFlag)
                {
                    this.BackColor = Color.SandyBrown;
                }
                else
                {
                    this.BackColor = Color.White;
                }

                //從UI上讀取 開賣後智能延遲的毫秒
                long clsLongTimerDelayAutoAI = 0;
                long.TryParse(txt_TimeDelayRun_AutoAI.Text, out clsLongTimerDelayAutoAI);
                g_DelayThreshold = clsLongTimerDelayAutoAI;
                

                g_SeatInformation = txtSeatInformation.Text;
                AllButtonEnableStatue(false);
                //for (int i = 0; i < 100;i++ )
                Task ttt =  Task.Factory.StartNew(() =>
                {
                    TimerStop(); 
                    bool bIsOpenAutoFlag = false;
                    while (bIsOpenAutoFlag == false)
                    {
                        if (g_IsAutoFlag)
                        {
                            // 有打勾
                            if (g_CookieServer.GetAutoFlag())   //伺服器上搜尋開關是否打開
                            {
                                bIsOpenAutoFlag = true;
                            }
                        }
                        else
                        {
                            // 沒打勾
                            bIsOpenAutoFlag = true;
                            break;
                        }

                        UpdateBackColor(Color.PaleGreen, this);
                        UpdateLable("等候主控端開啟中...", lblLogin);
                        Thread.Sleep(750);
                        UpdateLable("等候主控端開啟中", lblLogin);
                        Thread.Sleep(750);
                    } 
                    UpdateLable("執行開搶", lblLogin);
                    g_bFlagStartBuyStatue = true;
                    if (bIsUseOLDsch == true)
                    { 
                        //一般流程
                        Test();
                    }
                    else
                    {
                        //封包流程
                        Test2019();
                    }
                    UpdateCircleVisiable(circularProgressBar1, false);
                    AllButtonEnableStatue(true);
                    g_bFlagStartBuyStatue = false; 
                    TimerStop(); 

                }); 
            }
            catch (Exception ex)
            {

                VPState.Report(ex, MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
            }
        }

        private void btnSnapShot_Click(object sender, EventArgs e)
        {
            SubscrEr.ScreenShot();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {

        }

        public void ChangeLabelMessage(string strMsg)
        {
            lblDebug.Text = strMsg;
        }
        /// <summary>
        /// 訂單查詢頁面 
        /// </summary>
        public void GoToTicketInformationPage()
        {

            Task.Factory.StartNew(() =>
            {
                SubscrEr.GoTo("https://tixcraft.com/order");
            });
        }

        public void btnCheckCode_Click(object sender, EventArgs e)
        {
            //checkCode = inputbox
            //submitButton = button 
            SendCheckCode("45");
        }
        public void Set_BuyDataInformation(string sDay, string sTicketCount, string sSeats)
        {
            txtShowTime.Text = sDay;
            txtTicketCount.Text = sTicketCount;
            txtSeatInformation.Text = sSeats;
        }
        public void Set_CrediteCardInformation(string sAccount, string sMonth, string sYear , string sCVE)
        {
            txtCredit_Card_Account.Text = sAccount;
            txtCredit_Card_Month.Text = sMonth;
            txtCredit_Card_Year.Text = sYear;
            txtCredit_Card_CVE.Text = sCVE;
        }
        public void Set_YW0_MouseClickDelayTime(int iDelay)
        {
            g_Yw0_DelayTime = iDelay;
        }
        public string SendCheckCode(string strCheckCode)
        {
            if (true)
            {
                #region == JS填入考試答案 == 
                string strScrpit = SubscrEr.JS_ReadFile("Tix_KeyIn_Submit_CheckCode.txt");
                strScrpit += string.Format("\nTix_KeyInQuestionAnswer(\"{0}\");\n", strCheckCode);
                SubscrEr.InjectJavaScript(strScrpit);
                VPState.Report("填入考試答案 : " + strCheckCode, MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                #endregion 
            }
            else
            {
                #region == Seleunim 手動填入考試答案 == 
                IWebElement input_checkcode = null;
                IWebElement submit_checkcode = null;
                try
                {
                    input_checkcode = SubscrEr.Driver.FindElement(By.Id("checkCode"));
                }
                catch (Exception ex)
                {
                    VPState.Report(ex, MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                    return "操作失敗:不在驗證頁面(無Code)";
                }
                try
                {
                    submit_checkcode = SubscrEr.Driver.FindElement(By.Id("submitButton"));
                }
                catch (Exception ex)
                {
                    VPState.Report(ex, MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                    return "操作失敗:不在驗證頁面(無鈕)";
                }
                input_checkcode.Clear();
                input_checkcode.SendKeys(strCheckCode);
                submit_checkcode.Click();
                #endregion
            }
             
            IAlert alart = null;
            int iTargetTimeOut = 3000;
            int iCurrentTime = 0;
            int iDelayms = 100;
            while (alart == null)
            {
                try
                {

                    alart = SubscrEr.Driver.SwitchTo().Alert();
                }
                catch (Exception ex)
                {

                    //VPState.Report(ex, MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                }
                Thread.Sleep(iDelayms);
                iCurrentTime += iDelayms;
                if (iTargetTimeOut < iCurrentTime) return "找不到Alert視窗";
            }
            string strResult = "";
            if (alart.Text.Contains("不正確"))
            {
                strResult = "驗證碼輸入有誤";
            }
            else
            {
                strResult = "驗證碼正確";
            }
            VPState.Report(strResult +" : " + strCheckCode, MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                
            try
            { 
                try
                {
                    alart.Accept();
                }
                catch (Exception ex)
                {
                    alart.Accept();
                }
            }
            catch
            {

            }
            return strResult;
        }

        private void ckAutoTypeCheckCode_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void chkRandSeats_CheckedChanged(object sender, EventArgs e)
        {

            g_bIsRandSeats = chkRandSeats.Checked;
            txtSeatInformation.Enabled = !g_bIsRandSeats;
        } 
        private string DownLoadAnswer()
        {
            string strAnswer = "";
            //從網路上下載最新的答案   
            //List<SAStruct> lstTmpAllList = TixcraftSQL.ShareAnswerDatabase.GetTotalAccount();
            //if (lstTmpAllList.Count > 0)
            //{
            //    strAnswer = lstTmpAllList[lstTmpAllList.Count - 1].MAnswer;
            //}
            strAnswer = g_ShareAnswerServer.DownLoadTestAnswerFromSQL(); 
            return strAnswer;
        }
        private string DownLoadAnswer(string strMsg)
        {
            string strAnswer = ""; 
            strAnswer = g_ShareAnswerServer.DownLoadTestAnswerFromSQL(strMsg);
            return strAnswer;
        }

        public void LoginPixelPin()
        {
            string[] strGoogleExcelTable = strFormatEmailInfo.Split('\t');
            if(strGoogleExcelTable.Length >=2)
            {
                LoginPixelPin(strGoogleExcelTable[1]);
            } 
        }

        public void LoginGoogle()
        {

            string[] strGoogleExcelTable = strFormatEmailInfo.Split('\t');
            if (strGoogleExcelTable.Length >= 3)
            {
                LoginGoogle
                    (
                    strGoogleExcelTable[0], // gmail
                    strGoogleExcelTable[1], // pwd
                    strGoogleExcelTable[2] // backup email
                    );
            }

        }

        public void LoginGoogle(string strGmail , string strGPwd , string strBackupEmail)
        { 
            if (SubscrEr != null)
            {

                SubscrEr.GoTo("https://tixcraft.com/login");
                SubscrEr.GoTo("https://tixcraft.com/login/google");
                Thread.Sleep(500);
                SubscrEr.GoTo("https://tixcraft.com/login");
                SubscrEr.GoTo("https://tixcraft.com/login/google");
                Thread.Sleep(500); 
                string strJavaScript = SubscrEr.JS_ReadFile("GoogleLoginer.txt");
                strJavaScript = strJavaScript + string.Format("\nRunLogin(\'{0}\' , \'{1}\' , \'{2}\');\n", strGmail, strGPwd, strBackupEmail.Replace('\r',' '));
                Thread.Sleep(500);
                SubscrEr.InjectJavaScript(strJavaScript); 
            } 
        }

        public void LoginPixelPin(string strEmail)
        {

            //VIPGeneral.Window.VPProgressControl pdb = new VIPGeneral.Window.VPProgressControl();
            //pdb.MaxValue = 5;
            //pdb.Start();
            //pdb.Text = "進入會員登入頁面...";
            SubscrEr.GoTo("https://tixcraft.com/login");
            Thread.Sleep(1000);
            SubscrEr.GoTo("https://tixcraft.com/login/pixelpin");
            Thread.Sleep(1000);
            //pdb.Next(1);    //--1
            //pdb.Text = "進入會員登入頁面...OK";
            // 自動打帳號
            while (SubscrEr.Driver.Url.Contains("https://login.pixelpin.io/Account/emailselect") == true)
            {
                //pdb.Text = "輸入帳號 : " + strEmail;
                string strScrpitSelectSeat = SubscrEr.JS_ReadFile("Tix_PixelPinLogin.txt");
                strScrpitSelectSeat += string.Format("Pix_KeyInRegist_Username(\'{0}\');", strEmail);
                SubscrEr.InjectJavaScript(strScrpitSelectSeat);
            }
            //pdb.Next(1);    //--2
            // 自動打密碼
            //144,388 - 260,388 - 378,388 - 615,388 
            int iOldX = 0;
            int iOldY = 0;
            int iCurrentX = 0;
            int iCurrentY = 0;
            ReadOnlyCollection<IWebElement> iImage = null;
            // 載入後，等待影像從右邊往左滑動完畢
            Bitmap VerifyLoactionImage;
            HObject ho_Image, ho_ImageGray, ho_Region;
            HObject ho_ConnectedRegions, ho_SelectedRegions, ho_SelectedRegions1;
            HObject ho_SelectedRegions2, ho_SortedRegions, ho_ObjectSelected;


            // Local control variables 

            HTuple hv_Area, hv_Row=null, hv_Column=null;

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_ImageGray);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions2);
            HOperatorSet.GenEmptyObj(out ho_SortedRegions);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelected);
            bool Check = true;
            //pdb.Next(1);    //--3
            //pdb.Text = "定位中...";
            
            int iDoCount = 0;

            // 取得螢幕大小後，進行Resize到最大
            int iSWidth = Screen.PrimaryScreen.WorkingArea.Width;
            int iSHeigh = Screen.PrimaryScreen.WorkingArea.Height;
            SubscrEr.HwndController.SetToForegroundWindow();
            SubscrEr.HwndController.ReSize(0, 0, iSWidth, iSHeigh);

            while (Check)
            {

                //pdb.Text = "取像中...";
                SubscrEr.ScreenShot();
                Thread.Sleep(2000);
                SubscrEr.ScreenShot();
                //pdb.Text = "取像中...OK";
                VerifyLoactionImage = new Bitmap(SubscrEr.bSnapShot);

                HObject mySource = HImageConvertFromBitmap32(VerifyLoactionImage); 

                ho_ImageGray.Dispose();
                HOperatorSet.Rgb3ToGray(mySource, mySource, mySource, out ho_ImageGray);
                ho_Region.Dispose();
                HOperatorSet.VarThreshold(ho_ImageGray, out ho_Region, 15, 15, 0.2, 2, "dark");
                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
                ho_SelectedRegions.Dispose();
                HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area",
                    "and", 120, 150);
                ho_SelectedRegions1.Dispose();
                HOperatorSet.SelectShape(ho_SelectedRegions, out ho_SelectedRegions1, "inner_width",
                    "and", 9, 13);
                ho_SelectedRegions2.Dispose();
                HOperatorSet.SelectShape(ho_SelectedRegions1, out ho_SelectedRegions2, "inner_height",
                    "and", 9, 13);
                ho_SortedRegions.Dispose();
                HOperatorSet.SortRegion(ho_SelectedRegions2, out ho_SortedRegions, "first_point",
                    "true", "column");
                ho_ObjectSelected.Dispose();
                HOperatorSet.SelectObj(ho_SortedRegions, out ho_ObjectSelected, 1);
                HOperatorSet.AreaCenter(ho_ObjectSelected, out hv_Area, out hv_Row, out hv_Column);

                if (hv_Row.D > 0)
                {
                    Check = false;
                    //pdb.Text = "分數正確";
                }
                else 
                { 
                    //pdb.Text = "分數不對，重新判別";
                }

                if (iDoCount > 10)
                { 
                    //pdb.Text = "錯誤次數過多，手動登入後方可開始進行搶票";
                    Thread.Sleep(1000);
                    //pdb.Close(); 
                    SubscrEr.HwndController.ShowNormal();   // 視窗正常化
                    return;
                }

                Thread.Sleep(500);
                iDoCount++;
            }

            //pdb.Next(1);    //--4
            //pdb.Text = "開始解題";

            int iClick_x = (int)(hv_Column.D);
            int iClick_y = (int)(hv_Row.D);
            //pdb.Text = "開始解題 - 座標:(" + iClick_x.ToString() + " , " + iClick_y.ToString() + " )" + " hv_Score分數:" + hv_Score.D.ToString("F2");
            int iResultX = 0;
            int iResultY = 0;

            iResultX = iClick_x + 49;
            iResultY = iClick_y +5;
            Browser_MouseClickDelay(iResultX, iResultY);

            Thread.Sleep(250);

            iResultX = iClick_x  + 116 ;
            iResultY = iClick_y +5;
            Browser_MouseClickDelay(iResultX, iResultY);

            Thread.Sleep(250);

            iResultX = iClick_x  + 185;
            iResultY = iClick_y +5;
            Browser_MouseClickDelay(iResultX, iResultY);

            Thread.Sleep(250);

            iResultX = iClick_x  +253;
            iResultY = iClick_y +5;
            Browser_MouseClickDelay(iResultX, iResultY);
            Thread.Sleep(3000);
            SubscrEr.HwndController.ShowNormal();   // 視窗正常化
            SubscrEr.HwndController.Minimize();     // 視窗最小化
            Thread.Sleep(500);

            ho_Image.Dispose();
            ho_ImageGray.Dispose();
            ho_Region.Dispose();
            ho_ConnectedRegions.Dispose();
            ho_SelectedRegions.Dispose();
            ho_SelectedRegions1.Dispose();
            ho_SelectedRegions2.Dispose();
            ho_SortedRegions.Dispose();
            ho_ObjectSelected.Dispose();
        }

        private HObject HImageConvertFromBitmap32(Bitmap bmp)
        {
            HObject ho_Image;
            HOperatorSet.GenEmptyObj(out ho_Image);
            unsafe
            {
                BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                unsafe
                {
                    HOperatorSet.GenImageInterleaved(out ho_Image, bmpData.Scan0, "bgrx", bmp.Width, bmp.Height, -1, "byte", bmp.Width, bmp.Height, 0, 0, -1, 0);
                }
                bmp.UnlockBits(bmpData);
                return ho_Image;
            }

        }

        private void Browser_MouseClickDelay(int x, int y)
        {

            SubscrEr.HwndController.Mouse_LeftButton_Down(x-2, y+109);
            Thread.Sleep(200);
            SubscrEr.HwndController.Mouse_LeftButton_Up(x-2, y+109);
            Thread.Sleep(100);
        }


        private void chkAutoByPassQuestion_CheckedChanged(object sender, EventArgs e)
        {
            //g_bIsAutoByPassQuestion = chkAutoByPassQuestion.Checked;
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        public void btnShowTop_Click(object sender, EventArgs e)
        { 
            if (LoginMode == LoginType.PixelPin)
            {
                LoginPixelPin();
            }
            else 
            {
                LoginGoogle();
            } 
        }

        private void lblAddress_Click(object sender, EventArgs e)
        {

        }

        public void btnStopBuy_Click(object sender, EventArgs e)
        {
            g_bFlagStartBuyStatue = false;

            UpdateCircleVisiable(circularProgressBar1, false);
        }

        private void btnBetaImage_Click(object sender, EventArgs e)
        {
        }

        private void ckbAutoFlag_CheckedChanged(object sender, EventArgs e)
        {
            g_IsAutoFlag = ckbAutoFlag.Checked;
        }

        private void ckb_UsingOtherAnswer_CheckedChanged(object sender, EventArgs e)
        {

            g_bIsUsing_OtherAnswerByWindow = ckb_UsingOtherAnswer.Checked;
            btn_SetOtherAnswer.Enabled = g_bIsUsing_OtherAnswerByWindow;
            txtOtherAnswer.Enabled = g_bIsUsing_OtherAnswerByWindow;
            if (g_bIsUsing_OtherAnswerByWindow == false)
            { 
                UpdateLable("使用網路解答", lblLogin);
            }
        }

        private void btn_SetOtherAnswer_Click(object sender, EventArgs e)
        {
            g_OtherAnswerByWindow = txtOtherAnswer.Text;
            txtOtherAnswer.Text = "";
            UpdateLable("使用自定義解答 : " + g_OtherAnswerByWindow, lblInfo);
        }

        private void txtOtherAnswer_KeyDown(object sender, KeyEventArgs e)
        { 
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                btn_SetOtherAnswer_Click(null, null);
            }
        }

        public void btnAutoFillCreditCard_Click(object sender, EventArgs e)
        {
            if (txtCredit_Card_Account.Text == "") { lblCreditCard.Text = "信用卡卡號不能空白"; return; }
            if (txtCredit_Card_Month.Text == "") { lblCreditCard.Text = "月份不能空白"; return; }
            if (txtCredit_Card_Year.Text == "") { lblCreditCard.Text = "年份不能空白"; return; }
            if (txtCredit_Card_CVE.Text == "") { lblCreditCard.Text = "卡片檢查碼不能空白"; return; }


            if (this.SubscrEr != null)
            {
                this.SubscrEr.AutoFill_CridetCard(
                    txtCredit_Card_Account.Text,
                    txtCredit_Card_Month.Text,
                    txtCredit_Card_Year.Text,
                    txtCredit_Card_CVE.Text
                    );
                lblCreditCard.Text = "已將卡號填入網頁中";
            }
        }

        private void ckbSwitchPageStepByStep_CheckedChanged(object sender, EventArgs e)
        {
            bIsSwitchPageStepByStep = ckbSwitchPageStepByStep.Checked;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void btnRefreshOCRHistory_Click(object sender, EventArgs e)
        {
            lst_OCR_History.Items.Clear(); 
            foreach (OCR_History p in g_lstOCR_History)
            {
                lst_OCR_History.Items.Add(p.Answer);
            }
        }

        private void lst_OCR_History_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lst_OCR_History.SelectedIndex >= 0 && lst_OCR_History.SelectedIndex <= g_lstOCR_History.Count)
            {
                pbChar_History_A.Image = g_lstOCR_History[lst_OCR_History.SelectedIndex].ImageA;
                pbChar_History_B.Image = g_lstOCR_History[lst_OCR_History.SelectedIndex].ImageB;
                pbChar_History_C.Image = g_lstOCR_History[lst_OCR_History.SelectedIndex].ImageC;
                pbChar_History_D.Image = g_lstOCR_History[lst_OCR_History.SelectedIndex].ImageD;
                ptb_OCR_Source.Image = g_lstOCR_History[lst_OCR_History.SelectedIndex].SourceImage;
            }
        }

        private void ckb_listen_ocr_CheckedChanged(object sender, EventArgs e)
        {
            g_bIsListen_OCR_History = ckb_listen_ocr.Checked;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string strTempName = GetRandTaiwanName();
            RegisteTixPage_HongKong(strTempName, txt_window_gmail.Text); 

            lblTaiwanHongKong.Text = "已生成香港人！";
            txtGeneratorName.Text = strTempName;
        } 

        public string GetRandTaiwanName()
        {
            FastHttpWebDriver miniBrwoser = new FastHttpWebDriver();
            string strPageSource = miniBrwoser.GetWebSourceCodeBig5("http://www.richyli.com/name/index.asp");
            List<SWebElement> lstTds = HtmlAnalyze.FindElement(strPageSource, WebBy.Tag("td"));
            List<string> lstChineseNameList = new List<string>();
            SWebElement findEle = null;
            for (int i = 0; i < lstTds.Count; i++)
            {
                if (lstTds[i].Context.Contains("<td valign=\"top\">")) findEle = lstTds[i];
            }
            if (findEle != null)
            {
                string[] strSp = findEle.Context.Split('、');
                foreach (string pp in strSp)
                {
                    if (pp.Length == 3)
                    {
                        lstChineseNameList.Add(pp);
                    }
                }
            }
            Random rd1 = new Random();
            int indexRand = rd1.Next(0, lstChineseNameList.Count);
            //隨機住址 (檔案)
            string strRand = lstChineseNameList[indexRand];
            return strRand;
        }

        public void RegisteTixPage_HongKong(string Name , string sEmail)
        {
            if (SubscrEr != null)
            {
                //Tix_Register("張正揚", "N121598888", "704台南市北區西門路四段70巷17號", rand(1000000, 9000000).toString());
                string[] strAddress = SubscrEr.JS_ReadFile("TixRegScript\\HongKong_Addres.txt").Split('\n');
                List<string> lstAddress = new List<string>();
                foreach (string p in strAddress)
                {
                    if (p != "")
                    {
                        lstAddress.Add(p);
                    }
                }
                Random rd1 = new Random();
                int indexRand = rd1.Next(0, lstAddress.Count);
                //隨機住址 (檔案)
                string strRandAddress = strAddress[indexRand];

                string strJavaScript = SubscrEr.JS_ReadFile("TixRegScript\\HongKong_TixRegister.txt");
                //strJavaScript = strJavaScript + string.Format("\nTix_Register(\"{0}\", \"{1}\", rand(1000000, 9000000).toString());\n", Name, strRandAddress);
                strJavaScript = strJavaScript + string.Format("\nTix_Register(\"{0}\", \"{1}\", rand(1000000, 9000000).toString() , \"{2}\");\n", Name, strRandAddress, sEmail);
                SubscrEr.InjectJavaScript(strJavaScript);
            }
        }

        public void RegisteTixPage_Taiwan(string Name , string sEmail)
        {
            if (SubscrEr != null)
            {
                //Tix_Register("張正揚", "N121598888", "704台南市北區西門路四段70巷17號", rand(1000000, 9000000).toString());
                string[] strAddress = SubscrEr.JS_ReadFile("TixRegScript\\Taiwan_Addres.txt").Split('\n');
                List<string> lstAddress = new List<string>();
                foreach (string p in strAddress)
                {
                    if (p != "")
                    {
                        lstAddress.Add(p);
                    }
                }
                Random rd1 = new Random();
                int indexRand = rd1.Next(0, lstAddress.Count);
                //隨機住址 (檔案)
                string strRandAddress = strAddress[indexRand];

                string strJavaScript = SubscrEr.JS_ReadFile("TixRegScript\\Taiwan_TixRegister.txt");
                strJavaScript = strJavaScript + string.Format("\nTix_Register(\"{0}\", \"{1}\", rand(1000000, 9000000).toString() , \"{2}\");\n", Name, strRandAddress, sEmail);
                SubscrEr.InjectJavaScript(strJavaScript);
            }
        }
        private void btnRegister_Taiwan_Click(object sender, EventArgs e)
        { 
            string strTempName = GetRandTaiwanName();
            RegisteTixPage_Taiwan(strTempName, txt_window_gmail.Text);
            lblTaiwanHongKong.Text = "已生成台灣人！";
            txtGeneratorName.Text = strTempName;
        }

        private void txtCredit_Card_Year_TextChanged(object sender, EventArgs e)
        {
            if (txtCredit_Card_Year.Text.Length == 2)
            {
                txtCredit_Card_Year.Text = "20" + txtCredit_Card_Year.Text;
            }
        }

        private void btn_SnapScreen_Window_Click(object sender, EventArgs e)
        {
            //SubscrEr.ScreenShot();
            //Bitmap bTemp = new Bitmap(SubscrEr.bSnapShot);
            //string strSnapShotImage = "C://Window_Debug.bmp";

            //bTemp.Save(strSnapShotImage);
            //Process.Start(strSnapShotImage);
        }

        private void btn_CookieTests_Click(object sender, EventArgs e)
        {
            btn_CookieTests.Enabled = false;
            Task.Factory.StartNew(() => 
            {
                Stopwatch swCopyCookie = new Stopwatch();
                swCopyCookie.Restart();
                SubscrEr.CopyCookieTo(ref g_tTSBuyer.TixcraftWebDriver);
                swCopyCookie.Stop();

                g_tTSBuyer.TixcraftWebDriver.GetWebSourceCode("https://tixcraft.com");
                //g_tTSBuyer.TixcraftWebDriver.GetWebSourceCode("https://tixcraft.com/ticket/ticket/19_WuBai/5658/8/18");

                g_tTSBuyer.RefreshActivity();
                TSubscriber.TixcraftSubscriber.Activity eleActivity = g_tTSBuyer.GetActivity(g_ShowSelected);

                eleActivity.RefreshDate();
                eleActivity.GetShowDate(0).RefreshAllSeat();
                string strSeatName = "紅220區2800";
                eleActivity.GetShowDate(0).GetSeatTicket(strSeatName).GetTicket();

                UpdateLable(" CopyCookie : " + swCopyCookie.ElapsedMilliseconds, label18);
                UpdateLable(eleActivity.GetShowDate(0).GetSeatTicket(strSeatName).Text, label19);
                UpdateImage(eleActivity.GetShowDate(0).GetSeatTicket(strSeatName).VerificationCodeImage, pb_cookie_pb);
                SubscrEr.GoTo(eleActivity.GetShowDate(0).GetSeatTicket(strSeatName).url);
            });
            btn_CookieTests.Enabled = true;
        }

        private void btn_BetaSubmit_Click(object sender, EventArgs e)
        {
            string strSeatName = "紅220區2800";
            bool bIsBuySuccessful = g_tTSBuyer.GetActivity(g_ShowSelected).GetShowDate(0).GetSeatTicket(strSeatName).Buy(txt_BetaVeryfiCode.Text, 1);

            //通訊格式 : URL解碼
            string strResponse = g_tTSBuyer.GetActivity(g_ShowSelected).GetShowDate(0).GetSeatTicket(strSeatName).TixcraftWebDriver.GetWebSourceCode("https://tixcraft.com/order");

            Thread.Sleep(2000);
            strResponse = g_tTSBuyer.GetActivity(g_ShowSelected).GetShowDate(0).GetSeatTicket(strSeatName).TixcraftWebDriver.GetWebSourceCode("https://tixcraft.com/ticket/check");

            strResponse = Uri.UnescapeDataString(strResponse.Replace("\\\\","\\"));

            //for (int i = 0; i < 20; i++)
            //{
            //    strResponse = g_tTSBuyer.GetActivity(g_ShowSelected).GetShowDate(0).GetSeatTicket(0).TixcraftWebDriver.GetWebSourceCode("https://tixcraft.com/ticket/check");

            //    Thread.Sleep(100);
            //    VPState.Report(strResponse, MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
            //}
             

              

            UpdateLable(strResponse, label18);
            btn_CookieTests.Enabled = true;
        } 

        private void btnRefreshVeryfi_Click(object sender, EventArgs e)
        {

            g_tTSBuyer.GetActivity(g_ShowSelected).GetShowDate(0).GetSeatTicket(0).RefreshVeryfiImage();

            Image VerificationCodeImage = g_tTSBuyer.GetActivity(g_ShowSelected).GetShowDate(0).GetSeatTicket(0).VerificationCodeImage;

            UpdateImage(VerificationCodeImage, pb_cookie_pb); 

        }

        private void btn_BetaDownloadVeryfiCode_Click(object sender, EventArgs e)
        {
        }

        private void ckbASK_CheckPage_CheckedChanged(object sender, EventArgs e)
        {
            bIsASK_CheckPage = ckbASK_CheckPage.Checked;
        }

        private void btnShowIP_Click(object sender, EventArgs e)
        {
            SubscrEr.GoTo("https://showip.net/");
            lblProxyText.Text = this.g_strProxyInfo;
        }

        private void chk_TimerDelayAutoAI_CheckedChanged(object sender, EventArgs e)
        {
            g_bIsDelayTimerEnable = chk_TimerDelayAutoAI.Checked;
        }


    }
}
