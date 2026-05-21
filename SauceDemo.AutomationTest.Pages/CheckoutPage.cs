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
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        }

        public void ClickCheckout()
        {
            // Đợi nút Checkout xuất hiện và bấm
            var checkoutBtn = _wait.Until(d => d.FindElement(By.Id("checkout")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", checkoutBtn);
        }

        public void EnterInformationAndContinue(string firstName, string lastName, string postalCode)
        {
            // Đợi cho ô FirstName xuất hiện hiển thị trên màn hình rồi mới tương tác
            var firstNameInput = _wait.Until(d => {
                var el = d.FindElement(By.Id("first-name"));
                return el.Displayed ? el : null;
            });

            // Thực hiện điền dữ liệu an toàn
            firstNameInput.Clear();
            firstNameInput.SendKeys(firstName);

            var lastNameInput = _driver.FindElement(By.Id("last-name"));
            lastNameInput.Clear();
            lastNameInput.SendKeys(lastName);

            var postalCodeInput = _driver.FindElement(By.Id("postal-code"));
            postalCodeInput.Clear();
            postalCodeInput.SendKeys(postalCode);

            // Đợi nút Continue sẵn sàng và ép Click bằng JavaScript để không bị hụt
            var continueBtn = _wait.Until(d => d.FindElement(By.Id("continue")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", continueBtn);

            // Chờ cho URL chuyển hẳn sang trang Overview (Step Two)
            _wait.Until(d => d.Url.Contains("checkout-step-two.html"));
        }
    }
}