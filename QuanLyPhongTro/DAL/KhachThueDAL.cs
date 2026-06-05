using System;
using System.Data.SQLite;
using System.Collections.Generic;
using QuanLyPhongTro.Entities;

namespace QuanLyPhongTro.DAL;

public class KhachThueDAL : IKhachThueDAL
{
    private readonly DatabaseHelper _dbHelper;

    public KhachThueDAL(DatabaseHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public List<KhachThue> GetAll()
    {
        SQLiteConnection? connection = null;
        try
        {
            connection = _dbHelper.GetConnection();
            connection.Open();
            string sql = "SELECT MaKhach, HoTen, CCCD, SoDienThoai FROM KHACH_THUE";
            using var command = new SQLiteCommand(sql, connection);
            using var reader = command.ExecuteReader();
            var list = new List<KhachThue>();
            while (reader.Read())
            {
                list.Add(new KhachThue
                {
                    MaKhach = reader.GetString(0),
                    HoTen = reader.GetString(1),
                    CCCD = reader.GetString(2),
                    SoDienThoai = reader.GetString(3)
                });
            }
            return list;
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            connection?.Close();
        }
    }

    public KhachThue? GetById(string maKhach)
    {
        SQLiteConnection? connection = null;
        try
        {
            connection = _dbHelper.GetConnection();
            connection.Open();
            string sql = "SELECT MaKhach, HoTen, CCCD, SoDienThoai FROM KHACH_THUE WHERE MaKhach = @MaKhach";
            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@MaKhach", maKhach);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new KhachThue
                {
                    MaKhach = reader.GetString(0),
                    HoTen = reader.GetString(1),
                    CCCD = reader.GetString(2),
                    SoDienThoai = reader.GetString(3)
                };
            }
            return null;
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            connection?.Close();
        }
    }

    public bool Insert(KhachThue k)
    {
        SQLiteConnection? connection = null;
        try
        {
            connection = _dbHelper.GetConnection();
            connection.Open();
            // Check duplicate MaKhach
            string checkMaSql = "SELECT COUNT(*) FROM KHACH_THUE WHERE MaKhach = @MaKhach";
            using var checkMaCmd = new SQLiteCommand(checkMaSql, connection);
            checkMaCmd.Parameters.AddWithValue("@MaKhach", k.MaKhach);
            if ((long)checkMaCmd.ExecuteScalar()! > 0) return false;

            // Check duplicate CCCD
            string checkCccdSql = "SELECT COUNT(*) FROM KHACH_THUE WHERE CCCD = @CCCD";
            using var checkCccdCmd = new SQLiteCommand(checkCccdSql, connection);
            checkCccdCmd.Parameters.AddWithValue("@CCCD", k.CCCD);
            if ((long)checkCccdCmd.ExecuteScalar()! > 0) return false;

            string sql = "INSERT INTO KHACH_THUE (MaKhach, HoTen, CCCD, SoDienThoai) VALUES (@MaKhach, @HoTen, @CCCD, @SoDienThoai)";
            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@MaKhach", k.MaKhach);
            command.Parameters.AddWithValue("@HoTen", k.HoTen);
            command.Parameters.AddWithValue("@CCCD", k.CCCD);
            command.Parameters.AddWithValue("@SoDienThoai", k.SoDienThoai);
            return command.ExecuteNonQuery() > 0;
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            connection?.Close();
        }
    }

    public bool Update(KhachThue k)
    {
        SQLiteConnection? connection = null;
        try
        {
            connection = _dbHelper.GetConnection();
            connection.Open();
            // Check duplicate CCCD on other records
            if (CheckCCCDExists(k.CCCD, k.MaKhach))
            {
                return false;
            }

            string sql = "UPDATE KHACH_THUE SET HoTen = @HoTen, CCCD = @CCCD, SoDienThoai = @SoDienThoai WHERE MaKhach = @MaKhach";
            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@HoTen", k.HoTen);
            command.Parameters.AddWithValue("@CCCD", k.CCCD);
            command.Parameters.AddWithValue("@SoDienThoai", k.SoDienThoai);
            command.Parameters.AddWithValue("@MaKhach", k.MaKhach);
            return command.ExecuteNonQuery() > 0;
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            connection?.Close();
        }
    }

    public bool Delete(string maKhach)
    {
        SQLiteConnection? connection = null;
        try
        {
            connection = _dbHelper.GetConnection();
            connection.Open();
            string sql = "DELETE FROM KHACH_THUE WHERE MaKhach = @MaKhach";
            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@MaKhach", maKhach);
            return command.ExecuteNonQuery() > 0;
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            connection?.Close();
        }
    }

    public bool CheckCCCDExists(string cccd, string excludeMaKhach)
    {
        SQLiteConnection? connection = null;
        try
        {
            connection = _dbHelper.GetConnection();
            connection.Open();
            string sql = "SELECT COUNT(*) FROM KHACH_THUE WHERE CCCD = @CCCD AND MaKhach <> @MaKhach";
            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@CCCD", cccd);
            command.Parameters.AddWithValue("@MaKhach", excludeMaKhach);
            return (long)command.ExecuteScalar()! > 0;
        }
        finally
        {
            connection?.Close();
        }
    }
}
