using OpenQA.Selenium.Chrome;
using SL_TestAutomationFramework.lib.pages;
using SL_TestAutomationFramework.Utilities;
using TechTalk.SpecFlow;

namespace SL_TestAutomationFramework.BDD
{
    public class SL_Signin_SharedSteps
    {
        protected Credentials _credentials;
        public SL_Website<ChromeDriver> SL_Website { get; } = new SL_Website<ChromeDriver>();
        [Given(@"I am on the home page")]
        public void GivenIAmOnTheHomePage()
        {
            SL_Website.SL_HomePage.VisitHomePage();
        }

        [Given(@"I have entered a valid e-mail")]
        public void GivenIHaveEnteredAValidE_Mail()
        {
            SL_Website.SL_HomePage.EnterUserName(AppConfigReader.UserName);
        }

    }
}
