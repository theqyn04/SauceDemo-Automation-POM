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
Solution 'SauceDemo.AutomationTest'
│
├── 📂 SauceDemo.AutomationTest (Test Runner Project)
│   ├── 🛠️ Dependencies (NUnit, Selenium, ExtentReports, Newtonsoft.Json)
│   ├── 📄 TestData.json               # Kho lưu trữ các bộ dữ liệu tài khoản kiểm thử (DDT)
│   ├── 📄 TestDataModel.cs            # Class ánh xạ cấu trúc đối tượng JSON sang C#
│   └── 📄 PurchaseFlowTest.cs         # Kịch bản kiểm thử động & Điều hướng luồng vòng đời
│
└── 📂 SauceDemo.AutomationTest.Pages (Page Object Library Project)
    ├── 🛠️ Dependencies (Selenium WebDriver)
    ├── 📄 LoginPage.cs                # Quản lý Đăng nhập (Username, Password, Login Button)
    ├── 📄 InventoryPage.cs            # Quản lý danh sách sản phẩm, chọn Balo và vào giỏ hàng
    ├── 📄 CheckoutPage.cs             # Điền thông tin cá nhân (First Name, Last Name, Zip Code)
    └── 📄 OverviewPage.cs             # Xác nhận đơn hàng, bấm Finish và kiểm tra kết quả hiển thị
```

## ⚡ Các Tính Năng & Kỹ Thuật Đã Triển Khai

### 1. Kiểm thử hướng dữ liệu (Data-Driven Testing với JSON & TestCaseSource)
Framework loại bỏ hoàn toàn việc hard-code tài khoản kiểm thử trong mã nguồn, chuyển sang cơ chế nạp dữ liệu động:
* **JSON Array Storage:** Toàn bộ danh sách tài khoản (`standard_user`, `problem_user`) cùng thông tin giao hàng tương ứng được lưu trữ tập trung tại file `TestData.json`.
* **Dynamic Test Generation:** Sử dụng hàm static kết hợp từ khóa `yield return` và thuộc tính `[TestCaseSource(nameof(LoadTestData))]` của NUnit để bốc dữ liệu lên. Hệ thống sẽ tự động nhân bản bài test thành nhiều lượt chạy độc lập (Ma trận Test) dựa theo số lượng phần tử cấu hình trong file JSON mà không cần viết lại code kịch bản.

### 2. Bộ lọc Chrome Options chuyên sâu (Advanced Browser Capabilities)
Để đối phó với các Popup hệ thống hoặc cảnh báo bảo mật làm gãy luồng chạy (*Popup Blocking*), hàm `[SetUp]` đã được cấu hình các tham số User Profile Preference nâng cao nhằm vô hiệu hóa hoàn toàn tính năng quét rò rỉ mật khẩu mặc định của Chrome (`password_manager_leak_detection`), đảm bảo con Bot chạy xuyên suốt không bị che khuất tầm nhìn.

### 3. Cơ chế đồng bộ hóa chống nghẽn (Synchronization & Anti-Flaky Tests)
* **Implicit Wait:** Cấu hình `TimeSpan.FromSeconds(5)` toàn cục để tạo thời gian chờ nền cho DOM tải phần tử.
* **Explicit Wait (WebDriverWait):** Sử dụng các điều kiện logic đặc hiệu như `Url.Contains` để chặn đầu các bước chuyển hướng trang (giữa trang Checkout và Overview), tránh lỗi vội vàng của Bot khi trình duyệt chưa Render xong.
* **JavaScript Executor:** Ép lệnh click trực tiếp lên các nút bấm cốt lõi thông qua Script để loại bỏ hoàn toàn rủi ro hụt click do giao diện bị lệch tọa độ hoặc bị che khuất ngầm.

### 4. Tự động xuất báo cáo kết quả trực quan (Advanced Test Reporting)
Tích hợp **ExtentReports (ExtentSparkReporter)** quản lý vòng đời chặt chẽ thông qua `[OneTimeSetUp]` và `[OneTimeTearDown]`:
* Tự động sinh Node kiểm thử động theo tên tài khoản đang chạy (`SetName`).
* Bắt gọn trạng thái lỗi (`TestStatus.Failed`) kèm toàn bộ dữ liệu vầy vết Stack Trace định dạng thẻ `<pre>` dán trực tiếp vào file báo cáo HTML `ExtentReport.html` trực quan giúp dễ dàng Debug.

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
