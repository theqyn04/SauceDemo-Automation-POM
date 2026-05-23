using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace SauceDemo.AutomationTest.Pages
{
    public class LoginPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public LoginPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        private IWebElement UsernameInput => _wait.Until(d => d.FindElement(By.Id("user-name")));
        private IWebElement PasswordInput => _driver.FindElement(By.Id("password"));
        private IWebElement LoginButton => _driver.FindElement(By.Id("login-button"));
        private readonly By errorMessageContainer = By.XPath("//h3[@data-test='error']");

        public void Login(string username, string password)
        {
            UsernameInput.SendKeys(username);
            PasswordInput.SendKeys(password);
            LoginButton.Click();
        }

        public string GetErrorMessage()
        {
            return _driver.FindElement(errorMessageContainer).Text;
        }
    }
}