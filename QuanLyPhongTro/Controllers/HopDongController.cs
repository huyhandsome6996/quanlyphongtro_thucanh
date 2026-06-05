using Microsoft.AspNetCore.Mvc;
using QuanLyPhongTro.DAL;
using QuanLyPhongTro.Entities;

namespace QuanLyPhongTro.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HopDongController : ControllerBase
{
    private readonly IHopDongDAL _hopDongDAL;

    public HopDongController(IHopDongDAL hopDongDAL)
    {
        _hopDongDAL = hopDongDAL;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        try
        {
            var list = _hopDongDAL.GetAll();
            return Ok(new { success = true, message = "Lấy danh sách hợp đồng thành công", data = list });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Lỗi hệ thống: " + ex.Message });
        }
    }

    [HttpGet("active")]
    public IActionResult GetActiveContracts()
    {
        try
        {
            var list = _hopDongDAL.GetActiveContracts();
            return Ok(new { success = true, message = "Lấy danh sách hợp đồng hiệu lực thành công", data = list });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Lỗi hệ thống: " + ex.Message });
        }
    }

    [HttpGet("{id}")]
    public IActionResult GetById(string id)
    {
        try
        {
            var hopDong = _hopDongDAL.GetById(id);
            if (hopDong == null)
            {
                return Ok(new { success = false, message = "Không tìm thấy hợp đồng" });
            }
            return Ok(new { success = true, message = "Lấy thông tin hợp đồng thành công", data = hopDong });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Lỗi hệ thống: " + ex.Message });
        }
    }

    [HttpPost]
    public IActionResult Insert([FromBody] HopDong hopDong)
    {
        try
        {
            if (string.IsNullOrEmpty(hopDong.MaHopDong))
                return Ok(new { success = false, message = "Mã hợp đồng không được để trống" });
            if (string.IsNullOrEmpty(hopDong.MaPhong))
                return Ok(new { success = false, message = "Mã phòng không được để trống" });
            if (string.IsNullOrEmpty(hopDong.MaKhach))
                return Ok(new { success = false, message = "Mã khách không được để trống" });
            if (hopDong.TienCoc < 0)
                return Ok(new { success = false, message = "Tiền cọc không được âm" });

            var existing = _hopDongDAL.GetById(hopDong.MaHopDong);
            if (existing != null)
                return Ok(new { success = false, message = "Mã hợp đồng đã tồn tại" });

            // Set default TrangThaiHD if not provided
            if (string.IsNullOrEmpty(hopDong.TrangThaiHD))
                hopDong.TrangThaiHD = "Đang hiệu lực";

            bool result = _hopDongDAL.Insert(hopDong);
            if (result)
                return Ok(new { success = true, message = "Thêm hợp đồng thành công" });
            return Ok(new { success = false, message = "Thêm hợp đồng thất bại" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Lỗi hệ thống: " + ex.Message });
        }
    }

    [HttpPut("{id}/thanhly")]
    public IActionResult ThanhLy(string id)
    {
        try
        {
            var existing = _hopDongDAL.GetById(id);
            if (existing == null)
                return Ok(new { success = false, message = "Không tìm thấy hợp đồng" });

            if (existing.TrangThaiHD == "Đã thanh lý")
                return Ok(new { success = false, message = "Hợp đồng đã được thanh lý" });

            bool result = _hopDongDAL.ThanhLy(id);
            if (result)
                return Ok(new { success = true, message = "Thanh lý hợp đồng thành công" });
            return Ok(new { success = false, message = "Thanh lý hợp đồng thất bại" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Lỗi hệ thống: " + ex.Message });
        }
    }
}
