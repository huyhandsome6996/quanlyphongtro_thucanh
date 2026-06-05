using Microsoft.AspNetCore.Mvc;
using QuanLyPhongTro.DAL;
using QuanLyPhongTro.Entities;

namespace QuanLyPhongTro.Controllers;

[Route("api/[controller]")]
[ApiController]
public class KhachThueController : ControllerBase
{
    private readonly IKhachThueDAL _khachThueDAL;

    public KhachThueController(IKhachThueDAL khachThueDAL)
    {
        _khachThueDAL = khachThueDAL;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        try
        {
            var list = _khachThueDAL.GetAll();
            return Ok(new { success = true, message = "Lấy danh sách khách thuê thành công", data = list });
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
            var khach = _khachThueDAL.GetById(id);
            if (khach == null)
            {
                return Ok(new { success = false, message = "Không tìm thấy khách thuê" });
            }
            return Ok(new { success = true, message = "Lấy thông tin khách thuê thành công", data = khach });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Lỗi hệ thống: " + ex.Message });
        }
    }

    [HttpPost]
    public IActionResult Insert([FromBody] KhachThue khach)
    {
        try
        {
            if (string.IsNullOrEmpty(khach.MaKhach))
                return Ok(new { success = false, message = "Mã khách không được để trống" });
            if (string.IsNullOrEmpty(khach.HoTen))
                return Ok(new { success = false, message = "Họ tên không được để trống" });
            if (string.IsNullOrEmpty(khach.CCCD))
                return Ok(new { success = false, message = "CCCD không được để trống" });
            if (string.IsNullOrEmpty(khach.SoDienThoai))
                return Ok(new { success = false, message = "Số điện thoại không được để trống" });

            // Check duplicate MaKhach
            var existing = _khachThueDAL.GetById(khach.MaKhach);
            if (existing != null)
                return Ok(new { success = false, message = "Mã khách đã tồn tại" });

            // Check duplicate CCCD
            if (_khachThueDAL.CheckCCCDExists(khach.CCCD, ""))
                return Ok(new { success = false, message = "CCCD đã tồn tại" });

            bool result = _khachThueDAL.Insert(khach);
            if (result)
                return Ok(new { success = true, message = "Thêm khách thuê thành công" });
            return Ok(new { success = false, message = "Thêm khách thuê thất bại" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Lỗi hệ thống: " + ex.Message });
        }
    }

    [HttpPut("{id}")]
    public IActionResult Update(string id, [FromBody] KhachThue khach)
    {
        try
        {
            khach.MaKhach = id;
            var existing = _khachThueDAL.GetById(id);
            if (existing == null)
                return Ok(new { success = false, message = "Không tìm thấy khách thuê" });

            // Check duplicate CCCD on other records
            if (_khachThueDAL.CheckCCCDExists(khach.CCCD, id))
                return Ok(new { success = false, message = "CCCD đã tồn tại ở khách thuê khác" });

            bool result = _khachThueDAL.Update(khach);
            if (result)
                return Ok(new { success = true, message = "Cập nhật khách thuê thành công" });
            return Ok(new { success = false, message = "Cập nhật khách thuê thất bại" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Lỗi hệ thống: " + ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        try
        {
            bool result = _khachThueDAL.Delete(id);
            if (result)
                return Ok(new { success = true, message = "Xóa khách thuê thành công" });
            return Ok(new { success = false, message = "Xóa khách thuê thất bại" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Lỗi hệ thống: " + ex.Message });
        }
    }
}
