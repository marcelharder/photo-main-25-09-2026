
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Holiday } from '../holiday/holiday';
import { HolidayGalleryResponse } from '../_models/PagedHolidayResult';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class HolidayService {
  private readonly baseUrl = `${environment.apiUrl}`;

  constructor(private http: HttpClient) {}

  getGallery(page: number, pageSize: number) {
    return this.http.get<HolidayGalleryResponse>(
      `${this.baseUrl}Holiday/getAllHolidays?page=${page}&pageSize=${pageSize}`
    );
  }

  getHolidayDetails(id: number) {
    return this.http.get<Holiday>(`${this.baseUrl}Holiday/${id}`);
  }

  getImageFromServer(id: number) {
    return `${this.baseUrl}Holiday/getImage/${id}`;
  }




}
