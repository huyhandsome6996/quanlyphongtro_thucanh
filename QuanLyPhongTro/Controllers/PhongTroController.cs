using Microsoft.AspNetCore.Mvc;
using QuanLyPhongTro.DAL;
using QuanLyPhongTro.Entities;

namespace QuanLyPhongTro.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PhongTroController : ControllerBase
{
    private readonly IPhongTroDAL _phongTroDAL;
    private readonly IHopDongDAL _hopDongDAL;

    public PhongTroController(IPhongTroDAL phongTroDAL, IHopDongDAL hopDongDAL)
    {
        _phongTroDAL = phongTroDAL;
        _hopDongDAL = hopDongDAL;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        try
        {
            var list = _phongTroDAL.GetAll();
            return Ok(new { success = true, message = "Lấy danh sách phòng thành công", data = list });
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
            var phong = _phongTroDAL.GetById(id);
            if (phong == null)
            {
                return Ok(new { success = false, message = "Không tìm thấy phòng" });
            }
            return Ok(new { success = true, message = "Lấy thông tin phòng thành công", data = phong });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Lỗi hệ thống: " + ex.Message });
        }
    }

    [HttpPost]
    public IActionResult Insert([FromBody] PhongTro phong)
    {
        try
        {
            if (string.IsNullOrEmpty(phong.MaPhong))
                return Ok(new { success = false, message = "Mã phòng không được để trống" });
            if (string.IsNullOrEmpty(phong.TenPhong))
                return Ok(new { success = false, message = "Tên phòng không được để trống" });
            if (phong.GiaThue <= 0)
                return Ok(new { success = false, message = "Giá thuê phải lớn hơn 0" });

            var existing = _phongTroDAL.GetById(phong.MaPhong);
            if (existing != null)
                return Ok(new { success = false, message = "Mã phòng đã tồn tại" });

            bool result = _phongTroDAL.Insert(phong);
            if (result)
                return Ok(new { success = true, message = "Thêm phòng thành công" });
            return Ok(new { success = false, message = "Thêm phòng thất bại" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Lỗi hệ thống: " + ex.Message });
        }
    }

    [HttpPut("{id}")]
    public IActionResult Update(string id, [FromBody] PhongTro phong)
    {
        try
        {
            phong.MaPhong = id;
            var existing = _phongTroDAL.GetById(id);
            if (existing == null)
                return Ok(new { success = false, message = "Không tìm thấy phòng" });

            bool result = _phongTroDAL.Update(phong);
            if (result)
                return Ok(new { success = true, message = "Cập nhật phòng thành công" });
            return Ok(new { success = false, message = "Cập nhật phòng thất bại" });
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
            // Check if room has active contract
            var activeContract = _hopDongDAL.GetByPhong(id);
            if (activeContract != null)
                return Ok(new { success = false, message = "Phòng đang có hợp đồng hiệu lực, không thể xóa" });

            bool result = _phongTroDAL.Delete(id);
            if (result)
                return Ok(new { success = true, message = "Xóa phòng thành công" });
            return Ok(new { success = false, message = "Xóa phòng thất bại" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Lỗi hệ thống: " + ex.Message });
        }
    }
}
