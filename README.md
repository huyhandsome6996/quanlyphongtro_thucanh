# 🏢 Hệ thống Quản lý Phòng Trọ

Đồ án môn học: Hệ thống Quản lý Phòng Trọ được xây dựng theo kiến trúc 3 tầng (3-tier) hướng đối tượng.

## 🛠️ Công nghệ sử dụng

- **Backend:** C# ASP.NET Web API (.NET 8)
- **Frontend:** HTML5, CSS3, JavaScript (Vanilla JS)
- **Cơ sở dữ liệu:** SQLite
- **Kiến trúc:** 3-tier (Entities - DAL - Controllers)

## 📁 Cấu trúc thư mục

```
QuanLyPhongTro/
├── Program.cs                  # Entry point, cấu hình Web API + Desktop Mode
├── DatabaseInitializer.cs      # Khởi tạo database và seed data
├── appsettings.json            # Cấu hình kết nối và giá điện/nước
├── Entities/                   # Lớp thực thể
│   ├── TaiKhoan.cs
│   ├── PhongTro.cs
│   ├── KhachThue.cs
│   ├── HopDong.cs
│   └── HoaDon.cs
├── DAL/                        # Data Access Layer
│   ├── DatabaseHelper.cs       # Helper kết nối database
│   ├── ITaiKhoanDAL.cs        # Interface (bắt buộc theo yêu cầu)
│   ├── TaiKhoanDAL.cs         # Implementation
│   ├── IPhongTroDAL.cs
│   ├── PhongTroDAL.cs
│   ├── IKhachThueDAL.cs
│   ├── KhachThueDAL.cs
│   ├── IHopDongDAL.cs
│   ├── HopDongDAL.cs
│   ├── IHoaDonDAL.cs
│   └── HoaDonDAL.cs
├── Controllers/                # API Controllers
│   ├── TaiKhoanController.cs
│   ├── PhongTroController.cs
│   ├── KhachThueController.cs
│   ├── HopDongController.cs
│   └── HoaDonController.cs
└── wwwroot/                    # Frontend
    ├── login.html
    ├── index.html
    ├── quanly_phong.html
    ├── quanly_khach.html
    ├── quanly_hopdong.html
    ├── tinh_tien.html
    ├── xem_hoadon.html
    ├── css/style.css
    ├── js/
    │   ├── login.js
    │   ├── home.js
    │   ├── phong.js
    │   ├── khach.js
    │   ├── hopdong.js
    │   ├── tinhtien.js
    │   └── hoadon.js
    └── img/favicon.svg
```

## 📊 Cơ sở dữ liệu (SQLite)

### Các bảng:

1. **TAI_KHOAN** - Tài khoản đăng nhập (TenDangNhap PK, MatKhau Hash SHA256, HoTen)
2. **PHONG_TRO** - Phòng trọ (MaPhong PK, TenPhong, LoaiPhong, GiaThue, TrangThai)
3. **KHACH_THUE** - Khách thuê (MaKhach PK, HoTen, CCCD UNIQUE, SoDienThoai)
4. **HOP_DONG** - Hợp đồng thuê (MaHopDong PK, MaPhong FK, MaKhach FK, NgayBatDau, TienCoc, TrangThaiHD)
5. **HOA_DON** - Hóa đơn điện nước (MaHoaDon PK, MaHopDong FK, ThangNam, SoDienCu/Moi, SoNuocCu/Moi, TongTien, TrangThaiThanhToan)

### Dữ liệu mẫu:
- Tài khoản: `admin` / `123456`
- 5 phòng trọ (P001-P005)
- 3 khách thuê (KT001-KT003)
- 2 hợp đồng mẫu (HD001, HD002)

## ✅ Các yêu cầu đã đáp ứng

### Kiến trúc & Bảo mật
- [x] Kiến trúc 3 tầng: Entities - DAL - Controllers
- [x] Mọi DAL class đều có Interface tương ứng
- [x] SQL Parameterized Query (tuyệt đối không cộng chuỗi)
- [x] try-catch-finally trong mọi hàm DAL, connection.Close() trong finally
- [x] Transaction khi lập hợp đồng (cập nhật trạng thái phòng)
- [x] Transaction khi thanh lý hợp đồng (cập nhật trạng thái phòng)
- [x] Mật khẩu được Hash SHA256, không lưu plain-text
- [x] Kiểm tra lỗi rỗng, lỗi trùng khóa chính, trùng CCCD

### Giao diện
- [x] Desktop Mode: Process.Start gọi Chrome/Edge --app
- [x] Quy tắc đặt tên Control: txt (Textbox), cbo (Combobox), btn (Button), dtp (DatePicker)
- [x] UI/UX đẹp mắt, màu sắc chuyên nghiệp
- [x] Custom Favicon (SVG)
- [x] Form Modal cho Thêm/Sửa
- [x] Form Đăng nhập

### Chức năng
- [x] Đăng nhập (admin / 123456)
- [x] Trang chủ với bảng điều khiển thống kê
- [x] Quản lý Phòng Trọ (Thêm/Sửa/Xóa/Tìm kiếm)
- [x] Quản lý Khách Thuê (Thêm/Sửa/Xóa/Tìm kiếm, kiểm tra trùng CCCD)
- [x] Quản lý Hợp Đồng (Lập hợp đồng, Thanh lý với transaction)
- [x] Tính tiền điện nước (Tự động tính: Tiền phòng + Tiền điện + Tiền nước)
- [x] Xem hóa đơn (Lọc, Chi tiết, Thanh toán)

## 🚀 Hướng dẫn chạy dự án

### Yêu cầu:
- .NET 8 SDK
- Trình duyệt Chrome hoặc Edge

### Chạy dự án:
```bash
cd QuanLyPhongTro
dotnet restore
dotnet run
```

Ứng dụng sẽ tự động mở trình duyệt ở chế độ Desktop App.

### Đăng nhập:
- Tên đăng nhập: `admin`
- Mật khẩu: `123456`

## 📝 Công thức tính tiền

```
Tổng tiền = Tiền phòng + Tiền điện + Tiền nước

Trong đó:
- Tiền điện = (Số điện mới - Số điện cũ) × 3.500 đ/kWh
- Tiền nước = (Số nước mới - Số nước cũ) × 15.000 đ/m³
- Tiền phòng = Giá thuê phòng (từ bảng PHONG_TRO)
```

## 👨‍🎓 Thông tin sinh viên

Đồ án môn học - Quản lý phòng trọ
