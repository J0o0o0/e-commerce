import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css']
})
export class CartComponent implements OnInit {

  cartItems: any[] = [];
  totalPrice: number = 0;
  loading: boolean = true;
  errorMessage: string = '';

  private baseUrl = `${environment.baseUrl}/api/Cart`;

  constructor(
    private http: HttpClient,
    private cartService: CartService
  ) { }

  ngOnInit(): void {
    this.loadCart();
  }

  loadCart(): void {
    this.loading = true;
    this.http.get<any>(this.baseUrl).subscribe({
      next: (res) => {
        this.cartItems = res.data.items;
        this.totalPrice = res.data.totalPrice;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  increaseQuantity(item: any): void {
    this.http.post<any>(`${this.baseUrl}/add`, {
      productId: item.productId,
      quantity: 1
    }).subscribe({
      next: () => this.loadCart()
    ,
      error: (err: any) => {
        const errorMsg = err?.error?.message || err?.error || '';
        const msg = typeof errorMsg === 'string' ? errorMsg : '';

        if (msg.toLowerCase().includes('stock') || msg.toLowerCase().includes('quantity') || msg.toLowerCase().includes('available')) {
          item.stockError = 'stock limit reached.';
        } else {
          item.stockError = 'Something went wrong. Please try again.';
        }

        // Auto-dismiss after 4 seconds
        setTimeout(() => {
          item.stockError = '';
        }, 4000);
      }
    });
  }

  decreaseQuantity(item: any): void {
    if (item.quantity <= 1) return;

    this.http.post<any>(`${this.baseUrl}/add`, {
      productId: item.productId,
      quantity: -1
    }).subscribe({
      next: () => this.loadCart()
    });
  }

  removeItem(productId: number): void {
    this.http.delete<any>(`${this.baseUrl}/remove/${productId}`).subscribe({
      next: () => this.loadCart()
    });
  }

  clearCart(): void {
    this.http.delete<any>(`${this.baseUrl}/clear`).subscribe({
      next: () => this.loadCart()
    });
    this.cartService.resetCount();
  }
}
