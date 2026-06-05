using System.Data.SQLite;
using QuanLyPhongTro.Entities;

namespace QuanLyPhongTro.DAL;

public class HopDongDAL : IHopDongDAL
{
    private readonly DatabaseHelper _dbHelper;

    public HopDongDAL(DatabaseHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public List<HopDong> GetAll()
    {
        SQLiteConnection? connection = null;
        try
        {
            connection = _dbHelper.GetConnection();
            connection.Open();
            string sql = "SELECT MaHopDong, MaPhong, MaKhach, NgayBatDau, TienCoc, TrangThaiHD FROM HOP_DONG";
            using var command = new SQLiteCommand(sql, connection);
            using var reader = command.ExecuteReader();
            var list = new List<HopDong>();
            while (reader.Read())
            {
                list.Add(new HopDong
                {
                    MaHopDong = reader.GetString(0),
                    MaPhong = reader.GetString(1),
                    MaKhach = reader.GetString(2),
                    NgayBatDau = reader.GetString(3),
                    TienCoc = reader.GetDouble(4),
                    TrangThaiHD = reader.GetString(5)
                });
            }
            return list;
        }
        finally
        {
            connection?.Close();
        }
    }

    public HopDong? GetById(string maHopDong)
    {
        SQLiteConnection? connection = null;
        try
        {
            connection = _dbHelper.GetConnection();
            connection.Open();
            string sql = "SELECT MaHopDong, MaPhong, MaKhach, NgayBatDau, TienCoc, TrangThaiHD FROM HOP_DONG WHERE MaHopDong = @MaHopDong";
            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@MaHopDong", maHopDong);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new HopDong
                {
                    MaHopDong = reader.GetString(0),
                    MaPhong = reader.GetString(1),
                    MaKhach = reader.GetString(2),
                    NgayBatDau = reader.GetString(3),
                    TienCoc = reader.GetDouble(4),
                    TrangThaiHD = reader.GetString(5)
                };
            }
            return null;
        }
        finally
        {
            connection?.Close();
        }
    }

    public HopDong? GetByPhong(string maPhong)
    {
        SQLiteConnection? connection = null;
        try
        {
            connection = _dbHelper.GetConnection();
            connection.Open();
            string sql = "SELECT MaHopDong, MaPhong, MaKhach, NgayBatDau, TienCoc, TrangThaiHD FROM HOP_DONG WHERE MaPhong = @MaPhong AND TrangThaiHD = 'Đang hiệu lực'";
            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@MaPhong", maPhong);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new HopDong
                {
                    MaHopDong = reader.GetString(0),
                    MaPhong = reader.GetString(1),
                    MaKhach = reader.GetString(2),
                    NgayBatDau = reader.GetString(3),
                    TienCoc = reader.GetDouble(4),
                    TrangThaiHD = reader.GetString(5)
                };
            }
            return null;
        }
        finally
        {
            connection?.Close();
        }
    }

    public bool Insert(HopDong h)
    {
        SQLiteConnection? connection = null;
        try
        {
            connection = _dbHelper.GetConnection();
            connection.Open();
            // Check duplicate MaHopDong
            string checkSql = "SELECT COUNT(*) FROM HOP_DONG WHERE MaHopDong = @MaHopDong";
            using var checkCmd = new SQLiteCommand(checkSql, connection);
            checkCmd.Parameters.AddWithValue("@MaHopDong", h.MaHopDong);
            if ((long)checkCmd.ExecuteScalar()! > 0) return false;

            // Use transaction to insert contract and update room status
            using var transaction = connection.BeginTransaction();
            try
            {
                string insertSql = "INSERT INTO HOP_DONG (MaHopDong, MaPhong, MaKhach, NgayBatDau, TienCoc, TrangThaiHD) VALUES (@MaHopDong, @MaPhong, @MaKhach, @NgayBatDau, @TienCoc, @TrangThaiHD)";
                using var insertCmd = new SQLiteCommand(insertSql, connection, transaction);
                insertCmd.Parameters.AddWithValue("@MaHopDong", h.MaHopDong);
                insertCmd.Parameters.AddWithValue("@MaPhong", h.MaPhong);
                insertCmd.Parameters.AddWithValue("@MaKhach", h.MaKhach);
                insertCmd.Parameters.AddWithValue("@NgayBatDau", h.NgayBatDau);
                insertCmd.Parameters.AddWithValue("@TienCoc", h.TienCoc);
                insertCmd.Parameters.AddWithValue("@TrangThaiHD", h.TrangThaiHD);
                insertCmd.ExecuteNonQuery();

                string updateSql = "UPDATE PHONG_TRO SET TrangThai = @TrangThai WHERE MaPhong = @MaPhong";
                using var updateCmd = new SQLiteCommand(updateSql, connection, transaction);
                updateCmd.Parameters.AddWithValue("@TrangThai", "Đã thuê");
                updateCmd.Parameters.AddWithValue("@MaPhong", h.MaPhong);
                updateCmd.ExecuteNonQuery();

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }
        finally
        {
            connection?.Close();
        }
    }

    public bool Update(HopDong h)
    {
        SQLiteConnection? connection = null;
        try
        {
            connection = _dbHelper.GetConnection();
            connection.Open();
            string sql = "UPDATE HOP_DONG SET MaPhong = @MaPhong, MaKhach = @MaKhach, NgayBatDau = @NgayBatDau, TienCoc = @TienCoc, TrangThaiHD = @TrangThaiHD WHERE MaHopDong = @MaHopDong";
            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@MaPhong", h.MaPhong);
            command.Parameters.AddWithValue("@MaKhach", h.MaKhach);
            command.Parameters.AddWithValue("@NgayBatDau", h.NgayBatDau);
            command.Parameters.AddWithValue("@TienCoc", h.TienCoc);
            command.Parameters.AddWithValue("@TrangThaiHD", h.TrangThaiHD);
            command.Parameters.AddWithValue("@MaHopDong", h.MaHopDong);
            return command.ExecuteNonQuery() > 0;
        }
        finally
        {
            connection?.Close();
        }
    }

    public bool ThanhLy(string maHopDong)
    {
        SQLiteConnection? connection = null;
        try
        {
            connection = _dbHelper.GetConnection();
            connection.Open();
            // Get the contract's room
            string getRoomSql = "SELECT MaPhong FROM HOP_DONG WHERE MaHopDong = @MaHopDong";
            using var getRoomCmd = new SQLiteCommand(getRoomSql, connection);
            getRoomCmd.Parameters.AddWithValue("@MaHopDong", maHopDong);
            var maPhongObj = getRoomCmd.ExecuteScalar();
            if (maPhongObj == null) return false;
            string maPhong = maPhongObj.ToString()!;

            // Use transaction to update contract status and room status
            using var transaction = connection.BeginTransaction();
            try
            {
                string updateHDSql = "UPDATE HOP_DONG SET TrangThaiHD = @TrangThaiHD WHERE MaHopDong = @MaHopDong";
                using var updateHDCmd = new SQLiteCommand(updateHDSql, connection, transaction);
                updateHDCmd.Parameters.AddWithValue("@TrangThaiHD", "Đã thanh lý");
                updateHDCmd.Parameters.AddWithValue("@MaHopDong", maHopDong);
                updateHDCmd.ExecuteNonQuery();

                string updatePhongSql = "UPDATE PHONG_TRO SET TrangThai = @TrangThai WHERE MaPhong = @MaPhong";
                using var updatePhongCmd = new SQLiteCommand(updatePhongSql, connection, transaction);
                updatePhongCmd.Parameters.AddWithValue("@TrangThai", "Trống");
                updatePhongCmd.Parameters.AddWithValue("@MaPhong", maPhong);
                updatePhongCmd.ExecuteNonQuery();

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }
        finally
        {
            connection?.Close();
        }
    }

    public List<HopDong> GetActiveContracts()
    {
        SQLiteConnection? connection = null;
        try
        {
            connection = _dbHelper.GetConnection();
            connection.Open();
            string sql = "SELECT MaHopDong, MaPhong, MaKhach, NgayBatDau, TienCoc, TrangThaiHD FROM HOP_DONG WHERE TrangThaiHD = 'Đang hiệu lực'";
            using var command = new SQLiteCommand(sql, connection);
            using var reader = command.ExecuteReader();
            var list = new List<HopDong>();
            while (reader.Read())
            {
                list.Add(new HopDong
                {
                    MaHopDong = reader.GetString(0),
                    MaPhong = reader.GetString(1),
                    MaKhach = reader.GetString(2),
                    NgayBatDau = reader.GetString(3),
                    TienCoc = reader.GetDouble(4),
                    TrangThaiHD = reader.GetString(5)
                });
            }
            return list;
        }
        finally
        {
            connection?.Close();
        }
    }
}
