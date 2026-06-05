using System.Data.SQLite;
using QuanLyPhongTro.Entities;

namespace QuanLyPhongTro.DAL;

public class PhongTroDAL : IPhongTroDAL
{
    private readonly DatabaseHelper _dbHelper;

    public PhongTroDAL(DatabaseHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public List<PhongTro> GetAll()
    {
        SQLiteConnection? connection = null;
        try
        {
            connection = _dbHelper.GetConnection();
            connection.Open();
            string sql = "SELECT MaPhong, TenPhong, LoaiPhong, GiaThue, TrangThai FROM PHONG_TRO";
            using var command = new SQLiteCommand(sql, connection);
            using var reader = command.ExecuteReader();
            var list = new List<PhongTro>();
            while (reader.Read())
            {
                list.Add(new PhongTro
                {
                    MaPhong = reader.GetString(0),
                    TenPhong = reader.GetString(1),
                    LoaiPhong = reader.GetString(2),
                    GiaThue = reader.GetDouble(3),
                    TrangThai = reader.GetString(4)
                });
            }
            return list;
        }
        finally
        {
            connection?.Close();
        }
    }

    public PhongTro? GetById(string maPhong)
    {
        SQLiteConnection? connection = null;
        try
        {
            connection = _dbHelper.GetConnection();
            connection.Open();
            string sql = "SELECT MaPhong, TenPhong, LoaiPhong, GiaThue, TrangThai FROM PHONG_TRO WHERE MaPhong = @MaPhong";
            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@MaPhong", maPhong);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new PhongTro
                {
                    MaPhong = reader.GetString(0),
                    TenPhong = reader.GetString(1),
                    LoaiPhong = reader.GetString(2),
                    GiaThue = reader.GetDouble(3),
                    TrangThai = reader.GetString(4)
                };
            }
            return null;
        }
        finally
        {
            connection?.Close();
        }
    }

    public bool Insert(PhongTro p)
    {
        SQLiteConnection? connection = null;
        try
        {
            connection = _dbHelper.GetConnection();
            connection.Open();
            // Check duplicate MaPhong
            string checkSql = "SELECT COUNT(*) FROM PHONG_TRO WHERE MaPhong = @MaPhong";
            using var checkCmd = new SQLiteCommand(checkSql, connection);
            checkCmd.Parameters.AddWithValue("@MaPhong", p.MaPhong);
            long count = (long)checkCmd.ExecuteScalar()!;
            if (count > 0) return false;

            string sql = "INSERT INTO PHONG_TRO (MaPhong, TenPhong, LoaiPhong, GiaThue, TrangThai) VALUES (@MaPhong, @TenPhong, @LoaiPhong, @GiaThue, @TrangThai)";
            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@MaPhong", p.MaPhong);
            command.Parameters.AddWithValue("@TenPhong", p.TenPhong);
            command.Parameters.AddWithValue("@LoaiPhong", p.LoaiPhong);
            command.Parameters.AddWithValue("@GiaThue", p.GiaThue);
            command.Parameters.AddWithValue("@TrangThai", p.TrangThai);
            return command.ExecuteNonQuery() > 0;
        }
        finally
        {
            connection?.Close();
        }
    }

    public bool Update(PhongTro p)
    {
        SQLiteConnection? connection = null;
        try
        {
            connection = _dbHelper.GetConnection();
            connection.Open();
            string sql = "UPDATE PHONG_TRO SET TenPhong = @TenPhong, LoaiPhong = @LoaiPhong, GiaThue = @GiaThue, TrangThai = @TrangThai WHERE MaPhong = @MaPhong";
            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@TenPhong", p.TenPhong);
            command.Parameters.AddWithValue("@LoaiPhong", p.LoaiPhong);
            command.Parameters.AddWithValue("@GiaThue", p.GiaThue);
            command.Parameters.AddWithValue("@TrangThai", p.TrangThai);
            command.Parameters.AddWithValue("@MaPhong", p.MaPhong);
            return command.ExecuteNonQuery() > 0;
        }
        finally
        {
            connection?.Close();
        }
    }

    public bool Delete(string maPhong)
    {
        SQLiteConnection? connection = null;
        try
        {
            connection = _dbHelper.GetConnection();
            connection.Open();
            string sql = "DELETE FROM PHONG_TRO WHERE MaPhong = @MaPhong";
            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@MaPhong", maPhong);
            return command.ExecuteNonQuery() > 0;
        }
        finally
        {
            connection?.Close();
        }
    }

    public bool UpdateTrangThai(string maPhong, string trangThai)
    {
        SQLiteConnection? connection = null;
        try
        {
            connection = _dbHelper.GetConnection();
            connection.Open();
            string sql = "UPDATE PHONG_TRO SET TrangThai = @TrangThai WHERE MaPhong = @MaPhong";
            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@TrangThai", trangThai);
            command.Parameters.AddWithValue("@MaPhong", maPhong);
            return command.ExecuteNonQuery() > 0;
        }
        finally
        {
            connection?.Close();
        }
    }
}
