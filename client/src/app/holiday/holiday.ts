import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HolidayGalleryResponse } from '../_models/PagedHolidayResult';
import { HolidayService } from '../_services/holiday-service';
import { NgIf } from '@angular/common';
import {
  GalleryImage,
  GalleryLocation,
  Holiday as HolidayGalleryItem,
} from '../_models/holidayModel';

import { Fullholidayimage } from './fullholidayimage/fullholidayimage';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-holiday',
  standalone: true,
  imports: [NgIf, Fullholidayimage],
  templateUrl: './holiday.html',
  styleUrl: './holiday.css',
})
export class Holiday implements OnInit {
  private readonly holidayService = inject(HolidayService);
  private accountService = inject(AccountService);
  private router = inject(Router);
  selectedImageId: number | null = null;
  locationsList: GalleryLocation[] = [];
  imagesList: GalleryImage[] = [];
  selectedLocation: GalleryLocation | null = null;
  holidays_paginated: HolidayGalleryResponse = {
    page: 1,
    pageSize: 10,
    totalCount: 0,
    totalPages: 0,
    items: [],
  };
  showLocationsFlag: boolean = false;
  showLocationDetailsFlag: boolean = false;
  showMainPageFlag: boolean = true;
  private readonly route = inject(ActivatedRoute);

  ngOnInit(): void {
    this.route.data.subscribe((data) => {
      const result = data['hr'] as HolidayGalleryResponse | null | undefined;
      if (result) {
        this.holidays_paginated = result;
        // sort the holidays by date in descending order
        this.holidays_paginated.items.sort(
          (a, b) =>
            new Date(b.startDate ?? 0).getTime() -
            new Date(a.endDate ?? 0).getTime(),
        );
      }
    });
  }

  getImageFull(imageId: number): void {
    // go to the full image page
    this.router.navigate(['/holidayfullimage/', imageId]);
  }

  closeFullImage(): void {
    this.selectedImageId = null;
  }

  showLocationList(): void {
    this.showLocationsFlag = true;
    this.showLocationDetailsFlag = false;
    this.showMainPageFlag = false;
    this.selectedLocation = null;
    this.imagesList = [];
  }

  showLocationDetails(id: number): void {
    const selectedLocation = this.locationsList.find(
      (location) => location.id === id,
    );
    if (!selectedLocation) {alert('Location not found.');return;
    }

    this.selectedLocation = selectedLocation;
    this.imagesList = selectedLocation.images ?? [];
    // save the list of images in the account service
    this.accountService.HolidayImageArray.set(null);
    this.accountService.HolidayImageArray.set(this.imagesList);
    /* this.accountService.HolidayImageArray.set(
      this.holidays_paginated.items.flatMap((holiday) =>
        holiday.locations.flatMap((location) => location.images),
      ),
    ); */
    this.showLocationsFlag = false;
    this.showLocationDetailsFlag = true;
    this.showMainPageFlag = false;
  }

  showLocations(id: number): void {

    const selectedHoliday = this.holidays_paginated.items.find(
      (h) => h.id === id,
    );
    if (!selectedHoliday) {
      alert('Holiday not found.');
      return;
    }

    this.locationsList = selectedHoliday.locations ?? [];
    if (this.locationsList.length === 0) {
      alert(`No locations found for holiday "${selectedHoliday.title}".`);
    } else if (this.locationsList.length > 1) {
      this.showLocationList();
    } else {
      this.showLocationDetails(this.locationsList[0].id);
    }
  }

  showHolidayList(): void {
    this.locationsList = [];
    this.showMainPageFlag = true;
    this.showLocationsFlag = false;
    this.showLocationDetailsFlag = false;
    this.selectedLocation = null;
    this.imagesList = [];
  }

  getHolidayImageUrl(holiday: HolidayGalleryItem): string | null {
    const fallbackImageId = holiday.locations?.[0]?.images?.[0]?.id;
    return this.getImageUrl(holiday.coverImageUrl, fallbackImageId);
  }

  getLocationImageUrl(location: GalleryLocation): string | null {
    const fallbackImageId = location.images?.[0]?.id;
    return this.getImageUrl(location.coverImageUrl, fallbackImageId);
  }

  getGalleryImageUrl(image: GalleryImage): string | null {
    return this.getImageUrl(image.imageUrl, image.id);
  }

  private getImageUrl(
    imageUrl?: string,
    fallbackImageId?: number,
  ): string | null {
    if (imageUrl?.trim()) {
      const imageId = Number(imageUrl);
      if (Number.isInteger(imageId) && imageId > 0) {
        return this.holidayService.getImageFromServer(imageId);
      }
    }

    if (
      fallbackImageId &&
      Number.isInteger(fallbackImageId) &&
      fallbackImageId > 0
    ) {
      return this.holidayService.getImageFromServer(fallbackImageId);
    }

    return imageUrl?.trim() || null;
  }
}
