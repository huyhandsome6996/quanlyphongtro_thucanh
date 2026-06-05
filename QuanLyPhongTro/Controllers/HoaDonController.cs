using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using QuanLyPhongTro.DAL;
using QuanLyPhongTro.Entities;

namespace QuanLyPhongTro.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HoaDonController : ControllerBase
{
    private readonly IHoaDonDAL _hoaDonDAL;
    private readonly IHopDongDAL _hopDongDAL;
    private readonly IPhongTroDAL _phongTroDAL;
    private readonly IConfiguration _configuration;

    public HoaDonController(IHoaDonDAL hoaDonDAL, IHopDongDAL hopDongDAL, IPhongTroDAL phongTroDAL, IConfiguration configuration)
    {
        _hoaDonDAL = hoaDonDAL;
        _hopDongDAL = hopDongDAL;
        _phongTroDAL = phongTroDAL;
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        try
        {
            var list = _hoaDonDAL.GetAll();
            return Ok(new { success = true, message = "Lấy danh sách hóa đơn thành công", data = list });
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
            var hoaDon = _hoaDonDAL.GetById(id);
            if (hoaDon == null)
            {
                return Ok(new { success = false, message = "Không tìm thấy hóa đơn" });
            }
            return Ok(new { success = true, message = "Lấy thông tin hóa đơn thành công", data = hoaDon });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Lỗi hệ thống: " + ex.Message });
        }
    }

    [HttpGet("hopdong/{maHopDong}")]
    public IActionResult GetByHopDong(string maHopDong)
    {
        try
        {
            var list = _hoaDonDAL.GetByHopDong(maHopDong);
            return Ok(new { success = true, message = "Lấy danh sách hóa đơn theo hợp đồng thành công", data = list });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Lỗi hệ thống: " + ex.Message });
        }
    }

    [HttpPost]
    public IActionResult Insert([FromBody] HoaDon hoaDon)
    {
        try
        {
            // Validate all fields
            if (string.IsNullOrEmpty(hoaDon.MaHoaDon))
                return Ok(new { success = false, message = "Mã hóa đơn không được để trống" });
            if (string.IsNullOrEmpty(hoaDon.MaHopDong))
                return Ok(new { success = false, message = "Mã hợp đồng không được để trống" });
            if (string.IsNullOrEmpty(hoaDon.ThangNam))
                return Ok(new { success = false, message = "Tháng năm không được để trống" });

            // Check duplicate invoice for same month
            if (_hoaDonDAL.CheckExists(hoaDon.MaHopDong, hoaDon.ThangNam))
                return Ok(new { success = false, message = "Hóa đơn cho tháng này đã tồn tại" });

            // Get contract to find room
            var hopDong = _hopDongDAL.GetById(hoaDon.MaHopDong);
            if (hopDong == null)
                return Ok(new { success = false, message = "Không tìm thấy hợp đồng" });

            // Get room to find GiaThue
            var phong = _phongTroDAL.GetById(hopDong.MaPhong);
            if (phong == null)
                return Ok(new { success = false, message = "Không tìm thấy phòng" });

            // Calculate TongTien
            int giaDien = _configuration.GetValue<int>("GiaDien", 3500);
            int giaNuoc = _configuration.GetValue<int>("GiaNuoc", 15000);
            double tongTien = (hoaDon.SoDienMoi - hoaDon.SoDienCu) * giaDien
                            + (hoaDon.SoNuocMoi - hoaDon.SoNuocCu) * giaNuoc
                            + phong.GiaThue;
            hoaDon.TongTien = tongTien;

            // Set default TrangThaiThanhToan if not provided
            if (string.IsNullOrEmpty(hoaDon.TrangThaiThanhToan))
                hoaDon.TrangThaiThanhToan = "Chưa thanh toán";

            bool result = _hoaDonDAL.Insert(hoaDon);
            if (result)
                return Ok(new { success = true, message = "Thêm hóa đơn thành công", data = hoaDon });
            return Ok(new { success = false, message = "Thêm hóa đơn thất bại" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Lỗi hệ thống: " + ex.Message });
        }
    }

    [HttpPut("{id}/thanhtoan")]
    public IActionResult ThanhToan(string id)
    {
        try
        {
            var existing = _hoaDonDAL.GetById(id);
            if (existing == null)
                return Ok(new { success = false, message = "Không tìm thấy hóa đơn" });

            if (existing.TrangThaiThanhToan == "Đã thanh toán")
                return Ok(new { success = false, message = "Hóa đơn đã được thanh toán" });

            bool result = _hoaDonDAL.ThanhToan(id);
            if (result)
                return Ok(new { success = true, message = "Thanh toán hóa đơn thành công" });
            return Ok(new { success = false, message = "Thanh toán hóa đơn thất bại" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Lỗi hệ thống: " + ex.Message });
        }
    }
}
