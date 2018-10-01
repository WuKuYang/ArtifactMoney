using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeleniumUser.ExampleTests; 
namespace SeleniumUI
{
    class Program
    {
        static void Main(string[] args)
        {

            //GoogleSearch gTool = new GoogleSearch();
            //gTool.Setup();
            //gTool.Search_Returns_Results();


            TicketsTest gTool = new TicketsTest();
            gTool.Setup();
            gTool.AutoBuy();

            while (true) ;
        }
    }
}
