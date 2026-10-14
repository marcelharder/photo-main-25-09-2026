import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HolidayGalleryResponse } from '../../../_models/PagedHolidayResult';
import { NgFor, NgIf } from '@angular/common';
import { HolidayService } from '../../../_services/holiday-service';
import { Holiday } from '../../../_models/holidayModel';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-list-of-holidays',
  standalone: true,
  imports: [NgFor, NgIf],
  templateUrl: './list-of-holidays.component.html',
  styleUrl: './list-of-holidays.component.css',
})
export class ListOfHolidaysComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private router = inject(Router);
  private holidayService = inject(HolidayService);
  private listOfHolidays: Holiday[] = [];

  toast = inject(ToastrService);

  holidays_paginated: HolidayGalleryResponse = {
    page: 1,
    pageSize: 10,
    totalCount: 0,
    totalPages: 0,
    items: [],
  };

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
        this.listOfHolidays = this.holidays_paginated.items;
      }
    });
  }

  showAddHoliday() {
    // show the add-image child
  }

  showDeleteHoliday(id: number) {
    this.holidayService.deleteHoliday(id).subscribe((result) => {
      if (result === 1) {
        this.listOfHolidays = this.listOfHolidays.filter(
          (holiday) => holiday.id !== id,
        );
        this.holidays_paginated.items = this.holidays_paginated.items.filter(
          (holiday) => holiday.id !== id,
        );
      } else {
        this.toast.error('Could not be deleted');
      }
    });
  }
}
