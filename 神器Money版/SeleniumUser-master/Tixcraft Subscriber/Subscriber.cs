using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.PhantomJS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using OpenQA.Selenium.Remote;
using TSubscriber;
using TixWin;

namespace Tixcraft_Subscriber
{
    public enum Page
    {
        Section, //選擇區域 https://tixcraft.com/ticket/area/17_rene/2911
        OrderQuantity, //選擇張數  https://tixcraft.com/ticket/ticket/17_rene/2911/7/28
        Infomation, //節目訊息 https://tixcraft.com/activity
        Login,      //會員登入 https://tixcraft.com/login
        OrderBlueCircle, //藍圈圈 https://tixcraft.com/ticket/order
        OrderSuccessful , //訂購成功 https://tixcraft.com/ticket/payment
        none
    }


    public enum LoginType
    {
        FB,
        Google,
        PixelPin
    }
    public class Subscriber
    {
        public IWebDriver Driver { get; set; }
        public Bitmap bSnapShot = new Bitmap(10, 10);
        public WindowController HwndController = new WindowController();
        public List<TixcraftSubscriber.Activity> allactivity = new List<TixcraftSubscriber.Activity>();   //記錄所有節目資訊
        public List<TixcraftSubscriber.Activity> buyNowActivity = new List<TixcraftSubscriber.Activity>();   //記錄兩顆按鈕(立即訂購)

        public void GoToShowActivity(int index)
        {
            if (allactivity.Count > 0 && index < allactivity.Count)
            {
                GoTo(allactivity[index].url);
            }
        }
        public void GoToBuyNowActivity(int index)
        {
            if (buyNowActivity.Count > 0 && index < buyNowActivity.Count)
            {
                GoTo(buyNowActivity[index].url);
            }
        }
        public void GoTo(string strURL)
        {
            //Driver.Navigate().GoToUrl(strURL);
            string strScript = string.Format("window.location = \"{0}\";", strURL);
            InjectJavaScript(strScript);
        }

        public void GoTo_normal(string strURL)
        {
            Driver.Navigate().GoToUrl(strURL);

        }

        /// <summary>
        /// 注入JavaScript
        /// </summary>
        /// <param name="strScript"></param>
        /// <returns></returns>
        public string InjectJavaScript(string strScript)
        {
            string strResultMessage = "";
            try
            {
                IJavaScriptExecutor js = (IJavaScriptExecutor)Driver;
                strResultMessage = (string)js.ExecuteScript(strScript);
            }
            catch (Exception ex)
            {
                strResultMessage = "Inject Fail";
            }
            return strResultMessage;
        }
        public string JS_Get_Href()
        {
            return InjectJavaScript("return document.location.href");
        }
        public string JS_ReadFile(string strPath)
        {
            string strFile = "";
            try
            { // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(strPath, Encoding.Default))     //小寫TXT
                {
                    String line;
                    // Read and display lines from the file until the end of 
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        strFile += line + "\n";
                    }
                }
            }
            catch (Exception ex)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(ex.Message);
            }
            return strFile;
        }
        public string JS_Setting_Seats(string ScriptSource, string vFlagDays, string vFlagSeatText, string vFlagTicketCount, string vFlagPayType)
        {
            string strFix = ScriptSource;

            string strFindDay = "var vFlagDays = 1;";
            string strFindSeat = "var vFlagSeatText = \"1000\";";
            string strFindTicketCount = "var vFlagTicketCount = 1;";
            string strFindPayType = "var vFlagPayType = -1;";

            strFix = strFix.Replace(strFix.Substring(strFix.IndexOf(strFindDay), strFindDay.Length), "var vFlagDays = " + vFlagDays + ";");
            strFix = strFix.Replace(strFix.Substring(strFix.IndexOf(strFindSeat), strFindSeat.Length), "var vFlagSeatText = \"" + vFlagSeatText + "\";");
            strFix = strFix.Replace(strFix.Substring(strFix.IndexOf(strFindTicketCount), strFindTicketCount.Length), "var vFlagTicketCount = " + vFlagTicketCount + ";");
            strFix = strFix.Replace(strFix.Substring(strFix.IndexOf(strFindPayType), strFindPayType.Length), "var vFlagPayType = " + vFlagPayType + ";");


            return strFix;
        }



        public void OpenBrowser(bool bIsOpenWithGoogleChrome, bool bIsMountProxy)
        {
            if (bIsOpenWithGoogleChrome)
            {
                ChromeOptions options = new ChromeOptions();

                if (bIsMountProxy)
                {
                    Proxy proxy = new Proxy();

                    proxy.HttpProxy = string.Format("proxy.hinet.net:80"); // Taiwan
                    //proxy.HttpProxy = string.Format("104.129.202.48:8080"); // US
                    options.AddAdditionalCapability("proxy", proxy);
                }

                options.AddAdditionalCapability("profile.default_content_settings", 2);
                options.AddAdditionalCapability("pageLoadStrategy", "none");
                options.AddAdditionalCapability("webdriver.load.strategy", "unstable");
                Driver = new ChromeDriver(options);
                //== 取得視窗碼 ==
                HwndController.hwnd = WindowSnap.FindWindow(null, "data:, - Google Chrome");
            }
            else
            { 
                Driver = new PhantomJSDriver();
            }
        } 
        public void TearDown()
        {
            Driver.Quit();
        } 
        public Page GetNowPage(string strURL)
        {
            Page NowP = Page.none;
            string[] strSp = strURL.Split('/');
            if (strSp.Length > 0)
            {
                if (strSp.Length == 7)
                {
                    if (strSp[2] == "tixcraft.com" && strSp[3] == "ticket" && strSp[4] == "area") NowP = Page.Section;
                }
                if (strSp.Length == 9)
                {
                    if (strSp[2] == "tixcraft.com" && strSp[3] == "ticket" && strSp[4] == "ticket") NowP = Page.OrderQuantity;
                }
                if (strSp.Length == 4)
                {
                    if (strSp[2] == "tixcraft.com" && strSp[3] == "activity") NowP = Page.Infomation;
                    if (strSp[2] == "tixcraft.com" && strSp[3] == "login") NowP = Page.Login;
                }
                if (strSp.Length == 5)
                {
                    if (strSp[2] == "tixcraft.com" && strSp[3] == "ticket" && strSp[4] == "order") NowP = Page.OrderBlueCircle;
                    if (strSp[2] == "tixcraft.com" && strSp[3] == "ticket" && strSp[4] == "payment") NowP = Page.OrderSuccessful;
                }
            }
            return NowP;
        }
        public Page GetNowPage()
        {
            Page NowP = Page.none;
            string[] strSp = Driver.Url.Split('/');
            NowP = GetNowPage(Driver.Url); 
            return NowP;

        }
        public string GetCodeImageURL(string outerHTML)
        {
            //格式 : "<img style=\"margin-right: 20px;\" id=\"yw0\" src=\"/ticket/captcha?v=594b35903776d\" alt=\"\">
            string strSourceImageURL = "";
            string[] sp = outerHTML.Split(' ');
            foreach (string node in sp)
            {
                string[] strHead = node.Split('=');
                if (strHead.Length > 1)
                {
                    if (strHead[0] == "src")
                    {
                        string strEnd = strHead[1];
                        string[] strURL = strEnd.Split('"');
                        string[] strMagicNum = strHead[2].Split('"');

                        strSourceImageURL = "https://tixcraft.com" + strURL[1] + "=" + strMagicNum[0];
                    }
                }
            } 
            return strSourceImageURL; 
        }

        public string PreSubmit(int TicketCount)
        {
            string strMsg = "";

            if (false)
            {
                #region == Method 1 ==

                try
                {
                    //點擊同意
                    IWebElement Agree = Driver.FindElement(By.Id("TicketForm_agree"));

                    if (Agree.Selected == false)
                    {
                        int iClick_x = Agree.Location.X + 10;
                        int iClick_y = Agree.Location.Y + 10;

                        this.HwndController.Mouse_LeftButton_Down_ByOffset(iClick_x, iClick_y);
                        Thread.Sleep(10);
                        this.HwndController.Mouse_LeftButton_Up_ByOffset(iClick_x, iClick_y);
                        //Agree.Click();
                    }
                    //點張數 (消除驗證事件)
                    IWebElement element = Driver.FindElement(By.TagName("Select")); 
                    int ix_Start = element.Location.X + 10;
                    int iy_Start = element.Location.Y + 10;
                    //點起始點 ( 下拉選單 ) 點兩下 --> 有點跟沒點一樣
                    this.HwndController.Mouse_LeftButton_Down_ByOffset(ix_Start, iy_Start);
                    Thread.Sleep(3);
                    this.HwndController.Mouse_LeftButton_Up_ByOffset(ix_Start, iy_Start);
                    Thread.Sleep(3);
                    this.HwndController.Mouse_LeftButton_Down_ByOffset(ix_Start, iy_Start);
                    Thread.Sleep(3);
                    this.HwndController.Mouse_LeftButton_Up_ByOffset(ix_Start, iy_Start);
                    Thread.Sleep(3);
                    //注入
                    string strScrpit = JS_ReadFile("Tix_PreSubmit.txt");
                    strScrpit += string.Format("\nPreSubmit({0});\n", TicketCount);
                    InjectJavaScript(strScrpit);
                      
                    #region 同意條款
                    //IWebElement Agree = Driver.FindElement(By.Id("TicketForm_agree"));
                    //if (Agree.Selected == false)
                    //{ 
                    //    Agree.Click();
                    //}
                    #endregion

                    #region 選擇張數
                    //IWebElement element = Driver.FindElement(By.TagName("Select"));
                    ////ReadOnlyCollection<IWebElement> AllDropDownList = element.FindElements(By.XPath("//option"));
                    //ReadOnlyCollection<IWebElement> AllDropDownList = element.FindElements(By.TagName("option"));
                    //int DpListCount = AllDropDownList.Count;
                    //bool bIsFindCount = false;
                    //for (int i = 0; i < DpListCount; i++)
                    //{
                    //    if (AllDropDownList[i].Text == TicketCount.ToString())
                    //    {
                    //        bIsFindCount = true;
                    //        AllDropDownList[i].Click();
                    //        break;
                    //    }
                    //}

                    //if (bIsFindCount == false)
                    //{

                    //    for (int i = DpListCount - 1; i >= 0; i--)
                    //    {
                    //        int iOptionTicket = 0; int.TryParse(AllDropDownList[i].Text, out iOptionTicket);
                    //        if (iOptionTicket > 0)
                    //        {
                    //            bIsFindCount = true;
                    //            AllDropDownList[i].Click();
                    //            break;
                    //        }
                    //    }
                    //    if (AllDropDownList.Count > 0)
                    //    {
                    //        AllDropDownList[AllDropDownList.Count - 1].Click(); //沒有找到的話就點選最後一個張數
                    //    }
                    //}
                    #endregion
                    strMsg = "同意挑款 & 選擇張數 已填單完畢";
                }
                catch (Exception ex)
                {
                    strMsg = "找不到選擇張數！";
                }
                return strMsg;
                #endregion
            }
            else 
            { 
                //20180511 - 修正拓元偷偷修改網頁元素出去Submit的問題～
                string strError_Fix = JS_ReadFile("Tix_FixError.txt");
                InjectJavaScript(strError_Fix); 
                 
                string strScrpit = JS_ReadFile("Tix_PreSubmit.txt");
                strScrpit += string.Format("\nPreSubmit({0});\n", TicketCount);
                InjectJavaScript(strScrpit);
                strMsg = "同意挑款 & 選擇張數 已填單完畢";
            }
            return strMsg;

        }
        public void Submit(string strVerifyCode )
        {
            // 驗證碼輸入欄位 : id="TicketForm_verifyCode"
            // 同意條款 id="TicketForm_agree"
            // 認證碼 : id="yw0"
            // 確認張數 :  id="ticketPriceSubmit" name="ticketPriceSubmit"
            // 選擇張數 : <select name="TicketForm[ticketPrice][02]" id="TicketForm_ticketPrice_02">
            //< option value = "0" > 0 </ option >
            //< option value = "1" > 1 </ option >
            //< option value = "2" > 2 </ option >
            //< option value = "3" > 3 </ option >
            //< option value = "4" > 4 </ option >
            //</ select >strHomePage 
            // 同意閱讀條款 : id="TicketForm_agree" 
            if (Driver != null)
            {
                //Task trdAgree = Task.Factory.StartNew(() =>
                //{
                //    #region 同意條款 
                //    IWebElement Agree = Driver.FindElement(By.Id("TicketForm_agree"));
                //    Agree.Click();
                //    #endregion 
                //});

                //Task trdSelect = Task.Factory.StartNew(() =>
                //{
                //    #region 選擇張數
                //    IWebElement element = Driver.FindElement(By.TagName("Select"));
                //    //ReadOnlyCollection<IWebElement> AllDropDownList = element.FindElements(By.XPath("//option"));
                //    ReadOnlyCollection<IWebElement> AllDropDownList = element.FindElements(By.TagName("option"));
                //    int DpListCount = AllDropDownList.Count;
                //    for (int i = 0; i < DpListCount; i++)
                //    {
                //        if (AllDropDownList[i].Text == TicketCount.ToString())
                //        {
                //            AllDropDownList[i].Click();
                //            break;
                //        }
                //    }
                //    #endregion
                //});

                //Task trdCode = Task.Factory.StartNew(() =>
                //{
                    #region 輸入驗證碼 
                    SetVerifyCode(strVerifyCode); //mark-1
                    //ScreenShot();
                    #endregion
                //});
                //Task[] alltask = { trdAgree, trdSelect, trdCode };
                //Task.WaitAll(alltask);

                //確認送出
                #region 確認張數 
                //IWebElement Submit = Driver.FindElement(By.Id("ticketPriceSubmit"));
                //Submit.Click();
                #endregion


            }
        }
         

        public void SubmitAndPreSubtmi(string strVerifyCode, int iTicketCount )
        {
            if (Driver != null)
            {
                SetVerifyCodeAndPreSubmit(strVerifyCode, iTicketCount); 
            }
        }



        public void ScreenShot()
        { 
            //ITakesScreenshot iScreen = (ITakesScreenshot)Driver;
            //Byte[] pSourceImage = iScreen.GetScreenshot().AsByteArray;
            //Bitmap pPage = new System.Drawing.Bitmap(new System.IO.MemoryStream(pSourceImage));
            //bSnapShot = pPage;


            //this.HwndController.GetScreenShotBy_WindowHwnd();

            //Bitmap pPage = this.HwndController.GetScreenShotBy_WindowHwnd();

            bSnapShot = this.HwndController.GetScreenShotBy_WindowHwnd();

        }
        public Bitmap GetVerifyCode()
        {
            Bitmap bCode = null; 
            //if (Page.OrderQuantity == GetNowPage())
            {
                IWebElement WebTicketForm = Driver.FindElement(By.Id("TicketForm")); 
                IWebElement SrcVerify = WebTicketForm.FindElement(By.TagName("img")); 

                Point pCode = SrcVerify.Location;
                ITakesScreenshot iScreen = (ITakesScreenshot)Driver;
                Byte[] pSourceImage = iScreen.GetScreenshot().AsByteArray;
                Bitmap pPage = new System.Drawing.Bitmap(new System.IO.MemoryStream(pSourceImage));
                Rectangle ROI = new Rectangle(pCode.X, pCode.Y, 150, 120);
                bCode = ImageTool.GetBlock(pPage, ROI);
            } 
            return bCode;
        }

        private void SetVerifyCode(string strVerifyCode)
        {
            //if (Page.OrderQuantity == GetNowPage())
            //{
            //    IWebElement TicketForm_GG = Driver.FindElement(By.Id("TicketForm"));
            //    ReadOnlyCollection<IWebElement> hrefs = TicketForm_GG.FindElements(By.TagName("input"));
            //    IWebElement target = null;
            //    foreach (IWebElement p in hrefs)
            //    {
            //        string DHTMLQQ = p.GetAttribute("outerHTML");
            //        if (DHTMLQQ.Contains("請輸入驗證碼")) target = p;
            //    }

            //    ////target.Clear();
            //    target.SendKeys(strVerifyCode);

            //    //IWebElement InputText = Driver.FindElement(By.Id("TicketForm_verifyCode"));
            //    //InputText.Clear();
            //    //InputText.SendKeys(strVerifyCode);
            //}

            //ReadOnlyCollection<IWebElement> hrefs = Driver.FindElements(By.TagName("input"));
            //IWebElement target = null;
            //foreach (IWebElement p in hrefs)
            //{
            //    string DHTMLQQ = p.GetAttribute("outerHTML");
            //    if (DHTMLQQ.Contains("請輸入驗證碼"))
            //    {
            //        target = p;
            //        break;
            //    }
            //}

            //target.SendKeys(strVerifyCode);


            //string strSubmitFunction = JS_ReadFile("Tix_SubmitVeryfiCode.txt");
            //strSubmitFunction += string.Format("\nTix_KeyInVerifyCode(\"{0}\");", strVerifyCode);
            //InjectJavaScript(strSubmitFunction);

            //IWebElement iSubmit = this.Driver.FindElement(By.Id("ticketPriceSubmit"));
            //if(iSubmit != null)
            //{
            //    //iSubmit.Click();

            //    int iX = iSubmit.Location.X + 10;
            //    int iY = iSubmit.Location.Y + 10;

            //    this.HwndController.Mouse_LeftButton_Down_ByOffset(iX , iY);
            //    Thread.Sleep(10);
            //    this.HwndController.Mouse_LeftButton_Up_ByOffset(iX, iY);
            //    Thread.Sleep(10);
            //}


            //string strSubmitClick = JS_ReadFile("Tix_Click_Submit.txt");
            //strSubmitClick += string.Format("\nCallBackToDetectVeryfiCode();", strVerifyCode);
            //InjectJavaScript(strSubmitClick);
            

            //===========================
            string strRunScript = "";

            string strSubmitClick = JS_ReadFile("Tix_Click_Submit.txt");
            strRunScript += strSubmitClick + "\n";
            string strSubmitFunction = JS_ReadFile("Tix_SubmitVeryfiCode.txt");
            strRunScript += strSubmitFunction + "\n";

            strRunScript += string.Format("\nTix_KeyInVerifyCode(\"{0}\");", strVerifyCode) + "\n";
            strRunScript += string.Format("\nCallBackToDetectVeryfiCode();", strVerifyCode) + "\n";
            InjectJavaScript(strRunScript);

        } 
        /// <summary>
        /// 付款方式
        /// </summary>
        /// <param name="iMode">-1=不執行  0 => ATM付款   1 = ibon付款  2 = 信用卡</param>
        public void PayModeClick(int iMode = -1)
        {
            // 0 => ATM付款   1 = ibon付款  2 = 信用卡
            string strRunScript = JS_ReadFile("Tix_PayMode.txt");
            strRunScript = "\n" + strRunScript + "\n";
            strRunScript += string.Format("\nAutoSelect_PayType({0})", iMode) + "\n"; //付款方式
            InjectJavaScript(strRunScript);
        }
        private void SetVerifyCodeAndPreSubmit(string strVerifyCode, int TicketCount)
        {
            Stopwatch swTest = new Stopwatch();
            swTest.Restart();
            string strInjectScriptCode = "";

            //20180511 - 修正拓元偷偷修改網頁元素出去Submit的問題～
            string strError_Fix = JS_ReadFile("Tix_FixError.txt");
            //InjectJavaScript(strError_Fix);
            strInjectScriptCode += "\n" + strError_Fix + "\n";

            string strScrpit = JS_ReadFile("Tix_PreSubmit.txt");
            strScrpit += string.Format("\nPreSubmit({0});\n", TicketCount);
            strInjectScriptCode += "\n" + strScrpit + "\n";
            //InjectJavaScript(strScrpit);


            //===========================
            string strRunScript = "";

            string strSubmitClick = JS_ReadFile("Tix_Click_Submit.txt");
            strRunScript += strSubmitClick + "\n";
            string strSubmitFunction = JS_ReadFile("Tix_SubmitVeryfiCode.txt");
            strRunScript += strSubmitFunction + "\n";

            strRunScript += string.Format("\nTix_KeyInVerifyCode(\"{0}\");", strVerifyCode) + "\n";
            strRunScript += string.Format("\nCallBackToDetectVeryfiCode();", strVerifyCode) + "\n";
            //InjectJavaScript(strRunScript);

            strInjectScriptCode += "\n" + strRunScript + "\n";
            swTest.Stop();
            InjectJavaScript(strInjectScriptCode);

        }
        public string Login(LoginType LType, string strAddress, string strPassword)
        {
            try
            {

                if (Driver != null)
                {
                    //進入會員登入介面
                    Driver.Navigate().GoToUrl("https://tixcraft.com/login");
                    if (Page.Login != GetNowPage()) return "頁面錯誤，不是登入頁面無法登入";
                    //搜尋登入元素 
                    IWebElement elemes = Driver.FindElement(By.ClassName("login-area"));
                    ReadOnlyCollection<IWebElement> hrefs = elemes.FindElements(By.TagName("a"));   // 0 = FB , 1 = Google 
                    switch (LType)
                    {
                        case LoginType.FB:
                            hrefs[0].Click();
                            IWebElement ele_email = Driver.FindElement(By.Id("email"));
                            IWebElement ele_password = Driver.FindElement(By.Id("pass"));
                            IWebElement ele_LoginButton = Driver.FindElement(By.Id("loginbutton"));
                            ele_email.Clear();
                            ele_email.SendKeys(strAddress);
                            ele_password.Clear();
                            ele_password.SendKeys(strPassword);
                            ele_LoginButton.Click();
                            IWebElement userName = Driver.FindElement(By.Id("logout")); 
                            string DHTMLName = userName.GetAttribute("outerHTML");
                            //if (DHTMLQQ.Contains("請輸入驗證碼")) target = p;
                            string strName = DHTMLName.Split('B')[1]; 
                            return strName;

                            break;
                            //case LoginType.Google:   
                            //    break;
                    }
                }
                return "none name";
            }
            catch (Exception ex)
            {
                return "none name";
            }
        }

        public void AutoFill_CridetCard(string strAccount, string strMonth , string strYear , string strCardCheckNum)
        {
            Stopwatch swTest = new Stopwatch();
            swTest.Restart(); 


            string strRunScript = JS_ReadFile("Tix_CreditCard.txt");

            strRunScript = "\n" + strRunScript + "\n";

            strRunScript += string.Format("\nVisaCardNumber(\'{0}\');", strAccount) + "\n"; //信用卡帳號
            strRunScript += string.Format("\nVisacheck_num(\'{0}\');", strCardCheckNum) + "\n"; //信用卡三碼
            strRunScript += string.Format("\nVisa_Date(\'{0}\' , \'{1}\' );", strMonth, strYear) + "\n"; //信用卡到期日

            swTest.Stop();
            InjectJavaScript(strRunScript);

        }



        public void WaittingForBuyNow(string strSingShowNumber , int CheckinIndex)
        {
            CheckinIndex = CheckinIndex - 1;
            string strGameURL = "https://tixcraft.com/activity/game/" + strSingShowNumber;
            Driver.Navigate().GoToUrl(strGameURL);
            bool bIsClickBuy = false;
            //while (bIsClickBuy == false)
            {
                IWebElement glmeListes = Driver.FindElement(By.Id("gameList"));
                //List<string> lstGameList = GetGameList(glmeListes); 
                ReadOnlyCollection<IWebElement> hrefs = glmeListes.FindElements(By.TagName("input"));
                string DHTMLQQ = hrefs[CheckinIndex].GetAttribute("outerHTML");
                hrefs[0].Click();
            }


            //選座位
            ReadOnlyCollection<IWebElement> Seats = Driver.FindElements(By.ClassName("select_form_b"));
            IWebElement obj = Seats[0].FindElement(By.TagName("a"));
            obj.Click();
            //Seats[0].Click(); //Method 1
            //IJavaScriptExecutor js = (IJavaScriptExecutor)Driver;   //Method 2
            //js.ExecuteScript("arguments[0].click();", Seats[0]);    //Method 2
        }
         
        /// <summary>
        /// 找到座位 並點選
        /// </summary>
        /// 
        public void FindSeat(string strFilter)
        {
            if (strFilter != "")
            {
                //選座位
                ReadOnlyCollection<IWebElement> Seats = Driver.FindElements(By.ClassName("select_form_b")); 
                IWebElement obj = null;
                Parallel.For(0, Seats.Count() , (i) =>
                { 
                    string DHTMLQQ = Seats[i].GetAttribute("outerHTML");
                    if (DHTMLQQ.Contains(strFilter))
                    {
                        obj = Seats[i].FindElement(By.TagName("a")); 
                    }
                });
                if (obj == null && Seats.Count > 0)
                {
                    obj = Seats[Seats.Count-1].FindElement(By.TagName("a"));
                }
                if(Seats.Count != 0)
                { 
                    obj.Click();
                }
            }
            else
            { 
                //選座位
                ReadOnlyCollection<IWebElement> Seats = Driver.FindElements(By.ClassName("select_form_b"));
                IWebElement obj = Seats[0].FindElement(By.TagName("a"));
                obj.Click();
            }


            //IWebElement element = Driver.FindElement(By.ClassName("area_select"));
            ////尋找所有位置區域
            //ReadOnlyCollection<IWebElement> allseat = element.FindElements(By.TagName("li"));
            ////選擇第一個未售完區域
            //for (int i = 0; i < allseat.Count; i++)
            //{ 
            //    string name = allseat[i].Text;
            //    if (name.IndexOf("已售完") == -1)
            //    {
            //        allseat[i].Click();
            //        break;
            //    }
            //}
        }
        /// <summary>
        /// 找到立即購票按鈕
        /// </summary>
        /// <returns></returns>
        public bool FindBuyButton()
        {
             
            try
            {
                IWebElement element = Driver.FindElement(By.ClassName("btn"));
                ReadOnlyCollection<IWebElement> allbtn = element.FindElements(By.TagName("a"));
                return true;
            }
            catch
            {
               
                return false;
            }
            
        }
        private List<string> GetGameList(IWebElement glmeListes)
        {
            string strDHTML_fromGameList = glmeListes.Text;
            //string strDHTML_fromGameList = glmeListes.GetAttribute("outerHTML");
            List<string> lstGameList = new List<string>();
            string[] sp = strDHTML_fromGameList.Split('\n');
            for (int i = 1; i < sp.Length; i++)
            {
                lstGameList.Add(sp[i]);
            }
            return lstGameList;
        }
    }

    
    public class ActivityDate
    {
        public string Date;
        public string Url;
        public bool IsSaleOut = false;
    }
    public class SeatInformation
    {
        public string elemID = "";
        public string url = "";
        public string Text = "";
    }
}
