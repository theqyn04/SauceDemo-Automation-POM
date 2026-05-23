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
            // Cấu hình đường dẫn xuất báo cáo trực tiếp ra thư mục gốc Solution
            string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
            string reportPath = Path.Combine(projectDirectory, "TestResults");
            if (!Directory.Exists(reportPath)) Directory.CreateDirectory(reportPath);

            var sparkReporter = new ExtentSparkReporter(Path.Combine(reportPath, "ExtentReport.html"));
            extent = new ExtentReports();
            extent.AttachReporter(sparkReporter);

            extent.AddSystemInfo("Environment", "QA - Production Simulation");
            extent.AddSystemInfo("Tester", "Quyen Nguyen");
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
            // Khởi tạo node test trong ExtentReports với tên cụ thể của từng Username đang chạy
            test = extent.CreateTest(TestContext.CurrentContext.Test.Name);

            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--disable-extensions");

            //Tắt hoàn toàn Popup cảnh báo lộ mật khẩu của Chrome
            options.AddUserProfilePreference("profile.password_manager_leak_detection", false);
            options.AddUserProfilePreference("credentials_enable_service", false);
            options.AddUserProfilePreference("profile.password_manager_enabled", false);

            driver = new ChromeDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.Navigate().GoToUrl("https://www.saucedemo.com/");
            test.Log(Status.Info, "Trình duyệt Chrome khởi động thành công.");
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
            test.Log(Status.Info, $"Đăng nhập thành công với tài khoản: {data.Username}");

            inventoryPage.AddBackpackToCartAndGoToCart();
            test.Log(Status.Info, "Đã chọn sản phẩm và vào giỏ hàng.");

            checkoutPage.ClickCheckout();

            // Luồng chạy sẽ lấy thông tin FirstName, LastName động từ JSON
            checkoutPage.EnterInformationAndContinue(data.FirstName, data.LastName, data.PostalCode);
            test.Log(Status.Info, $"Điền thông tin Checkout: {data.FirstName} {data.LastName}");

            overviewPage.ClickFinish();
            test.Log(Status.Info, "Bấm Finish xác nhận đơn hàng.");

            string actualMessage = overviewPage.GetCompleteMessage();

            Assert.That(actualMessage, Is.EqualTo(data.ExpectedMessage), "LỖI: Thông báo hiển thị không khớp!");
            test.Log(Status.Pass, $"Kết quả kiểm thử cho user {data.Username} ĐẠT!");
        }

        [TearDown]
        public void CloseBrowser()
        {
            var status = TestContext.CurrentContext.Result.Outcome.Status;

            if (status == TestStatus.Failed)
            {
                //Lấy thông báo lỗi và Stack Trace
                string errorMessage = TestContext.CurrentContext.Result.Message;
                string stacktrace = string.IsNullOrEmpty(TestContext.CurrentContext.Result.StackTrace)
                    ? ""
                    : string.Format("<pre>{0}</pre>", TestContext.CurrentContext.Result.StackTrace);

                //Định nghĩa thư mục lưu trữ ảnh chụp màn hình (Nằm trong thư mục TestResults)
                string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
                string screenshotFolder = Path.Combine(projectDirectory, "TestResults", "Screenshots");

                if (!Directory.Exists(screenshotFolder))
                {
                    Directory.CreateDirectory(screenshotFolder);
                }

                //Tiến hành chụp ảnh màn hình thực tế thông qua ITakesScreenshot
                string testName = TestContext.CurrentContext.Test.Name; // Ví dụ: Test_With_User_problem_user
                string screenshotPath = Path.Combine(screenshotFolder, $"{testName}.png");

                try
                {
                    var screenshotDriver = (ITakesScreenshot)driver;
                    var screenshot = screenshotDriver.GetScreenshot();
                    screenshot.SaveAsFile(screenshotPath);

                    //Ghi vết lỗi và đính kèm bức ảnh trực tiếp vào file báo cáo ExtentReports
                    test.Log(Status.Fail, "Bài test bị THẤT BẠI: " + errorMessage);
                    test.Log(Status.Fail, "Ảnh chụp màn hình thời điểm lỗi xảy ra:",
                        MediaEntityBuilder.CreateScreenCaptureFromPath(Path.Combine("Screenshots", $"{testName}.png")).Build());
                    test.Log(Status.Fail, stacktrace);
                }
                catch (Exception ex)
                {
                    test.Log(Status.Warning, "Không thể chụp ảnh màn hình do: " + ex.Message);
                }
            }

            // Đóng trình duyệt giải phóng bộ nhớ
            driver?.Dispose();
        }

        [OneTimeTearDown]
        public void GlobalTearDown()
        {
            extent.Flush();
        }
    }
}