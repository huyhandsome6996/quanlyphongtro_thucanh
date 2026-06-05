namespace QuanLyPhongTro.Entities;

public class HoaDon
{
    public string MaHoaDon { get; set; } = string.Empty;
    public string MaHopDong { get; set; } = string.Empty;
    public string ThangNam { get; set; } = string.Empty;
    public int SoDienCu { get; set; }
    public int SoDienMoi { get; set; }
    public int SoNuocCu { get; set; }
    public int SoNuocMoi { get; set; }
    public double TongTien { get; set; }
    public string TrangThaiThanhToan { get; set; } = string.Empty;
}
