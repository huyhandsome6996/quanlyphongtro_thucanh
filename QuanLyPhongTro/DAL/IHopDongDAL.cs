using QuanLyPhongTro.Entities;
using System.Collections.Generic;

namespace QuanLyPhongTro.DAL;

public interface IHopDongDAL
{
    List<HopDong> GetAll();
    HopDong? GetById(string maHopDong);
    HopDong? GetByPhong(string maPhong);
    bool Insert(HopDong h);
    bool Update(HopDong h);
    bool ThanhLy(string maHopDong);
    List<HopDong> GetActiveContracts();
}
