using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Chrome;
using SeleniumUser.ExampleTests.Pages;
using System.Threading;
using System.Collections.Generic;

namespace SeleniumUser.ExampleTests
{
    [TestClass]
    public class GoogleSearch
    {
        private IWebDriver Driver { get; set; }

        [TestInitialize]
        public void Setup()
        {
            //Driver = new InternetExplorerDriver();
            //new InternetExplorerOptions { IntroduceInstabilityByIgnoringProtectedModeSettings = true }; 
            //Driver = new ChromeDriver();
            Driver.Navigate().GoToUrl("http://google.com"); 
            User.Driver = Driver;
        }

        [TestCleanup]
        public void TearDown()
        {
            Driver.Quit();
        }

        [TestMethod]
        public void Search_Returns_Results()
        {
            var page = new SearchPage()
                .SetSearchField("apple")
                .Search()
                .ShouldSeeResult("Apple");
        }
    }


    public class TicketsTest
    {
        private IWebDriver Driver { get; set; }
        //public string strHomePage = "https://tixcraft.com/activity/game/17_rene";
        public string strHomePage = "https://tixcraft.com/ticket/area/17_rene/2911";
        
        [TestInitialize]
        public void Setup()
        {
            //Driver = new InternetExplorerDriver();
            //new InternetExplorerOptions { IntroduceInstabilityByIgnoringProtectedModeSettings = true };
            ChromeOptions options = new ChromeOptions();
            options.AddAdditionalCapability("profile.default_content_settings", 2);
            Driver = new ChromeDriver(options);
            Driver.Navigate().GoToUrl(strHomePage);
            User.Driver = Driver;
        }

        [TestCleanup]
        public void TearDown()
        {
            Driver.Quit();
        }

        [TestMethod]
        public void Search_Returns_Results()
        {
            var page = new ThicketTool();
            page = page.Click("yt0");
            page.Click("2911_2");
        }
        public void AutoBuy()
        { 


            IWebElement BuyNow = null;
            for (int i = 0; i < 100; i++)
            {
                try
                {

                    //BuyNow = Driver.FindElement(By.Name("yt0gg"));
                    BuyNow = Driver.FindElement(By.ClassName("select_form_b"));
                    BuyNow.FindElement(By.TagName("a"));
                    //BuyNow = Driver.FindElement(By.ClassName("yt0gg"));
                    BuyNow.Click();
                    Console.Write("Clicked");
                    break;
                }
                catch (NoSuchElementException e)
                {

                    //throw e;
                }
                if (BuyNow == null) Driver.Navigate().Refresh();
                Console.Write(i.ToString() + " , ");
                Thread.Sleep(10);
            }
        }
    }

    

}
