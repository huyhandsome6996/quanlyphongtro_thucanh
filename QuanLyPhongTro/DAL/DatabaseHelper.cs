using System.Data.SQLite;

namespace QuanLyPhongTro.DAL;

public class DatabaseHelper
{
    private readonly string _connectionString;

    public DatabaseHelper(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default") ?? "Data Source=phongtro.db";
    }

    public SQLiteConnection GetConnection()
    {
        return new SQLiteConnection(_connectionString);
    }
}
