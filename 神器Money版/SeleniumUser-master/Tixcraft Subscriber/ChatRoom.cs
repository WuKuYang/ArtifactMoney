using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;

namespace Tixcraft_Subscriber
{
    public class ChatMessage
    {
        /// <summary>
        /// 傳送訊息的人
        /// </summary>
        public string Name;
        /// <summary>
        /// 傳送的訊息
        /// </summary>
        public string Msg;
    }
    public class ChatRoom
    {
        private string account = "ChatServer";
        private string password = "admin";
        private string chatfilter = "Chat";

        public FastHttpWebDriver ChatWebDriver = new FastHttpWebDriver();
        
        public bool StarReceiveMessage = true;
        /// <summary>
        /// 接收到的所有訊息
        /// </summary>
        private List<ChatMessage> mChatMessage = new List<ChatMessage>();
        /// <summary>
        /// 接收訊息
        /// </summary>
        public void StartingReceiveMessage()
        {
            while (StarReceiveMessage)
            {

                System.Diagnostics.Stopwatch p = new System.Diagnostics.Stopwatch();//引用stopwatch物件
                p.Reset();//碼表歸零
                p.Start();//碼表開始計時
                string strPageSourceCode = GetMessageSource();
                p.Stop();//碼錶停止
                //印出所花費的總豪秒數
                Debug.Print(p.Elapsed.TotalMilliseconds.ToString());
                try
                {
                    #region 轉換原始資料到有用資訊
                    MatchCollection TagBr = Regex.Matches(strPageSourceCode, "<br>");
                    string StrSource = strPageSourceCode.Substring(TagBr[1].Index, TagBr[TagBr.Count - 1].Index - TagBr[1].Index);
                    StrSource = StrSource.Replace("<br>", "\n");
                    StrSource = StrSource.Replace("帳號：", "");
                    StrSource = StrSource.Replace("電子郵件：", "");
                    StrSource = StrSource.Replace("電話：", "status:");
                    #endregion

                    string[] ReceiveMessage = StrSource.Split('\n');

                    List<ChatMessage> ChatMessageT = new List<ChatMessage>();

                    for (int i = 0; i < ReceiveMessage.Length; i++)
                    {
                        string[] subMsg = ReceiveMessage[i].Split(',');
                        if (subMsg.Length == 3)
                        {
                            if (subMsg[2].Contains("chat"))
                            {
                                ChatMessage TempChatMessage = new ChatMessage();
                                TempChatMessage.Name = subMsg[0].Replace(" - ", ",").Split(',')[1];
                                TempChatMessage.Msg = subMsg[1].Replace(" ","");
                                ChatMessageT.Add(TempChatMessage);
                            }
                        }
                    }
                    mChatMessage = ChatMessageT;
                }
                catch
                {

                }
                Thread.Sleep(5);
            }
        }
        public ChatRoom()
        {
            InitRoom();
            new Thread(StartingReceiveMessage).Start();
        }
        public void Register()
        {
            //註冊使用帳號 post 註冊(傳送訊息到聊天室)
            ChatWebDriver.UTF8PostDataToUrl("ac=" + account + "&pw=" + password + "&pw2=" + password + "&email=" + chatfilter + "&tel=" + chatfilter + "&button=%E7%A2%BA%E5%AE%9A", "http://120.108.111.146/omega87910/phdsay/register_finish.php");
        }
        public void InitRoom()
        {
            Register();
            //登入帳號進入管理後台(需要先登入進入後台)
            ChatWebDriver.UTF8PostDataToUrl("ac=" + account + "&pw=" + password, "http://120.108.111.146/omega87910/phdsay/connect.php");
        }

        public string GetMessageSource()
        {
            //取後台資料(取得聊天室內容)(這邊可以一直取就一直刷新)
            return ChatWebDriver.GetWebSourceCode("http://120.108.111.146/omega87910/phdsay/users.php");
        }
        /// <summary>
        /// 傳送訊息(Name,Msg)
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="msg"></param>
        public void SendMessage(string Name, string msg)
        {
            if (msg != "")
            {
                //註冊使用帳號 post 註冊(傳送訊息到聊天室)
                ChatWebDriver.UTF8PostDataToUrl("ac=" + Name + "&pw=" + password + "&pw2=" + password + "&email=" + msg + "&tel=" + "chat" + "&button=%E7%A2%BA%E5%AE%9A", "http://120.108.111.146/omega87910/phdsay/register_finish.php");
            }
        }
        public List<ChatMessage> GetMessage
        {
            get
            {
                return mChatMessage;
            }
        }
        /// <summary>
        /// 清除資料
        /// </summary>
        /// <param name="account"></param>
        public void Clear(string account)
        {
            ChatWebDriver.UTF8PostDataToUrl("ac=" + account + "&button=%E5%88%AA%E9%99%A4", "http://120.108.111.146/omega87910/phdsay/delete_finish.php");
        }
    }
}
