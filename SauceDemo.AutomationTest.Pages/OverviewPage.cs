using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace SauceDemo.AutomationTest.Pages
{
    public class OverviewPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public OverviewPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // 1. Locators
        private IWebElement FinishButton => _wait.Until(d => d.FindElement(By.Id("finish")));
        private IWebElement CompleteHeader => _wait.Until(d => d.FindElement(By.ClassName("complete-header")));

        // 2. Actions
        public void ClickFinish()
        {
            FinishButton.Click();
        }

        public string GetCompleteMessage()
        {
            return CompleteHeader.Text;
        }
    }
}