using System.Data.SQLite;
using QuanLyPhongTro.Entities;

namespace QuanLyPhongTro.DAL;

public class HoaDonDAL : IHoaDonDAL
{
    private readonly DatabaseHelper _dbHelper;

    public HoaDonDAL(DatabaseHelper dbHelper)
    {
        _dbHelper = dbHelper;
    }

    public List<HoaDon> GetAll()
    {
        SQLiteConnection? connection = null;
        try
        {
            connection = _dbHelper.GetConnection();
            connection.Open();
            string sql = "SELECT MaHoaDon, MaHopDong, ThangNam, SoDienCu, SoDienMoi, SoNuocCu, SoNuocMoi, TongTien, TrangThaiThanhToan FROM HOA_DON";
            using var command = new SQLiteCommand(sql, connection);
            using var reader = command.ExecuteReader();
            var list = new List<HoaDon>();
            while (reader.Read())
            {
                list.Add(new HoaDon
                {
                    MaHoaDon = reader.GetString(0),
                    MaHopDong = reader.GetString(1),
                    ThangNam = reader.GetString(2),
                    SoDienCu = reader.GetInt32(3),
                    SoDienMoi = reader.GetInt32(4),
                    SoNuocCu = reader.GetInt32(5),
                    SoNuocMoi = reader.GetInt32(6),
                    TongTien = reader.GetDouble(7),
                    TrangThaiThanhToan = reader.GetString(8)
                });
            }
            return list;
        }
        finally
        {
            connection?.Close();
        }
    }

    public HoaDon? GetById(string maHoaDon)
    {
        SQLiteConnection? connection = null;
        try
        {
            connection = _dbHelper.GetConnection();
            connection.Open();
            string sql = "SELECT MaHoaDon, MaHopDong, ThangNam, SoDienCu, SoDienMoi, SoNuocCu, SoNuocMoi, TongTien, TrangThaiThanhToan FROM HOA_DON WHERE MaHoaDon = @MaHoaDon";
            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@MaHoaDon", maHoaDon);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new HoaDon
                {
                    MaHoaDon = reader.GetString(0),
                    MaHopDong = reader.GetString(1),
                    ThangNam = reader.GetString(2),
                    SoDienCu = reader.GetInt32(3),
                    SoDienMoi = reader.GetInt32(4),
                    SoNuocCu = reader.GetInt32(5),
                    SoNuocMoi = reader.GetInt32(6),
                    TongTien = reader.GetDouble(7),
                    TrangThaiThanhToan = reader.GetString(8)
                };
            }
            return null;
        }
        finally
        {
            connection?.Close();
        }
    }

    public List<HoaDon> GetByHopDong(string maHopDong)
    {
        SQLiteConnection? connection = null;
        try
        {
            connection = _dbHelper.GetConnection();
            connection.Open();
            string sql = "SELECT MaHoaDon, MaHopDong, ThangNam, SoDienCu, SoDienMoi, SoNuocCu, SoNuocMoi, TongTien, TrangThaiThanhToan FROM HOA_DON WHERE MaHopDong = @MaHopDong";
            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@MaHopDong", maHopDong);
            using var reader = command.ExecuteReader();
            var list = new List<HoaDon>();
            while (reader.Read())
            {
                list.Add(new HoaDon
                {
                    MaHoaDon = reader.GetString(0),
                    MaHopDong = reader.GetString(1),
                    ThangNam = reader.GetString(2),
                    SoDienCu = reader.GetInt32(3),
                    SoDienMoi = reader.GetInt32(4),
                    SoNuocCu = reader.GetInt32(5),
                    SoNuocMoi = reader.GetInt32(6),
                    TongTien = reader.GetDouble(7),
                    TrangThaiThanhToan = reader.GetString(8)
                });
            }
            return list;
        }
        finally
        {
            connection?.Close();
        }
    }

    public bool Insert(HoaDon h)
    {
        SQLiteConnection? connection = null;
        try
        {
            connection = _dbHelper.GetConnection();
            connection.Open();
            string sql = "INSERT INTO HOA_DON (MaHoaDon, MaHopDong, ThangNam, SoDienCu, SoDienMoi, SoNuocCu, SoNuocMoi, TongTien, TrangThaiThanhToan) VALUES (@MaHoaDon, @MaHopDong, @ThangNam, @SoDienCu, @SoDienMoi, @SoNuocCu, @SoNuocMoi, @TongTien, @TrangThaiThanhToan)";
            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@MaHoaDon", h.MaHoaDon);
            command.Parameters.AddWithValue("@MaHopDong", h.MaHopDong);
            command.Parameters.AddWithValue("@ThangNam", h.ThangNam);
            command.Parameters.AddWithValue("@SoDienCu", h.SoDienCu);
            command.Parameters.AddWithValue("@SoDienMoi", h.SoDienMoi);
            command.Parameters.AddWithValue("@SoNuocCu", h.SoNuocCu);
            command.Parameters.AddWithValue("@SoNuocMoi", h.SoNuocMoi);
            command.Parameters.AddWithValue("@TongTien", h.TongTien);
            command.Parameters.AddWithValue("@TrangThaiThanhToan", h.TrangThaiThanhToan);
            return command.ExecuteNonQuery() > 0;
        }
        finally
        {
            connection?.Close();
        }
    }

    public bool ThanhToan(string maHoaDon)
    {
        SQLiteConnection? connection = null;
        try
        {
            connection = _dbHelper.GetConnection();
            connection.Open();
            string sql = "UPDATE HOA_DON SET TrangThaiThanhToan = @TrangThaiThanhToan WHERE MaHoaDon = @MaHoaDon";
            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@TrangThaiThanhToan", "Đã thanh toán");
            command.Parameters.AddWithValue("@MaHoaDon", maHoaDon);
            return command.ExecuteNonQuery() > 0;
        }
        finally
        {
            connection?.Close();
        }
    }

    public bool CheckExists(string maHopDong, string thangNam)
    {
        SQLiteConnection? connection = null;
        try
        {
            connection = _dbHelper.GetConnection();
            connection.Open();
            string sql = "SELECT COUNT(*) FROM HOA_DON WHERE MaHopDong = @MaHopDong AND ThangNam = @ThangNam";
            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@MaHopDong", maHopDong);
            command.Parameters.AddWithValue("@ThangNam", thangNam);
            return (long)command.ExecuteScalar()! > 0;
        }
        finally
        {
            connection?.Close();
        }
    }
}
