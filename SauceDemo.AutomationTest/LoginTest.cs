using AventStack.ExtentReports;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SauceDemo.AutomationTest.Pages;
using System;
using System.IO;
using System.Threading;

namespace SauceDemo.AutomationTest
{
    [TestFixture]
    [Parallelizable(ParallelScope.Children)] 
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class LoginTest
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

        [SetUp]
        public void Setup()
        {
            var testInstance = ReportManager.GetExtentInstance().CreateTest(TestContext.CurrentContext.Test.Name);
            ReportManager.ThreadTest.Value = testInstance;

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
            ReportManager.ThreadTest.Value.Log(Status.Info, "Khởi động Chrome thành công cho luồng Login.");
        }

        //Đăng nhập thất bại với tài khoản sai
        [Test]
        [Category("Functional_Login")]
        public void TestLoginWithInvalidCredentials()
        {
            LoginPage loginPage = new LoginPage(driver);

            loginPage.Login("wrong_user", "wrong_password");
            ReportManager.ThreadTest.Value.Log(Status.Info, "Đã cố tình nhập sai tài khoản: wrong_user / wrong_password");

            string actualError = loginPage.GetErrorMessage();
            string expectedError = "Epic sadface: Username and password do not match any user in this service";

            Assert.That(actualError, Is.EqualTo(expectedError), "LỖI: Thông báo sai mật khẩu hiển thị không chính xác!");
        }

        [TearDown]
        public void CloseBrowser()
        {
            var status = TestContext.CurrentContext.Result.Outcome.Status;

            if (status == TestStatus.Failed)
            {
                PurchaseFlowTest.CaptureScreenshotOnFailure(driver, ReportManager.ThreadTest.Value);
            }

            driver?.Dispose();
        }
    }
}