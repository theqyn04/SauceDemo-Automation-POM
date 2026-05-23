using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SauceDemo.AutomationTest.Pages;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace SauceDemo.AutomationTest
{
    [TestFixture]
    [Parallelizable(ParallelScope.Children)]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class PurchaseFlowTest
    {
        private IWebDriver driver;
        private static IConfiguration _config;

        [OneTimeSetUp]
        public static void GlobalSetup()
        {
            _config = new ConfigurationBuilder()
                            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                            .Build();

            ReportManager.GetExtentInstance();
        }

        // HÀM ĐỌC DATA TỪ JSON: Chuyển đổi file JSON thành danh sách các TestCase dữ liệu
        public static IEnumerable<TestCaseData> LoadTestData()
        {
            string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData.json");
            if (!File.Exists(jsonFilePath))
            {
                throw new FileNotFoundException($"Không tìm thấy file cấu hình test dữ liệu tại: {jsonFilePath}");
            }

            string jsonContent = File.ReadAllText(jsonFilePath);
            var dataList = JsonConvert.DeserializeObject<List<TestDataModel>>(jsonContent);

            foreach (var data in dataList)
            {
                // Trả về từng bộ dữ liệu và đặt tên hiển thị động trên Test Explorer theo tên Username
                yield return new TestCaseData(data).SetName($"Test_With_User_{data.Username}");
            }
        }

        [SetUp]
        public void Setup()
        {
            var testInstance = ReportManager.GetExtentInstance().CreateTest(TestContext.CurrentContext.Test.Name);
            ReportManager.ThreadTest.Value = testInstance;

            // Đọc dữ liệu môi trường động từ file cấu hình appsettings.json
            string baseUrl = _config["AppSettings:BaseUrl"];
            int timeoutSeconds = int.Parse(_config["AppSettings:ImplicitWaitSeconds"]);

            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--disable-extensions");
            options.AddUserProfilePreference("profile.password_manager_leak_detection", false);
            options.AddUserProfilePreference("credentials_enable_service", false);
            options.AddUserProfilePreference("profile.password_manager_enabled", false);

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GITHUB_ACTIONS")))
            {
                options.AddArgument("--headless=new");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--disable-dev-shm-usage");
            }

            driver = new ChromeDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(timeoutSeconds);
            driver.Navigate().GoToUrl(baseUrl);
            ReportManager.ThreadTest.Value.Log(Status.Info, $"Chrome khởi động. Môi trường: {baseUrl}");
        }

        //Ép NUnit lặp lại bài test dựa trên nguồn hàm LoadTestData
        [Test]
        [TestCaseSource(nameof(LoadTestData))]
        public void TestPurchaseFlowWithPOM(TestDataModel data)
        {
            LoginPage loginPage = new LoginPage(driver);
            InventoryPage inventoryPage = new InventoryPage(driver);
            CheckoutPage checkoutPage = new CheckoutPage(driver);
            OverviewPage overviewPage = new OverviewPage(driver);

            // Bốc dữ liệu từ tham số `data` được truyền động vào bài test
            loginPage.Login(data.Username, data.Password);
            ReportManager.ThreadTest.Value.Log(Status.Info, $"Đăng nhập thành công với tài khoản: {data.Username}");

            inventoryPage.AddBackpackToCartAndGoToCart();
            ReportManager.ThreadTest.Value.Log(Status.Info, "Đã chọn sản phẩm và vào giỏ hàng.");

            checkoutPage.ClickCheckout();

            // Luồng chạy sẽ lấy thông tin FirstName, LastName động từ JSON
            checkoutPage.EnterInformationAndContinue(data.FirstName, data.LastName, data.PostalCode);
            ReportManager.ThreadTest.Value.Log(Status.Info, $"Điền thông tin Checkout: {data.FirstName} {data.LastName}");

            overviewPage.ClickFinish();
            ReportManager.ThreadTest.Value.Log(Status.Info, "Bấm Finish xác nhận đơn hàng.");

            string actualMessage = overviewPage.GetCompleteMessage();

            Assert.That(actualMessage, Is.EqualTo(data.ExpectedMessage), "LỖI: Thông báo hiển thị không khớp!");
            ReportManager.ThreadTest.Value.Log(Status.Pass, $"Kết quả kiểm thử cho user {data.Username} ĐẠT!");
        }

        [TearDown]
        public void CloseBrowser()
        {
            var status = TestContext.CurrentContext.Result.Outcome.Status;

            if (status == TestStatus.Failed)
            {
                // Gọi hàm helper dùng chung ngay tại nội bộ class
                CaptureScreenshotOnFailure(driver, ReportManager.ThreadTest.Value);
            }

            driver?.Dispose();
        }

        public static void CaptureScreenshotOnFailure(IWebDriver currentDriver, ExtentTest currentTest)
        {
            string errorMessage = TestContext.CurrentContext.Result.Message;
            string stacktrace = string.IsNullOrEmpty(TestContext.CurrentContext.Result.StackTrace)
                ? ""
                : string.Format("<pre>{0}</pre>", TestContext.CurrentContext.Result.StackTrace);

            string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
            string screenshotFolder = Path.Combine(projectDirectory, "TestResults", "Screenshots");

            if (!Directory.Exists(screenshotFolder)) Directory.CreateDirectory(screenshotFolder);

            string testName = TestContext.CurrentContext.Test.Name;
            string screenshotPath = Path.Combine(screenshotFolder, $"{testName}.png");

            try
            {
                var screenshotDriver = (ITakesScreenshot)currentDriver;
                var screenshot = screenshotDriver.GetScreenshot();
                screenshot.SaveAsFile(screenshotPath);

                currentTest.Log(Status.Fail, "Bài test bị THẤT BẠI: " + errorMessage);
                currentTest.Log(Status.Fail, "Ảnh chụp màn hình thời điểm lỗi xảy ra:",
                    MediaEntityBuilder.CreateScreenCaptureFromPath(Path.Combine("Screenshots", $"{testName}.png")).Build());
                currentTest.Log(Status.Fail, stacktrace);
            }
            catch (Exception ex)
            {
                currentTest.Log(Status.Warning, "Không thể chụp ảnh màn hình: " + ex.Message);
            }
        }

        [OneTimeTearDown]
        public static void GlobalTearDown()
        {
            ReportManager.FlushReport();
        }
    }
}