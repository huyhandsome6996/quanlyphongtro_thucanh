// === tinhtien.js - Calculate Utilities ===

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

// Constants
const GIA_DIEN = 3500;  // per kWh
const GIA_NUOC = 15000; // per m3

// Global data
let activeContracts = [];
let contractDetails = {};

// Initialize
async function initPage() {
    const user = checkLogin();
    if (!user) return;

    document.getElementById('userGreeting').textContent = user.hoTen || user.tenDangNhap || 'Admin';
    document.getElementById('userAvatar').textContent = (user.hoTen || user.tenDangNhap || 'A').charAt(0).toUpperCase();

    await loadActiveContracts();
}

// Load active contracts
async function loadActiveContracts() {
    try {
        const result = await API.get('/api/hopdong/active');
        if (result.success) {
            activeContracts = result.data || [];
            const cboHopDong = document.getElementById('cboHopDong');
            cboHopDong.innerHTML = '<option value="">-- Chọn hợp đồng --</option>' +
                activeContracts.map(c => {
                    contractDetails[c.maHopDong] = c;
                    return `<option value="${c.maHopDong}">${c.maHopDong} - ${c.tenPhong || c.maPhong} - ${c.hoTen || c.maKhach}</option>`;
                }).join('');
        }
    } catch (error) {
        console.error('Error loading contracts:', error);
        showToast('Không thể tải danh sách hợp đồng!', 'error');
    }
}

// When contract changes
function onContractChange() {
    const maHopDong = document.getElementById('cboHopDong').value;
    const txtGiaThue = document.getElementById('txtGiaThue');

    if (maHopDong && contractDetails[maHopDong]) {
        const contract = contractDetails[maHopDong];
        txtGiaThue.value = formatCurrency(contract.giaThue || 0);
    } else {
        txtGiaThue.value = '';
    }

    calculate();
}

// Calculate totals in real-time
function calculate() {
    const maHopDong = document.getElementById('cboHopDong').value;
    let giaThue = 0;

    if (maHopDong && contractDetails[maHopDong]) {
        giaThue = contractDetails[maHopDong].giaThue || 0;
    }

    const soDienCu = parseFloat(document.getElementById('txtSoDienCu').value) || 0;
    const soDienMoi = parseFloat(document.getElementById('txtSoDienMoi').value) || 0;
    const soNuocCu = parseFloat(document.getElementById('txtSoNuocCu').value) || 0;
    const soNuocMoi = parseFloat(document.getElementById('txtSoNuocMoi').value) || 0;

    const soDien = Math.max(0, soDienMoi - soDienCu);
    const soNuoc = Math.max(0, soNuocMoi - soNuocCu);

    const tienDien = soDien * GIA_DIEN;
    const tienNuoc = soNuoc * GIA_NUOC;
    const tongCong = tienDien + tienNuoc + giaThue;

    document.getElementById('lblTienDien').textContent = `${soDien} kWh × ${formatCurrency(GIA_DIEN)} = ${formatCurrency(tienDien)}`;
    document.getElementById('lblTienNuoc').textContent = `${soNuoc} m³ × ${formatCurrency(GIA_NUOC)} = ${formatCurrency(tienNuoc)}`;
    document.getElementById('lblTienPhong').textContent = formatCurrency(giaThue);
    document.getElementById('lblTongCong').textContent = formatCurrency(tongCong);
}

// Validate form
function validateForm() {
    let valid = true;
    document.querySelectorAll('.form-control.is-invalid').forEach(el => el.classList.remove('is-invalid'));

    const cboHopDong = document.getElementById('cboHopDong');
    const txtThangNam = document.getElementById('txtThangNam');
    const txtSoDienCu = document.getElementById('txtSoDienCu');
    const txtSoDienMoi = document.getElementById('txtSoDienMoi');
    const txtSoNuocCu = document.getElementById('txtSoNuocCu');
    const txtSoNuocMoi = document.getElementById('txtSoNuocMoi');

    if (!cboHopDong.value) {
        cboHopDong.classList.add('is-invalid');
        valid = false;
    }

    // Validate month/year format MM/YYYY
    const thangNam = txtThangNam.value.trim();
    const regex = /^(0[1-9]|1[0-2])\/\d{4}$/;
    if (!thangNam || !regex.test(thangNam)) {
        txtThangNam.classList.add('is-invalid');
        valid = false;
    }

    if (txtSoDienCu.value === '' || parseFloat(txtSoDienCu.value) < 0) {
        txtSoDienCu.classList.add('is-invalid');
        valid = false;
    }

    if (txtSoDienMoi.value === '' || parseFloat(txtSoDienMoi.value) < 0) {
        txtSoDienMoi.classList.add('is-invalid');
        valid = false;
    }

    if (txtSoNuocCu.value === '' || parseFloat(txtSoNuocCu.value) < 0) {
        txtSoNuocCu.classList.add('is-invalid');
        valid = false;
    }

    if (txtSoNuocMoi.value === '' || parseFloat(txtSoNuocMoi.value) < 0) {
        txtSoNuocMoi.classList.add('is-invalid');
        valid = false;
    }

    if (parseFloat(txtSoDienMoi.value) < parseFloat(txtSoDienCu.value)) {
        txtSoDienMoi.classList.add('is-invalid');
        showToast('Số điện mới phải lớn hơn hoặc bằng số điện cũ!', 'warning');
        valid = false;
    }

    if (parseFloat(txtSoNuocMoi.value) < parseFloat(txtSoNuocCu.value)) {
        txtSoNuocMoi.classList.add('is-invalid');
        showToast('Số nước mới phải lớn hơn hoặc bằng số nước cũ!', 'warning');
        valid = false;
    }

    return valid;
}

// Save invoice
async function saveInvoice() {
    if (!validateForm()) return;

    const data = {
        maHoaDon: document.getElementById('txtMaHoaDon').value.trim() || undefined,
        maHopDong: document.getElementById('cboHopDong').value,
        thangNam: document.getElementById('txtThangNam').value.trim(),
        soDienCu: parseFloat(document.getElementById('txtSoDienCu').value),
        soDienMoi: parseFloat(document.getElementById('txtSoDienMoi').value),
        soNuocCu: parseFloat(document.getElementById('txtSoNuocCu').value),
        soNuocMoi: parseFloat(document.getElementById('txtSoNuocMoi').value)
    };

    try {
        const result = await API.post('/api/hoadon', data);
        if (result.success) {
            showToast('Lập hóa đơn thành công!', 'success');
            resetForm();
        } else {
            showToast(result.message || 'Lập hóa đơn thất bại!', 'error');
        }
    } catch (error) {
        showToast('Lỗi kết nối server!', 'error');
    }
}

// Reset form
function resetForm() {
    document.getElementById('txtMaHoaDon').value = '';
    document.getElementById('cboHopDong').value = '';
    document.getElementById('txtGiaThue').value = '';
    document.getElementById('txtThangNam').value = '';
    document.getElementById('txtSoDienCu').value = '';
    document.getElementById('txtSoDienMoi').value = '';
    document.getElementById('txtSoNuocCu').value = '';
    document.getElementById('txtSoNuocMoi').value = '';
    calculate();
    document.querySelectorAll('.form-control.is-invalid').forEach(el => el.classList.remove('is-invalid'));
}

// Run on page load
document.addEventListener('DOMContentLoaded', initPage);
