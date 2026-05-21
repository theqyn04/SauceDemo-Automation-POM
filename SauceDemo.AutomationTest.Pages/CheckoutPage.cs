using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace SauceDemo.AutomationTest.Pages
{
    public class CheckoutPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public CheckoutPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // 1. Locators
        private IWebElement CheckoutButton => _wait.Until(d => d.FindElement(By.Id("checkout")));
        private IWebElement FirstNameInput => _wait.Until(d => d.FindElement(By.Id("first-name")));
        private IWebElement LastNameInput => _driver.FindElement(By.Id("last-name"));
        private IWebElement PostalCodeInput => _driver.FindElement(By.Id("postal-code"));
        private IWebElement ContinueButton => _driver.FindElement(By.Id("continue"));

        // 2. Actions
        public void ClickCheckout()
        {
            CheckoutButton.Click();
        }

        public void EnterInformationAndContinue(string firstName, string lastName, string postalCode)
        {
            FirstNameInput.SendKeys(firstName);
            LastNameInput.SendKeys(lastName);
            PostalCodeInput.SendKeys(postalCode);
            ContinueButton.Click();
        }
    }
}