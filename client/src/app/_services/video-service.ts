import { inject, Injectable, signal } from '@angular/core';
import { imgParams } from '../_models/imgParams';
import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { VideoModel } from '../_models/VideoModel';
import { PaginatedResult } from '../_models/pagination';
import { environment } from '../../environments/environment';
import { map, of, tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class VideoService {
  getVideoById(id: number) {
    return this.http.get(this.baseUrl + 'Video/getSpecificVideo/' + id, {
      responseType: 'blob',
    });
  }



  http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  videoCache = new Map();
  img: imgParams = {
    Category: 0,
    PageNumber: 0,
    PageSize: 0,
  };
  paginatedResult = signal<PaginatedResult<VideoModel[]> | null>(null);

  getVideosFromCategory(x: number, pageNumber?: number, pageSize?: number) {
    this.img.Category = x;

    if (pageNumber != null && pageSize != null) {
      this.img.PageNumber = pageNumber;
      this.img.PageSize = pageSize;
    }

    let params = new HttpParams().set('Category', x.toString());

    if (pageNumber != null && pageSize != null) {
      params = params
        .set('PageNumber', pageNumber.toString())
        .set('PageSize', pageSize.toString());
    }

    /* const cacheKey = Object.values(this.img).join('-');
    const cachedResponse = this.videoCache.get(cacheKey);

    if (cachedResponse) {
      this.setPaginatedResponse(cachedResponse);
      return of(cachedResponse.body ?? []);
    } */

    return this.http
      .post<VideoModel[]>(
        this.baseUrl + 'Video/getVideosByCategory',
        {},
        {
          params,
          observe: 'response',
        },
      )
      .pipe(
        map((response: HttpResponse<VideoModel[]>) => {
          //this.videoCache.set(cacheKey, response);
          this.setPaginatedResponse(response);
          return response.body ?? [];
        }),
      );
  }

  getVideosForCurrentUser(x: number, pageNumber?: number, pageSize?: number) {
    if (pageNumber != null && pageSize != null) {
      this.img.PageNumber = pageNumber;
      this.img.PageSize = pageSize;
    }

    let params = new HttpParams().set('Category', x.toString());

    if (pageNumber != null && pageSize != null) {
      params = params
        .set('PageNumber', pageNumber.toString())
        .set('PageSize', pageSize.toString());
    }
    /* 
        const cacheKey = Object.values(this.img).join('-');
        const cachedResponse = this.videoCache.get(cacheKey);
    
        if (cachedResponse) {
          this.setPaginatedResponse(cachedResponse);
          return of(cachedResponse.body ?? []);
        } */

    return this.http
      .post<VideoModel[]>(
        this.baseUrl + 'Video/getVideosForUser',
        {},
        {
          params,
          observe: 'response',
        },
      )
      .pipe(
        map((response: HttpResponse<VideoModel[]>) => {
          //this.videoCache.set(cacheKey, response);
          this.setPaginatedResponse(response);
          return response.body ?? [];
        }),
      );
  }

  addVideo(video: VideoModel) {
    return this.http
      .post(this.baseUrl + 'Video/addVideo', video)
      .pipe(tap(() => this.videoCache.clear()));
  }

  updateVideo(video: VideoModel) {
    return this.http.put(this.baseUrl + 'Video/updateVideo', video);
  }

  videoByCategory(categoryId: number) {
    return this.http.get(this.baseUrl + 'Video/VideosByCategory/' + categoryId, { responseType: 'json' });
  }

  videoDetails(id: number) {
    return this.http
    .get(this.baseUrl + 'Video/getSpecificVideoDetails/' + id, { responseType: 'json' });
  }




  deleteVideo(id: string) {
    return this.http.delete(this.baseUrl + 'Video/deleteVideo/' + id);
  }

  private setPaginatedResponse(response: HttpResponse<VideoModel[]>) {
    this.paginatedResult.set({
      items: response.body ?? [],
      pagination: response.headers.get('Pagination')
        ? JSON.parse(response.headers.get('Pagination')!)
        : null,
    });
  }
}