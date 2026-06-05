using QuanLyPhongTro.Entities;
using System.Collections.Generic;

namespace QuanLyPhongTro.DAL;

public interface IHoaDonDAL
{
    List<HoaDon> GetAll();
    HoaDon? GetById(string maHoaDon);
    List<HoaDon> GetByHopDong(string maHopDong);
    bool Insert(HoaDon h);
    bool ThanhToan(string maHoaDon);
    bool CheckExists(string maHopDong, string thangNam);
}
