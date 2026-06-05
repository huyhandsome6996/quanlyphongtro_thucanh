// === phong.js - Room Management ===

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

// Global room data
let allRooms = [];

// Initialize
async function initPage() {
    const user = checkLogin();
    if (!user) return;

    document.getElementById('userGreeting').textContent = user.hoTen || user.tenDangNhap || 'Admin';
    document.getElementById('userAvatar').textContent = (user.hoTen || user.tenDangNhap || 'A').charAt(0).toUpperCase();

    await loadRooms();
}

// Load rooms
async function loadRooms() {
    try {
        const result = await API.get('/api/phongtro');
        if (result.success) {
            allRooms = result.data || [];
            renderTable(allRooms);
        } else {
            showToast('Không thể tải danh sách phòng!', 'error');
        }
    } catch (error) {
        console.error('Error loading rooms:', error);
        showToast('Lỗi kết nối server!', 'error');
        document.getElementById('tableBody').innerHTML = `
            <tr><td colspan="7">
                <div class="empty-state">
                    <div class="empty-icon">⚠️</div>
                    <p>Không thể kết nối đến server</p>
                </div>
            </td></tr>`;
    }
}

// Render table
function renderTable(rooms) {
    const tbody = document.getElementById('tableBody');

    if (!rooms || rooms.length === 0) {
        tbody.innerHTML = `
            <tr><td colspan="7">
                <div class="empty-state">
                    <div class="empty-icon">🏢</div>
                    <p>Chưa có phòng nào</p>
                </div>
            </td></tr>`;
        return;
    }

    tbody.innerHTML = rooms.map((room, index) => `
        <tr>
            <td>${index + 1}</td>
            <td><strong>${room.maPhong}</strong></td>
            <td>${room.tenPhong}</td>
            <td>${room.loaiPhong}</td>
            <td class="text-bold">${formatCurrency(room.giaThue)}</td>
            <td>
                <span class="badge ${room.trangThai === 'Trống' ? 'badge-success' : 'badge-danger'}">
                    ${room.trangThai}
                </span>
            </td>
            <td class="actions">
                <button class="btn btn-info btn-sm" onclick="openEditModal('${room.maPhong}')">✏️ Sửa</button>
                <button class="btn btn-danger btn-sm" onclick="deleteRoom('${room.maPhong}')">🗑️ Xóa</button>
            </td>
        </tr>
    `).join('');
}

// Search rooms
function searchRooms() {
    const keyword = document.getElementById('txtTimKiem').value.trim().toLowerCase();
    if (!keyword) {
        renderTable(allRooms);
        return;
    }
    const filtered = allRooms.filter(r =>
        r.maPhong.toLowerCase().includes(keyword) ||
        r.tenPhong.toLowerCase().includes(keyword) ||
        r.loaiPhong.toLowerCase().includes(keyword) ||
        r.trangThai.toLowerCase().includes(keyword)
    );
    renderTable(filtered);
}

// Enter key search
document.addEventListener('DOMContentLoaded', () => {
    document.getElementById('txtTimKiem').addEventListener('keypress', (e) => {
        if (e.key === 'Enter') searchRooms();
    });
});

// Open Add Modal
function openAddModal() {
    document.getElementById('modalTitle').textContent = 'Thêm phòng mới';
    document.getElementById('editMode').value = 'add';
    document.getElementById('txtMaPhong').value = '';
    document.getElementById('txtMaPhong').readOnly = false;
    document.getElementById('txtTenPhong').value = '';
    document.getElementById('cboLoaiPhong').value = 'Phòng quạt';
    document.getElementById('txtGiaThue').value = '';
    document.getElementById('cboTrangThai').value = 'Trống';
    clearValidation();
    document.getElementById('modal').style.display = 'flex';
}

// Open Edit Modal
async function openEditModal(maPhong) {
    try {
        const result = await API.get(`/api/phongtro/${maPhong}`);
        if (!result.success) {
            showToast('Không thể tải thông tin phòng!', 'error');
            return;
        }
        const room = result.data;
        document.getElementById('modalTitle').textContent = 'Sửa phòng';
        document.getElementById('editMode').value = 'edit';
        document.getElementById('txtMaPhong').value = room.maPhong;
        document.getElementById('txtMaPhong').readOnly = true;
        document.getElementById('txtTenPhong').value = room.tenPhong;
        document.getElementById('cboLoaiPhong').value = room.loaiPhong;
        document.getElementById('txtGiaThue').value = room.giaThue;
        document.getElementById('cboTrangThai').value = room.trangThai;
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
    const txtMaPhong = document.getElementById('txtMaPhong');
    const txtTenPhong = document.getElementById('txtTenPhong');
    const txtGiaThue = document.getElementById('txtGiaThue');

    if (editMode === 'add' && !txtMaPhong.value.trim()) {
        txtMaPhong.classList.add('is-invalid');
        valid = false;
    }

    if (!txtTenPhong.value.trim()) {
        txtTenPhong.classList.add('is-invalid');
        valid = false;
    }

    if (!txtGiaThue.value || parseFloat(txtGiaThue.value) <= 0) {
        txtGiaThue.classList.add('is-invalid');
        valid = false;
    }

    return valid;
}

// Save room
async function saveRoom() {
    if (!validateForm()) return;

    const editMode = document.getElementById('editMode').value;
    const data = {
        maPhong: document.getElementById('txtMaPhong').value.trim(),
        tenPhong: document.getElementById('txtTenPhong').value.trim(),
        loaiPhong: document.getElementById('cboLoaiPhong').value,
        giaThue: parseFloat(document.getElementById('txtGiaThue').value),
        trangThai: document.getElementById('cboTrangThai').value
    };

    try {
        let result;
        if (editMode === 'add') {
            result = await API.post('/api/phongtro', data);
        } else {
            result = await API.put(`/api/phongtro/${data.maPhong}`, data);
        }

        if (result.success) {
            showToast(editMode === 'add' ? 'Thêm phòng thành công!' : 'Cập nhật phòng thành công!', 'success');
            closeModal();
            await loadRooms();
        } else {
            showToast(result.message || 'Thao tác thất bại!', 'error');
        }
    } catch (error) {
        showToast('Lỗi kết nối server!', 'error');
    }
}

// Delete room
function deleteRoom(maPhong) {
    showConfirm('Xác nhận xóa', `Bạn có chắc muốn xóa phòng "${maPhong}"?`, async () => {
        try {
            const result = await API.delete(`/api/phongtro/${maPhong}`);
            if (result.success) {
                showToast('Xóa phòng thành công!', 'success');
                await loadRooms();
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
