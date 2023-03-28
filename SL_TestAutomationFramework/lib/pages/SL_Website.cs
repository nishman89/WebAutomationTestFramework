using OpenQA.Selenium;
using SL_TestAutomationFramework.lib.driver_config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL_TestAutomationFramework.lib.pages
{
    //Super class - essentiall acting as a service object for all pages
    //i.e. decomposes objects into manageable classes and methods
    public class SL_Website<T> where T : IWebDriver, new()
    {
        #region Accessible Page Objects and Selenium Driver
        public IWebDriver SeleniumDriver { get; set; }
        public SL_HomePage SL_HomePage { get; set; }
        public SL_InventoryPage SL_InventoryPage { get; set; }
        #endregion
        public SL_Website(int pageLoadInsecs = 10, int implicityWaitInSecs = 10, bool isHeadless = false)
        {
            //Instantiate the driver
            SeleniumDriver = new SeleniumDriverConfig<T>(pageLoadInsecs, implicityWaitInSecs, isHeadless).Driver;
            SL_HomePage = new SL_HomePage(SeleniumDriver);
            SL_InventoryPage = new SL_InventoryPage(SeleniumDriver);
        }
    }
}
