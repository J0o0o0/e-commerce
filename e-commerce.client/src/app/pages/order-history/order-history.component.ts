import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-order-history',
  templateUrl: './order-history.component.html',
  styleUrls: ['./order-history.component.css'],
  
})

export class OrderHistoryComponent implements OnInit {

  orders: any[] = [];
  loading: boolean = true;

  private orderUrl = `${environment.baseUrl}/api/Order`;

  constructor(
    private http: HttpClient,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.loading = true;
    this.http.get<any>(this.orderUrl + '/history').subscribe({
      next: (res) => {
        this.orders = res;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  viewOrder(orderId: number): void {
    this.router.navigate(['/orders', orderId]);
  }

  cancelOrder(orderId: number, event: Event): void {
    event.stopPropagation();

    if (!confirm('Are you sure you want to cancel this order?')) return;

    this.http.post<any>(`${this.orderUrl}/${orderId}/cancel`, {}).subscribe({
      next: () => {
        this.loadOrders();
      },
      error: (err: any) => {
        const msg = err?.error?.message || err?.error || 'Failed to cancel order.';
        alert(typeof msg === 'string' ? msg : 'Failed to cancel order.');
      }
    });
  }
  img(url: string): string {
    return url && url.trim() ? url : '/assets/no-image.png';
  }
  getStatusLabel(status: number): string {
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

  getStatusClass(status: number): string {
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
}
