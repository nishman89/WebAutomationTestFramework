using OpenQA.Selenium;
using SL_TestAutomationFramework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL_TestAutomationFramework.lib.pages
{
    public class SL_HomePage
    {
        private IWebDriver SeleniumDriver { get; }
        private string _homePageUrl = AppConfigReader.BaseUrl;
        private IWebElement _passwordField => SeleniumDriver.FindElement(By.Id("password"));
        private IWebElement _usernameField => SeleniumDriver.FindElement(By.Id("user-name"));
        private IWebElement _loginButton => SeleniumDriver.FindElement(By.Id("login-button"));
        private IWebElement _errorMessage => SeleniumDriver.FindElement(By.CssSelector("[data-test=\"error\"]"));
        //private IWebElement _errorMessage
        //{
        //    get { return SeleniumDriver.FindElement(By.CssSelector("[data-test=\"error\"]")); }
        //}
        public SL_HomePage(IWebDriver seleniumDriver)
        {
            SeleniumDriver = seleniumDriver;
        }

        public void VisitHomePage() => SeleniumDriver.Navigate().GoToUrl(_homePageUrl);
        public void EnterUserName(string username) => _usernameField.SendKeys(username);
        public void EnterPassword(string password) => _passwordField.SendKeys(password);
        public void ClickLoginButton() => _loginButton.Click();
        public string CheckErrorMessage() => _errorMessage.Text;

        public void EnterSigninCredentials(Credentials creds)
        {
            EnterUserName(creds.UserName);
            EnterPassword(creds.Password);
        }
    }
}
