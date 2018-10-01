using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TixWin;
using System.IO;

namespace Tixcraft_Subscriber
{
    public class GoogleBrowserSDK
    { 
        public IWebDriver Driver { get; set; } 
        public WindowController HwndController = new WindowController();

        public void OpenBrowser()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddAdditionalCapability("profile.default_content_settings", 2);
            options.AddAdditionalCapability("pageLoadStrategy", "none");
            options.AddAdditionalCapability("webdriver.load.strategy", "unstable");
            Driver = new ChromeDriver(options);
            //== 取得視窗碼 ==
            HwndController.hwnd = WindowSnap.FindWindow(null, "data:, - Google Chrome");
        }

        /// <summary>
        /// 注入JavaScript ， 並且執行
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
        public void GoURL(string strURL)
        {
            Driver.Navigate().GoToUrl(strURL); 
        }

    }
}
