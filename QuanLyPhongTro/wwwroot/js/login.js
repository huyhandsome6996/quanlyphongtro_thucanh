// === login.js ===

// API helper
const API = {
    get: async (url) => {
        const res = await fetch(url);
        return await res.json();
    },
    post: async (url, data) => {
        const res = await fetch(url, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });
        return await res.json();
    },
    put: async (url, data) => {
        const res = await fetch(url, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });
        return await res.json();
    },
    delete: async (url) => {
        const res = await fetch(url, { method: 'DELETE' });
        return await res.json();
    }
};

// Toast notification
function showToast(message, type = 'success') {
    const container = document.getElementById('toastContainer');
    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;

    const icons = {
        success: '✅',
        error: '❌',
        warning: '⚠️'
    };

    toast.innerHTML = `
        <span class="toast-icon">${icons[type] || '💬'}</span>
        <span>${message}</span>
        <div class="toast-progress"></div>
    `;

    container.appendChild(toast);

    setTimeout(() => {
        toast.classList.add('toast-out');
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}

// Format currency
function formatCurrency(amount) {
    return new Intl.NumberFormat('vi-VN').format(amount) + ' đ';
}

// Check if already logged in
(function checkExistingLogin() {
    const user = localStorage.getItem('user');
    if (user) {
        window.location.href = 'index.html';
    }
})();

// Handle login
async function handleLogin(event) {
    event.preventDefault();

    const txtUsername = document.getElementById('txtUsername');
    const txtPassword = document.getElementById('txtPassword');
    const loginError = document.getElementById('loginError');
    const btnLogin = document.getElementById('btnLogin');

    const tenDangNhap = txtUsername.value.trim();
    const matKhau = txtPassword.value.trim();

    // Clear previous errors
    loginError.style.display = 'none';
    txtUsername.classList.remove('is-invalid');
    txtPassword.classList.remove('is-invalid');

    if (!tenDangNhap) {
        txtUsername.classList.add('is-invalid');
        txtUsername.focus();
        return false;
    }

    if (!matKhau) {
        txtPassword.classList.add('is-invalid');
        txtPassword.focus();
        return false;
    }

    // Disable button while processing
    btnLogin.disabled = true;
    btnLogin.innerHTML = '⏳ Đang đăng nhập...';

    try {
        const result = await API.post('/api/taikhoan/login', {
            tenDangNhap: tenDangNhap,
            matKhau: matKhau
        });

        if (result.success) {
            localStorage.setItem('user', JSON.stringify({
                tenDangNhap: tenDangNhap,
                hoTen: result.data?.hoTen || tenDangNhap
            }));
            showToast('Đăng nhập thành công!', 'success');
            setTimeout(() => {
                window.location.href = 'index.html';
            }, 800);
        } else {
            loginError.textContent = result.message || 'Sai tên đăng nhập hoặc mật khẩu!';
            loginError.style.display = 'block';
            showToast(result.message || 'Đăng nhập thất bại!', 'error');
            txtPassword.value = '';
            txtPassword.focus();
        }
    } catch (error) {
        loginError.textContent = 'Không thể kết nối đến server!';
        loginError.style.display = 'block';
        showToast('Lỗi kết nối server!', 'error');
    } finally {
        btnLogin.disabled = false;
        btnLogin.innerHTML = '🔐 Đăng nhập';
    }

    return false;
}
