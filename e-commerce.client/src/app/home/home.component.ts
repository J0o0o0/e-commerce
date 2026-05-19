import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

interface Product {
  id: number
  sellerName: null
  rating : number
  reviewCount: number
  name: string
  description: string
  price: number
  stock: number
  categoryId: number
  categoryName: string
  images: string[]
}
interface ApiResponse {
  data: Product[]
  total: number
}
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
  searchTerm: string = '';
  products: Product[] = [];
  page: number = 1;
  pageSize: number = 20;
  hasMore: boolean = true;

  categories: any[] = [];

  selectedCategoryId: number | null = null;

  minPrice: number = 0;
  maxPrice: number = 1000;

  constructor(private route: ActivatedRoute, private http: HttpClient) { }

  ngOnInit() {

    this.http.get<any[]>('/api/Category').subscribe(res => {
      this.categories = res;
    });


    this.route.queryParams.subscribe((params: any) => {
      this.searchTerm = params['search'] || '';
      this.resetAndLoad();
    });
  }

  loadProducts() {
    this.http.get<ApiResponse>('/api/product', {
      params: {
        search: this.searchTerm ?? '',
        page: this.page,
        pageSize: this.pageSize,
        categoryId: this.selectedCategoryId?.toString() ?? '',
        minPrice: this.minPrice,
        maxPrice: this.maxPrice
      }
    }).subscribe(res => {
      this.products = [...this.products, ...res.data];
      if (this.products.length >= res.total) {
        this.hasMore = false;
      }
    });
  }
  loadMore() {
    this.page++;
    this.loadProducts();
  }
  
  resetAndLoad() {
    this.page = 1;
    this.products = [];
    this.hasMore = true;

    this.loadProducts();
  }
  applyFilters() {
    this.page = 1;
    this.products = [];
    this.resetAndLoad();
  }
  onPriceChange() {
    if (this.minPrice < 0) this.minPrice = 0;
    if (this.maxPrice > 1000) this.maxPrice = 1000;

    if (this.minPrice > this.maxPrice) {
      [this.minPrice, this.maxPrice] = [this.maxPrice, this.minPrice];
    }

    this.applyFilters();
  }
  onCategorySelect(categoryId: number) {
    this.selectedCategoryId = categoryId;
    this.applyFilters();
  }
  clearCategory() {
    this.selectedCategoryId = null;
    this.minPrice = 0;
    this.maxPrice = 1000;
    this.searchTerm = '';
    this.applyFilters();
  }

}
