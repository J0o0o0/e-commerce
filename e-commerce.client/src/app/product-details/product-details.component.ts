import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { CartService } from '../services/cart.service';

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.css'
})
export class ProductDetailsComponent implements OnInit {
  product: any;
  selectedImage: string = '';
  quantity: number = 1;
  addedToCart: boolean = false;
  cartError: string = '';

  private baseUrl = `${environment.baseUrl}/api/Cart`;

  constructor(
    private route: ActivatedRoute,
    private http: HttpClient,
    private router: Router,
    private cartService: CartService
  ) { }

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');

    this.http.get<any>(`${environment.baseUrl}/api/Product/${id}`).subscribe(res => {
      this.product = res;
      this.selectedImage = res.images?.[0];
    });
  }

  increaseQty(): void {
    
    if (this.product.stock > this.quantity) this.quantity++;
  }

  decreaseQty(): void {
    if (this.quantity > 1) this.quantity--;
  }

  addToCart(): void {
    this.cartError = '';

    this.http.post<any>(`${this.baseUrl}/add`, {
      productId: this.product.id,
      quantity: this.quantity
    }).subscribe({
      next: () => {
        this.addedToCart = true;
        this.cartService.getCartCount();
        setTimeout(() => {
          this.addedToCart = false;
        }, 2000);
      },
      error: (err: HttpErrorResponse) => {
        if (localStorage.getItem('role') == 'Seller') {
          this.cartError = 'Can\'t add to cart using "seller account"';
        }
        else if (err.status === 401) {
          // Not logged in → redirect to login
          this.router.navigate(['/login']);
        } else {
          // Stock error, server error, etc. → show message
          const errorMsg = err?.error?.message || err?.error || 'Failed to add to cart. Please try again.';
          if (errorMsg.includes("Not enough stock available")) {
            this.cartError = 'Not enough stock available';
          } else {
            this.cartError = typeof errorMsg === 'string' ? errorMsg : 'Failed to add to cart.';
          }
        }
      }
    });
  }

  goToCart(): void {
    this.router.navigate(['/cart']);
  }
}
