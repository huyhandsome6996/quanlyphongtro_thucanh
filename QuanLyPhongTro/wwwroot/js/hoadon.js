// === hoadon.js - View Invoices ===

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
const GIA_DIEN = 3500;
const GIA_NUOC = 15000;

// Global data
let allInvoices = [];
let allContracts = [];

// Initialize
async function initPage() {
    const user = checkLogin();
    if (!user) return;

    document.getElementById('userGreeting').textContent = user.hoTen || user.tenDangNhap || 'Admin';
    document.getElementById('userAvatar').textContent = (user.hoTen || user.tenDangNhap || 'A').charAt(0).toUpperCase();

    await loadInvoices();
}

// Load invoices
async function loadInvoices() {
    try {
        const result = await API.get('/api/hoadon');
        if (result.success) {
            allInvoices = result.data || [];
            renderTable(allInvoices);
            loadContractFilter();
        } else {
            showToast('Không thể tải danh sách hóa đơn!', 'error');
        }
    } catch (error) {
        console.error('Error loading invoices:', error);
        showToast('Lỗi kết nối server!', 'error');
        document.getElementById('tableBody').innerHTML = `
            <tr><td colspan="9">
                <div class="empty-state">
                    <div class="empty-icon">⚠️</div>
                    <p>Không thể kết nối đến server</p>
                </div>
            </td></tr>`;
    }
}

// Load contract filter dropdown
function loadContractFilter() {
    const contracts = [...new Set(allInvoices.map(i => i.maHopDong))];
    const cboFilter = document.getElementById('cboHopDongFilter');
    cboFilter.innerHTML = '<option value="">Tất cả</option>' +
        contracts.map(c => `<option value="${c}">${c}</option>`).join('');
}

// Render table
function renderTable(invoices) {
    const tbody = document.getElementById('tableBody');

    if (!invoices || invoices.length === 0) {
        tbody.innerHTML = `
            <tr><td colspan="9">
                <div class="empty-state">
                    <div class="empty-icon">💰</div>
                    <p>Chưa có hóa đơn nào</p>
                </div>
            </td></tr>`;
        return;
    }

    tbody.innerHTML = invoices.map((invoice, index) => {
        const soDien = (invoice.soDienMoi || 0) - (invoice.soDienCu || 0);
        const soNuoc = (invoice.soNuocMoi || 0) - (invoice.soNuocCu || 0);
        const isPaid = invoice.trangThaiThanhToan === 'Đã thanh toán';

        return `
        <tr>
            <td>${index + 1}</td>
            <td><strong>${invoice.maHoaDon}</strong></td>
            <td>${invoice.maHopDong}</td>
            <td>${invoice.thangNam}</td>
            <td>${invoice.soDienCu} → ${invoice.soDienMoi} <small>(${soDien} kWh)</small></td>
            <td>${invoice.soNuocCu} → ${invoice.soNuocMoi} <small>(${soNuoc} m³)</small></td>
            <td class="text-bold">${formatCurrency(invoice.tongTien)}</td>
            <td>
                <span class="badge ${isPaid ? 'badge-success' : 'badge-warning'}">
                    ${invoice.trangThaiThanhToan}
                </span>
            </td>
            <td class="actions">
                <button class="btn btn-info btn-sm" onclick="showDetail('${invoice.maHoaDon}')">👁️ Chi tiết</button>
                ${!isPaid ? `<button class="btn btn-warning btn-sm" onclick="markAsPaid('${invoice.maHoaDon}')">💳 Thanh toán</button>` : ''}
            </td>
        </tr>`;
    }).join('');
}

// Search invoices
function searchInvoices() {
    const keyword = document.getElementById('txtTimKiem').value.trim().toLowerCase();
    if (!keyword) {
        filterInvoices();
        return;
    }
    const filtered = allInvoices.filter(i =>
        i.maHoaDon.toLowerCase().includes(keyword) ||
        i.maHopDong.toLowerCase().includes(keyword) ||
        i.thangNam.toLowerCase().includes(keyword)
    );
    renderTable(filtered);
}

// Filter invoices
function filterInvoices() {
    const contractFilter = document.getElementById('cboHopDongFilter').value;
    const monthFilter = document.getElementById('txtThangFilter').value.trim().toLowerCase();

    let filtered = allInvoices;

    if (contractFilter) {
        filtered = filtered.filter(i => i.maHopDong === contractFilter);
    }

    if (monthFilter) {
        filtered = filtered.filter(i => i.thangNam.toLowerCase().includes(monthFilter));
    }

    renderTable(filtered);
}

// Clear filters
function clearFilters() {
    document.getElementById('cboHopDongFilter').value = '';
    document.getElementById('txtThangFilter').value = '';
    document.getElementById('txtTimKiem').value = '';
    renderTable(allInvoices);
}

// Enter key search
document.addEventListener('DOMContentLoaded', () => {
    document.getElementById('txtTimKiem').addEventListener('keypress', (e) => {
        if (e.key === 'Enter') searchInvoices();
    });
});

// Show detail modal
function showDetail(maHoaDon) {
    const invoice = allInvoices.find(i => i.maHoaDon === maHoaDon);
    if (!invoice) return;

    const soDien = Math.max(0, (invoice.soDienMoi || 0) - (invoice.soDienCu || 0));
    const soNuoc = Math.max(0, (invoice.soNuocMoi || 0) - (invoice.soNuocCu || 0));
    const tienDien = soDien * GIA_DIEN;
    const tienNuoc = soNuoc * GIA_NUOC;
    const tienPhong = invoice.tongTien - tienDien - tienNuoc;

    const modalBody = document.getElementById('modalBody');
    modalBody.innerHTML = `
        <div class="invoice-detail">
            <div class="detail-row">
                <span class="detail-label">Mã hóa đơn:</span>
                <span class="detail-value">${invoice.maHoaDon}</span>
            </div>
            <div class="detail-row">
                <span class="detail-label">Mã hợp đồng:</span>
                <span class="detail-value">${invoice.maHopDong}</span>
            </div>
            <div class="detail-row">
                <span class="detail-label">Tháng/Năm:</span>
                <span class="detail-value">${invoice.thangNam}</span>
            </div>
            <div class="detail-row">
                <span class="detail-label">Trạng thái:</span>
                <span class="detail-value">
                    <span class="badge ${invoice.trangThaiThanhToan === 'Đã thanh toán' ? 'badge-success' : 'badge-warning'}">
                        ${invoice.trangThaiThanhToan}
                    </span>
                </span>
            </div>
            <div style="margin:16px 0; border-top:1px solid var(--border-color);"></div>
            <div class="detail-row">
                <span class="detail-label">Số điện cũ:</span>
                <span class="detail-value">${invoice.soDienCu} kWh</span>
            </div>
            <div class="detail-row">
                <span class="detail-label">Số điện mới:</span>
                <span class="detail-value">${invoice.soDienMoi} kWh</span>
            </div>
            <div class="detail-row">
                <span class="detail-label">Tiêu thụ điện:</span>
                <span class="detail-value">${soDien} kWh</span>
            </div>
            <div class="detail-row">
                <span class="detail-label">Tiền điện (${formatCurrency(GIA_DIEN)}/kWh):</span>
                <span class="detail-value text-bold">${formatCurrency(tienDien)}</span>
            </div>
            <div style="margin:12px 0; border-top:1px dashed var(--border-color);"></div>
            <div class="detail-row">
                <span class="detail-label">Số nước cũ:</span>
                <span class="detail-value">${invoice.soNuocCu} m³</span>
            </div>
            <div class="detail-row">
                <span class="detail-label">Số nước mới:</span>
                <span class="detail-value">${invoice.soNuocMoi} m³</span>
            </div>
            <div class="detail-row">
                <span class="detail-label">Tiêu thụ nước:</span>
                <span class="detail-value">${soNuoc} m³</span>
            </div>
            <div class="detail-row">
                <span class="detail-label">Tiền nước (${formatCurrency(GIA_NUOC)}/m³):</span>
                <span class="detail-value text-bold">${formatCurrency(tienNuoc)}</span>
            </div>
            <div style="margin:12px 0; border-top:1px dashed var(--border-color);"></div>
            <div class="detail-row">
                <span class="detail-label">Tiền phòng:</span>
                <span class="detail-value text-bold">${formatCurrency(tienPhong > 0 ? tienPhong : 0)}</span>
            </div>
            <div class="detail-row total">
                <span class="detail-label">TỔNG CỘNG:</span>
                <span class="detail-value">${formatCurrency(invoice.tongTien)}</span>
            </div>
        </div>
    `;

    document.getElementById('modal').style.display = 'flex';
}

// Close modal
function closeModal() {
    document.getElementById('modal').style.display = 'none';
}

// Mark as paid
function markAsPaid(maHoaDon) {
    showConfirm('Xác nhận thanh toán', `Bạn có chắc muốn đánh dấu hóa đơn "${maHoaDon}" là đã thanh toán?`, async () => {
        try {
            const result = await API.put(`/api/hoadon/${maHoaDon}/thanhtoan`, {});
            if (result.success) {
                showToast('Đánh dấu thanh toán thành công!', 'success');
                await loadInvoices();
            } else {
                showToast(result.message || 'Cập nhật thất bại!', 'error');
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
