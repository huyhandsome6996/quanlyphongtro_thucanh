using System.Data.SQLite;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace QuanLyPhongTro;

public class DatabaseInitializer
{
    private readonly string _connectionString;

    public DatabaseInitializer(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default") ?? "Data Source=phongtro.db";
    }

    private static string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }

    public void Initialize()
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        // Create tables
        CreateTables(connection);
        // Seed data
        SeedData(connection);
    }

    private void CreateTables(SQLiteConnection connection)
    {
        var commands = new string[]
        {
            @"CREATE TABLE IF NOT EXISTS TAI_KHOAN (
                TenDangNhap TEXT PRIMARY KEY,
                MatKhau TEXT NOT NULL,
                HoTen TEXT NOT NULL
            )",
            @"CREATE TABLE IF NOT EXISTS PHONG_TRO (
                MaPhong TEXT PRIMARY KEY,
                TenPhong TEXT NOT NULL,
                LoaiPhong TEXT NOT NULL,
                GiaThue REAL NOT NULL,
                TrangThai TEXT NOT NULL
            )",
            @"CREATE TABLE IF NOT EXISTS KHACH_THUE (
                MaKhach TEXT PRIMARY KEY,
                HoTen TEXT NOT NULL,
                CCCD TEXT UNIQUE NOT NULL,
                SoDienThoai TEXT NOT NULL
            )",
            @"CREATE TABLE IF NOT EXISTS HOP_DONG (
                MaHopDong TEXT PRIMARY KEY,
                MaPhong TEXT NOT NULL,
                MaKhach TEXT NOT NULL,
                NgayBatDau TEXT NOT NULL,
                TienCoc REAL NOT NULL,
                TrangThaiHD TEXT NOT NULL,
                FOREIGN KEY (MaPhong) REFERENCES PHONG_TRO(MaPhong),
                FOREIGN KEY (MaKhach) REFERENCES KHACH_THUE(MaKhach)
            )",
            @"CREATE TABLE IF NOT EXISTS HOA_DON (
                MaHoaDon TEXT PRIMARY KEY,
                MaHopDong TEXT NOT NULL,
                ThangNam TEXT NOT NULL,
                SoDienCu INTEGER NOT NULL,
                SoDienMoi INTEGER NOT NULL,
                SoNuocCu INTEGER NOT NULL,
                SoNuocMoi INTEGER NOT NULL,
                TongTien REAL NOT NULL,
                TrangThaiThanhToan TEXT NOT NULL,
                FOREIGN KEY (MaHopDong) REFERENCES HOP_DONG(MaHopDong)
            )"
        };

        foreach (var sql in commands)
        {
            using var command = new SQLiteCommand(sql, connection);
            command.ExecuteNonQuery();
        }
    }

    private void SeedData(SQLiteConnection connection)
    {
        // Check if data already exists
        using var checkCmd = new SQLiteCommand("SELECT COUNT(*) FROM TAI_KHOAN", connection);
        long count = (long)checkCmd.ExecuteScalar()!;
        if (count > 0) return; // Already seeded

        // Insert admin account
        string hashedPassword = HashPassword("123456");
        using (var cmd = new SQLiteCommand("INSERT INTO TAI_KHOAN (TenDangNhap, MatKhau, HoTen) VALUES (@TenDangNhap, @MatKhau, @HoTen)", connection))
        {
            cmd.Parameters.AddWithValue("@TenDangNhap", "admin");
            cmd.Parameters.AddWithValue("@MatKhau", hashedPassword);
            cmd.Parameters.AddWithValue("@HoTen", "Quản trị viên");
            cmd.ExecuteNonQuery();
        }

        // Insert 5 sample rooms
        var rooms = new (string MaPhong, string TenPhong, string LoaiPhong, double GiaThue, string TrangThai)[]
        {
            ("P001", "Phòng 101", "Phòng quạt", 2000000, "Trống"),
            ("P002", "Phòng 102", "Phòng lạnh", 3500000, "Đã thuê"),
            ("P003", "Phòng 201", "Phòng quạt", 2500000, "Trống"),
            ("P004", "Phòng 202", "Phòng lạnh", 4000000, "Đã thuê"),
            ("P005", "Phòng 301", "Phòng quạt", 2200000, "Trống"),
        };

        foreach (var room in rooms)
        {
            using var cmd = new SQLiteCommand("INSERT INTO PHONG_TRO (MaPhong, TenPhong, LoaiPhong, GiaThue, TrangThai) VALUES (@MaPhong, @TenPhong, @LoaiPhong, @GiaThue, @TrangThai)", connection);
            cmd.Parameters.AddWithValue("@MaPhong", room.MaPhong);
            cmd.Parameters.AddWithValue("@TenPhong", room.TenPhong);
            cmd.Parameters.AddWithValue("@LoaiPhong", room.LoaiPhong);
            cmd.Parameters.AddWithValue("@GiaThue", room.GiaThue);
            cmd.Parameters.AddWithValue("@TrangThai", room.TrangThai);
            cmd.ExecuteNonQuery();
        }

        // Insert 3 sample tenants
        var tenants = new (string MaKhach, string HoTen, string CCCD, string SoDienThoai)[]
        {
            ("KT001", "Nguyễn Văn An", "079201001234", "0901234567"),
            ("KT002", "Trần Thị Bình", "079202005678", "0912345678"),
            ("KT003", "Lê Hoàng Cường", "079203009012", "0923456789"),
        };

        foreach (var tenant in tenants)
        {
            using var cmd = new SQLiteCommand("INSERT INTO KHACH_THUE (MaKhach, HoTen, CCCD, SoDienThoai) VALUES (@MaKhach, @HoTen, @CCCD, @SoDienThoai)", connection);
            cmd.Parameters.AddWithValue("@MaKhach", tenant.MaKhach);
            cmd.Parameters.AddWithValue("@HoTen", tenant.HoTen);
            cmd.Parameters.AddWithValue("@CCCD", tenant.CCCD);
            cmd.Parameters.AddWithValue("@SoDienThoai", tenant.SoDienThoai);
            cmd.ExecuteNonQuery();
        }

        // Insert 2 sample contracts for "Đã thuê" rooms
        var contracts = new (string MaHopDong, string MaPhong, string MaKhach, string NgayBatDau, double TienCoc, string TrangThaiHD)[]
        {
            ("HD001", "P002", "KT001", "2025-01-15", 7000000, "Đang hiệu lực"),
            ("HD002", "P004", "KT002", "2025-02-01", 8000000, "Đang hiệu lực"),
        };

        foreach (var contract in contracts)
        {
            using var cmd = new SQLiteCommand("INSERT INTO HOP_DONG (MaHopDong, MaPhong, MaKhach, NgayBatDau, TienCoc, TrangThaiHD) VALUES (@MaHopDong, @MaPhong, @MaKhach, @NgayBatDau, @TienCoc, @TrangThaiHD)", connection);
            cmd.Parameters.AddWithValue("@MaHopDong", contract.MaHopDong);
            cmd.Parameters.AddWithValue("@MaPhong", contract.MaPhong);
            cmd.Parameters.AddWithValue("@MaKhach", contract.MaKhach);
            cmd.Parameters.AddWithValue("@NgayBatDau", contract.NgayBatDau);
            cmd.Parameters.AddWithValue("@TienCoc", contract.TienCoc);
            cmd.Parameters.AddWithValue("@TrangThaiHD", contract.TrangThaiHD);
            cmd.ExecuteNonQuery();
        }
    }
}
