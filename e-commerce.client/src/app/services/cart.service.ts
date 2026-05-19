import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CartService {

  private baseUrl = `${environment.baseUrl}/api/Cart`;
  private cartCount = new BehaviorSubject<number>(0);
  cartCount$ = this.cartCount.asObservable();

  constructor(private http: HttpClient) { }

  getCartCount(): void {
    this.http.get<any>(this.baseUrl).subscribe({
      next: (res) => {
        const count = res.data?.items?.length || 0;
        this.cartCount.next(count);
      },
      error: () => {
        this.cartCount.next(0);
      }
    });
  }

  updateCount(change: number): void {
    const current = this.cartCount.value;
    this.cartCount.next(current + change);
  }

  resetCount(): void {
    this.cartCount.next(0);
  }
}
