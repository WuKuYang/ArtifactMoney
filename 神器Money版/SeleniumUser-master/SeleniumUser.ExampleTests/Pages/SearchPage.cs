using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumUser.ExampleTests.Pages
{
    public class SearchPage
    {
        public SearchPage Search()
        {
            User.Clicks(By.Name("btnK"));

            return new SearchPage();
        }

        public SearchPage SetSearchField(string text)
        {
            User.InputText(By.Name("q"), text);
            
            return new SearchPage();
        }

        public SearchPage ShouldSeeResult(string text)
        {
            User.ShouldSeeText(By.TagName("a"), text);

            return new SearchPage();
        }
    }




    public class ThicketTool
    {
        public ThicketTool Click(string strName)
        {
            User.Clicks(By.Name(strName));
            return new ThicketTool();
        }
        public ThicketTool ClickByID(string strName)
        {
            User.Clicks(By.Id(strName));
            return new ThicketTool();
        }

        public ThicketTool SetText(string strName , string text)
        {
            User.InputText(By.Name(strName), text); 
            return new ThicketTool();
        } 
    }
}
