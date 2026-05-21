using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using SauceDemo.AutomationTest.Pages;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;

namespace SauceDemo.AutomationTest
{
    [TestFixture]
    public class PurchaseFlowTest
    {
        private IWebDriver driver;

        private static ExtentReports extent;
        private ExtentTest test;

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
            string reportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestResults");
            if (!Directory.Exists(reportPath))
            {
                Directory.CreateDirectory(reportPath);
            }

            var sparkReporter = new ExtentSparkReporter(Path.Combine(reportPath, "ExtentReport.html"));

            
            extent = new ExtentReports();
            extent.AttachReporter(sparkReporter);
            extent.AddSystemInfo("Environment", "QA - Production Simulation");
            extent.AddSystemInfo("Tester", "Quyen Nguyen"); 
            extent.AddSystemInfo("Machine", Environment.MachineName);
        }

        [SetUp]
        public void Setup()
        {
            // Tạo một node test mới trong báo cáo cho mỗi bài test
            test = extent.CreateTest(TestContext.CurrentContext.Test.Name);

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
            // Tự động kiểm tra trạng thái bài test để ghi nhận vào báo cáo nếu có lỗi bất ngờ
            var status = TestContext.CurrentContext.Result.Outcome.Status;
            var stacktrace = string.IsNullOrEmpty(TestContext.CurrentContext.Result.StackTrace)
                ? ""
                : string.Format("<pre>{0}</pre>", TestContext.CurrentContext.Result.StackTrace);

            if (status == TestStatus.Failed)
            {
                test.Log(Status.Fail, "Bài test bị THẤT BẠI. Chi tiết lỗi: " + TestContext.CurrentContext.Result.Message);
                test.Log(Status.Fail, stacktrace);
            }

            // Giải phóng trình duyệt
            driver?.Dispose();
        }

        [OneTimeTearDown]
        public void GlobalTearDown()
        {
            // Ghi toàn bộ dữ liệu và xuất ra file HTML hoàn chỉnh
            extent.Flush();
        }
    }
}