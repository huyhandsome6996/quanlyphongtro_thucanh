// === hopdong.js - Contract Management ===

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

// Format date to Vietnamese
function formatDate(dateStr) {
    if (!dateStr) return '';
    const d = new Date(dateStr);
    return d.toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' });
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

// Global data
let allContracts = [];
let allRooms = [];
let allTenants = [];

// Initialize
async function initPage() {
    const user = checkLogin();
    if (!user) return;

    document.getElementById('userGreeting').textContent = user.hoTen || user.tenDangNhap || 'Admin';
    document.getElementById('userAvatar').textContent = (user.hoTen || user.tenDangNhap || 'A').charAt(0).toUpperCase();

    await loadContracts();
}

// Load contracts
async function loadContracts() {
    try {
        const result = await API.get('/api/hopdong');
        if (result.success) {
            allContracts = result.data || [];
            renderTable(allContracts);
        } else {
            showToast('Không thể tải danh sách hợp đồng!', 'error');
        }
    } catch (error) {
        console.error('Error loading contracts:', error);
        showToast('Lỗi kết nối server!', 'error');
        document.getElementById('tableBody').innerHTML = `
            <tr><td colspan="8">
                <div class="empty-state">
                    <div class="empty-icon">⚠️</div>
                    <p>Không thể kết nối đến server</p>
                </div>
            </td></tr>`;
    }
}

// Render table
function renderTable(contracts) {
    const tbody = document.getElementById('tableBody');

    if (!contracts || contracts.length === 0) {
        tbody.innerHTML = `
            <tr><td colspan="8">
                <div class="empty-state">
                    <div class="empty-icon">📄</div>
                    <p>Chưa có hợp đồng nào</p>
                </div>
            </td></tr>`;
        return;
    }

    tbody.innerHTML = contracts.map((contract, index) => `
        <tr>
            <td>${index + 1}</td>
            <td><strong>${contract.maHopDong}</strong></td>
            <td>${contract.tenPhong || contract.maPhong}</td>
            <td>${contract.hoTen || contract.maKhach}</td>
            <td>${formatDate(contract.ngayBatDau)}</td>
            <td class="text-bold">${formatCurrency(contract.tienCoc)}</td>
            <td>
                <span class="badge ${contract.trangThaiHD === 'Đang hiệu lực' ? 'badge-success' : 'badge-gray'}">
                    ${contract.trangThaiHD}
                </span>
            </td>
            <td class="actions">
                ${contract.trangThaiHD === 'Đang hiệu lực'
                    ? `<button class="btn btn-warning btn-sm" onclick="thanhLyContract('${contract.maHopDong}')">📋 Thanh lý</button>`
                    : `<span class="badge badge-gray">Đã thanh lý</span>`
                }
            </td>
        </tr>
    `).join('');
}

// Search contracts
function searchContracts() {
    const keyword = document.getElementById('txtTimKiem').value.trim().toLowerCase();
    if (!keyword) {
        renderTable(allContracts);
        return;
    }
    const filtered = allContracts.filter(c =>
        c.maHopDong.toLowerCase().includes(keyword) ||
        (c.tenPhong || '').toLowerCase().includes(keyword) ||
        (c.hoTen || '').toLowerCase().includes(keyword) ||
        c.trangThaiHD.toLowerCase().includes(keyword)
    );
    renderTable(filtered);
}

// Enter key search
document.addEventListener('DOMContentLoaded', () => {
    document.getElementById('txtTimKiem').addEventListener('keypress', (e) => {
        if (e.key === 'Enter') searchContracts();
    });
});

// Open Add Modal
async function openAddModal() {
    document.getElementById('modalTitle').textContent = 'Thêm hợp đồng mới';
    document.getElementById('txtMaHopDong').value = '';
    document.getElementById('dtpNgayBatDau').value = new Date().toISOString().split('T')[0];
    document.getElementById('txtTienCoc').value = '0';
    clearValidation();

    // Load dropdowns
    try {
        const [roomsRes, tenantsRes] = await Promise.all([
            API.get('/api/phongtro'),
            API.get('/api/khachthue')
        ]);

        allRooms = roomsRes.data || [];
        allTenants = tenantsRes.data || [];

        // Fill room dropdown (only empty rooms)
        const cboPhong = document.getElementById('cboPhong');
        const emptyRooms = allRooms.filter(r => r.trangThai === 'Trống');
        cboPhong.innerHTML = '<option value="">-- Chọn phòng trống --</option>' +
            emptyRooms.map(r => `<option value="${r.maPhong}">${r.maPhong} - ${r.tenPhong} (${formatCurrency(r.giaThue)})</option>`).join('');

        // Fill tenant dropdown
        const cboKhach = document.getElementById('cboKhach');
        cboKhach.innerHTML = '<option value="">-- Chọn khách thuê --</option>' +
            allTenants.map(t => `<option value="${t.maKhach}">${t.maKhach} - ${t.hoTen}</option>`).join('');

    } catch (error) {
        showToast('Không thể tải dữ liệu phòng/khách!', 'error');
    }

    document.getElementById('modal').style.display = 'flex';
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

    const txtMaHopDong = document.getElementById('txtMaHopDong');
    const cboPhong = document.getElementById('cboPhong');
    const cboKhach = document.getElementById('cboKhach');
    const dtpNgayBatDau = document.getElementById('dtpNgayBatDau');

    if (!txtMaHopDong.value.trim()) {
        txtMaHopDong.classList.add('is-invalid');
        valid = false;
    }

    if (!cboPhong.value) {
        cboPhong.classList.add('is-invalid');
        valid = false;
    }

    if (!cboKhach.value) {
        cboKhach.classList.add('is-invalid');
        valid = false;
    }

    if (!dtpNgayBatDau.value) {
        dtpNgayBatDau.classList.add('is-invalid');
        valid = false;
    }

    return valid;
}

// Save contract
async function saveContract() {
    if (!validateForm()) return;

    const data = {
        maHopDong: document.getElementById('txtMaHopDong').value.trim(),
        maPhong: document.getElementById('cboPhong').value,
        maKhach: document.getElementById('cboKhach').value,
        ngayBatDau: document.getElementById('dtpNgayBatDau').value,
        tienCoc: parseFloat(document.getElementById('txtTienCoc').value) || 0,
        trangThaiHD: 'Đang hiệu lực'
    };

    try {
        const result = await API.post('/api/hopdong', data);
        if (result.success) {
            showToast('Thêm hợp đồng thành công! Phòng đã cập nhật trạng thái.', 'success');
            closeModal();
            await loadContracts();
        } else {
            showToast(result.message || 'Thêm hợp đồng thất bại!', 'error');
        }
    } catch (error) {
        showToast('Lỗi kết nối server!', 'error');
    }
}

// Thanh ly contract
function thanhLyContract(maHopDong) {
    showConfirm('Xác nhận thanh lý', `Bạn có chắc muốn thanh lý hợp đồng "${maHopDong}"? Phòng sẽ chuyển về trạng thái "Trống".`, async () => {
        try {
            const result = await API.put(`/api/hopdong/${maHopDong}/thanhly`, {});
            if (result.success) {
                showToast('Thanh lý hợp đồng thành công!', 'success');
                await loadContracts();
            } else {
                showToast(result.message || 'Thanh lý thất bại!', 'error');
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
