using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using System;
using System.IO;
using System.Threading;

namespace SauceDemo.AutomationTest
{
    public static class ReportManager
    {
        private static ExtentReports _extent;
        private static readonly object _lock = new object();

        // Luồng bảo mật log độc lập cho từng Test Case ở mọi Class khác nhau
        public static ThreadLocal<ExtentTest> ThreadTest = new ThreadLocal<ExtentTest>();

        public static ExtentReports GetExtentInstance()
        {
            lock (_lock) // Đảm bảo chỉ khởi tạo một thực thể duy nhất dù nhiều class chạy cùng lúc
            {
                if (_extent == null)
                {
                    string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
                    string reportPath = Path.Combine(projectDirectory, "TestResults");
                    if (!Directory.Exists(reportPath)) Directory.CreateDirectory(reportPath);

                    var sparkReporter = new ExtentSparkReporter(Path.Combine(reportPath, "ExtentReport.html"));
                    _extent = new ExtentReports();
                    _extent.AttachReporter(sparkReporter);

                    _extent.AddSystemInfo("Environment", "QA - Production Simulation");
                    _extent.AddSystemInfo("Tester", "Quyen Nguyen");
                }
                return _extent;
            }
        }

        public static void FlushReport()
        {
            lock (_lock)
            {
                _extent?.Flush();
            }
        }
    }
}