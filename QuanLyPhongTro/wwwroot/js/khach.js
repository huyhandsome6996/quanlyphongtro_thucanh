// === khach.js - Tenant Management ===

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
    const icons = { success: '✅', error: '❌', warning: '⚠️' };
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

// Check login
function checkLogin() {
    const user = localStorage.getItem('user');
    if (!user) {
        window.location.href = 'login.html';
        return null;
    }
    return JSON.parse(user);
}

// Logout
function logout() {
    localStorage.removeItem('user');
    window.location.href = 'login.html';
}

// Toggle sidebar
function toggleSidebar() {
    const sidebar = document.getElementById('sidebar');
    const backdrop = document.getElementById('sidebarBackdrop');
    sidebar.classList.toggle('open');
    backdrop.classList.toggle('show');
}

// Global tenant data
let allTenants = [];

// Initialize
async function initPage() {
    const user = checkLogin();
    if (!user) return;

    document.getElementById('userGreeting').textContent = user.hoTen || user.tenDangNhap || 'Admin';
    document.getElementById('userAvatar').textContent = (user.hoTen || user.tenDangNhap || 'A').charAt(0).toUpperCase();

    await loadTenants();
}

// Load tenants
async function loadTenants() {
    try {
        const result = await API.get('/api/khachthue');
        if (result.success) {
            allTenants = result.data || [];
            renderTable(allTenants);
        } else {
            showToast('Không thể tải danh sách khách thuê!', 'error');
        }
    } catch (error) {
        console.error('Error loading tenants:', error);
        showToast('Lỗi kết nối server!', 'error');
        document.getElementById('tableBody').innerHTML = `
            <tr><td colspan="6">
                <div class="empty-state">
                    <div class="empty-icon">⚠️</div>
                    <p>Không thể kết nối đến server</p>
                </div>
            </td></tr>`;
    }
}

// Render table
function renderTable(tenants) {
    const tbody = document.getElementById('tableBody');

    if (!tenants || tenants.length === 0) {
        tbody.innerHTML = `
            <tr><td colspan="6">
                <div class="empty-state">
                    <div class="empty-icon">👤</div>
                    <p>Chưa có khách thuê nào</p>
                </div>
            </td></tr>`;
        return;
    }

    tbody.innerHTML = tenants.map((tenant, index) => `
        <tr>
            <td>${index + 1}</td>
            <td><strong>${tenant.maKhach}</strong></td>
            <td>${tenant.hoTen}</td>
            <td>${tenant.cccd}</td>
            <td>${tenant.soDienThoai}</td>
            <td class="actions">
                <button class="btn btn-info btn-sm" onclick="openEditModal('${tenant.maKhach}')">✏️ Sửa</button>
                <button class="btn btn-danger btn-sm" onclick="deleteTenant('${tenant.maKhach}')">🗑️ Xóa</button>
            </td>
        </tr>
    `).join('');
}

// Search tenants
function searchTenants() {
    const keyword = document.getElementById('txtTimKiem').value.trim().toLowerCase();
    if (!keyword) {
        renderTable(allTenants);
        return;
    }
    const filtered = allTenants.filter(t =>
        t.maKhach.toLowerCase().includes(keyword) ||
        t.hoTen.toLowerCase().includes(keyword) ||
        t.cccd.toLowerCase().includes(keyword) ||
        t.soDienThoai.toLowerCase().includes(keyword)
    );
    renderTable(filtered);
}

// Enter key search
document.addEventListener('DOMContentLoaded', () => {
    document.getElementById('txtTimKiem').addEventListener('keypress', (e) => {
        if (e.key === 'Enter') searchTenants();
    });
});

// Open Add Modal
function openAddModal() {
    document.getElementById('modalTitle').textContent = 'Thêm khách thuê mới';
    document.getElementById('editMode').value = 'add';
    document.getElementById('txtMaKhach').value = '';
    document.getElementById('txtMaKhach').readOnly = false;
    document.getElementById('txtHoTen').value = '';
    document.getElementById('txtCCCD').value = '';
    document.getElementById('txtSoDienThoai').value = '';
    clearValidation();
    document.getElementById('modal').style.display = 'flex';
}

// Open Edit Modal
async function openEditModal(maKhach) {
    try {
        const result = await API.get(`/api/khachthue/${maKhach}`);
        if (!result.success) {
            showToast('Không thể tải thông tin khách thuê!', 'error');
            return;
        }
        const tenant = result.data;
        document.getElementById('modalTitle').textContent = 'Sửa khách thuê';
        document.getElementById('editMode').value = 'edit';
        document.getElementById('txtMaKhach').value = tenant.maKhach;
        document.getElementById('txtMaKhach').readOnly = true;
        document.getElementById('txtHoTen').value = tenant.hoTen;
        document.getElementById('txtCCCD').value = tenant.cccd;
        document.getElementById('txtSoDienThoai').value = tenant.soDienThoai;
        clearValidation();
        document.getElementById('modal').style.display = 'flex';
    } catch (error) {
        showToast('Lỗi kết nối server!', 'error');
    }
}

// Close Modal
function closeModal() {
    document.getElementById('modal').style.display = 'none';
    clearValidation();
}

// Clear validation
function clearValidation() {
    document.querySelectorAll('.form-control.is-invalid').forEach(el => el.classList.remove('is-invalid'));
}

// Validate form
function validateForm() {
    let valid = true;
    clearValidation();

    const editMode = document.getElementById('editMode').value;
    const txtMaKhach = document.getElementById('txtMaKhach');
    const txtHoTen = document.getElementById('txtHoTen');
    const txtCCCD = document.getElementById('txtCCCD');
    const txtSoDienThoai = document.getElementById('txtSoDienThoai');

    if (editMode === 'add' && !txtMaKhach.value.trim()) {
        txtMaKhach.classList.add('is-invalid');
        valid = false;
    }

    if (!txtHoTen.value.trim()) {
        txtHoTen.classList.add('is-invalid');
        valid = false;
    }

    if (!txtCCCD.value.trim()) {
        txtCCCD.classList.add('is-invalid');
        valid = false;
    }

    if (!txtSoDienThoai.value.trim()) {
        txtSoDienThoai.classList.add('is-invalid');
        valid = false;
    }

    return valid;
}

// Save tenant
async function saveTenant() {
    if (!validateForm()) return;

    const editMode = document.getElementById('editMode').value;
    const data = {
        maKhach: document.getElementById('txtMaKhach').value.trim(),
        hoTen: document.getElementById('txtHoTen').value.trim(),
        cccd: document.getElementById('txtCCCD').value.trim(),
        soDienThoai: document.getElementById('txtSoDienThoai').value.trim()
    };

    try {
        let result;
        if (editMode === 'add') {
            result = await API.post('/api/khachthue', data);
        } else {
            result = await API.put(`/api/khachthue/${data.maKhach}`, data);
        }

        if (result.success) {
            showToast(editMode === 'add' ? 'Thêm khách thuê thành công!' : 'Cập nhật khách thuê thành công!', 'success');
            closeModal();
            await loadTenants();
        } else {
            showToast(result.message || 'Thao tác thất bại!', 'error');
        }
    } catch (error) {
        showToast('Lỗi kết nối server!', 'error');
    }
}

// Delete tenant
function deleteTenant(maKhach) {
    showConfirm('Xác nhận xóa', `Bạn có chắc muốn xóa khách "${maKhach}"?`, async () => {
        try {
            const result = await API.delete(`/api/khachthue/${maKhach}`);
            if (result.success) {
                showToast('Xóa khách thuê thành công!', 'success');
                await loadTenants();
            } else {
                showToast(result.message || 'Xóa thất bại!', 'error');
            }
        } catch (error) {
            showToast('Lỗi kết nối server!', 'error');
        }
    });
}

// Confirm dialog
function showConfirm(title, message, onConfirm) {
    const overlay = document.createElement('div');
    overlay.className = 'confirm-overlay';
    overlay.innerHTML = `
        <div class="confirm-box">
            <div class="confirm-icon">⚠️</div>
            <h4>${title}</h4>
            <p>${message}</p>
            <div class="confirm-actions">
                <button class="btn btn-danger" id="confirmYes">Xác nhận</button>
                <button class="btn btn-secondary" id="confirmNo">Hủy</button>
            </div>
        </div>
    `;
    document.body.appendChild(overlay);

    overlay.querySelector('#confirmYes').onclick = () => {
        overlay.remove();
        onConfirm();
    };
    overlay.querySelector('#confirmNo').onclick = () => {
        overlay.remove();
    };
}

// Run on page load
document.addEventListener('DOMContentLoaded', initPage);
