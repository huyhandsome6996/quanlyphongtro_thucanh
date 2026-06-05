// === home.js - Dashboard ===

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

// Toggle sidebar (mobile)
function toggleSidebar() {
    const sidebar = document.getElementById('sidebar');
    const backdrop = document.getElementById('sidebarBackdrop');
    sidebar.classList.toggle('open');
    backdrop.classList.toggle('show');
}

// Initialize dashboard
async function initDashboard() {
    const user = checkLogin();
    if (!user) return;

    // Display user info
    document.getElementById('userGreeting').textContent = user.hoTen || user.tenDangNhap || 'Admin';
    document.getElementById('userAvatar').textContent = (user.hoTen || user.tenDangNhap || 'A').charAt(0).toUpperCase();

    try {
        // Fetch all data in parallel
        const [roomsRes, tenantsRes, contractsRes, invoicesRes] = await Promise.all([
            API.get('/api/phongtro'),
            API.get('/api/khachthue'),
            API.get('/api/hopdong'),
            API.get('/api/hoadon')
        ]);

        // Rooms stats
        const rooms = roomsRes.data || [];
        const totalRooms = rooms.length;
        const occupiedRooms = rooms.filter(r => r.trangThai === 'Đã thuê').length;
        const emptyRooms = rooms.filter(r => r.trangThai === 'Trống').length;

        document.getElementById('statTotalRooms').textContent = totalRooms;
        document.getElementById('statOccupiedRooms').textContent = occupiedRooms;
        document.getElementById('statEmptyRooms').textContent = emptyRooms;

        // Tenants stats
        const tenants = tenantsRes.data || [];
        document.getElementById('statTotalTenants').textContent = tenants.length;

        // Contracts stats
        const contracts = contractsRes.data || [];
        const activeContracts = contracts.filter(c => c.trangThaiHD === 'Đang hiệu lực').length;
        document.getElementById('statActiveContracts').textContent = activeContracts;

        // Invoices stats
        const invoices = invoicesRes.data || [];
        const unpaidInvoices = invoices.filter(i => i.trangThaiThanhToan === 'Chưa thanh toán').length;
        document.getElementById('statUnpaidInvoices').textContent = unpaidInvoices;

    } catch (error) {
        console.error('Error loading dashboard data:', error);
        showToast('Không thể tải dữ liệu bảng điều khiển!', 'error');
    }
}

// Run on page load
document.addEventListener('DOMContentLoaded', initDashboard);
