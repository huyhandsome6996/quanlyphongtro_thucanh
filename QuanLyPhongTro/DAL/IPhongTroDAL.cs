using QuanLyPhongTro.Entities;
using System.Collections.Generic;

namespace QuanLyPhongTro.DAL;

public interface IPhongTroDAL
{
    List<PhongTro> GetAll();
    PhongTro? GetById(string maPhong);
    bool Insert(PhongTro p);
    bool Update(PhongTro p);
    bool Delete(string maPhong);
    bool UpdateTrangThai(string maPhong, string trangThai);
}
