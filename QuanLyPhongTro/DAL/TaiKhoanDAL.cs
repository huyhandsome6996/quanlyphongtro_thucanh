using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;
using QuanLyPhongTro.Entities;

namespace QuanLyPhongTro.DAL;

public class TaiKhoanDAL : ITaiKhoanDAL
{
    private readonly DatabaseHelper _dbHelper;

    public TaiKhoanDAL(DatabaseHelper dbHelper)
    {
        _dbHelper = dbHelper;
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

    public TaiKhoan? DangNhap(string tenDangNhap, string matKhau)
    {
        SQLiteConnection? connection = null;
        try
        {
            connection = _dbHelper.GetConnection();
            connection.Open();
            string hashedPassword = HashPassword(matKhau);
            string sql = "SELECT TenDangNhap, MatKhau, HoTen FROM TAI_KHOAN WHERE TenDangNhap = @TenDangNhap AND MatKhau = @MatKhau";
            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);
            command.Parameters.AddWithValue("@MatKhau", hashedPassword);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new TaiKhoan
                {
                    TenDangNhap = reader.GetString(0),
                    MatKhau = reader.GetString(1),
                    HoTen = reader.GetString(2)
                };
            }
            return null;
        }
        finally
        {
            connection?.Close();
        }
    }
}
