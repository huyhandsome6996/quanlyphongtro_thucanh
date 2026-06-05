using System;
using Microsoft.AspNetCore.Mvc;
using QuanLyPhongTro.DAL;
using QuanLyPhongTro.Entities;

namespace QuanLyPhongTro.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TaiKhoanController : ControllerBase
{
    private readonly ITaiKhoanDAL _taiKhoanDAL;

    public TaiKhoanController(ITaiKhoanDAL taiKhoanDAL)
    {
        _taiKhoanDAL = taiKhoanDAL;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        try
        {
            var taiKhoan = _taiKhoanDAL.DangNhap(request.TenDangNhap, request.MatKhau);
            if (taiKhoan != null)
            {
                return Ok(new { success = true, message = "Đăng nhập thành công", data = taiKhoan });
            }
            return Ok(new { success = false, message = "Sai tên đăng nhập hoặc mật khẩu" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Lỗi hệ thống: " + ex.Message });
        }
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] TaiKhoan request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.TenDangNhap) || string.IsNullOrEmpty(request.MatKhau) || string.IsNullOrEmpty(request.HoTen))
            {
                return Ok(new { success = false, message = "Vui lòng nhập đầy đủ thông tin" });
            }

            bool result = _taiKhoanDAL.DangKy(request);
            if (result)
            {
                return Ok(new { success = true, message = "Đăng ký thành công" });
            }
            return Ok(new { success = false, message = "Tên đăng nhập đã tồn tại" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Lỗi hệ thống: " + ex.Message });
        }
    }
}

public class LoginRequest
{
    public string TenDangNhap { get; set; } = string.Empty;
    public string MatKhau { get; set; } = string.Empty;
}
