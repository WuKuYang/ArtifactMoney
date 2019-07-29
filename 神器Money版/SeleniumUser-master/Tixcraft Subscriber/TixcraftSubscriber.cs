using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Net;
using Tixcraft_Subscriber;

namespace TSubscriber
{
    public class TixcraftSubscriber
    {

        public FastHttpWebDriver TixcraftWebDriver = new FastHttpWebDriver();

        private List<Activity> AllActivity = new List<Activity>();

        public const string TixcraftURL = "https://tixcraft.com";///activity";

        public TixcraftSubscriber()
        {
        }
        public bool FacebookLogin(CookieContainer LoginCookie)
        {
            bool IsLoginSuccess = false;
            TixcraftWebDriver.Session = LoginCookie;
            List<SWebElement> Logout = null;
            DateTime time_start = DateTime.Now;//計時開始 取得目前時間
            do
            {
                DateTime time_end = DateTime.Now;//計時結束 取得目前時間
                if ((time_end - time_start).TotalSeconds > 12)
                {
                    break;
                }
                TixcraftWebDriver.GetWebSourceCode("https://tixcraft.com/login/facebook");
                Logout = HtmlAnalyze.FindElement(TixcraftWebDriver.strPageSourceCode, WebBy.ID("logout"));
            }
            while (Logout.Count == 0);
            TixcraftWebDriver.GetWebSourceCode(TixcraftURL);  
            IsLoginSuccess = (Logout.Count != 0);
            return IsLoginSuccess;
        }
        private List<Activity> GetAllActivity
        {
            get
            {
                return AllActivity = GetActivityFromPageSource(TixcraftWebDriver.GetWebSourceCode(TixcraftURL + "/activity"));
            }
        }
        
        private Activity ConvertStringToActivity(string data)
        {
            data = data.Replace("detail", "game");
            List<SWebElement> TD_Data = HtmlAnalyze.FindElement(data,WebBy.Tag("td"));
            if (TD_Data.Count == 3)
            {
                Activity tempActivity = new Activity();
                tempActivity.ActivityDate = TD_Data[0].ElementName;
                List<SWebElement> Link = HtmlAnalyze.FindElement(data,WebBy.Tag("a"));
                tempActivity.url = TixcraftURL + Link[0].URL;
                tempActivity.Name = Link[0].ElementName;
                tempActivity.Location = TD_Data[2].ElementName;
                tempActivity.TixcraftWebDriver = TixcraftWebDriver;
                return tempActivity;
            }
            else
            {
                return null;
            }
        }


        private Activity ConvertStringToActivity_2018(string data)
        {
            data = data.Replace("detail", "game");
            List<SWebElement> TD_Data = HtmlAnalyze.FindElement(data, WebBy.Tag("h5"));  
            if (TD_Data.Count == 1)
            {
                Activity tempActivity = new Activity();
                tempActivity.ActivityDate = TD_Data[0].ElementName;
                List<SWebElement> Link = HtmlAnalyze.FindElement(TD_Data[0].Context, WebBy.Tag("a"));
                tempActivity.url = TixcraftURL + Link[0].URL;
                tempActivity.Name = Link[0].ElementName;
                //tempActivity.Location = TD_Data[0].ELocation;
                tempActivity.TixcraftWebDriver = TixcraftWebDriver;
                return tempActivity;
            }
            else
            {
                return null;
            }
        }

        private List<Activity> GetActivityFromPageSource(string strPageSourceCode)
        {

            List<Activity> lstAllActivityInformation = new List<Activity>();
            //將網頁原始碼進行分析
            string strCode = strPageSourceCode;
            List<SWebElement> TRData = HtmlAnalyze.FindElement(strCode, WebBy.Tag("tr"));
            for (int i = 0; i < TRData.Count; i++)
            {
                Activity temp = ConvertStringToActivity(TRData[i].ElementName);

                if (temp != null)
                {
                    lstAllActivityInformation.Add(temp);
                }
            }
            return lstAllActivityInformation;


            //List<Activity> lstAllActivityInformation = new List<Activity>();
            ////將網頁原始碼進行分析
            //string strCode = strPageSourceCode;
            //List<SWebElement> TRData = HtmlAnalyze.FindElement(strCode, WebBy.Class("col-md-4 col-sm-6 col-xs-12"));
            //for (int i = 0; i < TRData.Count; i++)
            //{ 

            //    Activity temp = ConvertStringToActivity_2018(TRData[i].ElementName);

            //    if (temp != null)
            //    {
            //        lstAllActivityInformation.Add(temp);
            //    }
            //}
            //return lstAllActivityInformation;
        }
        
        public int ActivityCount
        {
            get {
                if (AllActivity == null)
                    return 0;
                else
                return AllActivity.Count;
            }
        }

        public Activity GetActivity(int Index)
        {
            if (Index < AllActivity.Count)
            {
                return AllActivity[Index];
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 更新資料
        /// </summary>
        public void RefreshActivity()
        {
            AllActivity = GetAllActivity;
        }
        //活動
        public class Activity
        {
            /// <summary>
            /// 活動日期
            /// </summary>
            public string ActivityDate;
            /// <summary>
            /// 活動名稱
            /// </summary>
            public string Name;
            /// <summary>
            /// 活動地點
            /// </summary>
            public string Location;
            /// <summary>
            /// 活動網址
            /// </summary>
            public string url;
            
            public FastHttpWebDriver TixcraftWebDriver = new FastHttpWebDriver();

           

            #region 底下連結

            /// <summary>
            /// 所有活動日期
            /// </summary>
            private List<ShowDate> AllDate = new List<ShowDate>();
            /// <summary>
            /// 取得所有活動日期(會更新AllDate)
            /// </summary>
            private List<ShowDate> GetDate
            {
                get
                {
                    return AllDate = GetActivityDateFromPageSource(TixcraftWebDriver.GetWebSourceCode(url));
                }
            }
            /// <summary>
            /// 更新資料
            /// </summary>
            public void RefreshDate()
            {
                AllDate = GetDate;
            }
            public int DateCount
            {
                get {
                    if (AllDate == null)
                        return 0;
                    else
                    return AllDate.Count;
                }
            }

            public ShowDate GetShowDate(int Index)
            {
                if (Index < AllDate.Count)
                {
                    return AllDate[Index];
                }
                else
                {
                    return null;
                }
            }
            #endregion
          
            public Activity()
            {
            }
            
            private List<ShowDate> GetActivityDateFromPageSource(string strPageSourceCode)
            {
                List<ShowDate> AllShowDate = new List<ShowDate>();
                string strCode = strPageSourceCode;
                List<SWebElement> TR_Data = HtmlAnalyze.FindElement(strCode, WebBy.Tag("tr"));

                for (int i = 0; i < TR_Data.Count; i++)
                {
                    List<SWebElement> TD_Data = HtmlAnalyze.FindElement(TR_Data[i].ElementName, WebBy.Tag("td"));
                    if (TD_Data.Count == 4)
                    {
                        #region 取得售票資訊
                        ShowDate singleShowDate = new ShowDate();
                        singleShowDate.TixcraftWebDriver = TixcraftWebDriver;
                        singleShowDate.Date = TD_Data[0].ElementName;//0 時間
                        singleShowDate.Name = TD_Data[1].ElementName;//1 名稱
                        singleShowDate.Location = TD_Data[2].ElementName; //2 地點
                        SWebElement Input = new SWebElement(TD_Data[3].ElementName);
                        singleShowDate.url = TixcraftURL + Input.URL;
                        
                        string TicketInfo = TD_Data[3].ElementName;
                        //倒數計時器
                        List<SWebElement> CountDownInfo = HtmlAnalyze.FindElement(TicketInfo,WebBy.Tag( "div"));
                        if (CountDownInfo.Count != 0)
                        {
                            //倒數計時資訊
                            singleShowDate.info = CountDownInfo[0].ElementName;
                        }
                        else
                        {
                            singleShowDate.info = Input.value; //3 售票狀態
                        }
                        #endregion

                        AllShowDate.Add(singleShowDate);
                    }
                }
                return AllShowDate;
            }
            
            public class ShowDate
            {
                /// <summary>
                /// 演出日期
                /// </summary>
                public string Date;
                /// <summary>
                /// 演出名稱
                /// </summary>
                public string Name;
                /// <summary>
                /// 演出地點
                /// </summary>
                public string Location;
                /// <summary>
                /// 售票狀態
                /// </summary>
                public string info;
                /// <summary>
                /// 演出網址
                /// </summary>
                public string url;

                public FastHttpWebDriver TixcraftWebDriver = new FastHttpWebDriver();
                #region 底下連結
                /// <summary>
                /// 所有活動座位
                /// </summary>
                private List<SeatTicket> AllSeat = new List<SeatTicket>();
                private List<SeatTicket> GetSeat 
                {
                    get 
                    {
                        return AllSeat = GetSeatsFromPageSource(TixcraftWebDriver.GetWebSourceCode(url));
                    }
                }
                private List<SeatTicket> GetSeatsFromPageSource(string strPageSourceCode)
                {

                    List<SeatTicket> lstAllSeatInformation = new List<SeatTicket>();
                    //將網頁原始碼進行分析
                    string strCode = strPageSourceCode;
                    int iTableArray = strCode.IndexOf("areaUrlList");   //javascript的程式碼的位置陣列
                    if (iTableArray == -1)
                    {
                        return null;
                    }
                    int iTableArrayLeft = strCode.IndexOf("{", iTableArray); //陣列頭
                    int iTableArrayRight = strCode.IndexOf("}", iTableArray); //陣列尾

                    iTableArrayLeft++;
                    iTableArrayRight--;

                    string strSeatsArray = strCode.Substring(iTableArrayLeft, iTableArrayRight - iTableArrayLeft + 1);
                    string[] strEachSeat = strSeatsArray.Split(',');
                    lstAllSeatInformation.Clear();
                    for (int i = 0; i < strEachSeat.Length; i++)
                    {
                        if (strEachSeat[i][0] != '\"')
                        {
                            return null;
                        }
                        string[] strSp = strEachSeat[i].Split('"');

                        SeatTicket oneSeat = new SeatTicket();
                        oneSeat.TixcraftWebDriver = TixcraftWebDriver;
                        string strSeatID = strSp[1];
                        string[] strSeatURL = strSp[3].Split(' ');
                        strSp[3] = strSp[3].Replace("\\/", "/");

                        oneSeat.elemID = strSeatID; //相對應的ID！
                        oneSeat.url = TixcraftURL+strSp[3];     //位置URL.

                        List<SWebElement> SeatInfo = HtmlAnalyze.FindElement(strCode, WebBy.ID(strSeatID));
                        string SeatTemp = SeatInfo[0].ElementName;
                        SeatInfo = HtmlAnalyze.FindElement(SeatTemp, WebBy.Tag("font"));

                        //由ID去網頁原始碼中搜尋相對應的位置資訊。 
                        int elemIDStart = strCode.IndexOf(strSeatID);
                        int ieleTextStart = strCode.IndexOf("</span>", elemIDStart);
                        ieleTextStart = ieleTextStart + 7;
                        int ieleTextEnd = strCode.IndexOf("<font", elemIDStart);
                        oneSeat.Text = strCode.Substring(ieleTextStart, ieleTextEnd - ieleTextStart) + " " + SeatInfo[0].ElementName;
                        //由ID去網頁原始碼中搜尋相對應的位置資訊。 

                        lstAllSeatInformation.Add(oneSeat);

                    }
                    return lstAllSeatInformation;

                }
                /// <summary>
                /// 更新資料
                /// </summary>
                public void RefreshAllSeat()
                {
                    AllSeat = GetSeat;
                }
                public int SeatCount
                {
                    get {
                        if (AllSeat == null)
                            return 0;
                        else
                        return AllSeat.Count;
                    }
                }

                public SeatTicket GetSeatTicket(int Index)
                {
                    if (Index < AllSeat.Count)
                    {
                        return AllSeat[Index];
                    }
                    else
                    {
                        return null;
                    }
                }
                #endregion 
            
                public class SeatTicket
                {
                    public string elemID = "";
                    
                    public string url = "";
                    
                    public string Text = "";
                    
                    public FastHttpWebDriver TixcraftWebDriver = new FastHttpWebDriver();
                    /// <summary>
                    /// 購買張數
                    /// </summary>
                    public int ticketPrice;
                    /// <summary>
                    /// 最大可購買張數
                    /// </summary>
                    public int MaxTicketPrice;
                    /// <summary>
                    /// 認證碼
                    /// </summary>
                    public string VerificationCode;
                    /// <summary>
                    /// 認證碼圖片
                    /// </summary>
                    public Image VerificationCodeImage;
                    
                    #region 提交用資訊
                    private string strCSRFTOKEN = "";
                    
                    private string strTicketPriceName = "";

                    private string strTicketAgree_MagicNumber = "";
                    #endregion

                    private string GetTicketAgree_MagicNumber(string strPageSource)
                    {

                        try
                        { 
                            int strLeftIndex = strPageSource.IndexOf("agree][");
                            string strTemp = strPageSource.Substring(strLeftIndex + 7);
                            int strRightIndex = strTemp.IndexOf("]");
                            string strResult = strTemp.Substring(0, strRightIndex);
                            return strResult;
                        }
                        catch (Exception)
                        {
                            return "null";
                        }

                        /* 
                         (this).attr("name", "TicketForm[agree][tW7/VhF8A1gPKHDPj63E30M3STsfP2jH4ateq/ZiNXM=]");
                            }
                        }).on("change", "[id^=TicketForm_ticketPrice_]", function(event) {
                            var count = valueCount($("[id^=TicketForm_ticketPrice_]")),
                                maxQuota = 4;

                         * */ 
                    }

                    /// <summary>
                    /// 取得票資訊
                    /// </summary>
                    public void GetTicket()
                    {
                        TixcraftWebDriver.GetWebSourceCode(url);
                        //取得CSRFTOKEN
                        List<SWebElement> RecDataElement = HtmlAnalyze.FindElement(TixcraftWebDriver.strPageSourceCode,WebBy.ID("CSRFTOKEN"));
                        if (RecDataElement.Count > 0)
                        {
                            strCSRFTOKEN = RecDataElement[0].GetAttribute("value");
                        }
                        RecDataElement = HtmlAnalyze.FindElement(TixcraftWebDriver.strPageSourceCode, WebBy.Tag("select"));
                        strTicketPriceName = RecDataElement[0].Name;
                        string Temp = RecDataElement[0].Context;
                        RecDataElement = HtmlAnalyze.FindElement(Temp, WebBy.Tag("option"));
                        MaxTicketPrice = RecDataElement.Count;
                        List<SWebElement> Verification = HtmlAnalyze.FindElement(TixcraftWebDriver.strPageSourceCode, WebBy.ID("yw0"));
                        #region 取得認證碼
                        if (Verification.Count > 0)
                        {
                            string VerificationUrl = Verification[0].GetAttribute("src");
                            VerificationCodeImage = TixcraftWebDriver.DownloadWebImage(TixcraftURL + VerificationUrl);
                        }
                        #endregion
                        
                        //Get Agree Number

                        strTicketAgree_MagicNumber = GetTicketAgree_MagicNumber(TixcraftWebDriver.strPageSourceCode);

                    }

                    public void RefreshVeryfiImage()
                    {
                        string strPageSource = TixcraftWebDriver.GetWebSourceCode("https://tixcraft.com/ticket/captcha?refresh=1");

                        string strVericationURL = strPageSource.Substring(strPageSource.IndexOf("v="));

                        strVericationURL = strVericationURL.Substring(0,strVericationURL.IndexOf("\""));

                        VerificationCodeImage = TixcraftWebDriver.DownloadWebImage("https://tixcraft.com/ticket/captcha?" + strVericationURL);

                    }

                    /// <summary>
                    /// 開始買票
                    /// </summary>
                    /// <param name="_VerificationCode">認證碼</param>
                    /// <param name="_ticketPrice">購買張數</param>
                    /// <returns></returns>
                    public bool Buy(string _VerificationCode, int _ticketPrice)
                    {
                        VerificationCode = _VerificationCode;    
                        ticketPrice = _ticketPrice;
                        BuyTicketSubmit(url, strCSRFTOKEN, ticketPrice, VerificationCode);
                        List<SWebElement> test = HtmlAnalyze.FindElement(TixcraftWebDriver.strPageSourceCode,WebBy.Tag("select"));
                        if (test.Count == 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    /// <summary>
                    /// 提交資料
                    /// </summary>
                    /// <param name="sUrl"></param>
                    /// <param name="CSRFTOKEN"></param>
                    /// <param name="ticketPrice"></param>
                    /// <param name="verifyCode"></param>
                    /// <returns></returns>
                    private bool BuyTicketSubmit(string sUrl, string CSRFTOKEN, int ticketPrice, string verifyCode)
                    {
                        string postData = "CSRFTOKEN=" + CSRFTOKEN +
                              "&" + strTicketPriceName + "=" + ticketPrice +
                              "&TicketForm[verifyCode]=" + verifyCode +
                              //"&TicketForm[agree]=" + "1" +
                              "&TicketForm[agree][" + strTicketAgree_MagicNumber + "]=" + "1" + //20190729 wuku add
                              //"&ticketPriceSubmit=%E7%A2%BA%E8%AA%8D%E5%BC%B5%E6%95%B8";
                                "&ticketPriceSubmit=";
                        
                        /*
                            CSRFTOKEN: SWl-MHRFYWFKQW9rYmtkX3NjUE9ZZlFXbGJ-STdQVEQ85RE9p0uxPVwTX263u7iF4hnRK5746PwsjbSEyLMNvQ==
                            TicketForm[ticketPrice][01]: 1
                            TicketForm[verifyCode]: 888
                            TicketForm[agree][fvETzoRQXeRUi9PB/q3bKpxhdKW4EX5GuumL23Cum2U=]: 1
                            ticketPriceSubmit: 
                         
                         */ 
                        TixcraftWebDriver.PostDataToUrl(postData, sUrl);
                        //TixcraftWebDriver.GetWebSourceCode("https://tixcraft.com/ticket/order");
                        //string strCheck =  TixcraftWebDriver.GetWebSourceCode("https://tixcraft.com/ticket/check");

                        return true;

                    }
                }
            }
        }


    }
}
