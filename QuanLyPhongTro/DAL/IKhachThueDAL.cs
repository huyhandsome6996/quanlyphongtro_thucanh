using QuanLyPhongTro.Entities;
using System.Collections.Generic;

namespace QuanLyPhongTro.DAL;

public interface IKhachThueDAL
{
    List<KhachThue> GetAll();
    KhachThue? GetById(string maKhach);
    bool Insert(KhachThue k);
    bool Update(KhachThue k);
    bool Delete(string maKhach);
    bool CheckCCCDExists(string cccd, string excludeMaKhach);
}
