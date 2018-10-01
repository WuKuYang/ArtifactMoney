using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Drawing;
using Tixcraft_Subscriber;

namespace TSubscriber
{
    class mFacebook
    {
        
        public delegate void UpdateInfomation();
        
        public static UpdateInfomation UpdateStatus = null;
        
        public string ConectionStatus = ""; 
        /// <summary>
        /// 使用者帳號
        /// </summary>
        public string email;
        /// <summary>
        /// 使用者密碼
        /// </summary>
        public string password;
        /// <summary>
        /// 使用者名稱
        /// </summary>
        public string UserName;
        /// <summary>
        /// 使用者圖片
        /// </summary>
        public Image UserImage = null;
        /// <summary>
        /// 迷你瀏覽器
        /// </summary>
        public FastHttpWebDriver miniFacebookBrowser = new FastHttpWebDriver();
        
        public mFacebook()
        {
            email = "";
            password = "";
            UserName = "";
        }
        
        public mFacebook(string email, string pass)
        {
            this.email = email;
            this.password = pass;
        }
        /// <summary>
        /// 登入
        /// </summary>
        /// <returns></returns>
        public bool Login()
        {
            string FacebookLoginURL = "https://www.facebook.com/login.php?login_attempt=1&lwv=110";
            
            #region 連接Facebook 取得相關資訊
            ConectionStatus = "連接伺服器.";
            if (UpdateStatus != null)
            {
                UpdateStatus();
            }
            miniFacebookBrowser.GetWebSourceCode(FacebookLoginURL);
            ConectionStatus = "連接伺服器...";
            #endregion 
            
            #region 取得隱藏資訊並建立Post資料
            List<SWebElement> FBhiddenCode = HtmlAnalyze.FindElement(miniFacebookBrowser.strPageSourceCode, WebBy.Type("hidden"));
            //取得所有post 資料
            string PostData = "";
            for (int i = 0; i < FBhiddenCode.Count; i++)
            {
                PostData += (FBhiddenCode[i].Name + "=" + FBhiddenCode[i].value + "&");
            }
            PostData += "email=" + email + "&pass=" + password;
            #endregion

            #region 傳送資料等待回應
            ConectionStatus = "等待伺服器回應.";
            if (UpdateStatus != null)
            {
                UpdateStatus();
            }
            miniFacebookBrowser.PostDataToUrl(PostData, FacebookLoginURL);
            ConectionStatus = "等待伺服器回應...";
            #endregion

            List<Cookie> Cookies = miniFacebookBrowser.getCookiesList();
            //判斷是否登入成功旗標
            bool IsLoginSuccess = false;
            for (int i = 0; i < Cookies.Count; i++)
            {
                //根據cookie內有沒有c_user判斷是否登入成功
                if (Cookies[i].Name == "c_user")
                {
                    IsLoginSuccess = true;
                    break;
                }
            }
           
            if (IsLoginSuccess)
            {
                #region 取得個人檔案
                string FacebookPage = miniFacebookBrowser.GetWebSourceCode("https://www.facebook.com");
                List<SWebElement> profileData = HtmlAnalyze.FindElement(miniFacebookBrowser.strPageSourceCode, WebBy.ID("bookmarkmenu"));
                if (profileData.Count != 0)
                {
                    //取得使用者大頭貼
                    List<SWebElement> urlFacebookUserData = HtmlAnalyze.FindElement(profileData[0].ElementName, WebBy.Tag("img"));
                    SWebElement FacebookUserData = urlFacebookUserData[0];
                    string UserImageURL = FacebookUserData.GetAttribute("src").Replace("amp;", "");
                    UserImage = miniFacebookBrowser.DownloadWebImage(UserImageURL);
                    //取得使用者名稱
                    UserName = FacebookUserData.GetAttribute("alt");
                }
                #endregion

                ConectionStatus = "登入成功!";
                if (UpdateStatus != null)
                {
                    UpdateStatus();
                }

                return true;
            }
            else
            {
                UserImage = null;
                UserName = "";

                ConectionStatus = "登入失敗!";
                if (UpdateStatus != null)
                {
                    UpdateStatus();
                }
                return false;
            }
        }
    }
}
