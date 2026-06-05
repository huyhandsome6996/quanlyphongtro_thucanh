async function handleRegister(event) {
    event.preventDefault();
    
    const hoTen = document.getElementById('txtHoTen').value.trim();
    const username = document.getElementById('txtUsername').value.trim();
    const password = document.getElementById('txtPassword').value.trim();
    const errorDiv = document.getElementById('registerError');
    const btnRegister = document.getElementById('btnRegister');

    if (!hoTen || !username || !password) {
        showError('Vui lòng nhập đầy đủ thông tin');
        return false;
    }

    try {
        // Show loading state
        btnRegister.disabled = true;
        btnRegister.innerHTML = '⏳ Đang xử lý...';
        errorDiv.style.display = 'none';

        const response = await fetch('/api/taikhoan/register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                HoTen: hoTen,
                TenDangNhap: username,
                MatKhau: password
            })
        });

        const result = await response.json();

        if (result.success) {
            alert('Đăng ký thành công! Vui lòng đăng nhập.');
            window.location.href = 'login.html';
        } else {
            showError(result.message || 'Đăng ký thất bại');
        }
    } catch (error) {
        console.error('Lỗi:', error);
        showError('Không thể kết nối đến máy chủ');
    } finally {
        // Restore button state
        btnRegister.disabled = false;
        btnRegister.innerHTML = '📝 Đăng ký';
    }

    return false;
}

function showError(message) {
    const errorDiv = document.getElementById('registerError');
    errorDiv.textContent = message;
    errorDiv.style.display = 'block';
}
