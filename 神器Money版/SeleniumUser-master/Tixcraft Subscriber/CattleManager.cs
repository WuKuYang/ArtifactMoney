using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tixcraft_Subscriber;

namespace TSubscriber
{
    public class CattleManager
    {
        public static bool CheckCattleVerification(string CattleID)
        {
            FastHttpWebDriver FastHttpDriver = new FastHttpWebDriver();
            string CheckVerificationURL = "https://sites.google.com/site/akgmljsgoapitrko/home?previewAsViewer=1";
            FastHttpDriver.GetWebSourceCode(CheckVerificationURL);
            List<SWebElement> Data = HtmlAnalyze.FindElement(FastHttpDriver.strPageSourceCode, WebBy.Tag("th"));
            bool checkUser = false;
            for (int i = 0; i < Data.Count; i++)
            {
                if (Data[i].ElementName == CattleID)
                {
                    checkUser = true;
                    break;
                }
            }
            return checkUser;
            //return true;
        }
        public static void RequestCattleVerification(string RequestCattleID)
        {
            FastHttpWebDriver FastHttpDriver = new FastHttpWebDriver();
            string RequestVerificationURL = "https://docs.google.com/forms/d/e/1FAIpQLScmRJOK-iWqBNUggQ7FEYr2HQ5KjqYSbzRwYkWuTv5XWsBukg/formResponse";
            FastHttpDriver.GetWebSourceCode(RequestVerificationURL);
            FastHttpDriver.strPageSourceCode = FastHttpDriver.strPageSourceCode.Replace("&quot;", "");
            FastHttpDriver.strPageSourceCode = FastHttpDriver.strPageSourceCode.Replace("\n", "");
            List<SWebElement> Text = HtmlAnalyze.FindElement(FastHttpDriver.strPageSourceCode, WebBy.Type("text"));
            List<SWebElement> Hidden = HtmlAnalyze.FindElement(FastHttpDriver.strPageSourceCode, WebBy.Type("hidden"));
            Text.AddRange(Hidden);
            string PostData = "";
            for (int i = 0; i < Text.Count; i++)
            {
                PostData += (Text[i].Name + "=" + Text[i].value);
                if (i == 0)
                {
                    PostData += "id:"+RequestCattleID;
                }
                if (i != Text.Count - 1)
                {
                    PostData += "&";
                }
            }
            FastHttpDriver.PostDataToUrl(PostData, RequestVerificationURL);
        }
    }
}
