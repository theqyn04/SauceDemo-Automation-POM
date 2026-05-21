using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace SauceDemo.AutomationTest.Pages
{
    public class InventoryPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public InventoryPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // 1. Locators
        private IWebElement AddBackpackToCartButton => _wait.Until(d => d.FindElement(By.Id("add-to-cart-sauce-labs-backpack")));
        private IWebElement ShoppingCartLink => _driver.FindElement(By.ClassName("shopping_cart_link"));

        // 2. Actions
        public void AddBackpackToCartAndGoToCart()
        {
            AddBackpackToCartButton.Click();
            ShoppingCartLink.Click();
        }
    }
}