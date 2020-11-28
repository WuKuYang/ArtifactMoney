using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tixcraft_Subscriber
{
    public class LineBot
    {

        /// <summary>
        /// 傳送圖片
        /// </summary>
        /// <param name="strToken"></param>
        /// <param name="strMessage"></param>
        /// <param name="strPicURL"></param>
        public void Line_SendImage(string strToken, string strMessage, string strPicURL)
        {
            try
            {

                // Create a request using a URL that can receive a post.

                WebRequest request = WebRequest.Create("https://notify-api.line.me/api/notify");



                // Set the Method property of the request to POST.

                request.Method = "POST";



                // Create POST data and convert it to a byte array.

                string Token = strToken;

                string message = "message=" + WebUtility.HtmlEncode("\r\n" + strMessage);

                string picURL = strPicURL;

                string imageThumbnail = "&imageThumbnail=" + picURL;

                string imageFullsize = "&imageFullsize=" + picURL;

                string postData = message + imageThumbnail + imageFullsize;
                //string postData =  imageThumbnail + imageFullsize;

                byte[] byteArray = Encoding.UTF8.GetBytes(postData);



                // Set the ContentType property of the WebRequest.

                request.ContentType = "application/x-www-form-urlencoded";



                // Set the Header property of the WebRequest.

                request.Headers["Authorization"] = "Bearer " + Token;



                // Set the ContentLength property of the WebRequest.

                request.ContentLength = byteArray.Length;



                // Get the request stream.

                Stream dataStream = request.GetRequestStream();



                // Write the data to the request stream.

                dataStream.Write(byteArray, 0, byteArray.Length);



                // Close the Stream object.

                dataStream.Close();



                // Get the response.

                WebResponse response = request.GetResponse();



                // Display the status.

                Console.WriteLine(((HttpWebResponse)response).StatusDescription);



                // Get the stream containing content returned by the server.

                dataStream = response.GetResponseStream();



                // Open the stream using a StreamReader for easy access.

                StreamReader reader = new StreamReader(dataStream);



                // Read the content.

                string responseFromServer = reader.ReadToEnd();



                // Display the content.

                Console.WriteLine(responseFromServer);



                // Clean up the streams.

                reader.Close();

                dataStream.Close();

                response.Close();
                request.Abort();

            }
            catch (Exception)
            {

            }

        }

        /// <summary>
        /// 傳送文字
        /// </summary>
        /// <param name="message"></param>
        /// <param name="token"></param>
        public void Line_SendMessage(string message, string token)
        {
            try
            {
                try
                {
                    string strDateTime = string.Format("\n-----------\n[時間戳記]:{0}:{1}:{2}ms", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Millisecond);
                    message = message + strDateTime;
                }
                catch (Exception)
                {
                     
                }
                

                string url = "https://notify-api.line.me/api/notify";
                string postData = "message=" + WebUtility.HtmlEncode("\r\n" + message);
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                Uri target = new Uri(url);
                WebRequest request = WebRequest.Create(target);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;
                request.Headers.Add("Authorization", "Bearer " + token);

                using (var dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }
                Thread.Sleep(1000);
                request.Abort();
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 上傳本地端影像到網路，並取得回傳之URL
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public string ImgurUpdate(string FilePath)
        {
            try
            {
                //建立ImgurClient(其中的"CLIENT_ID", "CLIENT_SECRET"要換成你自己的)
                var client = new ImgurClient("64aeca0a4d9b45a", "cff6f5cf99d28b28d2a57a35181873b66bee7810");
                var endpoint = new ImageEndpoint(client);
                IImage image;
                //取得圖片檔案FileStream
                using (var fs = new FileStream(FilePath, FileMode.Open))
                {
                    image = endpoint.UploadImageStreamAsync(fs).GetAwaiter().GetResult();
                }
                //顯示圖檔位置
                return image.Link;
            }
            catch (Exception)
            {
                return "https://cdn.windowsreport.com/wp-content/uploads/2018/01/Class-not-registered-error-on-Windows-10-886x590.jpg";
            }
        }


    }
}
