# 🚀 SauceDemo Automation Testing Framework (C# & Selenium WebDriver)

Một Framework kiểm thử tự động toàn trình (End-to-End Purchase Flow) được xây dựng từ con số 0 trên trang thương mại điện tử giả lập **SauceDemo**. Dự án áp dụng mô hình thiết kế **Page Object Model (POM)** tách biệt ở cấp độ Project, kết hợp kỹ thuật **Data-Driven Testing (DDT)** đọc dữ liệu động từ file cấu hình ngoài để tối ưu hóa kịch bản kiểm thử ma trận.

## 🛠️ Công Nghệ & Thư Viện Sử Dụng

* **Ngôn ngữ lập trình:** C# (.NET 8.0)
* **Automation Tool:** Selenium WebDriver (v4.x)
* **Test Framework:** NUnit (v3.x)
* **Reporting Tool:** ExtentReports (ExtentSparkReporter v5.x)
* **Data Parsing:** Newtonsoft.Json (v13.x)
* **Design Pattern:** Page Object Model (POM) với Kiến trúc Đa dự án (Multi-project Solution)
* **IDE & Version Control:** Visual Studio 2022, Git & GitHub

---

## 📐 Kiến Trúc Dự Án (Multi-Project POM Structure)

Mã nguồn được tổ chức chặt chẽ thành 2 project riêng biệt nhằm phân định rõ ràng vai trò giữa logic kiểm thử và định nghĩa thành phần giao diện:

```text
SauceDemo.Automation/
├── .github/
│   └── workflows/
│       └── dotnet-test.yml                  # Pipeline CI/CD tự động chạy test trên mây (GitHub Actions)
│
├── SauceDemo.AutomationTest.Pages/          # [PROJECT LAYER 1] - TẦNG ĐỊNH NGHĨA PHẦN TỬ & TƯƠNG TÁC UI
│   ├── Pages/
│   │   ├── LoginPage.cs                     # Quản lý Locators và hành vi trang Đăng nhập (Login)
│   │   ├── InventoryPage.cs                 # Quản lý danh sách sản phẩm và giỏ hàng sơ cấp
│   │   ├── CartPage.cs                      # Quản lý thông tin và kiểm tra giỏ hàng
│   │   ├── CheckoutPage.cs                  # Quản lý form điền thông tin khách hàng mua hàng
│   │   └── OverviewPage.cs                  # Quản lý trang xác nhận đơn hàng cuối cùng
│   └── SauceDemo.AutomationTest.Pages.csproj
│
├── SauceDemo.AutomationTest/                # [PROJECT LAYER 2] - TẦNG KỊCH BẢN, DATA-DRIVEN & VẬN HÀNH
│   ├── LoginTest.cs                         # Kịch bản Functional Test (Đăng nhập đúng/sai)
│   ├── PurchaseFlowTest.cs                  # Kịch bản End-to-End (E2E) Test luồng mua hàng toàn trình
│   ├── ReportManager.cs                     # Bộ quản lý ExtentReports tập trung (Thread-Safe bảo mật đa luồng)
│   ├── TestDataModel.cs                     # Model C# định nghĩa kiểu dữ liệu để ánh xạ từ file JSON
│   ├── TestData.json                        # Ma trận dữ liệu đầu vào động (Data-Driven Testing)
│   ├── appsettings.json                     # Quản lý cấu hình môi trường toàn cục (URL, Timeout, Browser)
│   ├── TestResults/                         # Thư mục tự động sinh ra sau khi chạy test
│   │   ├── ExtentReport.html                # Báo cáo Dashboard đồ họa hợp nhất tổng thể
│   │   └── Screenshots/                     # Nơi lưu trữ ảnh chụp màn hình tự động khi có kịch bản bị lỗi (Fail)
│   └── SauceDemo.AutomationTest.csproj
│
└── SauceDemo.sln                            # Tập tin Solution liên kết, quản lý chung toàn bộ hệ thống
```


## ⚡ Các Tính Năng & Kỹ Thuật Nổi Bật (Level 4 - Enterprise Ready)

Hệ thống Automation Testing Framework được thiết kế và tối ưu hóa dựa trên các tiêu chuẩn kiến trúc phần mềm nghiêm ngặt trong môi trường doanh nghiệp lớn. Dưới đây là phân tích kỹ thuật chuyên sâu về 7 trục tính năng cốt lõi giúp framework đạt trạng thái **Enterprise Ready**:

---

### 1. Kiến Trúc Đa Dự Án (Multi-Project Page Object Model)
* **Bản chất công nghệ:** Phân tách hệ thống thành 2 Class Library/Project độc lập trong cùng một Visual Studio Solution: `SauceDemo.AutomationTest.Pages` (chứa Locators và tương tác UI sơ cấp) và `SauceDemo.AutomationTest` (chứa Runner, cấu hình vòng đời kịch bản và Assertions).
* **Cơ chế hoạt động:** Tầng kịch bản kiểm thử muốn tương tác với ứng dụng bắt buộc phải thông qua các hàm hành vi được đóng gói sẵn của tầng Pages. Nguyên tắc này ngăn chặn tuyệt đối việc sử dụng trực tiếp các đối tượng WebDriver (`driver.FindElement`) trong file Test.
* **Giá trị thực tế:** * Khắc phục triệt để lỗi trùng lặp mã nguồn (*Code Duplication*). Khi UI hệ thống thay đổi (ví dụ: Thay đổi thuộc tính `Id` của nút Login), kỹ sư chỉ cần cập nhật tại một vị trí duy nhất trong tập tin Page tương ứng, toàn bộ ma trận hàng trăm kịch bản test tự động chạy đúng mà không cần sửa đổi.
  * Tăng tính mô-đun hóa, cho phép nhiều kỹ sư QA cùng tham gia đóng góp mã nguồn vào các cấu trúc Page độc lập mà không gây xung đột (Git Conflicts).

---

### 2. Kiểm Thử Hướng Dữ Liệu Động (Data-Driven Testing)
* **Bản chất công nghệ:** Cô lập hoàn toàn lớp dữ liệu kiểm thử (Test Data) ra khỏi lớp xử lý logic mã nguồn bằng cấu trúc mảng đối tượng trong tập tin ngoại vi **`TestData.json`**.
* **Cơ chế hoạt động:** * Sử dụng thư viện **`Newtonsoft.Json`** để bóc tách cấu trúc dữ liệu thô từ file JSON.
  * Kết hợp thuộc tính **`[TestCaseSource(nameof(LoadTestData))]`** của NUnit framework đóng vai trò như một bộ nạp dữ liệu động (Data Provider). Tại thời điểm runtime, phương thức sẽ thực hiện vòng lặp `yield return new TestCaseData(data)` để giải nén mảng JSON, tự động khởi tạo và gán nhãn (`SetName`) động cho từng Test Case riêng biệt trong Test Explorer.
* **Giá trị thực tế:** Cho phép mở rộng kịch bản kiểm thử theo cấp số nhân. Kỹ sư QA có thể thực thi thêm hàng loạt kịch bản kiểm thử (kiểm thử biên, kiểm thử tài khoản lỗi, kiểm thử SQL Injection...) chỉ bằng cách thêm các dòng dữ liệu mới vào file JSON mà không phải viết thêm bất kỳ dòng code C# nào.

---

### 3. Tối Ưu Hiệu Năng Với Chạy Test Song Song (Parallel Execution)
* **Bản chất công nghệ:** Định cấu hình kiến trúc kiểm thử đa luồng (Multi-threading) cấp độ phần cứng thông qua thuộc tính toàn cục **`[Parallelizable(ParallelScope.Children)]`** và quản lý vòng đời **`[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]`**.
* **Cơ chế hoạt động:** NUnit Framework sẽ can thiệp vào tiến trình cấp phát tài nguyên hệ thống. Thay vì chạy tuần tự từ trên xuống dưới, NUnit sẽ tính toán số lượng lõi CPU (CPU Cores) khả dụng của máy để khởi tạo các luồng thực thi (Threads) đồng thời. Thuộc tính `InstancePerTestCase` ép buộc hệ thống phải tạo ra một thực thể (Instance) class kiểm thử mới cùng một Driver Chrome độc lập cho từng bộ dữ liệu, cô lập tuyệt đối bộ nhớ RAM giữa các bài test.
* **Giá trị thực tế:** Rút ngắn tổng thời gian thực thi toàn bộ Test Suite xuống hơn **50% - 70%** (tùy thuộc số luồng CPU cấu hình). Giải quyết triệt để bài toán thắt nút cổ chai hiệu năng khi số lượng kịch bản kiểm thử tăng lên hàng trăm bài.

---

### 4. Cơ Chế Báo Cáo Hợp Nhất An Toàn Luồng (Thread-Safe Reporting)
* **Bản chất công nghệ:** Thiết kế Class **`ReportManager`** tập trung dựa trên mẫu thiết kế Thread-Safe Singleton, kết hợp lớp bọc dữ liệu luồng **`ThreadLocal<ExtentTest>`** và khối lệnh khóa đồng bộ **`lock(object)`**.
* **Cơ chế hoạt động:** Khi chạy song song đa luồng, các tiến trình kiểm thử sẽ liên tục gửi log về file báo cáo. Nếu không có cơ chế bảo mật luồng, hiện tượng tranh chấp tài nguyên (*Race Condition*) sẽ xảy ra, dẫn đến log luồng này ghi đè lên log luồng khác gây lỗi crash file HTML. `ThreadLocal<ExtentTest>` đảm bảo mỗi Thread CPU chỉ được quyền đọc/ghi trên một thực thể vùng nhớ log được cấp phát riêng cho nó. Khối lệnh `lock` kiểm soát việc ghi dữ liệu đầu ra về tập tin cấu trúc đồ họa **ExtentReports (ExtentSparkReporter)** diễn ra tuần tự một cách an toàn.
* **Giá trị thực tế:** Kết xuất ra một file báo cáo tĩnh `ExtentReport.html` hợp nhất duy nhất cực kỳ chuyên nghiệp. Toàn bộ kết quả, tiến trình, thời gian chạy chi tiết của tất cả các Class Test chạy song song đều được phân tách, hiển thị trực quan theo từng tab riêng biệt mà không bị mất mát hay lẫn lộn log dữ liệu chéo.

---

### 5. Quản Lý Cấu Hình Môi Trường Tập Trung (Environment Setup)
* **Bản chất công nghệ:** Sử dụng thư viện tiêu chuẩn doanh nghiệp **`Microsoft.Extensions.Configuration`** để xây dựng cơ chế nạp cấu hình hệ thống từ file cấu hình **`appsettings.json`**.
* **Cơ chế hoạt động:** Sử dụng `ConfigurationBuilder` để trỏ trực tiếp vào thư mục build cơ sở của dự án, thiết lập thuộc tính `reloadOnChange: true` để nạp động các tham số cấu hình hệ thống ngay tại chu kỳ `[OneTimeSetUp]`. Toàn bộ thông tin nhạy cảm và tham số vận hành như địa chỉ URL vùng chạy (`BaseUrl`), thời gian đợi tải phần tử (`ImplicitWaitSeconds`), và loại trình duyệt điều khiển đều được bốc tách ra khỏi code.
* **Giá trị thực tế:** * Loại bỏ hoàn toàn lỗi "Mã nguồn cứng" (*Hard-coded Strings*). 
  * Cho phép kỹ sư kiểm thử linh hoạt chuyển đổi toàn bộ hệ thống kiểm thử chạy qua lại giữa các môi trường (Dev, Staging, UAT, Production) chỉ bằng cách sửa đổi 1 dòng text trong file JSON cấu hình, hoàn toàn không cần phải mở file code C# hay thực hiện biên dịch lại (*Recompile*) dự án.

---

### 6. Tự Động Chụp Ảnh Màn Hình Khi Lỗi (Auto Screenshot on Failure)
* **Bản chất công nghệ:** Tích hợp bộ lọc kiểm tra trạng thái vòng đời kiểm thử của NUnit (`TestContext.CurrentContext.Result.Outcome.Status`) kết hợp cơ chế ép kiểu chụp ảnh giao diện đồ họa **`ITakesScreenshot`**.
* **Cơ chế hoạt động:** Tại chu kỳ giải phóng trình duyệt `[TearDown]`, hệ thống tự động kiểm tra xem kịch bản kiểm thử vừa chạy có kết quả là `TestStatus.Failed` hay không. Nếu phát hiện lỗi, hệ thống sẽ kích hoạt Driver thực hiện lệnh `GetScreenshot().SaveAsFile()`, tự động đặt tên file theo tên của Test Case lỗi, lưu vào thư mục `TestResults/Screenshots/` và chèn link đường dẫn tương đối trực tiếp vào thẻ HTML của ExtentReport.
* **Giá trị thực tế:** Hỗ trợ tối đa cho quy trình Debug lỗi của các lập trình viên. Khi kịch bản kiểm thử chạy ngầm (hoặc chạy ban đêm trên CI/CD) bị tạch, kỹ sư chỉ cần mở file báo cáo là có thể nhìn thấy ngay ảnh chụp trực quan của giao diện ứng dụng tại chính xác mili-giây xảy ra lỗi, kèm theo chuỗi Stack Trace chi tiết giúp xác định nguyên nhân gây lỗi (như lỗi Dev thả Bug, lỗi mạng lag, hay lỗi UI) trong vòng vài giây.

---

### 7. Tích Hợp Liên Tục Toàn Trình (CI/CD với GitHub Actions)
* **Bản chất công nghệ:** Xây dựng Pipeline tự động hóa tích hợp liên tục (CI) dựa trên chuẩn YAML (`.github/workflows/dotnet-test.yml`).
* **Cơ chế hoạt động:** * Mỗi khi có hành động `git push` hoặc duyệt một `pull request` vào nhánh `main`, máy chủ GitHub sẽ tự động kích hoạt một Workflow chạy trên máy ảo Linux (`ubuntu-latest`).
  * Hệ thống tự động thiết lập môi trường .NET 8 SDK, khôi phục các gói thư viện NuGet (`dotnet restore`) và biên dịch hệ thống.
  * Hệ thống tự động nhận diện môi trường CI thông qua biến `Environment.GetEnvironmentVariable("GITHUB_ACTIONS")` để ép Chrome chạy ở chế độ **Headless Mode** (Chạy ngầm không giao diện UI). 
  * Kết thúc tiến trình, GitHub Actions kích hoạt gói `upload-artifact` để đóng gói toàn bộ thư mục `TestResults` (gồm ExtentReport và các file ảnh chụp lỗi Screenshots) thành một file `.zip` lưu trữ trực tiếp trên đám mây.
* **Giá trị thực tế:** Giải phóng hoàn toàn sức lao động của con người. Bộ kịch bản kiểm thử tự động biến thành một "người gác cổng" trung thành cho dự án. Bất kỳ một đoạn code mới nào của Dev đẩy lên làm hỏng tính năng cũ (Regression Bug) đều sẽ bị Pipeline phát hiện, chặn lại và cảnh báo ngay lập tức, đảm bảo độ ổn định tuyệt đối cho sản phẩm trước khi Deploy.

---

## 🏃‍♂️ Hướng Dẫn Khởi Chạy Dự Án

### 1. Thực thi kịch bản kiểm thử (Execute Test)
Mở Terminal tại thư mục gốc Solution và thực hiện lệnh:
```bash
dotnet test
```

## 📝 Kết Quả Kiểm Thử Đạt Được

* **Total Tests:** 2 (Chạy lặp động theo cấu trúc file cấu hình JSON)
* **Passed:** 1 (`Test_With_User_standard_user` - Hoàn thành toàn bộ luồng mua hàng mượt mà chỉ trong 2.2 giây).
* **Failed:** 1 (`Test_With_User_problem_user` - Bị chặn đứng và bắt lỗi chính xác tại bước nhập thông tin Checkout do tài khoản giả lập bị lỗi logic ứng dụng).
* **Báo cáo đầu ra:** File `ExtentReport.html` sinh ra tại thư mục `TestResults/` hiển thị biểu đồ phân tích tỷ lệ Pass/Fail sắc nét.
