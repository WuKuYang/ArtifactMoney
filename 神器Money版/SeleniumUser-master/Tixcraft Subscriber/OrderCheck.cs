using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TSubscriber;
using System.Text.RegularExpressions;

namespace Tixcraft_Subscriber
{
    public class OrderCheck
    {
        private List<SWebElement> PageOrderInfo = new List<SWebElement>();
        public List<OrderStruct> OrderInfo = new List<OrderStruct>();
        public string Email = "";
        public OrderCheck(List<SWebElement> lstPageElem , string strEmail)
        {
            PageOrderInfo = lstPageElem;
            Email = strEmail;
            Trans_Data();
        }

        private void Trans_Data()
        {
            OrderInfo.Clear();
            if (PageOrderInfo != null)
            {
                for (int i = 0; i < PageOrderInfo.Count; i++)
                {
                    OrderStruct PerOrder = new OrderStruct();
                    List<SWebElement> myEm = HtmlAnalyze.FindElement(PageOrderInfo[i].Context, WebBy.Tag("em"));

                    PerOrder.Date = myEm[0].ElementName; //日期
                    PerOrder.OrderID = myEm[1].ElementName; //訂單編號
                    PerOrder.BuyStatue = myEm[3].ElementName; //訂單狀態 

                    string strOrderInformation = RemoveHTMLTag(myEm[2].ElementName); 

                    #region 取得匯款帳號、代碼 
                    int start = strOrderInformation.IndexOf("銀行代號");
                    int end = strOrderInformation.IndexOf("前付款完成");

                    int startibon = strOrderInformation.IndexOf("ibon取票序號 ");
                    int endibon = strOrderInformation.IndexOf("請於");
                    string infomation = "";

                    #region ibon

                    if ((startibon != -1) && (endibon != -1))
                    {
                        infomation = strOrderInformation.Substring(startibon, endibon - startibon - 1).Replace("<br>", "");
                        PerOrder.BankAddress = infomation; //節目


                        //int BuyShowstart = strOrderInformation.IndexOf("\n");
                        //int BuyShowend = strOrderInformation.IndexOf("銀行代號");
                        //string Buyinfomation = "";
                        //if ((BuyShowstart != -1) && (BuyShowend != -1))
                        //{
                        //    Buyinfomation = strOrderInformation.Substring(BuyShowstart, BuyShowend - BuyShowstart - 1).Replace("<br>", "");
                        //    PerOrder.BuyShow = Buyinfomation;
                        //}
                    }
                    else
                    {
                        PerOrder.BuyShow = strOrderInformation;
                    }
                    #endregion


                    #region 匯款資訊 
                    if ((start != -1) && (end != -1))
                    {
                        infomation = strOrderInformation.Substring(start, end - start - 1).Replace("<br>", "");
                        PerOrder.BankAddress = infomation; //節目


                        int BuyShowstart = strOrderInformation.IndexOf("\n");
                        int BuyShowend = strOrderInformation.IndexOf("銀行代號");
                        string Buyinfomation = "";
                        if ((BuyShowstart != -1) && (BuyShowend != -1))
                        {
                            Buyinfomation = strOrderInformation.Substring(BuyShowstart, BuyShowend - BuyShowstart - 1).Replace("<br>", "");
                            PerOrder.BuyShow = Buyinfomation;
                        }
                    }
                    else 
                    {
                        PerOrder.BuyShow = strOrderInformation;
                    }
                    PerOrder.BuyShow = PerOrder.BuyShow.Replace(" ", "");
                    PerOrder.BuyShow = PerOrder.BuyShow.Replace("\n", "");
                    #endregion
                    #endregion
                    OrderInfo.Add(PerOrder);
                }
            }
        }


       private string RemoveHTMLTag(string htmlSource)
        {
            //移除  javascript code.
            htmlSource = Regex.Replace(htmlSource, @"<script[\d\D]*?>[\d\D]*?</script>", String.Empty);

            //移除html tag.
            htmlSource = Regex.Replace(htmlSource, @"<[^>]*>", String.Empty);
            return htmlSource;
        }


    }
}
