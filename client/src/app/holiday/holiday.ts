import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HolidayGalleryResponse } from '../_models/PagedHolidayResult';
import { HolidayService } from '../_services/holiday-service';
import { environment } from '../../environments/environment.development';
import { NgIf } from '@angular/common';
import { GalleryImage, GalleryLocation } from '../_models/holidayModel';

@Component({
  selector: 'app-holiday',
  standalone: true,
  imports: [NgIf],
  templateUrl: './holiday.html',
  styleUrl: './holiday.css',
})
export class Holiday implements OnInit {
  baseUrl = environment.apiUrl;
  holidayservice = inject(HolidayService);
  locationsList: GalleryLocation[] = [];
  imagesList: GalleryImage[] = [];
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
  selectedHolidayId: number | null = null;

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
            new Date(a.startDate ?? 0).getTime(),
        );
      }
    });
  }

  showLocationList(): void {
    this.showLocationsFlag = true;
    this.showLocationDetailsFlag = false;
    this.showMainPageFlag = false;
  }

  showLocationDetails(hno: number, id: number): void {
    var selectedHoliday = this.holidays_paginated.items.find(
      (h) => h.id === hno,
    );
    if (selectedHoliday) {
      this.locationsList = selectedHoliday.locations;
    } else {
      alert('Holiday not found.');
    }
    var selectedLocation = this.locationsList.find((l) => l.id === id);
    if (selectedLocation) {
      // do something with the selected location, e.g., display its details
      this.imagesList = selectedLocation.images;
      console.log('Selected Location:', selectedLocation);
    } else {
      alert('Location not found.');
    }
    this.showLocationsFlag = false;
    this.showLocationDetailsFlag = true;
    this.showMainPageFlag = false;
  }

  showLocations(id: number): void {
    var selectedHoliday = this.holidays_paginated.items.find(
      (h) => h.id === id,
    );
    if (selectedHoliday) {
      const locations = selectedHoliday.locations;
      if (locations && locations.length > 0) {
        const locationNames = locations
          .map((location) => location.name)
          .join(', ');
      } else {
        alert(`No locations found for holiday "${selectedHoliday.title}".`);
      }
    } else {
      alert('Holiday not found.');
    }
    // if there are more than one location, show them all in a list, otherwise show the single location
    if (selectedHoliday && selectedHoliday.locations.length > 1) {
      const locationList = selectedHoliday.locations
        .map((location) => location.name)
        .join('\n');

      // and then open the dedicated locations section, where the user can select a location to see the pictures for that location
      this.showLocationList();
    } else if (selectedHoliday && selectedHoliday.locations.length === 1) {
      // open the location details section which will show a paged list of pictures for that location
      this.showLocationDetails(
        selectedHoliday.id,
        selectedHoliday.locations[0].id,
      );
    } else {
      alert(`No locations found for holiday "${selectedHoliday?.title}".`);
    }
  }

  getImageFromServer(id: number): string {
    // the id here is the id of the holiday
    // find the image for the holiday with the given id
    var selectedHoliday = this.holidays_paginated.items.find(
      (h) => h.id === id,
    );
    if (selectedHoliday && selectedHoliday.coverImageUrl) {
      var pictureId = +selectedHoliday.coverImageUrl;
      return this.baseUrl + 'Holiday/getImage/' + pictureId;
    } else {
      id = 412;
      return this.baseUrl + 'Holiday/getImage/' + id;
    }
  }
}
