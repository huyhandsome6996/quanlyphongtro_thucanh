# KỊCH BẢN BÁO CÁO GIỮA KỲ - ĐỒ ÁN QUẢN LÝ PHÒNG TRỌ

Dưới đây là kịch bản chi tiết kèm "lời mớm" để bạn của bạn có thể tự tin thuyết trình trước giảng viên. Hãy đọc với phong thái tự tin, rõ ràng.

---

## BƯỚC 1: MỞ ĐẦU (CHÀO HỎI VÀ GIỚI THIỆU)
*(Mở sẵn trang Đăng nhập hoặc Trang chủ của phần mềm lên màn hình)*

**🗣️ Lời nói:**
"Dạ em chào thầy/cô. Hôm nay nhóm em xin phép được trình bày báo cáo Giữa kỳ cho Đồ án **Phần mềm Quản lý Phòng trọ**. Mục tiêu của phần mềm là giúp chủ trọ quản lý số lượng phòng, thông tin khách thuê, lập hợp đồng và tính tiền điện nước, hóa đơn hàng tháng một cách hoàn toàn tự động và chính xác."

"Sau đây, em xin phép đi thẳng vào việc demo phần mềm để chứng minh đồ án của nhóm em đã đáp ứng đầy đủ 100% các tiêu chí mà thầy/cô đã đề ra trong Barem Giữa kỳ ạ."

---

## BƯỚC 2: DEMO VÀ ĐỐI CHIẾU TIÊU CHÍ GIỮA KỲ

*(Vừa nói vừa thao tác trực tiếp chuột trên màn hình)*

**1. Về Tiêu chí Tính thực thi & Trang trí (Favicon)**
**🗣️ Lời nói:**
"Như thầy/cô có thể thấy, phần mềm của nhóm em đã được build và chạy thành công, hoàn toàn không bị văng lỗi (crash). Nhóm em cũng đã thiết kế riêng biểu tượng (Favicon) hình tòa nhà cho phần mềm ở trên tab trình duyệt, thay vì dùng icon mặc định ạ."

**2. Về Tiêu chí Điều hướng và UI/UX cơ bản**
*(Thao tác: Click vào các menu trên Sidebar bên trái để nhảy qua lại giữa các trang)*
**🗣️ Lời nói:**
"Về mặt giao diện, nhóm em thiết kế bố cục theo dạng Sidebar Navigation để tối ưu UI/UX. Việc điều hướng từ trang chủ sang các chức năng khác cực kỳ mượt mà, người dùng không bị mất phương hướng khi sử dụng."

**3. Về Tiêu chí Số lượng Form**
*(Thao tác: Lần lượt mở nhanh các form)*
**🗣️ Lời nói:**
"Theo yêu cầu là tối thiểu 3-4 form quản lý riêng biệt, thì nhóm em đã phát triển tổng cộng **5 form quản lý cốt lõi** (không tính form Đăng nhập/Đăng ký hay Trang chủ). Đó là:
1. Form Quản lý Phòng trọ.
2. Form Quản lý Khách thuê.
3. Form Quản lý Hợp đồng.
4. Form Tính tiền điện nước hàng tháng.
5. Form Xem danh sách Hóa đơn."

**4. Về Tiêu chí Form Quan hệ N-N**
*(Thao tác: Mở form Quản lý Hợp đồng hoặc Tính tiền)*
**🗣️ Lời nói:**
"Đối với yêu cầu bắt buộc phải có Form quản lý quan hệ N-N (nhiều-nhiều), nhóm em đã xây dựng **Form Quản lý Hợp đồng** và **Hóa đơn**. Tại đây, một hợp đồng sẽ liên kết giữa đối tượng 'Phòng trọ' và 'Khách thuê'. Khi làm Hợp đồng, dữ liệu sẽ được map chuẩn xác từ 2 bảng kia sang. Hay ở form Hóa đơn, nó giải quyết bài toán tính tiền điện nước phát sinh hàng tháng cho từng Hợp đồng."

**5. Về Tiêu chí Chuẩn đặt tên Control (Naming Convention)**
*(Thao tác: Mở mã nguồn HTML/JS của 1 form lên (ví dụ quanly_phong.html) hoặc F12 Inspect)*
**🗣️ Lời nói:**
"Một điểm mà nhóm em rất chú trọng là tính chuẩn mực trong Code. Toàn bộ các thẻ Input, Combobox hay Button trong phần mềm đều được đặt ID tuân thủ nghiêm ngặt quy tắc tiền tố mà thầy đã dạy. 
Ví dụ: TextBox tụi em đặt là `txtHoTen`, `txtMaPhong`; Combobox là `cboLoaiPhong`; Nút bấm là `btnLuu`, `btnTimKiem`, `btnCloseModal`, `btnMenu`... Không có bất kỳ control nào đặt tên tùy tiện ạ."

*(Mở rộng thêm nếu thầy hỏi về tính an toàn)*: 
"Ngoài ra thì nhóm em cũng đã chuẩn bị sẵn nền tảng cho cuối kỳ như: Tất cả các thao tác CSDL đều dùng `try-catch-finally` để chống crash, bắt buộc có `connection.Close()`, và đã băm mật khẩu bảo mật (Hash Password) khi đăng nhập, đăng ký."

---

## BƯỚC 3: KẾT THÚC (MỜI GÓP Ý)
*(Quay lại màn hình Trang chủ)*

**🗣️ Lời nói:**
"Dạ, trên đây là toàn bộ phần demo báo cáo Giữa kỳ của nhóm em. Hệ thống đã đáp ứng được các yêu cầu căn bản về UI, số lượng form, liên kết dữ liệu và chuẩn coding. Em xin phép mời thầy/cô xem xét và cho nhóm em xin những lời nhận xét, góp ý để tụi em có thể hoàn thiện tốt hơn ở giai đoạn Cuối kỳ ạ. Em cảm ơn thầy/cô!"
