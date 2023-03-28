using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL_TestAutomationFramework
{
    public static class AppConfigReader
    {
        public static readonly string BaseUrl = ConfigurationManager.AppSettings["base_url"];
        public static readonly string InventoryPageUrl = ConfigurationManager.AppSettings["inventorypage_url"];
        public static readonly string UserName = ConfigurationManager.AppSettings["username"];
        public static readonly string Password = ConfigurationManager.AppSettings["password"];

    }
}
