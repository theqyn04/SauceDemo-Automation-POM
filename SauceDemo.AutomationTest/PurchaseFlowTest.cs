using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using SauceDemo.AutomationTest.Pages; 

namespace SauceDemo.AutomationTest
{
    [TestFixture]
    public class PurchaseFlowTest
    {
        private IWebDriver driver;

        [SetUp]
        public void Setup()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--disable-extensions");

            driver = new ChromeDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.Navigate().GoToUrl("https://www.saucedemo.com/");
        }

        [Test]
        public void TestPurchaseFlowWithPOM()
        {
            // Khởi tạo các đối tượng Page tương ứng
            LoginPage loginPage = new LoginPage(driver);
            InventoryPage inventoryPage = new InventoryPage(driver);
            CheckoutPage checkoutPage = new CheckoutPage(driver);
            OverviewPage overviewPage = new OverviewPage(driver);

            // Bước 1: Đăng nhập
            loginPage.Login("standard_user", "secret_sauce");

            // Bước 2: Thêm balo vào giỏ và đi tới trang giỏ hàng
            inventoryPage.AddBackpackToCartAndGoToCart();

            // Bước 3: Tiến hành checkout và điền thông tin khách hàng
            checkoutPage.ClickCheckout();
            checkoutPage.EnterInformationAndContinue("Quyen", "Nguyen", "100000");

            // Bước 4: Hoàn tất đặt hàng
            overviewPage.ClickFinish();

            // Bước 5: Kiểm tra kết quả hiển thị cuối cùng
            string actualMessage = overviewPage.GetCompleteMessage();
            string expectedMessage = "Thank you for your order!";

            Assert.That(actualMessage, Is.EqualTo(expectedMessage), "LỖI: Luồng mua hàng không hiển thị thông báo thành công!");
        }

        [TearDown]
        public void CloseBrowser()
        {
            driver?.Dispose();
        }
    }
}