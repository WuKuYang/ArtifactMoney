using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tixcraft_Subscriber
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
            Application.Run(new SubscriberLoginer());
            //Application.Run(new TixBuyTicket());
            //Application.Run(new TixBrowserTest());
            //Application.Run(new StockSnaper());
            
        }
    }
}
