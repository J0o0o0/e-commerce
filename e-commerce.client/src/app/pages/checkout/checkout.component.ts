import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.css']
})
export class CheckoutComponent implements OnInit {

  cartItems: any[] = [];
  totalPrice: number = 0;
  loading: boolean = true;
  submitting: boolean = false;
  editingAddress: boolean = false;
  shippingAddress: string = '';
  selectedPayment: number = 1; // CashOnDelivery
  errorMessages: string[] = [];

  private cartUrl = `${environment.baseUrl}/api/Cart`;
  private orderUrl = `${environment.baseUrl}/api/Order`;

  constructor(
    private http: HttpClient,
    private router: Router,
    private cartService: CartService
  ) { }

  ngOnInit(): void {
    this.loadCart();
  }

  loadCart(): void {
    this.loading = true;
    this.http.get<any>(this.cartUrl).subscribe({
      next: (res) => {
        this.cartItems = res.data.items;
        this.totalPrice = res.data.totalPrice;
        this.shippingAddress = res.data.shippingAddress //localStorage.getItem('shippingAddress') || '';
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  saveAddress(): void {
    // Save to localStorage for now — can update via API later
    localStorage.setItem('shippingAddress', this.shippingAddress);
    this.editingAddress = false;
  }

  placeOrder(): void {
    this.errorMessages = [];
    this.submitting = true;

    if (!this.shippingAddress.trim()) {
      this.errorMessages = ['Please add a shipping address.'];
      this.submitting = false;
      return;
    }

    this.http.post<any>(`${this.orderUrl}/place`, {
      paymentProvider: this.selectedPayment
    }).subscribe({
      next: (res) => {
        this.cartService.resetCount();
        this.router.navigate(['/order-confirmation'], {
          state: { order: res }
        });
      },
      error: (err: any) => {
        this.submitting = false;
        const errorMsg = err?.error?.message || err?.error || 'Failed to place order.';
        this.errorMessages = typeof errorMsg === 'string' ? errorMsg.split('. ') : ['Failed to place order.'];
      }
    });
  }
}
