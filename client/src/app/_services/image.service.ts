import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { slideModel } from '../_models/slideModel';
import { CarouselModel } from '../_models/CarouselModel';
import { catParams } from '../_models/catParams';
import { PaginatedResult } from '../_models/pagination';
import { imgParams } from '../_models/imgParams';
import { NasImageEntry } from '../_models/nasImageEntry';

@Injectable({
  providedIn: 'root',
})
export class ImageService {
  http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  paginatedResult = signal<PaginatedResult<slideModel[]> | null>(null);
  imageCache = new Map();
  img: imgParams = {
    Category: 0,
    PageNumber: 0,
    PageSize: 0,
  };
  cat: catParams = {
    userId: 0,
    pageNumber: 0,
    pageSize: 0,
  };

  constructor() {}

  getDiasFromCategory(x: number, pageNumber?: number, pageSize?: number) {
    if (pageNumber && pageSize) {
      this.img.Category = x;
      this.img.PageNumber = pageNumber;
      this.img.PageSize = pageSize;
    }

    let params = new HttpParams();

    if (pageNumber && pageSize) {
      params = params.append('Category', x);
      params = params.append('PageNumber', pageNumber);
      params = params.append('PageSize', pageSize);
    }

    const response = this.imageCache.get(Object.values(this.img).join('-'));
    if (response) return this.setPaginatedResponse(response);

    return this.http
      .get<slideModel[]>(this.baseUrl + 'Images/getImagesByCategory', {
        observe: 'response',
        params,
      })
      .subscribe({
        next: (response) => {
          this.setPaginatedResponse(response);
          this.imageCache.set(Object.values(this.img).join('-'), response);
        },
      });
  }

  getDiasForUser(x: number, pageNumber?: number, pageSize?: number) {
    if (pageNumber && pageSize) {
      this.cat.userId = x;
      this.cat.pageNumber = pageNumber;
      this.cat.pageSize = pageSize;
      this.cat.category = 0;
    }

    let params = new HttpParams();

    if (pageNumber && pageSize) {
      params = params.append('id', x);
      params = params.append('PageNumber', pageNumber);
      params = params.append('PageSize', pageSize);
      params = params.append('Category', 0);
    }

    const response = this.imageCache.get(Object.values(this.cat).join('-'));
    if (response) return this.setPaginatedResponse(response);

    return this.http
      .get<slideModel[]>(this.baseUrl + 'Images/getFilesForThisUser', {
        observe: 'response',
        params,
      })
      .subscribe({
        next: (response) => {
          this.setPaginatedResponse(response);
          this.imageCache.set(Object.values(this.img).join('-'), response);
        },
      });
  }

  getCarouselData(id: string) {
    return this.http.get<CarouselModel>(
      this.baseUrl + 'Images/getCarousel/' + id
    );
  }
  getSpecificFileFromId(id: string) {
    return this.http.get<slideModel>(this.baseUrl + 'Images/findImage/' + id);
  }

  browseNasImages(directory: string) {
    const params = new HttpParams().set('directory', directory);
    return this.http.get<NasImageEntry[]>(this.baseUrl + 'Images/browseNasImages', { params });
  }

  getNasImagePreviewUrl(path: string) {
    const params = new HttpParams().set('path', path);
    return this.http.get(this.baseUrl + 'Images/nasImagePreview', {
      params,
      responseType: 'blob',
    });
  }

   createImage(sm: slideModel) {
    return this.http.post<string>(this.baseUrl + 'Images/addImage', sm,{ responseType: 'text' as 'json'});
  }

  uploadSpecificSlideModel(x: slideModel) {
    return this.http.put<any>(this.baseUrl + 'Images/updateImage', x,{ responseType: 'text' as 'json'});
  }

  private setPaginatedResponse(response: HttpResponse<slideModel[]>) {
    this.paginatedResult.set({
      items: response.body as slideModel[],
      pagination: JSON.parse(response.headers.get('Pagination')!),
    });
  }
}
