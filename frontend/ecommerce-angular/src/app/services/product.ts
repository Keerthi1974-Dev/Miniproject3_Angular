import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Product } from '../models/product';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private baseUrl = `${environment.apiUrl}/api/products`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Product[]> {
    return this.http.get<Product[]>(this.baseUrl);
  }

  getById(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.baseUrl}/${id}`);
  }

  // For file upload (Cloudinary)
  createWithFile(formData: FormData): Observable<string> {
    return this.http.post(`${this.baseUrl}`, formData, {
      responseType: 'text'
    });
  }

  // For URL only (no file)
  create(formData: FormData): Observable<string> {
    return this.http.post(`${this.baseUrl}`, formData, {
      responseType: 'text'
    });
  }

  update(id: number, formData: FormData): Observable<string> {
    return this.http.put(`${this.baseUrl}/${id}`, formData, {
      responseType: 'text'
    });
  }

  delete(id: number): Observable<string> {
    return this.http.delete(`${this.baseUrl}/${id}`, {
      responseType: 'text'
    });
  }
}