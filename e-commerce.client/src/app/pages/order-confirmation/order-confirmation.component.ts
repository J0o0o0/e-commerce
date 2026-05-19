import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-order-confirmation',
  templateUrl: './order-confirmation.component.html',
  styleUrls: ['./order-confirmation.component.css']
})
export class OrderConfirmationComponent implements OnInit {

  order: any;

  constructor(private router: Router) { }

  ngOnInit(): void {
    const navigation = this.router.getCurrentNavigation();
    this.order = navigation?.extras?.state?.['order'];

    // If no order data, redirect to home
    if (!this.order) {
      this.router.navigate(['/']);
    }
  }

  getPaymentLabel(provider: number): string {
    switch (provider) {
      case 1: return 'Cash on Delivery';
      case 2: return 'Stripe';
      case 3: return 'Paymob';
      default: return 'Unknown';
    }
  }
}
