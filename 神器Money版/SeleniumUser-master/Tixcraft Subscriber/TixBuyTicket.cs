using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OCRInspection;
using OpenQA.Selenium;
using System.Collections.ObjectModel;
using System.Diagnostics;
using VIPGeneral;
using System.Threading.Tasks;
using System.Threading;
using HTool;
using TixcraftAutomationSystem;
using System.Reflection;
using SQLTest;
namespace Tixcraft_Subscriber
{
    public partial class TixBuyTicket : Form
    {
        public TixBuyTicket()
        {
            InitializeComponent();
        }
        //==主要控件== 
        //==> 瀏覽器 
        public Subscriber SubscrEr = new Subscriber();
        //==> OCR辨識器
        OCRServer myOCRServer = new OCRServer();
        //==> 自動回答伺服器
        TixcraftSQLServer myAnswerServer = new TixcraftSQLServer();
        

        public int Tix_Setting_Day = 0;
        public string Tix_Setting_SeatName = "";

        // 參數
        public int g_SelectDay = 0;
        public string g_SelectSeat = "區800";
        public int g_SelectTicket = 1;

        public bool g_bIsRunAuto = false;
        public Task g_RunThread = null;

        private delegate void udDrawImage(PictureBox ctr, Bitmap msg);
        private udDrawImage degDrawImageVirfyCode;
        private delegate void udRefreshText(Control ctr, string msg);
        private udRefreshText degRefreshText;
        private delegate void udControlEnable(Control ctr, bool bIsEnable);
        private udControlEnable degControlEnable;


        #region Sam => 委派更新畫面
        private delegate void myIGCallBack(Image myIG, PictureBox ctl);
        private delegate void myTxtCallBack(string text, Label ctl);
        private delegate void myTxtBoxCallBack(string text, TextBox ctl);
        private delegate void myComboBoxCallBack(List<string> text, int index, ComboBox ctl);
        private delegate void myControlCallBack(string text, Control ctl);
        private delegate void myControlVisibleCallBack(bool value, Control ctl);
        private delegate void myFormCallBack(string text, Form ctl);
        private delegate void myWebCallBack(string text, WebBrowser ctl);

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

        private void TixBuyTicket_Load(object sender, EventArgs e)
        {
            //--
            degDrawImageVirfyCode = DrawImage;
            degRefreshText = RefreshText;
            degControlEnable = ControlEnable;
            //--
            myAnswerServer.Connect();
            string strAnswer = myAnswerServer.DownLoadTestAnswerFromSQL();
            //==開Google瀏覽器==
            VIPGeneral.Window.VPProgressControl pro = new VIPGeneral.Window.VPProgressControl();
            pro.MaxValue = 100;
            pro.Text = "共享伺服器資訊 : " + strAnswer  + " ....."+ "開啟瀏覽器中...";
            pro.Start();
            SubscrEr.OpenBrowser(true);
            pro.Next(20);
            pro.Text = "AI連線.....";
            myOCRServer.strServerIP = "127.0.0.1";
            myOCRServer.Connect(); 
            pro.Next(30);
            pro.Text = "進入網頁.....";
            SubscrEr.GoTo_normal("https://tixcraft.com/activity/detail/2018_SJ");
            pro.Next(50);
            pro.Close();
            //SubscrEr.GoTo_normal("https://tixcraft.com/activity/Game/18_JACKY");
            //
        }

        /// <summary>
        /// 取消一次Alert視窗，若無則返回
        /// </summary>
        public void CheckAlert()
        {
            try
            {

                IAlert alart = SubscrEr.Driver.SwitchTo().Alert();
                alart.Accept();
            }
            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// 兩秒內專門取消Alert視窗，至少Accept() 一個視窗，否則超過2秒則跳出
        /// </summary>
        /// <returns></returns>
        private string CheckAlert_Delay2S()
        {
            IAlert alart = null;
            int iTargetTimeOut = 2000;
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
                    return strResult;
                }
                catch (Exception ex)
                {
                    //VPState.Report(ex, MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);

                }
                Thread.Sleep(iDelayms);
                iCurrentTime += iDelayms;
                if (iTargetTimeOut < iCurrentTime) return "找不到Alert視窗";
            }
            return "返回";
        }
        private void btn_Start_Buy_Click(object sender, EventArgs e)
        {
            string strSeat = txtSeatInformation.Text;
            int iTicket = 0; int.TryParse(txtTicketCount.Text, out iTicket);
            int iDay = 0; int.TryParse(txtShowTime.Text, out iDay);


            if (g_RunThread == null)
            {
                g_RunThread = Task.Factory.StartNew(() =>
                {

                    while (g_bIsRunAuto == true)
                    {
                        try
                        {
                            long cost_time_selectDay = 0; //選擇天數耗時
                            long cost_time_selectSeat = 0; //選擇座位耗時
                            long cost_time_selectCount = 0; //選擇張數耗時

                            //  == 刷新日期直到進入日期成功 ==  
                            CheckAlert(); //-> 先把Alert視窗關掉
                            string strCurrentURL = SubscrEr.Driver.Url; //->取得當前網址

                            //--> 判斷是否在訂購頁面(自動跳轉)
                            if (strCurrentURL.Contains("detail"))
                            {
                                SubscrEr.GoTo_normal(strCurrentURL.Replace("detail", "game"));
                            }
                            // 選擇天期
                            if (strCurrentURL.Contains("game"))
                            {
                                Tix_SelectDay(iDay, ref cost_time_selectDay);
                            } 
                            // 取得座位
                            if (strCurrentURL.Contains("area"))
                            {
                                List<TixElementSeats> lstSeats = TixGet_Seats();
                                // 選座位
                                Tix_SelectSeat(lstSeats, strSeat, false, ref cost_time_selectSeat);
                            }
                            if (strCurrentURL.Contains("ticket/ticket"))
                            {
                                //填寫張數 & 打勾
                                Tix_PreSubmit(iTicket, ref cost_time_selectCount);
                                Tix_KeyInVeryfiCode();
                            }
                            // 自動回答防黃牛
                            if (strCurrentURL.Contains("veryfi"))
                            {
                                Tix_KeyInTestAnswer();
                            }
                            string strMsg = string.Format("days : {0} seats : {1} , Presubmit :{2} ", cost_time_selectDay, cost_time_selectSeat, cost_time_selectCount);
                            UpdateLable(strMsg, label1);
                        }
                        catch (Exception)
                        {

                        }
                    }
                    g_RunThread = null;
                });
            }

            //string strMsg = string.Format("days : {0} seats : {1} , Presubmit :{2} ", cost_time_selectDay, cost_time_selectSeat, cost_time_selectCount);
            //label1.Text = strMsg;

        }


        //選擇天數
        public List<IWebElement> TixGet_Days()
        {
            //===搜尋立即訂購按鈕===
            List<IWebElement> lstDays = new List<IWebElement>();
            try
            {
                ReadOnlyCollection<IWebElement> Days = SubscrEr.Driver.FindElements(By.TagName("input"));
                foreach (IWebElement p in Days)
                {
                    if (p.GetAttribute("value") == "立即訂購")
                    {
                        lstDays.Add(p);
                    }
                }
            }
            catch (Exception)
            {

            }
            return lstDays;
        }
        public void Tix_SelectDay(int index, ref long cost_time_ms)
        {
            Stopwatch sw_cost = new Stopwatch();
            sw_cost.Restart();
            string strURLCheck = SubscrEr.Driver.Url;
            if (strURLCheck.Contains("game"))
            {
                while (true)
                {
                    List<IWebElement> Days = TixGet_Days();
                    if (Days.Count > 0 && Days.Count > index)
                    {
                        string strURL = Days[index].GetAttribute("data-href");
                        SubscrEr.GoTo_normal("https://tixcraft.com" + strURL);
                        break;
                    }
                    else
                    {
                        try
                        {
                            string mref = SubscrEr.Driver.Url;
                            if (mref.Contains("game"))
                            {
                                SubscrEr.Driver.Navigate().Refresh();
                            }
                            else
                            {
                                break;
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }
            sw_cost.Stop();
            cost_time_ms = sw_cost.ElapsedMilliseconds;
        }

        //選擇座位
        public List<TixElementSeats> TixGet_Seats()
        {
            //==最終結果回傳==
            List<TixElementSeats> lstResult = new List<TixElementSeats>();
            if (SubscrEr.Driver.Url.Contains("area"))
            {
                while (lstResult.Count == 0)
                {
                    #region == 瘋狂刷新直到有JavaScript加載完畢 ==

                    // 搜尋 JS 上的腳本 ( 查座位對應的URL在哪 )
                    string strScript = "";
                    List<IWebElement> lstScripts_Seats = new List<IWebElement>();
                    ReadOnlyCollection<IWebElement> lstScripts = SubscrEr.Driver.FindElements(By.TagName("script"));
                    foreach (IWebElement p in lstScripts)
                    {
                        if (p.GetAttribute("innerHTML").Contains("var areaUrlList"))
                        {
                            lstScripts_Seats.Add(p);
                        }
                    }
                    if (lstScripts_Seats.Count > 0)
                    {
                        //取得網頁Javascript原始碼 ==> 有座位資訊要做字串分割
                        strScript = lstScripts_Seats[0].GetAttribute("innerHTML");
                    }
                    if (strScript != "")
                    {
                        //切割座位資訊
                        int iStartFind = strScript.IndexOf("var areaUrlList");
                        int iSplitStart = strScript.IndexOf("{", iStartFind);
                        int iSplitEnd = strScript.IndexOf("}", iStartFind);
                        string strSeatURLs = strScript.Substring(iSplitStart + 1, iSplitEnd - iSplitStart - 1);
                        strSeatURLs = strSeatURLs.Replace("\"", "");
                        string[] listSeats = strSeatURLs.Split(',');
                        foreach (string seat in listSeats)
                        {
                            string[] sp = seat.Split(':');
                            TixElementSeats one_seat = new TixElementSeats();
                            one_seat.SeatID = sp[0];
                            one_seat.SeatURL = "https://tixcraft.com" + sp[1].Replace("\\", "");
                            lstResult.Add(one_seat);
                        }
                    }
                    if (lstResult.Count > 0)
                    {
                        //如果有切割出座位，那麼就去尋找該座位的名稱
                        for (int i = 0; i < lstResult.Count; i++)
                        {
                            IWebElement page_element = SubscrEr.Driver.FindElement(By.Id(lstResult[i].SeatID));
                            lstResult[i].SeatName = page_element.Text;
                        }
                    }
                    #endregion
                }
            }
            return lstResult;
        }
        public void Tix_SelectSeat(List<TixElementSeats> AllSeat, string KeyWord, bool IsRandom, ref long cost_time_ms)
        {
            Stopwatch sw_cost_time = new Stopwatch();
            sw_cost_time.Restart();
            int iMaxSeats = AllSeat.Count;
            if (iMaxSeats <= 0) return;
            int Index = -1; //座位選擇
            if (IsRandom)
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
                    for (int iS = 0; iS < AllSeat.Count; iS++)
                    {
                        if (AllSeat[iS].SeatName.Contains(KeyWord))
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
            SubscrEr.GoTo_normal(AllSeat[Index].SeatURL);
            sw_cost_time.Stop();
            cost_time_ms = sw_cost_time.ElapsedMilliseconds;
        }

        //選擇張數 + 打勾
        public string Tix_PreSubmit(int TicketCount, ref long cost_time_ms)
        {
            Stopwatch sw_cost_time = new Stopwatch();
            sw_cost_time.Restart();
            string strMsg = "";

            try
            {
                #region 同意條款
                IWebElement Agree = SubscrEr.Driver.FindElement(By.Id("TicketForm_agree"));
                if (Agree.Selected == false)
                {
                    Agree.Click();

                    #region 選擇張數
                    IWebElement element = SubscrEr.Driver.FindElement(By.TagName("Select"));
                    //ReadOnlyCollection<IWebElement> AllDropDownList = element.FindElements(By.XPath("//option"));
                    ReadOnlyCollection<IWebElement> AllDropDownList = element.FindElements(By.TagName("option"));
                    int DpListCount = AllDropDownList.Count;
                    bool bIsFindCount = false;
                    for (int i = 0; i < DpListCount; i++)
                    {
                        if (AllDropDownList[i].Text == TicketCount.ToString())
                        {
                            bIsFindCount = true;
                            AllDropDownList[i].Click();
                            break;
                        }
                    }
                    if (bIsFindCount == false)
                    {

                        for (int i = DpListCount - 1; i >= 0; i--)
                        {
                            int iOptionTicket = 0; int.TryParse(AllDropDownList[i].Text, out iOptionTicket);
                            if (iOptionTicket > 0)
                            {
                                bIsFindCount = true;
                                AllDropDownList[i].Click();
                                break;
                            }
                        }
                        if (AllDropDownList.Count > 0)
                        {
                            AllDropDownList[AllDropDownList.Count - 1].Click(); //沒有找到的話就點選最後一個張數
                        }
                    }
                    #endregion

                }
                #endregion

                strMsg = "同意挑款 & 選擇張數 已填單完畢";
            }
            catch (Exception ex)
            {
                strMsg = "找不到選擇張數！";
            }
            sw_cost_time.Stop();
            cost_time_ms = sw_cost_time.ElapsedMilliseconds;
            return strMsg;
        }

        //自動回答驗證碼答案
        public void Tix_KeyInVeryfiCode()
        {
            int iDpwnLoadCount = 0;
            List<Bitmap> lstbitmap = new List<Bitmap>();
            while (lstbitmap.Count != 4)
            {
                try
                {
                    if (iDpwnLoadCount > 0)
                    {
                        try
                        {
                            IAlert ala = SubscrEr.Driver.SwitchTo().Alert();
                            ala.Dismiss();
                        }
                        catch (Exception ex)
                        {

                            //VPState.Report(ex, MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
                        }
                        IWebElement iImage = SubscrEr.Driver.FindElement(By.Id("yw0"));
                        iImage.Click();
                        Thread.Sleep(350);
                    }
                    SubscrEr.ScreenShot();
                    Bitmap myBrowserScreen = new Bitmap(SubscrEr.bSnapShot);
                    Bitmap VerifyCodeImage = new Bitmap(myBrowserScreen);
                    HImagTool myTool = new HImagTool();
                    int iRoi_x1 = 0;
                    int iRoi_y1 = 0;
                    int iRoi_x2 = 0;
                    int iRoi_y2 = 0;
                    lstbitmap = myTool.SplitVerifyFromScreenShot(VerifyCodeImage, ref iRoi_x1, ref iRoi_y1, ref iRoi_x2, ref iRoi_y2);
                    //lstbitmap[0].Save("C:\\bbb.bmp");


                    if (lstbitmap.Count > 0)
                    {
                        lstbitmap = HalconProcess.Vision.SplitVerifyCode(lstbitmap[0]); //驗證碼取出四個字
                    }
                    iDpwnLoadCount++;
                    if (iDpwnLoadCount > 10) break;
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
                    strResult = "";
                    strResult += this.myOCRServer.GetChar(lstbitmap[0]);
                    strResult += this.myOCRServer.GetChar(lstbitmap[1]);
                    strResult += this.myOCRServer.GetChar(lstbitmap[2]);
                    strResult += this.myOCRServer.GetChar(lstbitmap[3]);
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
            SetVerifyCode(strResult);
            CheckAlert_Delay2S();
        }

        private void SetVerifyCode(string strVerifyCode)
        {
            //if (Page.OrderQuantity == GetNowPage())
            //{
            //IWebElement TicketForm_GG = this.SubscrEr.Driver.FindElement(By.Id("TicketForm"));
            //ReadOnlyCollection<IWebElement> hrefs = TicketForm_GG.FindElements(By.TagName("input"));
            //IWebElement target = null;
            //foreach (IWebElement p in hrefs)
            //{
            //    string DHTMLQQ = p.GetAttribute("outerHTML");
            //    if (DHTMLQQ.Contains("請輸入驗證碼")) target = p;
            //}  
            //target.SendKeys(strVerifyCode); 


            string strSubmitFunction = SubscrEr.JS_ReadFile("Tix_SubmitVeryfiCode.txt");
            strSubmitFunction += string.Format("\nTix_KeyInVerifyCode(\"{0}\");", strVerifyCode);
            SubscrEr.InjectJavaScript(strSubmitFunction);


            string strSubmitClick = SubscrEr.JS_ReadFile("Tix_Click_Submit.txt");
            strSubmitClick += string.Format("\nCallBackToDetectVeryfiCode();", strVerifyCode);
            SubscrEr.InjectJavaScript(strSubmitClick);

        }


        //自動回答考試題目== 下載999999次不同答案輸入
        public void Tix_KeyInTestAnswer()
        {
            try
            {
                //==確認有找到元素==
                ReadOnlyCollection<IWebElement> TD_CheckCode = SubscrEr.Driver.FindElements(By.Id("checkCode"));
                ReadOnlyCollection<IWebElement> TD_SubmitButton = SubscrEr.Driver.FindElements(By.Id("submitButton")); 
                if (TD_CheckCode == null) return;
                if (TD_SubmitButton == null) return;
                if (TD_CheckCode.Count == 0) return;
                if (TD_SubmitButton.Count == 0) return;

                
                string tempstrMyAnswer = "";
                // --> 自動回答防黃牛問題 回答999999次，直到頁面切換
                for (int iReAnswerIdx = 0; iReAnswerIdx < 999999; )
                {
                    //下載一筆新的答案
                    string strMyAnswer = myAnswerServer.DownLoadTestAnswerFromSQL();  
                    if ((strMyAnswer != tempstrMyAnswer) && (strMyAnswer != ""))//如果抓下來的答案跟上一個不一樣(再重新提交答案)
                    {
                        iReAnswerIdx++;
                        SendCheckCode(strMyAnswer);                 //透過流覽器提交答案
                        CheckAlert_Delay2S();
                        tempstrMyAnswer = strMyAnswer;//紀錄上一個答案
                    }

                    try
                    {
                        string strCurrentURL = SubscrEr.Driver.Url;
                        if (strCurrentURL.Contains("area")) { break; }
                        if (strCurrentURL.Contains("ticket/ticket")) { break; }
                    }
                    catch (Exception)
                    {
                        CheckAlert(); 
                    }
                }

            }
            catch (Exception)
            {
                CheckAlert();
            }
        }
        public string SendCheckCode(string strCheckCode)
        { 
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

                    VPState.Report(ex, MethodBase.GetCurrentMethod(), VPState.eVPType.Windows);
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

        private void ck_Run_CheckedChanged(object sender, EventArgs e)
        {
            g_bIsRunAuto = ck_Run.Checked;
        }
    }
}
