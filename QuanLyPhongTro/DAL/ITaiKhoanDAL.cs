using QuanLyPhongTro.Entities;

namespace QuanLyPhongTro.DAL;

public interface ITaiKhoanDAL
{
    TaiKhoan? DangNhap(string tenDangNhap, string matKhau);
    bool DangKy(TaiKhoan t);
}
