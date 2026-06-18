import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { environment } from '../../../environments/environment.prod';

@Component({
  selector: 'app-order-details',
  templateUrl: './order-details.component.html',
  styleUrls: ['./order-details.component.css']
})
export class OrderDetailsComponent implements OnInit {

  order: any;
  loading: boolean = true;
  private orderUrl = `${environment.baseUrl}/Order`;

  constructor(
    private http: HttpClient,
    private router: Router,
    private route: ActivatedRoute
  ) { }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadOrder(parseInt(id));
    } else {
      this.router.navigate(['/orders']);
    }
  }

  loadOrder(orderId: number): void {
    this.loading = true;
    this.http.get<any>(`${this.orderUrl}/${orderId}`).subscribe({
      next: (res) => {
        this.order = res;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.router.navigate(['/orders']);
      }
    });
  }

  cancelOrder(): void {
    if (!confirm('Are you sure you want to cancel this order?')) return;

    this.http.post<any>(`${this.orderUrl}/${this.order.id}/cancel`, {}).subscribe({
      next: () => {
        this.loadOrder(this.order.id);
      },
      error: (err: any) => {
        const msg = err?.error?.message || err?.error || 'Failed to cancel order.';
        alert(typeof msg === 'string' ? msg : 'Failed to cancel order.');
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/orders']);
  }

  getPaymentLabel(provider: number): string {
    switch (provider) {
      case 1: return 'Cash on Delivery';
      case 2: return 'Stripe';
      case 3: return 'Paymob';
      default: return 'Unknown';
    }
  }

  getStatusLabel(status: number): string {
    const labels: any = {
      1: 'Pending', 2: 'Processing', 3: 'Shipped',
      4: 'Out for Delivery', 5: 'Delivered', 6: 'Cancelled'
    };
    return labels[status] || 'Unknown';
  }

  getStatusClass(status: number): string {
    const classes: any = {
      1: 'pending', 2: 'processing', 3: 'shipped',
      4: 'outfordelivery', 5: 'delivered', 6: 'cancelled'
    };
    return classes[status] || 'pending';
  }
}
