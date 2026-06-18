import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../../../environments/environment.prod';

@Component({
  selector: 'app-seller-dashboard',
  templateUrl: './seller-dashboard.component.html',
  styleUrls: ['./seller-dashboard.component.css']
})
export class SellerDashboardComponent implements OnInit {

  // === LAYOUT ===
  currentSection: string = 'products';

  // === PRODUCTS ===
  products: any[] = [];
  loading: boolean = true;
  submitting: boolean = false;
  errorMessages: string[] = [];
  editingProductId: number | null = null;

  productForm_data: any = {
    name: '',
    description: '',
    price: 0,
    stock: 0,
    categoryName: '',
    categoryId: 0,
    images: [''],
    isActive: true
  };

  // === ORDERS ===
  orders: any[] = [];
  ordersLoading: boolean = true;
  selectedOrder: any = null;
  currentPage: number = 1;
  pageSize: number = 10;
  totalPages: number = 0;
  totalCount: number = 0;
  orderStatusFilter: number | null = null;

  private baseUrl = `${environment.baseUrl}/Product`;
  private orderUrl = `${environment.baseUrl}/Order`;

  constructor(
    private http: HttpClient,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.validateAccess();
    this.loadProducts();
  }

  // ========================
  // SECTION SWITCHING
  // ========================

  showSection(section: string): void {
    this.currentSection = section;
    this.errorMessages = [];
    this.selectedOrder = null;

    if (section === 'add-product') {
      this.resetForm();
    }

    if (section === 'orders') {
      this.loadOrders();
    }
  }

  // ========================
  // PRODUCTS
  // ========================

  loadProducts(): void {
    this.loading = true;
    const sellerId = localStorage.getItem('userId');

    this.http.get<any>(`${this.baseUrl}/seller/${sellerId}`).subscribe({
      next: (res) => {
        this.products = Array.isArray(res) ? res : res.data || [];
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  resetForm(): void {
    this.editingProductId = null;
    this.errorMessages = [];
    this.productForm_data = {
      name: '',
      description: '',
      price: 0,
      stock: 0,
      categoryName: '',
      categoryId: 0,
      images: [''],
      isActive: true
    };
  }

  editProduct(product: any): void {
    this.editingProductId = product.id;
    this.currentSection = 'edit-product';
    this.errorMessages = [];

    this.productForm_data = {
      name: product.name,
      description: product.description,
      price: product.price,
      stock: product.stock,
      categoryName: product.categoryName,
      categoryId: product.categoryId,
      images: product.images?.length ? [...product.images] : [''],
      isActive: product.isActive
    };
  }

  deleteProduct(productId: number): void {
    if (!confirm('Are you sure you want to delete this product?')) return;

    this.http.delete<any>(`${this.baseUrl}/${productId}`).subscribe({
      next: () => {
        this.loadProducts();
      },
      error: () => {
        alert('Failed to delete product.');
      }
    });
  }

  addImageInput(): void {
    this.productForm_data.images.push('');
  }

  removeImageInput(index: number): void {
    if (this.productForm_data.images.length <= 1) return;
    this.productForm_data.images.splice(index, 1);
  }

  onSubmitProduct(): void {
    this.errorMessages = [];
    this.submitting = true;

    if (!this.productForm_data.name || !this.productForm_data.description ||
      !this.productForm_data.price || !this.productForm_data.stock ||
      !this.productForm_data.categoryName) {
      this.errorMessages = ['All fields are required.'];
      this.submitting = false;
      return;
    }

    const body = {
      ...this.productForm_data,
      images: this.productForm_data.images.filter((img: string) => img.trim() !== '')
    };

    const isEdit = this.currentSection === 'edit-product' && this.editingProductId;

    if (isEdit) {
      this.http.put<any>(`${this.baseUrl}/${this.editingProductId}`, body).subscribe({
        next: () => {
          this.submitting = false;
          this.showSection('products');
          this.loadProducts();
        },
        error: (err: any) => {
          this.submitting = false;
          const errorMsg = err?.error?.message || err?.error || 'Failed to update product.';
          this.errorMessages = typeof errorMsg === 'string' ? errorMsg.split(', ') : ['Failed to update product.'];
        }
      });
    } else {
      this.http.post<any>(this.baseUrl, body).subscribe({
        next: () => {
          this.submitting = false;
          this.showSection('products');
          this.loadProducts();
        },
        error: (err: any) => {
          this.submitting = false;
          const errorMsg = err?.error?.message || err?.error || 'Failed to add product.';
          this.errorMessages = typeof errorMsg === 'string' ? errorMsg.split(', ') : ['Failed to add product.'];
        }
      });
    }
  }

  // ========================
  // ORDERS
  // ========================

  loadOrders(): void {
    this.ordersLoading = true;

    let url = `${this.orderUrl}/seller?page=${this.currentPage}&pageSize=${this.pageSize}`;
    if (this.orderStatusFilter !== null) {
      url += `&status=${this.orderStatusFilter}`;
    }

    this.http.get<any>(url).subscribe({
      next: (res) => {
        this.orders = res.data || [];
        this.totalPages = res.totalPages || 0;
        this.totalCount = res.totalCount || 0;
        this.ordersLoading = false;
      },
      error: () => {
        this.ordersLoading = false;
      }
    });
  }

  filterOrders(status: number | null): void {
    this.orderStatusFilter = status;
    this.currentPage = 1;
    this.loadOrders();
  }

  changePage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
    this.loadOrders();
  }

  getPageNumbers(): number[] {
    const pages: number[] = [];
    const maxVisible = 5;
    let start = Math.max(1, this.currentPage - Math.floor(maxVisible / 2));
    let end = Math.min(this.totalPages, start + maxVisible - 1);

    if (end - start < maxVisible - 1) {
      start = Math.max(1, end - maxVisible + 1);
    }

    for (let i = start; i <= end; i++) {
      pages.push(i);
    }

    return pages;
  }

  viewOrderDetail(order: any): void {
    this.selectedOrder = null;
    const url = `${this.orderUrl}/seller/${order.orderId}`;

    this.http.get<any>(url).subscribe({
      next: (res) => {
        this.selectedOrder = res;
      },
      error: () => {
        alert('Failed to load order details.');
      }
    });
  }

  closeOrderDetail(): void {
    this.selectedOrder = null;
    this.loadOrders();
  }

  updateItemStatus(orderItemId: number, newStatus: number): void {
    if (!confirm('Are you sure you want to update this item status?')) return;

    this.http.put<any>(`${this.orderUrl}/seller/item/${orderItemId}/status`, {
      newStatus: newStatus
    }).subscribe({
      next: () => {
        if (this.selectedOrder) {
          this.viewOrderDetail({ orderId: this.selectedOrder.orderId });
        }
        this.loadOrders();
      },
      error: (err: any) => {
        const msg = err?.error?.message || err?.error || 'Failed to update status.';
        alert(typeof msg === 'string' ? msg : 'Failed to update status.');
      }
    });
  }

  canUpdateStatus(itemStatus: number): boolean {
    return itemStatus === 1 || itemStatus === 2 || itemStatus === 3 || itemStatus === 4 || itemStatus === 5;
  }

  // ========================
  // AUTH
  // ========================

  private validateAccess(): void {
    const token = localStorage.getItem('token');
    const role = localStorage.getItem('role');

    if (!token || role !== 'Seller') {
      this.router.navigate(['/login']);
      return;
    }

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const isExpired = payload.exp * 1000 < Date.now();

      if (isExpired) {
        localStorage.clear();
        this.router.navigate(['/login']);
      }
    } catch {
      localStorage.clear();
      this.router.navigate(['/login']);
    }
  }

  // ========================
  // HELPERS
  // ========================

  img(url: string): string {
    return url && url.trim() ? url : '/assets/no-image.png';
  }

  getOrderStatusLabel(status: number): string {
    const labels: any = {
      1: 'Pending',
      2: 'Processing',
      3: 'Shipped',
      4: 'Out for Delivery',
      5: 'Delivered',
      6: 'Cancelled'
    };
    return labels[status] || 'Unknown';
  }

  getOrderStatusClass(status: number): string {
    const classes: any = {
      1: 'pending',
      2: 'processing',
      3: 'shipped',
      4: 'outfordelivery',
      5: 'delivered',
      6: 'cancelled'
    };
    return classes[status] || 'pending';
  }

  getItemStatusLabel(status: number): string {
    const labels: any = {
      1: 'Pending',
      2: 'Approved',
      3: 'Processing',
      4: 'Shipped',
      5: 'out-for-delivery',
      6: 'Delivered',
      7: 'Cancelled'
    };
    return labels[status] || 'Unknown';
  }

  getItemStatusClass(status: number): string {
    const classes: any = {
      1: 'pending',
      2: 'approved',
      3: 'processing',
      4: 'shipped',
      5: 'out-for-delivery',
      6: 'delivered',
      7: 'cancelled'
    };
    return classes[status] || 'pending';
  }

  getPaymentLabel(provider: number): string {
    switch (provider) {
      case 1: return 'Cash on Delivery';
      case 2: return 'Stripe';
      case 3: return 'Paymob';
      default: return 'Unknown';
    }
  }

  getPaymentStatusLabel(status: number): string {
    const labels: any = {
      1: 'Pending',
      2: 'Paid',
      3: 'Failed',
      4: 'Refunded'
    };
    return labels[status] || 'Unknown';
  }

  getPaymentStatusClass(status: number): string {
    const classes: any = {
      1: 'pending-payment',
      2: 'paid',
      3: 'refunded',
      4: 'refunded'
    };
    return classes[status] || '';
  }
}
