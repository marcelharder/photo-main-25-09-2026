import {
  HttpClient,
  HttpClientModule,
  HttpParams,
  HttpResponse,
} from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { categoryModel } from '../_models/categoryModel';
import { PaginatedResult } from '../_models/pagination';
import { catParams } from '../_models/catParams';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root',
})
export class CategoryService {
  private http = inject(HttpClient);
  private acc = inject(AccountService);
  baseUrl = environment.apiUrl;
  paginatedResult = signal<PaginatedResult<categoryModel[]> | null>(null);
  cat: catParams = {
    userId: 0,
    pageNumber: 0,
    pageSize: 0,
  };
  categoryCache = new Map();

  getAllowedCategories(pageNumber?: number, pageSize?: number) {
    if (pageNumber && pageSize) {
      this.cat.userId = this.acc.currentUser()!.Id;
      this.cat.pageNumber = pageNumber;
      this.cat.pageSize = pageSize;
    }

    const response = this.categoryCache.get(Object.values(this.cat).join('-'));
    if (response) return this.setPaginatedResponse(response);

    let params = new HttpParams();

    if (pageNumber && pageSize) {
      params = params.append('Id', this.acc.currentUser()!.Id);
      params = params.append('PageNumber', pageNumber);
      params = params.append('PageSize', pageSize);
    }

    return this.http
      .get<
        categoryModel[]
      >(this.baseUrl + 'Category/getAllowedCategories', { observe: 'response', params })
      .subscribe({
        next: (response) => {
          this.setPaginatedResponse(response);
          this.categoryCache.set(Object.values(this.cat).join('-'), response);
        },
      });
  }

  private setPaginatedResponse(response: HttpResponse<categoryModel[]>) {
    var l = response.body;

    if (l) {
      l = l.sort((a, b) => {
        return (a.YearTaken ?? 0) - (b.YearTaken ?? 0);
      });
    }
    this.paginatedResult.set({
      items: l as categoryModel[],
      pagination: JSON.parse(response.headers.get('Pagination')!),
    });
  }

  getAllCategories() {
    return this.http.get<categoryModel[]>(
      this.baseUrl + 'Category/getAllCategoriesForAdmin',
    );
  }

  updateCategory(category: categoryModel) {
    return this.http.put(this.baseUrl + 'Category/updateCategory', category, {
      responseType: 'text' as 'json',
    });
  }
  createCategory(category: categoryModel) {
    return this.http.post(this.baseUrl + 'Category/createCategory', category, {
      responseType: 'text' as 'json',
    });
  }
  getCategoryById(id: number) {
    return this.http.get<categoryModel>(
      this.baseUrl + 'Category/getSpecificCategory/' + id,
    );
  }

  getDescription(category: any) {
    return this.http.get<string>(
      this.baseUrl + 'Category/getDescription/' + category,
    );
  }
}
