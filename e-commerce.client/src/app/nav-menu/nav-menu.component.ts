import { Router, NavigationEnd } from '@angular/router';
import { Component, HostListener } from '@angular/core';
import { filter } from 'rxjs/operators';
import { CartService } from '../services/cart.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrl: './nav-menu.component.css'
})
export class NavMenuComponent {

  searchTerm: string = '';
  isLoggedIn: boolean = false;
  userName: string | null = null;
  userRole: string | null = null;
  menuOpen: boolean = false;
  cartItemCount: number = 0;

  constructor(
    private router: Router,
    private cartService: CartService
  )
  {
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => {
        this.checkLoginStatus();
        this.menuOpen = false;
      });
  }

  ngOnInit() {
    this.checkLoginStatus();
    this.loadCartCount();
    // Listen for cart count changes
    this.cartService.cartCount$.subscribe(count => {
      this.cartItemCount = count;
    });
  }
  loadCartCount(): void {
    if (this.isLoggedIn) {
      this.cartService.getCartCount();
    } else {
      this.cartItemCount = 0;
    }
  }

  checkLoginStatus() {
    const token = localStorage.getItem('token');

    this.isLoggedIn = !!token;

    this.userName = localStorage.getItem('userName');
    this.userRole = localStorage.getItem('role')

    this.loadCartCount();
    
  }

  onSearch() {
    this.router.navigate(['/'], {
      queryParams: {
        search: this.searchTerm
      }
    });
  }

  toggleMenu() {
    this.menuOpen = !this.menuOpen;
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: Event) {

    const target = event.target as HTMLElement;

    if (!target.closest('.user-dropdown')) {
      this.menuOpen = false;
    }
  }

  logout() {
    localStorage.clear();

    this.isLoggedIn = false;
    this.userName = null;
    this.menuOpen = false;
    this.cartService.resetCount();

    this.router.navigate(['/']);
  }
}
