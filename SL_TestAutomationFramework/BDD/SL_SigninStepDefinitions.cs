using TechTalk.SpecFlow;
using SL_TestAutomationFramework.Utilities;
using TechTalk.SpecFlow.Assist;

namespace SL_TestAutomationFramework.BDD
{
    [Binding]
    public class SL_SigninStepDefinitions : SL_Signin_SharedSteps
    {
       
        [Given(@"I have entered a valid password")]
        public void GivenIHaveEnteredAValidPassword()
        {
            SL_Website.SL_HomePage.EnterPassword(AppConfigReader.Password);
        }

        [When(@"I click the login button")]
        public void WhenIClickTheLoginButton()
        {
            SL_Website.SL_HomePage.ClickLoginButton();
        }

        [Given(@"I have entered an invalid password of ""([^""]*)""")]
        public void GivenIHaveEnteredAnInvalidPasswordOf(string passwords)
        {
            SL_Website.SL_HomePage.EnterPassword(passwords);
        }
        [Given(@"I have the following credentials:")]
        public void GivenIHaveTheFollowingCredentials(Table table)
        {
             _credentials = table.CreateInstance<Credentials>();
        }

        [When(@"enter these credentials")]
        public void WhenEnterTheseCredentials()
        {
            SL_Website.SL_HomePage.EnterSigninCredentials(_credentials);
        }

        [Then(@"I should see an error message that contains ""([^""]*)""")]
        public void ThenIShouldSeeAnErrorMessageThatContains(string expected)
        {
            Assert.That(SL_Website.SL_HomePage.CheckErrorMessage, Does.Contain(expected));
        }


        [Then(@"I should land on the invetory page")]
        public void ThenIShouldLandOnTheInvetoryPage()
        {
            Assert.That(SL_Website.SeleniumDriver.Url, Is.EqualTo(AppConfigReader.InventoryPageUrl));
        }

        [AfterScenario]
        public void DiposeWebDriver()
        {
            SL_Website.SeleniumDriver.Quit();
        }
    }
}
