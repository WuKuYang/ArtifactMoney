using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace Tixcraft_Subscriber
{
    public class FastHttpWebDriver
    {
        #region 網頁協定
        //const string sUserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";//"Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36";
        const string sUserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36";
        //"Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        const string sContentType = "application/x-www-form-urlencoded";
        const string sRequestEncoding = "ASCII";
        const string sResponseEncoding = "UTF-8";
        #endregion

        /// <summary>
        /// 網頁原始碼
        /// </summary>
        public string strPageSourceCode = "";
        /// <summary>
        /// 紀錄目前CookieContainer
        /// </summary>
        public CookieContainer Session = new CookieContainer();
        /// <summary>
        /// 目前網址
        /// </summary>
        public string nUrl = "";
        /// <summary>
        /// 取得所有cookie
        /// </summary>
        /// <returns></returns>
        public List<Cookie> getCookiesList()
        {
            List<Cookie> lstCookie = new List<Cookie>();
            try
            {
                CookieCollection CCookie = Session.GetCookies(new Uri(nUrl));
                for (int i = 0; i < CCookie.Count; i++)
                {
                    Cookie nCookie = CCookie[i];
                    lstCookie.Add(nCookie);
                    Debug.Print("{" + nCookie.Name + "=" + nCookie.Value + "}");
                }
                Debug.Print("========================================");
            }
            catch
            {
            }
            return lstCookie;
        }
        /// <summary>
        /// 取得網頁原始碼
        /// </summary>
        /// <param name="Url"></param>
        /// <returns></returns>
        public string GetWebSourceCode(string Url)
        {
            nUrl = Url;
            string strResult = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Timeout = 3000;
                request.Headers.Set("Pragma", "no-cache");
                //request.Headers.Add("Accept-Language", "en-US,en;q=0.5");
                //request.Headers.Add("Accept-Encoding", "gzip");
                request.Proxy = null;

                //////不建立持久性連結 
                request.KeepAlive = true;
                //request.AllowAutoRedirect = false;
                request.UserAgent = sUserAgent;
                request.ContentType = sContentType;
                request.CookieContainer = Session;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream streamReceive = response.GetResponseStream();
                Encoding encoding = Encoding.GetEncoding("UTF-8");
                StreamReader streamReader = new StreamReader(streamReceive, encoding);
                strResult = streamReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(e.Message);
            }
            strPageSourceCode = strResult;
            return strResult;
        }
        /// <summary>
        /// Post資料到網站
        /// </summary>
        /// <param name="data">Post參數</param>
        /// <param name="url">網址</param>
        /// <returns></returns>
        public string PostDataToUrl(string data, string url)
        {

            Encoding encoding = Encoding.GetEncoding(sRequestEncoding);
            byte[] bytesToPost = encoding.GetBytes(data);
            return PostDataToUrl(bytesToPost, url);
            
        }
        public string UTF8PostDataToUrl(string data, string url)
        {

            Encoding encoding = Encoding.GetEncoding(sResponseEncoding);
            byte[] bytesToPost = encoding.GetBytes(data);
            return PostDataToUrl(bytesToPost, url);
           
        }
        
        /// <summary>
        /// Post資料到網站
        /// </summary>
        /// <param name="data">Post參數</param>
        /// <param name="url">網址</param>
        /// <returns></returns>
        public string PostDataToUrl(byte[] data, string url)
        {
            nUrl = url;
            #region 创建httpWebRequest对象
            WebRequest webRequest = WebRequest.Create(url);
            HttpWebRequest httpRequest = webRequest as HttpWebRequest;
            if (httpRequest == null)
            {
                throw new ApplicationException(string.Format("Invalid url string: {0}", url));
            }
            #endregion

            #region 填充httpWebRequest的基本信息
            httpRequest.UserAgent = sUserAgent;
            httpRequest.ContentType = sContentType;
            httpRequest.Method = "POST";

            httpRequest.CookieContainer = Session;
            httpRequest.Proxy = null;

            //httpRequest.Method = WebRequestMethods.Http.Post;
            //httpRequest.AllowWriteStreamBuffering = true;
            //httpRequest.ProtocolVersion = HttpVersion.Version11;
            //httpRequest.AllowAutoRedirect = true;

            #endregion
            #region 填充要post的内容
            httpRequest.ContentLength = data.Length;
            Stream requestStream = httpRequest.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();
            #endregion
            #region 发送post请求到服务器并读取服务器返回信息
            Stream responseStream;
            try
            {
                responseStream = httpRequest.GetResponse().GetResponseStream();
            }
            catch (Exception ex)
            {
                // log error
                Console.WriteLine(string.Format("POST操作发生异常：{0}", ex.Message));
                throw ex;
            }
            #endregion
            #region 读取服务器返回信息
            string stringResponse = string.Empty;
            using (StreamReader responseReader = new StreamReader(responseStream, Encoding.GetEncoding(sResponseEncoding)))
            {
                stringResponse = responseReader.ReadToEnd();
            }
            responseStream.Close();
            #endregion
            strPageSourceCode = stringResponse;
            return stringResponse;
        }
        /// <summary>
        /// 下載圖片
        /// </summary>
        /// <param name="Url"></param>
        /// <returns></returns>
        public Image DownloadWebImage(string Url)
        {
            try
            {
                HttpWebRequest lxRequest = (HttpWebRequest)WebRequest.Create(Url);
                //lxRequest.UserAgent = sUserAgent;
                //lxRequest.ContentType = sContentType;
                //lxRequest.Method = "GET";
                lxRequest.AllowAutoRedirect = false;
                lxRequest.Proxy = null;
                lxRequest.CookieContainer = Session;
                // returned values are returned as a stream, then read into a string
                String lsResponse = string.Empty;
                Image tmep = null;
                using (HttpWebResponse lxResponse = (HttpWebResponse)lxRequest.GetResponse())
                {
                    using (BinaryReader reader = new BinaryReader(lxResponse.GetResponseStream()))
                    {
                        Byte[] lnByte = reader.ReadBytes(1 * 1024 * 1024 * 10);
                        tmep = byteArrayToImage(lnByte);
                    }
                }

                return tmep;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 轉換byte 到 image
        /// </summary>
        /// <param name="byteArrayIn"></param>
        /// <returns></returns>
        private Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

    }
}
