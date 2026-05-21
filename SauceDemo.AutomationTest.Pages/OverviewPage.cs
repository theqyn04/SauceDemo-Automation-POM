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
        private IWebElement FinishButton => _wait.Until(d => {
            var element = d.FindElement(By.Id("finish"));
            // Ép driver cuộn màn hình xuống dưới nếu nút finish bị che khuất (Tuyệt chiêu chống lỗi click)
            ((IJavaScriptExecutor)d).ExecuteScript("arguments[0].scrollIntoView(true);", element);
            return element;
        });
        private IWebElement CompleteHeader => _wait.Until(d => d.FindElement(By.ClassName("complete-header")));

        // 2. Actions
        public void ClickFinish()
        {
            FinishButton.Click();
        }

        public string GetCompleteMessage()
        {
            // Tạo một bộ đợi ngắn tại chỗ để đảm bảo phần tử hiển thị 100% rồi mới bắt đầu đọc chữ (Text)
            var localWait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            IWebElement element = localWait.Until(d => {
                var el = d.FindElement(By.ClassName("complete-header"));
                return (el.Displayed && !string.IsNullOrEmpty(el.Text)) ? el : null;
            });

            return element.Text;
        }
    }
}