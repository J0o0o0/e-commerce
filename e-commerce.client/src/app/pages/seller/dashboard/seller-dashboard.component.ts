import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { Router } from '@angular/router';

@Component({
  selector: 'app-seller-dashboard',
  templateUrl: './seller-dashboard.component.html',
  styleUrls: ['./seller-dashboard.component.css']
})
export class SellerDashboardComponent implements OnInit {

  currentSection: string = 'products';
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

  private baseUrl = `${environment.baseUrl}/api/Product`;

  constructor(
    private http: HttpClient,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.validateAccess();
    this.loadProducts();
  }
  private validateAccess(): void {
    const token = localStorage.getItem('token');
    const role = localStorage.getItem('role');

    if (!token || role !== 'Seller') {
      this.router.navigate(['/login']);
      return;
    }
    // Double-check token expiry
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
  showSection(section: string): void {
    this.currentSection = section;
    this.errorMessages = [];

    if (section === 'add-product') {
      this.resetForm();
    }
  }

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

    // Validation
    if (!this.productForm_data.name || !this.productForm_data.description ||
      !this.productForm_data.price || !this.productForm_data.stock ||
      !this.productForm_data.categoryName) {
      this.errorMessages = ['All fields are required.'];
      this.submitting = false;
      return;
    }

    // Filter out empty image URLs
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
}
